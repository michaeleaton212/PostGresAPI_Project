using PostGresAPI.Extensions;
using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.UnitTests.Extensions;

public class RoomExtensionsTests
{
    [Fact]
    public void ToDto_BedroomRoom_ReturnsDtoWithBedroomType()
    {
        // Arrange
        var bedroom = new Bedroom("Luxury Suite", 2);
        bedroom.SetImagePath("bedroom.jpg");
        Room room = bedroom;

        // Act
        var result = room.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Luxury Suite", result.Name);
        Assert.Equal("Bedroom", result.Type);
        Assert.Equal(2, result.NumberOfBeds);
        Assert.Null(result.NumberOfChairs);
        Assert.Equal("bedroom.jpg", result.image);
    }

    [Fact]
    public void ToDto_MeetingroomRoom_ReturnsDtoWithMeetingroomType()
    {
        // Arrange
        var meetingroom = new Meetingroom("Conference Room A", 10);
        meetingroom.SetImagePath("meeting.jpg");
        Room room = meetingroom;

        // Act
        var result = room.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Conference Room A", result.Name);
        Assert.Equal("Meetingroom", result.Type);
        Assert.Null(result.NumberOfBeds);
        Assert.Equal(10, result.NumberOfChairs);
        Assert.Equal("meeting.jpg", result.image);
    }

    [Fact]
    public void ToDto_BaseRoom_ReturnsDtoWithUnknownType()
    {
        // Arrange
        var room = new TestRoom("Generic Room");
        room.SetImagePath("generic.jpg");
        Room baseRoom = room;

        // Act
        var result = baseRoom.ToDto();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Generic Room", result.Name);
        Assert.Equal("Unknown", result.Type);
        Assert.Null(result.NumberOfBeds);
        Assert.Null(result.NumberOfChairs);
        Assert.Equal("generic.jpg", result.image);
    }

    [Fact]
    public void ToDto_BedroomWithNoImage_ReturnsDtoWithNullImage()
    {
        // Arrange
        var bedroom = new Bedroom("Simple Room", 1);
        Room room = bedroom;

        // Act
        var result = room.ToDto();

        // Assert
        Assert.Null(result.image);
    }

    [Fact]
    public void ToDto_MeetingroomWithZeroChairs_ReturnsDtoCorrectly()
    {
        // Arrange
        var meetingroom = new Meetingroom("Standing Room", 0);
        Room room = meetingroom;

        // Act
        var result = room.ToDto();

        // Assert
        Assert.Equal(0, result.NumberOfChairs);
        Assert.Equal("Meetingroom", result.Type);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void ToDto_BedroomWithVariousBeds_MapsBedCountCorrectly(int bedCount)
    {
        // Arrange
        var bedroom = new Bedroom($"Room-{bedCount}beds", bedCount);
        Room room = bedroom;

        // Act
        var result = room.ToDto();

        // Assert
        Assert.Equal(bedCount, result.NumberOfBeds);
        Assert.Equal("Bedroom", result.Type);
    }

    private class TestRoom : Room
    {
        public TestRoom(string name) : base(name) { }
    }
}
