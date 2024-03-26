using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace AddressCorrectionTool.Models;

public class Address
{
    [Name("id")]
    public string? Id { get; set; }
    [Name("number")]
    public string? Number { get; set; }
    [Name("street")]
    public string? Street { get; set; }
    [Name("unit")]
    public string? Unit { get; set; }
    [Name("lon")]
    public string? Longitude { get; set; }
    [Name("lat")]
    public string? Latitude { get; set; }
    [Name("city")]
    public string? City { get; set; }
    [Name("postcode")]
    public int Postcode { get; set; }
    [Name("region")]
    public string? Region { get; set; }
    [Name("accuracy")]
    public int Accuracy { get; set; }
}

public class InputAddress
{
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid number")]
    public string? Number { get; set; }
    [Required]
    public string? Street { get; set; }
    [RegularExpression(@"^\d+$", ErrorMessage = "Invalid number")]
    public string? Unit { get; set; }
    [Required]
    public string? City { get; set; }
    [Required]
    public int? Postcode { get; set; }
    [Required]
    public string? Region { get; set; }
    public int? Result { get; set; } = 0;
    public long? ProcessingTime { get; set; }
    public string? CorrectedStreet { get; set; }
    public string? CorrectedCity { get; set; }
    public int? CorrectedPostcode { get; set; }
    public DateTime TimeStamp { get; set; }
}
public class Metrics
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int TotalAddresses { get; set; }
    public int CorrectedAddresses { get; set; }
    public int FailedAddresses { get; set; }
    public int MiscorrectedAddresses { get; set; }
}