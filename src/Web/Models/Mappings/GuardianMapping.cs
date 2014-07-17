using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class GuardianMapping : ClassMap<Guardian>
    {
        public GuardianMapping()
        {
            Table("Guardians");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.FirstName);
            Map(c => c.LastName);
            Map(c => c.Email);
            Map(c => c.PhoneNumber);
            Map(c => c.Address);
            Map(c => c.City);
            Map(c => c.State);
            Map(c => c.Zip);
            Map(c => c.InvitationSentOn);
            Map(c => c.InviteToken);
            Map(c => c.CreatedOn).Not.Nullable();
            References(c => c.User);
            HasMany(c => c.Players).Cascade.None();
        }
    }
}