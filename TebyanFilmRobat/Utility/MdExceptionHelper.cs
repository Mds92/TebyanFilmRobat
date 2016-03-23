using System;

namespace TebyanFilmRobat
{
	public static class MdExceptionHelper
	{
		public static string GetExceptionDetails(Exception ex)
		{
			Exception exception = ex;
			string exceptionMessage = "Exception :";
			exceptionMessage += string.Format("Message: {0}", exception.Message);
			while (exception.InnerException != null)
			{
				exceptionMessage += string.Format("{0}InnerMessage: {1}", Environment.NewLine, exception.InnerException.Message);
				exception = exception.InnerException;
			}
			return exceptionMessage;
		}
	}
}