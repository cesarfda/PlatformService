using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var platforms = Assert.IsAssignableFrom<IEnumerable<PlatformReadDto>>(okResult.Value);
            Assert.Equal(platformItems.Count, platforms.Count());
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
            var result = _controller.GetPlatformById(Id:1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var platform = Assert.IsType<PlatformReadDto>(okResult.Value);
            Assert.Equal(platformItems[0].Id, platform.Id);
            Assert.Equal(platformItems[0].Name, platform.Name);
            Assert.Equal(platformItems[0].Publisher, platform.Publisher);
            Assert.Equal(platformItems[0].Cost, platform.Cost);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}