using ivNet.Listing.Helpers;
using ivNet.Listing.Models;
using ivNet.Listing.Service;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ivNet.Listing.Controllers
{
    public class SecureSiteController : BaseController
    {
        private readonly IOrchardServices _orchardServices;
        //private readonly IAuthenticationService _authenticationService;
        private readonly IOwnerServices _ownerServices;
        
        public SecureSiteController(
            IOrchardServices orchardServices, 
            IAuthenticationService authenticationService,
            IOwnerServices ownerServices)
        {
            _orchardServices = orchardServices;
            _ownerServices = ownerServices;
            //_authenticationService = authenticationService;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            CurrentUser = authenticationService.GetAuthenticatedUser();
        }

        private IUser CurrentUser { get; set; }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [HttpPost]
        public ActionResult AddListing(EditListingViewModel model)
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/");

            var ownerKey = CustomStringHelper.BuildKey(new[] { CurrentUser.Email });
            var ownerId = _ownerServices.GetOwnerIdByKey(ownerKey);
       
            _ownerServices.AddListing(ownerId, model);
            
            const string returnUrl = "/owner/my-listings";
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ContentResult UploadFiles()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivOwnerTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/");
            
            var r = new List<UploadFilesResult>();

            var ownerKey = CustomStringHelper.BuildKey(new[] { CurrentUser.Email });
            var ownerId = _ownerServices.GetOwnerIdByKey(ownerKey);
            var filePath = Server.MapPath(string.Format("~/Media/Default/ListingImages/{0}", ownerId));                  

            foreach (string file in Request.Files)
            {
                var hpf = Request.Files[file] as HttpPostedFileBase;

                if (hpf == null || hpf.ContentLength == 0)
                    continue;

                var now = DateTime.Now;
                var fileName = string.Format("{0}{1}{2}{3}{4}{5}.jpg",
                now.Year, now.Month, now.Day,
                now.Hour, now.Minute, now.Second);

                ImageHelper.UploadImage(hpf, fileName, filePath, 690, 300);
                filePath = filePath + "/thumbnails";
                ImageHelper.UploadImage(hpf, fileName, filePath, 60, 55);

                //var fileName = ImageHelper.UploadOwnerImage(hpf, filePath);
                //ImageHelper.UploadImageThumbnail(hpf, fileName, filePath);

                r.Add(new UploadFilesResult()
                {
                    Name = string.Format("/Media/Default/ListingImages/{0}/thumbnails/{1}",ownerId,fileName),
                    Length = hpf.ContentLength,
                    Type = hpf.ContentType
                });
            }

            // Returns json
            return Content("{\"name\":\"" + r[0].Name + "\",\"type\":\"" + r[0].Type + "\",\"size\":\"" + string.Format("{0} bytes", r[0].Length) + "\"}", "application/json");
        }
    }
}