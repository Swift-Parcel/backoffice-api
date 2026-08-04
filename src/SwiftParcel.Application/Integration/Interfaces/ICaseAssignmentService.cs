using System.Threading;
using System.Threading.Tasks;

public interface ICaseAssignmentService
{
    Task AssignCaseAsync(string caseNumber, int handlerId, CancellationToken cancellationToken = default);
}