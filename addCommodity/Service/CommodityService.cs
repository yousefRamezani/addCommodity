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
        public static Commodity get(long barcode,string name)
        {
            using (var db = new Context())
            {

                return db.Commodities.FirstOrDefault(c => c.Name == name &&
                c.BarCode == barcode && c.MarketType == 1);

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
                var list = db.Commodities.Where(c => c.State > 0).OrderBy(c=> c.date)
                    .Select(c=> new {c.ID,c.BarCode,c.Name,c.Brand,c.date,c.Price})
                    .ToList();
                var listCommodities = new List<Commodity>();
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        listCommodities.Add(new Commodity
                        {
                            ID=item.ID,
                            BarCode = item.BarCode,
                            Name = item.Name,
                            Brand = item.Brand,
                            PriceFormat = item.Price.ToString("N0"),
                            dateString = item.date == null?"---": new PersianDateTime((DateTime)item.date).ToString("HH:mm - yy/MM/dd")
                        });
                    } 
                }
                return listCommodities;

            }
        }
        public static bool add(Commodity commodity)
        {
            using (var db = new Context())
            {
                commodity.State = 1;
                commodity.MarketType = 1;
                commodity.date = DateTime.Now;
                commodity.Unit = "عدد";
                commodity.Multiplier = 1;
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
                oldCommodity.BarCode = newCommodity.BarCode;
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

        public static bool removeCommodity(long barcode,string name)
        {
            using (var db = new Context())
            {
                var commodity = db.Commodities.FirstOrDefault(c =>c.Name == name && c.BarCode == barcode
                 && c.MarketType == 1);
                if (commodity != null)
                {
                    commodity.State = 0;
                    commodity.date = DateTime.Now;
                    db.SaveChanges();
                }

                return true;
            }
        }
        

    }
}
