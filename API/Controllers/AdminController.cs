using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles() {
            var users =  await _userManager.Users.OrderBy(u => u.UserName)
            .Select(u => new {
                Id = u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList() 
            }).ToListAsync();
            return Ok(users);
        }
        [Authorize(Policy = "RequiredAdminRole")]
        [HttpPost("edit-roles/{username}")]
        public async Task<ActionResult> AddRole(string username, [FromQuery]string roles) {
            if(string.IsNullOrEmpty(roles)) return BadRequest("Parameter roles not found");
            var selectedRoles = roles.Split(",").ToArray();
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            var rolesUser = await _userManager.GetRolesAsync(user);
            if(user is null) return NotFound();

            var result = await _userManager.AddToRolesAsync(user,selectedRoles.Except(rolesUser));
            if(!result.Succeeded) BadRequest("Failed to added role");
            result = await _userManager.RemoveFromRolesAsync(user,rolesUser.Except(selectedRoles));
            if(!result.Succeeded) return BadRequest("Failed to remove role");
            return Ok(await _userManager.GetRolesAsync(user));

        }
        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration() {
            return Ok("only admin and moderator can see that");
        }
    }
}