using CommandService.SyncDataServices.Grpc;
using CommandsService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandsService.Data;

public static class PrepDb{
    public static void PrepPopulation(WebApplication app){
        using (var serviceScope = app.Services.CreateScope())
        {
            var GrpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
            var platforms = GrpcClient.ReturnAllPlatfroms();

            SeedData(serviceScope.ServiceProvider.GetService<ICommandRepo>(), platforms);
        }
    }

    private static void SeedData(ICommandRepo repository, IEnumerable<Platform> platforms){
        
        Console.WriteLine("---> Seeding new platforms...");
        foreach (var plat in platforms)
        {
            if (!repository.ExternalPlatformExists(plat.ExternalId))
            {
                repository.CreatePlatform(plat);
            }

            repository.SaveChanges();
        } 
    }
}