using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedLibrary.Dtos;
using SharedLibrary.Models;
using WebApplication.Components.Database;
using WebApplication.Components.Models;
using WebApplication.Components.Services;
using WebApplication.Components.Services.Interfaces;
using WebApplication.Components.Validation;
using static WebApplication.Components.Models.FacebookApiResponse;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private readonly IHostingEnvironment _environment;
        public AuthController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            ITokenService tokenService,
            IHostingEnvironment environment
            )
        {
            _userManager = userManager ?? throw new NullReferenceException();
            _dbContext = dbContext ?? throw new NullReferenceException();
            _tokenService = tokenService ?? throw new NullReferenceException();
            _environment = environment ?? throw new NullReferenceException();
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is null)
                return Ok(new AuthDto { Succeeded = false, Error = "We cannot find an account with that email address" });

            var res = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!res)
                return Ok(new AuthDto { Succeeded = false, Error = "Your password is incorrect" });

            var claims = await _userManager.GetClaimsAsync(user);
            var token = _tokenService.GenerateToken(claims, 60);
            return Ok(new AuthDto { AuthToken = token, Succeeded = true });
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var val = new RegisterValidatior();
            var valRes = await val.ValidateAsync(dto);
            if (!valRes.IsValid)
                return Ok(new AuthDto { Succeeded = false, Error = valRes.Errors.First().ErrorMessage });
           
            var user = new ApplicationUser()
            {
                Email = dto.Email,
                UserName = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Name = dto.Name,
                Gender = dto.Gender
            };

            //var res = await _userManager.CreateAsync(user, dto.Password);
            //if (!res.Succeeded)
            //    return Ok(new AuthDto { Succeeded = false, Error = res.Errors.First().Description });

            user.MinAge = 18;
            user.MaxAge = 99;
            user.MaxDist = 50;
            if (user.Gender == SharedLibrary.Models.Gender.Female)
                user.SearchedGender = SharedLibrary.Models.Gender.Male;
            else
                user.SearchedGender = SharedLibrary.Models.Gender.Female;

            user.Longitude = 0.0;
            user.Latitude = 0.0;

            var res = await _userManager.CreateAsync(user, dto.Password);
            if (!res.Succeeded)
                return Ok(new AuthDto { Succeeded = false, Error = res.Errors.First().Description });

            await _userManager.AddClaimAsync(user, new Claim("UserId", user.Id));

            var uploads = Path.Combine(_environment.WebRootPath, "uploads", user.Id);
            Directory.CreateDirectory(uploads);

            var claims = await _userManager.GetClaimsAsync(user);
            var token = _tokenService.GenerateToken(claims,60);

            return Ok(new AuthDto { AuthToken = token, Succeeded = true });
        }          

        [HttpPost("[action]")]
        public IActionResult ValidateToken()
        {
            return Ok();
        }
    }


    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class FacebookAuthController : Controller
    {
        private readonly FbService _fbService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITokenService _tokenService;
        private HttpClient httpClient { get; set; }
        public FacebookAuthController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext,
            FbService fbService,
            ITokenService tokenService)
        {
            _fbService = fbService ?? throw new NullReferenceException();
            _userManager = userManager ?? throw new NullReferenceException();
            _dbContext = dbContext ?? throw new NullReferenceException();
            _tokenService = tokenService ?? throw new NullReferenceException();
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public async Task<IActionResult> SignInWithFacebook([FromQuery] string authToken)
        {
            try
            {
                if (authToken is null)
                    return Ok(new AuthDto { Succeeded = false, Error = "Token is null" });
                using (httpClient = new HttpClient())
                {
                    var appAccessTokenResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={_fbService.AppId}&client_secret={_fbService.AppSecret}&grant_type=client_credentials");
                    var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
                    var userAccessTokenValidationResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={appAccessToken.AccessToken}");
                    var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);
                    if (!userAccessTokenValidation.Data.IsValid)
                    {
                        return BadRequest(new AuthDto { Succeeded = false, Error = "Token is invalid" });
                    }
                    var userInfoResponse = await httpClient.GetStringAsync($"https://graph.facebook.com/v{_fbService.AppVersion}/me?fields=id,email,first_name,last_name,name,gender,locale,birthday,picture&access_token={authToken}");
                    var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);
                    var user = await _userManager.FindByEmailAsync(userInfo.Email);
                    if (user is null)
                    {
                        user = new ApplicationUser()
                        {
                            Email = userInfo.Email,
                            UserName = userInfo.Email,
                            DateOfBirth = DateTime.Parse(userInfo.Birthday),
                            Name = userInfo.FirstName
                        };
                        if (userInfo.Gender == "male")
                            user.Gender = Gender.Male;
                        else
                            user.Gender = Gender.Female;

                        user.MinAge = 18;
                        user.MaxAge = 99;
                        user.MaxDist = 50;
                        user.Longitude = 0.0;
                        user.Latitude = 0.0;

                        var res = await _userManager.CreateAsync(user);
                        if (!res.Succeeded)
                            return BadRequest(new AuthDto { Succeeded = false, Error = res.Errors.First().Description });

                        await _userManager.AddClaimAsync(user, new Claim("UserId", user.Id));
                    }
                    var claims = await _userManager.GetClaimsAsync(user);

                    var token = _tokenService.GenerateToken(claims, 60);

                    return Ok(new AuthDto { AuthToken = token, Succeeded = true });
                }
            }
            catch(Exception e)
            {
                return Ok(new AuthDto { Error = e.Message, Succeeded = false });
            }
        }
    }
}
