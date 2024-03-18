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


string connectionString = String.Empty;

//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddSingleton<SecretsService>();

//    SecretsService secretsService = new(builder.Configuration);

//    connectionString = secretsService.GetSecret("ConnectionString");
//}
//else
//{
connectionString = "Server=tcp:address-correction-server.database.windows.net,1433;Initial Catalog=address-correction-db;Persist Security Info=False;User ID=CloudSA622bf291;Password=datS47fDvK#PZw@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
//}

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
       AddressCorrectionService addressCorrectionService = new(context);
       addresses = await addressCorrectionService.LoadAddressesFromCsvAsync(context);
      
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

    if(metrics == null)
    {
        return Results.NotFound();
    }
    metrics.CorrectedAddresses = 0;
    metrics.FailedAddresses = 0;
    metrics.MiscorrectedAddresses = 0;
    metrics.TotalAddresses = 0;

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

    InputAddress correctedAddresses = await addressCorrectionService.BatchCorrectAddressesAsync(inputAddress, addresses);

    context.AddRange(correctedAddresses);
    context.SaveChanges();
    if (correctedAddresses != null)
    {
        return Results.Ok(correctedAddresses);
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
