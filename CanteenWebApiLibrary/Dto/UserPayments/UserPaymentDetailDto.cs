public class UserPaymentDetailsDto
{
    public string Name { get; set; }
    public string OrderCode { get; set; }
    public decimal? Amount { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
}
