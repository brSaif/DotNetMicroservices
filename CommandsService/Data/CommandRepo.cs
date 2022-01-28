using CommandsService.Models;

namespace CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }
    public void CreateCommand(int platformId, Command command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }

     

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    
    public IEnumerable<Command> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands.Where(p => p.PlatformId == platformId)
                    .OrderBy(c => c.Platform.Name);
    }

    public Command GetCommand(int platformId, int commandId)
    {
        return _context.Commands
                    .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId);
    }

    public void CreatePlatform(Platform platform)
    {
        if (platform is null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        _context.Platforms.Add(platform);
    }

    public bool PlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool ExternalPlatformExists(int ExternalId)
    {
        return _context.Platforms.Any(p => p.ExternalId == ExternalId);
    }

    public bool SaveChanges()
    {
        return (_context.SaveChanges() >= 0);
    }
}