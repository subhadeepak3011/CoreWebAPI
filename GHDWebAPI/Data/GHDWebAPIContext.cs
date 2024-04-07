using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GHDWebAPI.Model;

namespace GHDWebAPI.Data
{
    public class GHDWebAPIContext : DbContext
    {
        public GHDWebAPIContext (DbContextOptions<GHDWebAPIContext> options)
            : base(options)
        {
        }

        public DbSet<GHDWebAPI.Model.Product> Product { get; set; } = default!;
    }
}
