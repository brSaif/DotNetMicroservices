using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public interface ICommandDataClient{
    Task SendPlatformsToCommand(PlatformReadDto plat);
}