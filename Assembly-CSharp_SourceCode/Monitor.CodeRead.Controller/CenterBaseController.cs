using System.Collections;
using Process.CodeRead;
using UI;
using UnityEngine;

namespace Monitor.CodeRead.Controller
{
	public class CenterBaseController : CodeReadControllerBase
	{
		private const string DerakkumaPath = "Derakkuma/Body/Hand_Item/Up/Hand_";

		private const string LeftHand = "Derakkuma/Body/Hand_Item/Up/Hand_L/Null_Hand_Item";

		private const string RightHand = "Derakkuma/Body/Hand_Item/Up/Hand_R/Null_Hand_Item";

		private const string DerakkumaCardSwingKeyName = "Card_Swing";

		protected readonly int Insert1P = Animator.StringToHash("1P_Read_Image_Insert");

		protected readonly int Insert2P = Animator.StringToHash("2P_Read_Image_Insert");

		protected readonly int Problem = Animator.StringToHash("1P2P_Read_Image_Problem");

		[SerializeField]
		private GameObject _readImageSetObject;

		[SerializeField]
		private GameObject _centerBase2;

		[SerializeField]
		private RectTransform _derakkuma;

		[SerializeField]
		private GameObject _originalDerakkuma;

		[SerializeField]
		private GameObject _originalDerakkumaItemCard;

		private Transform _leftHand;

		private Transform _rightHand;

		private Animator _derakkumaAnimator;

		private int _playerIndex = -1;

		private bool _isSinglePlay;

		private CodeReadProcess.CodeReaderState _readerState;

		public override void Initialize(int playerIndex)
		{
			base.Initialize(playerIndex);
			GameObject gameObject = Object.Instantiate(_originalDerakkuma, _derakkuma);
			_leftHand = gameObject.transform.Find("Derakkuma/Body/Hand_Item/Up/Hand_L/Null_Hand_Item");
			_rightHand = gameObject.transform.Find("Derakkuma/Body/Hand_Item/Up/Hand_R/Null_Hand_Item");
			Object.Instantiate(_originalDerakkumaItemCard, _leftHand);
			Object.Instantiate(_originalDerakkumaItemCard, _rightHand).GetComponentInChildren<MultipleImage>().ChangeSprite(1);
			_derakkumaAnimator = gameObject.GetComponent<Animator>();
		}

		public void SetData(int playerIndex, bool isSinglePlay)
		{
			_playerIndex = playerIndex;
			_isSinglePlay = isSinglePlay;
			_readImageSetObject.SetActive(isSinglePlay);
		}

		public override void Play(AnimationType type)
		{
			if (IsAnimationActive())
			{
				switch (type)
				{
				case AnimationType.In:
					StartCoroutine(PlayCoroutine());
					break;
				case AnimationType.Out:
					MainAnimator.Play(Out);
					break;
				}
			}
		}

		public void PlayCenterBase(CodeReadProcess.CodeReaderState state)
		{
			_readerState = state;
			if (IsAnimationActive())
			{
				switch (state)
				{
				case CodeReadProcess.CodeReaderState.Insert:
					MainAnimator.Play((_playerIndex == 0) ? Insert1P : Insert2P);
					break;
				case CodeReadProcess.CodeReaderState.Error:
					MainAnimator.Play(Problem);
					break;
				case CodeReadProcess.CodeReaderState.Normal:
					break;
				}
			}
		}

		private IEnumerator PlayCoroutine()
		{
			MainAnimator.Play(In);
			_derakkumaAnimator.StopPlayback();
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(MainAnimator.GetCurrentAnimatorStateInfo(0).length);
			if (_isSinglePlay)
			{
				MainAnimator.Play((_playerIndex == 0) ? Animator.StringToHash("1P_Read_Image_Large") : Animator.StringToHash("2P_Read_Image_Large"));
			}
			else
			{
				_derakkumaAnimator.SetLayerWeight(_derakkumaAnimator.GetLayerIndex("Result"), 0f);
				_derakkumaAnimator.Play(Animator.StringToHash("Card_Swing"));
			}
			PlayCenterBase(_readerState);
		}
	}
}
