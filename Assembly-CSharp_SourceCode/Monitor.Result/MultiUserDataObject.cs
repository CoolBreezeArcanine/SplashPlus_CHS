using DB;
using Manager;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Result
{
	public class MultiUserDataObject : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _achievementText;

		[SerializeField]
		private MultipleImage _difficultyImage;

		[SerializeField]
		private TextMeshProUGUI _userNameText;

		[SerializeField]
		private Image _userIcon;

		[SerializeField]
		private Animator _originalMedal;

		private Animator _mainAnimator;

		private PlayComboflagID _comboType;

		private int _rank;

		public Animator MainAnimator => _mainAnimator;

		private void Awake()
		{
			_mainAnimator = GetComponent<Animator>();
		}

		public void SetData(string userName, Sprite userIcon, MusicDifficultyID difficulty, uint achievement, int rank, PlayComboflagID comboType)
		{
			if (_mainAnimator == null)
			{
				_mainAnimator = GetComponent<Animator>();
			}
			_comboType = comboType;
			_userNameText.text = userName;
			_userIcon.sprite = userIcon;
			_achievementText.text = $"{(float)achievement / 10000f:##0.0000}%";
			_difficultyImage.ChangeSprite((int)difficulty);
			_rank = rank;
			for (int i = 0; i < 4; i++)
			{
				if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
				{
					_mainAnimator.SetLayerWeight(i + 1, (_rank == i) ? 1 : 0);
				}
			}
			if (comboType <= PlayComboflagID.None)
			{
				_originalMedal.gameObject.SetActive(value: false);
			}
			else
			{
				LoopComboMedal();
			}
		}

		public void PlayMedal()
		{
			if (_comboType != 0)
			{
				string text = "";
				switch (_comboType)
				{
				case PlayComboflagID.AllPerfectPlus:
					text = "Get_APp";
					break;
				case PlayComboflagID.AllPerfect:
					text = "Get_AP";
					break;
				case PlayComboflagID.Gold:
					text = "Get_FCp";
					break;
				case PlayComboflagID.Silver:
					text = "Get_FC";
					break;
				}
				_originalMedal.Play(Animator.StringToHash(text), 0, 0f);
			}
		}

		public void SetActiveAnimation(bool isUser, bool isDetails, bool isSingle)
		{
			if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				for (int i = 0; i < 4; i++)
				{
					_mainAnimator.SetLayerWeight(i + 1, (_rank == i) ? 1 : 0);
				}
				string text = (isUser ? "Loop_User" : "Loop");
				_mainAnimator.Play(Animator.StringToHash(text));
				if (_comboType > PlayComboflagID.None)
				{
					LoopComboMedal();
				}
			}
		}

		private void LoopComboMedal()
		{
			string text = "";
			switch (_comboType)
			{
			case PlayComboflagID.AllPerfectPlus:
				text = "Get_APp_Loop";
				break;
			case PlayComboflagID.AllPerfect:
				text = "Get_AP_Loop";
				break;
			case PlayComboflagID.Gold:
				text = "Get_FCp_Loop";
				break;
			case PlayComboflagID.Silver:
				text = "Get_FC_Loop";
				break;
			}
			_originalMedal.Play(Animator.StringToHash(text), 0, 0f);
		}

		public void Skip()
		{
		}
	}
}
