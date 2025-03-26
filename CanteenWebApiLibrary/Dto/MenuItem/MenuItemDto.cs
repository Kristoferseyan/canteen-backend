using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Dto
{
    public class MenuItemDto
    {
        public Guid? Id { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal? price { get; set; }
        public Guid CategoryId { get; set; }
        public int Stock { get; set; }
        public DateTime? FeaturedStartTime { get; set; }
        public DateTime? FeaturedEndTime { get; set; }
    }
}
