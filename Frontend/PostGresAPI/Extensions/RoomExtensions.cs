using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Extensions;

public static class RoomMappingExtensions
{
    // Apply name to Room entity
    public static void ApplyName(this Room entity, string name)
        => entity.SetName(name);

    // Convert Room to RoomDto
    public static RoomDto ToDto(this Room room)
    {
        string type = room switch
        {
            Meetingroom => "Meetingroom",
            Bedroom => "Bedroom",
            _ => "Unknown"
        };

        int? numberOfBeds = room is Bedroom bedroom ? bedroom.NumberOfBeds : null;
        int? numberOfChairs = room is Meetingroom meetingroom ? meetingroom.NumberOfChairs : null;

        return new RoomDto(room.Id, room.Name, type, numberOfBeds, numberOfChairs);
    }
}
