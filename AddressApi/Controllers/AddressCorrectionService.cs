using AddressApi.Models;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;

namespace AddressApi.Controllers
{
    public class AddressCorrectionService
    {
        // _context is the database context
        private readonly AddressContext _context;

        public AddressCorrectionService(AddressContext context)
        {
            _context = context;
        }

        public async Task<InputAddress> CorrectAddressAsync(InputAddress inputAddress)
        {
            AddressCorrectionService addressCorrectionService = new AddressCorrectionService(_context);

            Console.WriteLine("Correcting address...", inputAddress.Street);

            if(inputAddress.Street == null)
            {
                Console.WriteLine("No street name provided.");
                return null;
            }

            string inputStreetName = inputAddress.Street;

            var knownStreetNames = await _context.Addresses.Select(a => a.Street).ToListAsync();

            // find the highest match
            var match = Process.ExtractOne(inputStreetName.ToUpper(), knownStreetNames); 
            
            Console.WriteLine ("Match: ", match.Value, match.Score);

            var correctedAddress = new InputAddress
            {
                Number = inputAddress.Number,
                Street = match.Value,
                Unit = inputAddress.Unit,
                City = inputAddress.City,
                Postcode = inputAddress.Postcode,
                Region = inputAddress.Region
            };
            return await Task.FromResult(correctedAddress);
        }
    }
}
