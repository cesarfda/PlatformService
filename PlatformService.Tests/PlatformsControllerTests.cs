using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using FluentAssertions;


namespace PlatformService.Tests
{
    public class PlatformsControllerTests : IDisposable
    {
        private readonly ILogger<PlatformsController> _logger;
        private readonly IMapper _mapper;
        private readonly IPlatformRepo _repository;
        private readonly AppDbContext _context;
        private readonly PlatformsController _controller;

        public class PlatformsProfile : Profile
        {
            public PlatformsProfile()
            {
                // Source -> Target
                CreateMap<Platform, PlatformReadDto>();
                CreateMap<PlatformCreateDto, Platform>();
            }
        }

        public PlatformsControllerTests()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("PlatformServiceTestDb")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new AppDbContext(options);
            _context.Database.EnsureCreated();

            _logger = new LoggerFactory().CreateLogger<PlatformsController>();
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PlatformsProfile>();
            });
            _mapper = mapperConfig.CreateMapper();
            _repository = new PlatformRepo(_context);
            _controller = new PlatformsController(_logger, _mapper, _repository);
        }

        [Fact]
        public void GetPlatforms_ReturnsOk_WithListOfPlatforms()
        {
            // Arrange
            var platformItems = new List<Platform>()
            {
                new Platform { Id = 1, Name = "Platform 1", Publisher = "Publisher 1", Cost = "Free" },
                new Platform { Id = 2, Name = "Platform 2", Publisher = "Publisher 2", Cost = "Paid" }
            };
            _context.Platforms.AddRange(platformItems);
            _context.SaveChanges();

            // Act
            var result = _controller.GetPlatforms();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<IEnumerable<PlatformReadDto>>()
                .Which.Should().HaveCount(platformItems.Count);
        }

        [Fact]
        public void GetPlatformsId_ReturnsOk_WithListOfPlatforms()
        {
            // Arrange
            var platformItems = new List<Platform>()
            {
                new Platform { Id = 1, Name = "Platform 1", Publisher = "Publisher 1", Cost = "Free" },
                new Platform { Id = 2, Name = "Platform 2", Publisher = "Publisher 2", Cost = "Paid" }
            };
            _context.Platforms.AddRange(platformItems);
            _context.SaveChanges();

            // Act
            var result = _controller.GetPlatformById(Id: 1);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeOfType<PlatformReadDto>()
                .Which.Should().BeEquivalentTo(platformItems[0], options => options.ExcludingMissingMembers());
        }

        [Fact]
        public void CreatePlatform_ReturnsOk()
        {
            // Arrange
            var platformItems = new List<Platform>()
            {
                new Platform { Id = 1, Name = "Platform 1", Publisher = "Publisher 1", Cost = "Free" },
                new Platform { Id = 2, Name = "Platform 2", Publisher = "Publisher 2", Cost = "Paid" }
            };
            _context.Platforms.AddRange(platformItems);
            _context.SaveChanges();

            // Act
            var result = _controller.CreatePlatform(new PlatformCreateDto { Name = "Platform 3", Publisher = "Publisher 3", Cost = "Free" });

            // Assert
            result.Result.Should().BeOfType<CreatedAtRouteResult>()
                .Which.StatusCode.Should().Be(201);
            result.Result.As<CreatedAtRouteResult>().RouteName.Should().Be("GetPlatformById");
            result.Result.As<CreatedAtRouteResult>().RouteValues["id"].Should().Be(3);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}