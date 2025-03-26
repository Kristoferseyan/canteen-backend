namespace CanteenWebApiLibrary.Dto
{
    public class UserPaymentsDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
        public decimal? Amount { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        

    }
}
