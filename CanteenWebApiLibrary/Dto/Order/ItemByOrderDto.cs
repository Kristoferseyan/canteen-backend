namespace CanteenWebApiLibrary.Dto
{
    public class ItemByOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public String? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal? Price { get; set; }
    }

}