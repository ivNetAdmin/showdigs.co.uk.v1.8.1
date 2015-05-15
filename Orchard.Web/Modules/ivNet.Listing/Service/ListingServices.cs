using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using Newtonsoft.Json;
using NHibernate.Mapping;
using Orchard;
using Orchard.Security;

namespace ivNet.Listing.Service
{
    public interface IListingServices : IDependency
    {
        IEnumerable<ListingDetailViewModel> GetListings(string eMail);
        IEnumerable<ListingDetailViewModel> GetAdminListings();
        IEnumerable<ListingDetailViewModel> GetPackageListings(); 
        
        IEnumerable<ListingCategoryViewModel> GetListingCategories();
        IEnumerable<ListingPackageViewModel> GetListingPackages();
        EditListingViewModel GetListing(int id);

        void AuthoriseListing(int id);
        void CancelListing(int id);        
    }

    public class ListingServices : BaseService, IListingServices
    {
        public ListingServices(IAuthenticationService authenticationService)
            : base(authenticationService)
        {
        }

        public IEnumerable<ListingDetailViewModel> GetListings(string eMail)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var ownerKey = CustomStringHelper.BuildKey(new[] { eMail });

                var listingDetailList = session.CreateCriteria(typeof (ListingDetail))
                    .List<ListingDetail>().Where(x => x.Owner.OwnerKey.Equals(ownerKey));

                return (from listingDetail in listingDetailList
                    let listingDetailViewModel = new ListingDetailViewModel()
                    select MapperHelper.Map(listingDetailViewModel, listingDetail)).ToList();

            }
        }

        public IEnumerable<ListingDetailViewModel> GetPackageListings()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetailList = session.CreateCriteria(typeof (ListingDetail))
                    .List<ListingDetail>().Where(x => x.IsVetted.Equals(0));

                return (from listingDetail in listingDetailList
                    let listingDetailViewModel = new ListingDetailViewModel()
                    select MapperHelper.Map(listingDetailViewModel, listingDetail)).ToList();

            }
        }

        public IEnumerable<ListingDetailViewModel> GetAdminListings()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetailList = session.CreateCriteria(typeof(ListingDetail))
                    .List<ListingDetail>();

                return (from listingDetail in listingDetailList
                        let listingDetailViewModel = new ListingDetailViewModel()
                        select MapperHelper.Map(listingDetailViewModel, listingDetail)).ToList();

            }
        }

        public IEnumerable<ListingCategoryViewModel> GetListingCategories()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingCategoryList = session.CreateCriteria(typeof (Category))
                    .List<Category>().OrderBy(x => x.Name);

                return (from listingCategory in listingCategoryList
                    let listingCategoryViewModel = new ListingCategoryViewModel()
                    select MapperHelper.Map(listingCategoryViewModel, listingCategory)).ToList();

            }
        }

        public IEnumerable<ListingPackageViewModel> GetListingPackages()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingPackageList = session.CreateCriteria(typeof (PaymentPackage))
                    .List<PaymentPackage>().OrderBy(x => x.Name);

                return (from listingPackage in listingPackageList
                    let listingPackageViewModel = new ListingPackageViewModel()
                    select MapperHelper.Map(listingPackageViewModel, listingPackage)).ToList();

            }
        }

        public EditListingViewModel GetListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingDetail = session.CreateCriteria(typeof (ListingDetail))
                    .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id));

                var listing = MapperHelper.Map(new EditListingViewModel(), listingDetail);

                listing.Rooms = JsonConvert.SerializeObject((from room in listingDetail.Rooms
                    let roomViewModel = new RoomViewModel()
                    select MapperHelper.Map(roomViewModel, room)).ToList());

                listing.Theatres = JsonConvert.SerializeObject((from theatre in listingDetail.Theatres
                    let theatreViewModel = new TheatreViewModel()
                    select MapperHelper.Map(theatreViewModel, theatre)).ToList());

                listing.Images = JsonConvert.SerializeObject((from image in listingDetail.Images
                    let imageViewModel = new ImageViewModel()
                    select MapperHelper.Map(imageViewModel, image)).ToList());

                listing.Tags = JsonConvert.SerializeObject((from tag in listingDetail.Tags
                    let tagViewModel = new TagViewModel()
                    select MapperHelper.Map(tagViewModel, tag)).ToList());

                listing.Package = listingDetail.PaymentPackage.Name;

                var description = new StringWriter();
                HttpUtility.HtmlDecode(listing.Description, description);
                listing.Description = description.ToString();

                return listing;
            }
        }

        public void AuthoriseListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var listingDetail = session.CreateCriteria(typeof(ListingDetail))
                   .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id));

                    if (listingDetail != null)
                    {
                        listingDetail.IsVetted = 1;
                        SetAudit(listingDetail);
                        session.SaveOrUpdate(listingDetail);

                        transaction.Commit();    
                        return;
                    }
                    transaction.Rollback();
                }
            }
        }

        public void CancelListing(int id)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var listingDetail = session.CreateCriteria(typeof(ListingDetail))
                   .List<ListingDetail>().FirstOrDefault(x => x.Id.Equals(id));

                    if (listingDetail != null)
                    {
                        listingDetail.IsVetted = 0;
                        SetAudit(listingDetail);
                        session.SaveOrUpdate(listingDetail);

                        transaction.Commit();    
                        return;
                    }
                    transaction.Rollback();
                }
            }
        }        
    }
}