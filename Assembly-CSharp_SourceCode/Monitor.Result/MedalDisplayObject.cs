using System.Collections;
using DB;
using UI;
using UnityEngine;

namespace Monitor.Result
{
	[RequireComponent(typeof(CanvasGroup))]
	public class MedalDisplayObject : MonoBehaviour
	{
		public enum MedalTarget
		{
			Combo,
			Sync
		}

		[SerializeField]
		[Header("コンボ")]
		private Animator _comboAnimator;

		[SerializeField]
		private MultipleImage _comboTextImage;

		[SerializeField]
		private MultipleImage _comboTextEffImage;

		[SerializeField]
		[Header("シンク")]
		private Animator _syncAnimator;

		[SerializeField]
		private MultipleImage _syncTextImage;

		[SerializeField]
		private MultipleImage _syncTextEffImage;

		private MedalTarget _medalTarget;

		[SerializeField]
		private CanvasGroup _comboGroup;

		[SerializeField]
		private CanvasGroup _syncGroup;

		[SerializeField]
		private CanvasGroup _medalGroup;

		[SerializeField]
		[Header("詳細")]
		private MultipleImage _datailsSyncMedalImage;

		[SerializeField]
		private MultipleImage _datailsSyncMedalEffectImage;

		private Animator _animator;

		private Coroutine _coroutine;

		private string _loopKey = "";

		private PlayComboflagID _currentComboType;

		private PlaySyncflagID _currentSyncType;

		public Animator SubMAnimator => _syncAnimator;

		public PlayComboflagID ComboMedal => _currentComboType;

		public PlaySyncflagID SyncMedal => _currentSyncType;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void SetAlpha(MedalTarget target, float alpha)
		{
			switch (target)
			{
			case MedalTarget.Combo:
				_comboGroup.alpha = alpha;
				break;
			case MedalTarget.Sync:
				_syncGroup.alpha = alpha;
				break;
			}
		}

		public void SetData(MedalTarget useTarget, uint toValue, PlayComboflagID comboType, PlaySyncflagID syncType)
		{
			_currentComboType = comboType;
			_currentSyncType = syncType;
			_medalTarget = useTarget;
			switch (_medalTarget)
			{
			case MedalTarget.Combo:
			{
				int num2 = (int)(_currentComboType - 1);
				if (num2 >= 0)
				{
					_comboTextImage.ChangeSprite(num2);
					_comboTextEffImage.ChangeSprite(num2);
				}
				break;
			}
			case MedalTarget.Sync:
			{
				int num = (int)(_currentSyncType - 1);
				if (num >= 0)
				{
					_syncTextImage.ChangeSprite(num);
					_syncTextEffImage.ChangeSprite(num);
				}
				break;
			}
			}
			if (_currentSyncType == PlaySyncflagID.None && _currentComboType == PlayComboflagID.None)
			{
				_medalGroup.gameObject.SetActive(value: false);
			}
			else if (_currentSyncType > PlaySyncflagID.None)
			{
				_datailsSyncMedalImage.ChangeSprite((int)(_currentSyncType - 1));
				_datailsSyncMedalEffectImage.ChangeSprite((int)(_currentSyncType - 1));
			}
			else
			{
				_datailsSyncMedalImage.gameObject.SetActive(value: false);
				_datailsSyncMedalEffectImage.gameObject.SetActive(value: false);
			}
		}

		public void SetMedalAnimationOnly(bool isUseTimeline)
		{
			if (_animator == null)
			{
				_animator = GetComponent<Animator>();
			}
			_comboGroup.gameObject.SetActive(value: false);
			_syncGroup.gameObject.SetActive(value: false);
			if (!isUseTimeline)
			{
				_animator.enabled = false;
				_medalGroup.gameObject.SetActive(value: false);
			}
		}

