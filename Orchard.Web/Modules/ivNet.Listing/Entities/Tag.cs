
using FluentNHibernate.Mapping;

namespace ivNet.Listing.Entities
{
    public class Tag : BaseEntity
    {
        public virtual string Name { get; set; }
        public virtual ListingDetail ListingDetail { get; set; }
      
    }

    public class TagMap : ClassMap<Tag>
    {
        public TagMap()
        {
            Id(x => x.Id);

            Map(x => x.Name).Not.Nullable().Length(50);

            References(x => x.ListingDetail);
        }
    }
}