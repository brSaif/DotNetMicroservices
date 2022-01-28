using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
if (builder.Environment.IsProduction())
{
    Console.WriteLine("---> Using mssql db");
    builder.Services.AddDbContext<AppDbContext>(opt 
        => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));
}
else
{
    Console.WriteLine("---> Using inMem db");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMemory"));
}

// Inject Platform dependency
builder.Services.AddScoped<IPlatformRepo,PlatformRepo>();

builder.Services.AddSingleton<IMessageBusClient,MessageBusClient>();
builder.Services.AddHttpClient<ICommandDataClient,HttpCommandDataClient>();

builder.Services.AddGrpc();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var srt = builder.Configuration.GetValue<string>("CommandService");
Console.WriteLine($"---> Remote EndPoint : {srt}");


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/proto/platforms.proto",async ctx => {
    await ctx.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

PrepDb.PrepPopulation(app);

app.Run();
