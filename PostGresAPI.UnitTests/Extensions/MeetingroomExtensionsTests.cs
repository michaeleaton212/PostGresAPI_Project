using PostGresAPI.Extensions;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Extensions;

public class MeetingroomExtensionsTests
{
    [Fact]
    public void ToDto_ValidMeetingroom_ReturnsMappedDto()
    {
        // Arrange
        var meetingroom = new Meetingroom("Board Room", 12);
        meetingroom.SetImagePath("boardroom.jpg");

        // Act
        var result = meetingroom.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Board Room", result.Name);
        Assert.Equal(12, result.NumberOfChairs);
        Assert.Equal("boardroom.jpg", result.ImagePath);
    }

    [Fact]
    public void ToDto_MeetingroomWithoutImage_ReturnsDtoWithNullImagePath()
    {
        // Arrange
        var meetingroom = new Meetingroom("Simple Room", 5);

        // Act
        var result = meetingroom.ToDto();

        // Assert
        Assert.Null(result.ImagePath);
    }

    [Fact]
    public void ToEntity_ValidCreateDto_ReturnsMeetingroomEntity()
    {
        // Arrange
        var dto = new CreateMeetingroomDto
        {
            Name = "New Conference Room",
            NumberOfChairs = 20,
            ImagePath = "conference.jpg"
        };

        // Act
        var result = dto.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Conference Room", result.Name);
        Assert.Equal(20, result.NumberOfChairs);
        Assert.Equal("conference.jpg", result.ImagePath);
    }

    [Fact]
    public void ToEntity_CreateDtoWithoutImage_ReturnsMeetingroomWithNullImagePath()
    {
        // Arrange
        var dto = new CreateMeetingroomDto
        {
            Name = "Basic Meeting Room",
            NumberOfChairs = 8
        };

        // Act
        var result = dto.ToEntity();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Basic Meeting Room", result.Name);
        Assert.Equal(8, result.NumberOfChairs);
        Assert.Null(result.ImagePath);
    }

    [Fact]
    public void ApplyUpdate_ValidUpdateDto_UpdatesMeetingroomProperties()
    {
        // Arrange
        var meetingroom = new Meetingroom("Old Name", 10);
        meetingroom.SetImagePath("old.jpg");
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Updated Conference",
            NumberOfChairs = 25,
            ImagePath = "updated.jpg"
        };

        // Act
        meetingroom.ApplyUpdate(updateDto);

        // Assert
        Assert.Equal("Updated Conference", meetingroom.Name);
        Assert.Equal(25, meetingroom.NumberOfChairs);
        Assert.Equal("updated.jpg", meetingroom.ImagePath);
    }

    [Fact]
    public void ApplyUpdate_UpdateDtoWithNullImage_SetsImagePathToNull()
    {
        // Arrange
        var meetingroom = new Meetingroom("Room with Image", 15);
        meetingroom.SetImagePath("existing.jpg");
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Updated Room",
            NumberOfChairs = 15
        };

        // Act
        meetingroom.ApplyUpdate(updateDto);

        // Assert
        Assert.Equal("Updated Room", meetingroom.Name);
        Assert.Null(meetingroom.ImagePath);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void ToEntity_VariousChairCounts_CreatesMeetingroom(int chairCount)
    {
        // Arrange
        var dto = new CreateMeetingroomDto
        {
            Name = $"Room-{chairCount}chairs",
            NumberOfChairs = chairCount,
            ImagePath = "test.jpg"
        };

        // Act
        var result = dto.ToEntity();

        // Assert
        Assert.Equal(chairCount, result.NumberOfChairs);
    }

    [Fact]
    public void ApplyUpdate_ChangesOnlyChairCount_KeepsOtherPropertiesIntact()
    {
        // Arrange
        var meetingroom = new Meetingroom("Conference Room", 10);
        meetingroom.SetImagePath("conf.jpg");
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Conference Room",
            NumberOfChairs = 20,
            ImagePath = "conf.jpg"
        };

        // Act
        meetingroom.ApplyUpdate(updateDto);

        // Assert
        Assert.Equal("Conference Room", meetingroom.Name);
        Assert.Equal(20, meetingroom.NumberOfChairs);
        Assert.Equal("conf.jpg", meetingroom.ImagePath);
    }

    [Fact]
    public void ToDto_MeetingroomWithSpecialCharacters_MapsCorrectly()
    {
        // Arrange
        var meetingroom = new Meetingroom("Room A&B (Main)", 30);
        meetingroom.SetImagePath("rooms/main/image.png");

        // Act
        var result = meetingroom.ToDto();

        // Assert
        Assert.Equal("Room A&B (Main)", result.Name);
        Assert.Equal("rooms/main/image.png", result.ImagePath);
    }

    [Fact]
    public void ApplyUpdate_UpdateFromZeroToNonZeroChairs_UpdatesCorrectly()
    {
        // Arrange
        var meetingroom = new Meetingroom("Standing Room", 0);
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Seated Room",
            NumberOfChairs = 15,
            ImagePath = "seated.jpg"
        };

        // Act
        meetingroom.ApplyUpdate(updateDto);

        // Assert
        Assert.Equal("Seated Room", meetingroom.Name);
        Assert.Equal(15, meetingroom.NumberOfChairs);
    }

    [Fact]
    public void ApplyUpdate_UpdateFromNonZeroToZeroChairs_UpdatesCorrectly()
    {
        // Arrange
        var meetingroom = new Meetingroom("Seated Room", 20);
        var updateDto = new UpdateMeetingroomDto
        {
            Name = "Standing Room",
            NumberOfChairs = 0,
            ImagePath = "standing.jpg"
        };

        // Act
        meetingroom.ApplyUpdate(updateDto);

        // Assert
        Assert.Equal("Standing Room", meetingroom.Name);
        Assert.Equal(0, meetingroom.NumberOfChairs);
    }
}
