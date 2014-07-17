using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    /// <summary>
    /// Location for an event.
    /// </summary>
    public class LocationNewModel
    {
        [Required]
        [DisplayName("Location Name")]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Zip { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }
        
        [DisplayName("Phone Number")]
        public string GroundsKeeperPhone { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
        
        public DateTime CreatedOn { get; set; }
    }

    public class LocationUpdateModel : LocationNewModel
    {
        public int Id { get; set; }
    }

    public class LocationCreateResult : AjaxResponse
    {
        public int LocationId { get; set; }
        public IList<Location> Locations { get; set; }
    }
}