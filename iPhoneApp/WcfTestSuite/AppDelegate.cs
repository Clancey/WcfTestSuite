using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreLocation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfWebApp.Models;
using System.IO;
using MonoTouch.Dialog;
using ClanceysLib;

namespace WcfTestSuite
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		UIButton uploadImage;
		UIButton getUsers;
		UIButton getTask;
		CLLocationManager manager;
		Service1Client client;
		DialogViewController dvc;
		MBProgressHUD loading;
		UINavigationController navigation;
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			navigation = new UINavigationController();
			dvc = new DialogViewController (createRoot ());
			navigation.PushViewController(dvc,false);
			window.RootViewController = navigation;
			window.MakeKeyAndVisible ();
			
			client = new Service1Client (new BasicHttpBinding (), new EndpointAddress ("http://192.168.2.17/WcfWebApp/Service1.svc"));
			client.GetUsersCompleted += HandleClientGetUsersCompleted;
			client.GetTasksCompleted += HandleClientGetTasksCompleted;
			client.UploadImageCompleted += HandleClientUploadImageCompleted;
			
			
			loading = new MBProgressHUD();
			loading.TitleText = "Loading";
			return true;
		}
		
		RootElement createRoot ()
		{
			return new RootElement ("WCF"){
				new Section ()
				{
					new  StringElement ("Get Users", delegate() {
						loading.Show(true);
						client.GetUsersAsync ();
					}),
					new StringElement ("Get Tasks", delegate {
						loading.Show(true);
						client.GetTasksAsync (1);
					}),
				},
				new Section ()
				{
					new ImagePickerElement ("testphoto.jpg", "Take a new photo"){
						OnSelect = (image) => {
							if (!string.IsNullOrEmpty (image)) {
								var imageData = GetImageData (image);
								if (imageData != null)
								{
									loading.Show(true);
									client.UploadImageAsync (imageData);
								}
							}
						}
					}
				}
			};
		}

		void HandleClientGetUsersCompleted (object sender, GetUsersCompletedEventArgs e)
		{
			loading.Hide(true);
			if(e.Error != null)
			{
				var alert = new UIAlertView("Error",e.Error.Message,null,"Ok");
				alert.Show();
				return;
			}
			
			this.BeginInvokeOnMainThread(delegate{
				dvc.ActivateController(new DialogViewController(new RootElement("Users"){new Section(){Elements = e.Result.Select(x=> new StringElement(x.Name,x.Email) as Element).ToList()}},true));
			});
		}
		
		void HandleClientUploadImageCompleted (object sender, UploadImageCompletedEventArgs e)
		{
			loading.Hide(true);
			Console.WriteLine (e.Result);
		}
		
		void HandleClientGetTasksCompleted (object sender, GetTasksCompletedEventArgs e)
		{			
			loading.Hide(true);
			if(e.Error != null)
			{
				var alert = new UIAlertView("Error",e.Error.Message,null,"Ok");
				alert.Show();
				return;
			}
			
			this.BeginInvokeOnMainThread(delegate{
				dvc.ActivateController(new DialogViewController(new RootElement("Users"){new Section(){Elements = e.Result.Items.Select(x=> new BooleanElement(x.Title,false) as Element).ToList()}},true));
			});
		}

		private static ImageData GetImageData (string imageFile)
		{
			var path = Path.Combine (Util.PicDir, imageFile);
			if (!File.Exists (path))
				return null;
			return new ImageData (){FileName = imageFile, ImageContent = File.ReadAllBytes (path)};
		}
	}
}

