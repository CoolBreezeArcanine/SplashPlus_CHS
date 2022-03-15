using Process;
using UI;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.CharacterSelect
{
	public class CharacterChainObject : ChainObject
	{
		[SerializeField]
		[Header("Advanced")]
		private CanvasGroup _center;

		[SerializeField]
		private CanvasGroup _sub;

		[SerializeField]
		private InstantiateGenerator _selectIgemGenerator;

		[SerializeField]
		private InstantiateGenerator _selectItemSGenerator;

		private CharacterSelectCenterItemObject _centerItemObject;

		private CharacterSelectSubItemObject _subItemObject;

		public void Initialize()
		{
			_centerItemObject = _selectIgemGenerator.Instantiate<CharacterSelectCenterItemObject>();
			_subItemObject = _selectItemSGenerator.Instantiate<CharacterSelectSubItemObject>();
			_centerItemObject.Initialize();
			_subItemObject.Initialize();
		}

		public void SetInformation(bool isParty, string charaName, int distance, WeakPoint weakPoint, CharacterSelectProces.ArrowDirection arrow)
		{
			_centerItemObject.SetInformation(isParty, charaName, distance, weakPoint, arrow);
			_subItemObject.SetInformation(isParty, weakPoint);
		}

		public void SetData(int level, float amount, int awakeNum, Sprite face, CharacterMapColorData data, Color shadowColor)
		{
			_centerItemObject.SetData(level, amount, awakeNum, data.Base, face, data.Frame, data.Level, data.SmallAwakeStar, data.AwakeStarBase);
			_subItemObject.SetData(level, amount, awakeNum, data.SmallBase, face, data.SmallFrame, data.Level, data.SmallAwakeStar, data.AwakeStarBase, shadowColor);
		}

		public override void OnCenterIn()
		{
			_center.alpha = 1f;
			_sub.alpha = 0f;
			_centerItemObject.OnCenterIn();
			base.OnCenterIn();
		}

		public override void OnCenter()
		{
			_center.alpha = 1f;
			_sub.alpha = 0f;
			base.OnCenter();
		}

		public override void OnCenterOut()
		{
			_center.alpha = 0f;
			_sub.alpha = 1f;
			_centerItemObject.OnCenterOut();
			base.OnCenterOut();
		}

		public void SetVisibleCenterDecorate(bool isVisible)
		{
			_centerItemObject.SetVisibleDecorate(isVisible);
		}

		public void SetBlank()
		{
			_centerItemObject.SetVisibleDecorate(isVisible: false);
		}
	}
}
