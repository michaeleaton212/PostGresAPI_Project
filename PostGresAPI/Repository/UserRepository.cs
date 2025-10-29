using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models;

using PostGresAPI.Contracts;
namespace PostGresAPI.Repository;

// Data access for users
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _db;

    public UserRepository(ApplicationDbContext db) => _db = db; // constructor to inject the database context

    // read all users
    public async Task<List<User>> GetAll()
    {
        try
        {
            return await _db.Users!
                .AsNoTracking() // improves performance for read-only queries because get dosnt need any tracking of changes that can be made to the entities.
                .OrderBy(u => u.Id)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Abrufen aller Users", ex);
        }
    }

    // get by id
    public async Task<User?> GetById(int id)
    {
        try
        {
            return await _db.Users!
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Abrufen von User mit ID {id}", ex);
        }
    }

    // create
    public async Task<User> Add(CreateUserDto createUserDto) // type of parameter is CreateUserDto and create a new user based on the dto
    {
        try
        {
            var user = new User(createUserDto.UserName, createUserDto.Email);
            _db.Users!.Add(user); //! to tell compiler that Users is not null
            await _db.SaveChangesAsync(); // wait until the changes are saved to the database
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception("Fehler beim Hinzufügen des Users", ex);
        }
    }

    // update
    public async Task<User?> Update(int id, string userName, string email, string phone)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return null;

            entity.UserName = userName;
            entity.Email = email;
            entity.Phone = phone;
            _db.Users!.Update(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Aktualisieren von User mit ID {id}", ex);
        }
    }

    // delete
    public async Task<bool> Delete(int id)
    {
        try
        {
            var entity = await GetById(id);
            if (entity is null)
                return false;

            _db.Users!.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Fehler beim Löschen von User mit ID {id}", ex);
        }
    }
}