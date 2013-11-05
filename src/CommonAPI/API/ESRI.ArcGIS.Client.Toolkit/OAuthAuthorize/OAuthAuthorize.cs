// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

#if SILVERLIGHT
using System.Json;
using System.Windows.Browser;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
#endif

namespace ESRI.ArcGIS.Client.Toolkit
{
#if SILVERLIGHT
	/// <summary>
	/// Manages the Silverlight OAuth authorization process.
	/// Instantiate an instance of this class to initialize the <see cref="IdentityManager.ServerInfo.OAuthClientInfo"/> OAuthAuthorize property.
	/// </summary>
	public class OAuthAuthorize : IOAuthAuthorize
	{
		private string _authorizeUrl;
		private TaskCompletionSource<IDictionary<string, string>> _tcs;
		private string _portalUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="OAuthAuthorize"/> class.
		/// </summary>
		public OAuthAuthorize()
		{
			// allow to come back from JS by using: slCtl.Content.OAuthAuthorize.SetResult(hash)
			HtmlPage.RegisterScriptableObject("OAuthAuthorize", this);
		}

		/// <summary>
		/// Redirects the user to the authorization URL.
		/// </summary>
		/// <param name="url">The service URL.</param>
		/// <param name="authorizeUrl">The authorize URL.</param>
		/// <param name="callbackUrl">The callback URL.</param>
		/// <returns>Dictionary of parameters returned by the authorization URL (code, access_token, refresh_token, ...)</returns>
		public Task<IDictionary<string, string>> AuthorizeAsync(string url, string authorizeUrl, string callbackUrl)
		{
			_authorizeUrl = authorizeUrl;
			_portalUrl = url;
			_tcs = new TaskCompletionSource<IDictionary<string, string>>();
			try
			{
				OpenAuthorizeWindow();
			}
			catch (Exception e)
			{
				_tcs.TrySetException(e);
			}
			return _tcs.Task;
		}

		/// <summary>
		/// Initializes the Silverlight application for getting authorization from an OAuth authorization page.
		/// Calling this method is optional but offers the following advantages: 
		/// <list type="bullet">
		/// <item>Initializes a default IdentityManager challenge method that manages the single sign in to the portal.</item>
		/// <item>Restores the portal OAuth credential that may have been persisted during a previous session (using the 'Keep Me Signed In' option).</item>
		/// <item>Injects in the main HTML page the OAuth scripts that manage the answers of the OAuth authorization server.</item>
		/// </list>
		/// </summary>
		/// <param name="portalUrl">The portal URL.</param>
		public static void Initialize(string portalUrl)
		{
			// Define a default challenge method
			if (IdentityManager.Current.ChallengeMethod == null)
			{
				IdentityManager.Current.ChallengeMethod = (url, handler, options) =>
				{
					var info = IdentityManager.Current.FindServerInfo(url);
					if (info != null && info.OAuthClientInfo != null) // else  not an oauth server
					{
						var crd = IdentityManager.Current.FindCredential(url);
						if (crd == null) // else already signed in --> manage single sign in
						{
							IdentityManager.Current.GenerateCredentialAsync(url, handler, options);
							return;
						}
					}
					handler(null, new Exception()); // case not managed by this default challenge method
				};
			}

			// Manage the default credential (either coming from fragments or persisted)
			HtmlDocument htmlDoc = HtmlPage.Document;
			IdentityManager.Credential credential;
			if (!string.IsNullOrEmpty(htmlDoc.DocumentUri.Fragment))
			{
				// Here we are managing the case where the authorization is initiated in HTML

				// Create the OAuth token from the fragments
				var parameters = DecodeParameters(htmlDoc.DocumentUri);
				credential = FromDictionary(parameters);

				// Persit the token if user checked that option
				if (parameters.ContainsKey("persist") && parameters["persist"] == "true")
					SaveOAuthToken(portalUrl, parameters);
				else
					RemovePersistedOAuthToken(portalUrl);
			}
			else 
				// get a previously stored token
				credential = LoadOAuthToken(portalUrl);

			if (credential != null && !string.IsNullOrEmpty(portalUrl))
			{
				credential.Url = portalUrl;
				IdentityManager.Current.AddCredential(credential);
			}

			// Create the script block
			var res = htmlDoc.GetElementById("oauth");
			if (res != null)
				return; // use script defined in the HTML page

			var scriptElement = htmlDoc.CreateElement("script");
			scriptElement.SetAttribute("type", "text/javascript");
			scriptElement.Id = "oauth";

			const string scriptText = @"
    if (opener) {
        // Page initiated as redirect uri of another SL instance --> pass in the fragments to the other instance and close this one
        try {
            opener.setAuthData(location.href);
        }
        catch (err) // the SL window may have been closed
        { }
        close();
    }

    var timer = null;
    var slCtl = null; // Silverlight control that contains the managed object
    function openWindow(url, name, options, plugin) {
        slCtl = plugin;
        var child = window.open(url, name, options);
        if (!child) {
            alert(""No child? You may need to enable Protected Mode for the Internet zone"");
            setAuthData(null);
        } else {
            timer = setInterval(function() { checkChild(); }, 1000);
            function checkChild() {
                // User may have closed the window without using 'Cancel' button
                if (child.closed) {
                    setAuthData(null);
                }
            }
        }
    }

    // called from the redirect uri (may be the same as the app)
    function setAuthData(href) {
        // OAuthAuthorize.SetResult must be a SL scriptable member that has been registered
        slCtl.Content.OAuthAuthorize.SetResult(href);
        clearInterval(timer);
    }";

			// Add standard script in the HTML page
			scriptElement.SetProperty("text", scriptText);
			htmlDoc.Body.AppendChild(scriptElement);
		}

