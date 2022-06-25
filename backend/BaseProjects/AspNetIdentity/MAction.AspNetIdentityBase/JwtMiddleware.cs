using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAction.AspNetIdentity.Base
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;


        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var handler = new JwtSecurityTokenHandler();
            string authHeader = context.Request.Headers["Authorization"];
            if (!String.IsNullOrEmpty(authHeader))
            {
                if (authHeader.StartsWith("Bearer "))
                {
                    authHeader = authHeader.Replace("Bearer ", "");
                    var jsonToken = handler.ReadToken(authHeader);
                    var tokens = handler.ReadToken(authHeader) as JwtSecurityToken;
                    context.Items["UserId"] = tokens.Claims.First(claim => claim.Type == "sub").Value;
                }
            }
            await _next(context);
        }
    }
}
