using System;
using UnityEngine;

namespace Datas.DebugData
{
	[Serializable]
	public class DebugNoteScore
	{
		[SerializeField]
		private uint _criticalPerfect;

		[SerializeField]
		private uint _perfect;

		[SerializeField]
		private uint _great;

		[SerializeField]
		private uint _good;

		[SerializeField]
		private uint _miss;

		[SerializeField]
		[Range(0f, 1f)]
		private float _criticalPerfectRate;

		[SerializeField]
		[Range(0f, 1f)]
		private float _perfectRate;

		[SerializeField]
		[Range(0f, 1f)]
		private float _greatRate;

		[SerializeField]
		[Range(0f, 1f)]
		private float _goodRate;

		[SerializeField]
		private uint _count;

		public uint Count => _count;

		public uint GetScore(NoteJudge.JudgeBox timing)
		{
			uint result = 0u;
			switch (timing)
			{
			case NoteJudge.JudgeBox.Critical:
				result = _criticalPerfect;
				break;
			case NoteJudge.JudgeBox.Perfect:
				result = _perfect;
				break;
			case NoteJudge.JudgeBox.Great:
				result = _great;
				break;
			case NoteJudge.JudgeBox.Good:
				result = _good;
				break;
			case NoteJudge.JudgeBox.Miss:
				result = _miss;
				break;
			}
			return result;
		}

		public float Get(NoteJudge.ETiming timing)
		{
			float result = 0f;
			switch (NoteJudge.ConvertJudge(timing))
			{
			case NoteJudge.JudgeBox.Critical:
				result = _criticalPerfectRate;
				break;
			case NoteJudge.JudgeBox.Perfect:
				result = _perfectRate;
				break;
			case NoteJudge.JudgeBox.Great:
				result = _greatRate;
				break;
			case NoteJudge.JudgeBox.Good:
				result = _goodRate;
				break;
			}
			return result;
		}
	}
}
