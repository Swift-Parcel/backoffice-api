using Microsoft.EntityFrameworkCore;
using SwiftParcel.Domain.Entities;
using SwiftParcel.Infrastructure.Persistence.Seeding.Helpers;
using SwiftParcel.Infrastructure.Persistence.Seeding.Interfaces;

namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

public class CaseNoteSeeder : IEntitySeeder
{
    public int Order => 130;

    public async Task SeedAsync(LegacyDbContext oldDbContext, AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.CaseNotes.AnyAsync(cancellationToken))
        {
            return;
        }

        // Caches for fast in-memory lookups
        var casesByNumber = await dbContext.Cases
            .ToDictionaryAsync(c => c.CaseNumber, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var usersByEmail = await dbContext.Users
            .ToDictionaryAsync(u => u.Email, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var legacyNotes = await oldDbContext.Database
            .SqlQueryRaw<LegacyCaseNoteDto>(@"SELECT id, case_id, case_number, author, author_email, note_text, created_date, is_internal, attachment 
                FROM case_notes")
            .ToListAsync(cancellationToken);

        var newCaseNotes = new List<CaseNote>();

        foreach (var oldNote in legacyNotes)
        {
            int caseId = 0;

            if (!string.IsNullOrWhiteSpace(oldNote.case_number) && 
                casesByNumber.TryGetValue(oldNote.case_number.Trim(), out var parsedCaseId))
            {
                caseId = parsedCaseId;
            }
            else if (!string.IsNullOrWhiteSpace(oldNote.case_id))
            {
                var rawId = StringParserHelper.ExtractInteger(oldNote.case_id);
                if (casesByNumber.ContainsValue(rawId))
                {
                    caseId = rawId;
                }
            }

            if (caseId == 0)
            {
                continue;
            }

            int authorId = 0;
            if (!string.IsNullOrWhiteSpace(oldNote.author_email) && 
                usersByEmail.TryGetValue(oldNote.author_email.Trim(), out var parsedAuthorId))
            {
                authorId = parsedAuthorId;
            }

            var newNote = new CaseNote
            {
                Id = StringParserHelper.ExtractInteger(oldNote.id),
                CaseId = caseId, // Most már garantáltan létező Case.Id
                AuthorId = authorId,
                NoteText = oldNote.note_text ?? string.Empty,
                CreatedDate = TimestampParserHelper.ParseOrFallback(oldNote.created_date),
                IsInternal = StringParserHelper.ParseBoolean(oldNote.is_internal),
                Attachment = oldNote.attachment ?? string.Empty
            };

            newCaseNotes.Add(newNote);
        }

        await dbContext.CaseNotes.AddRangeAsync(newCaseNotes, cancellationToken);
    }

    private record LegacyCaseNoteDto(
        string id,
        string case_id,
        string case_number,
        string author,
        string author_email,
        string note_text,
        string created_date,
        string is_internal,
        string? attachment);
}