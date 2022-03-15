using Process;
using UI;
using UnityEngine;

namespace Monitor.CharacterSelect
{
	public class NewcomerComparisonObject : MonoBehaviour
	{
		[SerializeField]
		private Animator _changeArrowAnimator;

		[SerializeField]
		private Animator _newcomerAnimator;

		[SerializeField]
		private InstantiateGenerator _selectCharaGenerator;

		[SerializeField]
		private InstantiateGenerator _newcomerCharaGenerator;

		[SerializeField]
		private InstantiateGenerator _selectStateGenerator;

		[SerializeField]
		private InstantiateGenerator _newcomerStateGenerator;

		private CharaParts _selectParts;

		private CharaParts _newcomerParts;

		private CharacterStateObject _selectState;

		private CharacterStateObject _newcomerState;

		public void Initialize()
		{
			_selectParts = _selectCharaGenerator.Instantiate<CharaParts>();
			_newcomerParts = _newcomerCharaGenerator.Instantiate<CharaParts>();
			_selectState = _selectStateGenerator.Instantiate<CharacterStateObject>();
			_newcomerState = _newcomerStateGenerator.Instantiate<CharacterStateObject>();
			_selectState.Initialize();
			_newcomerState.Initialize();
			_changeArrowAnimator.Play(Animator.StringToHash("Loop_Color_Y"));
			_newcomerAnimator.Play(Animator.StringToHash("Loop"));
		}

		public void SetSelectParts(int level, float amount, int awakeNum, Sprite backBase, Sprite face, Sprite levelBase, Color shadowColor)
		{
			_selectParts.SetParts(backBase, face, null, null, null, null, amount, level, awakeNum, shadowColor);
			_selectParts.SetVisibleDecorate(isVisible: false);
		}

		public void SetSelectData(string charaName, int distance, WeakPoint weak)
		{
			_selectState.SetData(charaName, distance, weak, CharacterSelectProces.ArrowDirection.Stay);
		}

		public void SetSelectBlank()
		{
			_selectParts.SetBlank();
			_selectState.SetBlank();
		}

		public void SetNewcomerParts(int level, float amount, int awakeNum, Sprite backBase, Sprite face, Sprite levelBase)
		{
			_newcomerParts.SetParts(backBase, face, null, null, null, null, amount, level, awakeNum, Color.clear);
			_newcomerParts.SetVisibleDecorate(isVisible: false);
		}

		public void SetNewcomerData(string charaName, int distance, WeakPoint weak, CharacterSelectProces.ArrowDirection arrow)
		{
			_newcomerState.SetData(charaName, distance, weak, arrow);
		}

		public void SetNewcomerComparisonArrow(CharacterSelectProces.ArrowDirection arrow)
		{
			_newcomerState.SetArrowDirection(arrow);
		}
	}
}
