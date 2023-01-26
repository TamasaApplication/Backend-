using System;
namespace AhmadBase.Web.Dtos
{
	public class CreateRelationInputDto
	{
        public CreateRelationInputDto(string ownerId, string contactId, string relationTypeId, string location, string discription)
        {
            OwnerId = ownerId;
            ContactId = contactId;
            RelationTypeId = relationTypeId;
            Location = location;
            Discription = discription;
        }

        public string OwnerId { get; set; }
        public string ContactId { get; set; }
        public string RelationTypeId { get; set; }
        public string Location { get; set; }
        public string Discription { get; set; }
    }
}

