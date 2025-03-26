namespace CanteenWebApiLibrary.Dto
{
    public class OrderItemDto
    {
        public Guid? Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }

}