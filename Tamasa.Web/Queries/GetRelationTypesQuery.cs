using System;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.Dtos;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.web.Queries;
using System.Collections.Generic;
using AhmadBase.Inferastracter.Datas.Entities;
using System.Linq;

namespace AhmadBase.Web.Queries
{

    public class GetRelationDtos
    {
        public string  Id { get; set; }
        public string Relations { get; set; }
    }

	public class GetRelationTypesQuery : IRequest<ServiceResult<List<GetRelationDtos>>>
    {
		public GetRelationTypesQuery()
		{
		}
    }



public class GetRelationTypesQueryHandler : IRequestHandler<GetRelationTypesQuery, ServiceResult<List<GetRelationDtos>>>
{
    private readonly IUnitOfWork<AppDbContext> unitOfWork;

    public GetRelationTypesQueryHandler(IUnitOfWork<AppDbContext> unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }
    public async Task<ServiceResult<List<GetRelationDtos>>> Handle(GetRelationTypesQuery request, CancellationToken cancellationToken)
    {
        var repo = unitOfWork.GetRepository<RelationTypeEntity>();
        var result = repo.GetAll().ToList().Select(x => new GetRelationDtos {
            Relations = x.RelationType,
            Id = x.Id.ToString()
        }).ToList();

        return ServiceResult.Create<List<GetRelationDtos>>(result);
    }
}
}

