using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Enviroment {
    public class Context : DbContext {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Domain.EnviromentParameter> EnviromentParameters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Domain.EnviromentParameter>().ToTable("EnviromentParameter");
        }
    }
}
