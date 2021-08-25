using System.Collections.Generic;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Data;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;
        private readonly DataContext _context;
        
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository, DataContext context)
        {
            _likesRepository = likesRepository;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var SourceUserName = User.GetUsername();
            var query = _context.Users.Where(u => u.UserName == SourceUserName).Select(a => a.Id).FirstOrDefault();
            var sourceUserId = query;
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null)
                return NotFound();

            if (sourceUser.UserName == username)
                return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null)
                return BadRequest("You already liked this user");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };
            
            sourceUser.LikedUsers.Add(userLike);
            if (await _userRepository.SaveAllAsync())
                return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            var likesParamsUsername = User.GetUsername();
            var query = _context.Users.Where(u => u.UserName == likesParamsUsername).Select(a => a.Id).FirstOrDefault();
            likesParams.UserId = query;
            var users = await _likesRepository.GetUserLikes(likesParams);
            
            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            
            return Ok(users);
        }
    }
}