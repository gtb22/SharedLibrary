namespace Shared.Results;

public class PagedResult<T>
{
    public bool Success { get; set; } = true;
    public List<T> Data { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
    public int Total { get; set; }

    public bool HasNextPage => Page * Size < Total;
    public bool HasPreviousPage => Page > 1;

    public PagedResult(List<T> data, int page, int size, int total)
    {
        Data = data;
        Page = page;
        Size = size;
        Total = total;
    }
}
