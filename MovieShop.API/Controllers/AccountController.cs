using System.Threading.Tasks;
using ApplicationCore.Models;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MovieShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;

        public AccountController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        [HttpGet]
        [Route("{id:int}", Name = "GetUser")]
        public async Task<ActionResult> GetUserByIdAsync(int id)
        {
            var user = await _userService.GetUserDetails(id);
            return Ok(user);
        }

        /// <summary>
        ///     Creates a new User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>A newly created User</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the validation fails </response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> RegisterUserAsync([FromBody] UserRegisterRequestModel user)
        {
            var createdUser = await _userService.CreateUser(user);
            return CreatedAtRoute("GetUser", new { id = createdUser.Id }, createdUser);
        }

        [HttpGet]
        [Route("checkemail")]
        public async Task<ActionResult> EmailExists([FromQuery] string email)
        {
            var user = await _userService.GetUser(email);
            return Ok(user == null ? new { emailExists = false } : new { emailExists = true });
        }

        /// <summary>
        ///     Account Login
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns>Json Web Token(JWT) is correct email/password</returns>
        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequestModel loginRequest)
        {
            //   "email": "ab@example.com",
            //   "password": "Abhi1234!!"
            //    Roles Admin, SuperAdmin

            /* Abc123!!
             * 	"email": "abhilash@abhilash.com",
	            "password" : "Abc1234!!!",
            {
                "email": "Bill6@gmail.com",
                "password": "abc1234!"
            }
           */

            var user = await _userService.ValidateUser(loginRequest.Email, loginRequest.Password);
            if (user == null) return Unauthorized();

            return Ok(new { token = _jwtService.GenerateToken(user) });
        }
    }
}