using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace addCommodity.Model
{
    class Commodity
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Unit { get; set; }
        public short Multiplier { get; set; }
        public long BarCode { get; set; }
        public string Picture { get; set; }
        public short MarketType { get; set; }
        public int? CategoryID { get; set; }
        public int Price { get; set; }
        public int State { get; set; }
        public DateTime date { get; set; }
    }
}
