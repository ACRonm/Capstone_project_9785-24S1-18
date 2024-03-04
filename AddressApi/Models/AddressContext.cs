using Microsoft.EntityFrameworkCore;

namespace AddressApi.Models
{
    public class AddressContext : DbContext
    {
        public AddressContext(DbContextOptions<AddressContext> options)
            : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<InputAddress> InputAddresses { get; set; }
    }
}
