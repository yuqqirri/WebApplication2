using Microsoft.EntityFrameworkCore;
using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Infrastructure.Data;

namespace WebApplication2.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetUserAsync(LoginModel request)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Username == request.Username);
    }

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}