using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
    public interface ILikesRepository
    {
        public Task<UserLike> GetUserLike(int sourceUserId,int TargetUserId);
        public Task<AppUser> GetUserWithLikes(int userId);
        public Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams);
    }
}