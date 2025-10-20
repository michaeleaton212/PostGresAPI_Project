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
    => new(room.Id, room.Name, room.Type);

}
