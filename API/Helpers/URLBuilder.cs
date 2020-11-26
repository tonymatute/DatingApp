using System;
using System.Globalization;
using System.Web;

namespace API.Helpers
{
    public static class URLBuilder
    {
        public enum MessageType
        {
            ConfirmEmail = 0,
            ResetPassword = 1            
        };

        public static string BuildUrl(string urlPath, string token, string userId)
        {
            var uriBuilder = new UriBuilder(urlPath);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["token"] = token;
            query["userid"] = userId;
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
        public static string BuildContext(MessageType messageType, string link, string username)
        {
           
            var usernameToTittleCase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(username);
            string contextMessage = "";
            switch (messageType)
            {
                case MessageType.ConfirmEmail:
                    contextMessage = string.Format("<font size=4 color=black>Dear {0}<br/><br/>We are Happy you register with us. <br />" +
                        "Please click on the link below or copy the link to your browser to confirm your email address.<br />" +
                        "This confirm email link is only valid for the next 24 hours.</font><br/><br/>", usernameToTittleCase) + link;
                        break;
                case MessageType.ResetPassword:
                    contextMessage = string.Format("<font size=4 color=black>Dear {0}<br /> <br />You recently request to reset your password account. <br />" +
                        "Please click on the link below or copy the link to your browser to reset it. <br />" +
                        "This password reset link is only valid for the next 24 hours.</font> <br /> <br />", usernameToTittleCase) + link;
                    break;
            }

            return contextMessage;
        }
    }
}
