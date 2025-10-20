using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;
using PostGresAPI.Contracts;
using PostGresAPI.Extensions;

namespace PostGresAPI.Repository;

public class BedroomRepository : IBedroomRepository
{
    private readonly ApplicationDbContext _db;

    public BedroomRepository(ApplicationDbContext db) => _db = db; // constructor to inject the database context

    // Get all
    public async Task<List<Bedroom>> GetAll()
    {
        try
        {
            return await _db.Bedrooms
                .AsNoTracking()
                .OrderBy(b => b.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Bedrooms", ex);
        }
    }

    // Get by Id
    public async Task<Bedroom?> GetById(int id)
    {
        try
        {
            return await _db.Bedrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Abrufen von Bedroom mit ID {id}", ex);
        }
    }

    // Create
    public async Task<Bedroom> Add(CreateBedroomDto createBedroomDto)
    {
        try
        {
            var bedroom = createBedroomDto.ToEntity();
            _db.Bedrooms.Add(bedroom); // <— statt: Add(CreateBedroomDto createBedroomDto)
            await _db.SaveChangesAsync();
            return bedroom;
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Hinzufügen des Bedrooms", ex);
        }
    }


    // Update
    public async Task<Bedroom?> Update(int id, string name, int numberOfBeds)
    {
        try
        {
            var entity = await _db.Bedrooms.FindAsync(id);
            if (entity is null)
                return null;

            entity.ApplyUpdate(name, numberOfBeds);
            _db.Bedrooms.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Aktualisieren von Bedroom mit ID {id}", ex);
        }
    }

    // Check if Bedroom exists
    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _db.Bedrooms.AnyAsync(b => b.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Prüfen der Existenz von Bedroom mit ID {id}", ex);
        }
    }

    // Delete
    public async Task<bool> Delete(int id)
    {
        try
        {
            var entity = await _db.Bedrooms.FindAsync(id);
            if (entity is null)
                return false;

            _db.Bedrooms.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Löschen von Bedroom mit ID {id}", ex);
        }
    }
}