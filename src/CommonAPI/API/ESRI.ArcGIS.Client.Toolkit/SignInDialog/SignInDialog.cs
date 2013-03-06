// (c) Copyright ESRI.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
#if !SILVERLIGHT
using System.Net;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
#endif

namespace ESRI.ArcGIS.Client.Toolkit
{
#if SILVERLIGHT
	/// <summary>The SignInDialog Control challenges the user for a name and password
	/// when trying to access secured ArcGIS services.
	/// From these name and password, the SignInDialog generates a Token and returns this token encapsulated in a <see cref="IdentityManager.Credential"/> object.
	/// </summary>
	/// <remarks>
	/// This control is designed to work with the <see cref="IdentityManager"/>.
	/// The IdentityManager can be actived with code like:
	/// ESRI.ArcGIS.Client.IdentityManager.Current.ChallengeMethod = ESRI.ArcGIS.Client.Toolkit.SignInDialog.DoSignIn;
	/// In this case, the SignInDialog is created and activated in a child window.
	/// 
	/// It's also possible to put the SignInDialog in the Visual Tree and to write your own challenge method activating this SignInDialog.
	/// </remarks>
#endif
#if !SILVERLIGHT
	/// <summary>
	/// The SignInDialog Control challenges the user for a credential
	/// when trying to access secured ArcGIS services.
	/// The SignInDialog Control can manage Network, Certificate or Token credential.
	/// </summary>
	/// <remarks>
	/// This control is designed to work with the <see cref="IdentityManager" />.
	/// The IdentityManager can be actived with code like:
	/// ESRI.ArcGIS.Client.IdentityManager.Current.ChallengeMethod = ESRI.ArcGIS.Client.Toolkit.SignInDialog.DoSignIn;
	/// or 
	/// ESRI.ArcGIS.Client.IdentityManager.Current.ChallengeMethodEx = ESRI.ArcGIS.Client.Toolkit.SignInDialog.DoSignInEx;
	/// In this case, the SignInDialog is created and activated in a child window.
	/// It's also possible to put the SignInDialog in the Visual Tree and to write your own challenge method activating this SignInDialog.
	/// </remarks>
#endif
	[TemplatePart(Name = "RichTextBoxMessage", Type = typeof(RichTextBox))]
	[TemplatePart(Name = "RichTextBoxErrorMessage", Type = typeof(RichTextBox))]
	public class SignInDialog : Control, INotifyPropertyChanged
	{

		#region Constructors
		private RichTextBox _rtbMessage;
		private RichTextBox _rtbErrorMessage;
		private string _rtbMessageInitialXaml;
		private string _rtbErrorMessageInitialXaml;
		private long _requestID; // flag allowing the reuse of the same dialog after cancelling a request
		private bool _callbackHasBeenCalled; // flag to assure that the callback will be called once (possibly with Cancel exception) as soon as the signin dialog has been activated

		/// <summary>
		/// Initializes a new instance of the <see cref="SignInDialog"/> control.
		/// </summary>
		public SignInDialog()
		{
#if SILVERLIGHT
			DefaultStyleKey = typeof(SignInDialog);
#endif
			GenerateCredentialCommand = new GenerateCredentialCommandImpl(this);
			CancelCommand = new CancelCommandImpl(this);
			DataContext = this;
		}

		/// <summary>
		/// Static initialization for the <see cref="SignInDialog"/> control.
		/// </summary>
		static SignInDialog()
		{
#if !SILVERLIGHT
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SignInDialog),
				new FrameworkPropertyMetadata(typeof(SignInDialog)));
#endif
		}
		#endregion

		#region Override OnApplyTemplate
		/// <summary>
		/// When overridden in a derived class, is invoked whenever application
		/// code or internal processes (such as a rebuilding layout pass) call 
		/// <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In
		/// simplest terms, this means the method is called just before a UI
		/// element displays in an application.
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			_rtbMessage = GetTemplateChild("RichTextBoxMessage") as RichTextBox;
			_rtbErrorMessage = GetTemplateChild("RichTextBoxErrorMessage") as RichTextBox;
#if SILVERLIGHT
			if (_rtbMessage != null)
				_rtbMessageInitialXaml = _rtbMessage.Xaml;
			if (_rtbErrorMessage != null)
				_rtbErrorMessageInitialXaml = _rtbErrorMessage.Xaml;
