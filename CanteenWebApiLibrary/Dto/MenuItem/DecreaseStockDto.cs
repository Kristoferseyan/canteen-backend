using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Dto
{
    public class DecreaseStockDto
    {
        public Guid Id { get; set; }
        public int Stock { get; set; }
    }
}
