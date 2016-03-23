using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TebyanFilmRobat
{
	public static class ApiCaller
	{
		public static string BaseAddress { get; set; }

		public static async Task<T> SendRequestAsync<T>(string apiAddress, 
			Type modelTypeToPost, object modelToPost,
			HttpMethodsEnum httpMethodsEnum)
		{
			Task<T> resultTask = null;
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(BaseAddress);
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpStatusCode httpStatusCode;
				switch (httpMethodsEnum)
				{
					case HttpMethodsEnum.Get:
						{
							HttpResponseMessage response = await client.GetAsync(apiAddress);
							httpStatusCode = response.StatusCode;
							if (response.IsSuccessStatusCode)
								resultTask = response.Content.ReadAsAsync<T>();
						}
						break;

					case HttpMethodsEnum.Post:
						{
							// HTTP POST
							var httpContent = new ObjectContent(modelTypeToPost, modelToPost, new JsonMediaTypeFormatter(), "application/json");
							HttpResponseMessage response = await client.PostAsync(apiAddress, httpContent);
							httpStatusCode = response.StatusCode;
							if (response.IsSuccessStatusCode)
								resultTask = response.Content.ReadAsAsync<T>();
						}
						break;
					default:
						throw new ArgumentOutOfRangeException("httpMethodsEnum", httpMethodsEnum, null);
				}
				if (resultTask == null)
					throw new Exception(string.Format("Error: Status code {0} {1} ", (int)httpStatusCode, httpStatusCode));
				return await resultTask;
			}
		}
	}
}
