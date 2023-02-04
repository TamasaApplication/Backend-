using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tamasa.Inferastracter.Datas.Entities;

namespace AhmadBase.Inferastracter.Datas.Entities
{
	public class ContactEntities : EntityBase
    {
        public ContactEntities(string contectName, string contectPhone, string ownerId , string description) 
        {
            ContectName = contectName;
            ContectPhone = contectPhone;
            OwnerId = ownerId;
            Id = Guid.NewGuid();
            IsDeleted = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = null;
            Description = description;
        }

        public string ContectName { get; set; }
        public string ContectPhone { get; set; }
        public string OwnerId { get; set; }
        public string Description { get; set; }

    }

    public class ContactEntitiesTypeConfiguration : BaseEntityTypeConfiguration<ContactEntities>
    {

        public void Configure(EntityTypeBuilder<ContactEntities> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.Property(x => x.ContectName).IsRequired();
            builder.Property(x => x.ContectPhone).IsRequired();
            builder.Property(x => x.OwnerId).IsRequired();
            builder.Property(x => x.Description).HasDefaultValue(null);

            builder.ToTable($"tbl{nameof(ContactEntities)}");
        }
    }
}

