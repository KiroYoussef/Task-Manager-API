using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskAPI.Data;
using TaskAPI.DTO;
using TaskAPI.Model;

namespace TaskAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AccountController : ControllerBase
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _usermanager;
        private readonly ContextDb _contextDB;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AccountController(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> usermanager, ContextDb contextDB,
                                 IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _usermanager = usermanager;
            _contextDB = contextDB;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("register")]  // api/account/register

        public async Task<IActionResult> Registeration(RegisterUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userExists = await _usermanager.FindByNameAsync(userDto.UserName);
                    if (userExists != null)
                    {
                        return BadRequest("User Already Exists !");
                    }



                    var emailExists = await _usermanager.FindByEmailAsync(userDto.Email);
                    if (emailExists != null)
                    {
                        return BadRequest("Email Already Exists !");
                    }
                    //save

                    ApplicationUser user = new ApplicationUser();
                    user.UserName = userDto.UserName;
                    user.Email = userDto.Email;
                    user.PhoneNumber = userDto.Phone;
                    Microsoft.AspNetCore.Identity.IdentityResult result = await _usermanager.CreateAsync(user, userDto.Password);
                    if (result.Succeeded)
                    {
                        if (userDto.RoleName.ToLower() == "Admin".ToLower())
                        {

                            await _usermanager.AddToRoleAsync(user, RoleEnums.Roles.Admin.ToString());
                        }
                        else
                        {
                            await _usermanager.AddToRoleAsync(user, RoleEnums.Roles.User.ToString());

                        }
                        return Ok("Account Added Successfully");

                    }
                    else
                    {
                        foreach (var res in result.Errors)
                        {
                            return BadRequest($"Errors are : {res.Description} -- ");
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

            }
            return BadRequest(ModelState);
        }
        //check Account valid "Login" "Post"
        [HttpPost("login")] // api/account/login
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // check - create token
                    ApplicationUser user = await _usermanager.FindByEmailAsync(userDto.Email);
                    bool found = await _usermanager.CheckPasswordAsync(user, userDto.Password);

                    if (user != null && found) //username found    &&    //password is correct 
                    {
                            //Claims token
                            var claims = new List<Claim>();
                            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                            claims.Add(new Claim(ClaimTypes.Email, user.Email));
                            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                            //get role
                            var roles = await _usermanager.GetRolesAsync(user);
                            foreach (var itemRole in roles)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, itemRole));
                            }

                            SecurityKey securityKey =
                                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                            SigningCredentials signingCred =
                                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                            // Create Token
                            JwtSecurityToken mytoken = new JwtSecurityToken
                                (
                                    issuer: _configuration["JWT:ValidIssuer"], // url web api
                                    audience: _configuration["JWT:ValidAudience"], // url consumer angular
                                    claims: claims,
                                    expires: DateTime.Now.AddHours(3),
                                    signingCredentials: signingCred
                                );
                            return Ok(new
                            {
                                token = new JwtSecurityTokenHandler().WriteToken(mytoken),
                                expiration = mytoken.ValidTo,
                                role = roles.FirstOrDefault(),
                                nameUser = user.UserName,
                                id = user.Id
                            });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return Unauthorized();
        }
    }
}
