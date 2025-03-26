using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanteenWebApiLibrary.Dto
{
    public class UserRoleDto
    {
        public Guid? id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }

    }
}
