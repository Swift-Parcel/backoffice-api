using System.Runtime.InteropServices.JavaScript;
using SwiftParcel.Domain.Exceptions;

namespace SwiftParcel.Domain.Entities;

public class Handler
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Department { get; private set; } = string.Empty; //could be an enum
    public DateTime HireDate { get; private set; }
    public int MaxCases { get; private set; }
    public bool IsActive { get; set; } = true;

    private readonly List<Case> _cases = new();
    public IReadOnlyCollection<Case> Cases => _cases.AsReadOnly();

    public Handler()
    { }
    
    public Handler(int userId, string department, DateTime hireDate, int maxCases, bool isActive)
    {
        UserId = userId;
        Department = department;
        HireDate = hireDate;
        MaxCases = maxCases;
        IsActive = isActive;
    }
    
    public Handler(int id,int userId, string department, DateTime hireDate, int maxCases,  bool isActive)
    {
        Id = id;
        UserId = userId;
        Department = department;
        HireDate = hireDate;
        MaxCases = maxCases;
        IsActive = isActive;
    }
    
    public void UpdateHandler(int userId, 
        string department, 
        DateTime hireDate, 
        int maxCases,
        bool isActive)
    {
        UserId = userId; //todo: validate this
        ChangeDepartment(department);
        HireDate = hireDate; //todo: validate this
        ChangeCapacity(maxCases);
        if(isActive)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }
    public void ChangeDepartment(string newDepartment)
    {
        if (string.IsNullOrWhiteSpace(newDepartment))
            throw new ArgumentException("Department cannot be empty."
                , nameof(newDepartment));

        Department = newDepartment;
    }

    public void ChangeCapacity(int newMaxCases)
    {
        if (newMaxCases < _cases.Count)
            throw new InvalidOperationException(
                "Capacity cannot be lower than current active cases of a handler.");

        MaxCases = newMaxCases;
    }
    
    public void Activate() => IsActive = true;

    public void Deactivate()
    {
        if (_cases.Count > 0)
            throw new InvalidOperationException(
                "Handler with active cases cannot be deactivated.");

        IsActive = false;
    }
    
    public bool CanAssignCase() => _cases.Count < MaxCases;
    
    public void AssignCase(Case @case)
    {
        if (!CanAssignCase())
            throw new HandlerCapacityExceededException();

        _cases.Add(@case);
    }
    
    
}