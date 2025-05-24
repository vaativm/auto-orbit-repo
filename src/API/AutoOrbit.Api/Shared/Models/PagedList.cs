namespace AutoOrbit.Api.Shared.Models;

public class PagedList<T> : List<T>
{
    public PagingMetaData PagingMetaData { get; set; }

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PagingMetaData = new PagingMetaData
        {
            TotalCount = count,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        AddRange(items);
    }
}