		public string GetMedalSeCueCode()
		{
			string result = string.Empty;
			if (_currentSyncType > PlaySyncflagID.None)
			{
				switch (_currentSyncType)
				{
				case PlaySyncflagID.SyncLow:
				case PlaySyncflagID.SyncHi:
					result = "SE_RESULT_ICON_FULL_SYNC_DX";
					break;
				case PlaySyncflagID.ChainLow:
				case PlaySyncflagID.ChainHi:
					result = "SE_RESULT_ICON_FULL_SYNC";
					break;
				}
			}
			else
			{
				switch (_currentComboType)
				{
				case PlayComboflagID.AllPerfect:
				case PlayComboflagID.AllPerfectPlus:
					result = "SE_RESULT_ICON_AP";
					break;
				case PlayComboflagID.Silver:
				case PlayComboflagID.Gold:
					result = "SE_RESULT_ICON_FC";
					break;
				}
			}
			return result;
		}

		public bool IsMedalAnimation()
		{
			if (_currentComboType <= PlayComboflagID.None)
			{
				return _currentSyncType > PlaySyncflagID.None;
			}
			return true;
		}

		public bool IsFullCombo()
		{
			return _currentComboType == PlayComboflagID.Silver;
		}

		public void PlayMedalAnimation(bool isPlayInAnimation)
		{
			string value = "";
			string text = "";
			switch (_medalTarget)
			{
			case MedalTarget.Combo:
				switch (_currentComboType)
				{
				case PlayComboflagID.Silver:
					value = "Get_FC";
					text = "Get_FC_Loop";
					break;
				case PlayComboflagID.Gold:
					value = "Get_FCp";
					text = "Get_FCp_Loop";
					break;
				case PlayComboflagID.AllPerfect:
					value = "Get_AP";
					text = "Get_AP_Loop";
					break;
				case PlayComboflagID.AllPerfectPlus:
					value = "Get_APp";
					text = "Get_APp_Loop";
					break;
				}
				break;
			case MedalTarget.Sync:
				switch (_currentSyncType)
				{
				case PlaySyncflagID.ChainLow:
					value = "Get_FS";
					text = "Get_FS_Loop";
					break;
				case PlaySyncflagID.ChainHi:
					value = "Get_FSp";
					text = "Get_FSp_Loop";
					break;
				case PlaySyncflagID.SyncLow:
					value = "Get_FSD";
					text = "Get_FSD_Loop";
					break;
				case PlaySyncflagID.SyncHi:
					value = "Get_FSDp";
					text = "Get_FSDp_Loop";
					break;
				}
				break;
			}
			_loopKey = text;
			if (!string.IsNullOrEmpty(value) && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				if (!_animator.enabled)
				{
					_animator.enabled = true;
				}
				if (!_medalGroup.gameObject.activeSelf)
				{
					_medalGroup.gameObject.SetActive(value: true);
				}
				_animator.Play(Animator.StringToHash(value), 0, 0f);
				if (_coroutine == null)
				{
					_coroutine = StartCoroutine(MedalInToLoopCoroutine(text, isPlayInAnimation));
				}
			}
		}

		private IEnumerator MedalInToLoopCoroutine(string key, bool isPlayInAnimation)
		{
			if (isPlayInAnimation)
			{
				yield return new WaitForEndOfFrame();
				float length = _animator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length);
			}
			if (!string.IsNullOrEmpty(key))
			{
				_animator.Play(Animator.StringToHash(key), 0, 0f);
			}
			_coroutine = null;
		}

		public void Skip()
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			if (string.IsNullOrEmpty(_loopKey))
			{
				PlayMedalAnimation(isPlayInAnimation: true);
			}
			_animator.Play(Animator.StringToHash(_loopKey), 0, 0f);
		}

		private string CurrentAnimationName()
		{
			string result = "";
			AnimationClip[] animationClips = _animator.runtimeAnimatorController.animationClips;
			foreach (AnimationClip animationClip in animationClips)
			{
				if (_animator.GetCurrentAnimatorStateInfo(0).IsName(animationClip.name))
				{
					result = animationClip.name;
				}
			}
			return result;
		}
	}
}
