public class OrderDto
{
    public Guid? Id { get; set; }
    public decimal? TlAmnt { get; set; }
    public DateTime OrderDate { get; set; }
    public Guid UserId { get; set; }
    public string? Status { get; set; }
    public string? PaymentMethod { get; set; }
    public string ShortId => Id.HasValue ? Id.Value.ToString().Substring(0, 6) : "N/A";
}
