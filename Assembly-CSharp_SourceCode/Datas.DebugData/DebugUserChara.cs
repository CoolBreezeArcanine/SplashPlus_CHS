using System;
using UnityEngine;

namespace Datas.DebugData
{
	[Serializable]
	public class DebugUserChara
	{
		public int ID;

		public uint Exp;

		public uint Level;

		public int NextAwake;

		[Range(0f, 1f)]
		public float NextAwakePercent;

		public bool Favorite;

		public uint Awakening;

		public uint Flower;
	}
}
