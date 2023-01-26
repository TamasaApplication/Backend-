using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.Dtos;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using System.Collections.Generic;
using System.Linq;
using AhmadBase.Inferastracter.Datas.Entities;

namespace AhmadBase.Web.Queries
{

    public class GetMyContactsResultDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Id { get; set; }
    }


    public class GetMyContactsQuery : IRequest<ServiceResult<List<GetMyContactsResultDto>>>
    {
        public string ContactId { get; set; }
        public GetMyContactsQuery( string contactId)
		{
            this.ContactId = contactId;
		}
	}

    public class GetMyContactsQueryHandler : IRequestHandler<GetMyContactsQuery, ServiceResult<List<GetMyContactsResultDto>>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;

        public GetMyContactsQueryHandler(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult<List<GetMyContactsResultDto>>> Handle(GetMyContactsQuery request, CancellationToken cancellationToken)
        {
            var repo = unitOfWork.GetRepository<ContactEntities>();
            var res = repo.GetAll().Where(x => x.OwnerId == request.ContactId)
                .Select(x => new GetMyContactsResultDto
                {
                    Name = x.ContectName,
                    PhoneNumber = x.ContectPhone,
                    Id = x.Id.ToString()
                }).ToList();

            return ServiceResult.Create<List<GetMyContactsResultDto>>(res);
        }
    }
}

