using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserSystem.API.Models.Entities.Users;
using UserSystem.API.Models.Helpers;

namespace UserSystem.API.Data
{
    public class UserSystemDBContext : DbContext
    {
        private readonly AppSettings _config;
        public UserSystemDBContext(IOptions<AppSettings> config)
        {
            _config = config.Value;
        }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasQueryFilter(x => !x.DeletedDate.HasValue);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // connect to sql server database
            optionsBuilder.UseSqlServer(_config.ConnectionStrings.UserSystemDBContext);
        }
    }
}