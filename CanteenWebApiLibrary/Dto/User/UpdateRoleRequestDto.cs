using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Dto
{
    public class UpdateRoleRequestDto
    {
        public Guid UserId { get; set; }
        public string NewRoleId { get; set; }
    }
}

