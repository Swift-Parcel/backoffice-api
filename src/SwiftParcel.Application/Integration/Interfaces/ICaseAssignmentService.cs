using System.Threading;
using System.Threading.Tasks;

public interface ICaseAssignmentService
{
    Task AssignCaseAsync(int caseId, int handlerId, CancellationToken cancellationToken = default);
}