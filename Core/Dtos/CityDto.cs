using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}

