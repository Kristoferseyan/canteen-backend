
namespace CanteenWebApiLibrary.Dto
{
        public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal? TlAmnt { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }

}

