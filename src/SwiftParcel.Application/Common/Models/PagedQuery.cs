namespace SwiftParcel.Application.Common.Models;

public abstract record PagedQuery
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 20;

    private readonly int _pageNumber = 1;
    private readonly int _pageSize = DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }
}

