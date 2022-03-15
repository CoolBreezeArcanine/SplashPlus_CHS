using System.Diagnostics;
using System.Text;
using Game;
using UnityEngine;

namespace Manager
{
	public class NotesManager
	{
		public enum BarDefine
		{
			BarDefine_Game,
			BarDefine_Score,
			BarDefine_Max
		}

		private static NotesManager[] _instance = new NotesManager[4];

		private const string DebugWindowName = "ノーツマネージャ";

		private static float _curMSec;

		private static float _curMSecPre;

		private static float _msecStartGap;

		private static Stopwatch _stopwatch = null;

		private static bool _isPlaying = false;

		private int _playerID;

		private bool _isEnableScore;

		private const float _framelimit_large = 0.75f;

		private const float _framelimit_small = 0.1f;

		private const float _frameadjust_large = 1f;

		private const float _frameadjust_small = 0.01f;

		private SessionInfo _sessionInfo;

		private NotesReader _notesReader = new NotesReader();

		private bool[] _isBar = new bool[2];

		private bool[] _isBeat = new bool[2];

		private float[] _barFloat = new float[2];

		private float[] _beatFloat = new float[2];

		private uint[] _bar = new uint[2];

		private uint[] _beat = new uint[2];

		private bool _isHold;

		private float _frameNoteStart;

		private float _frameNoteEnd;

		private float _framePlayStart;

		private float _framePlayEnd;

		private bool _isFinishNote;

		private bool _isFinishPlay;

		public static NotesManager Instance()
		{
			if (_instance[0] == null)
			{
				_instance[0] = new NotesManager();
			}
			return _instance[0];
		}

		public static NotesManager Instance(int monitorId)
		{
			if (_instance[monitorId] == null)
			{
				_instance[monitorId] = new NotesManager();
				_instance[monitorId]._playerID = monitorId;
			}
			return _instance[monitorId];
		}

		public void initialize()
		{
			reset();
		}

		public void reset()
		{
			clearNotes();
			_notesReader.init(GetPlayerID());
			_isHold = false;
			for (int i = 0; i < 2; i++)
			{
				_isBar[i] = false;
				_isBeat[i] = false;
				_barFloat[i] = 0f;
				_beatFloat[i] = 0f;
				_bar[i] = 0u;
				_beat[i] = 0u;
			}
		}

		public void clearNotes()
		{
			_stopwatch = null;
			_isFinishNote = true;
			_isFinishPlay = true;
			_msecStartGap = 0f;
			_isPlaying = false;
			_isEnableScore = false;
			_curMSec = 0f;
			_curMSecPre = 0f;
		}

		public static void StartPlay(float msecStartGap)
		{
			_msecStartGap = msecStartGap;
			_stopwatch = Stopwatch.StartNew();
			_isPlaying = true;
		}

		public static void Pause(bool isPause)
		{
			if (isPause)
			{
				_stopwatch?.Stop();
			}
			else
			{
				_stopwatch?.Start();
			}
		}

		public static void StopPlay()
		{
			_stopwatch = null;
			_isPlaying = false;
		}

		public bool loadScore(SessionInfo sessionInfo)
		{
			_notesReader.init(GetPlayerID());
			_sessionInfo = sessionInfo;
			string c2SPath = getC2SPath();
			if (!_notesReader.load(c2SPath))
			{
				return false;
			}
			_frameNoteStart = getReader().GetCompositioin()._startGameTime.frame;
			_frameNoteEnd = getReader().GetCompositioin()._endGameTime.frame;
			_framePlayStart = Mathf.Min(_framePlayStart, getReader().GetCompositioin()._startNotesTime.frame);
			_framePlayEnd = Mathf.Max(_framePlayEnd, getReader().GetCompositioin()._endNotesTime.frame);
			_isEnableScore = true;
			return true;
		}

		public bool loadScoreHeader(SessionInfo sessionInfo)
		{
			_notesReader.init(GetPlayerID());
			_sessionInfo = sessionInfo;
			string c2SPath = getC2SPath();
			if (!_notesReader.load(c2SPath, LoadType.LOAD_HEADER))
			{
				return false;
			}
			return true;
		}

		public static void UpdateTimer()
		{
			double num = 0.0;
			if (_isPlaying && _stopwatch != null)
			{
				num = (double)_stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0;
			}
			_curMSecPre = _curMSec;
			_curMSec = (float)num + _msecStartGap;
		}

