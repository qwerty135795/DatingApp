using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;

        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, targetUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate == "liked") {
                likes = likes.Where(d => d.SourceUserId == likesParams.UserId);
                users = likes.Select(l => l.TargetUser);
            }
            if(likesParams.Predicate == "likedBy") {
                likes = likes.Where(l => l.TargetUserId == likesParams.UserId);
                users = likes.Select(l => l.SourceUser);
            }

            var query =  users.Select(user => new LikeDTO {
                UserName = user.UserName,
                Age = user.DateOfBirth.CalculateAge(),
                KnownAs = user.KnownAs,
                City = user.City,
                Id = user.Id,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url
            });

            return await PagedList<LikeDTO>.CreateAsync(query,likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(u => u.LikedUsers)
            .SingleOrDefaultAsync(u => u.Id == userId);
        }
    }
}