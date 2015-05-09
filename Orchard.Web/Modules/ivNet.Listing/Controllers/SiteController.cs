
using System;
using System.Configuration;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ivNet.Listing.Models;
using ivNet.Listing.Service;
using ivNet.Mail.Helpers;
using ivNet.Mail.Services;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;
using Orchard.Users.Events;

namespace ivNet.Listing.Controllers
{
    public class SiteController : BaseController
    {
        private readonly IRegistrationServices _registrationServices;
        private readonly IEmailServices _emailServices;

        private readonly IAuthenticationService _authenticationService;    
        private readonly IUserEventHandler _userEventHandler;

        public SiteController(
            IRegistrationServices registrationServices,
            IAuthenticationService authenticationService,
            IUserEventHandler userEventHandler,
            IEmailServices emailServices)
        {
            _registrationServices = registrationServices;
            _emailServices = emailServices;
            _authenticationService = authenticationService;       
            _userEventHandler = userEventHandler;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult Registration()
        {                      
            return View("Registration/Index");
        }

        [HttpPost]
        [Themed]
        public ActionResult Registration(RegistrationViewModel model)
        {
            if (!ModelState.IsValid) return View("Registration/Index", model);

            try
            {
                _registrationServices.UpdateOwner(model);
           
                var registrationSubject = ConfigurationManager.AppSettings.Get("RegistrationSubject");

                _emailServices.SendActivationEmail(model.Email, model.Password, registrationSubject);
               
                const string returnUrl = "/owner/registration-activation";               

                return Redirect(returnUrl);
               
            }
            catch (ArgumentException)
            {
                const string returnUrl = "/owner/registration-duplicate";
                return Redirect(returnUrl);               
            }
            catch
            {
                const string returnUrl = "/owner/registration-failure";
                return Redirect(returnUrl);       
            }
        }

        [Themed]
        public ActionResult Activation(string id)
        {
            Session["OwnerId"] = id;
            var ownerId = QueryStringHelper.Base64ForUrlDecode(HttpUtility.UrlDecode(id));
            var idParts = ownerId.Split('|');

            var activationViewModel = new ActivationViewModel()
            {
                Email = idParts[0],
                Message = "Please confirm your eMail address and password"
            };

            return View("Activation/Index",activationViewModel);
        }

        [HttpPost]
        [Themed]
        public ActionResult Activation(ActivationViewModel model)
        {
            if (!ModelState.IsValid) return View("Activation/Index", model);

            var id = Session["OwnerId"].ToString();
            var ownerId = QueryStringHelper.Base64ForUrlDecode(HttpUtility.UrlDecode(id));

            if (ownerId == string.Format("{0}|{1}", model.Email, model.Password).ToLowerInvariant())
            {
                try
                {
                    const string returnUrl = "/owner/my-listings";                

                    var user = _registrationServices.CreateOwnerUser(model);
                    _registrationServices.UpdateOwnerUserId(model.Email, user.Id);                    

                    _authenticationService.SignIn(user, false);
                    _userEventHandler.LoggedIn(user);
               
                    return Redirect(returnUrl);
                  
                }
                catch
                {
                    const string returnUrl = "/owner/activation-failure";
                    return Redirect(returnUrl);    
                }

            }

            var activationViewModel = new ActivationViewModel()
            {
                Email = model.Email,
                Message = "Your password does not match the registration password, activation failed."
            };

            return View("Activation/Index", activationViewModel);

        }
      
    }
}