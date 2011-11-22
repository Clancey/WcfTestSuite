using System;
using System.IO;

namespace WcfTestSuite
{
	public static class Util
	{
		public static readonly string BaseDir = Directory.GetParent(Environment.GetFolderPath (Environment.SpecialFolder.Personal)).ToString();
		public static readonly string PicDir = Path.Combine (BaseDir, "Library/Caches/Pictures/");
	}
}

