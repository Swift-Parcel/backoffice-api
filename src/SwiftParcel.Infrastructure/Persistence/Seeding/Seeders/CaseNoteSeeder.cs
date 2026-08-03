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

        var casesByNumber = await dbContext.Cases
            .ToDictionaryAsync(c => c.CaseNumber, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var customersByEmail = await SeedingLookupHelper.GetCustomerLookupByEmailAsync(dbContext, cancellationToken);
        var customersByName = await SeedingLookupHelper.GetCustomerLookupByNameAsync(dbContext, cancellationToken);
        
        var usersByEmail = await SeedingLookupHelper.GetUserLookupByEmailAsync(dbContext, cancellationToken);
        var handlersByName = await SeedingLookupHelper.GetHandlerLookupByNameAsync(dbContext, cancellationToken);
        
        var handlersByUserId = await dbContext.Handlers
            .AsNoTracking()
            .ToDictionaryAsync(h => h.UserId, h => h.Id, cancellationToken);

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

            if (caseId == 0) continue;

            int? customerId = null;
            int? handlerId = null;
            
            var email = oldNote.author_email?.Trim();
            var authorName = oldNote.author?.Trim();

            if (!string.IsNullOrWhiteSpace(email))
            {
                if (customersByEmail.TryGetValue(email, out var parsedCustId))
                {
                    customerId = parsedCustId;
                }
                else if (usersByEmail.TryGetValue(email, out var parsedUserId) && 
                         handlersByUserId.TryGetValue(parsedUserId, out var parsedHandlerId))
                {
                    handlerId = parsedHandlerId;
                }
            }

            if (customerId == null && handlerId == null && !string.IsNullOrWhiteSpace(authorName))
            {
                if (handlersByName.TryGetValue(authorName, out var parsedHandlerIdFromName))
                {
                    handlerId = parsedHandlerIdFromName;
                }
                else if (customersByName.TryGetValue(authorName, out var parsedCustIdFromName))
                {
                    customerId = parsedCustIdFromName;
                }
            }

            if (customerId == null && handlerId == null)
            {
                continue; 
            }

            var newNote = new CaseNote
            {
                Id = StringParserHelper.ExtractInteger(oldNote.id),
                CaseId = caseId,
                HandlerId = handlerId,
                CustomerId = customerId,
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