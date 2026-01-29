using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using pract14mobile.DTOs;

namespace pract14mobile
{
    public static class Data
    {
        public static ProductDTO Product { get; set; }
        public static StockItemInfoDTO StockItem { get; set; }
        public static void Clear()
        {
            Product = null;
            StockItem = null;
        }
    }
}
