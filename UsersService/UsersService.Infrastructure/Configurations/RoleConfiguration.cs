﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UsersService.Infrastructure.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        builder.HasData(
            new IdentityRole
            {
                Id = "1",
                Name = "User",
                NormalizedName = "USER"
            },
            new IdentityRole
            {
                Id = "2", 
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            }
        );
    }
}
