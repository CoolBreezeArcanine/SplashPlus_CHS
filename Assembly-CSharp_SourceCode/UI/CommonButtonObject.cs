using System.Collections;
using Fx;
using IO;
using Mai2.Mai2Cue;
using Manager;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class CommonButtonObject : MonoBehaviour
	{
		public enum LedColors
		{
			None,
			White,
			Red,
			Blue,
			Purple,
			Green,
			Orange,
			Black,
			Basic,
			Advanced,
			Expert,
			Master,
			Remaster,
			Max
		}

		public const string PressedTrigger = "Pressed";

		public const string PressedUpTrigger = "PressedUp";

		public const string Hold = "Hold";

		public const string Idle = "Idle";

		public const string In = "In";

		public const string InReverse = "InReverse";

		public const string Out = "Out";

		public const string Loop = "Loop";

		public const string Disabled = "Disabled";

		public const string Activated = "Activated";

		public const string NonActive = "NonActive";

		public const string PrefubName = "CMN_DecideButton/FX_CMN_DecideButton_v110";

		[SerializeField]
		protected Image _symbolImage;

		[SerializeField]
		[Header("エフェクト発生オブジェクト")]
		private GameObject _effectTarget;

		protected Transform EffectTransform;

		protected Animator ButtonAnimator;

		protected InputManager.ButtonSetting ButtonIndex;

		protected LedColors _ledColor;

		private Cue _cueIndex = (Cue)(-1);

		protected int MonitorIndex;

		protected bool IsButtonVisible;

		protected bool Initialized;

		private GameObject _particleRootObject;

		protected FX_Controler ParticleControler;

		protected RectTransform RectTransform;

		private Coroutine _disableCoroutine;

		private void Awake()
		{
			ButtonAnimator = GetComponent<Animator>();
			RectTransform = GetComponent<RectTransform>();
			IsButtonVisible = true;
			if (ButtonAnimator != null)
			{
				ButtonAnimator.Rebind();
			}
			if (!(base.transform.parent == null))
			{
				Transform transform = base.transform.parent.Find("CMN_DecideButton/FX_CMN_DecideButton_v110");
				if (transform == null)
				{
					_particleRootObject = Object.Instantiate(Resources.Load<GameObject>("CMN_DecideButton/FX_CMN_DecideButton_v110"), base.transform.parent);
					_particleRootObject.name = _particleRootObject.name.Replace("(Clone)", "");
				}
				else
				{
					_particleRootObject = transform.gameObject;
				}
				if (_effectTarget != null)
				{
					EffectTransform = _effectTarget.transform;
				}
				else
				{
					EffectTransform = base.transform;
				}
				ParticleControler = _particleRootObject.GetComponent<FX_Controler>();
				ParticleControler?.Stop();
			}
		}

		public void Initialize(int monitorIndex, InputManager.ButtonSetting buttonIndex, LedColors ledColor)
		{
			MonitorIndex = monitorIndex;
			ButtonIndex = buttonIndex;
			_ledColor = ledColor;
			Initialized = true;
		}

		public void ChangeColor(LedColors ledColor, bool nowFlash = false)
		{
			_ledColor = ledColor;
			if (nowFlash && _ledColor != 0 && base.gameObject.activeInHierarchy && IsButtonVisible)
			{
				MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, LedColors32(_ledColor));
			}
		}

		public virtual void SetSymbol(Sprite synbolSprite, bool isFlip)
		{
			SetSymbolSprite(synbolSprite, isFlip);
			IsButtonVisible = true;
			if (base.gameObject.activeInHierarchy && IsButtonVisible)
			{
				SetTrigger("In");
			}
		}

		public virtual void SetSymbol(ButtonControllerBase.ButtonInformation buttonInfo, bool isFlip)
		{
			SetSymbolSprite(buttonInfo.Image, isFlip);
			ChangeColor(buttonInfo.LedColor);
			SetSE(buttonInfo.Cue);
			IsButtonVisible = true;
			if (base.gameObject.activeInHierarchy && IsButtonVisible)
			{
				SetTrigger("In");
			}
		}

		public void SetSymbolStartDisable(Sprite synbolSprite, bool isFlip)
		{
			SetSymbolSprite(synbolSprite, isFlip);
			if (base.gameObject.activeInHierarchy)
			{
				SetActiveImmediateButton(isActive: false);
			}
		}

		public virtual void SetSymbolSprite(Sprite synbolSprite, bool isFlip)
		{
			_symbolImage.sprite = synbolSprite;
			_symbolImage.SetNativeSize();
			_symbolImage.transform.localEulerAngles = new Vector3(0f, isFlip ? 180 : 0, 0f);
		}

		public void SetSE(int cueIndex)
		{
			_cueIndex = (Cue)cueIndex;
		}

		public void SetSE(Cue cueIndex)
		{
			_cueIndex = cueIndex;
		}

		public void ViewUpdate(float syncTime)
		{
			if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				ButtonAnimator.SetFloat("SyncTimer", syncTime);
			}
		}

		public virtual void Pressed()
		{
			if (base.gameObject.activeInHierarchy)
			{
				SetTrigger("Pressed");
				if (_cueIndex != (Cue)(-1))
				{
					SoundManager.PlaySE(_cueIndex, MonitorIndex);
				}
				ParticleControler?.SetTransform(EffectTransform.position);
				ParticleControler?.Play();
			}
		}

		public virtual void PressedFlip(bool isFlip)
		{
			if (base.gameObject.activeInHierarchy)
			{
				if (!isFlip)
				{
					SetTrigger("Pressed");
				}
				else
				{
					SetTrigger("PressedUp");
				}
				if (_cueIndex != (Cue)(-1))
				{
					SoundManager.PlaySE(_cueIndex, MonitorIndex);
				}
				ParticleControler?.SetTransform(EffectTransform.position);
				ParticleControler?.Play();
			}
		}

		public void PressedOut()
		{
			StartCoroutine(PressedOutCoroutine());
		}

		private IEnumerator PressedOutCoroutine()
		{
			Pressed();
			yield return new WaitForSeconds(0.5f);
			IsButtonVisible = false;
			SetTrigger("Out");
			if (_disableCoroutine == null)
			{
				_disableCoroutine = StartCoroutine(DisableCoroutine());
			}
		}

		public virtual void SetActiveButton(bool isActive)
		{
			if (!isActive)
			{
				MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, LedColors32(LedColors.Black));
			}
			if (ButtonAnimator == null || !ButtonAnimator.gameObject.activeSelf || !ButtonAnimator.gameObject.activeInHierarchy)
			{
				return;
			}
			if (isActive)
			{
				AnimatorStateInfo currentAnimatorStateInfo = ButtonAnimator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName("Disabled") || currentAnimatorStateInfo.IsName("Out") || currentAnimatorStateInfo.IsName("Idle"))
				{
					IsButtonVisible = true;
					if (_disableCoroutine != null)
					{
						StopCoroutine(_disableCoroutine);
						_disableCoroutine = null;
					}
					if (base.gameObject.activeInHierarchy)
					{
						SetTrigger("In");
					}
				}
			}
			else if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy && !ButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Disabled") && !ButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Out"))
			{
				IsButtonVisible = false;
				SetTrigger("Out");
				if (_disableCoroutine == null)
				{
					_disableCoroutine = StartCoroutine(DisableCoroutine());
				}
			}
		}

		public virtual void SetActiveButtonFlip(bool isActive, bool isFlip = false)
		{
			if (!isActive)
			{
				MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, LedColors32(LedColors.Black));
			}
			if (ButtonAnimator == null || !ButtonAnimator.gameObject.activeSelf || !ButtonAnimator.gameObject.activeInHierarchy)
			{
				return;
			}
			if (isActive)
			{
				AnimatorStateInfo currentAnimatorStateInfo = ButtonAnimator.GetCurrentAnimatorStateInfo(0);
				if (!currentAnimatorStateInfo.IsName("Disabled") && !currentAnimatorStateInfo.IsName("Out") && !currentAnimatorStateInfo.IsName("Idle"))
				{
					return;
				}
				IsButtonVisible = true;
				if (_disableCoroutine != null)
				{
					StopCoroutine(_disableCoroutine);
					_disableCoroutine = null;
				}
				if (base.gameObject.activeInHierarchy)
				{
					if (!isFlip)
					{
						SetTrigger("In");
					}
					else
					{
						SetTrigger("InReverse");
					}
				}
			}
			else if (base.gameObject.activeSelf && base.gameObject.activeInHierarchy && !ButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Disabled") && !ButtonAnimator.GetCurrentAnimatorStateInfo(0).IsName("Out"))
			{
				IsButtonVisible = false;
				SetTrigger("Out");
				if (_disableCoroutine == null)
				{
					_disableCoroutine = StartCoroutine(DisableCoroutine());
				}
			}
		}

		public void SetNonActive(bool isShow)
		{
			SetTrigger(isShow ? "Activated" : "NonActive");
		}

		public void SetActiveImmediateButton(bool isActive)
		{
			SetTrigger(isActive ? "In" : "Disabled");
		}

		public void SetLoop()
		{
			SetTrigger("Loop");
		}

		public void SetHold()
		{
			SetTrigger("Hold");
		}

		private IEnumerator DisableCoroutine()
		{
			yield return new WaitForSeconds(0.5f);
			_symbolImage?.SetNativeSize();
			_disableCoroutine = null;
		}

		public bool IsVisible()
		{
			return IsButtonVisible;
		}

		public static Color32 LedColors32(LedColors cols)
		{
			return cols switch
			{
				LedColors.White => CommonScriptable.GetLedSetting().ButtonReactionColor, 
				LedColors.Red => CommonScriptable.GetLedSetting().ButtonYesColor, 
				LedColors.Blue => CommonScriptable.GetLedSetting().ButtonNoColor, 
				LedColors.Purple => CommonScriptable.GetLedSetting().ButtonDetailSettingColor, 
				LedColors.Green => CommonScriptable.GetLedSetting().ButtonTimeSkipColor, 
				LedColors.Orange => CommonScriptable.GetLedSetting().ButtonUploadColor, 
				LedColors.Black => new Color32(0, 0, 0, byte.MaxValue), 
				LedColors.Basic => CommonScriptable.GetLedSetting().ButtonBasicColor, 
				LedColors.Advanced => CommonScriptable.GetLedSetting().ButtonAdvancesColor, 
				LedColors.Expert => CommonScriptable.GetLedSetting().ButtonExpertColor, 
				LedColors.Master => CommonScriptable.GetLedSetting().ButtonMasterColor, 
				LedColors.Remaster => CommonScriptable.GetLedSetting().ButtonReMasterColor, 
				_ => new Color32(0, 0, 0, byte.MaxValue), 
			};
		}

		protected virtual void SetTrigger(string trigger)
		{
			ButtonAnimator.SetTrigger(trigger);
			if (_ledColor != 0)
			{
				_ = Initialized;
				switch (trigger)
				{
				case "In":
				case "InReverse":
				case "Activated":
				case "Loop":
					MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, LedColors32(_ledColor));
					break;
				case "Out":
				case "Disabled":
				case "NonActive":
					MechaManager.LedIf[MonitorIndex].SetColorButton((byte)ButtonIndex, LedColors32(LedColors.Black));
					break;
				case "Pressed":
				case "PressedUp":
				case "Hold":
					MechaManager.LedIf[MonitorIndex].SetColorButtonPressed((byte)ButtonIndex, LedColors32(LedColors.White));
					break;
				}
			}
		}
	}
}
