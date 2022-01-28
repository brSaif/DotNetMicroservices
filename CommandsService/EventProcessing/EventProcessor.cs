using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing;
public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory ssf,
                IMapper mapper)
    {
        _serviceScopeFactory = ssf;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;

            default:
                break;
        }
    
    }

    private void AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
            var platformPublishedDto = JsonSerializer
                .Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var plat = _mapper.Map<Platform>(platformPublishedDto);
                if (!repo.ExternalPlatformExists(plat.ExternalId))
                {
                    repo.CreatePlatform(plat);
                    repo.SaveChanges();
                    Console.WriteLine("---> Platform Added");
                }else
                {
                    Console.WriteLine("---> Platform already exist");
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"---> Could not add Platform to Db : {ex.Message}");
            }
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("---> Determining Event Type");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType.Event)
        {
            case "Platform_Published" : 
                Console.WriteLine("---> Platform Published Event Detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("---> Could not detect Platform Event");
                return EventType.Undetermined;
        };
    }
    
}

enum EventType 
{
    PlatformPublished,
    Undetermined
}