#else
			if (_rtbMessage != null)
				_rtbMessageInitialXaml = XamlWriter.Save(_rtbMessage.Document);
			if (_rtbErrorMessage != null)
				_rtbErrorMessageInitialXaml = XamlWriter.Save(_rtbErrorMessage.Document);
#endif
			SetRichTextBoxMessage();
			SetRichTextBoxErrorMessage();
		}

		#endregion

		#region Public property Url
		private string _url;
		/// <summary>
		/// Gets or sets the URL of the token rest end point.
		/// </summary>
		/// <value>
		/// The URL.
		/// </value>
		public string Url
		{
			get { return _url; }
			set
			{
				const string propertyName = "Url";
				if (_url != value)
				{
					_url = value;

					SetRichTextBoxMessage();
					SetRichTextBoxErrorMessage();
					OnPropertyChanged(propertyName);
					UpdateIsReady();
					if (string.IsNullOrEmpty(value))
						throw new ArgumentNullException(propertyName);
				}
			}
		}
		#endregion

		#region Public property Callback
		private Action<IdentityManager.Credential, Exception> _callback;
		/// <summary>
		/// Gets or sets the callback that is called when the token generation is done (maybe by user cancellation)
		/// </summary>
		/// <value>
		/// The callback.
		/// </value>
		public Action<IdentityManager.Credential, Exception> Callback
		{
			get { return _callback; }
			set
			{
				const string propertyName = "Callback";
				if (_callback != value)
				{
					_callback = value;

					OnPropertyChanged(propertyName);
					UpdateIsReady();
					if (_callback == null)
						throw new ArgumentNullException(propertyName);
				}
			}
		}

		#endregion

		#region Public property UserName
		private string _userName;
		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>
		/// The name of the user.
		/// </value>
		public string UserName
		{
			get { return _userName; }
			set
			{
				const string propertyName = "UserName";
				if (_userName != value)
				{
					_userName = value;

					OnPropertyChanged(propertyName);
					UpdateIsReady();
					if (string.IsNullOrEmpty(value))
						throw new ArgumentNullException();
				}
			}
		}
		#endregion

		#region Public property Password
		private string _password;
		/// <summary>
		/// Gets or sets the password uised to get the token.
		/// </summary>
		/// <value>
		/// The password.
		/// </value>
		public string Password
		{
			get { return _password; }
			set
			{
				const string propertyName = "Password";
				if (_password != value)
				{
					_password = value;
					OnPropertyChanged(propertyName);
					UpdateIsReady();

					if (string.IsNullOrEmpty(value))
						throw new ArgumentNullException();
				}
			}
		}
		#endregion

		#region Public property IsActive
		private bool _isActive;
		/// <summary>
		/// Gets or sets a value indicating whether this instance is active.
		/// </summary>
		/// <remarks>As soon as IsActive is set to true, we assure that the callback will be called once
		/// when the token generation is done.</remarks>
		/// <value>
		///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
		/// </value>
		public bool IsActive
		{
			get { return _isActive; }
			set
			{
				if (_isActive != value)
				{
					_isActive = value;
					if (value)
					{
						ErrorMessage = null;
						_callbackHasBeenCalled = false;
					}
					else
					{
						_isActive = false;
						_requestID++; // to avoid issue when the token will eventually be generated
						if (Callback != null && !_callbackHasBeenCalled)
						{
							_callbackHasBeenCalled = true;
							Callback(null, new OperationCanceledException());
						}
					}

					OnPropertyChanged("IsActive");
					UpdateIsReady();
					UpdateCanCancel();
				}
			}
		}
#if SILVERLIGHT4
		private class OperationCanceledException : SystemException
		{
		}
