using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tamasa.Core.Dtos;
using Tamasa.Inferastracter.Datas.Entities;
using Tamasa.Web.Commands;
using Tamasa.Web.Dtos;
using Mapster;
using AhmadBase.Inferastracter.Datas.Entities;
using AhmadBase.Web.Queries;

namespace Tamasa.Web
{
    public class MapsterDtosConfigurations
    {
        static MapsterDtosConfigurations _instance;

        public static MapsterDtosConfigurations Instance =>
            _instance ?? (_instance = new MapsterDtosConfigurations());

        public void Initialize()
        {
            ConfigFor_MakeSellerShopRequestCommand_SelllerShopRequestEntity();
        }






        public void ConfigFor_MakeSellerShopRequestCommand_SelllerShopRequestEntity()
        {
            TypeAdapterConfig<ContactEntities, GetMyContactsResultDto>
                .NewConfig()
                .Map(x => x.Id, x => x.Id)
                .Map(x => x.Name, x => x.ContectName)
                .Map(x => x.Description, x => x.Description)
                .Map(x => x.PhoneNumber, x => x.ContectPhone);
        }



    }

      
}