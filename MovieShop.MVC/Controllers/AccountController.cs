using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Models;
using ApplicationCore.ServiceInterfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MovieShop.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public AccountController(ICurrentUserService currentUserService, IUserService userService,
            IConfiguration config)
        {
            _currentUserService = currentUserService;
            _userService = userService;
            _config = config;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile profileImage)
        {
            var userProfile = new UserProfileRequestModel()
            {
                Id = _currentUserService.UserId.Value,
                FullName = _currentUserService.FullName,
                File = profileImage
            };

            var response = _userService.UploadUserProfilePicture(userProfile);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegisterRequestModel registerModel)
        {
            if (!ModelState.IsValid) return View();

            var registeredUser = await _userService.CreateUser(registerModel);

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated) return LocalRedirect("~/");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginRequestModel loginRequest, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid) return View();

            var user = await _userService.ValidateUser(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            if (user.Roles != null) claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role.Name)));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return LocalRedirect(returnUrl);
        }
    }
}