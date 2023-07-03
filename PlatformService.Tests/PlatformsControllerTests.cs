using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PlatformService.Controllers;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using Xunit;

namespace PlatformService.Tests
{
    public class PlatformsControllerTests
    {
        private readonly Mock<ILogger<PlatformsController>> _loggerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IPlatformRepo> _repositoryMock;
        private readonly PlatformsController _controller;

        public PlatformsControllerTests()
        {
            _loggerMock = new Mock<ILogger<PlatformsController>>();
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IPlatformRepo>();
            _controller = new PlatformsController(_loggerMock.Object, _mapperMock.Object, _repositoryMock.Object);
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
            _repositoryMock.Setup(repo => repo.GetAllPlatforms()).Returns(platformItems);
            var platformReadDtos = new List<PlatformReadDto>()
            {
                new PlatformReadDto { Id = 1, Name = "Platform 1" },
                new PlatformReadDto { Id = 2, Name = "Platform 2" }
            };
            _mapperMock.Setup(mapper => mapper.Map<IEnumerable<PlatformReadDto>>(platformItems)).Returns(platformReadDtos);

            // Act
            var result = _controller.GetPlatforms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var platforms = Assert.IsAssignableFrom<IEnumerable<PlatformReadDto>>(okResult.Value);
        }
    }
}