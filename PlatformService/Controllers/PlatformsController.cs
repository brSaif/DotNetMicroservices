using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repo;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;
        private readonly IMessageBusClient _messageBusClient;

        public PlatformsController(IPlatformRepo repo, 
            IMapper mapper,
            ICommandDataClient commandDataClient,
            IMessageBusClient messageBusClient)
        {
            _repo = repo;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
            _messageBusClient = messageBusClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Platform>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");
            var platformItems = _repo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("--> gettting item by Id");
            var platform = _repo.GetPlatformById(id);
            if (platform is null) return NotFound();
            return Ok(_mapper.Map<PlatformReadDto>( platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repo.CreatePlatform(platformModel);
            _repo.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // Send Sync Message
            try
            {
                 await _commandDataClient.SendPlatformsToCommand(platformReadDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> couldn't send Synchronously : {ex.Message}");
            }

            // Send Async Message
            try
            {
                var publishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
                publishedDto.Event = "Platform_Published";
                _messageBusClient.PublishNewPlatform(publishedDto);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"--> couldn't send Asynchronously : {ex.Message}");
            }
            return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id },platformReadDto);
        }
    }
}
