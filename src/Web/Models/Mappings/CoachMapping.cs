using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class CoachMapping : ClassMap<Coach>
    {
        public CoachMapping()
        {
            Table("Coaches");
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
            References(c => c.User);
            References(c => c.CreatedBy);

            HasManyToMany(c => c.Teams)
                .Table("TeamCoaches")
                .Inverse()
                .LazyLoad();
        }
    }
}