using InvoiceManagementFinalProject.DTOs.User_DTOs;
using InvoiceManagementFinalProject.Models;

namespace InvoiceManagementFinalProject.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> CreateUserAsync(RegisterRequest registerRequest);
        Task<AuthResponseDto> LoginUserAsync(LoginRequest loginRequest);
        Task<AuthResponseDto> ProfileUpdateAsync(string userId, AppUser updatedUser);
        Task DeleteOwnProfileAsync(string userId);
        Task<AuthResponseDto> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task RevokeRefreshTokenAsync(string refreshToken);
    }
}