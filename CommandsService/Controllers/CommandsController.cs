using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICommandRepo _repo;

    public CommandsController(IMapper mapper, ICommandRepo repo)
    {
        _mapper = mapper;
        _repo = repo;
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(
         int platformId,
         CommandCreateDto commandDto)
    {

        Console.WriteLine($"---> Hit Create CommandForPlatform : {platformId}");

        if (!_repo.PlatformExists(platformId)) return NotFound();

        var command = _mapper.Map<Command>(commandDto);

        _repo.CreateCommand(platformId, command);
        _repo.SaveChanges();

        var commandRead = _mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommadForPlatform),
            new { platformId = platformId, commandId = commandRead.Id },
            commandRead);
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {

        Console.WriteLine($"---> Hit GetCommandsForPlatform : {platformId}");

        if (!_repo.PlatformExists(platformId)) return NotFound();

        var commands = _repo.GetCommandsForPlatform(platformId);

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommadForPlatform")]
    public ActionResult<CommandReadDto> GetCommadForPlatform(int platformId, int commandId)
    {

        Console.WriteLine($"---> Hit GetCommandForPlatform : {platformId} - {commandId}");

        if (!_repo.PlatformExists(platformId)) return NotFound();

        var command = _repo.GetCommand(platformId, commandId);

        if (command is null) return NotFound();

        return Ok(_mapper.Map<CommandReadDto>(command));
    }
}
