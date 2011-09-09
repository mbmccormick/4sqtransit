using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Scope;
using System.Globalization;
using System.Net;
using System.Web.Helpers;

public class Foursquare
{
    private const string InitializedExceptionMessage = "The Foursquare Helper has not been initialized. You should call the Initialize method.";
    private const string ArgumentNullExceptionMessage = "Argument cannot be null or empty.";
    private const string AuthenticationCodeMissingMessage = "Authentication Code is missing.";
    private const string DefaultCallbackPage = "register.aspx";    

    private static readonly object _consumerKey = new object();
    private static readonly object _consumerSecretKey = new object();
    private static readonly object _callbackUrlKey = new object();
    private static readonly object _initializedKey = new object();

    public static string ConsumerKey {
        get {
            if (!Initialized) {
                throw new InvalidOperationException(InitializedExceptionMessage);
            }
            return (string)(ScopeStorage.CurrentScope[_consumerKey] ?? "");
        }
        private set {
            if (value == null) {
                throw new ArgumentNullException("ConsumerKey");
            }
            ScopeStorage.CurrentScope[_consumerKey] = value;
        }
    }

    public static string ConsumerSecret {
        get {
            if (!Initialized) {
                throw new InvalidOperationException(InitializedExceptionMessage);
            }            
            return (string)(ScopeStorage.CurrentScope[_consumerSecretKey] ?? "");
        }
        private set {
            if (value == null) {
                throw new ArgumentNullException("ConsumerSecret");
            }
            ScopeStorage.CurrentScope[_consumerSecretKey] = value;
        }
    }

    public static string CallbackUrl {
        get {
            if (!Initialized) {
                throw new InvalidOperationException(InitializedExceptionMessage);                
            }
            return (string)(ScopeStorage.CurrentScope[_callbackUrlKey] ?? "");
        }
        private set {
            if (value == null) {
                throw new ArgumentNullException("CallbackUrl");
            }
            ScopeStorage.CurrentScope[_callbackUrlKey] = value;
        }
    }
    
    private static bool Initialized {
        get {
            return (bool)(ScopeStorage.CurrentScope[_initializedKey] ?? false);
        }
        set {
            ScopeStorage.CurrentScope[_initializedKey] = value;
        }
    }

    /// <summary>
    ///  Initializes the Foursquare helper with the consumer key and secret values. Calling
    ///  this method is not required if you only want to show an "Add to my Foursquare" button.
    /// </summary>    
    /// <param name="consumerKey">The Foursquare API consumer key.</param>
    /// <param name="consumerSecret">The Foursquare API consumer secret.</param>
    /// <param name="callbackUrl">The absolute callback url specified in the Consumer Registration Page. By default, the helper uses [root]/Foursquare/RequestToken.</param>
    public static void Initialize(string consumerKey, string consumerSecret, string callbackUrl = "") {
        if (consumerKey == string.Empty) {
            throw new ArgumentException(ArgumentNullExceptionMessage, "consumerKey");
        }

        if (consumerSecret == string.Empty) {
            throw new ArgumentException(ArgumentNullExceptionMessage, "consumerSecret");
        }       
        
        ConsumerKey = consumerKey;
        ConsumerSecret = consumerSecret;
        CallbackUrl = callbackUrl;

        Initialized = true;
    }

    /// <summary>
    /// Retrieves the Foursquare login Url, for your users to login to Foursquare throught your site.
    /// </summary>
    /// <param name="returnUrl">The url where the user is redirected to after login.</param>
    public static string GetAuthorizationUrl(string returnUrl = "") {        
        var url = "https://foursquare.com/oauth2/authenticate?client_id={0}&response_type=code&redirect_uri={1}";
        string link = string.Format(CultureInfo.InvariantCulture, url, ConsumerKey, System.Web.HttpContext.Current.Server.UrlEncode(GetCallbackUrl(returnUrl)));

        return link;
    }

