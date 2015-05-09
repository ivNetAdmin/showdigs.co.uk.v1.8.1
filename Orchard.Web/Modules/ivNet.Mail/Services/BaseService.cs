
using Orchard.Security;

namespace ivNet.Mail.Services
{
    public class BaseService
    {
        protected readonly IUser CurrentUser;

        public BaseService(IAuthenticationService authenticationService)
        {
            CurrentUser = authenticationService.GetAuthenticatedUser();
        }
    }
}