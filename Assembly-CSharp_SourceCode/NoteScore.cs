using System.Collections.Generic;

public class NoteScore
{
	public enum EScoreType
	{
		Tap,
		Hold,
		Slide,
		Break,
		Touch,
		BreakBonus,
		End
	}

	public enum EDeluxScoreType
	{
		None,
		Great,
		Perfect,
		Critical,
		End
	}

	private class JuggeScore
	{
		private uint[] judgeScoreList = new uint[15];

		public uint this[int i]
		{
			get
			{
				return judgeScoreList[i];
			}
			set
			{
				judgeScoreList[i] = value;
			}
		}

		public JuggeScore(uint[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				judgeScoreList[i] = array[i];
			}
		}
	}

	private static readonly List<JuggeScore> judgeScoreTbl = new List<JuggeScore>
	{
		new JuggeScore(new uint[15]
		{
			0u, 250u, 400u, 400u, 400u, 500u, 500u, 500u, 500u, 500u,
			400u, 400u, 400u, 250u, 0u
		}),
		new JuggeScore(new uint[15]
		{
			0u, 500u, 800u, 800u, 800u, 1000u, 1000u, 1000u, 1000u, 1000u,
			800u, 800u, 800u, 500u, 0u
		}),
		new JuggeScore(new uint[15]
		{
			0u, 750u, 1200u, 1200u, 1200u, 1500u, 1500u, 1500u, 1500u, 1500u,
			1200u, 1200u, 1200u, 750u, 0u
		}),
		new JuggeScore(new uint[15]
		{
			0u, 1000u, 1250u, 1500u, 2000u, 2500u, 2500u, 2500u, 2500u, 2500u,
			2000u, 1500u, 1250u, 1000u, 0u
		}),
		new JuggeScore(new uint[15]
		{
			0u, 250u, 400u, 400u, 400u, 500u, 500u, 500u, 500u, 500u,
			400u, 400u, 400u, 250u, 0u
		}),
		new JuggeScore(new uint[15]
		{
			0u, 30u, 40u, 40u, 40u, 50u, 75u, 100u, 75u, 50u,
			40u, 40u, 40u, 30u, 0u
		})
	};

	public static uint GetJudgeScore(NoteJudge.ETiming judge, EScoreType scoreType = EScoreType.Tap)
	{
		return judgeScoreTbl[(int)scoreType][(int)judge];
	}
}
