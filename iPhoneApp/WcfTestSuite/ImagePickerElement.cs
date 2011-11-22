using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using ClanceysLib;
using System.IO;
using System.Threading;

namespace WcfTestSuite
{	
	public class ImagePickerElement : Element ,IElementSizing {
		public string Value;
		static RectangleF rect = new RectangleF (0, 0, dimx, dimy);
		static NSString ikey = new NSString ("ImagePickerElement");
		UIImage scaled;
		UIPopoverController popover;
		static string DefaulImage ="noImage.png";
		public Action<string> OnSelect {get;set;}
		// Apple leaks this one, so share across all.
		static UIImagePickerController picker;
		
		// Height for rows
		const int dimx = 150;
		const int dimy = 150;
		
		// radius for rounding
		const int rad = 10;
		
		public ImagePickerElement (string imagePath,string text) : base ("")
		{
			Value = imagePath;
			Caption = text;
			var image = UIImage.FromFile(Path.Combine(Util.PicDir,imagePath));
			if (image == null){
				scaled = Graphics.RemoveSharpEdges(UIImage.FromFile(DefaulImage),dimx,rad);
			} else {		
				scaled = Graphics.RemoveSharpEdges(image,dimx,rad);
			}
		}
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell (ikey);
			if (cell == null){
				cell = new UITableViewCell (UITableViewCellStyle.Default, ikey);
			}
			
			if (scaled == null)
				return cell;
			
			//Section psection = Parent as Section;
			cell.ImageView.Frame = new RectangleF(10,10,dimx,dimy);
			cell.ImageView.Image = scaled;
			cell.TextLabel.Text = Caption;
			cell.TextLabel.Lines = 0;
			cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			return cell;
		}
		
		protected override void Dispose (bool disposing)
		{
			if (disposing){
				if (scaled != null){
					scaled.Dispose ();
					scaled = null;
					Value = null;
				}
			}
			base.Dispose (disposing);
		}

		class MyDelegate : UIImagePickerControllerDelegate {
			ImagePickerElement container;
			UITableView table;
			NSIndexPath path;
			
			public MyDelegate (ImagePickerElement container, UITableView table, NSIndexPath path)
			{
				this.container = container;
				this.table = table;
				this.path = path;
			}
			
			public override void FinishedPickingImage (UIImagePickerController picker, UIImage image, NSDictionary editingInfo)
			{
				container.Picked (image);
				table.ReloadRows (new NSIndexPath [] { path }, UITableViewRowAnimation.None);
			}
			public override void Canceled (UIImagePickerController picker)
			{
				container.Picked (null);
				table.ReloadRows (new NSIndexPath [] { path }, UITableViewRowAnimation.None);
			}
		}
		MBProgressHUD savingMessage;
		UIImage selectedImage;
		void Picked (UIImage image)
		{
			if(image == null)
			{
				completed();
				return;
			}
			Console.WriteLine(image.Orientation);
			savingMessage = new MBProgressHUD();
			savingMessage.TitleText = "Saving";
			savingMessage.Show(true);
			selectedImage = image;
			Thread thread = new Thread(save);
			thread.Start();			
		}
		const float width = 640;
		void save()
		{
			using (new NSAutoreleasePool())
			{
				NSError error = new NSError();
				selectedImage = Graphics.PrepareForUpload(selectedImage);
				selectedImage.AsJPEG().Save(Path.Combine(Util.PicDir,Value),NSDataWritingOptions.Atomic,out error);
				NSObject invoker = new NSObject();
				invoker.InvokeOnMainThread(delegate {
					completed();
				});
				
			}
		}
		void completed()
		{
			if(savingMessage != null)
				savingMessage.Hide(true);
			if(selectedImage != null)
				scaled = Graphics.RemoveSharpEdges(selectedImage,dimx,rad);
			if(selectedImage != null)
				selectedImage.Dispose();
			currentController.DismissModalViewControllerAnimated (true);
			if(OnSelect != null)
				OnSelect(Value);
			
		}
		
		UIViewController currentController;
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			if (picker == null)
				picker = new UIImagePickerController ();
			picker.Delegate = new MyDelegate (this, tableView, path);
			
			var hasCamera = picker.MediaTypes;
			picker.SourceType = hasCamera.Length == 1 ? UIImagePickerControllerSourceType.PhotoLibrary : UIImagePickerControllerSourceType.Camera;
			
			switch (UIDevice.CurrentDevice.UserInterfaceIdiom){
			case UIUserInterfaceIdiom.Pad:
				RectangleF useRect;
				popover = new UIPopoverController (picker);
				var cell = tableView.CellAt (path);
				if (cell == null)
					useRect = rect;
				else
					rect = cell.Frame;
				popover.PresentFromRect (rect, dvc.View, UIPopoverArrowDirection.Any, true);
				break;
				
			default:
			case UIUserInterfaceIdiom.Phone:
			dvc.ActivateController (picker);
				break;
			}
			currentController = dvc;
		}

		#region IElementSizing implementation
		float IElementSizing.GetHeight (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return 170;
		}
		#endregion
	}
}

