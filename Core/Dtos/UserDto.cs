using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public int? ShopId { get; set; }
        public int? BrandId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Password { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime CreateAt { get; set; }
        public int CreateBy { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UpdateBy { get; set; }

        // Form binding
        public int? RoleId { get; set; }

        // Display properties
        public string? ShopName { get; set; }
        public string? BrandName { get; set; }
        public string? RoleName { get; set; }
    }
}

