using System;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Inferastracter.Datas.Entities;
using Tamasa.Web;
using Tamasa.Web.Commands;
using AhmadBase.Inferastracter.Datas.Entities;
using AhmadBase.Web.Commands;

namespace AhmadBase.Web.Commands
{
    public class CreateRelationTypeCommand : IRequest<ServiceResult<string>>
    {
        public string Input { get; set; }
        public CreateRelationTypeCommand(string input)
        {
            this.Input = input;
        }
    }

    public class CreateRelationTypeCommandHandler : IRequestHandler<CreateRelationTypeCommand, ServiceResult<string>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        private readonly IAES AES;
        public CreateRelationTypeCommandHandler(IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
        {
            this.unitOfWork = unitOfWork;
            this.AES = aES;
        }

        public async Task<ServiceResult<string>> Handle(CreateRelationTypeCommand request, CancellationToken cancellationToken)
        {

            var repo = unitOfWork.GetRepository<RelationTypeEntity>();
            var RES = "";
            var realationExist = repo.GetFirstOrDefault(predicate: x => x.RelationType.Equals(request.Input));
            if(request.Input is null)
                return ServiceResult.Empty.SetError("is null").To<string>();

            if (realationExist != null)
                return ServiceResult.Empty.SetError("This relation is  Exist").To<string>();

            else
            {
                var instance = new RelationTypeEntity(request.Input);
                repo.Insert(instance);
                unitOfWork.SaveChanges();
                RES = instance.Id.ToString();
            }
            return ServiceResult.Create<string>(RES);
        }
    }


}