using addCommodity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace addCommodity.Service
{
    class AppService
    {
        Context db;
        public AppService()
        {
            db = new Context();
        }
        public AppSetting get()
        {
            return db.AppSetting.First();
        }

        public void save(string date)
        {
            var apps = this.get();
            apps.Date = date;
            db.SaveChanges();
        }
    }
}
