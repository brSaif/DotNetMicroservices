using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(WebApplication app)
    {
        using (var serviceScope = app.Services.CreateScope())
        {
            SeedData(
                context: serviceScope.ServiceProvider.GetService<AppDbContext>(),
                app.Environment.IsProduction());
        }
    }

    private static void SeedData(AppDbContext context, bool isProd)
    {
        if (isProd)
        {
            Console.WriteLine("---> Attempting to apply migrations");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {

                Console.WriteLine($"--> Could not run migrations : {ex}");
            }
        }

        if (!context.Platforms.Any())
        {
            Console.WriteLine("---> Seeding Data");
            PopulateDb(context);
        }
        else
        {
            Console.WriteLine("---> We already have data");
        }
    }

    private static void PopulateDb(AppDbContext context)
    {
        context.Platforms.AddRange(
            new Models.Platform() { Name = "Dotnet  3.1", Publisher = "MS", Cost = "Free" },
            new Models.Platform() { Name = "Dotnet 5", Publisher = "MS", Cost = "Free" },
            new Models.Platform() { Name = "Dotnet 6", Publisher = "MS", Cost = "Free" }
            );
        context.SaveChanges();
    }
}
