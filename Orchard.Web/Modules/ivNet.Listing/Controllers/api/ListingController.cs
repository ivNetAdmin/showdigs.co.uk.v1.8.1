﻿
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.Listing.Service;
using Orchard;
using Orchard.Logging;

namespace ivNet.Listing.Controllers.api
{
    public class ListingController : ApiController
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IListingServices _listingServices;

        public ListingController(
            IOrchardServices orchardServices,
            IListingServices listingServices
            )
        {
            _orchardServices = orchardServices;
            _listingServices = listingServices;
            Logger = NullLogger.Instance;            
        }

        public ILogger Logger { get; set; }

        public HttpResponseMessage Get()
        {

            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);        
            
            var eMail = _orchardServices.WorkContext.CurrentUser.Email;

            return Request.CreateResponse(HttpStatusCode.OK,
               _listingServices.GetListings(eMail));
        }

        public HttpResponseMessage Get(int id)
        {

            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab))
                return Request.CreateResponse(HttpStatusCode.Forbidden);          

            return Request.CreateResponse(HttpStatusCode.OK,
               _listingServices.GetListing(id));
        }      
    }
}