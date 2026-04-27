using Microsoft.EntityFrameworkCore;
using WebApplication2.Domain.Models;
using WebApplication2.Infrastructure.Data;

namespace WebApplication2.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context)
{
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User> GetUser(LoginModel loginModel)
    {
        return await context.Users.Where(x => x.Username == loginModel.Username).FirstOrDefaultAsync();
    }
}
