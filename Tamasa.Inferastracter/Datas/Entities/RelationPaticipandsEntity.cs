using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamasa.Inferastracter.Datas.Entities;

namespace AhmadBase.Inferastracter.Datas.Entities
{
	public class RelationPaticipandsEntity : EntityBase
    {
        public RelationPaticipandsEntity()
        {

        }
		public RelationPaticipandsEntity(string relationId , string contact)
		{
            Id = Guid.NewGuid();
            IsDeleted = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = null;
            this.RelationId = relationId;
            this.ContactId = contact;
        }


        public string  RelationId { get; set; }
        public string ContactId { get; set; }

    }

    public class RelationPaticipandsTypeConfiguration : BaseEntityTypeConfiguration<RelationPaticipandsEntity>
    {

        public void Configure(EntityTypeBuilder<RelationPaticipandsEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.ContactId).IsRequired();

            builder.ToTable($"tbl{nameof(RelationPaticipandsEntity)}");
        }
    }
}

