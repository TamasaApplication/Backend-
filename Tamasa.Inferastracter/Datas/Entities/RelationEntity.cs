using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamasa.Inferastracter.Datas.Entities;

namespace AhmadBase.Inferastracter.Datas.Entities
{
	public class RelationEntity : EntityBase
    {
        public RelationEntity(string ownerId, string contactId, string relationTypeId, string location, string discription)
        {
            Id = Guid.NewGuid();
            IsDeleted = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = null;
            OwnerId = ownerId;
            ContactId = contactId;
            RelationTypeId = relationTypeId;
            Location = location;
            Discription = discription;
        }

        public string  OwnerId{ get; set; }
        public string  ContactId{ get; set; }
        public string  RelationTypeId { get; set; }
        public string  Location{ get; set; }
        public string  Discription { get; set; }

    }

    public class RelationEntityTypeConfiguration : BaseEntityTypeConfiguration<RelationEntity>
    {

        public void Configure(EntityTypeBuilder<RelationEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.OwnerId).IsRequired();
            builder.Property(x => x.ContactId).IsRequired();
            builder.Property(x => x.RelationTypeId).IsRequired();
            builder.Property(x => x.Location).IsRequired();
            builder.Property(x => x.Discription).IsRequired();

            builder.ToTable($"tbl{nameof(RelationEntity)}");
        }
    }
}

