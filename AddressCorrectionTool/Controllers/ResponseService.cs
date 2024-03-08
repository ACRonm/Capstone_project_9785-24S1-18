using System;
using System.Net.Http;
using System.Threading.Tasks;
using AddressCorrectionTool.Models;

namespace AddressCorrectionTool.Controllers
{
    public class ResponseService
    {
        public InputAddress? CorrectedAddress { get; set; }

        public string CorrectedAddressToString()
        {
            if (CorrectedAddress == null)
            {
                return "No address to display";
            }
            else
            {
                return $"UNIT {CorrectedAddress.Unit} of {CorrectedAddress.Number} {CorrectedAddress.Street} {CorrectedAddress.City} {CorrectedAddress.Postcode} {CorrectedAddress.Region}";
            }
        }

    }

}