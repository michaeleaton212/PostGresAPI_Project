using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;

namespace PostGresAPI.Repository;

public class BedroomRepository : IBedroomRepository
{
    private readonly ApplicationDbContext _db;

    public BedroomRepository(ApplicationDbContext db) => _db = db;

    // Get all
    public Task<List<Bedroom>> GetAll() =>
        _db.Bedrooms
           .AsNoTracking()
           .OrderBy(b => b.Id)
           .ToListAsync();

    // Get by Id
    public Task<Bedroom?> GetById(int id) =>
        _db.Bedrooms
           .AsNoTracking()
           .FirstOrDefaultAsync(b => b.Id == id);

    // Create
    public async Task<Bedroom> Add(Bedroom bedroom)
    {
        _db.Bedrooms.Add(bedroom);
        await _db.SaveChangesAsync();
        return bedroom;
    }

    // Update
    public async Task<Bedroom?> Update(int id, string name, int numberOfBeds)
    {
        var entity = await _db.Bedrooms.FindAsync(id);
        if (entity is null)
            return null;

        entity.SetName(name);
        entity.NumberOfBeds = numberOfBeds;

        _db.Bedrooms.Update(entity);
        await _db.SaveChangesAsync();

        return entity;
    }

    // Check if Bedroom exists
    public Task<bool> ExistsAsync(int id) =>
        _db.Bedrooms.AnyAsync(b => b.Id == id);

    // Delete
    public async Task<bool> Delete(int id)
    {
        var entity = await _db.Bedrooms.FindAsync(id);
        if (entity is null)
            return false;

        _db.Bedrooms.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}