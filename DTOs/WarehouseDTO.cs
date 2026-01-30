using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pract14mobile.DTOs
{
    public class WarehouseDTO
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public int? Phone { get; set; }
        public string ManagerLastName { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Address) &&
                   Phone.HasValue &&
                   Phone > 0 &&
                   !string.IsNullOrWhiteSpace(ManagerLastName);
        }
    }
}
