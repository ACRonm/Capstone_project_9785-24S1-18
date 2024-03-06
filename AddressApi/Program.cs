using AddressApi.Models;
using AddressApi.Controllers;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    connection = Environment.GetEnvironmentVariable("DefaultConnection");
}

builder.Services.AddDbContext<AddressContext>(options =>
    options.UseSqlServer(connection));

builder.Services.AddScoped<AddressCorrectionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// get number of addresses in the database
app.MapGet("/Addresses/number", (AddressContext context) =>
{
    // set timeout to 5 minutes
    context.Database.SetCommandTimeout(300);

    return context.Addresses.Count();
})
    .WithName("NumberOfAddressesInDatabase")
    .WithOpenApi();

//get address by street name
app.MapGet("/Addresses/street/{street}", (string street, AddressContext context) =>
{
    return context.Addresses.Where(a => a.Street == street.ToUpper()).ToList();
})
    .WithName("AddressesByStreet")
    .WithOpenApi();

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
    context.Add(inputAddress);
    context.SaveChanges();

    var addressCorrectionService = new AddressCorrectionService(context);

    var correctedAddress = addressCorrectionService.CorrectAddressAsync(inputAddress).Result;
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

app.MapGet("/InputAddresses", (AddressContext context) =>
{
    return context.InputAddresses.ToList();
})
    .WithName("InputAddresses")
    .WithOpenApi();

app.MapGet("/InputAddresses/{id}", (int id, AddressContext context) =>
{
    return context.InputAddresses.Find(id);
})
    .WithName("InputAddress")
    .WithOpenApi();

app.MapDelete("/InputAddresses/{id}", (int id, AddressContext context) =>
{
    var inputAddress = context.InputAddresses.Find(id);
    //check to see if it object exits in db
    if (inputAddress != null)
    {
        context.InputAddresses.Remove(inputAddress);
        context.SaveChanges();
    }
})
    .WithName("DeleteInputAddress")
    .WithOpenApi();

//delete all
app.MapDelete("/InputAddresses", (AddressContext context) =>
{
    context.InputAddresses.RemoveRange(context.InputAddresses);
    context.SaveChanges();
})
    .WithName("DeleteAllInputAddresses")
    .WithOpenApi();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
