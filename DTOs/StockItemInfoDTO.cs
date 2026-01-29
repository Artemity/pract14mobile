using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pract14mobile.DTOs
{
    public class StockItemInfoDTO
    {
        public int Id { get; set; }
        public string WarehouseAddress { get; set; }
        public string ProductName { get; set; }
        public string ProductCategory { get; set; }
        public string Manufacturer { get; set; }
        public int Quantity { get; set; }
    }
}
