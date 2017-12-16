using addCommodity.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace addCommodity
{
    class Context : DbContext
    {
        public DbSet<Commodity> Commodities { get; set; }
        public DbSet<AppSetting> AppSetting { get; set; }
    }
}
