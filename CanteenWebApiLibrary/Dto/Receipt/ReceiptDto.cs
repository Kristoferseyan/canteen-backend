namespace CanteenWebApiLibrary.Dto
{
    public class ReceiptDto
    {
        public Guid Id { get; set; }
        public string receiptNumber { get; set; }
        public Guid orderId { get; set; }
        public Guid paymentId { get; set; }
        public DateTime issuedDate { get; set; }
        public decimal totalAmount { get; set; }
        public bool voided { get; set; }
        public string? voidReason { get; set; }
    }

}