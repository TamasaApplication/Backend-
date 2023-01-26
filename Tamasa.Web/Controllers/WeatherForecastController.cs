using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tamasa.Core.Dtos;
using Tamasa.Core.interfere.IReposetory;
using Tamasa.Core.Types;
using Tamasa.Inferastracter;
using Tamasa.Inferastracter.Datas.Entities;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Tamasa.Core.Dtos;
using Tamasa.Web.Commands;
using Tamasa.Web.Dtos;
using Tamasa.web.Queries;
using Microsoft.IdentityModel.Tokens;
using AhmadBase.Web.Commands;
using AhmadBase.Web.Queries;
using AhmadBase.Web.Dtos;
using Microsoft.CodeAnalysis;
using AhmadBase.Core;
using AhmadBase.Inferastracter.Datas.Entities;
using Tamasa.Core.Logic;

namespace Tamasa.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        public WeatherForecastController(IMediator mediator, ILogger<WeatherForecastController> logger, IUnitOfWork<AppDbContext> unitOfWork)
        {
            _mediator = mediator;
            this.unitOfWork = unitOfWork;
        }




        //[HttpGet]
        //public async Task<ServiceResult<MessageResultDto>> GetMessageById(string id)
        //{
        //    var query = new GetMessageQuery(id);
        //    var result = await _mediator.Send(query);
        //    return result;
        //}


        //[HttpPost]
        //[Authorize]
        //public async Task<IActionResult> GetUserIdsFromToken()
        //{
        //    var usrId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
        //    return Ok(usrId.ToString());
        //}




        [HttpPost("CreateRelationType")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateRelationType(string realationInput)
        {
            var command = new CreateRelationTypeCommand(realationInput);
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }




        [HttpPost("InsertContaces")]
       /// [Authorize]
        public async Task<ActionResult> AddContact([FromBody] List<AddContactDto> input)
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95";
            //HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new InsertToMyContactCommand(input,ownerId.ToString());
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }






        [HttpGet("GetMyContacts")]
      //  [Authorize]
        public async Task<ActionResult> GetMyContacts()
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95";
            ///HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new GetMyContactsQuery(ownerId.ToString());
            var result = await _mediator.Send<ServiceResult<List<GetMyContactsResultDto>>>(command);
            return await result.AsyncResult();
        }




        [HttpPost("CreateRelation")]
      ///  [Authorize]
        public async Task<ActionResult> CreateRelation(CreateRelationInputDto input)
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95";
            ///HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new CreateRelationCommad(input.OwnerId, input.ContactId,input.RelationTypeId, input.Location, input.Discription);
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }



        [HttpGet("GetMyRelations")]
      ///  [Authorize]
        public async Task<ActionResult> GetMyRelations()
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95"; 
            ///HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new GetMyRelationsQuery(ownerId.ToString());
            var result = await _mediator.Send<ServiceResult<List<GetMyRelationsQueryResultDto>>>(command);
            return await result.AsyncResult();
        }





        [HttpGet("GetSRelationType")]
        [AllowAnonymous]
        public async Task<ActionResult> GetRelationTypes()
        {
            var command = new GetRelationTypesQuery();
            var result = await _mediator.Send<ServiceResult<List<GetRelationDtos>>>(command);
            return await result.AsyncResult();
        }



        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register([FromBody] RegisterInputDto input)
        {
            var command = input.Adapt<RegisterCommand>();
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }


        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginDto input)
        {
            var command = input.Adapt<LoginCommand>();
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }




        [HttpGet("SearchOnMyCantacts")]
        [AllowAnonymous]
        public async Task<ActionResult> SearchOnMyCantacts([FromBody] SearchFilter sf)
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95";
                ///HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            sf = sf?.CheckNullDefault();
            var whereClause = sf.Translate<ContactEntities>();

            var pagedList = unitOfWork.GetRepository<ContactEntities>()
                .GetAll().Where(
                    predicate: whereClause)
                .OrderByDescending(x => x.CreatedAt).ToList();

            var res = pagedList.Where(x => x.OwnerId == ownerId).ToList();

            var result = res.Adapt<List<GetMyContactsResultDto>>();
            return Ok(result);
        }





        [HttpGet("SearchOnRElationTypes")]
        [AllowAnonymous]
        public async Task<ActionResult> SearchOnRElationTypes([FromBody] SearchFilter sf)
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95";
            //HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            sf = sf?.CheckNullDefault();
            var whereClause = sf.Translate<RelationTypeEntity>();

            var pagedList = unitOfWork.GetRepository<RelationTypeEntity>()
                .GetAll().Where(
                    predicate: whereClause)
                .OrderByDescending(x => x.CreatedAt).Select(x => x.RelationType)
                .ToList();


            var result = pagedList.Adapt<List<string>>();
            return Ok(result);
        }





        [HttpGet("SearchOnRElation")]
        [AllowAnonymous]
        public async Task<ActionResult> SearchOnRElation([FromBody] SearchFilter sf)
        {
            var ownerId = "45078241-b18a-48b2-b99f-c92cdc110d95";
            //HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            var contactRepo = unitOfWork.GetRepository<ContactEntities>();
            var relationTypeRepo = unitOfWork.GetRepository<RelationTypeEntity>();


            sf = sf?.CheckNullDefault();
            var whereClause = sf.Translate<RelationEntity>();

            var pagedList = unitOfWork.GetRepository<RelationEntity>()
                .GetAll().Where(
                    predicate: whereClause)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var result = new List<GetMyRelationsQueryResultDto>();
            foreach (var item in pagedList)
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



            return Ok(result);
        }





    }
}
