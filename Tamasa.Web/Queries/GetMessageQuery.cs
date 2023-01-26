using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tamasa.Core.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using System.Threading.Tasks;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Inferastracter.Datas.Entities;
using Mapster;

namespace Tamasa.web.Queries
{
    public class GetMessageQuery : IRequest<ServiceResult<MessageResultDto>>
    {
        public string Id { get; set; }

        public GetMessageQuery(string id)
        {
            Id = id;
        }
    }



    public class GetMessageQueryHandler : IRequestHandler<GetMessageQuery,ServiceResult<MessageResultDto>>
    {
        private readonly IUnitOfWork<AppDbContext> unitOfWork;

        public GetMessageQueryHandler(IUnitOfWork<AppDbContext> unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<ServiceResult<MessageResultDto>> Handle(GetMessageQuery request, CancellationToken cancellationToken)
        {
            //var repo = unitOfWork.GetRepository<MessageEntity>();
            //var message = repo.GetFirstOrDefault(predicate: x => x.Id.Equals(request.Id));
            var res  =  new MessageResultDto(); 
                //message.Adapt<MessageResultDto>();
        
            return ServiceResult.Create<MessageResultDto>(res);
        }
    }
}
