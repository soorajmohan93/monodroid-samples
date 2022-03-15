using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Common.Apis;
using Android.Support.V7.App;
using Android.Gms.Common;
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Auth.Api;

namespace SigninQuickstart
{
	[Activity (MainLauncher = true, Theme = "@style/ThemeOverlay.MyNoTitleActivity")]
	[Register("com.xamarin.signinquickstart.MainActivity")]
	public class MainActivity : AppCompatActivity, View.IOnClickListener, GoogleApiClient.IOnConnectionFailedListener
	{
		const string TAG = "MainActivity";

		const int RC_SIGN_IN = 9001;

		GoogleApiClient mGoogleApiClient;
		TextView mStatusTextView;
		

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			mStatusTextView = FindViewById<TextView>(Resource.Id.status);
			FindViewById(Resource.Id.sign_in_button).SetOnClickListener(this);
			FindViewById(Resource.Id.sign_out_button).SetOnClickListener(this);

			GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
					.RequestEmail()
					.Build();
			
			mGoogleApiClient = new GoogleApiClient.Builder(this)
					.EnableAutoManage(this , this )
			        .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
					.Build();

			var signInButton = FindViewById<SignInButton>(Resource.Id.sign_in_button);
			signInButton.SetSize(SignInButton.SizeStandard);
		}

		protected override void OnStart()
		{
			base.OnStart();

			var opr = Auth.GoogleSignInApi.SilentSignIn(mGoogleApiClient);
			if (opr.IsDone)
			{
				var result = opr.Get() as GoogleSignInResult;
				HandleSignInResult(result);
			}
			else
			{
				opr.SetResultCallback(new SignInResultCallback { Activity = this });
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
			if (requestCode == RC_SIGN_IN)
			{
				var result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
				HandleSignInResult(result);
			}
		}

		public void HandleSignInResult(GoogleSignInResult result)
		{
			if (result.IsSuccess)
			{
				var acct = result.SignInAccount;
				mStatusTextView.Text = string.Format(GetString(Resource.String.signed_in_fmt), acct.DisplayName);

				Toast.MakeText(this, "Welcome " + acct.DisplayName + "!", ToastLength.Short).Show();
				UpdateUI(true);
			}
			else
			{
				UpdateUI(false);
				Toast.MakeText(this, "Not Logged In!", ToastLength.Short).Show();
			}
		}

		void SignIn()
		{
			var signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
			StartActivityForResult(signInIntent, RC_SIGN_IN);
		}

		void SignOut()
		{
			Auth.GoogleSignInApi.SignOut(mGoogleApiClient).SetResultCallback(new SignOutResultCallback { Activity = this });
			Toast.MakeText(this, "Logged Out!", ToastLength.Short).Show();
		}

		

		public void OnConnectionFailed(ConnectionResult result)
		{
			Toast.MakeText(this, "Connection Failed", ToastLength.Long).Show();
		}

		protected override void OnStop()
		{
			base.OnStop();
			mGoogleApiClient.Disconnect();
		}


		public void UpdateUI (bool isSignedIn)
		{
			if (isSignedIn)
			{
				FindViewById(Resource.Id.sign_in_button).Visibility = ViewStates.Gone;
				FindViewById(Resource.Id.sign_out).Visibility = ViewStates.Visible;
			}
			else
			{
				mStatusTextView.Text = GetString(Resource.String.signed_out);

				FindViewById(Resource.Id.sign_in_button).Visibility = ViewStates.Visible;
				FindViewById(Resource.Id.sign_out).Visibility = ViewStates.Gone;
			}
		}

		public void OnClick(View v)
		{
			switch (v.Id)
			{
				case Resource.Id.sign_in_button:
					SignIn();
					break;
				case Resource.Id.sign_out_button:
					SignOut();
					break;
			}
		}
	}
}


