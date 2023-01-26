

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AhmadBase.Core;
using AhmadBase.Inferastracter.Datas.Entities;
using Mapster;
using MediatR;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Web;

namespace AhmadBase.Web.Queries
{
    //public class SearchOnMyCantactsQuery : IRequest<ServiceResult<List<GetMyContactsResultDto>>>
    //{
    //    public SearchOnMyCantactsQuery(string userId, SearchFilter sf)
    //    {
    //        this.userId = userId;
    //        this.sf = sf;
    //    }

    //    public string userId { get; set; }
    //    public SearchFilter sf { get; set; }
    //}


    //public class SearchOnMyCantactsQueryHandler : IRequestHandler<SearchOnMyCantactsQuery, ServiceResult<List<GetMyContactsResultDto>>>
    //{
    //    private readonly IUnitOfWork<AppDbContext> unitOfWork;
    //    private readonly IAES AES;
    //    public SearchOnMyCantactsQueryHandler(IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
    //    {
    //        this.unitOfWork = unitOfWork;
    //        this.AES = aES;
    //    }

    //    public Task<ServiceResult<List<GetMyContactsResultDto>>> Handle(SearchOnMyCantactsQuery request, CancellationToken cancellationToken)
    //    {
    //        //var relationTypeRepo = unitOfWork.GetRepository<RelationTypeEntity>();
    //        //var contactRepo = unitOfWork.GetRepository<ContactEntities>();
    //        //var relationRepo = unitOfWork.GetRepository<RelationEntity>();
    //        //var RES = "";

      

    //        return ServiceResult.Create<List<GetMyContactsResultDto>>(result);

    //    }

     
    //}
public static class FiltersExtensions
{
    public static T CheckNullDefault<T>(this T tf, int pgSizeDefailt = 15)
    where T : TermFilter, new()
    {
        var tmp = new T();
        if (tf is null && !(tmp is SearchFilter))
            return new TermFilter() { PgNumber = 0, PgSize = pgSizeDefailt } as T;
        if (tf.PgNumber < 0)
            tf.PgNumber = 0;
        if (tf.PgSize <= 0)
            tf.PgSize = pgSizeDefailt;

        return tf;
    }
}
}

