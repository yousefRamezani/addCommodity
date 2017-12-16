using addCommodity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace addCommodity.Service
{
    static class CommodityService
    {
        public static Commodity get(long barcode)
        {
            using (var db = new Context())
            {

                return db.Commodities.FirstOrDefault(c => c.BarCode == barcode && c.MarketType == 1);
                
            }
        }
        public static object getall()
        {
            using (var db = new Context())
            {
                if (db.Commodities.Count(c => c.State > 0) == 0)
                {
                    return null;
                }
                return db.Commodities.Where(c => c.State > 0).OrderBy(c=> c.date)
                    .Select(c=> new {c.BarCode,c.Name,c.Brand,c.Unit,c.Multiplier,c.Price })
                    .ToList();

            }
        }
        public static bool add(Commodity commodity)
        {
            using (var db = new Context())
            {
                commodity.State = 1;
                commodity.MarketType = 1;
                commodity.date = DateTime.Now;
                db.Commodities.Add(commodity);
                db.SaveChanges();
                return true;
            }
        }
        public static bool update(Commodity newCommodity)
        {
            using (var db = new Context())
            {
                var oldCommodity = db.Commodities.Find(newCommodity.ID);
                oldCommodity.Name = newCommodity.Name;
                oldCommodity.Brand = newCommodity.Brand;
                oldCommodity.Unit = newCommodity.Unit;
                oldCommodity.Multiplier = newCommodity.Multiplier;
                oldCommodity.Price = newCommodity.Price;
                oldCommodity.State = newCommodity.State;
                oldCommodity.date = DateTime.Now;
                db.SaveChanges();
                return true;
            }
        }
    }
}
