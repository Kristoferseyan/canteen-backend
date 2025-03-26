using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Dto
{
    public class CategoryDto
    {
        public Guid? Id { get; set; }
        public string CategoryName { get; set; }
        public Guid? ParentCategoryId { get; set; } 
    }
}
