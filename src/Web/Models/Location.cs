using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SqlServer.Types;
using NetTopologySuite.Geometries;

namespace Web.Models
{
    /// <summary>
    /// Location for an event.
    /// </summary>
    public class Location
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Address { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual double Latitude { get; set; }
        public virtual double Longitude { get; set; }
        public virtual string Url { get; set; }
        public virtual string Notes { get; set; }
        public virtual DateTime CreatedOn { get; set; }
        public virtual string GroundsKeeperPhone { get; set; }
        //public virtual Point Point { get; set; }

        public static IList<Location> GetAllLocations()
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.QueryOver<Web.Models.Location>().List();
        }

        public static Location GetLocationById(int id)
        {
            var session = MvcApplication.SessionFactory.GetCurrentSession();
            return session.Get<Location>(id);
        }

        //public static IList<LocationDistance> GetLocationsWithinDistanceOfPoint(Point point, int miles)
        //{
        //    var session = MvcApplication.SessionFactory.GetCurrentSession();
        //    var query = session.CreateSQLQuery("SELECT {Location.*}, Point.STDistance(:Location) AS Distance FROM Locations Location WHERE Point.STDistance(:Location) < :Dist");

        //    GeoAPI.Geometries.IGeometry geom = point as GeoAPI.Geometries.IGeometry;
        //    var builder = new SqlGeographyBuilder();
        //    builder.SetSrid(4326);
        //    builder.BeginGeography(OpenGisGeographyType.Point);
        //    builder.BeginFigure(point.Y, point.X);
        //    builder.EndFigure();
        //    builder.EndGeography();

        //    query.AddEntity("Location", typeof(Location));
        //    query.SetParameter("Location", builder.ConstructedGeography, new NHibernate.Spatial.Type.SqlGeographyType());
        //    query.SetParameter("Dist", miles * 1609.34);
        //    query.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<LocationDistance>());
        //    return query.List<LocationDistance>();            
        //}
    }

    public class LocationDistance
    {
        public Location Location { get; set; }
        public double Distance { get; set; }
    }
}