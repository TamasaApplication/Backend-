using System;
using AhmadBase.Inferastracter.Datas.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Web;
using AhmadBase.Web.Dtos;
using System.Collections.Generic;

namespace AhmadBase.Web.Commands
{
    public class InsertToMyContactCommand : IRequest<ServiceResult<string>>
    {
        public List<AddContactDto> ContetS { get; set; }
        public string ownerId { get; set; }
        public InsertToMyContactCommand(List<AddContactDto> ContetS,string ownerId )
        {
            this.ContetS = ContetS;
            this.ownerId = ownerId;
        }
    }

    public class InsertToMyContactCommandHandler : IRequestHandler<InsertToMyContactCommand, ServiceResult<string>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        private readonly IAES AES;
        public InsertToMyContactCommandHandler(IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
        {
            this.unitOfWork = unitOfWork;
            this.AES = aES;
        }

        public async Task<ServiceResult<string>> Handle(InsertToMyContactCommand request, CancellationToken cancellationToken)
        {

            var repo = unitOfWork.GetRepository<ContactEntities>();
            var RES = "";



            if (request.ContetS == null)
                return ServiceResult.Empty.SetError("This User Is Not Exist").To<string>();

            var inserInstances = new List<ContactEntities>();

            foreach (var item in request.ContetS)
            {
                var instance = new ContactEntities(item.ContactName, item.ContactPhone, request.ownerId, item.Description);
                inserInstances.Add(instance);
            }

            await repo.InsertAsync(inserInstances);
            unitOfWork.SaveChanges();
            var res = inserInstances[0].Id.ToString();
            return ServiceResult.Create<string>(res);
        }
    }
}

