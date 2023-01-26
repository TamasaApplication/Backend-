using System;
using AhmadBase.Inferastracter.Datas.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Web;
using System.Collections.Generic;
using System.Linq;

namespace AhmadBase.Web.Queries
{
    public class GetMyRelationsQueryResultDto
    {


        public string OwnerId { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string RelationTypeName { get; set; }
        public string Location { get; set; }
        public string Discription { get; set; }
    }


    public class GetMyRelationsQuery : IRequest<ServiceResult<List<GetMyRelationsQueryResultDto>>>
    {
        public GetMyRelationsQuery(string ownerId)
        {
            this.OwnerId = ownerId;
        }

        public string OwnerId { get; set; }
    }


    public class GetMyRelationsQueryHandler : IRequestHandler<GetMyRelationsQuery, ServiceResult<List<GetMyRelationsQueryResultDto>>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        private readonly IAES AES;
        public GetMyRelationsQueryHandler(IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
        {
            this.unitOfWork = unitOfWork;
            this.AES = aES;
        }

        public async Task<ServiceResult<List<GetMyRelationsQueryResultDto>>> Handle(GetMyRelationsQuery request, CancellationToken cancellationToken)
        {
            var relationTypeRepo = unitOfWork.GetRepository<RelationTypeEntity>();
            var contactRepo = unitOfWork.GetRepository<ContactEntities>();
            var relationRepo = unitOfWork.GetRepository<RelationEntity>();
            var RES = "";

            var relations = relationRepo.GetAll().Where(x => x.OwnerId == request.OwnerId).ToList();
            var result = new List<GetMyRelationsQueryResultDto>();
            foreach (var item in relations)
            {
                var Contact = contactRepo.GetFirstOrDefault(predicate: x => x.Id.ToString() == item.ContactId);
                var RelationsType = relationTypeRepo.GetFirstOrDefault(predicate: x => x.Id.ToString() == item.RelationTypeId);
                var sss = new GetMyRelationsQueryResultDto()
                {
                    OwnerId = item.OwnerId,
                    ContactName = Contact.ContectName,
                    ContactPhone = Contact.ContectPhone,
                    RelationTypeName = RelationsType.RelationType,
                    Location = item.Location,
                    Discription = item.Discription
                };
                result.Add(sss);
            }


            return ServiceResult.Create<List<GetMyRelationsQueryResultDto>>(result);
        }

    }
}

