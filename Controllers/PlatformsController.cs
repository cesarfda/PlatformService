using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;

namespace PlatformService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlatformsController : ControllerBase
    {
        private readonly ILogger<PlatformsController> _logger;
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _repository;

        public PlatformsController(ILogger<PlatformsController> logger, IMapper mapper, IPlatformRepo repository)
        {
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
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
    }
}