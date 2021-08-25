using System;
using System.Security.Claims;
using DatingApp.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.Extensions
{
    public static class ClaimsPrincipleExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }
        
        public static int GetUserId(this ClaimsPrincipal user)
        {
            // return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userId = int.TryParse(userIdClaim, out var id) ? id : 0;
            return userId;
        }
    }
}