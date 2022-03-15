namespace Manager.UserDatas
{
	public class UserScoreRanking
	{
		public int tournamentId { get; set; }

		public long totalScore { get; set; }

		public int ranking { get; set; }

		public string rankingDate { get; set; }

		public UserScoreRanking()
		{
			Clear();
		}

		public UserScoreRanking(int id, long score, int rank, string date)
		{
			tournamentId = id;
			totalScore = score;
			ranking = rank;
			rankingDate = date;
		}

		public void Clear()
		{
			tournamentId = 0;
			totalScore = 0L;
			ranking = 0;
			rankingDate = "";
		}
	}
}
