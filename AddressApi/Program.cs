using AddressApi.Models;
using AddressApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Microsoft.OpenApi.Models;

List<Address> addresses = new List<Address>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var connectionString = string.Empty;

// get Connectionstring from user secrets
if (builder.Environment.IsDevelopment())
{
    var config = builder.Configuration;
    connectionString = config["ConnectionString"];
    Console.WriteLine($"Connection String: {connectionString}");
}
else
    connectionString = builder.Configuration.GetConnectionString("ConnectionString");

builder.Services.AddDbContext<AddressContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<AddressCorrectionService>();

var app = builder.Build();

app.UseSwagger();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var context = services.GetRequiredService<AddressContext>();

    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine("Seeding database");
        AddressCorrectionService addressCorrectionService = new(context);
        addresses = await addressCorrectionService.LoadAddressesFromCsvAsync(context);
        Console.WriteLine("Number of addresses: " + addresses.Count);
    }
    else
        addresses = GetAddresses(context);
}

static List<Address> GetAddresses(AddressContext context)
{
    // set timeout to 5 minutes
    context.Database.SetCommandTimeout(300);
    return context.Addresses.ToList();
}

// mapget - get all addresses from InputAddresses table
app.MapGet("InputAddresses/CorrectedAddresses", (AddressContext context) =>
{
    // get list of input addresses
    List<InputAddress> correctedAddresses = context.InputAddresses.ToList();

    if (correctedAddresses != null)
    {
        return Results.Ok(correctedAddresses);
    }
    else
    {
        return Results.NotFound();
    }

})
    .WithName("CorrectedAddresses")
    .WithOpenApi();


// post metrics
app.MapPost("/Metrics", (Metrics metrics, AddressContext context) =>
{
    if (metrics != null)
    {
        context.Add(metrics);
        context.SaveChanges();
        return Results.Ok();
    }
    else
    {
        return Results.NotFound();
    }
})
    .WithName("CreateMetrics")
    .WithOpenApi();

//mapPut - update metrics
app.MapPut("/Metrics", (Metrics metrics, AddressContext context) =>
{
    // set all metrics to 0

    metrics = new Metrics();
    metrics = context.Metrics.Find(1);

    context.InputAddresses.RemoveRange(context.InputAddresses);
    context.SaveChanges();

    if (metrics == null)
    {
        return Results.NotFound();
    }
    metrics.CorrectedAddresses = 0;
    metrics.FailedAddresses = 0;
    metrics.MiscorrectedAddresses = 0;
    metrics.TotalAddresses = 0;

    //delete the input addresses
    context.InputAddresses.RemoveRange(context.InputAddresses);

    context.SaveChanges();
    return Results.Ok();

})
    .WithName("UpdateMetrics")
    .WithOpenApi();

app.MapPost("/InputAddresses", (InputAddress inputAddress, AddressContext context) =>
{
    //if addresses is empty , get addresses from db
    if (addresses.IsNullOrEmpty())
        addresses = GetAddresses(context);

    var addressCorrectionService = new AddressCorrectionService(context);

    InputAddress correctedAddress = addressCorrectionService.CorrectAddressAsync(inputAddress, addresses).Result;

    correctedAddress.TimeStamp = DateTime.Now;
    context.Add(correctedAddress);
    context.SaveChanges();
    if (correctedAddress != null)
    {
        return Results.Ok(correctedAddress);
    }
    else
    {
        return Results.NotFound();
    }
})
    .WithName("CreateInputAddress")
    .WithOpenApi();

// mappost batchinputaddresses
app.MapPost("/BatchInputAddresses", async (Address inputAddress, AddressContext context) =>
{

    var addressCorrectionService = new AddressCorrectionService(context);

    InputAddress correctedAddress = await addressCorrectionService.BatchCorrectAddressesAsync(inputAddress, addresses);

    correctedAddress.TimeStamp = DateTime.Now;

    context.Add(correctedAddress);
    context.SaveChanges();
    if (correctedAddress != null)
    {
        return Results.Ok(correctedAddress);
    }
    else
    {
        return Results.NotFound();
    }
})
    .WithName("CreateBatchInputAddresses")
    .WithOpenApi();

//delete all
app.MapDelete("/InputAddresses", (AddressContext context) =>
{
    context.InputAddresses.RemoveRange(context.InputAddresses);
    context.SaveChanges();
})
    .WithName("DeleteAllInputAddresses")
    .WithOpenApi();

//mapget metrics
app.MapGet("/Metrics", (AddressContext context) =>
{
    Metrics metrics = context.Metrics.Find(1);

    if (metrics != null)
    {
        return Results.Ok(metrics);
    }
    else
    {
        return Results.NotFound();
    }
})
    .WithName("Metrics")
    .WithOpenApi();

//mapget addresses
app.MapGet("/SampleAddresses", (AddressContext context) =>
{
    // get only first 1000 addresses
    var addresses = context.Addresses.Take(1000).ToList();
    if (addresses != null)
    {
        return Results.Ok(addresses);
    }
    else
    {
        return Results.NotFound();
    }
})
    .WithName("Addresses")
    .WithOpenApi();

// mapget inputaddresses
app.MapGet("/InputAddresses", (AddressContext context) =>
{
    List<TimeSeries> timeSeriesList = new List<TimeSeries>();

    List<InputAddress> InputAddresses = context.InputAddresses.ToList();

    foreach (var inputAddress in InputAddresses)
    {
        TimeSeries timeSeries = new TimeSeries();
        timeSeries.TimeStamp = inputAddress.TimeStamp;
        timeSeries.ProcessingTime = inputAddress.ProcessingTime;
        timeSeries.Id = inputAddress.Id;
        timeSeriesList.Add(timeSeries);
    }

    if (timeSeriesList != null)
    {
        return Results.Ok(timeSeriesList);
    }
    else
    {
        return Results.NotFound();
    }
})
    .WithName("InputAddresses")
    .WithOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
