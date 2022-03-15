using DB;
using MAI2.Util;
using MAI2System;
using Process;
using UnityEngine;

namespace Monitor
{
	public class SlideJudge : MonoBehaviour
	{
		public enum SlideJudgeType
		{
			Normal,
			Circle,
			Fan,
			End
		}

		public enum SlideAngle
		{
			Left,
			Right,
			End
		}

		[SerializeField]
		private SlideJudgeType _judgeType;

		private SlideAngle _angle;

		private NoteJudge.ETiming _judge;

		private Animator _animator;

		protected SpriteRenderer SpriteRender;

		private OptionDispjudgeID _dispJudge;

		private TextMesh _msec;

		public Transform ParentTransform { get; set; }

		public void SetJudgeType(SlideJudgeType type)
		{
			_judgeType = type;
		}

		protected virtual float GetBaseZPosition()
		{
			return -9f;
		}

		private void Awake()
		{
			SpriteRender = base.gameObject.GetComponent<SpriteRenderer>();
			_animator = base.gameObject.GetComponent<Animator>();
			if (Singleton<SystemConfig>.Instance.config.IsDebugDisp)
			{
				GameObject gameObject = new GameObject();
				gameObject.gameObject.AddComponent<TextMesh>();
				_msec = gameObject.GetComponent<TextMesh>();
				_msec.transform.SetParent(base.gameObject.transform);
				_msec.transform.localPosition = new Vector3(-100f, 60f, 0f);
				_msec.transform.localScale = new Vector3(30f, 30f, 0f);
				_msec.transform.localRotation = Quaternion.identity;
				_msec.alignment = TextAlignment.Center;
				_msec.anchor = TextAnchor.MiddleCenter;
			}
		}

		public void SetOption(OptionDispjudgeID dispJudge)
		{
			_dispJudge = dispJudge;
		}

		public void Flip(bool flip)
		{
			_angle = ((!flip) ? SlideAngle.Right : SlideAngle.Left);
		}

		public void Execute()
		{
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
			{
				base.transform.SetParent(ParentTransform, worldPositionStays: false);
				base.gameObject.SetActive(value: false);
			}
		}

		public void PreLoad()
		{
			base.gameObject.SetActive(value: true);
			_animator.Play("SlideJudge@Action", -1, 0f);
		}

		public void PreLoadStop()
		{
			base.gameObject.SetActive(value: false);
		}

		public void Initialize(NoteJudge.ETiming judge, float msec)
		{
			base.gameObject.SetActive(value: true);
			_animator.Play("SlideJudge@Action", -1, 0f);
			_animator.Update(0f);
			_judge = judge;
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
			switch (_judge)
			{
			case NoteJudge.ETiming.TooFast:
				SpriteRender.sprite = GameNoteImageContainer.JudgeSlideTooFast[(int)_judgeType, (int)_angle];
				break;
			case NoteJudge.ETiming.TooLate:
				SpriteRender.sprite = GameNoteImageContainer.JudgeSlideTooLate[(int)_judgeType, (int)_angle];
				break;
			case NoteJudge.ETiming.FastGood:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3C:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideFastGoodCol[(int)_judgeType, (int)_angle];
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideFastGood[(int)_judgeType, (int)_angle];
					break;
				}
				break;
			case NoteJudge.ETiming.LateGood:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3C:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideLateGoodCol[(int)_judgeType, (int)_angle];
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideLateGood[(int)_judgeType, (int)_angle];
					break;
				}
				break;
			case NoteJudge.ETiming.LateGreat:
			case NoteJudge.ETiming.LateGreat2nd:
			case NoteJudge.ETiming.LateGreat3rd:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3C:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideLateGreatCol[(int)_judgeType, (int)_angle];
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideLateGreat[(int)_judgeType, (int)_angle];
					break;
				}
				break;
			case NoteJudge.ETiming.FastGreat3rd:
			case NoteJudge.ETiming.FastGreat2nd:
			case NoteJudge.ETiming.FastGreat:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type1C:
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2C:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3C:
				case OptionDispjudgeID.Type3D:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideFastGreatCol[(int)_judgeType, (int)_angle];
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideFastGreat[(int)_judgeType, (int)_angle];
					break;
				}
				break;
			case NoteJudge.ETiming.FastPerfect2nd:
			case NoteJudge.ETiming.FastPerfect:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideFastPerfectCol[(int)_judgeType, (int)_angle];
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type3D:
					base.gameObject.SetActive(value: false);
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlidePerfect[(int)_judgeType, (int)_angle];
					break;
				}
				break;
			case NoteJudge.ETiming.LatePerfect:
			case NoteJudge.ETiming.LatePerfect2nd:
				switch (_dispJudge)
				{
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3C:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideLatePerfectCol[(int)_judgeType, (int)_angle];
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type3D:
					base.gameObject.SetActive(value: false);
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlidePerfect[(int)_judgeType, (int)_angle];
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
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlidePerfect[(int)_judgeType, (int)_angle];
					break;
				case OptionDispjudgeID.Type1D:
				case OptionDispjudgeID.Type2D:
				case OptionDispjudgeID.Type3D:
					base.gameObject.SetActive(value: false);
					break;
				default:
					SpriteRender.sprite = GameNoteImageContainer.JudgeSlideCritical[(int)_judgeType, (int)_angle];
					break;
				}
				break;
			}
		}
	}
}
