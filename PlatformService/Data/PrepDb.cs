using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(WebApplication app)
        {
            using (var serviceScope = app.Services.CreateScope())
            {
                SeedData(context: serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context)
        {
            if (!context.Platforms.Any())
            {
                Console.WriteLine("---> Seeding Data");
                context.Platforms.AddRange(
                    new Models.Platform() { Name = "Dotnet  ", Publisher = "MS", Cost = "Free"},
                    new Models.Platform() { Name = "Dotnet 1", Publisher = "MS", Cost = "Free"},
                    new Models.Platform() { Name = "Dotnet 2", Publisher = "MS", Cost = "Free"}
                    );
                context.SaveChanges();
            }
            else
            {
                 Console.WriteLine("---> We already have data");
            }
        }
    }
}
