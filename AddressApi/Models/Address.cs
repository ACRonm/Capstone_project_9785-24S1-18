using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressApi.Models
{
    [Index(nameof(Street), nameof(City), nameof(Postcode), nameof(Region))]
    public class Address
    {
        [Key]
        public string? Id { get; set; }
        public string? Number { get; set; }
        public string? Street { get; set; }
        public string? Unit { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public string? City { get; set; }
        public int Postcode { get; set; }
        public string? Region { get; set; }
        public int Accuracy { get; set; }
    }

    [Index(nameof(Street), nameof(City), nameof(Postcode), nameof(Region), nameof(Result))]
    public class  InputAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Street { get; set; }
        public string? Unit { get; set; }
        public string? City { get; set; }
        public int Postcode { get; set; }
        public string? Region { get; set; }
        public int Result { get; set; }
        public float Score { get; set; }
        public TimeSpan ProcessingTime { get; set; }
    }

    public class Metrics
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TotalAddresses { get; set; }
        public int CorrectedAddresses { get; set; }
        public int FailedAddresses { get; set; }
    }

}
