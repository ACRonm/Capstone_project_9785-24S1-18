using System.ComponentModel.DataAnnotations;

namespace AddressCorrectionTool.Models;


public class Address
{
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
}