		/// <summary>
		/// Gets or sets a value indicating whether the authorization page shows up in a popup window
		/// </summary>
		/// <value>
		///   <c>true</c> if the authorization page is displayed in a popup window; otherwise, the page is displayed in a new window or new tab of the browser.<c>false</c>.
		/// </value>
		public bool UsePopup { get; set; }

		internal void OpenAuthorizeWindow()
		{
			// Open Authorization window in a popup or a new tab
			if (UsePopup)
				HtmlPage.Window.Invoke("openWindow", _authorizeUrl, "oauth", "height = 320, width = 480, location = no, resizable = yes, scrollbars, status = no, left = 200, top = 200",  HtmlPage.Plugin); // popup
			else
				HtmlPage.Window.Invoke("openWindow", _authorizeUrl, "_blank", "", HtmlPage.Plugin); // create another tab
		}

		/// <summary>
		/// *FOR INTERNAL USE ONLY* Called from JavaScript
		/// </summary>
		/// <exclude/>
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
		[ScriptableMember]
		public void SetResult(string callbackUrl)
		{
			if (string.IsNullOrEmpty(callbackUrl))
				_tcs.TrySetException(new OperationCanceledException());
			else
			{
				IDictionary<string, string> parameters = DecodeParameters(new Uri(callbackUrl));
				if (parameters.ContainsKey("persist") && parameters["persist"] == "true")
				{
					SaveOAuthToken(_portalUrl, parameters);
				}
				else
					RemovePersistedOAuthToken(_portalUrl);

				_tcs.TrySetResult(parameters);
			}
		}



		/// <summary>
		/// Decodes the parameters returned when the user agent is redirected to the callback url
		/// The parameters can be returned as fragments (e.g. access_token for Browser based app) or as query parameter (e.g. code for Server based app)
		/// </summary>
		/// <param name="uri">The URI.</param>
		internal static IDictionary<string, string> DecodeParameters(Uri uri)
		{
			string answer = !string.IsNullOrEmpty(uri.Fragment)
								? uri.Fragment.Substring(1)
								: (!string.IsNullOrEmpty(uri.Query) ? uri.Query.Substring(1) : string.Empty);

			// decode parameters from format key1=value1&key2=value2&...
			return answer.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split('=')).ToDictionary(pair => pair[0], pair => pair.Length > 1 ? HttpUtility.UrlDecode(pair[1]) : null);
		}

