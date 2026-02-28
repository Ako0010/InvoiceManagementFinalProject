using InvoiceManagementFinalProject.Common;
using InvoiceManagementFinalProject.DTOs.User_DTOs;
using InvoiceManagementFinalProject.Models;
using InvoiceManagementFinalProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.Security.Claims;

namespace InvoiceManagementFinalProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService userService)
    {
        _authService = userService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequest registerRequest)
    {
        Console.WriteLine($"Email: '{registerRequest?.Email}'");

        var result = await _authService.CreateUserAsync(registerRequest);
        if (result == null)
        {
            return BadRequest("Registration failed.");
        }
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequest loginRequest)
    {
        var result = await _authService.LoginUserAsync(loginRequest);
        if (result == null)
        {
            return Unauthorized("Invalid email or password.");
        }
        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
    }

    [Authorize(Policy = "User")]
    [HttpPut("me/profile")]
    public async Task<ActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _authService.ProfileUpdateAsync(userId, new AppUser
        {
            Name = req.Name,
            Address = req.Address,
            PhoneNumber = req.PhoneNumber
        });

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result,"Profile changed successfully"));
    }


    [HttpPut("me/change-password")]
    public async Task<ActionResult> ChangeMyPassword([FromBody] ChangePasswordRequest req)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _authService.ChangePasswordAsync(userId, req.CurrentPassword, req.NewPassword);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Password changed successfully."));
    }


    [HttpDelete("me/delete")]
    public async Task<IActionResult> DeleteMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        await _authService.DeleteOwnProfileAsync(userId);
        return NoContent();
    }
}