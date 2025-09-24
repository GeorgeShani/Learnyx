using learnyx.Data;
using learnyx.SMTP.Templates;
using learnyx.Models.Requests;
using learnyx.SMTP.Interfaces;
using Microsoft.AspNetCore.Mvc;
using learnyx.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Controllers;

[ApiController]
[Route("api/auth/forgot-password")]
public class ForgotPasswordController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordController> _logger;

    public ForgotPasswordController(
        DataContext context,
        IEmailService emailService, 
        ILogger<ForgotPasswordController> logger
    ) {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost("send-code")]
    public async Task<IActionResult> SendVerificationCode([FromBody] SendCodeRequest request)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email && 
                     u.VerificationCode == null && 
                     u.CodeDeadline == null && 
                     u.PasswordResetToken == null && 
                     u.TokenDeadline == null
            );
            if (user == null) return Ok(new { message = "If the email exists, a verification code has been sent." });

            var code = Verification.Generate6DigitCode();
            user.VerificationCode = code;
            user.CodeDeadline = DateTime.Now.AddMinutes(10);
            
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                request.Email,
                "Password Reset Verification Code",
                EmailTemplates.GetVerificationEmailTemplate(code)
            );
            
            _logger.LogInformation("Password reset code sent to user: {UserEmail}", user.Email);
            return Ok(new { message = "Verification code sent to your email." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending verification code");
            return StatusCode(500, new { message = "An error occurred while sending the verification code." });
        }
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationCode == request.Code && u.CodeDeadline > DateTime.Now);
            if (user == null) return BadRequest(new { message = "Invalid or expired verification code." });
            
            user.VerificationCode = null;
            user.CodeDeadline = null;

            var resetToken = Verification.GenerateResetToken();
            user.PasswordResetToken = resetToken;
            user.TokenDeadline = DateTime.Now.AddMinutes(10);
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Verification code verified for user: {Email}", user.Email);

            return Ok(new { 
                message = "Code verified successfully.",
                resetToken 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying code");
            return StatusCode(500, new { message = "An error occurred while verifying the code." });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.ResetToken && u.TokenDeadline > DateTime.Now);
            if (user == null) return BadRequest(new { message = "Invalid or expired reset token." });
            if (request.Password != request.ConfirmPassword) return BadRequest(new { message = "Passwords do not match." });
            
            user.PasswordResetToken = null;
            user.TokenDeadline = null;
            
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Password reset successfully for user: {UserEmail}", user.Email);
            return Ok(new { message = "Password reset successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return StatusCode(500, new { message = "An error occurred while resetting the password." });
        }
    }
}