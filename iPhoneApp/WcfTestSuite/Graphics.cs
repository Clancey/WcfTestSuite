//
// Utilities for dealing with graphics
//
// Copyright 2010 Miguel de Icaza
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreAnimation;
using MonoTouch.Dialog;

namespace WcfTestSuite
{
	public static class Graphics
	{
		static CGPath smallPath = GraphicsUtil.MakeRoundedPath (57, 4);
		static CGPath largePath = GraphicsUtil.MakeRoundedPath (114, 4);
		
		// Check for multi-tasking as a way to determine if we can probe for the "Scale" property,
		// only available on iOS4 
		public static bool HighRes = UIDevice.CurrentDevice.IsMultitaskingSupported && UIScreen.MainScreen.Scale > 1;
		
		static Selector sscale;
		
		internal static void ConfigLayerHighRes (CALayer layer)
		{
			if (!HighRes)
				return;
			
			if (sscale == null)
				sscale = new Selector ("setContentsScale:");
			
			Messaging.void_objc_msgSend_float (layer.Handle, sscale.Handle, 2.0f);
		}
		
		// Child proof the image by rounding the edges of the image
		internal static UIImage RemoveSharpEdges (UIImage image)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			
			float size =  57;
			
			UIGraphics.BeginImageContext (new SizeF (size, size));
			var c = UIGraphics.GetCurrentContext ();
			
			//if (HighRes)
			//	c.AddPath (largePath);
			//else 
				c.AddPath (smallPath);
			
			c.Clip ();
			image.Draw (new RectangleF (0, 0, size, size));
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;
		}
		
		const float width = 640;
		internal static UIImage PrepareForUpload(UIImage image)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			
			var max = Math.Max(image.Size.Height,image.Size.Width);

			float scale = 1f;
			if(max > width)
				scale = width / max;
			else
				return image;
				

			UIGraphics.BeginImageContext (new SizeF (image.Size.Width * scale, image.Size.Height * scale));
			var c = UIGraphics.GetCurrentContext ();
			
			image.Draw (new RectangleF (0, 0, image.Size.Width * scale, image.Size.Height * scale));
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;	
		}
		internal static UIImage RemoveSharpEdges (UIImage image,float size,float radius)
		{
			if (image == null)
				throw new ArgumentNullException ("image");
			
			UIGraphics.BeginImageContext (new SizeF (size, size));
			var c = UIGraphics.GetCurrentContext ();
			
			//if (HighRes)
			//	c.AddPath (largePath);
			//else 
				c.AddPath (GraphicsUtil.MakeRoundedPath (size, radius));
			
			c.Clip ();
			image.Draw (new RectangleF (0, 0, size, size));
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;
		}
		
		//
		// Centers image, scales and removes borders
		//
		internal static UIImage PrepareForProfileView (UIImage image)
		{
			float size = 114;
			if (image == null)
				image = UIImage.FromFile("icon.png");
			
			UIGraphics.BeginImageContext (new SizeF (size, size));
			var c = UIGraphics.GetCurrentContext ();
			
			c.AddPath (largePath);
			c.Clip ();

			// Twitter not always returns squared images, adjust for that.
			var cg = image.CGImage;
			float width = cg.Width;
			float height = cg.Height;
			if (width != height){
				float x = 0, y = 0;
				if (width > height){
					x = (width-height)/2;
					width = height;
				} else {
					y = (height-width)/2;
					height = width;
				}
				c.ScaleCTM (1, -1);
				using (var copy = cg.WithImageInRect (new RectangleF (x, y, width, height))){
					c.DrawImage (new RectangleF (0, 0, size, -size), copy);
				}
			} else 
				image.Draw (new RectangleF (0, 0, size, size));
			
			var converted = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return converted;
		}
	}
	
	
}
