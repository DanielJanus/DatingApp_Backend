using System;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        private readonly UserManager<AppUser> _userManager;
        public LogUserActivity(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            
            var username = resultContext.HttpContext.User.GetUsername();
            var query = _userManager.Users.Where(u => u.UserName == username).Select(a => a.Id).FirstOrDefault();
            var appUserId = query;
            
            if(!resultContext.HttpContext.User.Identity.IsAuthenticated)
                return;
            var userId = appUserId;
            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(userId);
            //user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}