		public void updateNotes()
		{
			float curMSec = _curMSec;
			for (int i = 0; i < 2; i++)
			{
				uint num = 0u;
				uint num2 = 0u;
				BarType type = BarType.GAMEBAR;
				BarType type2 = BarType.GAMEBEAT;
				if (i == 1)
				{
					type = BarType.SCOREBAR;
					type2 = BarType.SCOREBEAT04;
				}
				num = _notesReader.getBarData(type, curMSec).numTotal;
				num2 = _notesReader.getBarData(type2, curMSec).numBar;
				_barFloat[i] = _notesReader.getBarFloat(curMSec, type);
				_beatFloat[i] = _notesReader.getBarFloat(curMSec, type2);
				_isBar[i] = num != _bar[i];
				_isBeat[i] = num2 != _beat[i];
				if (_isBar[i])
				{
					_bar[i] = num;
				}
				if (_isBeat[i])
				{
					_beat[i] = num2;
				}
			}
		}

		public NotesReader getReader()
		{
			return _notesReader;
		}

		public bool isFinishNote()
		{
			return _isFinishNote;
		}

		public bool isFinishPlay()
		{
			return _isFinishPlay;
		}

		public bool isAutoPlay()
		{
			return false;
		}

		public static float GetAddMSec()
		{
			return _curMSec - _curMSecPre;
		}

		public static float GetCurrentMsec()
		{
			return _curMSec;
		}

		public float getCurrentDrawFrame(float frame)
		{
			return _notesReader.getSoflanSeqTiming(GetCurrentMsec(), frame * 16.666666f) * 0.06f;
		}

		public float getCurrentDrawMsec(float msec)
		{
			return _notesReader.getSoflanSeqTiming(GetCurrentMsec(), msec);
		}

		public bool isBar(BarDefine bar = BarDefine.BarDefine_Game)
		{
			return _isBar[(int)bar];
		}

		public bool isBeat(BarDefine bar = BarDefine.BarDefine_Game)
		{
			return _isBeat[(int)bar];
		}

		public float getBarFloat(BarDefine bar = BarDefine.BarDefine_Game)
		{
			return _barFloat[(int)bar];
		}

		public uint getBar(BarDefine bar = BarDefine.BarDefine_Game)
		{
			return _bar[(int)bar];
		}

		public float getBeatFloat(BarDefine bar = BarDefine.BarDefine_Game)
		{
			return _beatFloat[(int)bar];
		}

		public uint getBeat(BarDefine bar = BarDefine.BarDefine_Game)
		{
			return _beat[(int)bar];
		}

		public float getSoflan()
		{
			return getReader().getSoflanChange(GetCurrentMsec());
		}

		public float getEndNoteFrame()
		{
			return _frameNoteEnd;
		}

		public float getEndPlayFrame()
		{
			return _framePlayEnd;
		}

		public float getNowBpm(float msec)
		{
			return getReader().GetBPM_Time(msec);
		}

		public uint getPlayFirstMsec()
		{
			return (uint)(_frameNoteStart * 16.666666f);
		}

		public float getPlayFirstFrame()
		{
			return _frameNoteStart;
		}

		public uint getPlayFinalMsec()
		{
			return (uint)(_frameNoteEnd * 16.666666f);
		}

		public float getPlayFinalFrame()
		{
			return _frameNoteEnd;
		}

		public float getPlayProgress()
		{
			if (_isFinishNote)
			{
				return 1f;
			}
			float num = getPlayFirstMsec();
			float num2 = getPlayFinalMsec();
			float result = 0f;
			if (num2 > num)
			{
				result = Mathf.Clamp((GetCurrentMsec() - num) / (num2 - num), 0f, 1f);
			}
			return result;
		}

		public void setHoldPlay()
		{
			_isHold = true;
		}

		public bool isHoldPlay()
		{
			return _isHold;
		}

		public int GetPlayerID()
		{
			return _playerID;
		}

		public SessionInfo GetSessionInfo()
		{
			return _sessionInfo;
		}

		public bool IsPlaying()
		{
			return _isPlaying;
		}

		public bool IsEnableScore()
		{
			return _isEnableScore;
		}

		private string getC2SPath()
		{
			int musicId = _sessionInfo.musicId;
			int difficulty = _sessionInfo.difficulty;
			if (GameManager.IsNoteCheckMode)
			{
				return GameManager.NoteCheckPath;
			}
			if (_sessionInfo.isTutorial)
			{
				return GameManager.StreamingAssetsPath + string.Format("/A000/music/music{0:000000}/{0:000000}_{1:00}.ma2", musicId, difficulty);
			}
			return _sessionInfo.notesData.file.path;
		}

		public void drawInfo(StringBuilder str)
		{
			if (str != null)
			{
				str.Append("\n");
				str.Append(" ＴＩＭＥ－－－－－－－－\n");
				str.AppendFormat(" \u3000ＭＳＥＣ:{0, 7:00000.0}\n", _curMSec);
				str.AppendFormat(" \u3000\u3000ＡＤＤ:{0, 7:000.000}\n", GetAddMSec());
				str.AppendFormat(" \u3000\u3000ＢＡＲ:{0, 5:000.0}\n", getBar() + 1);
				str.AppendFormat(" \u3000ＢＥＡＴ:{0, 5:000.0}\n", getBeat());
			}
		}
	}
}
