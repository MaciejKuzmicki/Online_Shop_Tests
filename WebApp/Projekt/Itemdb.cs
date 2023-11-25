using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;

namespace Projekt
{

    public class Itemdb : DbContext
    {
        public Itemdb(DbContextOptions<Itemdb> options) : base(options) { }

       
        public DbSet<Item> Itemos { get; set; }

    }
}
