namespace TebyanFilmRobat.Models
{
	public class TebyanMovieRequestModel
	{
		public TebyanMovieRequestToolsModel tools { get; set; }
	}

	public class TebyanMovieRequestToolsModel
	{
		public int CategoryID { get; set; }
		public int CountDayOld { get; set; }
		public int PageIndex { get; set; }
		public int PageSize
		{
			get { return 21; }
		}
		public string Order
		{
			get { return "desc"; }
		}
		public string OrderBy
		{
			get { return "ShowDate"; }
		}
		public bool VideoSharing { get; set; }
		public bool TebyanFilm
		{
			get
			{
				return true;
			}
		}
	}
}
