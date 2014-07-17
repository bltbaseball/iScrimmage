using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class PlayerMapping : ClassMap<Player>
    {
        public PlayerMapping()
        {
            Table("Players");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.FirstName).Not.Nullable();
            Map(c => c.LastName).Not.Nullable();
            Map(c => c.DateOfBirth);
            Map(c => c.Email);
            Map(c => c.Gender).Not.Nullable();
            Map(c => c.JerseyNumber);
            Map(c => c.PhoneNumber);
            Map(c => c.InvitationSentOn);
            Map(c => c.InviteToken);
            Map(c => c.CreatedOn).Not.Nullable();
            Map(c => c.IsLookingForTeam);
            HasMany(c => c.Teams).Inverse().Cascade.Delete();
            References(c => c.Guardian).Nullable();
            References(c => c.User);
        }
    }
}