using System;
using AhmadBase.Inferastracter.Datas.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Web;
using System.Linq;

namespace AhmadBase.Web.Commands
{
    public class CreateRelationCommad : IRequest<ServiceResult<string>>
    {

        public string OwnerId { get; set; }
        public string ContactId { get; set; }
        public string RelationTypeId { get; set; }
        public string Location { get; set; }
        public string Discription { get; set; }

        public CreateRelationCommad(string ownerId, string contactId, string relationTypeId, string location, string discription)
        {
            OwnerId = ownerId;
            ContactId = contactId;
            RelationTypeId = relationTypeId;
            Location = location;
            Discription = discription;
        }
    }

    public class CreateRelationCommandHandler : IRequestHandler<CreateRelationCommad, ServiceResult<string>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        private readonly IAES AES;
        public CreateRelationCommandHandler(IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
        {
            this.unitOfWork = unitOfWork;
            this.AES = aES;
        }

        public async Task<ServiceResult<string>> Handle(CreateRelationCommad request, CancellationToken cancellationToken)
        {

            var relationTypeRepo = unitOfWork.GetRepository<RelationTypeEntity>();
            var contactRepo = unitOfWork.GetRepository<ContactEntities>();
            var relationRepo = unitOfWork.GetRepository<RelationEntity>();
            var RES = "";


            var ttt = relationRepo.GetAll();
            var tdtt = relationTypeRepo.GetAll();
            var tdddtt = contactRepo.GetAll();

            var realationTypeExist = relationTypeRepo.GetAll().Where(x => x.Id.ToString() == request.RelationTypeId);
            var contactExist = contactRepo.GetAll().Where(x => x.Id.Equals(request.ContactId));


            if (realationTypeExist == null || contactExist == null)
                return ServiceResult.Empty.SetError("This Contact Is Not Exist").To<string>();

            else
            {
                var instance =
                    new RelationEntity(request.OwnerId, request.ContactId, request.RelationTypeId, request.Location, request.Discription);
                relationRepo.Insert(instance);
                unitOfWork.SaveChanges();
                RES = instance.Id.ToString();
            }
            return ServiceResult.Create<string>(RES);
        }


    }
}




