using Fx;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Monitor
{
	public class TouchEffect : MonoBehaviour
	{
		public enum Particle
		{
			Touch,
			Tap,
			HoldOn,
			HoldOff,
			ExTap,
			Break,
			Center,
			End
		}

		private GameObject[] _particleRootObject = new GameObject[7];

		private FX_Mai2_Note_Color[] _particleControler = new FX_Mai2_Note_Color[7];

		private const int InvalidPlayID = -1;

		private int _playingIndex;

		private int _touchEffectFrame;

		private const int DelayTouchEffectFrame = 7;

		private void Awake()
		{
			_playingIndex = -1;
			_touchEffectFrame = 0;
		}

		public void SetUpParticle(int monitorIndex)
		{
			_playingIndex = -1;
			int tapDesign = (int)Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).UserOption.TapDesign;
			int holdDesign = (int)Singleton<GamePlayManager>.Instance.GetGameScore(monitorIndex).UserOption.HoldDesign;
			_particleRootObject[0] = Object.Instantiate(GameParticleContainer.TouchEffect, base.transform);
			_particleRootObject[1] = Object.Instantiate(GameParticleContainer.TapEffect[tapDesign], base.transform);
			_particleRootObject[4] = Object.Instantiate(GameParticleContainer.TapExEffect[tapDesign], base.transform);
			_particleRootObject[6] = Object.Instantiate(GameParticleContainer.CenterEffect, base.transform);
			_particleRootObject[2] = Object.Instantiate(GameParticleContainer.HoldEffect[holdDesign], base.transform);
			_particleRootObject[3] = Object.Instantiate(GameParticleContainer.HoldReleaseEffect[holdDesign], base.transform);
			_particleRootObject[5] = Object.Instantiate(GameParticleContainer.BreakEffect[tapDesign], base.transform);
			for (int i = 0; i < _particleRootObject.Length; i++)
			{
				_particleRootObject[i].SetActive(value: true);
				_particleControler[i] = _particleRootObject[i].GetComponent<FX_Mai2_Note_Color>();
				_particleControler[i]?.Play();
			}
			base.gameObject.SetActive(value: true);
		}

		public void PreLoad()
		{
			_playingIndex = -1;
			base.gameObject.SetActive(value: true);
			for (int i = 0; i < 7; i++)
			{
				_particleRootObject[i].SetActive(value: true);
				_particleControler[i]?.Play();
			}
		}

		public void PreLoadFinish()
		{
			_playingIndex = -1;
			base.gameObject.SetActive(value: false);
			for (int i = 0; i < 7; i++)
			{
				_particleRootObject[_playingIndex].SetActive(value: false);
				_particleControler[_playingIndex]?.Stop();
			}
		}

		private void PlayParticles(int playingIndex, FX_Mai2_Note_Color.FxColor color)
		{
			_playingIndex = playingIndex;
			for (int i = 0; i < 7; i++)
			{
				if (i == playingIndex)
				{
					if (!base.gameObject.activeSelf)
					{
						base.gameObject.SetActive(value: true);
					}
					if (!_particleRootObject[i].activeSelf)
					{
						_particleRootObject[i].SetActive(value: true);
					}
					_particleControler[i]?.ChangeColor(color);
					_particleControler[i]?.Play();
				}
				else if (_particleRootObject[i].activeSelf)
				{
					_particleRootObject[i].SetActive(value: false);
				}
			}
		}

		public void StopAll()
		{
			_playingIndex = -1;
			base.gameObject.SetActive(value: false);
			for (int i = 0; i < 7; i++)
			{
				_particleRootObject[i]?.SetActive(value: false);
				_particleControler[i]?.Stop();
			}
		}

		public void Initialize()
		{
			if ((_playingIndex == -1 || _playingIndex == 0) && _touchEffectFrame == 0)
			{
				_touchEffectFrame = 7;
				PlayParticles(0, FX_Mai2_Note_Color.FxColor.NoEffect);
			}
		}

		public void Initialize(NoteJudge.ETiming judge)
		{
			switch (NoteJudge.ConvertJudge(judge))
			{
			case NoteJudge.JudgeBox.Good:
				PlayParticles(1, FX_Mai2_Note_Color.FxColor.Good);
				break;
			case NoteJudge.JudgeBox.Great:
				PlayParticles(1, FX_Mai2_Note_Color.FxColor.Great);
				break;
			case NoteJudge.JudgeBox.Perfect:
			case NoteJudge.JudgeBox.Critical:
				PlayParticles(1, FX_Mai2_Note_Color.FxColor.Perfect);
				break;
			case NoteJudge.JudgeBox.Miss:
				break;
			}
		}

		public void InitializeEx(NoteJudge.ETiming judge)
		{
			if (judge != 0 && judge != NoteJudge.ETiming.TooLate)
			{
				PlayParticles(4, FX_Mai2_Note_Color.FxColor.Perfect);
			}
		}

		public void InitializeCenter(NoteJudge.ETiming judge)
		{
			switch (NoteJudge.ConvertJudge(judge))
			{
			case NoteJudge.JudgeBox.Good:
				PlayParticles(6, FX_Mai2_Note_Color.FxColor.Good);
				break;
			case NoteJudge.JudgeBox.Great:
				PlayParticles(6, FX_Mai2_Note_Color.FxColor.Great);
				break;
			case NoteJudge.JudgeBox.Perfect:
			case NoteJudge.JudgeBox.Critical:
				PlayParticles(6, FX_Mai2_Note_Color.FxColor.Perfect);
				break;
			case NoteJudge.JudgeBox.Miss:
				break;
			}
		}

		public void InitializeHold(NoteJudge.ETiming judge)
		{
			switch (NoteJudge.ConvertJudge(judge))
			{
			case NoteJudge.JudgeBox.Miss:
				PlayParticles(2, FX_Mai2_Note_Color.FxColor.White);
				break;
			case NoteJudge.JudgeBox.Good:
				PlayParticles(2, FX_Mai2_Note_Color.FxColor.Good);
				break;
			case NoteJudge.JudgeBox.Great:
				PlayParticles(2, FX_Mai2_Note_Color.FxColor.Great);
				break;
			case NoteJudge.JudgeBox.Perfect:
			case NoteJudge.JudgeBox.Critical:
				PlayParticles(2, FX_Mai2_Note_Color.FxColor.Perfect);
				break;
			}
		}

		public void FinishHold(NoteJudge.ETiming judge)
		{
			switch (NoteJudge.ConvertJudge(judge))
			{
			case NoteJudge.JudgeBox.Miss:
				StopHoldPlay();
				break;
			case NoteJudge.JudgeBox.Good:
				PlayParticles(3, FX_Mai2_Note_Color.FxColor.Good);
				break;
			case NoteJudge.JudgeBox.Great:
				PlayParticles(3, FX_Mai2_Note_Color.FxColor.Great);
				break;
			case NoteJudge.JudgeBox.Perfect:
			case NoteJudge.JudgeBox.Critical:
				PlayParticles(3, FX_Mai2_Note_Color.FxColor.Perfect);
				break;
			}
		}

		public void InitializeBreak(NoteJudge.ETiming judge)
		{
			switch (NoteJudge.ConvertJudge(judge))
			{
			case NoteJudge.JudgeBox.Good:
				PlayParticles(5, FX_Mai2_Note_Color.FxColor.Good);
				break;
			case NoteJudge.JudgeBox.Great:
				PlayParticles(5, FX_Mai2_Note_Color.FxColor.Great);
				break;
			case NoteJudge.JudgeBox.Perfect:
			case NoteJudge.JudgeBox.Critical:
				PlayParticles(5, FX_Mai2_Note_Color.FxColor.Perfect);
				break;
			case NoteJudge.JudgeBox.Miss:
				break;
			}
		}

		public void StopHoldPlay()
		{
			if (_playingIndex == 2)
			{
				base.gameObject.SetActive(value: false);
				_particleRootObject[_playingIndex].SetActive(value: false);
				_particleControler[_playingIndex]?.Stop();
				_playingIndex = -1;
			}
		}

		public virtual void Execute()
		{
			if (_touchEffectFrame != 0)
			{
				_touchEffectFrame--;
			}
			if (_playingIndex != -1 && _particleControler[_playingIndex].IsStop())
			{
				base.gameObject.SetActive(value: false);
				_particleRootObject[_playingIndex].SetActive(value: false);
				_playingIndex = -1;
				_touchEffectFrame = 0;
			}
		}
	}
}
