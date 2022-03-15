using System;
using System.Collections;
using IO;
using Manager;
using Mecha;
using Monitor.PleaseWait.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class PleaseWaitMonitor : MonitorBase
	{
		[Serializable]
		public class SpriteTimer
		{
			[SerializeField]
			[Header("通常カラー")]
			private Color _normal = Color.white;

			[SerializeField]
			[Header("一分前カラー")]
			private Color _oneMinuteColor = Color.white;

			[SerializeField]
			private TimeSpriteCounter _minute;

			[SerializeField]
			private TimeSpriteCounter _secondes;

			[SerializeField]
			private Image _colonShadow;

			[SerializeField]
			private Image _colonOutline;

			[SerializeField]
			private Image _colon;

			public void SetTimerColor(bool isOneMinute)
			{
				Color color = (isOneMinute ? _oneMinuteColor : _normal);
				_minute.SetColor(color);
				_secondes.SetColor(color);
				_colon.color = color;
			}

			public void SetTime(int minutes, int secondes)
			{
				_minute.SetTime(minutes.ToString());
				_secondes.SetTime(secondes.ToString("00"));
			}

			public void SetVisible(bool isVisible)
			{
				_minute.SetVisible(isVisible);
				_secondes.SetVisible(isVisible);
				_colon.gameObject.SetActive(isVisible);
				_colonShadow.gameObject.SetActive(isVisible);
				_colonOutline.gameObject.SetActive(isVisible);
			}
		}

		[Serializable]
		public class TimeSpriteCounter
		{
			[SerializeField]
			[Header("タイマー部品")]
			private SpriteCounter _shadow;

			[SerializeField]
			[Header("タイマー部品")]
			private SpriteCounter _outline;

			[SerializeField]
			[Header("タイマー部品")]
			private SpriteCounter _timer;

			public void SetColor(Color color)
			{
				_timer.SetColor(color);
			}

			public void SetTime(string time)
			{
				_shadow.ChangeText(time);
				_outline.ChangeText(time);
				_timer.ChangeText(time);
			}

			public void SetVisible(bool isVisible)
			{
				_shadow.gameObject.SetActive(isVisible);
				_outline.gameObject.SetActive(isVisible);
				_timer.gameObject.SetActive(isVisible);
			}
		}

		[SerializeField]
		[Header("フリーダムモード関連")]
		private CanvasGroup _freedomModeObject;

		[SerializeField]
		private CustomTextScroll[] _scrollText;

		[SerializeField]
		[Header("フリーダムモード")]
		private Image[] _rainbow;

		[SerializeField]
		private Animator _freedomModeAnimator;

		[SerializeField]
		private Animator _freedomModeCountDownAnimator;

		[SerializeField]
		[Header("１０分タイマー")]
		private SpriteTimer _time10;

		[SerializeField]
		[Header("１分タイマー")]
		private SpriteTimer _time1;

		[SerializeField]
		[Header("ボタン管理")]
		private PleaseWaitButtonController _buttonController;

		public bool _isDispButton;

		private int _minutes;

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				return;
			}
			CustomTextScroll[] scrollText = _scrollText;
			for (int i = 0; i < scrollText.Length; i++)
			{
				scrollText[i].SetData("");
			}
			_freedomModeObject.gameObject.SetActive(value: false);
			_isDispButton = false;
		}

		public void ReInitialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
				return;
			}
			Main.gameObject.SetActive(value: true);
			Sub.gameObject.SetActive(value: true);
			CustomTextScroll[] scrollText = _scrollText;
			for (int i = 0; i < scrollText.Length; i++)
			{
				scrollText[i].SetData("");
			}
			_freedomModeObject.gameObject.SetActive(value: false);
			_isDispButton = false;
		}

		public override void ViewUpdate()
		{
			CustomTextScroll[] scrollText = _scrollText;
			for (int i = 0; i < scrollText.Length; i++)
			{
				scrollText[i].ViewUpdate();
			}
		}

		public void SetRainbow(int minutes, int seconds)
		{
			if (minutes <= 4)
			{
				for (int i = 0; i < 4 - minutes; i++)
				{
					_rainbow[i].fillAmount = 0f;
				}
				_rainbow[4 - minutes].fillAmount = (float)seconds / 60f;
			}
		}

		public void SetButtonColor(int minutes)
		{
			_minutes = minutes;
			Bd15070_4IF bd15070_4IF = MechaManager.LedIf[monitorIndex];
			Color color = CommonScriptable.GetLedSetting().ButtonSubColor;
			switch (minutes)
			{
			case 3:
				color = CommonScriptable.GetLedSetting().ButtonBasicColor;
				break;
			case 2:
				color = CommonScriptable.GetLedSetting().ButtonCriticalColor;
				break;
			case 1:
				color = CommonScriptable.GetLedSetting().ButtonPerfectColor;
				break;
			case 0:
				color = CommonScriptable.GetLedSetting().ButtonAttentionColor;
				break;
			}
			for (byte b = 0; b < 8; b = (byte)(b + 1))
			{
				if ((b != 3 && b != 4) || !_isDispButton)
				{
					bd15070_4IF.SetColorButton(b, color);
				}
			}
		}

		private void ResetLEDColor()
		{
			SetButtonColor(_minutes);
		}

		public void PressedButton(InputManager.ButtonSetting button)
		{
			_buttonController.SetAnimationActive((int)button);
		}

		public void PlayFreedomModeTimerIntroduction()
		{
			if (isPlayerActive)
			{
				_buttonController.Initialize(base.MonitorIndex);
				_buttonController.SetVisibleImmediate(false, 3, 4);
				_freedomModeObject.gameObject.SetActive(value: true);
				_freedomModeObject.alpha = 1f;
				StartCoroutine(IntroductionCoroutine());
				double num = (double)GameManager.GetFreedomStartTime() * 0.001;
				int seconds = (int)(num % 60.0);
				int num2 = (int)(num / 60.0);
				SetRainbow(num2, seconds);
				SetTime(num2, seconds);
				MechaManager.LedIf[monitorIndex].ButtonLedReset();
				Bd15070_4IF bd15070_4IF = MechaManager.LedIf[monitorIndex];
				Color buttonSubColor = CommonScriptable.GetLedSetting().ButtonSubColor;
				for (byte b = 0; b < 8; b = (byte)(b + 1))
				{
					bd15070_4IF.SetColorButton(b, buttonSubColor);
				}
			}
		}

		public void SetTerminationCheck()
		{
			_buttonController.SetVisible(true, 3, 4);
			_isDispButton = true;
		}

		public void SetCountDown()
		{
			_buttonController.SetVisible(false, 3, 4);
			_isDispButton = false;
			ResetLEDColor();
		}

		private IEnumerator IntroductionCoroutine()
		{
			_freedomModeAnimator.enabled = true;
			_freedomModeAnimator.Play(Animator.StringToHash("In"));
			yield return new WaitForEndOfFrame();
			float length = _freedomModeAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_freedomModeAnimator.enabled = false;
		}

		public void PlayTimeUp()
		{
			_buttonController.SetVisible(false, 3, 4);
			ResetLEDColor();
			_isDispButton = false;
			StartCoroutine(TimeUpCoroutine());
			GameManager.IsFreedomTimeUp = true;
		}

		private IEnumerator TimeUpCoroutine()
		{
			_freedomModeAnimator.enabled = true;
			SetTime(0, 0);
			_freedomModeAnimator.Play(Animator.StringToHash("TimeUp@In"));
			yield return new WaitForEndOfFrame();
			float length = _freedomModeAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_freedomModeAnimator.Play(Animator.StringToHash("TimeUp@Loop"));
		}

		public void PlayFreedomModeTimerOut()
		{
			StartCoroutine(FreedomModeTimerOutCoroutine());
		}

		private IEnumerator FreedomModeTimerOutCoroutine()
		{
			MechaManager.LedIf[monitorIndex].ButtonLedReset();
			_freedomModeAnimator.enabled = true;
			if (_freedomModeAnimator.gameObject.activeSelf && _freedomModeAnimator.gameObject.activeInHierarchy)
			{
				_freedomModeAnimator.Play(Animator.StringToHash("TimeUp@Out"));
				yield return new WaitForEndOfFrame();
				float length = _freedomModeAnimator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length);
			}
			_freedomModeObject.gameObject.SetActive(value: false);
		}

		public void PauseTimer(bool isPause)
		{
			if (_freedomModeObject.gameObject.activeSelf && _freedomModeObject.gameObject.activeInHierarchy)
			{
				StartCoroutine(isPause ? TimerPause() : TimerResume());
			}
		}

		private IEnumerator TimerPause()
		{
			yield return new WaitForSeconds(0.5f);
			_freedomModeAnimator.enabled = true;
			_freedomModeAnimator.Play(Animator.StringToHash("Stop@In"));
			yield return new WaitForEndOfFrame();
			float length = _freedomModeAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_freedomModeAnimator.Play(Animator.StringToHash("Stop@Loop"));
		}

		private IEnumerator TimerResume()
		{
			yield return new WaitForSeconds(0.5f);
			_freedomModeAnimator.Play(Animator.StringToHash("Stop@Out"));
			yield return new WaitForEndOfFrame();
			float length = _freedomModeAnimator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length);
			_freedomModeAnimator.enabled = false;
			double num = (double)GameManager.GetFreedomModeMSec() * 0.001;
			int seconds = (int)(num % 60.0);
			int minutes = (int)(num / 60.0);
			SetRainbow(minutes, seconds);
		}

		public void SetTime(int minute, int seconds)
		{
			if (minute >= 10)
			{
				_time1.SetVisible(isVisible: false);
				_time10.SetVisible(isVisible: true);
				_time10.SetTime(minute, seconds);
			}
			else
			{
				_time10.SetVisible(isVisible: false);
				_time1.SetVisible(isVisible: true);
				_time1.SetTime(minute, seconds);
			}
		}

		public void Play10CountDown()
		{
			_freedomModeCountDownAnimator.Play(Animator.StringToHash("CountDown"), 0, 0f);
		}

		public void SetVisibleRainbow(bool isVisible)
		{
			Image[] rainbow = _rainbow;
			foreach (Image image in rainbow)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, isVisible ? 1 : 0);
			}
		}

		public void SetHalfAlphaRainbo(bool isHalfAlpha)
		{
			Image[] rainbow = _rainbow;
			foreach (Image image in rainbow)
			{
				image.color = new Color(image.color.r, image.color.g, image.color.b, isHalfAlpha ? 0.5f : 1f);
			}
		}

		public void SetTimerColor(Color color)
		{
		}

		public void SetOneMinute(bool isOneMinute)
		{
			_time1.SetTimerColor(isOneMinute);
		}

		public bool IsVisibleFreedomMode()
		{
			return _freedomModeObject.gameObject.activeSelf;
		}

		public void SetMinuteHand(float angle)
		{
		}

		public void SetSeconHand(float angle)
		{
		}
	}
}
