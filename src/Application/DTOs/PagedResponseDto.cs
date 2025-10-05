namespace ProductAPI.Application.DTOs;

/// <summary>
/// Paginated response DTO
/// </summary>
/// <typeparam name="T">Type of items in the response</typeparam>
public class PagedResponseDto<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
