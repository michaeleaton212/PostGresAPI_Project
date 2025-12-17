// BedroomsControllerTests.cs
using Microsoft.AspNetCore.Mvc;
using Moq;
using PostGresAPI.Controllers;
using PostGresAPI.Contracts;
using PostGresAPI.Services;
using Xunit;

namespace PostGresAPI.Tests.Controllers;

public sealed class BedroomsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOk_WithRooms()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        var expected = new List<BedroomDto>
        {
            new BedroomDto { Id = 1, Name = "Room 1", NumberOfBeds = 2, ImagePath = "/a.png" },
            new BedroomDto { Id = 2, Name = "Room 2", NumberOfBeds = 1, ImagePath = "/b.png" }
        };
        service.Setup(s => s.GetAll()).ReturnsAsync(expected);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsAssignableFrom<IEnumerable<BedroomDto>>(ok.Value);
        Assert.Equal(expected.Count, value.Count());
        Assert.Equal(expected[0].Id, value.First().Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenNull()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        service.Setup(s => s.GetById(123)).ReturnsAsync((BedroomDto?)null);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.GetById(123);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        var expected = new BedroomDto { Id = 5, Name = "X", NumberOfBeds = 3, ImagePath = "/x.png" };
        service.Setup(s => s.GetById(5)).ReturnsAsync(expected);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.GetById(5);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<BedroomDto>(ok.Value);
        Assert.Equal(expected.Id, value.Id);
        Assert.Equal(expected.Name, value.Name);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithCreatedDto()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        var input = new CreateBedroomDto { Name = "New", NumberOfBeds = 2, ImagePath = "/n.png" };
        var created = new BedroomDto { Id = 10, Name = "New", NumberOfBeds = 2, ImagePath = "/n.png" };
        service.Setup(s => s.Create(input)).ReturnsAsync(created);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.Create(input);

        // Assert
        var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BedroomsController.GetById), createdAt.ActionName);
        Assert.Equal(10, createdAt.RouteValues?["id"]);
        var value = Assert.IsType<BedroomDto>(createdAt.Value);
        Assert.Equal(10, value.Id);
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenNull()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        var dto = new UpdateBedroomDto { Name = "Upd", NumberOfBeds = 4, ImagePath = "/u.png" };
        service.Setup(s => s.Update(7, dto)).ReturnsAsync((BedroomDto?)null);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.Update(7, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenUpdated()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        var dto = new UpdateBedroomDto { Name = "Upd", NumberOfBeds = 4, ImagePath = "/u.png" };
        var updated = new BedroomDto { Id = 7, Name = "Upd", NumberOfBeds = 4, ImagePath = "/u.png" };
        service.Setup(s => s.Update(7, dto)).ReturnsAsync(updated);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.Update(7, dto);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var value = Assert.IsType<BedroomDto>(ok.Value);
        Assert.Equal(7, value.Id);
        Assert.Equal("Upd", value.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleted()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        service.Setup(s => s.Delete(3)).ReturnsAsync(true);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.Delete(3);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        // Arrange
        var service = new Mock<IBedroomService>();
        service.Setup(s => s.Delete(3)).ReturnsAsync(false);
        var controller = new BedroomsController(service.Object);

        // Act
        var result = await controller.Delete(3);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
