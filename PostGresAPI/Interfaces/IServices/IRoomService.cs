using System.Collections.Generic;
using System.Threading.Tasks;
using PostGresAPI.Contracts;

namespace PostGresAPI.Services;

public interface IRoomService
{
    // Read filter and mapping logic lies here in the service
    Task<List<RoomDto>> GetAll(string? type = null);
    Task<RoomDto?> GetById(int id);

    // Create 
    Task<RoomDto> CreateMeetingroom(CreateMeetingroomDto createMeetingroomDto);
    Task<RoomDto> CreateBedroom(CreateBedroomDto createBedroomDto);

    // Update
    Task<RoomDto?> UpdateName(int id, string name);

    // Delete
    Task<bool> Delete(int id);
}

// The interface tells my service what it has to offer