using System;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

namespace Process.Timeline
{
	[Serializable]
	public class TextCommentPlayableAsset : PlayableAsset
	{
		public ExposedReference<MonitorBase> Monitor;

		public ExposedReference<TextMeshProUGUI> text;

		public string MessageID;

		public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
		{
			TextCommentPlayableBehaviour textCommentPlayableBehaviour = new TextCommentPlayableBehaviour();
			textCommentPlayableBehaviour.text = text.Resolve(graph.GetResolver());
			textCommentPlayableBehaviour.MessageID = MessageID;
			if (Monitor.Resolve(graph.GetResolver()) != null)
			{
				textCommentPlayableBehaviour.PlayerIndex = Monitor.Resolve(graph.GetResolver()).MonitorIndex;
			}
			return ScriptPlayable<TextCommentPlayableBehaviour>.Create(graph, textCommentPlayableBehaviour);
		}
	}
}
