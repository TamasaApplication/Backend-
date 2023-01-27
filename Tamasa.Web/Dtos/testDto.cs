using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tamasa.Web.Dtos
{
    public class testDto
    {
        public string Mail { get; set; }
        public string PassWord { get; set; }

    }





    public class MyRelationsByRelationTypeResult
    {
        public string relationId { get; set; }
        public string ContactId { get; set; }
    }


    public class GetUsersInfosDtos
    {
        public GetUsersInfosDtos(string contactName, string contactPhone)
        {
            ContactName = contactName;
            ContactPhone = contactPhone;
        }

        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
    }
    public class GetMyRelationsByRelationTypeResultDtos
    {
        public string Location { get; set; }
        public string Discription { get; set; }
        public List<GetUsersInfosDtos> contats { get; set; }
    }
}
