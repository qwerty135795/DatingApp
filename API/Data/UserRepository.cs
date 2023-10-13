using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public UserRepository(DataContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users.Where(u => 
            u.UserName == username).ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams param)
        {
            var query =  _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != param.CurrentUsername);
            query = query.Where(u => u.Gender == param.Gender);
            DateOnly minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-param.MaxAge - 1));
            DateOnly maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-param.MinAge));
            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            query = param.OrderBy switch {
                "created" => query.OrderByDescending(u => u.Created),
                "lastActive" => query.OrderByDescending(u => u.LastActive),
                _ => throw new Exception()
            };
            return await PagedList<MemberDTO>.CreateAsync(query.AsNoTracking()
            .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),
            param.PageNumber,param.PageSize);
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
           return await _context.Users.Include(u => u.Photos).FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(u => u.Photos).ToListAsync(); 
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}