using CommandsService.Models;

namespace CommandsService.Data;

public interface ICommandRepo
{
    // Platforms
    
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExists(int id);
    bool ExternalPlatformExists(int ExternalId);

    // Commmands
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);

    bool SaveChanges();
}