		internal static void SaveOAuthToken(string portalUrl, IDictionary<string, string> parameters)
		{
			if (parameters != null && !parameters.ContainsKey("error"))
				SaveOAuthTokenAsCookie(parameters);
		}

		internal static IdentityManager.Credential LoadOAuthToken(string portalUrl)
		{
			return string.IsNullOrEmpty(portalUrl) ? null : LoadOAuthTokenFromCookie();
		}

		internal static void RemovePersistedOAuthToken(string portalUrl)
		{
			RemoveOAuthTokenFromCookie();
		}

		// Mangement by cookie
		private const string OAuthCookieKey = "arcgis_auth";

		// store the token as cookie for further usage
		private static void SaveOAuthTokenAsCookie(IDictionary<string, string> parameters)
		{
			if (!IsCookieEnabled())
				return; //If cookies not enabled we should use another way to store token 

			DateTime expiration;
			if (parameters.ContainsKey("expires_in"))
			{
				long expiresIn;
				Int64.TryParse(parameters["expires_in"], out expiresIn);
				expiration = DateTime.UtcNow + TimeSpan.FromSeconds(expiresIn);
			}
			else
			{
				expiration = DateTime.UtcNow + TimeSpan.FromHours(2); // 2 hours by default
			}

			var jsonObject = new JsonObject(parameters.ToDictionary(kvp => kvp.Key, kvp => (JsonValue)kvp.Value));
			string value = jsonObject.ToString();

			string cookie = String.Format("{0}={1};expires={2}", OAuthCookieKey, value, expiration.ToString("R"));

			HtmlPage.Document.SetProperty("cookie", cookie);
		}

		private static bool IsCookieEnabled()
		{
			return HtmlPage.BrowserInformation.CookiesEnabled;
		}

		private static void RemoveOAuthTokenFromCookie()
		{
			// Set the cookie with a past date
			const string cookie = OAuthCookieKey + "=; expires=Fri, 31 Dec 1999 23:59:59 GMT;";
			HtmlPage.Document.SetProperty("cookie", cookie);
		}

		private static IdentityManager.Credential LoadOAuthTokenFromCookie()
		{
			if (!IsCookieEnabled())
				return null; //If cookies not enabled we should use another way to store token 

			string[] cookies = HtmlPage.Document.Cookies.Split(';');
			var value = (from cookie in cookies
							let keyValue = cookie.Split('=')
							where keyValue.Length == 2 && keyValue.First().Trim() == OAuthCookieKey
							select keyValue[1]).FirstOrDefault();

			if (string.IsNullOrEmpty(value))
				return null;
			value = Uri.UnescapeDataString(value);

			JsonValue jsonValue;
			try
			{
				jsonValue = JsonValue.Parse(value);
			}
			catch (Exception)
			{
				jsonValue = null;
			}
			return FromJsonValue(jsonValue);
		}


		private static IdentityManager.Credential FromJsonValue(JsonValue jsonValue)
		{
			var dictionary = jsonValue as IDictionary<string, JsonValue>;
			if (dictionary == null)
				return null;
			Dictionary<string, string> dict = dictionary.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.JsonType == JsonType.String ? (string)kvp.Value :  kvp.Value.ToString());
			return FromDictionary(dict);
		}

		private static IdentityManager.Credential FromDictionary(IDictionary<string, string> dictionary)
		{
			var credential = new IdentityManager.Credential();

			if (dictionary.ContainsKey("access_token"))
			{
				// Token returned --> no error
				credential.Token = dictionary["access_token"];
			}

			if (dictionary.ContainsKey("expires_in"))
			{
				long expiresIn;
				Int64.TryParse(dictionary["expires_in"], out expiresIn);
				credential.ExpirationDate = DateTime.UtcNow + TimeSpan.FromSeconds(expiresIn);
			}

			if (dictionary.ContainsKey("refresh_token"))
				credential.OAuthRefreshToken = dictionary["refresh_token"];
			if (dictionary.ContainsKey("username"))
				credential.UserName = dictionary["username"];

			return credential;
		}

	}

