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
    public class TamasaController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IUnitOfWork<AppDbContext> unitOfWork;
        private readonly IAES AES;
        public TamasaController(IMediator mediator, ILogger<TamasaController> logger, IUnitOfWork<AppDbContext> unitOfWork, IAES aES)
        {
            _mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.AES = aES;
        }




        [HttpGet("GetUsdersInfoo")]
        [Authorize]
        public async Task<ActionResult> GetUsdersInfo(string id)
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            var contactRepo = unitOfWork.GetRepository<UserEntity>();

            var instace = unitOfWork.GetRepository<UserEntity>()
                .GetAll().Where(x => x.Id.ToString() == ownerId).First();


            var tttt = new UsersInFoDtos()
            {
                Email = instace.Email,
                Phone = instace.Phone
            };

            return Ok(tttt);
        }




        [HttpGet("UpdateMyInfoos")]
        [Authorize]
        public async Task<ActionResult> UpdateMyInfos(string Email, string Phone, string pass)
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            var contactRepo = unitOfWork.GetRepository<UserEntity>();

            var ttt = await contactRepo.GetFirstOrDefaultAsync(predicate: x => x.Id.ToString() == ownerId);

            var passWordHash = AES.Encrypt(pass);
            ttt.Email = Email;
            ttt.Phone = Phone;
            ttt.PassWordHash = passWordHash;
            ttt.PassWordSalt = passWordHash;

            contactRepo.Update(ttt);



            return Ok(ttt.Id);
        }




        [HttpGet("GetMyRelationss")]
        [Authorize]
        public async Task<ActionResult> GetMyRelationsByRelationTypeId(string typeId)
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            var relationsRepo = unitOfWork.GetRepository<RelationEntity>();
            var participandRelationsRepo = unitOfWork.GetRepository<RelationPaticipandsEntity>();
            var contactsRepo = unitOfWork.GetRepository<ContactEntities>();

            var myRelation = relationsRepo.GetAll().Where(x => x.OwnerId == ownerId && x.RelationTypeId == typeId).First();




            var participandsRelation = participandRelationsRepo.GetAll().Where(x => myRelation.Id == x.Id).Select(x => new MyRelationsByRelationTypeResult()
            {
                relationId = x.RelationId,
                ContactId = x.ContactId
            }).ToList();


            var contactIds = participandsRelation.Select(x => x.ContactId).ToList();


            var contas = contactsRepo.GetAll().Where(x => contactIds.Contains(x.Id.ToString())).ToList();

            var res = new GetMyRelationsByRelationTypeResultDtos();
            res.Location = myRelation.Location;
            res.Discription = myRelation.Discription;
            res.contats = contas.Select(x => new GetUsersInfosDtos(x.ContectName, x.ContectPhone)).ToList();

            return Ok(res);
        }


        [HttpPost("CreateRelationType")]
        [AllowAnonymous]
        public async Task<ActionResult> CreateRelationType(string realationInput)
        {
            var command = new CreateRelationTypeCommand(realationInput);
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }




        [HttpPost("InsertContaces")]
        [Authorize]
        public async Task<ActionResult> AddContact([FromBody] List<AddContactDto> input)
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new InsertToMyContactCommand(input, ownerId.ToString());
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }






        [HttpGet("GetMyContacts")]
        [Authorize]
        public async Task<ActionResult> GetMyContacts()
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new GetMyContactsQuery(ownerId.ToString());
            var result = await _mediator.Send<ServiceResult<List<GetMyContactsResultDto>>>(command);
            return await result.AsyncResult();
        }




        [HttpPost("CreateRelation")]
        [Authorize]
        public async Task<ActionResult> CreateRelation(CreateRelationInputDto input)
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
            var command = new CreateRelationCommad(input.OwnerId, input.ContactId, input.RelationTypeId, input.Location, input.Discription);
            var result = await _mediator.Send<ServiceResult<string>>(command);
            return await result.AsyncResult();
        }



        [HttpGet("GetMyRelations")]
        [Authorize]
        public async Task<ActionResult> GetMyRelations()
        {
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId");
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
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

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
            var ownerId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId").ToString();

            var contactRepo = unitOfWork.GetRepository<ContactEntities>();
            var relationTypeRepo = unitOfWork.GetRepository<RelationTypeEntity>();
            var RelationPaticipandsRepo = unitOfWork.GetRepository<RelationPaticipandsEntity>();


            sf = sf?.CheckNullDefault();
            var whereClause = sf.Translate<RelationEntity>();

            var pagedList = unitOfWork.GetRepository<RelationEntity>()
                .GetAll().Where(
                    predicate: whereClause)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            var relationIds = pagedList.Select(x => x.Id.ToString()).ToList();


            var participandsRelation = RelationPaticipandsRepo.GetAll().Where(x => relationIds.Contains(x.RelationId)).Select(x => new MyRelationsByRelationTypeResult()
            {
                relationId = x.RelationId,
                ContactId = x.ContactId
            }).ToList();

            var result = new List<GetMyRelationsQueryResultDto>();
            foreach (var item in pagedList)
            {
                var Contact = participandsRelation;
                var RelationsType = relationTypeRepo.GetFirstOrDefault(predicate: x => x.Id.ToString() == item.RelationTypeId);
                var sss = new GetMyRelationsQueryResultDto()
                {
                    OwnerId = item.OwnerId,
                    contacts = participandsRelation.Where(x => x.relationId == item.Id.ToString()).ToList(),
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
