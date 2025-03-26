public class PaginatedOrdersDto
{
    public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
}
