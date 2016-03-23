using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ZetaLongPaths;

namespace TebyanFilmRobat
{
	public class WgetDownloader
	{
		public static void DownloadFile(string fileUrlToDownload, string destinationAbsoluteFileName,
			Action<string, LeechersActionTypeEnum> action)
		{
			if (!File.Exists(UploadedPaths.WgetPath))
				throw new Exception(string.Format("شما باید فایل wget.exe را در مسیر '{0}' قرار دهید", UploadedPaths.WgetPath));

			string directoryPath = Path.GetDirectoryName(destinationAbsoluteFileName);
			ZlpIOHelper.CreateDirectory(directoryPath);

			const int timeoutInMiliSeconds = 2 * 60 * 60 * 1000; // 2 hours
			string arguments = string.Format("-O \"{0}\" \"{1}\" " +
			                                 "--header=\"Accept:  text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\" " +
			                                 "--user-agent=\"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11\"",
				destinationAbsoluteFileName, fileUrlToDownload);

			using (Process process = new Process())
			{
				process.StartInfo.FileName = UploadedPaths.WgetPath;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

				using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
				using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
				{
					process.OutputDataReceived += (sender, e) =>
					{
						try
						{
							if (e.Data == null)
								outputWaitHandle.Set();
							else if (action != null)
								action(e.Data, LeechersActionTypeEnum.Normal);
						}
						catch (Exception ex)
						{
							if (action != null)
								action(string.Format("{0} <br /> {1}", ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.Message), LeechersActionTypeEnum.Error);
						}
					};
					process.ErrorDataReceived += (sender, e) =>
					{
						try
						{
							if (e.Data == null)
								errorWaitHandle.Set();
							else if (action != null)
								action(e.Data, LeechersActionTypeEnum.Normal);
						}
						catch (Exception ex)
						{
							if (action != null)
								action(string.Format("{0} <br /> {1}", ex.Message, ex.InnerException == null ? string.Empty : ex.InnerException.Message), LeechersActionTypeEnum.Error);
						}
					};

					process.Start();

					process.BeginErrorReadLine();
					process.BeginOutputReadLine();

					if (process.WaitForExit(timeoutInMiliSeconds) &&
						(outputWaitHandle.WaitOne(timeoutInMiliSeconds) || errorWaitHandle.WaitOne(timeoutInMiliSeconds)))
					{
						if (!process.HasExited)
							process.Kill();
						if (action != null)
							action(string.Format("'{0}' Successfully Downloaded", fileUrlToDownload), LeechersActionTypeEnum.Success);
						return;
					}
					if (!process.HasExited)
						process.Kill();

					if (action != null)
						action(string.Format("Error in Downloading '{0}'", fileUrlToDownload), LeechersActionTypeEnum.Error);
				}
			}
		}

		public static Tuple<long, string> GetFileSizeInBytesWithMimeType(string fileUrl,
			Action<string, LeechersActionTypeEnum> action)
		{
			if (!File.Exists(UploadedPaths.WgetPath))
			{
				throw new Exception(string.Format("شما باید فایل wget.exe را در مسیر {0} قرار دهید",
					UploadedPaths.WgetPath));
			}

			const int timeoutInMiliSeconds = 5 * 1000; // 5 seconds

			string arguments = 
				string.Format("--spider \"{0}\" " +
			                "--user-agent=\"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11\"",
							fileUrl);

			using (Process process = new Process())
			{
				process.StartInfo.FileName = UploadedPaths.WgetPath;
				process.StartInfo.Arguments = arguments;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

				string lengthString = "0";

				using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
				using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
				{
					process.OutputDataReceived += (sender, e) =>
					{
						try
						{
							if (e.Data == null)
								outputWaitHandle.Set();
							else if (e.Data.Contains("Length"))
								lengthString = e.Data;
							if (action != null)
								action(e.Data, LeechersActionTypeEnum.Normal);
						}
						catch { }

					};
					process.ErrorDataReceived += (sender, e) =>
					{
						try
						{
							if (e.Data == null)
								errorWaitHandle.Set();
							else if (e.Data.Contains("Length"))
								lengthString = e.Data;
							if (action != null)
								action(e.Data, LeechersActionTypeEnum.Normal);
						}
						catch { }
					};

					process.Start();

					process.BeginOutputReadLine();
					process.BeginErrorReadLine();

					process.WaitForExit(timeoutInMiliSeconds);
					outputWaitHandle.WaitOne(timeoutInMiliSeconds);
					errorWaitHandle.WaitOne(timeoutInMiliSeconds);

					if (process.WaitForExit(timeoutInMiliSeconds) &&
						(outputWaitHandle.WaitOne(timeoutInMiliSeconds) || errorWaitHandle.WaitOne(timeoutInMiliSeconds)))
					{
						if (!process.HasExited)
							process.Kill();

						return GetFileSizeInBytesWithMimeType(lengthString);
					}

					if (!process.HasExited)
						process.Kill();

					if (!lengthString.ToLower().Contains("length"))
						throw new Exception("در عملیات دریافت اطلاعات نوع و سایز فایل خطایی رخ داده، دوباره تلاش کنید");

					return GetFileSizeInBytesWithMimeType(lengthString);
				}
			}
		}

		public static void ChangeProxy(string proxy)
		{
			if (!File.Exists(UploadedPaths.WgetPath))
				throw new Exception(string.Format("شما باید فایل wget.exe را در مسیر '{0}' قرار دهید", UploadedPaths.WgetPath));

			proxy = string.IsNullOrWhiteSpace(proxy) || proxy.Contains("http")
				? proxy
				: string.Format("http://{0}/", proxy);

			string httpProxy = string.IsNullOrWhiteSpace(proxy)
				? string.Format("http_proxy=")
				: string.Format("http_proxy={0}", proxy);
			string httpsProxy = string.IsNullOrWhiteSpace(proxy)
				? string.Format("https_proxy=")
				: string.Format("https_proxy={0}", proxy);
			string ftpProxy = string.IsNullOrWhiteSpace(proxy)
				? string.Format("ftp_proxy=")
				: string.Format("ftp_proxy={0}", proxy);

			using (File.Open(UploadedPaths.WgetIniPath, FileMode.OpenOrCreate)) { }
			File.WriteAllLines(UploadedPaths.WgetIniPath, new[] { httpProxy, httpsProxy, ftpProxy },
				new UTF8Encoding(false));
		}

		static Tuple<long, string> GetFileSizeInBytesWithMimeType(string lengthString)
		{
			long length = long.Parse(Regex.Match(lengthString, @"(?<=\s)\d+").Value);
			string mimeType = Regex.Match(lengthString, @"(?<=\[)[^\]]*").Value;

			return new Tuple<long, string>(length, mimeType.ToLower());
		}
	}
}