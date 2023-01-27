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
using System.Collections.Generic;

namespace AhmadBase.Web.Commands
{
    public class CreateRelationCommad : IRequest<ServiceResult<string>>
    {

        public string OwnerId { get; set; }
        public List<string>  ContactIds { get; set; }
        public string RelationTypeId { get; set; }
        public string Location { get; set; }
        public string Discription { get; set; }

        public CreateRelationCommad(string ownerId, List<string> contactId, string relationTypeId, string location, string discription)
        {
            OwnerId = ownerId;
            ContactIds = contactId;
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
            var relatioParticipandsRepo = unitOfWork.GetRepository<RelationPaticipandsEntity>();
            var RES = "";


            var ttt = relationRepo.GetAll();
            var tdtt = relationTypeRepo.GetAll();
            var tdddtt = contactRepo.GetAll();

            var realationTypeExist = relationTypeRepo.GetAll().Where(x => x.Id.ToString() == request.RelationTypeId);
            var contactExist = contactRepo.GetAll().Where(x => x.Id.Equals(request.ContactIds));


            if (realationTypeExist == null || contactExist == null)
                return ServiceResult.Empty.SetError("This Contact Is Not Exist").To<string>();

            else
            {
                var participandsRelations = new List<RelationPaticipandsEntity>();


                var instance =  new RelationEntity(request.OwnerId, request.RelationTypeId, request.Location, request.Discription);
                relationRepo.Insert(instance);


                foreach (var item in request.ContactIds)
                    participandsRelations.Add(new RelationPaticipandsEntity(instance.Id.ToString(), item));


                relatioParticipandsRepo.Insert(participandsRelations);

                unitOfWork.SaveChanges();
                RES = instance.Id.ToString();
            }
            return ServiceResult.Create<string>(RES);
        }


    }
}




