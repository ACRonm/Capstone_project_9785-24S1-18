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

        public async Task<InputAddress?> CorrectAddressAsync(InputAddress inputAddress, List<Address> addresses)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Restart();

            AddressCorrectionService addressCorrectionService = new AddressCorrectionService(_context);

            if (inputAddress.Street == null || inputAddress.City == null)
            {
                Console.WriteLine("No street name provided.");
                return null;
            }

            string inputStreetName = inputAddress.Street.ToUpper();
            string inputCityName = inputAddress.City.ToUpper();

            List<Address> filteredAddresses;

            filteredAddresses = addresses.Where(a => Math.Abs(a.Postcode - inputAddress.Postcode) <= 10).ToList();

            HashSet<string> knownStreetNames = new HashSet<string>();
            HashSet<string> knownCityNames = new HashSet<string>();

            foreach (Address address in filteredAddresses)
            {
                knownStreetNames.Add(address.Street);
                knownCityNames.Add(address.City);
            }

            var closestStreetMatchTask = Task.Run(() => FuzzySharp.Process.ExtractOne(inputStreetName, knownStreetNames));
            var closestCityMatchTask = Task.Run(() => FuzzySharp.Process.ExtractOne(inputCityName, knownCityNames));

            // Wait for both tasks to complete
            Task.WaitAll(closestStreetMatchTask, closestCityMatchTask);

            // Get the results
            var closestStreetMatchResult = closestStreetMatchTask.Result;
            var closestCityMatchResult = closestCityMatchTask.Result;

            InputAddress correctedAddress;

            if (closestStreetMatchResult?.Score > 66 && closestCityMatchResult?.Score > 66)
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

            Metrics? metrics = _context.Metrics.Find(1);

            if (metrics == null)
            {
                _context.Metrics.Add(new Metrics { TotalAddresses = 1, CorrectedAddresses = 0, FailedAddresses = 0 });
            }
            else
            {
                metrics.TotalAddresses++;

                if (correctedAddress != null && correctedAddress.Result == 0)
                {
                    metrics.FailedAddresses++;
                }
                else
                {
                    metrics.CorrectedAddresses++;
                }

            }

            stopwatch.Stop();
            long timeTaken = stopwatch.ElapsedMilliseconds;

            correctedAddress.ProcessingTime = timeTaken;

            return await Task.FromResult(correctedAddress ?? new InputAddress());
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

            InputAddress? correctedAddress = await CorrectAddressAsync(input, addresses);

            // find the address in addresses with the same id
            Address originalAddress = addresses.Find(a => a.Id == inputAddress.Id);

            Metrics? metrics = _context.Metrics.Find(1);

            if (originalAddress.City == correctedAddress.City && originalAddress.Street == correctedAddress.Street)
            {
                return correctedAddress;
            }
            else if (metrics != null) // if the corrected address is different from the original address but it was still corrected, then the correction was incorrect. We need to track this, so we increment the miscalculated addresses metric and decrement the corrected addresses metric
            {

                correctedAddress.Result = 0;
                // increment the miscalcalculated addresses metric
                metrics.MiscorrectedAddresses++;
                // decrement the corrected addresses metric
                metrics.CorrectedAddresses--;
            }

            correctedAddress.TimeStamp = DateTime.Now;

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
