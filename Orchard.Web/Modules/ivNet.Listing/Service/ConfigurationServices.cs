
using System.Linq;
using ivNet.Listing.Entities;
using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using Orchard;
using Orchard.Security;

namespace ivNet.Listing.Service
{
    public interface IConfigurationServices : IDependency
    {
        ConfigurationViewModel GetConfiguration();
        void Delete(int id, string type);
        Category CreateCategory(string name);
    }

    public class ConfigurationServices : BaseService, IConfigurationServices
    {
        public ConfigurationServices(IAuthenticationService authenticationService)
            : base(authenticationService)
        {
        }

        public ConfigurationViewModel GetConfiguration()
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var listingCategoryList = session.CreateCriteria(typeof (Category))
                    .List<Category>().OrderBy(x => x.Name);

                var configurationViewModel = new ConfigurationViewModel
                {
                    Categories = (from listingCategory in listingCategoryList
                        let listingCategoryViewModel = new ListingCategoryViewModel()
                        select MapperHelper.Map(listingCategoryViewModel, listingCategory)).ToList()
                };

                return configurationViewModel;
            }
        }

        public void Delete(int id, string type)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    switch (type)
                    {
                        case "category":
                            var category = session.CreateCriteria(typeof (Category))
                                .List<Category>().FirstOrDefault(x => x.Id == id);

                            session.Delete(category);
                            transaction.Commit();
                            break;

                        default:
                            transaction.Rollback();
                            break;
                    }
                }
            }
        }

        public Category CreateCategory(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var category = new Category {Name = name};

                    SetAudit(category);
                    session.SaveOrUpdate(category);
                    transaction.Commit();

                    return category;
                }
            }
        }
    }
}