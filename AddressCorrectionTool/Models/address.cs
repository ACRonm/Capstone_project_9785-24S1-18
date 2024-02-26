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
    public string? Number { get; set; }
    public string? Street { get; set; }
    public string? Unit { get; set; }
    public string? City { get; set; }
    public string? Postcode { get; set; }
    public string? Region { get; set; }
}