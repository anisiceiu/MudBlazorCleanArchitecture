using Application.DTOs;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    //public class AuthService
    //{
    //    private readonly IUserRepository _userRepo;
    //    private readonly IHttpContextAccessor _httpContextAccessor;

    //    public AuthService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepo)
    //    {
    //        _httpContextAccessor = httpContextAccessor;
    //        _userRepo = userRepo;
    //    }

    //    public async Task<UserClaims?> ValidateUserAsync(string username, string password)
    //    {

    //        var user = await _userRepo.GetByUsernameAsync(username);
    //        var hashString = Encoding.UTF8.GetString(user.PasswordHash);
    //        //if (BCrypt.Net.BCrypt.Verify(password, hashString))
    //        //{
    //        //    return new UserClaims
    //        //    {
    //        //        Username = user.Username,
    //        //        Role = user.Role
    //        //    };
    //        //}

    //        //if(user != null)//&& hashString == password TODO: Remove this after implementing proper password hashing
    //        //{
    //        //    var claims = new List<Claim>
    //        //            {
    //        //                new Claim(ClaimTypes.Name, user.Username),
    //        //                new Claim(ClaimTypes.Role, "Admin")
    //        //            };
    //        //    var identity = new ClaimsIdentity(claims, "Cookies");
    //        //    var principal = new ClaimsPrincipal(identity);

    //        //    // persist identity in cookie
    //        //    try
    //        //    {
    //        //        await _httpContextAccessor.HttpContext!.SignInAsync(
    //        //    CookieAuthenticationDefaults.AuthenticationScheme,
    //        //    principal,
    //        //    new AuthenticationProperties
    //        //    {
    //        //        IsPersistent = true, // stay logged in after reload
    //        //        ExpiresUtc = DateTime.UtcNow.AddHours(8)
    //        //    });


    //        //    }
    //        //    catch (Exception ex)
    //        //    {

    //        //        throw;
    //        //    }

    //        var claims = new List<Claim>
    //                    {
    //                        new Claim(ClaimTypes.Name, user.Username),
    //                        new Claim(ClaimTypes.Role, "Admin")
    //                    };
    //        var identity = new ClaimsIdentity(claims, "Cookies");
    //        var principal = new ClaimsPrincipal(identity);
    //        await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    //        return new UserClaims
    //            {
    //                Username = user.Username,
    //                Role = "Admin" // TODO: Remove this after implementing proper role management
    //            };
    //        }

    //    public async Task LogoutAsync()
    //    {
    //        await _httpContextAccessor.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    //    }
    //}

    public class AuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepo,
            ILogger<AuthService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<UserClaims?> ValidateUserAsync(string username, string password)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Login attempt with empty username or password");
                    return null;
                }

                // Get user from repository
                var user = await _userRepo.GetByUsernameAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt for non-existent user: {Username}", username);
                    return null;
                }

                // Verify password - UNCOMMENT when you implement proper password hashing
                // var hashString = Encoding.UTF8.GetString(user.PasswordHash);
                // if (!BCrypt.Net.BCrypt.Verify(password, hashString))
                // {
                //     _logger.LogWarning("Invalid password for user: {Username}", username);
                //     return null;
                // }

                // TEMPORARY: Remove this after implementing proper password hashing
                // This is a temporary workaround - NEVER use this in production!
                //var hashString = user.PasswordHash != null ? Encoding.UTF8.GetString(user.PasswordHash) : string.Empty;
                //if (hashString != password) // Simple comparison for demo only
                //{
                //    _logger.LogWarning("Invalid password for user: {Username}", username);
                //    return null;
                //}

                // Create claims
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? "User"), // Use actual role from database
                //new Claim(ClaimTypes.NameIdentifier, user.Id?.ToString() ?? string.Empty)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Check if response has already started to avoid "Headers are read-only" error
                if (_httpContextAccessor.HttpContext?.Response.HasStarted == true)
                {
                    _logger.LogWarning("Cannot sign in - response has already started");
                    return new UserClaims
                    {
                        Username = user.Username,
                        Role = user.Role ?? "User"
                    };
                }

                // Sign in user with proper authentication properties
                await _httpContextAccessor.HttpContext!.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(8),
                        AllowRefresh = true
                    });

                _logger.LogInformation("User {Username} successfully logged in", username);

                return new UserClaims
                {
                    Username = user.Username,
                    Role = user.Role ?? "User"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user validation for {Username}", username);
                throw new ApplicationException("Authentication failed", ex);
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                if (_httpContextAccessor.HttpContext?.Response.HasStarted == false)
                {
                    await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    _logger.LogInformation("User logged out successfully");
                }
                else
                {
                    _logger.LogWarning("Cannot sign out - response has already started");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                throw;
            }
        }
    }
}
