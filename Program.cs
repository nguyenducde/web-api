using Catalog.Repositories;
using Catalog.Settings;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
 var mongoDBSettings =  builder.Configuration.GetSection(nameof (MongoDBSettings)).Get<MongoDBSettings>();
builder.Services.AddEndpointsApiExplorer();

BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
 
builder.Services.AddSingleton<IMongoClient>(ServiceProvider =>{
    
    return new MongoClient(mongoDBSettings.ConnectionString);
});
builder.Services.AddSingleton<IItemRepository,MongoDbItemRepository>();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false);
builder.Services.AddHealthChecks().AddMongoDb(mongoDBSettings.ConnectionString,
 name:"mongodb",
  timeout :TimeSpan.FromSeconds(3),
  tags: new []{"ready"}
  ); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapHealthChecks("/health/ready", new HealthCheckOptions{
 Predicate = (check) => check.Tags.Contains("ready"),
 ResponseWriter = async (context, report)=>{
    var result = JsonSerializer.Serialize(
        new {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                exception = entry.Value.Exception != null ? entry.Value.Exception.Message: "none",
                duration = entry.Value.Duration.ToString()
            })
        }
    );
    context.Response.ContentType = MediaTypeNames.Application.Json;
    await context.Response.WriteAsync(result);
 }

     });
app.MapHealthChecks("/health/live", new HealthCheckOptions{
         Predicate = (_)=> false
     });
app.MapControllers();
// app.UseRouting()
// ;app.UseEndpoints(enpoint=>{
   
//     enpoint.MapHealthChecks("/health/ready", new HealthCheckOptions{
//         Predicate = (check) => check.Tags.Contains("ready")
//     });
//      enpoint.MapHealthChecks("/health/live", new HealthCheckOptions{
//         Predicate = (_)=> false
//     });
// });
app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }
