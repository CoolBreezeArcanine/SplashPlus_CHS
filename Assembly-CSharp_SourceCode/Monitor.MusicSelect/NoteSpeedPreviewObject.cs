using Fx;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor.MusicSelect
{
	public class NoteSpeedPreviewObject : MonoBehaviour
	{
		[SerializeField]
		protected Animator _animator;

		[SerializeField]
		private float _speed = 550f;

		[SerializeField]
		private FX_Mai2_Note_Color _particle;

		[SerializeField]
		private NoteJudge.EJudgeType _judgeType = NoteJudge.EJudgeType.End;

		protected int PlayerIndex;

		private float _speedRate;

		private float _judgeTimingDiffMsec;

		private void Start()
		{
			if (_animator == null)
			{
				_animator = GetComponent<Animator>();
			}
			SetSpeed(_speed);
		}

		public virtual void Initialize(int playerIndex)
		{
			PlayerIndex = playerIndex;
		}

		public virtual void AnimationReset()
		{
			_animator?.Rebind();
		}

		public virtual void SetSpeed(float speed)
		{
			_speed = speed;
			float num = 1000f / (speed / 60f) * 4f;
			_speedRate = 60f / (num / 1000f * 60f);
			if (_animator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				_animator.SetFloat("Speed", _speedRate);
			}
		}

		public virtual void SetSpeedRateAnimation()
		{
			if (_animator != null && base.gameObject.activeSelf && base.gameObject.activeInHierarchy)
			{
				_animator.SetFloat("Speed", _speedRate);
			}
		}

		public virtual void Pressed()
		{
			if (!(_animator == null))
			{
				AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
				if (currentAnimatorStateInfo.IsName("Base Layer.Staging"))
				{
					float num = 1000f / (_speed / 60f) * 4f;
					_judgeTimingDiffMsec = num * currentAnimatorStateInfo.normalizedTime - num;
					Judge();
				}
				else if (currentAnimatorStateInfo.IsName("Base Layer.OverRun"))
				{
					float num2 = 1000f / (_speed / 60f) * 4f;
					_judgeTimingDiffMsec = num2 * (1f + currentAnimatorStateInfo.normalizedTime * 0.5f) - num2;
					Judge();
				}
				else if (currentAnimatorStateInfo.IsName("Base Layer.Interval"))
				{
					StartParticle(FX_Mai2_Note_Color.FxColor.White);
				}
			}
		}

		protected void StartParticle(FX_Mai2_Note_Color.FxColor color)
		{
			_particle?.ChangeColor(color);
			_particle?.Play();
			_animator.SetTrigger("Pressed");
		}

		protected void Judge()
		{
			if (_judgeType != NoteJudge.EJudgeType.End)
			{
				NoteJudge.ETiming judgeTiming = NoteJudge.GetJudgeTiming(ref _judgeTimingDiffMsec, Singleton<UserDataManager>.Instance.GetUserData(PlayerIndex).Option.GetJudgeTimingFrame(), _judgeType);
				FX_Mai2_Note_Color.FxColor color = FX_Mai2_Note_Color.FxColor.White;
				switch (NoteJudge.ConvertJudge(judgeTiming))
				{
				case NoteJudge.JudgeBox.Good:
					color = FX_Mai2_Note_Color.FxColor.Good;
					SoundManager.PlayGameSE(Cue.SE_GAME_GOOD, PlayerIndex, 1f);
					break;
				case NoteJudge.JudgeBox.Great:
					color = FX_Mai2_Note_Color.FxColor.Great;
					SoundManager.PlayGameSE(Cue.SE_GAME_GREAT, PlayerIndex, 1f);
					break;
				case NoteJudge.JudgeBox.Perfect:
					color = FX_Mai2_Note_Color.FxColor.Perfect;
					SoundManager.PlayGameSE(Cue.SE_GAME_PERFECT, PlayerIndex, 1f);
					break;
				case NoteJudge.JudgeBox.Critical:
					color = FX_Mai2_Note_Color.FxColor.Perfect;
					SoundManager.PlayGameSE(Cue.SE_GAME_PERFECT, PlayerIndex, 1f);
					SoundManager.PlayGameSE(Cue.SE_GAME_CRITICAL_PERFECT, PlayerIndex, 1f);
					break;
				}
				StartParticle(color);
			}
		}
	}
}
