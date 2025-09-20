using AutoMapper;
using learnyx.Data;
using learnyx.Models.DTOs;
using learnyx.Models.Enums;
using learnyx.Models.Entities;
using learnyx.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Services.Implementation;

public class UserService : IUserService
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserService(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        return _mapper.Map<IEnumerable<UserDTO>>(await _context.Users.ToListAsync());
    }

    public async Task<UserDTO?> GetUserByIdAsync(int id)
    {
        return _mapper.Map<UserDTO>(await _context.Users.FindAsync(id));
    }

    public async Task<UserDTO?> GetUserByEmailAsync(string email)
    {
        return _mapper.Map<UserDTO>(await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower()));
    }

    public async Task<UserDTO?> GetUserByGoogleIdAsync(string googleId)
    {
        return _mapper.Map<UserDTO>(await _context.Users
            .FirstOrDefaultAsync(u => u.GoogleId == googleId));
    }

    public async Task<UserDTO?> GetUserByFacebookIdAsync(string facebookId)
    {
        return _mapper.Map<UserDTO>(await _context.Users
            .FirstOrDefaultAsync(u => u.FacebookId == facebookId));
    }

    public async Task<UserDTO> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<UserDTO?> UpdateUserAsync(int id, User user)
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
            return null;

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.Role = user.Role;
        existingUser.Avatar = user.Avatar;
        existingUser.AuthProvider = user.AuthProvider;
        existingUser.GoogleId = user.GoogleId;
        existingUser.FacebookId = user.FacebookId;

        // Only update password if provided
        if (!string.IsNullOrEmpty(user.Password))
        {
            existingUser.Password = user.Password;
        }

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDTO>(existingUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        return await _context.Users.AnyAsync(u => u.Id == id);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<IEnumerable<UserDTO>> GetUsersByRoleAsync(UserRole role)
    {
        return _mapper.Map<IEnumerable<UserDTO>>(await _context.Users
            .Where(u => u.Role == role)
            .ToListAsync());
    }
    
    public async Task<UserDTO?> UpdateUserAvatarAsync(int userId, string avatarUrl)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return null;

        user.Avatar = avatarUrl;
        await _context.SaveChangesAsync();
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<bool> DeleteUserAvatarAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.Avatar = null;
        await _context.SaveChangesAsync();
        return true;
    }
}