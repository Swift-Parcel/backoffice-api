namespace SwiftParcel.Application.Common.Models;

public record PagedQuery
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    public int PageNumber { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init
        {
            if (value > MaxPageSize)
            {
                _pageSize = MaxPageSize;
            }

            if (value < 0)
            {
                _pageSize = 20;
            }
            _pageSize = value;
        }
    }
}

