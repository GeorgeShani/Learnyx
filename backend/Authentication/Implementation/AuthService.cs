using AutoMapper;
using learnyx.Data;
using FluentValidation;
using learnyx.Models.DTOs;
using System.Security.Claims;
using learnyx.Models.Entities;
using learnyx.Models.Requests;
using learnyx.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using learnyx.Authentication.Interfaces;
using learnyx.SMTP.Interfaces;
using learnyx.SMTP.Templates;

namespace learnyx.Authentication.Implementation;

public class AuthService : IAuthService
{
    private readonly DataContext _context;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        DataContext context, 
        IValidator<RegisterRequest> registerRequestValidator, 
        IValidator<LoginRequest> loginRequestValidator, 
        IHttpContextAccessor httpContextAccessor,
        IEmailService emailService,
        IJwtService jwtService, 
        IMapper mapper
    ) {
        _context = context;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
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

        await _emailService.SendEmailAsync(
            user.Email,
            "Welcome to Learnyx!",
            EmailTemplates.GetWelcomeEmailTemplate(user.FirstName)       
        );
        
        return MapToAuthResponse(user);       
    }

    public async Task<UserDTO?> GetAuthenticatedUserAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return null;
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var userDto = _mapper.Map<UserDTO>(user);
        return userDto;
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