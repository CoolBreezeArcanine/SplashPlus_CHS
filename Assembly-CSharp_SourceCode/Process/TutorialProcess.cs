using System;
using System.Diagnostics;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using Manager.UserDatas;
using Monitor;
using Process.MusicSelectInfo;
using UnityEngine;

namespace Process
{
	public class TutorialProcess : ProcessBase
	{
		private enum TutorialSequence
		{
			Init,
			Start,
			StartWait,
			Play,
			Release
		}

		private TutorialSequence _sequence;

		private TutorialMonitor[] _monitors;

		private bool _startMusic;

		private bool endMusic;

		private const int GamePlayWaitTime = 1000;

		private int _tutorialSkipTime = 3000;

		private readonly Stopwatch _startWait = new Stopwatch();

		private MovieController _gameMovie;

		private readonly long[] _skipInputTimer = new long[2];

		public TutorialProcess(ProcessDataContainer dataContainer)
			: base(dataContainer)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameManager.IsTutorial = true;
			GameObject prefs = Resources.Load<GameObject>("Process/Tutorial/TutorialProcess");
			_monitors = new TutorialMonitor[2]
			{
				CreateInstanceAndSetParent(prefs, container.LeftMonitor).GetComponent<TutorialMonitor>(),
				CreateInstanceAndSetParent(prefs, container.RightMonitor).GetComponent<TutorialMonitor>()
			};
			int num = ((GameManager.TutorialPlayed == GameManager.TutorialEnum.BasicPlay) ? 1 : 0);
			for (int i = 0; i < _monitors.Length; i++)
			{
				GameManager.SelectMusicID[i] = num;
				GameManager.SelectDifficultyID[i] = 0;
				_skipInputTimer[i] = 0L;
			}
			Singleton<GamePlayManager>.Instance.AddPlayLog();
			Singleton<GamePlayManager>.Instance.Initialize();
			for (int j = 0; j < _monitors.Length; j++)
			{
				if (Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry)
				{
					UserOption userOption = Singleton<GamePlayManager>.Instance.GetGameScore(j).UserOption;
					userOption.Initialize();
					userOption.OptionKind = OptionKindID.Custom;
					userOption.NoteSpeed = OptionNotespeedID.Speed1_0;
					userOption.SlideSpeed = OptionSlidespeedID.Fast1_0;
					userOption.TouchSpeed = OptionTouchspeedID.Speed1_0;
				}
				_monitors[j].Initialize(j, Singleton<UserDataManager>.Instance.GetUserData(j).IsEntry);
				SoundManager.StopBGM(j);
			}
			SoundManager.MusicPrepare(num);
			_gameMovie = UnityEngine.Object.Instantiate(CommonPrefab.GetMovieCtrlObject());
			_gameMovie.SetMusicMovie(num);
			GC.Collect();
		}

		public override void OnUpdate()
		{
			TutorialMonitor[] monitors;
			switch (_sequence)
			{
			case TutorialSequence.Init:
				if (SoundManager.IsMusicPrepare() && _gameMovie.IsMoviePrepare() && _monitors[0].IsReady() && _monitors[1].IsReady())
				{
					container.processManager.NotificationFadeIn();
					_sequence = TutorialSequence.Start;
					_startWait.Reset();
					_startWait.Start();
					for (int i = 0; i < _monitors.Length; i++)
					{
						_monitors[i].SetMovieSize(_gameMovie.GetMovieHeight(), _gameMovie.GetMovieWidth());
					}
				}
				break;
			case TutorialSequence.Start:
				if (_startWait.ElapsedMilliseconds > 1000)
				{
					_sequence = TutorialSequence.StartWait;
					NotesManager.StartPlay(0f);
					monitors = _monitors;
					for (int j = 0; j < monitors.Length; j++)
					{
						monitors[j].PlayStart();
					}
					_startWait.Reset();
					_startWait.Start();
				}
				break;
			case TutorialSequence.StartWait:
				UpdateNotes();
				if (_startWait.ElapsedMilliseconds > 91)
				{
					_sequence = TutorialSequence.Play;
					_gameMovie.Play();
					SoundManager.StartMusic();
					_startMusic = true;
				}
				break;
			case TutorialSequence.Play:
				UpdateNotes();
				GameManager.AutoPlay = ((_monitors[0].IsDemoPlaying() || _monitors[1].IsDemoPlaying()) ? GameManager.AutoPlayMode.Critical : GameManager.AutoPlayMode.None);
				break;
			}
			if (_startMusic && SoundManager.IsEndMusic() && !endMusic && _gameMovie.IsEnd() && _monitors[0].IsPlayEnd() && _monitors[1].IsPlayEnd() && _sequence != TutorialSequence.Release && _sequence == TutorialSequence.Play)
			{
				SetRelease();
			}
			if (_sequence == TutorialSequence.Play)
			{
				for (int k = 0; k < _monitors.Length; k++)
				{
					if (Singleton<UserDataManager>.Instance.GetUserData(k).IsEntry)
					{
						if (InputManager.GetButtonPush(k, InputManager.ButtonSetting.Button04) && InputManager.GetButtonPush(k, InputManager.ButtonSetting.Button05))
						{
							_skipInputTimer[k] += GameManager.GetGameMSecAdd();
						}
						else
						{
							_skipInputTimer[k] = 0L;
						}
					}
				}
				if (_tutorialSkipTime <= _skipInputTimer[0] || _tutorialSkipTime <= _skipInputTimer[1])
				{
					SetRelease();
				}
			}
			monitors = _monitors;
			for (int j = 0; j < monitors.Length; j++)
			{
				monitors[j].ViewUpdate();
			}
		}

		public void UpdateNotes()
		{
			NotesManager.UpdateTimer();
			for (int i = 0; i < 4; i++)
			{
				NotesManager.Instance(i).updateNotes();
			}
		}

		private void SetRelease()
		{
			container.processManager.AddProcess(new FadeProcess(container, this, new MusicSelectInfoProcess(container)), 50);
			SoundManager.StopMusic();
			_sequence = TutorialSequence.Release;
			_gameMovie.Stop();
			NotesManager.StopPlay();
			TutorialMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].PlayStop();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			UnityEngine.Object.Destroy(_gameMovie.gameObject);
			_gameMovie = null;
			TutorialMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				UnityEngine.Object.Destroy(monitors[i].gameObject);
			}
			Singleton<GamePlayManager>.Instance.ClaerLog();
			GameManager.AutoPlay = GameManager.AutoPlayMode.None;
			SoundManager.StopBGM(2);
			SoundManager.PlayBGM(Cue.BGM_ENTRY, 2);
			GameManager.IsTutorial = false;
		}
	}
}
