namespace Toon.Models;

/// <summary>
/// Represents a weather forecast entry used to demonstrate ToonSharp serialization.
/// </summary>
public class WeatherForecast
{
    public string City { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string? Summary { get; set; }
}

/// <summary>
/// Represents a user profile with nested address information.
/// </summary>
public class UserProfile
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public List<string> Hobbies { get; set; } = [];
}

/// <summary>
/// Represents a physical address.
/// </summary>
public class Address
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
}
