using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EmailService;
using Message = EmailService.Message;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.Extensions.Configuration;
using System.Web;
using API.Helpers;

namespace API.Controllers
{
    public class AccountController : BaseAPIController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IMapper mapper,
            IEmailSender emailSender,
            IConfiguration config
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _emailSender = emailSender;
            _config = config;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExist(registerDto.Username)) return BadRequest("UserName already taken.");

            var user = _mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();
            user.Email = registerDto.Email;

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string urlPath = "";
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env.ToLower() == "development")
            {
                urlPath = _config["returnPaths:ConfirmEmail"];
            }
            else
            {
                urlPath = Environment.GetEnvironmentVariable("ReturnPaths:ConfirmEmail");
            }

            var confirmationLink = URLBuilder.BuildUrl(urlPath, token, user.Id.ToString());

            var message = new Message(new string[] { user.Email }, "Confirmation Email Link",
                URLBuilder.BuildContext(URLBuilder.MessageType.ConfirmEmail, confirmationLink, user.UserName), null);
            await _emailSender.SendEmailAsync(message);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto)
        {
            if (loginDto.Username == null) return Unauthorized("Missing Username!");
            if (loginDto.Password == null) return Unauthorized("Missing Password!");

            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Username not Found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (result == Microsoft.AspNetCore.Identity.SignInResult.Failed)
            {
                return Unauthorized("Invalid Password.");
            }
            else if (result == Microsoft.AspNetCore.Identity.SignInResult.NotAllowed)
            {
                return Unauthorized("User cannot sign in without a confirmed email.");
            }

            return new UserDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(a => a.IsMain == true)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExist(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }

        [HttpPost("confirmemail")]
        public async Task<ActionResult> ConfirmEmail(ConfirmEmailDto confirmEmail)
        {
            var user = await _userManager.FindByIdAsync(confirmEmail.userid);
            if (user == null) return Unauthorized("Username not Found");

            var confirm = await _userManager.ConfirmEmailAsync(user, confirmEmail.token);
            if (confirm.Succeeded) return Ok();

            return BadRequest("Unable to confirm email");
        }

        [HttpPost("forgotpassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPassword)
        {
            if (forgotPassword.Email == null) return BadRequest("Missing Email!");

            var user = await _userManager.Users
                        .IgnoreQueryFilters()
                        .Where(e => e.Email.ToLower() == forgotPassword.Email.ToLower())
                        .FirstOrDefaultAsync();

            if (user == null) return Unauthorized("Username not Found");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            string urlPath = "";
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower() == "development")
            {
                urlPath = _config["returnPaths:PasswordChange"];
            }
            else
            {
                urlPath = Environment.GetEnvironmentVariable("ReturnPaths:PasswordChange");                
            }
            
            var changePasswordLink = URLBuilder.BuildUrl(urlPath, token, user.Id.ToString());

            var message = new Message(new string[] { user.Email }, "Reset Password link", 
                URLBuilder.BuildContext(URLBuilder.MessageType.ResetPassword, changePasswordLink,user.UserName), null);
            await _emailSender.SendEmailAsync(message);

            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPassword)
        {
            var user = await _userManager.FindByIdAsync(resetPassword.UserId);
            if (user == null) return Unauthorized("Username not Found");

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password);
            if (result.Succeeded) return Ok();

            return BadRequest("Unable to reset password");
        }
    }
}
