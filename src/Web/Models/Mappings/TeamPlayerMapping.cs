using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class TeamPlayerMapping : ClassMap<TeamPlayer>
    {
        public TeamPlayerMapping()
        {
            Table("TeamPlayers");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.IsPhotoVerified).Not.Nullable();
            Map(c => c.Photo);
            Map(c => c.Status);
            Map(c => c.SignWaiverId);
            Map(c => c.WaiverStatus);
            Map(c => c.JerseyNumber);
            Map(c => c.CreatedOn).Not.Nullable();

            References(c => c.Team).Not.Nullable();
            References(c => c.Player).Not.Nullable();
            //References(c => c.PhotoVerifiedBy);
            
            HasMany(c => c.PlayerGameStats).Inverse().Cascade.Delete();
        }
    }

    
}