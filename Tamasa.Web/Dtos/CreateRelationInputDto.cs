using System;
using System.Collections.Generic;

namespace AhmadBase.Web.Dtos
{
	public class CreateRelationInputDto
	{
        public CreateRelationInputDto(string ownerId, List<string>  contactId, string relationTypeId, string location, string discription)
        {
            OwnerId = ownerId;
            ContactId = contactId;
            RelationTypeId = relationTypeId;
            Location = location;
            Discription = discription;
        }

        public string OwnerId { get; set; }
        public List<string> ContactId { get; set; }
        public string RelationTypeId { get; set; }
        public string Location { get; set; }
        public string Discription { get; set; }
    }
}

