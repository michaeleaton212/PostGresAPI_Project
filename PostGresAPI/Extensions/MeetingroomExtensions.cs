using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Extensions;

public static class MeetingroomMappingExtensions
{
    public static MeetingroomDto ToDto(this Meetingroom room)
        => new MeetingroomDto
        {
            Id = room.Id,
            Name = room.Name,
            NumberOfChairs = room.NumberOfChairs,
            ImagePath = room.ImagePath
        };

    public static Meetingroom ToEntity(this CreateMeetingroomDto dto)
    {
        var entity = new Meetingroom(dto.Name, dto.NumberOfChairs);
        entity.SetImagePath(dto.ImagePath);
        return entity;
    }

    public static void ApplyUpdate(this Meetingroom entity, UpdateMeetingroomDto dto)
    {
        entity.SetName(dto.Name);
        entity.NumberOfChairs = dto.NumberOfChairs;
        entity.SetImagePath(dto.ImagePath);
    }
}
