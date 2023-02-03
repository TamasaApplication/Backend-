using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Inferastracter.Datas.Entities;

namespace Tamasa.Web.Commands
{
    public class LoginCommand : IRequest<ServiceResult<string>>
    {
        public string Email { get; set; }
        public string PassWord { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, ServiceResult<string>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        private readonly IAES AES;
        public LoginCommandHandler(IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
        {
            this.unitOfWork = unitOfWork;
            this.AES = aES;
        }

        public async Task<ServiceResult<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var repo = unitOfWork.GetRepository<UserEntity>();

            var userExist = repo.GetFirstOrDefault(predicate: x => x.Email.Equals(request.Email));

             
            if (userExist == null)
                return ServiceResult.Empty.SetError("This User does  Not Exist").To<string>();



            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("TamasaTamasaTamasaTamasaTamasaTamasa1234");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim("userId", userExist.Id.ToString()),
                }),

                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1gfbfghdfghfghgfh ghghhfghdfhfghdgfhfghghdgfhdgfh23*/"));


            //var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);



            //var token = new JwtSecurityToken("Ahmad.Com", "Ahmad.Com", expires: DateTime.Today.AddDays(5),
            //    claims: new List<Claim>()
            //    {
            //        new Claim("userId",userExist.Id.ToString()),
            //        new Claim("role","Admin")
            //    }, signingCredentials: cred);



            return ServiceResult.Create<string>(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
