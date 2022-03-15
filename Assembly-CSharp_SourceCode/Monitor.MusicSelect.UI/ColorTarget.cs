using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.MusicSelect.UI
{
	[Serializable]
	public class ColorTarget
	{
		[SerializeField]
		public List<Graphic> TargetGraphicList;

		[SerializeField]
		public List<Shadow> ShadowList;

		[SerializeField]
		public List<Color> ColorList;
	}
}
