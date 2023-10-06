using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _dataContext;

        public BuggyController(DataContext dataContext) {
            _dataContext = dataContext;
        }
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret() {
            return "secret";
        }
        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound() {
            var user = _dataContext.Users.Find(-1);
            if(user is null) return NotFound();
            return user;
        }
        [HttpGet("server-error")]
        public ActionResult<string> GetServerError() {
            var user = _dataContext.Users.Find(-1);
            string userString = user.ToString();
            return userString;
        }
        [HttpGet("badReq")]
        public ActionResult<string> GetBadReguest() {
            return BadRequest("Not a good request");
        }
    }
}