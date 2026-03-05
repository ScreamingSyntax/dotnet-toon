using System.Diagnostics;
using System.Text.Json;
using Toon.Models;
using ToonSharp;

Console.WriteLine("=============================================================");
Console.WriteLine("  ToonSharp Serialization Demo — Toon vs JSON Comparison");
Console.WriteLine("=============================================================\n");

// ---------------------------------------------------------------------------
// 1. Basic serialization: convert a simple object to the Toon format
// ---------------------------------------------------------------------------
var forecast = new WeatherForecast
{
    City = "Seattle",
    Date = new DateOnly(2025, 7, 15),
    TemperatureC = 28,
    Summary = "Warm and sunny"
};

string toonOutput = ToonSerializer.Serialize(forecast);

Console.WriteLine("--- 1. Basic Toon Serialization ---");
Console.WriteLine(toonOutput);
Console.WriteLine();

// ---------------------------------------------------------------------------
// 2. Deserialization: parse a Toon string back into a typed object
// ---------------------------------------------------------------------------
var restored = ToonSerializer.Deserialize<WeatherForecast>(toonOutput);

Console.WriteLine("--- 2. Deserialized Object ---");
Console.WriteLine($"  City         : {restored?.City}");
Console.WriteLine($"  Date         : {restored?.Date}");
Console.WriteLine($"  TemperatureC : {restored?.TemperatureC}");
Console.WriteLine($"  TemperatureF : {restored?.TemperatureF}");
Console.WriteLine($"  Summary      : {restored?.Summary}");
Console.WriteLine();

// ---------------------------------------------------------------------------
// 3. Nested object serialization
// ---------------------------------------------------------------------------
var userProfile = new UserProfile
{
    Name = "Alice Johnson",
    Age = 29,
    Email = "alice@example.com",
    Address = new Address
    {
        Street = "742 Evergreen Terrace",
        City = "Springfield",
        State = "IL",
        ZipCode = "62704"
    },
    Hobbies = ["Reading", "Hiking", "Photography"]
};

string userToon = ToonSerializer.Serialize(userProfile);

Console.WriteLine("--- 3. Nested Object (Toon) ---");
Console.WriteLine(userToon);
Console.WriteLine();

// ---------------------------------------------------------------------------
// 4. Deserialization with TryDeserialize (safe parsing)
// ---------------------------------------------------------------------------
Console.WriteLine("--- 4. Safe Deserialization with TryDeserialize ---");

if (ToonSerializer.TryDeserialize<UserProfile>(userToon, out var parsedProfile))
{
    Console.WriteLine($"  Name    : {parsedProfile.Name}");
    Console.WriteLine($"  Email   : {parsedProfile.Email}");
    Console.WriteLine($"  Address : {parsedProfile.Address.Street}, {parsedProfile.Address.City}");
    Console.WriteLine($"  Hobbies : {string.Join(", ", parsedProfile.Hobbies)}");
}
else
{
    Console.WriteLine("  Failed to deserialize the Toon string.");
}
Console.WriteLine();

// ---------------------------------------------------------------------------
// 5. Serializer options
// ---------------------------------------------------------------------------
Console.WriteLine("--- 5. Custom Serializer Options ---");

var options = new ToonSerializerOptions
{
    IndentSize = 4,
    Delimiter = ToonDelimiter.Pipe
};

string customToon = ToonSerializer.Serialize(userProfile, options);
Console.WriteLine(customToon);
Console.WriteLine();

// ---------------------------------------------------------------------------
// 6. Async serialization / deserialization via streams
// ---------------------------------------------------------------------------
Console.WriteLine("--- 6. Async Stream Serialization ---");

using var memoryStream = new MemoryStream();
await ToonSerializer.SerializeAsync(memoryStream, forecast);

memoryStream.Position = 0;
var asyncRestored = await ToonSerializer.DeserializeAsync<WeatherForecast>(memoryStream);
Console.WriteLine($"  Async deserialized city: {asyncRestored?.City}");
Console.WriteLine();

// ---------------------------------------------------------------------------
// 7. Performance comparison: Toon vs JSON
// ---------------------------------------------------------------------------
Console.WriteLine("--- 7. Toon vs JSON — Performance Comparison ---");

// Build a large list of objects to benchmark
var largeList = Enumerable.Range(0, 10_000).Select(i => new WeatherForecast
{
    City = $"City_{i}",
    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
    TemperatureC = Random.Shared.Next(-40, 50),
    Summary = "Sample forecast entry"
}).ToList();

// --- Toon serialization benchmark ---
var sw = Stopwatch.StartNew();
var toonResults = new List<string>(largeList.Count);
foreach (var item in largeList)
    toonResults.Add(ToonSerializer.Serialize(item));
sw.Stop();
long toonSerializeMs = sw.ElapsedMilliseconds;
long toonTotalSize = toonResults.Sum(t => (long)t.Length);

// --- JSON serialization benchmark ---
sw.Restart();
var jsonResults = new List<string>(largeList.Count);
foreach (var item in largeList)
    jsonResults.Add(JsonSerializer.Serialize(item));
sw.Stop();
long jsonSerializeMs = sw.ElapsedMilliseconds;
long jsonTotalSize = jsonResults.Sum(j => (long)j.Length);

// --- Toon deserialization benchmark ---
sw.Restart();
foreach (var t in toonResults)
    ToonSerializer.Deserialize<WeatherForecast>(t);
sw.Stop();
long toonDeserializeMs = sw.ElapsedMilliseconds;

// --- JSON deserialization benchmark ---
sw.Restart();
foreach (var j in jsonResults)
    JsonSerializer.Deserialize<WeatherForecast>(j);
sw.Stop();
long jsonDeserializeMs = sw.ElapsedMilliseconds;

Console.WriteLine($"  Items serialized   : {largeList.Count:N0}");
Console.WriteLine();
Console.WriteLine($"  {"Metric",-25} {"Toon",12} {"JSON",12}");
Console.WriteLine($"  {new string('-', 25)} {new string('-', 12)} {new string('-', 12)}");
Console.WriteLine($"  {"Serialize (ms)",-25} {toonSerializeMs,12:N0} {jsonSerializeMs,12:N0}");
Console.WriteLine($"  {"Deserialize (ms)",-25} {toonDeserializeMs,12:N0} {jsonDeserializeMs,12:N0}");
Console.WriteLine($"  {"Total size (chars)",-25} {toonTotalSize,12:N0} {jsonTotalSize,12:N0}");
Console.WriteLine();

// ---------------------------------------------------------------------------
// 8. Side-by-side output comparison
// ---------------------------------------------------------------------------
Console.WriteLine("--- 8. Side-by-Side: Toon vs JSON ---");
Console.WriteLine();

string sampleToon = ToonSerializer.Serialize(userProfile);
string sampleJson = JsonSerializer.Serialize(userProfile, new JsonSerializerOptions { WriteIndented = true });

Console.WriteLine("  [Toon Format]");
foreach (var line in sampleToon.Split('\n'))
    Console.WriteLine("  " + line);
Console.WriteLine();

Console.WriteLine("  [JSON Format]");
foreach (var line in sampleJson.Split('\n'))
    Console.WriteLine("  " + line);
Console.WriteLine();

Console.WriteLine("=============================================================");
Console.WriteLine("  Demo complete. See README.md for full documentation.");
Console.WriteLine("=============================================================");
