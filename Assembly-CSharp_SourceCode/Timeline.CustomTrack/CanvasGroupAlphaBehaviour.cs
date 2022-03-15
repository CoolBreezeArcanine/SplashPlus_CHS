using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline.CustomTrack
{
	[Serializable]
	public class CanvasGroupAlphaBehaviour : PlayableBehaviour
	{
		[SerializeField]
		[Range(0f, 1f)]
		public float Alpha;
	}
}
