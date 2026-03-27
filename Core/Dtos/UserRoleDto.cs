using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UserRoleDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