#else // WPF
	/// <summary>
	/// Manages the OAuth authorization process.
	/// Instantiate an instance of this class to initialize the <see cref="IdentityManager.ServerInfo.OAuthClientInfo"/> OAuthAuthorize property.
	/// </summary>
	public class OAuthAuthorize: IOAuthAuthorize
	{

		private string _callbackUrl;
		private TaskCompletionSource<IDictionary<string, string>> _tcs;

		private Window _window;

		/// <summary>
		/// Manages the OAuth sign in and response process for accessing the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="authorizeUrl">The authorize URL.</param>
		/// <param name="callbackUrl">The callback URL.</param>
		/// <returns>Dictionary of parameters returned by the authorization porcess (code, access_token, refresh_token, ...)</returns>
		public Task<IDictionary<string, string>> AuthorizeAsync(string url, string authorizeUrl, string callbackUrl)
		{
			if (_tcs != null || _window != null)
				throw new Exception(); // only one authorization process at a time

			_callbackUrl = callbackUrl;
			_tcs = new TaskCompletionSource<IDictionary<string, string>>();

			// Set an embedded webBrowser that displays the authorize page
			var webBrowser = new WebBrowser();
			webBrowser.Navigating += WebBrowserOnNavigating;

			// Display the webBrowser in a window (default behavior, may be customized by an application)
			_window = new Window
			{
				Content = webBrowser,
				Height = 480,
				Width = 480,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				Owner = Application.Current != null && Application.Current.MainWindow != null
							? Application.Current.MainWindow
							: null
			};

			_window.Closed += OnWindowClosed;
			webBrowser.Navigate(authorizeUrl);

			// Display the Window
			var tcs = _tcs;
			_window.ShowDialog();

			return tcs.Task;
		}

		void OnWindowClosed(object sender, EventArgs e)
		{
			if (_window != null && _window.Owner != null)
				_window.Owner.Focus();
			if (_tcs != null && !_tcs.Task.IsCompleted)
				_tcs.SetException(new OperationCanceledException()); // user closed the window
			_tcs = null;
			_window = null;
		}

		// Check if the web browser is redirected to the callback url
		void WebBrowserOnNavigating(object sender, NavigatingCancelEventArgs e)
		{
			var webBrowser = sender as WebBrowser;
			Uri uri = e.Uri;
			if (webBrowser == null || uri == null || _tcs == null)
				return;

			if (!String.IsNullOrEmpty(uri.AbsoluteUri) && uri.AbsoluteUri.StartsWith(_callbackUrl))
			{
				// The web browser is redirected to the callbackUrl ==> close the window, decode the parameters returned as fragments or query, and return these parameters as result of the Task
				e.Cancel = true;
				var tcs = _tcs;
				_tcs = null;
				if (_window != null)
				{
					_window.Close();
				}
				tcs.SetResult(DecodeParameters(uri));
			}
		}
		
		/// <summary>
		/// Decodes the parameters returned when the user agent is redirected to the callback url
		/// The parameters can be returned as fragments (e.g. access_token for Browser based app) or as query parameter (e.g. code for Server based app)
		/// </summary>
		/// <param name="uri">The URI.</param>
		private static IDictionary<string, string> DecodeParameters(Uri uri)
		{
			string answer = !string.IsNullOrEmpty(uri.Fragment)
								? uri.Fragment.Substring(1)
								: (!string.IsNullOrEmpty(uri.Query) ? uri.Query.Substring(1) : string.Empty);

			// decode parameters from format key1=value1&key2=value2&...
			return answer.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Split('=')).ToDictionary(pair => pair[0], pair => pair.Length > 1 ? Uri.UnescapeDataString(pair[1]) : null);
		}
	}

#endif
}
