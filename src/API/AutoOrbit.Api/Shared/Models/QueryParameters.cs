namespace AutoOrbit.Api.Shared.Models;

public abstract class QueryParameters
{
    const int maxPageSize = 50;
    private readonly int _pageSize = 10;
    public int PageNumber { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _ = value > maxPageSize ? maxPageSize : value;
    }
    public string? OrderBy { get; set; }
}

