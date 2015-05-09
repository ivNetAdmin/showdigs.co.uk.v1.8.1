using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Room : BaseEntity
    {
        public virtual string Type { get; set; }
        public virtual string Description { get; set; }
        public virtual ListingDetail ListingDetail { get; set; }
    }

    public class RoomMap : ClassMap<Room>
    {
        public RoomMap()
        {
            Id(x => x.Id);

            Map(x => x.Type).Not.Nullable().Length(50);
            Map(x => x.Description).Not.Nullable().Length(2000);

            References(x => x.ListingDetail);
        }
    }
}