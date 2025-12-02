using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Extensions;

public static class RoomMappingExtensions
{
    public static RoomDto ToDto(this Room room)
    {
        return room switch
        {
            Bedroom b => new RoomDto
            {
                Id = b.Id,
                Name = b.Name,
                Type = "Bedroom",
                NumberOfBeds = b.NumberOfBeds,
                image = b.ImagePath
            },

            Meetingroom m => new RoomDto
            {
                Id = m.Id,
                Name = m.Name,
                Type = "Meetingroom",
                NumberOfChairs = m.NumberOfChairs,
                image = m.ImagePath
            },

            _ => new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Type = "Unknown",
                image = room.ImagePath
            }
        };
    }
}
