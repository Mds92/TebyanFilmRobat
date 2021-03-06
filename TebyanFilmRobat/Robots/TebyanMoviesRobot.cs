﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;
using TebyanFilmRobat.Models;
using ZetaLongPaths;

namespace TebyanFilmRobat.Robots
{
	public class TebyanMoviesRobot
	{
		#region Const

		const string TebyanFilmDomain = "http://film.tebyan.net/";
		const string TebyanFilmPageStringFormat = "http://film.tebyan.net/film/{0}";
		const string TebyanFilmGetCategoryPageApiAddress = "WebServices/Music/Music.asmx/GetCategoryPage";

		#endregion

		/// <summary>
		/// بدست آوردن اچ تی ام ال یک صفحه
		/// </summary>
		/// <param name="categoryId">گروه</param>
		/// <param name="page">صفحه</param>
		/// <returns>اچ تی ام ال صفحه مورد نظر</returns>
		static async Task<KeyValuePair<int, string>> GetPage(int categoryId, int page)
		{
			ApiCaller.BaseAddress = TebyanFilmDomain;
			TebyanMovieRequestModel requestModel = new TebyanMovieRequestModel
			{
				tools = new TebyanMovieRequestToolsModel
				{
					CategoryID = categoryId,
					PageIndex = page
				}
			};
			object result =
				await ApiCaller.SendRequestAsync<object>(TebyanFilmGetCategoryPageApiAddress, typeof(TebyanMovieRequestModel), requestModel, HttpMethodsEnum.Post);
			TebyanMovieResponseModel tebyanMovieResponseModel = JsonConvert.DeserializeObject<TebyanMovieResponseModel>(result.ToString());
			return new KeyValuePair<int, string>(page, tebyanMovieResponseModel.d);
		}

		static async Task<TebyanMovieModel> GetDownloadLink(string pageUrl)
		{
			return await Task.Run(() =>
			{
				TebyanMovieModel model = new TebyanMovieModel
				{
					PageUrl = pageUrl
				};
				string tempFilePath = ZlpPathHelper.Combine(UploadedPaths.TempPath, string.Format("{0}.html", Guid.NewGuid()));
				Aria2CDownloader.DownloadFile(pageUrl, tempFilePath, "", null);
				if (!ZlpIOHelper.FileExists(tempFilePath))
					return model;

				var htmlDocument = new HtmlDocument();
				htmlDocument.Load(tempFilePath, Encoding.UTF8);

				var titleTag = htmlDocument.DocumentNode.Descendants("title").FirstOrDefault();
				if (titleTag == null)
					return model;

				var downloadLink = htmlDocument.DocumentNode.Descendants("a")
					.FirstOrDefault(q => q.Attributes != null &&
										 q.Attributes["class"] != null &&
										 q.Attributes["class"].Value.IndexOf("DownloadFilm", StringComparison.InvariantCultureIgnoreCase) > -1);
				if (downloadLink == null)
					return model;

				model.IsSuccess = true;
				model.PageUrl = pageUrl;
				model.DownloadUrl = downloadLink.GetAttributeValue("href", "");
				model.Title = string.Format("{0}, {1}", titleTag.InnerText.Trim(), downloadLink.GetAttributeValue("title", ""));

				return model;
			});
		}

