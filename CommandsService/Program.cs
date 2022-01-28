using CommandService.SyncDataServices.Grpc;
using CommandsService.AsyncDataServices;
using CommandsService.Data;
using CommandsService.EventProcessing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// if (builder.Environment.IsProduction())
// {
//     Console.WriteLine("---> Using Mssql DB");
//     builder.Services.AddDbContext<AppDbContext>(opt 
//         => opt.UseSqlServer(builder.Configuration.GetConnectionString("")));
// }else{
    Console.WriteLine("---> Using In Memory Db");
    builder.Services.AddDbContext<AppDbContext>(opt 
        => opt.UseInMemoryDatabase("InMemory")) ;
// }

builder.Services.AddScoped<ICommandRepo, CommandRepo>();

builder.Services.AddControllers();

builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

PrepDb.PrepPopulation(app);

app.Run();
