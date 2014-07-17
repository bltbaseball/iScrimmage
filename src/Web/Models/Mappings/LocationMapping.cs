using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentNHibernate.Mapping;

namespace Web.Models.Mappings
{
    public class LocationMapping : ClassMap<Location>
    {
        public LocationMapping()
        {
            //ImportType<NetTopologySuite.Geometries.Point>();

            Table("Locations");
            Id(c => c.Id).Column("Id").GeneratedBy.Native();
            Map(c => c.Name);
            Map(c => c.Address);
            Map(c => c.City);
            Map(c => c.State);
            Map(c => c.Zip);
            Map(c => c.Url);
            Map(c => c.Notes);
            Map(c => c.Latitude);
            Map(c => c.Longitude);
            Map(c => c.CreatedOn).Not.Nullable();
            Map(c => c.GroundsKeeperPhone);
            //Map(c => c.Point).CustomType<Web.Helpers.Wgs84GeographyType>();
        }
    }
}