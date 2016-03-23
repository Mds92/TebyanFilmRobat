using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TebyanFilmRobat.Models;
using TebyanFilmRobat.Robots;
using ZetaLongPaths;

namespace TebyanFilmRobat
{
	public partial class FormMain : Form
	{
		public FormMain()
		{
			InitializeComponent();
		}

		#region Properties

		Thread RobotThread { get; set; }

		int FromPage
		{
			get
			{
				int fromPage;
				int.TryParse(maskedTextBoxFromPage.Text, out fromPage);
				return fromPage;
			}
		}
		int ToPage
		{
			get
			{
				int toPage;
				int.TryParse(maskedTextBoxToPage.Text, out toPage);
				return toPage;
			}
		}
		int CategoryId
		{
			get
			{
				int categoryId;
				int.TryParse(maskedTextBoxCategoryId.Text, out categoryId);
				return categoryId;
			}
		}

		#endregion

		#region Helpers

		private int _lineNumber;
		const int MaxLineNumber = 5000;
		void LogMessage(string message)
		{
			if (string.IsNullOrWhiteSpace(message)) return;
			message += Environment.NewLine;
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action<string>(messageString =>
				{
					if (_lineNumber > MaxLineNumber)
					{
						textBoxLogs.Clear();
						_lineNumber = 0;
					}
					textBoxLogs.AppendText(messageString);
					_lineNumber++;
					textBoxLogs.ScrollToCaret();
				}), message);
			}
			else
			{
				_lineNumber++;
				if (_lineNumber > MaxLineNumber)
				{
					textBoxLogs.Clear();
					_lineNumber = 0;
				}
				textBoxLogs.AppendText(message);
				textBoxLogs.ScrollToCaret();
			}
			Application.DoEvents();
		}

		#endregion

		#region Buttons

		private void buttonStart_Click(object sender, EventArgs e)
		{
			buttonStart.Enabled = false;
			buttonStop.Enabled = true;
			if (RobotThread != null && RobotThread.ThreadState != ThreadState.Stopped) return;
			ThreadStart threadStart = () =>
			{
				try
				{
					List<TebyanMovieModel> tebyanMovieModels =
						Task.Run(() => TebyanMoviesRobot.GetLinks(CategoryId, FromPage, ToPage, LogMessage)).GetAwaiter().GetResult();

					#region success

					var successLinks = tebyanMovieModels.Where(q => q.IsSuccess).Distinct().ToList();
					StringBuilder stringBuilder = new StringBuilder("");
					StringBuilder htmlStringBuilder =
						new StringBuilder(
							"<!DOCTYPE HTML><html><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\"><head><title>لینک های دانلود</title></head><body><ul style=\"direction:rtl;\">");

					int coutner = 0;
					foreach (var item in successLinks)
					{
						coutner++;
						stringBuilder.AppendLine(item.DownloadUrl);
						htmlStringBuilder.AppendLine(string.Format("<li><a href=\"{0}\">{1}- {2}</a></li>", item.DownloadUrl, coutner, item.Title));
					}
					htmlStringBuilder.AppendLine("</ul><div>By Mohammad Dayyan, 1395</div></body></html>");

					ZlpIOHelper.WriteAllText(UploadedPaths.SuccessLinksFileName, stringBuilder.ToString(), Encoding.UTF8);
					ZlpIOHelper.WriteAllText(UploadedPaths.SuccessHtmlLinksFileName, htmlStringBuilder.ToString(), Encoding.UTF8);

					#endregion

					#region failed links

					var failedLinks = tebyanMovieModels.Where(q => !q.IsSuccess).Distinct().ToList();
					stringBuilder = new StringBuilder("");

					foreach (var item in failedLinks)
						stringBuilder.AppendLine(item.PageUrl);

					ZlpIOHelper.WriteAllText(UploadedPaths.FailedLinksFileName, stringBuilder.ToString(), Encoding.UTF8);

					#endregion
				}
				catch (Exception ex)
				{
					LogMessage(MdExceptionHelper.GetExceptionDetails(ex));
				}
				finally
				{
					buttonStop_Click(null, null);
				}
			};
			RobotThread = new Thread(threadStart)
			{
				IsBackground = true
			};
			RobotThread.Start();
		}

		private void buttonStop_Click(object sender, EventArgs e)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action(() =>
				{
					buttonStart.Enabled = true;
					buttonStop.Enabled = false;
				}));
			}
			else
			{
				buttonStart.Enabled = true;
				buttonStop.Enabled = false;
			}
			if (RobotThread != null && RobotThread.ThreadState == ThreadState.Running)
				RobotThread.Abort();
			ZlpIOHelper.DeleteDirectory(UploadedPaths.TempPath, true);
		}

		#endregion
	}
}