    /// <summary>
    /// Once the user has logged into Foursquare, this method returns the user's AccessToken.
    /// </summary>    
    public static string GetAccessToken() {        
        var code = HttpContext.Current.Request["code"];
        if (code == string.Empty) {
            var errorMessage = AuthenticationCodeMissingMessage;
            var error = HttpContext.Current.Request["error"];
            if (error != string.Empty) {
                errorMessage += " (error: " + error + ")";
            }
            
            throw new InvalidOperationException(errorMessage);
        }
        
        var url = "https://foursquare.com/oauth2/access_token?client_id={0}&client_secret={1}&grant_type=authorization_code&redirect_uri={2}&code={3}";

        var accessTokenRequest = string.Format(url, ConsumerKey, ConsumerSecret, System.Web.HttpContext.Current.Server.UrlEncode(GetCallbackUrl()), code);
        
        var client = new WebClient();        
        var jsonResult = client.DownloadString(accessTokenRequest);
        var result = Json.Decode(jsonResult);

        return result.access_token;
    }

    /// <summary>
    /// Returns a list of friends
    /// </summary>
    /// <param name="accessToken">The access token of the authenticating user.</param>
    /// <param name="userId">The Id of the person for whom to pull a friend graph. if not specified, the authenticating user's list of friends will be returned.</param>
    public static IList<dynamic> GetFriends(string accessToken, int userId = 0) {
        var url = "https://api.foursquare.com/v2/users/{0}/friends?oauth_token={1}";

        var client = new WebClient();
        var jsonResult = client.DownloadString(string.Format(url, userId == 0 ? "self" : userId.ToString(), accessToken));
        var result = Json.Decode(jsonResult);
        
        return new List<dynamic>(result.response.friends.items);
    }

    /// <summary>
    /// Returns profile information (badges, etc.) for a given user.
    /// </summary>
    /// <param name="accessToken">The access token of the authenticating user.</param>
    /// <param name="userId">The Id of the user</param>
    public static dynamic GetUser(string accessToken, int userId = 0) {
        var url = "https://api.foursquare.com/v2/users/{0}?oauth_token={1}";
        
        var client = new WebClient();
        var jsonResult = client.DownloadString(string.Format(url, userId == 0 ? "self" : userId.ToString(), accessToken));
        var result = Json.Decode(jsonResult);

        return result.response.user;
    }
    
    /// <summary>
    /// Returns badges for a given user.
    /// </summary>
    /// <param name="accessToken">The access token of the authenticating user</param>
    /// <param name="userId">The Id of the user</param>
    public static IList<dynamic> GetBadges(string accessToken, int userId = 0) {
        var url = "https://api.foursquare.com/v2/users/{0}/badges?oauth_token={1}";
        
        var client = new WebClient();
        var jsonResult = client.DownloadString(string.Format(url, userId == 0 ? "self" : userId.ToString(), accessToken));
        var result = Json.Decode(jsonResult);
        
        var badges = new List<dynamic>();
		var groups = new List<dynamic>(result.response.sets.groups);
        var allBadgesGroup = groups.FirstOrDefault(g => g.type == "all");
		if (allBadgesGroup != null) {
			foreach(var badgeId in allBadgesGroup.items) {
				badges.Add(result.response.badges[badgeId]);
			}
		}
        
        return badges;
    }
    
    private static string GetCallbackUrl(string returnUrl = "") {           
        var callbackUrl = CallbackUrl;        
        if (callbackUrl == string.Empty) {
            var hostUrl = "";
            if (System.Web.HttpContext.Current.Request.Url.GetComponents(UriComponents.Host, UriFormat.Unescaped) == "localhost")
                hostUrl = System.Web.HttpContext.Current.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
            else
                hostUrl = System.Web.HttpContext.Current.Request.Url.GetComponents(UriComponents.Scheme, UriFormat.Unescaped) + "://" + System.Web.HttpContext.Current.Request.Url.GetComponents(UriComponents.Host, UriFormat.Unescaped);
            callbackUrl = hostUrl + System.Web.HttpContext.Current.Request.ApplicationPath + DefaultCallbackPage;
        }
        
        if (returnUrl != string.Empty) {
            if ((new Uri(callbackUrl)).Query == string.Empty) {
                callbackUrl = callbackUrl + "?returnUrl=" + System.Web.HttpContext.Current.Server.UrlEncode(returnUrl);
            } else {
                callbackUrl = callbackUrl + "&returnUrl=" + System.Web.HttpContext.Current.Server.UrlEncode(returnUrl);
            }
        }
        
        return callbackUrl;
    }
}
