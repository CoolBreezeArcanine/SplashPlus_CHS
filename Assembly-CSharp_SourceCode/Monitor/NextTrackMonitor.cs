using System.Collections;
using IO;
using Manager;
using UnityEngine;

namespace Monitor
{
	public class NextTrackMonitor : MonitorBase
	{
		[SerializeField]
		private GameObject _fadeRoot;

		[SerializeField]
		private GameObject _background;

		[SerializeField]
		private GameObject _addBaseObject;

		[SerializeField]
		[Header("フリーダムモードタイムアップ")]
		private GameObject _originalFreedomModeTimeUp;

		[SerializeField]
		[Header("トラック数表示")]
		private GameObject _originalTrackBase;

		private Animator _addAnimator;

		private TrackBase _tarckbase;

		public GameObject MainFadeRoot => _fadeRoot;

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			_background.SetActive(value: false);
			_addBaseObject.SetActive(value: false);
			_addAnimator = null;
		}

		public void SetFreedomTimeOut()
		{
			if (GameManager.IsFreedomMode && GameManager.IsFreedomTimeUp)
			{
				int childCount = _background.transform.childCount;
				_background.SetActive(value: true);
				if (childCount >= 2)
				{
					_background.transform.GetChild(childCount - 2).gameObject.SetActive(value: false);
					_background.transform.GetChild(childCount - 1).gameObject.SetActive(value: false);
				}
				_background.GetComponent<Animator>().Play("Loop", 0, 1f);
			}
			if (IsActive() && GameManager.IsFreedomMode && GameManager.IsFreedomTimeUp)
			{
				_addAnimator = Object.Instantiate(_originalFreedomModeTimeUp, _addBaseObject.transform).GetComponent<Animator>();
				_addBaseObject.SetActive(value: false);
			}
		}

		public void SetGoToEnd()
		{
			_addAnimator = null;
		}

		public void SetNextTrack(uint nextTrack)
		{
			if (IsActive())
			{
				_tarckbase = Object.Instantiate(_originalTrackBase, _addBaseObject.transform).GetComponent<TrackBase>();
				_addAnimator = _tarckbase.GetComponent<Animator>();
				_tarckbase.SetTrack(nextTrack);
			}
		}

		public override void Release()
		{
			_background.SetActive(value: false);
			if (null != _addAnimator)
			{
				Object.Destroy(_addAnimator.gameObject);
			}
			if (null != _tarckbase)
			{
				Object.Destroy(_tarckbase.gameObject);
			}
			base.Release();
		}

		public void PlayAnimIn()
		{
			StartCoroutine(AddAnimInCoroutine());
		}

		private IEnumerator AddAnimInCoroutine()
		{
			if (!(null == _addAnimator))
			{
				_addBaseObject.SetActive(value: true);
				_addAnimator.gameObject.SetActive(value: true);
				_addAnimator.Play(Animator.StringToHash("In"));
				_addAnimator.Update(0f);
				yield return new WaitForEndOfFrame();
				float length = _addAnimator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length);
				_addAnimator.Play(Animator.StringToHash("Loop"));
			}
		}

		public void PlayAnimOut()
		{
			StartCoroutine(AddAnimOutCoroutine());
		}

		private IEnumerator AddAnimOutCoroutine()
		{
			if (!(null == _addAnimator))
			{
				_addAnimator.Play(Animator.StringToHash("Out"));
				_addAnimator.Update(0f);
				yield return new WaitForEndOfFrame();
				float length = _addAnimator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length);
				_addAnimator.gameObject.SetActive(value: false);
				_addBaseObject.SetActive(value: false);
			}
		}

		public override void ViewUpdate()
		{
		}
	}
}
