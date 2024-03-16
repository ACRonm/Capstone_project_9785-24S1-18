using AddressApi.Models;
using CsvHelper;
using System.Globalization;
using System.Diagnostics;

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

        public bool checkPostcode(int inputPostcode, int postcode)
        {
            // check if postcode is null
            if (postcode == 0)
                return false;
            return Math.Abs(inputPostcode - postcode) <= 10;
        }

        public async Task<InputAddress> CorrectAddressAsync(InputAddress inputAddress, List<Address> addresses)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            AddressCorrectionService addressCorrectionService = new AddressCorrectionService(_context);
            Console.WriteLine("Correcting address...", inputAddress.Street);

            if (inputAddress.Street == null)
            {
                Console.WriteLine("No street name provided.");
                return null;
            }

            string inputStreetName = inputAddress.Street.ToUpper();
            string inputCityName = inputAddress.City.ToUpper();

            List<Address> filteredAddresses;

            filteredAddresses = addresses.Where(a => Math.Abs(a.Postcode - inputAddress.Postcode) <= 10).ToList();

            List<string> knownStreetNames = new List<string>();
            List<string> knownCityNames = new List<string>();

            foreach (Address address in filteredAddresses)
            {
                knownStreetNames.Add(address.Street);
                knownCityNames.Add(address.City);
            }

            // find the highest match            
            var closestStreetMatchResult = FuzzySharp.Process.ExtractOne(inputStreetName, knownStreetNames);

            var closestCityMatchResult = FuzzySharp.Process.ExtractOne(inputCityName, knownCityNames);

            InputAddress correctedAddress;

            if (closestStreetMatchResult == null)
            {
                inputAddress.Result = 0;
                return inputAddress;
            }

            if (closestStreetMatchResult.Score > 66 && closestCityMatchResult.Score > 66)
            {
                correctedAddress = new InputAddress
                {

                    Number = inputAddress.Number,
                    Street = closestStreetMatchResult.Value,
                    Unit = inputAddress.Unit,
                    City = closestCityMatchResult.Value,
                    Postcode = inputAddress.Postcode,
                    Region = inputAddress.Region,
                    Result = 1
                };
            }
            else
            {
                correctedAddress = inputAddress;
                correctedAddress.Result = 0;
            }

              

            // increment the metrics
            if (_context.Metrics.Find(1) == null)
            {
                _context.Metrics.Add(new Metrics { TotalAddresses = 1, CorrectedAddresses = 0, FailedAddresses = 0 });
            }
            else
            {
                _context.Metrics.Find(1).TotalAddresses++;
            }

            if (correctedAddress != null && correctedAddress.Result == 0)
            {
                _context.Metrics.Find(1).FailedAddresses++;
            }
            else
            {
                _context.Metrics.Find(1).CorrectedAddresses++;
            }

            stopwatch.Stop();
            long timeTaken = stopwatch.ElapsedMilliseconds;



            // increment the "corrected addresses" metric


            return await Task.FromResult(correctedAddress);
        }

        public async Task<InputAddress> BatchCorrectAddressesAsync(Address inputAddress, List<Address> addresses)
        {
            Console.WriteLine("Correcting addresses...");
 
                // convert to input address
                InputAddress input = new InputAddress
                {
                    Number = inputAddress.Number,
                    Street = inputAddress.Street,
                    Unit = inputAddress.Unit,
                    City = inputAddress.City,
                    Postcode = inputAddress.Postcode,
                    Region = inputAddress.Region
                };

            InputAddress correctedAddress = await CorrectAddressAsync(input, addresses);

            Debug.WriteLine("Address id: " + inputAddress.Id);

            // find the address in addresses with the same id
            Address originalAddress = addresses.Find(a => a.Id == inputAddress.Id);


                if(originalAddress.City == correctedAddress.City && originalAddress.Street == correctedAddress.Street)
                {
                    return correctedAddress;
                }
                else // if the corrected address is different from the original address but it was still corrected, then the correction was incorrect. We need to track this, so we increment the miscalculated addresses metric and decrement the corrected addresses metric
                {
                correctedAddress.Result = 0;
                // increment the miscalcalculated addresses metric
                _context.Metrics.Find(1).MiscorrectedAddresses++;
                // decrement the corrected addresses metric
                _context.Metrics.Find(1).CorrectedAddresses--;
                }

            return correctedAddress;
        }   


        public async Task<List<Address>> LoadAddressesFromCsvAsync(AddressContext context)
        {
            using (var reader = new StreamReader("./Data/au.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<Address>().ToList();
                return await Task.FromResult(records);
            }
        }
    }
}
