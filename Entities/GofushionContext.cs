using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mini.Entities
{
    public class GofushionContext : DbContext
    {        
        public GofushionContext(DbContextOptions<GofushionContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product>();
            base.OnModelCreating(builder);
        }
        public DbSet<Product> Products { get; set; }
    }
}
