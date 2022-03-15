using Manager;
using UnityEngine;

namespace Monitor
{
	public class TouchNoteC : TouchNoteB
	{
		protected TapCEffect TapCEffectObject;

		protected override bool IsJudgeNote()
		{
			return true;
		}

		protected override float GetBaseZPosition()
		{
			return -8f + (float)NoteIndex * 0.0001f;
		}

		public override void Initialize(NoteData note)
		{
			base.Initialize(note);
			StartPos = 0f;
			EndPos = 0f;
			NoteObj.transform.localPosition = new Vector3(0f, StartPos, GetBaseZPosition());
		}

		private void Start()
		{
			NoteKind = NoteTypeID.Def.TouchTap;
		}

		public void SetEffectObject(TapCEffect effectObj)
		{
			TapCEffectObject = effectObj;
		}

		protected override void PlayJudgeSe()
		{
			if (!ShotJudgeSound)
			{
				if (AddEffect != 0)
				{
					if (NoteJudge.ConvertJudge(JudgeResult) != 0)
					{
						TapCEffectObject.Intialize();
					}
					ReserveCenterEffectJudgeSe(NoteJudge.ConvertJudge(JudgeResult));
				}
				else
				{
					ReserveTapJudgeSe(NoteJudge.ConvertJudge(JudgeResult));
				}
			}
			ShotJudgeSound = true;
		}
	}
}
