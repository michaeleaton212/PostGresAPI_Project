using Microsoft.EntityFrameworkCore;
using PostGresAPI.Data;
using PostGresAPI.Models; 

namespace PostGresAPI.Repository;

// Data access for users
public class UserRepository
{
    private readonly ApplicationDbContext _db;
    public UserRepository(ApplicationDbContext db) => _db = db; // constructor to inject the database context

    // read all users
    public Task<List<User>> GetAll() =>
        _db.Users!
           .AsNoTracking() // improves performance for read-only queries because get dosnt need any tracking of changes that can be made to the entities.
           .OrderBy(u => u.Id)
           .ToListAsync();

    // get by id
    public Task<User?> GetById(int id) =>
        _db.Users!
           .AsNoTracking()
           .FirstOrDefaultAsync(u => u.Id == id);

    // create
    public async Task<User> Add(User user)
    {
        _db.Users!.Add(user); //! to tell compiler that Users is not null
        await _db.SaveChangesAsync(); // wait until the changes are saved to the database
        return user;
    }

    // update
    public async Task<User> Update(User user)
    {
        _db.Users!.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    // delete
    public async Task<bool> Delete(int id)
    {
        var entity = await _db.Users!.FindAsync(id);
        if (entity is null) return false;

        _db.Users!.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

}
