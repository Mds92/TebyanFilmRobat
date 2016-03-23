using System.Reflection;
using ZetaLongPaths;

namespace TebyanFilmRobat
{
	public static class UploadedPaths
	{
		static string _applicationPath;
		public static string ApplicationPath
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_applicationPath)) return _applicationPath;
				_applicationPath = ZlpPathHelper.GetDirectoryPathNameFromFilePath(Assembly.GetExecutingAssembly().Location);
				return _applicationPath;
			}
		}

		static string _successLinksFileName;
		public static string SuccessLinksFileName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_successLinksFileName)) return _successLinksFileName;
				_successLinksFileName = ZlpPathHelper.Combine(ApplicationPath, "success-links.txt");
				return _successLinksFileName;
			}
		}

		static string _failedLinksFileName;
		public static string FailedLinksFileName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_failedLinksFileName)) return _failedLinksFileName;
				_failedLinksFileName = ZlpPathHelper.Combine(ApplicationPath, "failed-links.txt");
				return _failedLinksFileName;
			}
		}

		static string _successHtmlLinksFileName;
		public static string SuccessHtmlLinksFileName
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_successHtmlLinksFileName)) return _successHtmlLinksFileName;
				_successHtmlLinksFileName = ZlpPathHelper.Combine(ApplicationPath, "success-html-links.html");
				return _successHtmlLinksFileName;
			}
		}

		static string _wgetPath;
		public static string WgetPath
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_wgetPath)) return _wgetPath;
				_wgetPath = ZlpPathHelper.Combine(ApplicationPath, "Tools\\wget.exe");
				return _wgetPath;
			}
		}

		static string _aria2CPath;
		public static string Aria2CPath
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_aria2CPath)) return _aria2CPath;
				_aria2CPath = ZlpPathHelper.Combine(ApplicationPath, "Tools\\aria2c.exe");
				return _aria2CPath;
			}
		}

		private static string _aria2ConfigPath;
		public static string AbsoluteAria2ConfigPath
		{
			get
			{

				if (!string.IsNullOrWhiteSpace(_aria2ConfigPath)) return _aria2ConfigPath;
				_aria2ConfigPath = ZlpPathHelper.Combine(ApplicationPath, "Tools\\aria2.conf");
				return _aria2ConfigPath;
			}
		}

		private static string _wgetIniPath;
		public static string WgetIniPath
		{
			get
			{

				if (!string.IsNullOrWhiteSpace(_wgetIniPath)) return _wgetIniPath;
				_wgetIniPath = ZlpPathHelper.Combine(ApplicationPath, "Tools\\wget.ini");
				return _wgetIniPath;
			}
		}

		private static string _tempPath;
		public static string TempPath
		{
			get
			{

				if (!string.IsNullOrWhiteSpace(_tempPath)) return _tempPath;
				_tempPath = ZlpPathHelper.Combine(ApplicationPath, "Temp");
				return _tempPath;
			}
		}
	}
}