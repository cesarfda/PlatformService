using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ILogger<PlatformsController> _logger;
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _repository;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(ILogger<PlatformsController> logger, IMapper mapper, IPlatformRepo repository, ICommandDataClient commandDataClient)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            _logger.LogInformation("--> Getting Platforms...");

            var platformItems = _repository.GetAllPlatforms();

            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
        }

        [HttpGet("{id:int}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int Id)
        {
            _logger.LogInformation("--> Getting Platform...");

            try
            {
                var platformItem = _repository.GetPlatformById(Id);
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            catch (Exception ex)
            {
                _logger.LogError($"--> {ex.Message}");
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            _logger.LogInformation("--> Creating Platform...");

            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch(Exception ex)
            {
                _logger.LogError($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { platformReadDto.Id }, platformReadDto);
        }
    }
}