using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class UmpireMapping : ClassMap<Umpire>
    {
        public UmpireMapping()
        {
            Table("Umpires");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Email);
            Map(c => c.FirstName);
            Map(c => c.LastName);
            Map(c => c.PhoneNumber);
            Map(c => c.Photo);
            Map(c => c.PhotoType);
            Map(c => c.CreatedOn).Not.Nullable();
            Map(c => c.InviteToken);
            Map(c => c.InvitationSentOn).Nullable();
            References(c => c.League);
            References(c => c.User);
            References(c => c.CreatedBy);

            HasMany(c => c.Games)
                .Inverse()
                .LazyLoad();
        }
    }
}