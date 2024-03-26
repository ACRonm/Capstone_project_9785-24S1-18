using AddressApi.Models;
using CsvHelper;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using System.Diagnostics.Metrics;

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

        public async Task<InputAddress?> CorrectAddressAsync(InputAddress inputAddress, List<Address> addresses)
        {
            InputAddress correctedAddress = new InputAddress();

            Console.WriteLine("Correcting address: " + inputAddress.Number + " " + inputAddress.Street + " " + inputAddress.City + " " + inputAddress.Postcode + " " + inputAddress.Region);

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

            // replace street type abbreviations with full words if they exist
            inputStreetName = Regex.Replace(inputStreetName, @"\bST\b|\bST\.\b", "STREET");
            inputStreetName = Regex.Replace(inputStreetName, @"\bRD\b|\bRD\.\b", "ROAD");
            inputStreetName = Regex.Replace(inputStreetName, @"\bCR\b|\bCR\.\b", "CRESCENT");
            inputStreetName = Regex.Replace(inputStreetName, @"\bDR\b|\bDR\.\b", "DRIVE");

            List<Address> filteredAddresses;

            HashSet<string> knownStreetNames = new HashSet<string>();
            HashSet<string> knownCityNames = new HashSet<string>();


            bool addressFound = false;
            int counter = 0;


            while (!addressFound)
            {


                if (counter > 2)
                    break;

                filteredAddresses = FilterAddresses(addresses, counter, inputAddress);

                foreach (Address address in filteredAddresses)
                {
                    knownStreetNames.Add(address.Street);
                    knownCityNames.Add(address.City);
                }

                var closestStreetMatchTask = Task.Run(() => FuzzySharp.Process.ExtractOne(inputStreetName, knownStreetNames, s => s, ScorerCache.Get<DefaultRatioScorer>()));
                var closestCityMatchTask = Task.Run(() => FuzzySharp.Process.ExtractOne(inputCityName, knownCityNames, s => s, ScorerCache.Get<DefaultRatioScorer>()));


                // Wait for both tasks to complete
                Task.WaitAll(closestStreetMatchTask, closestCityMatchTask);

                // Get the results
                var closestStreetMatchResult = closestStreetMatchTask.Result;
                var closestCityMatchResult = closestCityMatchTask.Result;


                if (closestStreetMatchResult?.Score > 40 && closestCityMatchResult?.Score > 40)
                {
                    addressFound = true;

                    correctedAddress = new InputAddress
                    {

                        Number = inputAddress.Number,
                        Street = inputAddress.Street,
                        Unit = inputAddress.Unit,
                        City = inputAddress.City,
                        Postcode = inputAddress.Postcode,
                        Region = inputAddress.Region,
                        Result = 1,
                        CorrectedStreet = closestStreetMatchResult.Value,
                        CorrectedCity = closestCityMatchResult.Value,
                        CorrectedPostcode = filteredAddresses.Find(a => a.Street == closestStreetMatchResult.Value && a.City == closestCityMatchResult.Value)?.Postcode,
                    };
                    addressFound = true;
                }
                else
                {
                    correctedAddress = inputAddress;
                    correctedAddress.CorrectedStreet = closestStreetMatchResult?.Value;
                    correctedAddress.CorrectedCity = closestCityMatchResult?.Value;
                    correctedAddress.CorrectedPostcode = inputAddress.Postcode;
                    correctedAddress.Result = 0;
                }
                counter++;
            }


            Metrics? metrics = _context.Metrics.Find(1);

            if (metrics == null)
            {
                _context.Metrics.Add(new Metrics { TotalAddresses = 1, CorrectedAddresses = 0, FailedAddresses = 0 });
            }
            else
            {
                metrics.TotalAddresses++;

                if (correctedAddress.Result == 0)
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

            if (originalAddress.City == correctedAddress.CorrectedCity && originalAddress.Street == correctedAddress.CorrectedStreet)
            {
                return correctedAddress;
            }
            else if (metrics != null) // if the corrected address is different from the original address but it was still corrected, then the correction was incorrect. We need to track this, so we increment the miscalculated addresses metric and decrement the corrected addresses metric
            {

                correctedAddress.Result = 2;
                // increment the miscorrected addresses metric
                metrics.MiscorrectedAddresses++;
                // decrement the corrected addresses metric
                metrics.CorrectedAddresses--;
            }

            correctedAddress.TimeStamp = DateTime.Now;

            return correctedAddress;
        }

        private List<Address> FilterAddresses(List<Address> addresses, int counter, InputAddress inputAddress)
        {
            if (counter == 0)
            {
                // filter addresses by postcode
                return addresses.Where(a => a.Postcode == inputAddress.Postcode).ToList();
            }
            else if (counter == 1)
            {
                // filter addresses by region
                return addresses.Where(a => a.Region == inputAddress.Region).ToList();
            }
            else
            {
                return addresses;
            }
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
