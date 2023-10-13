using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(ITokenService tokenService,
        IMapper mapper, UserManager<AppUser> userManager)
        {
           
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.UserName)) return BadRequest("Username is taken");
            var user = _mapper.Map<AppUser>(registerDTO);

            user.UserName = registerDTO.UserName.ToLower();
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);
            var roleResult = await _userManager.AddToRoleAsync(user,"Member");
            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.Users.Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.UserName == loginDTO.UserName.ToLower());
            if (user == null ) return Unauthorized("invalid username");
            if(!await _userManager.CheckPasswordAsync(user,loginDTO.Password)) return Unauthorized();
            return new UserDTO
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}