using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Numerics;
using Tamasa.Inferastracter.Datas.Entities;
using Microsoft.EntityFrameworkCore;

namespace AhmadBase.Inferastracter.Datas.Entities
{
	public class RelationTypeEntity : EntityBase
    {
        public RelationTypeEntity(string relationType)
        {
            Id = Guid.NewGuid();
            IsDeleted = false;
            CreatedAt = DateTime.Now;
            UpdatedAt = null;
            RelationType = relationType;
        }

        public string RelationType { get; set; }
}

public class RelationTypeEntityTypeConfiguration : BaseEntityTypeConfiguration<RelationTypeEntity>
{

    public void Configure(EntityTypeBuilder<RelationTypeEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.IsDeleted).HasDefaultValue(false);
        builder.Property(x => x.RelationType).IsRequired();

        builder.ToTable($"tbl{nameof(RelationTypeEntity)}");
    }
}
}

