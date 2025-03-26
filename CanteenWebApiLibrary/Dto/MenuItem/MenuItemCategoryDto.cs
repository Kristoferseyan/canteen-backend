using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Dto
{
    public class MenuItemCategoryDto
    {
        public Guid? Id { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal? price { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public int Stock { get; set; }
    }
}
