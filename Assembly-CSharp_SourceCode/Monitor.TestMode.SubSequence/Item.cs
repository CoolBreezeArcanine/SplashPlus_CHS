using UnityEngine;

namespace Monitor.TestMode.SubSequence
{
	public class Item : ItemText
	{
		public enum State
		{
			Selectable,
			UnselectableTemp,
			InvisibleTemp,
			Unselectable
		}

		public enum TextColor
		{
			White,
			Gray,
			Hide,
			Red,
			Max
		}

		public ItemDefine Define;

		protected readonly Color32[] TextColorTbl = new Color32[4]
		{
			new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
			new Color32(128, 128, 128, byte.MaxValue),
			new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0),
			new Color32(byte.MaxValue, 0, 0, byte.MaxValue)
		};

		public State ItemState { get; private set; }

		public bool IsSelectable => ItemState == State.Selectable;

		public void SetState(State state)
		{
			ItemState = state;
			switch (ItemState)
			{
			case State.Selectable:
			case State.Unselectable:
				if (LabelText != null)
				{
					LabelText.color = TextColorTbl[0];
				}
				if (base.ValueText != null)
				{
					base.ValueText.color = TextColorTbl[0];
				}
				break;
			case State.UnselectableTemp:
				if (LabelText != null)
				{
					LabelText.color = TextColorTbl[1];
				}
				if (base.ValueText != null)
				{
					base.ValueText.color = TextColorTbl[1];
				}
				break;
			case State.InvisibleTemp:
				if (LabelText != null)
				{
					LabelText.color = TextColorTbl[2];
				}
				if (base.ValueText != null)
				{
					base.ValueText.color = TextColorTbl[2];
				}
				break;
			}
		}
	}
}
