using Manager;
using TMPro;
using UI.DaisyChainList;
using UnityEngine;

namespace UI.Common
{
	public class TabControllerBase : MonoBehaviour
	{
		protected const string ChangeRight = "ChangeRight";

		protected const string ChangeLeft = "ChangeLeft";

		[SerializeField]
		protected Animator _splitFlap;

		[SerializeField]
		protected TextMeshProUGUI _tabName;

		[SerializeField]
		protected TextMeshProUGUI _tabCount;

		[SerializeField]
		[Header("ボタン")]
		protected CategorytabButtonObject _rightButtonObject;

		[SerializeField]
		protected CategorytabButtonObject _leftButtonObject;

		protected string LeftName;

		protected string CurrentName;

		protected string RightName;

		protected string ElementNum;

		protected float Timer;

		protected bool IsAnimation;

		private readonly Animator _animator;

		public Animator Animator => _animator ?? GetComponent<Animator>();

		public virtual void ViewUpdate()
		{
			if (IsAnimation)
			{
				if (Timer > _splitFlap.GetCurrentAnimatorClipInfo(0)[0].clip.length)
				{
					Timer = 0f;
					IsAnimation = false;
					_tabName.text = CurrentName;
					_tabCount.text = ElementNum;
					_rightButtonObject.SetTabName(RightName, ElementNum);
					_leftButtonObject.SetTabName(LeftName, ElementNum);
				}
				else
				{
					Timer += GameManager.GetGameMSecAdd();
				}
			}
		}

		public virtual void UpdateElement(string elementNum)
		{
			_tabCount.text = (ElementNum = elementNum);
		}

		public virtual void ChangeAll(string leftName, string currentName, string rightName, string elementNum)
		{
			LeftName = leftName;
			RightName = rightName;
			CurrentName = currentName;
			ElementNum = elementNum;
			_tabName.text = CurrentName;
			_tabCount.text = ElementNum;
			_rightButtonObject.SetTabName(RightName, ElementNum);
			_leftButtonObject.SetTabName(LeftName, ElementNum);
		}

		public virtual void Scroll(Direction direction, string left, string current, string right, string elementNum)
		{
			Scroll(direction);
			LeftName = left;
			RightName = right;
			CurrentName = current;
			ElementNum = elementNum;
			IsAnimation = true;
		}

		protected void Scroll(Direction direction)
		{
			_splitFlap.SetTrigger((direction == Direction.Right) ? "ChangeRight" : "ChangeLeft");
		}

		public void Play(string trigger)
		{
			Animator.SetTrigger(trigger);
		}
	}
}
