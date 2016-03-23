using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using ZetaLongPaths;

namespace TebyanFilmRobat
{
	public class Aria2CDownloader
	{
		public static void DownloadFile(string fileUrlToDownload, string destinationAbsoluteFileName, string proxy, Action<string, LeechersActionTypeEnum> action)
		{
			if (!File.Exists(UploadedPaths.Aria2CPath))
				throw new Exception(string.Format("شما باید فایل aria2c.exe را در مسیر '{0}' قرار دهید", UploadedPaths.Aria2CPath));

			string fileName = Path.GetFileName(destinationAbsoluteFileName);
			string directoryPath = Path.GetDirectoryName(destinationAbsoluteFileName);
			ZlpIOHelper.CreateDirectory(directoryPath);

			const int timeoutInMiliSeconds = 2 * 60 * 60 * 1000; // 2 hours

			Uri myUri = new Uri(fileUrlToDownload);
			string host = myUri.Host;

			Process currentProcess = Process.GetCurrentProcess();

			string arguments =
				"--header=\"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\" " +
				"--user-agent=\"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11\" " +
				" --allow-overwrite=true -x4 -s4 ";
			arguments += string.Format(" --referer=\"{0}\"", host);
			//arguments += string.Format(" --conf-path=\"{0}\"", UploadedPaths.AbsoluteAria2ConfigPath);
			arguments += string.Format(" --connect-timeout={0} --timeout={0}", 20);
			arguments += string.Format(" --stop-with-process={0}", currentProcess.Id);
			arguments += string.Format(" --dir \"{0}\"", directoryPath);
			arguments += string.Format(" --out \"{0}\" \"{1}\"", fileName, fileUrlToDownload);
			if (!string.IsNullOrWhiteSpace(proxy))
				arguments += string.Format(" --all-proxy={0}", proxy);

			using (Process process = new Process())
			{
				process.StartInfo.FileName = UploadedPaths.Aria2CPath;
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

		public static Tuple<long, string> GetFileSizeInBytesWithMimeType(string fileUrl, string proxy, Action<string, LeechersActionTypeEnum> action)
		{
			if (!File.Exists(UploadedPaths.Aria2CPath))
				throw new Exception(string.Format("شما باید فایل aria2c.exe را در مسیر {0} قرار دهید", UploadedPaths.Aria2CPath));

			const int timeoutInMiliSeconds = 5 * 1000; // 5 seconds

			string arguments = string.Format("--dry-run=true --user-agent=\"Mozilla/5.0 (Windows NT 6.0) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11\" \"{0}\"", fileUrl);
			if (!string.IsNullOrWhiteSpace(proxy))
				arguments += string.Format(" --all-proxy={0}", proxy);

			using (Process process = new Process())
			{
				process.StartInfo.FileName = UploadedPaths.Aria2CPath;
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
						catch
						{
							// ignored
						}
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
						catch
						{
							// ignored
						}
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

		public static void ChangeConfigProxy(string proxy)
		{
			if (!File.Exists(UploadedPaths.Aria2CPath))
				throw new Exception(string.Format("شما باید فایل aria2c.exe را در مسیر '{0}' قرار دهید", UploadedPaths.Aria2CPath));

			const string comment = "# Configuration file for aria2c by Mohammad Dayyan";
			proxy = string.Format("all-proxy={0}", proxy);

			using (File.Open(UploadedPaths.AbsoluteAria2ConfigPath, FileMode.OpenOrCreate)) { }
			File.WriteAllLines(UploadedPaths.AbsoluteAria2ConfigPath, new[] { comment, proxy }, Encoding.ASCII);
		}

		static Tuple<long, string> GetFileSizeInBytesWithMimeType(string lengthString)
		{
			long length = long.Parse(Regex.Match(lengthString, @"(?<=\s)\d+").Value);
			string mimeType = Regex.Match(lengthString, @"(?<=\[)[^\]]*").Value;

			return new Tuple<long, string>(length, mimeType.ToLower());
		}
	}
}
