
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Location : BaseEntity
    {
        public virtual string Postcode { get; set; }
        public virtual decimal Longitude { get; set; }
        public virtual decimal Latitude { get; set; }
    }

    public class LocationMap : ClassMap<Location>
    {
        public LocationMap()
        {
            Id(x => x.Id);

            Map(x => x.Postcode).Not.Nullable().Length(12);
            Map(x => x.Longitude).Nullable().Length(50);
            Map(x => x.Latitude).Nullable().Length(50);          
        }
    }
}