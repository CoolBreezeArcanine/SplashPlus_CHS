using System;
using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	[Serializable]
	public class ItemDefine
	{
		public int LineNumber;

		public string Label;

		public bool IsSelectable;

		public bool HasValueField;

		public int NumValueField;

		public GameObject NextPagePrefab;

		public bool IsFinishOnSelect;

		public bool IsDefaultSelection;
	}
}
