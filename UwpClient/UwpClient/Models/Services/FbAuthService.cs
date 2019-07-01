using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace UwpClient.Models.Services
{
    public class FbAuthService
    {
        private readonly Uri _loginUri;

        private readonly Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();

        private readonly string _facebookAppId = ""

        private readonly string _facebookPermissions = "email,user_birthday,user_gender";
        public string AccessToken { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(AccessToken);

        public FbAuthService()
        {
            var fb = new FacebookClient();

            _loginUri = fb.GetLoginUrl(new { client_id = _facebookAppId, redirect_uri = _callbackUri.AbsoluteUri, scope = _facebookPermissions, display = "popup", response_type = "token" });
        }
        public async Task<bool> SignInWithFacebook()
        {
            try
            {
                var result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _loginUri, _callbackUri); AccessToken = GetAccessToken(result);
                return true;
            }
            catch (Exception)
            { return false; }
        }

        private string GetAccessToken(WebAuthenticationResult result)
        {
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                return new FacebookClient().ParseOAuthCallbackUrl(new Uri(result.ResponseData)).AccessToken;
            }
            return null;
        }
    }
}
