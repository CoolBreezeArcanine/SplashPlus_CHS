using System;
using DB;
using TMPro;
using UnityEngine.Playables;

namespace Process.Timeline
{
	public class TextCommentPlayableBehaviour : PlayableBehaviour
	{
		public int PlayerIndex;

		public TextMeshProUGUI text;

		public string MessageID;

		public override void OnGraphStart(Playable playable)
		{
		}

		public override void OnGraphStop(Playable playable)
		{
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (!string.IsNullOrEmpty(MessageID))
			{
				try
				{
					text.SetText(((CommonMessageID)Enum.Parse(typeof(CommonMessageID), MessageID)).GetName());
				}
				catch
				{
				}
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
		}
	}
}
