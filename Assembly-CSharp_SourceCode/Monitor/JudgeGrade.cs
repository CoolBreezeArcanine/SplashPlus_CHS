using System.Collections.Generic;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using Manager;
using Process;
using UnityEngine;

namespace Monitor
{
	public class JudgeGrade : MonoBehaviour
	{
		[SerializeField]
		[Header("判定位置")]
		protected float[] _dispPosition = new float[6];

		private TextMesh _msec;

		private Animator _animator;

		protected SpriteRenderer SpriteRender;

		[SerializeField]
		protected SpriteRenderer SpriteRenderFastLate;

		[SerializeField]
		protected SpriteRenderer SpriteRenderAdd;

		protected int _buttonIndex;

		protected int _monitorIndex;

		private List<Jvs.LedPwmFadeParam> _fadeGoodList = new List<Jvs.LedPwmFadeParam>();

		private List<Jvs.LedPwmFadeParam> _fadeGreatList = new List<Jvs.LedPwmFadeParam>();

		private List<Jvs.LedPwmFadeParam> _fadePerfectList = new List<Jvs.LedPwmFadeParam>();

		protected OptionDispjudgeID _dispJudge;

		protected int _dispPos;

		protected virtual float GetBaseZPosition()
		{
			return -9f;
		}

		protected virtual void Awake()
		{
			SpriteRender = base.gameObject.GetComponent<SpriteRenderer>();
			_animator = base.gameObject.GetComponent<Animator>();
			base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, GetBaseZPosition());
			_msec = null;
			_buttonIndex = -1;
			_monitorIndex = -1;
			if (Singleton<SystemConfig>.Instance.config.IsDebugDisp)
			{
				GameObject gameObject = new GameObject();
				gameObject.gameObject.AddComponent<TextMesh>();
				_msec = gameObject.GetComponent<TextMesh>();
				_msec.transform.SetParent(base.gameObject.transform);
				_msec.transform.localPosition = new Vector3(0f, -60f, 0f);
				_msec.transform.localScale = new Vector3(30f, 30f, 0f);
				_msec.transform.localRotation = Quaternion.identity;
				_msec.alignment = TextAlignment.Center;
				_msec.anchor = TextAnchor.MiddleCenter;
			}
			base.gameObject.SetActive(value: false);
		}

		public void SetOption(OptionDispjudgeID dispJudge, int dispPos)
		{
			_dispJudge = dispJudge;
			_dispPos = dispPos;
		}

