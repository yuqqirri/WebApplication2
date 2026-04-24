using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Repositories;

public class UserRepository(ApplicationDbContext context)
{
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }
}