#endif
		#endregion

		#region Public Read-Only property IsBusy
		private bool _isBusy;
		/// <summary>
		/// Gets a value indicating whether this instance is busy getting a token.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
		/// </value>
		public bool IsBusy
		{
			get { return _isBusy; }
			private set
			{
				if (_isBusy != value)
				{
					_isBusy = value;
					OnPropertyChanged("IsBusy");
					UpdateIsReady();
				}
			}
		}
		#endregion

		#region Public Read-Only Property ErrorMessage
		private string _errorMessage;
		/// <summary>
		/// Gets the error that occured during the latest token generation.
		/// </summary>
		/// <value>
		/// The error message.
		/// </value>
		public string ErrorMessage
		{
			get { return _errorMessage; }
			private set
			{
				if (_errorMessage != value)
				{
					_errorMessage = value;
					SetRichTextBoxErrorMessage();
					OnPropertyChanged("ErrorMessage");
				}
			}
		}
		#endregion

		#region Public property GenerateTokenOptions
		/// <summary>
		/// Gets or sets the generate token options.
		/// </summary>
		/// <value>
		/// The generate token options.
		/// </value>
		public IdentityManager.GenerateTokenOptions GenerateTokenOptions { get; set; }

		#endregion

		#region Dependency Property Title
		/// <summary>
		/// Gets or sets the title that can be displayed by the SignInDialog or by the container of the SignInDialog.
		/// </summary>
		/// <remarks>The default SignInDialog template doesn't display the Title.</remarks>
		/// <value>
		/// The title.
		/// </value>
		public string Title
		{
			// This is a DP so the value can be initialized in XAML allowing easy L10N
			get { return (string)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Title"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty TitleProperty =
			DependencyProperty.Register("Title", typeof(string), typeof(SignInDialog), null);

		#endregion

		/// <summary>
		/// Gets the generate credential command allowing to get a generate a credential asynchronously.
		/// </summary>
		/// <remarks>This command is generally executed on the 'OK' button.</remarks>
		public ICommand GenerateCredentialCommand { get; private set; }

		/// <summary>
		/// Gets the cancel command allowing to cancel the ongoing token request.
		/// </summary>
		/// <remarks>This command doesn't desactivate the SignInDialog.</remarks>
		public ICommand CancelCommand { get; private set; }

		#region DoSignIn static challenge method

#pragma warning disable 1574 // cref attribute 'DoSignInEx' that could not be resolved
		/// <summary>
		/// Static challenge method leaveraging the SignInDialog in a child window.
		/// </summary>
		/// <ESRIWPF><remarks>This method manages token credential only. See <see cref="DoSignInEx"/> for the extended method managing all credential types.</remarks></ESRIWPF>
		/// <param name="url">The URL.</param>
		/// <param name="callback">The callback.</param>
		/// <param name="generateTokenOptions">The generate token options.</param>
		/// <ESRIWPF><seealso cref="DoSignInEx"/></ESRIWPF>
		public static void DoSignIn(string url, Action<IdentityManager.Credential, Exception> callback, IdentityManager.GenerateTokenOptions generateTokenOptions = null)
		{
			System.Windows.Threading.Dispatcher d = null;

#if SILVERLIGHT
			// Note that RootVisual is only accessible from UI Thread so Application.Current.RootVisual.Dispatcher crashes
			if (Deployment.Current != null) // should always be the case
				d = Deployment.Current.Dispatcher;
#else
			if (Application.Current != null)
				d = Application.Current.Dispatcher;
#endif

			if (d != null && !d.CheckAccess())
			{
				//Ensure we are showing up the SignInDialog on the UI thread
				d.BeginInvoke((Action) delegate { DoSignInInUIThread(url, callback, generateTokenOptions); });
			}
			else
				DoSignInInUIThread(url, callback, generateTokenOptions);
		}
#pragma warning restore 1574

		private static void DoSignInInUIThread(string url, Action<IdentityManager.Credential, Exception> callback, IdentityManager.GenerateTokenOptions generateTokenOptions
#if !SILVERLIGHT
			, IdentityManager.AuthenticationType authenticationType = IdentityManager.AuthenticationType.Token
#endif
			)
		{
			// In SL and WPF : Create the ChildWindow that contains the SignInDialog
#if SILVERLIGHT
			ChildWindow childWindow = new ChildWindow();
			DependencyProperty titleProperty = ChildWindow.TitleProperty;
#else
			var childWindow = new Window
			{
				ShowInTaskbar = false,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				WindowStyle = WindowStyle.ToolWindow,
				SizeToContent = SizeToContent.WidthAndHeight,
				ResizeMode = ResizeMode.NoResize,
				WindowState = WindowState.Normal
			};

			if (Application.Current != null && Application.Current.MainWindow != null)
			{
				try
				{
					childWindow.Owner = Application.Current.MainWindow;
				}
				catch (Exception)
				{
					// May fire an exception when used inside an excel or powerpoint addins
				}
			}

			DependencyProperty titleProperty = Window.TitleProperty;
#endif

			// Create the SignInDialog with the parameters given as arguments
			var signInDialog = new SignInDialog
			{
				Url = url,
				Callback = (credential, error) =>
				{
					childWindow.Close();
					callback(credential, error);
				},
				GenerateTokenOptions = generateTokenOptions,
				IsActive = true,
				Width = 300,
#if !SILVERLIGHT
				_authenticationType = authenticationType
#endif
			};

			childWindow.Content = signInDialog;

			// Bind the Title so the ChildWindow Title is the SignInDialog title (taht will be initialized later)
			Binding binding = new Binding("Title") { Source = signInDialog };
			childWindow.SetBinding(titleProperty, binding);
			childWindow.Closed += (s, e) => signInDialog.IsActive = false; // be sure the SignInDialog is deactivated (i.e. Callback executed once) when closing the childwindow using the X
			// Show the window
#if SILVERLIGHT
			childWindow.Show();
#else
			childWindow.ShowDialog();
#endif
		}

		#endregion

#if !SILVERLIGHT // AuthenticationType and DoSignInEx
		private IdentityManager.AuthenticationType _authenticationType;
		/// <summary>
		/// Gets or sets the type of the authentication managed by the sign in dialog.
		/// </summary>
		/// <value>
		/// The type of the authentication (Token or NetworkCredential).
		/// </value>
		public IdentityManager.AuthenticationType AuthenticationType
		{
			get { return _authenticationType; }
			set { _authenticationType = value; }
		}

		#region DoSignInEx static challenge method
		/// <summary>
		/// Static challenge method leaveraging the SignInDialog in a child window.
		/// This method manages all credential types.
		/// Note however that in case of <see cref="IdentityManager.AuthenticationType.Certificate">certificate authentication</see> the SignInDialog UI is not used.
		/// The standard .Net dialog box for selecting an X.509 certificate from a certificate collection is used instead.
		/// </summary>
		/// <seealso cref="DoSignIn"/>
		/// <param name="credentialRequestInfos">The information about the credential to get.</param>
		/// <param name="callback">The callback.</param>
		/// <param name="generateTokenOptions">The generate token options.</param>
		public static void DoSignInEx(IdentityManager.CredentialRequestInfos credentialRequestInfos, Action<IdentityManager.Credential, Exception> callback, IdentityManager.GenerateTokenOptions generateTokenOptions = null)
		{
			System.Windows.Threading.Dispatcher d = null;
			if (Application.Current != null)
				d = Application.Current.Dispatcher;

			if (d != null && !d.CheckAccess())
			{
				//Ensure we are showing up the SignInDialog on the UI thread
				d.BeginInvoke((Action)(() => DoSignInInUIThreadEx(credentialRequestInfos, callback, generateTokenOptions)));
			}
			else
				DoSignInInUIThreadEx(credentialRequestInfos, callback, generateTokenOptions);
		}

		private static void DoSignInInUIThreadEx(IdentityManager.CredentialRequestInfos credentialRequestInfos, Action<IdentityManager.Credential, Exception> callback, IdentityManager.GenerateTokenOptions generateTokenOptions)
		{
			switch(credentialRequestInfos.AuthenticationType)
			{
				case IdentityManager.AuthenticationType.Token:
				case IdentityManager.AuthenticationType.NetworkCredential:
					DoSignInInUIThread(credentialRequestInfos.Url, callback, generateTokenOptions, credentialRequestInfos.AuthenticationType);
					break;

				case IdentityManager.AuthenticationType.Certificate:
					ChallengeCertificate(credentialRequestInfos, callback);
					break;
			}
		}

		private static void ChallengeCertificate(IdentityManager.CredentialRequestInfos credentialRequestInfos, Action<IdentityManager.Credential, Exception> callback)
		{
			var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
			X509Certificate2Collection certificates;
			try
			{
				const string clientAuthOid = "1.3.6.1.5.5.7.3.2"; // Client Authentication OID
				store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
				// Find Client Authentication certificate
				certificates = store.Certificates.Find(X509FindType.FindByApplicationPolicy, clientAuthOid, true); // todo true);
			}
			catch (Exception)
			{
				certificates = null;
			}
			finally
			{
				store.Close();
			}

			if (certificates != null && certificates.Count >= 1)
			{
				// Let the user select/validate the certificate
				string url = credentialRequestInfos.Url;
				string resourceName = GetResourceName(url);
				IdentityManager.ServerInfo serverInfo = IdentityManager.Current.FindServerInfo(url);
				string server = serverInfo == null ? Regex.Match(url, "http.?//[^/]*").ToString() : serverInfo.ServerUrl;
				string message = string.Format(Properties.Resources.SignInDialog_CertificateRequired, resourceName, server); // certicate required to access {0} on {1}
				certificates = X509Certificate2UI.SelectFromCollection(certificates, null, message, X509SelectionFlag.SingleSelection);
			}

			IdentityManager.Credential credential = null;
			Exception error = null;
			if (certificates != null && certificates.Count > 0)
			{
				credential = new IdentityManager.Credential {ClientCertificate = certificates[0]};
			}
			else
			{
				// Note : Error type is not that important since the error returned to the user is the initial HTTP error (Authorization Error)
				error = new System.Security.Authentication.AuthenticationException();
			}

			callback(credential, error);
		}

		#endregion

		#region NetworkCredential Generation

		private void GenerateNetworkCredential()
		{
			if (!IsReady)
				return;

			_callbackHasBeenCalled = true; // Avoid that IsActive = false calls the callback with Cancel exception
			IsActive = false;
			var credential = new IdentityManager.Credential {Credentials = new NetworkCredential(UserName, Password)};
			if (Callback != null)
				Callback(credential, null);
		}
		#endregion
#endif

		#region INotifyPropertyChanged Members

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		internal void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChanged = PropertyChanged;
			if (propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion

		#region Token Generation

		private void GenerateToken()
		{
			if (!IsReady)
				return;

			IsBusy = true;
			ErrorMessage = null;

			long requestID = ++_requestID;
			IdentityManager.Current.GenerateCredentialAsync(Url, UserName, Password, (crd, exc) => TokenGenerated(crd, exc, requestID), GenerateTokenOptions);
		}

		private void TokenGenerated(IdentityManager.Credential credential, Exception exc, long requestID)
		{
			if (requestID != _requestID)
				return; // No more the current request

			IsBusy = false;
			string error = null;
			if (exc != null)
			{
				error = exc.Message;
				if (string.IsNullOrEmpty(error) && exc.InnerException != null)
					error = exc.InnerException.Message;
			}
			ErrorMessage = error;

			if (exc == null) // else the user can try again
			{
				_callbackHasBeenCalled = true; // Avoid that IsActive = false calls the callback with Cancel exception
				IsActive = false;
				if (Callback != null)
					Callback(credential, exc);
			}
		}
		#endregion

		#region Private helper methods to generate message in RichTextBox

		private void SetRichTextBoxMessage()
		{
			MakeReplacements(_rtbMessage, _rtbMessageInitialXaml);
		}

		private void SetRichTextBoxErrorMessage()
		{
			MakeReplacements(_rtbErrorMessage, _rtbErrorMessageInitialXaml);
		}

		private void MakeReplacements(RichTextBox richTextBox, string xaml)
		{
			if (richTextBox == null || string.IsNullOrEmpty(xaml))
				return;

			string url = Url;

			if (string.IsNullOrEmpty(url) && DesignerProperties.GetIsInDesignMode(this))
				url = "http://myServer.com/rest/services/myService"; // Use design Url

			if (!string.IsNullOrEmpty(url))
			{
				string resourceName = GetResourceName(url);
				IdentityManager.ServerInfo serverInfo = IdentityManager.Current.FindServerInfo(url);
				string server = serverInfo == null ? Regex.Match(url, "http.?//[^/]*").ToString() : serverInfo.ServerUrl;
				xaml = xaml.Replace("$RESOURCENAME", XamlEncode(resourceName));
				xaml = xaml.Replace("$URL", XamlEncode(url));
				xaml = xaml.Replace("$SERVER", XamlEncode(server));
			}
#if !SILVERLIGHT
			xaml = xaml.Replace("$AUTHENTICATIONTYPE", _authenticationType.ToString());
#endif
			xaml = xaml.Replace("$ERRORMESSAGE", XamlEncode(ErrorMessage));

			string previousError = null;
			string referer = null;
			string proxyUrl = null;
			if (GenerateTokenOptions != null)
			{
				previousError = GenerateTokenOptions.PreviousError.ToString();
				referer = GenerateTokenOptions.Referer;
				proxyUrl = GenerateTokenOptions.ProxyUrl;
			}
			xaml = xaml.Replace("$PREVIOUSERROR", XamlEncode(previousError));
			xaml = xaml.Replace("$SERVER", XamlEncode(referer));
			xaml = xaml.Replace("$PROXYURL", XamlEncode(proxyUrl));

#if SILVERLIGHT
			richTextBox.Xaml = xaml;
#else
			StringReader stringReader = new StringReader(xaml);
			XmlReader xmlReader = XmlReader.Create(stringReader);
			richTextBox.Document = XamlReader.Load(xmlReader) as FlowDocument;
#endif
		}

		private static string XamlEncode(string inputStr)
		{
			return string.IsNullOrEmpty(inputStr)
				       ? inputStr
				       : inputStr.Replace("&", "&amp;")
				                 .Replace("<", "&lt;")
				                 .Replace(">", "&gt;")
				                 .Replace("\"", "&quot;")
				                 .Replace("'", "&apos;");
		}

		private static string GetResourceName(string url)
		{
			if (url.IndexOf("/rest/services", StringComparison.OrdinalIgnoreCase) > 0)
				return GetSuffix(url);

			url = Regex.Replace(url, "http.?//[^/]*", "");
			url = Regex.Replace(url, ".*/items/([^/]+).*", "$1");
			return url;
		}

		private static string GetSuffix(string url)
		{
			url = Regex.Replace(url, "http.+/rest/services/?", "", RegexOptions.IgnoreCase);
			url = Regex.Replace(url, "(/(MapServer|GeocodeServer|GPServer|GeometryServer|ImageServer|NAServer|FeatureServer|GeoDataServer|GlobeServer|MobileServer)).*", "$1", RegexOptions.IgnoreCase);
			return url;
		}
		#endregion

		// Private methods
		private bool IsReady
		{
			get { return IsActive && !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password) && !IsBusy && !string.IsNullOrEmpty(Url) && Callback != null; }
		}

		private void UpdateIsReady()
		{
			((GenerateCredentialCommandImpl)GenerateCredentialCommand).OnCanExecuteChanged();
		}

		private void UpdateCanCancel()
		{
			((CancelCommandImpl)CancelCommand).OnCanExecuteChanged();
		}

		#region Commands implementation
		private class GenerateCredentialCommandImpl : ICommand
		{
			private readonly SignInDialog _signInDialog;

			internal GenerateCredentialCommandImpl(SignInDialog signInDialog)
			{
				_signInDialog = signInDialog;
			}

			public bool CanExecute(object parameter)
			{
				return _signInDialog.IsReady;
			}

			public event EventHandler CanExecuteChanged;
			internal void OnCanExecuteChanged()
			{
				if (CanExecuteChanged != null)
					CanExecuteChanged(this, EventArgs.Empty);
			}

			public void Execute(object parameter)
			{
#if SILVERLIGHT
				_signInDialog.GenerateToken();
#else
				if (_signInDialog.AuthenticationType == IdentityManager.AuthenticationType.Token)
					_signInDialog.GenerateToken();
				else
					_signInDialog.GenerateNetworkCredential();
#endif
			}
		}

		private class CancelCommandImpl : ICommand
		{
			private readonly SignInDialog _signInDialog;

			internal CancelCommandImpl(SignInDialog signInDialog)
			{
				_signInDialog = signInDialog;
			}

			public bool CanExecute(object parameter)
			{
				return _signInDialog.IsActive;
			}

			public event EventHandler CanExecuteChanged;
			internal void OnCanExecuteChanged()
			{
				if (CanExecuteChanged != null)
					CanExecuteChanged(this, EventArgs.Empty);
			}

			public void Execute(object parameter)
			{
				_signInDialog.IsActive = false;
			}
		}
		#endregion

	}

	/// <summary>
 /// *FOR INTERNAL USE ONLY* Helper to execute a command when user types 'Enter' and to update the binding source when the text changes.
	/// </summary>
 /// <exclude/>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class TextInputManager
	{
		/// <summary>
		/// Gets the command.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		public static ICommand GetEnterCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(EnterCommandProperty);
		}

		/// <summary>
		/// Sets the command.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetEnterCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(EnterCommandProperty, value);
		}

		/// <summary>
		/// Identifies the Command attached property.
		/// </summary>
		public static readonly DependencyProperty EnterCommandProperty =
			DependencyProperty.RegisterAttached("EnterCommand", typeof(ICommand), typeof(TextInputManager), new PropertyMetadata(OnEnterCommandChanged));

		private static void OnEnterCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBox)
			{
				TextBox textBox = d as TextBox;
				if (e.OldValue != null)
					textBox.KeyUp -= KeyUpHandler;
				if (e.NewValue != null)
					textBox.KeyUp += KeyUpHandler;
			}
			else if (d is PasswordBox)
			{
				PasswordBox textBox = d as PasswordBox;
				if (e.OldValue != null)
					textBox.KeyUp -= KeyUpHandler;
				if (e.NewValue != null)
					textBox.KeyUp += KeyUpHandler;
			}
		}

		static void KeyUpHandler(object sender, KeyEventArgs e)
		{
			if (!(sender is DependencyObject))
				return;

			if (e.Key == Key.Enter)
			{
				ICommand command = GetEnterCommand((DependencyObject)sender);

				if (command != null && command.CanExecute(null))
				{
					command.Execute(null);
					e.Handled = true;
				}
			}
#if !SILVERLIGHT
			else
			{
				if (sender is PasswordBox)
				{
					PasswordBox passwordBox = (PasswordBox) sender;
					SetPasswordText(passwordBox, passwordBox.Password);
				}
			}
#endif
		}


