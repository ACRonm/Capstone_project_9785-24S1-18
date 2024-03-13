using AddressApi.Models;
using AddressApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

List<Address> addresses = new List<Address>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{

    builder.Services.AddSingleton<SecretsService>();

    SecretsService secretsService = new SecretsService(builder.Configuration);
     
    connection = secretsService.GetSecret("ConnectionString");
}
else
{
    connection = builder.Configuration.GetConnectionString("DefaultConnection");
}

builder.Services.AddDbContext<AddressContext>(options =>
    options.UseSqlServer(connection));

builder.Services.AddScoped<AddressCorrectionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var context = services.GetRequiredService<AddressContext>();

    addresses = GetAddresses(context);
}

// get addresses from csv
app.MapGet("/Addresses/csv", (AddressContext context) =>
{
    var addressCorrectionService = new AddressCorrectionService(context);
    var addresses = addressCorrectionService.LoadAddressesFromCsvAsync().Result;

    var count = addresses.Count;

    // insert into db
    context.AddRange(addresses);

    context.SaveChangesAsync();

    if (addresses != null)
    {
        return Results.Ok("Addresses loaded from csv: " + count);
    }
    else
    {
        return Results.NotFound();
    }

})
    .WithName("AddressesFromCsv")
    .WithOpenApi();

    static List<Address> GetAddresses(AddressContext context)
    {

        Debug.WriteLine("Getting Addresses");
        // set timeout to 5 minutes
        context.Database.SetCommandTimeout(300);
        return context.Addresses.ToList();
    }

app.MapPost("/Addresses", (List<Address> addresses, AddressContext context) =>
{

    if (addresses != null)
    {
        context.AddRange(addresses);
        context.SaveChanges();

        return Results.Ok();
    }
    else { return Results.NotFound(); }
})
.WithName("CreateAddress")
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

// Get address by id
app.MapGet("/Addresses/{id}", (string id, AddressContext context) =>
{
    return context.Addresses.Find(id);
})
    .WithName("Address")
    .WithOpenApi();

app.MapDelete("/Addresses/{id}", (string id, AddressContext context) =>
{
    var address = context.Addresses.Find(id);
    if (address != null)
    {
        context.Addresses.Remove(address);
        context.SaveChanges();
    }
})
    .WithName("DeleteAddress")
    .WithOpenApi();

// delete all addresses
app.MapDelete("/Addresses", (AddressContext context) =>
{
    context.Addresses.RemoveRange(context.Addresses);
    context.SaveChanges();
})
    .WithName("DeleteAllAddresses")
    .WithOpenApi();


app.MapPost("/InputAddresses", (InputAddress inputAddress, AddressContext context) =>
{
    //if addresses is empty , get addresses from db
    if (addresses.IsNullOrEmpty())
        addresses = GetAddresses(context);

    var addressCorrectionService = new AddressCorrectionService(context);

    var correctedAddress = addressCorrectionService.CorrectAddressAsync(inputAddress, addresses).Result;

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
    //print to debug

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
