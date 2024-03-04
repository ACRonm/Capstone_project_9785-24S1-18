using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AddressApi.Models
{
    public class Address
    {
        [Key]
        public string? Id { get; set; }
        public int Number { get; set; }
        public string? Street { get; set; }
        public string? Unit { get; set; }
        public string? Longitude { get; set; }
        public string? Latitude { get; set; }
        public string? City { get; set; }
        public int Postcode { get; set; }
        public string? Region { get; set; }
        public int Accuracy { get; set; }
    }

    public class  InputAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Number { get; set; }
        public string? Street { get; set; }
        public string? Unit { get; set; }
        public string? City { get; set; }
        public int Postcode { get; set; }
        public string? Region { get; set; }
    }
}