#if !SILVERLIGHT
		/// <summary>
		/// Identifies the PasswordText attached property.
		/// In WPF this a workaround for the Password property that is not a DP.
		/// </summary>
		public static readonly DependencyProperty PasswordTextProperty =
			DependencyProperty.RegisterAttached("PasswordText", typeof(string), typeof(TextInputManager), new PropertyMetadata(OnPasswordTextChanged));

		private static void OnPasswordTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

			if (d is PasswordBox)
			{
				PasswordBox textBox = d as PasswordBox;
				string value = (string) e.NewValue;
				if (textBox.Password != value)
					textBox.Password = value;
			}
		}

		/// <summary>
		/// Gets the password text.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns></returns>
		public static string GetPasswordText(DependencyObject obj)
		{
			return (string)obj.GetValue(PasswordTextProperty);
		}

		/// <summary>
		/// Sets the password text.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="value">The value.</param>
		public static void SetPasswordText(DependencyObject obj, string value)
		{
			obj.SetValue(PasswordTextProperty, value);
		}
#endif

#if SILVERLIGHT
		/// <summary>
		/// Gets a flag that indicates if the binding source must be updated when the text changes.
		/// In SL4, this is a workaround to the lack of UpdateSourceTrigger=PropertyChanged option.
		/// </summary>
		/// <param name="obj">The obj.</param>
		public static bool GetUpdateSourceOnTextChanged(DependencyObject obj)
		{
			return (bool)obj.GetValue(UpdateSourceOnTextChangedProperty);
		}

		/// <summary>
		/// Sets a flag that indicates if the binding source must be updated when the text changes.
		/// </summary>
		public static void SetUpdateSourceOnTextChanged(DependencyObject obj, bool value)
		{
			obj.SetValue(UpdateSourceOnTextChangedProperty, value);
		}

		/// <summary>
		/// Identifies the UpdateSourceOnTextChanged attached property.
		/// </summary>
		public static readonly DependencyProperty UpdateSourceOnTextChangedProperty =
			DependencyProperty.RegisterAttached("UpdateSourceOnTextChanged", typeof(bool), typeof(TextInputManager), new PropertyMetadata(OnUpdateSourceOnTextChangedChanged));

		private static void OnUpdateSourceOnTextChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBox)
			{
				TextBox textBox = d as TextBox;
				if ((bool)e.OldValue)
					textBox.TextChanged -= OnTextChanged;
				if ((bool)e.NewValue)
					textBox.TextChanged +=OnTextChanged;
			}
			else if (d is PasswordBox)
			{
				PasswordBox passwordBox = d as PasswordBox;
				if ((bool)e.OldValue)
					passwordBox.PasswordChanged -= OnPasswordChanged;
				if ((bool)e.NewValue)
					passwordBox.PasswordChanged += OnPasswordChanged;
			}
		}


		static void OnPasswordChanged(object sender, RoutedEventArgs e)
		{
			var textBox = sender as PasswordBox;
			if (textBox != null)
			{
				BindingExpression binding = textBox.GetBindingExpression(PasswordBox.PasswordProperty);
				if (binding != null)
					binding.UpdateSource();
			}
		}

		static void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = sender as TextBox;
			if (textBox != null)
			{
				BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
				if (binding != null)
					binding.UpdateSource();
			}
		}
#endif
	}

} 

