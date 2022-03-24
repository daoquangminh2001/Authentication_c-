using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
namespace JSONWebToken.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        
        private readonly IConfiguration _configuration;
        private readonly IUserSevices _IUserService;
        public AuthController (IConfiguration configuration, IUserSevices userService)
        {
            _configuration = configuration;
            _IUserService = userService;
        }
        [HttpGet,Authorize]   
        public ActionResult<string> Get()
        {
            var userName = _IUserService.GetName();
            return Ok(userName);
        }
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.UserName = request.UserName;  
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            
            return Ok(user);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        [HttpPost("Login")]

        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            if(user.UserName != request.UserName)
            {
                return BadRequest("đéo đăng nhập được");
            }
            if(!VerifiPassHash  (request.Password, user.PasswordHash,user.PasswordSalt))
            {
                return BadRequest("sai con mẹ m mật khẩu r");
            }
            string token = CreateToken(user);
            return Ok(token);
        }
        //aaaaaaaaaaaaaaaaaaa
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                //new Claim(ClaimTypes.Role, "Admin")
                new Claim(ClaimTypes.Role, (user.UserName!="Minh"?"Noob":"Admin"))
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        private bool VerifiPassHash(string password,byte[] passwordHash,byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var check = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return check.SequenceEqual(passwordHash);
            }
        }
    }
}
