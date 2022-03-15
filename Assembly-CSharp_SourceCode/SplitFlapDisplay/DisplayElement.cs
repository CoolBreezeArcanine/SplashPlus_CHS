using System;
using UnityEngine;

namespace SplitFlapDisplay
{
	[Serializable]
	public class DisplayElement
	{
		[SerializeField]
		private Sprite _sprite;

		[SerializeField]
		private string _text;

		public Sprite GetSprite => _sprite;

		public string GetText => _text;
	}
}