		/// <summary>
		/// بدست آوردن تمامی لینک های دانلود
		/// </summary>
		public static List<TebyanMovieModel> GetLinks(int categoryId, int startPage, int endPage, Action<string> logAction)
		{
			try
			{
				if (!ZlpIOHelper.DirectoryExists(UploadedPaths.TempPath))
					ZlpIOHelper.CreateDirectory(UploadedPaths.TempPath);
				List<TebyanMovieModel> contentsUrl = new List<TebyanMovieModel>();
				logAction("By Mohammad Dayyan, 1395");
				logAction(new string('~', 70));

				List<string> pageUrlsToGetDownloadLinks = new List<string>(); // آدرس صفحات برای پیدا کردن لینک های دانلود از داخل آنها

				// سایت تبیان بیشتر از 25 ریکوئست همزمان جواب نمیده
				// برای همین من اومدم 10 تا 10 تا ریکوئست ها رو میفرستم
				const int concurrentRequests = 10;

				int startPageIndex = startPage <= 0 ? 0 : startPage - 1;
				int endPageIndex = endPage <= 0 ? 0 : endPage - 1;

				int tempStartPageIndex = startPageIndex;
				int tempEndPageIndex = endPageIndex > concurrentRequests ? concurrentRequests : endPageIndex;

				#region بدست آوردن لینک صفحات

				while (true)
				{
					List<Task<KeyValuePair<int, string>>> getPageLinksTasks = new List<Task<KeyValuePair<int, string>>>();

					for (int i = tempStartPageIndex; i <= tempEndPageIndex; i++)
						getPageLinksTasks.Add(GetPage(categoryId, i));

					foreach (Task<KeyValuePair<int, string>> task in getPageLinksTasks)
					{
						KeyValuePair<int, string> keyValuePair = task.GetAwaiter().GetResult();
						int page = keyValuePair.Key;
						string responseHtml = keyValuePair.Value;
						logAction(string.Format("{0} بررسی صفحه", page + 1));

						HtmlDocument htmlDocument = new HtmlDocument();
						htmlDocument.LoadHtml(responseHtml);

						var pageLinksHtmlNodes = htmlDocument.DocumentNode.Descendants("a")
							.Where(
								q =>
									q.Attributes != null && q.Attributes["href"] != null && q.Attributes["class"] != null &&
									q.Attributes["class"].Value.IndexOf("RedBullet", StringComparison.InvariantCultureIgnoreCase) > -1 &&
									q.Attributes["href"].Value.StartsWith("/film", true, CultureInfo.InvariantCulture))
							.ToList();

						foreach (HtmlNode itemHtmlNode in pageLinksHtmlNodes)
						{
							int linkCode = 0;
							var relativePageLink = itemHtmlNode.Attributes["href"].Value;
							var codeMatch = Regex.Match(relativePageLink, @"\d+$", RegexOptions.IgnoreCase);
							if (codeMatch.Success)
								int.TryParse(codeMatch.Value, out linkCode);
							if (linkCode <= 0)
							{
								logAction(string.Format("'{0}' فاقد کد صحیح است", relativePageLink));
								continue;
							}
							string pageUrl = string.Format(TebyanFilmPageStringFormat, linkCode);
							pageUrlsToGetDownloadLinks.Add(pageUrl);
						}
					}

					if (tempEndPageIndex >= endPageIndex) break;

					if (tempStartPageIndex < endPageIndex)
					{
						tempStartPageIndex = tempStartPageIndex + concurrentRequests + 1;
						if (tempStartPageIndex > endPageIndex)
							tempStartPageIndex = endPageIndex;
					}
					if (tempEndPageIndex < endPageIndex)
					{
						tempEndPageIndex = tempEndPageIndex + concurrentRequests + 1;
						if (tempEndPageIndex > endPageIndex)
							tempEndPageIndex = endPageIndex;
					}
				}

				#endregion

				#region بدست آوردن لینک دانلودها

				int totalPage = pageUrlsToGetDownloadLinks.Count;
				logAction(string.Format("{0} محتوا یافت شد", totalPage));
				logAction("لطفا تا اتمام کار صبر نمایید");
				logAction(new string('/', 40));

				int counter = 0;
				endPageIndex = endPage <= 0 ? 0 : totalPage - 1;
				tempStartPageIndex = 0;
				tempEndPageIndex = endPageIndex > concurrentRequests ? concurrentRequests : endPageIndex;

				while (true)
				{
					List<Task<TebyanMovieModel>> getDownloadLinksTasks = new List<Task<TebyanMovieModel>>();

					for (int i = tempStartPageIndex; i <= tempEndPageIndex; i++)
					{
						string pageUrl = pageUrlsToGetDownloadLinks[i];
						getDownloadLinksTasks.Add(GetDownloadLink(pageUrl));
					}

					foreach (Task<TebyanMovieModel> task in getDownloadLinksTasks)
					{
						counter++;
						TebyanMovieModel tebyanMovieModel = task.GetAwaiter().GetResult();
						if (tebyanMovieModel.IsSuccess)
							logAction(string.Format("{0}/{1} دانلود شد", counter, totalPage));
						else
							logAction(string.Format("{0}/{1} خطا دارد", counter, totalPage));
						logAction(new string('-', 40));
						contentsUrl.Add(tebyanMovieModel);
					}

					if (tempEndPageIndex >= endPageIndex) break;

					if (tempStartPageIndex < endPageIndex)
					{
						tempStartPageIndex = tempStartPageIndex + concurrentRequests + 1;
						if (tempStartPageIndex > endPageIndex)
							tempStartPageIndex = endPageIndex;
					}
					if (tempEndPageIndex < endPageIndex)
					{
						tempEndPageIndex = tempEndPageIndex + concurrentRequests + 1;
						if (tempEndPageIndex > endPageIndex)
							tempEndPageIndex = endPageIndex;
					}
				}

				#endregion

				if (contentsUrl.Count > 0)
					logAction(string.Format("{0} لینک دانلود جمع آوری شد", contentsUrl.Count));
				else
					logAction("هیچ لینک دانلودی یافت نشد");

				logAction(new string('~', 70));
				logAction(string.Format("اتمام کار {0}", MD.PersianDateTime.PersianDateTime.Now));
				return contentsUrl;
			}
			catch (Exception ex)
			{
				logAction(ex.Message);
				return null;
			}
		}
	}
}
