using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public PlatformDataClient(
        IConfiguration config,
        IMapper mapper)
    {
        _config = config;
        _mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatfroms()
    {
        Console.WriteLine($"---> Calling gRPC Service {_config["GrpcPlatform"]}");
        var channel = GrpcChannel.ForAddress(_config["GrpcPlatform"]);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (System.Exception ex )
        {
            Console.WriteLine($"---> Could not call gRPC server : {ex.Message}");
            return null;
        }
    }
}