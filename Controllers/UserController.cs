using System;
using System.Linq;
using HOGI.Models;
using HOGI.Models.DbModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HOGI.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly PostgresqlContext _dbContext;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, PostgresqlContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        
        [HttpPost]
        [Route("signup")]
        public IActionResult SignUp([FromBody] User user)
        {
            if(_dbContext.Users.Any(obj => obj.Email == user.Email))
            {
                return BadRequest("帳號已註冊");
            }

            try
            {
                user.Password = Common.HMACSHA256(user.Password);
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();

                return Ok(new {userId = user.Id});
            }
            catch(Exception ex)
            {
                ObjectResult result = StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                return result; 
            }
        }

        [HttpPost]
        [Route("signin")]
        public IActionResult SignIn([FromBody] User user)
        {
            User userInDB = _dbContext.Users.Where(obj => obj.Email == user.Email).FirstOrDefault();

            if(userInDB == null)
            {
                return BadRequest("尚未註冊");
            }
            else
            {
                string password = Common.HMACSHA256(user.Password);
                if(password == userInDB.Password)
                {
                    return Ok(new {userId = userInDB.Id});
                }
                else
                {
                    return BadRequest("密碼錯誤");
                }
            }
        }
    }
}