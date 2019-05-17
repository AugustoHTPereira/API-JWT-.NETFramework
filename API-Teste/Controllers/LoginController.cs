using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace API_Teste.Controllers
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
    }
    public class LoginController : ApiController
    {

        [AllowAnonymous]
        public IHttpActionResult Post([FromBody]User RequestUser)
        {
            if (RequestUser == null)
                return BadRequest();

            if (string.IsNullOrEmpty(RequestUser.Login) || string.IsNullOrEmpty(RequestUser.Senha))
                return BadRequest();
            var Users = new User[]{
                new User{ Id = 1, Login = "augusto", Senha = "123456"},
                new User{ Id = 2, Login = "1234", Senha = "123456"},
                new User{ Id = 3, Login = "1", Senha = "123456"}
            };
            User InstanceUsers = new User();
            RequestUser = Users.FirstOrDefault(u => u.Senha == RequestUser.Senha && u.Login == RequestUser.Login);

            if (RequestUser == null || RequestUser.Id.Equals(0))
                return ResponseMessage(new HttpResponseMessage(HttpStatusCode.Unauthorized));
            try
            {
                return Ok<string>(Token(RequestUser.Login));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        public User[] Get()
        {
            return (new User[]{
                new User{ Id = 1, Login = "augusto", Senha = "123456"},
                new User{ Id = 2, Login = "1234", Senha = "123456"},
                new User{ Id = 3, Login = "1", Senha = "123456"}
            });
        }
        private string Token(string username)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddSeconds(30);

            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, username)
            });

            const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            //create the jwt
            var token =
                (JwtSecurityToken)
                    tokenHandler.CreateJwtSecurityToken(issuer: "TESTE", audience: "TESTE",
                        subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
            return tokenHandler.WriteToken(token);
        }
    }

}
