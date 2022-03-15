using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manager
{
	public class TouchChainList : List<int>
	{
		public int RootNoteId;

		public int EndCount;

		public NoteJudge.ETiming ChainJudge = NoteJudge.ETiming.TooLate;

		public const float ChainBorder = 0.51f;

		public bool Empty()
		{
			return !this.Any();
		}

		public bool IsChainJudge()
		{
			return EndCount >= Mathf.CeilToInt((float)base.Count * 0.51f);
		}
	}
}
