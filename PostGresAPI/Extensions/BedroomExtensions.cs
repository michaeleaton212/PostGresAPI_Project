using PostGresAPI.Models;
using PostGresAPI.Contracts;

namespace PostGresAPI.Extensions;

public static class BedroomMappingExtensions
{
    public static BedroomDto ToDto(this Bedroom room)
        => new BedroomDto
        {
            Id = room.Id,
            Name = room.Name,
            NumberOfBeds = room.NumberOfBeds,
            ImagePath = room.ImagePath,
            PricePerNight = room.PricePerNight
        };

    public static Bedroom ToEntity(this CreateBedroomDto dto)
    {
        var entity = new Bedroom(dto.Name, dto.NumberOfBeds);
        entity.SetImagePath(dto.ImagePath);
        entity.PricePerNight = dto.PricePerNight;
        return entity;
    }

    public static void ApplyUpdate(this Bedroom entity, UpdateBedroomDto dto)
    {
        entity.SetName(dto.Name);
        entity.NumberOfBeds = dto.NumberOfBeds;
        entity.SetImagePath(dto.ImagePath);
        entity.PricePerNight = dto.PricePerNight;
    }
}

//extension files are for mapping between entities and DTOs