		public void Execute()
		{
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				base.gameObject.SetActive(value: false);
			}
		}

		public virtual void SetLedSetting(int buttonIndex, int monitorIndex)
		{
			_buttonIndex = buttonIndex;
			_monitorIndex = monitorIndex;
			_fadeGoodList.Clear();
			_fadeGoodList.Add(new Jvs.LedPwmFadeParam
			{
				StartFadeColor = Color.green,
				EndFadeColor = Color.white,
				FadeTime = 100L,
				NextIndex = -1
			});
			_fadeGreatList.Clear();
			_fadeGreatList.Add(new Jvs.LedPwmFadeParam
			{
				StartFadeColor = Color.red,
				EndFadeColor = Color.white,
				FadeTime = 100L,
				NextIndex = -1
			});
			_fadePerfectList.Clear();
			_fadePerfectList.Add(new Jvs.LedPwmFadeParam
			{
				StartFadeColor = Color.yellow,
				EndFadeColor = Color.white,
				FadeTime = 100L,
				NextIndex = -1
			});
		}

		protected virtual bool IsDispPosition()
		{
			return _dispPos != 0;
		}

		public void PreLoad()
		{
			base.gameObject.transform.localPosition = new Vector3(0f, _dispPosition[0], 0f);
			base.gameObject.SetActive(value: true);
			SpriteRender.sprite = GameNoteImageContainer.JudgeCritical;
			if (null != SpriteRenderFastLate)
			{
				SpriteRenderFastLate.gameObject.SetActive(value: true);
				SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeFast;
			}
			if (null != SpriteRenderAdd)
			{
				SpriteRenderAdd.sprite = GameNoteImageContainer.JudgeCriticalBreak;
				SpriteRenderAdd.gameObject.SetActive(value: true);
			}
			_animator.Play("JudgeGrade@Action", -1, 0f);
		}

		public void PreLoadStop()
		{
			base.gameObject.SetActive(value: false);
			if (null != SpriteRenderFastLate)
			{
				SpriteRenderFastLate.gameObject.SetActive(value: false);
			}
			if (null != SpriteRenderAdd)
			{
				SpriteRenderAdd.gameObject.SetActive(value: false);
			}
		}

		public void Initialize(NoteJudge.ETiming judge, float msec, NoteJudge.EJudgeType type)
		{
			if (!IsDispPosition())
			{
				return;
			}
			base.gameObject.transform.localPosition = new Vector3(0f, _dispPosition[_dispPos], 0f);
			base.gameObject.SetActive(value: true);
			if (null != SpriteRenderFastLate)
			{
				SpriteRenderFastLate.gameObject.SetActive(value: false);
			}
			if (null != SpriteRenderAdd)
			{
				SpriteRenderAdd.gameObject.SetActive(value: false);
			}
			_animator.Play("JudgeGrade@Action", -1, 0f);
			_animator.Update(0f);
			if (null != _msec)
			{
				_msec.text = $"{msec / 16.666666f:0.##}";
				if ((double)(msec / 16.666666f) < -3.0)
				{
					_msec.color = Color.blue;
				}
				else if ((double)(msec / 16.666666f) > 3.0)
				{
					_msec.color = Color.red;
				}
				else
				{
					_msec.color = Color.gray;
				}
			}
			DispLed(judge);
			switch (judge)
			{
			case NoteJudge.ETiming.TooFast:
			case NoteJudge.ETiming.TooLate:
				SpriteRender.sprite = GameNoteImageContainer.JudgeMiss;
				break;
			case NoteJudge.ETiming.FastGood:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type2A:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGood;
					break;
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1E:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2E:
				case OptionDispjudgeID.Type3B:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGood;
					if (null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeFast;
					}
					break;
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeFastGood;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeFast;
					break;
				}
				break;
			case NoteJudge.ETiming.LateGood:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type2A:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGood;
					break;
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1E:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2E:
				case OptionDispjudgeID.Type3B:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGood;
					if (null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeLate;
					}
					break;
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeLateGood;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeLate;
					break;
				}
				break;
			case NoteJudge.ETiming.FastGreat3rd:
			case NoteJudge.ETiming.FastGreat2nd:
			case NoteJudge.ETiming.FastGreat:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type2A:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGreat;
					break;
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1E:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2E:
				case OptionDispjudgeID.Type3B:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGreat;
					if (null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeFast;
					}
					break;
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeFastGreat;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeFast;
					break;
				}
				break;
			case NoteJudge.ETiming.LateGreat:
			case NoteJudge.ETiming.LateGreat2nd:
			case NoteJudge.ETiming.LateGreat3rd:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type2A:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGreat;
					break;
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1E:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2E:
				case OptionDispjudgeID.Type3B:
					SpriteRender.sprite = GameNoteImageContainer.JudgeGreat;
					if (null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeLate;
					}
					break;
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeLateGreat;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeLate;
					break;
				}
				break;
			case NoteJudge.ETiming.FastPerfect2nd:
			case NoteJudge.ETiming.FastPerfect:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type2A:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2C:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					break;
				case OptionDispjudgeID.Type1E:
				case OptionDispjudgeID.Type2E:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					if (type == NoteJudge.EJudgeType.Break && null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeFast;
					}
					break;
				case OptionDispjudgeID.Type3B:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					if (null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeFast;
					}
					break;
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeFastPerfect;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
					base.gameObject.SetActive(value: false);
					break;
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeFast;
					break;
				}
				break;
			case NoteJudge.ETiming.LatePerfect:
			case NoteJudge.ETiming.LatePerfect2nd:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type2A:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2C:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					break;
				case OptionDispjudgeID.Type1E:
				case OptionDispjudgeID.Type2E:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					if (type == NoteJudge.EJudgeType.Break && null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeLate;
					}
					break;
				case OptionDispjudgeID.Type3B:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					if (null != SpriteRenderFastLate)
					{
						SpriteRenderFastLate.gameObject.SetActive(value: true);
						SpriteRenderFastLate.sprite = GameNoteImageContainer.JudgeLate;
					}
					break;
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeLatePerfect;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
					base.gameObject.SetActive(value: false);
					break;
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeLate;
					break;
				}
				break;
			case NoteJudge.ETiming.Critical:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type1E:
					SpriteRender.sprite = GameNoteImageContainer.JudgePerfect;
					break;
				case OptionDispjudgeID.Type2A:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type2E:
				case OptionDispjudgeID.Type3B:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeCritical;
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3D:
					base.gameObject.SetActive(value: false);
					break;
				}
				break;
			}
		}

		public void InitializeBreak(NoteJudge.ETiming judge, float msec, NoteJudge.EJudgeType type)
		{
			Initialize(judge, msec, type);
			if (judge == NoteJudge.ETiming.Critical)
			{
				SpriteRenderAdd.gameObject.SetActive(value: true);
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1A:
				case OptionDispjudgeID.Type1B:
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type1E:
					SpriteRenderAdd.sprite = GameNoteImageContainer.JudgePerfectBreak;
					SpriteRenderAdd.gameObject.SetActive(value: true);
					break;
				case OptionDispjudgeID.Type2A:
				case OptionDispjudgeID.Type2B:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type2E:
				case OptionDispjudgeID.Type3B:
				case OptionDispjudgeID.Type3C:
					SpriteRenderAdd.sprite = GameNoteImageContainer.JudgeCriticalBreak;
					SpriteRenderAdd.gameObject.SetActive(value: true);
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
					break;
				}
			}
		}

		private void DispLed(NoteJudge.ETiming judge)
		{
			if (_buttonIndex != -1 && !GameManager.IsAdvDemo)
			{
				switch (NoteJudge.ConvertJudge(judge))
				{
				case NoteJudge.JudgeBox.Good:
					MechaManager.LedIf[_monitorIndex].SetColorButtonPressed((byte)_buttonIndex, Color.green);
					break;
				case NoteJudge.JudgeBox.Great:
					MechaManager.LedIf[_monitorIndex].SetColorButtonPressed((byte)_buttonIndex, Color.red);
					break;
				case NoteJudge.JudgeBox.Perfect:
				case NoteJudge.JudgeBox.Critical:
					MechaManager.LedIf[_monitorIndex].SetColorButtonPressed((byte)_buttonIndex, Color.yellow);
					break;
				}
			}
		}
	}
}
