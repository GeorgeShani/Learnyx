using AutoMapper;
using learnyx.Data;
using FluentValidation;
using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Requests;
using learnyx.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using learnyx.Authentication.Interfaces;

namespace learnyx.Authentication.Implementation;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        DataContext context, 
        IValidator<RegisterRequest> registerRequestValidator, 
        IValidator<LoginRequest> loginRequestValidator, 
        IJwtService jwtService, 
        IMapper mapper
    ) {
        _context = context;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;
        _jwtService = jwtService;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Login([FromBody] LoginRequest request)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (existingUser == null || !BCrypt.Net.BCrypt.Verify(request.Password, existingUser.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");
        
        return MapToAuthResponse(existingUser);
    }

    public async Task<AuthResponse> Register([FromBody] RegisterRequest request)
    {
        var validationResult = await _registerRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = _mapper.Map<User>(request);
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return MapToAuthResponse(user);       
    }
    
    private AuthResponse MapToAuthResponse(User user)
    {
        return new AuthResponse
        {
            Token = _jwtService.GenerateToken(user),
            User = _mapper.Map<UserDTO>(user)
        };
    }   
}