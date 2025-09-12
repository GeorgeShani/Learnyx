using AutoMapper;
using FluentValidation;
using learnyx.Models.DTOs;
using learnyx.Models.Entities;
using learnyx.Models.Requests;
using learnyx.Models.Responses;
using learnyx.Repositories.Interfaces;
using learnyx.Authentication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learnyx.Authentication.Implementation;

public class AuthService : IAuthService
{
    private readonly IRepository<User> _userRepository;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        IRepository<User> userRepository, 
        IValidator<RegisterRequest> registerRequestValidator, 
        IValidator<LoginRequest> loginRequestValidator, 
        IJwtService jwtService, 
        IMapper mapper
    ) {
        _userRepository = userRepository;
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

        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email);
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
        
        await _userRepository.CreateAsync(user);
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