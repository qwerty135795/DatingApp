using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesRepository;

        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesRepository = likesRepository;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username) {
            var sourceUserId = User.GetUserId();
            var SourceUser = await _likesRepository.GetUserWithLikes(sourceUserId);
            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            if(likedUser is null) return NotFound();
            if(SourceUser.UserName == username) return BadRequest("You cannot likes yourself");
            var userLike = await _likesRepository.GetUserLike(SourceUser.Id, likedUser.Id);
            if(userLike != null) return BadRequest();
            userLike = new UserLike { SourceUserId = SourceUser.Id, TargetUserId = likedUser.Id};
            SourceUser.LikedUsers.Add(userLike);
            if(await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest();
        }
        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams) {
            likesParams.UserId = User.GetUserId();
            var users = await _likesRepository.GetUserLikes(likesParams);
            var headers = new PaginationHeaders(users.CurrentPage,
            users.PageSize,users.TotalCount,users.TotalPage);
            Response.AddPaginationHeaders(headers);
            return Ok(users);
        }
    }
}