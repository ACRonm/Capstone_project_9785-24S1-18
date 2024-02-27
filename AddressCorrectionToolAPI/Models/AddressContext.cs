using Microsoft.EntityFrameworkCore;


namespace AddressCorrectionToolAPI.Models
{
    public class AddressContext : DbContext
    {
        public AddressContext(DbContextOptions<AddressContext> options)
            : base(options)
        {
        }
        public DbSet<Address> Address { get; set; }
    }
}
