namespace SwiftParcel.Infrastructure.Persistence.Seeding.Seeders;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Helpers;
using Interfaces;

public class CaseNoteSeeder : IEntitySeeder
{
    public int Order => 12;

    private static readonly HashSet<string> TruthyValues = new(StringComparer.OrdinalIgnoreCase)
    {
        "yes", "true", "1", "internal"
    };

    public async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.CaseNotes.AnyAsync(cancellationToken))
            return;

        // Cache users by email for fast lookup
        var usersByEmail = await dbContext.Users
            .ToDictionaryAsync(u => u.Email, u => u, StringComparer.OrdinalIgnoreCase, cancellationToken);

        // Cache cases by Id for fast lookup
        var casesById = await dbContext.Cases
            .ToDictionaryAsync(c => c.Id, cancellationToken);

        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = @"SELECT case_id, author, author_email, note_text, created_date, is_internal, attachment FROM case_notes";

        await dbContext.Database.OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var notesToInsert = new List<CaseNote>();

        while (await reader.ReadAsync(cancellationToken))
        {
            var caseIdRaw = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            var authorEmail = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var noteText = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            var createdAtRaw = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
            var isInternalRaw = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            var attachment = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);

            // Safe string-to-int conversion for case_id
            if (!int.TryParse(caseIdRaw, out var caseId) || !casesById.TryGetValue(caseId, out var parentCase))
                continue;

            // Resolve User entity by author email
            if (!usersByEmail.TryGetValue(authorEmail, out var author))
                continue;

            // Parse IsInternal flag
            var isInternal = TruthyValues.Contains(isInternalRaw.Trim());

            // Parse created timestamp using helper
            var createdDate = TimestampParserHelper.ParseOrFallback(createdAtRaw);

            notesToInsert.Add(new CaseNote
            {
                CaseId = parentCase.Id,
                AuthorId = author.Id,
                NoteText = noteText,
                CreatedDate = createdDate,
                IsInternal = isInternal,
                Attachment = attachment
            });
        }

        if (notesToInsert.Count > 0)
        {
            await dbContext.CaseNotes.AddRangeAsync(notesToInsert, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}