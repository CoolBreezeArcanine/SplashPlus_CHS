using System;
using DB;
using Mai2.Mai2Cue;
using MAI2.Util;
using Manager;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Game
{
	public class GameAchiveNum : MonoBehaviour
	{
		public enum AchiveColorEnum
		{
			UnderRankA,
			UnderRankSSS,
			UpperAP,
			End
		}

		public enum OrderColorEnum
		{
			VsWin,
			VsLose,
			End
		}

		[SerializeField]
		[Header("中央表示")]
		private MultipleImage _title;

		[SerializeField]
		private Animator _titleAnim;

		[SerializeField]
		private SpriteCounter _intNum;

		[SerializeField]
		[Header("正数データセンタリングのためのずらし量")]
		private int _intXOffset;

		private SpriteCounter _intNumMain;

		[SerializeField]
		[Header("達成率表示整数部")]
		private SpriteCounter _floatNum;

		private SpriteCounter _floatNumMain;

		[SerializeField]
		[Header("達成率表示少数部")]
		private SpriteCounter _floatNumDenomi;

		private SpriteCounter _floatNumDenomiMain;

		private int _intCount;

		[SerializeField]
		[Header("達成率画像Obj")]
		private Image _achiveTiele;

		[SerializeField]
		[Header("SYNC表示")]
		private Image _titleSync;

		[SerializeField]
		[Header("SYNCデータセンタリングのためのずらし量")]
		private int _syncXOffset;

		[SerializeField]
		private SpriteCounter _syncNum;

		private SpriteCounter _syncNumMain;

		private int _syncCount;

		[SerializeField]
		[Header("VS表示")]
		private Image _titleVs;

		[SerializeField]
		[Header("VS表示 達成率差分整数部")]
		private SpriteCounter _achieveNum;

		private SpriteCounter _achieveNumMain;

		[SerializeField]
		[Header("VS表示 達成率差分小数部")]
		private SpriteCounter _achieveNumDenomi;

		private SpriteCounter _achieveNumDenomiMain;

		[SerializeField]
		[Header("VS表示 達成率差分順位")]
		private MultipleImage _achieveRank;

		[SerializeField]
		[Header("Life")]
		private GameObject _lifeObj;

		[SerializeField]
		private SpriteCounter _lifeNum;

		[SerializeField]
		[Header("CourseLife")]
		private GameObject _courseLifeObj;

		[SerializeField]
		private Transform _courseLifeTransform;

		private CourseLife _courseLife;

		[SerializeField]
		[Header("シャッター")]
		private Animator _shutter;

		private int _monitorIndex;

		private OptionCenterdisplayID _displayCenter;

		private OptionDispchainID _displayChain;

		private decimal _boarderAchiveS;

		private decimal _boarderAchiveSS;

		private decimal _boarderAchiveSSS;

		private decimal _boarderAchiveBest;

		private int _preData;

		private int _preSyncData;

		private decimal _preAchieveData;

		private uint _preVsRank;

		private bool _isGhost;

		private int _dispDelayCount;

		private int _dispDelayCountForVs;

		private const int DISP_DELAY_FRAME = 3;

		private int _animCount;

		private const int WAIT_ANIM_COUNT = 30;

		private bool _isChallenge;

		private int _life = 100;

		private bool _isStartLifeZero;

		private bool _forceShutter;

		private bool _isCourse;

		private bool _isCallShutter;

		public void SetLife()
		{
			string text = _life.ToString("D000");
			if (_life > 999)
			{
				text = "999";
			}
			_lifeNum.ChangeText(text);
			if (_life < 100)
			{
				_lifeNum.FrameList[2].Scale = 0f;
				_lifeNum.FrameList[0].RelativePosition.x = 52f;
				_lifeNum.FrameList[1].RelativePosition.x = 24f;
				if (_life < 10)
				{
					_lifeNum.FrameList[1].Scale = 0f;
					_lifeNum.FrameList[0].RelativePosition.x = 74f;
				}
			}
		}

		public void SetCourseLife()
		{
			_courseLife.SetLife((uint)_life, isGame: true);
		}

		public void SetForceShutter()
		{
			_forceShutter = true;
		}

		public void Initialize(int monIndex, int myBest)
		{
			_monitorIndex = monIndex;
			_displayCenter = Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).UserOption.DispCenter;
			_displayChain = Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).UserOption.DispChain;
			_courseLife = UnityEngine.Object.Instantiate(_courseLifeObj, _courseLifeTransform).GetComponent<CourseLife>();
			_isChallenge = false;
			if (GameManager.IsPerfectChallenge)
			{
				_isChallenge = true;
			}
			_isCourse = false;
			if (GameManager.IsCourseMode)
			{
				_isCourse = true;
			}
			_isCallShutter = false;
			_lifeObj.gameObject.transform.parent.gameObject.SetActive(value: false);
			_lifeObj.gameObject.SetActive(value: false);
			_courseLife.gameObject.transform.parent.gameObject.SetActive(value: false);
			_courseLife.gameObject.SetActive(value: false);
			bool flag = false;
			if (_isChallenge)
			{
				int num = GameManager.SelectMusicID[_monitorIndex];
				if (num > 0)
				{
					NotesWrapper notesWrapper = Singleton<NotesListManager>.Instance.GetNotesList()[num];
					for (int i = 0; i < notesWrapper.ChallengeDetail.Length; i++)
					{
						if (notesWrapper.ChallengeDetail[i].isEnable && notesWrapper.ChallengeDetail[i].startLife > 0)
						{
							_life = notesWrapper.ChallengeDetail[i].startLife;
							flag = true;
							break;
						}
					}
				}
				SetLife();
			}
			if (!flag)
			{
				_isChallenge = false;
			}
			if (_isChallenge)
			{
				_displayCenter = OptionCenterdisplayID.Off;
				_displayChain = OptionDispchainID.Off;
				_lifeObj.gameObject.transform.parent.gameObject.SetActive(value: true);
				_lifeObj.gameObject.SetActive(value: true);
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsChallenge = true;
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).SetLife((uint)_life);
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).SetStartLife((uint)_life);
			}
			bool flag2 = false;
			if (_isCourse)
			{
				if (GameManager.IsCourseMode)
				{
					_life = Singleton<CourseManager>.Instance.GetRestLife(monIndex);
					flag2 = true;
					if (_life == 0)
					{
						_isStartLifeZero = true;
					}
				}
				SetCourseLife();
			}
			if (!flag2)
			{
				_isCourse = false;
			}
			if (_isCourse)
			{
				_displayCenter = OptionCenterdisplayID.Off;
				_displayChain = OptionDispchainID.Off;
				bool isCourseForceFail = Singleton<CourseManager>.Instance.IsForceFail();
				_courseLife.gameObject.transform.parent.gameObject.SetActive(value: true);
				_courseLife.gameObject.SetActive(value: true);
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsCourse = true;
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsCourseForceFail = isCourseForceFail;
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).SetLife((uint)_life);
				Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).SetStartLife((uint)_life);
			}
			_shutter.gameObject.transform.parent.gameObject.SetActive(value: false);
			_shutter.gameObject.gameObject.SetActive(value: false);
			_intNumMain = _intNum.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
			_floatNumMain = _floatNum.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
			_floatNumDenomiMain = _floatNumDenomi.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
			_syncNumMain = _syncNum.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
			_achieveNumMain = _achieveNum.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
			_achieveNumDenomiMain = _achieveNumDenomi.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
			_intCount = _intNum.FrameList.Count;
			_syncCount = _syncNum.FrameList.Count;
			_preAchieveData = -1m;
			_preVsRank = 1u;
			_isGhost = GameManager.SelectGhostID[_monitorIndex] != GhostManager.GhostTarget.End;
			if (_displayCenter == OptionCenterdisplayID.Off)
			{
				_title.gameObject.SetActive(value: false);
			}
			else
			{
				_achiveTiele.gameObject.SetActive(value: false);
				_title.gameObject.SetActive(value: true);
				_title.ChangeSprite((int)_displayCenter);
				_intNum.gameObject.SetActive(value: false);
				_floatNum.gameObject.SetActive(value: false);
				_floatNumDenomi.gameObject.SetActive(value: false);
				_intNumMain.SetColor(CommonScriptable.GetColorSetting().GameCenterNumColor[(int)_displayCenter]);
				_floatNumMain.SetColor(CommonScriptable.GetColorSetting().GameCenterNumColor[(int)_displayCenter]);
				_floatNumDenomiMain.SetColor(CommonScriptable.GetColorSetting().GameCenterNumColor[(int)_displayCenter]);
				switch (_displayCenter)
				{
				case OptionCenterdisplayID.Combo:
					_intNum.gameObject.SetActive(value: true);
					break;
				case OptionCenterdisplayID.AchivePlus:
				case OptionCenterdisplayID.AchiveMinus1:
				case OptionCenterdisplayID.AchiveMinus2:
					_achiveTiele.gameObject.SetActive(value: true);
					_floatNum.gameObject.SetActive(value: true);
					_floatNumDenomi.gameObject.SetActive(value: true);
					break;
				case OptionCenterdisplayID.DeluxScore:
				case OptionCenterdisplayID.DeluxScoreMinus:
					_intNum.gameObject.SetActive(value: true);
					break;
				case OptionCenterdisplayID.BoarderS:
				case OptionCenterdisplayID.BoarderSS:
				case OptionCenterdisplayID.BoarderSSS:
				case OptionCenterdisplayID.BoarderBest:
					_floatNum.gameObject.SetActive(value: true);
					_floatNumDenomi.gameObject.SetActive(value: true);
					break;
				}
			}
			switch (_displayChain)
			{
			case OptionDispchainID.Off:
				_titleSync.gameObject.SetActive(value: false);
				_titleVs.gameObject.SetActive(value: false);
				break;
			case OptionDispchainID.Achievement:
				_titleSync.gameObject.SetActive(value: false);
				if (_isGhost)
				{
					_titleVs.gameObject.SetActive(value: true);
					_achieveRank.gameObject.SetActive(value: false);
				}
				else if (Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() != 1)
				{
					_titleVs.gameObject.SetActive(value: true);
				}
				else
				{
					_titleVs.gameObject.SetActive(value: false);
				}
				break;
			case OptionDispchainID.Sync:
				_titleVs.gameObject.SetActive(value: false);
				if (Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() != 1)
				{
					_titleSync.gameObject.SetActive(value: true);
				}
				else
				{
					_titleSync.gameObject.SetActive(value: false);
				}
				break;
			}
			_boarderAchiveS = 101.0m - (decimal)MusicClearrankID.Rank_S.GetAchvement() / 10000m;
			_boarderAchiveSS = 101.0m - (decimal)MusicClearrankID.Rank_SS.GetAchvement() / 10000m;
			_boarderAchiveSSS = 101.0m - (decimal)MusicClearrankID.Rank_SSS.GetAchvement() / 10000m;
			_boarderAchiveBest = (decimal)myBest / 10000m;
			_preData = -1;
			_preSyncData = -1;
			_dispDelayCount = 0;
			_dispDelayCountForVs = 0;
			_animCount = -30;
		}

		public void Execute()
		{
			int num = 0;
			switch (_displayCenter)
			{
			case OptionCenterdisplayID.Combo:
			case OptionCenterdisplayID.DeluxScore:
			case OptionCenterdisplayID.DeluxScoreMinus:
				if (OptionCenterdisplayID.Combo == _displayCenter)
				{
					num = (int)Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).Combo;
				}
				else if (OptionCenterdisplayID.DeluxScore == _displayCenter)
				{
					num = (int)Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetDeluxeScoreAll();
				}
				else if (OptionCenterdisplayID.DeluxScoreMinus == _displayCenter)
				{
					num = Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetDeluxeScoreMinusAll();
				}
				if (_animCount == -30 && OptionCenterdisplayID.Combo == _displayCenter)
				{
					_titleAnim.gameObject.SetActive(value: false);
				}
				else if (_animCount == -29 && OptionCenterdisplayID.Combo == _displayCenter)
				{
					_titleAnim.gameObject.SetActive(value: true);
				}
				else
				{
					if (num == _preData)
					{
						break;
					}
					if (num == 0 && OptionCenterdisplayID.Combo == _displayCenter)
					{
						_titleAnim.gameObject.SetActive(value: false);
					}
					else
					{
						int num2 = ((num == 0) ? ((OptionCenterdisplayID.DeluxScoreMinus != _displayCenter) ? 1 : 2) : ((0 >= num) ? ((int)Math.Log10(-num) + 2) : ((int)Math.Log10(num) + 1)));
						num2 = _intCount - num2;
						_intNum.transform.localPosition = new Vector3(-(_intXOffset * num2), _intNum.transform.localPosition.y, _intNum.transform.localPosition.z);
						_titleAnim.gameObject.SetActive(value: true);
						if (_displayCenter == OptionCenterdisplayID.DeluxScoreMinus && num == 0)
						{
							_intNum.ChangeText("   -0");
							_intNumMain.ChangeText("   -0");
						}
						else
						{
							_intNum.ChangeText(StringEx.ToStringInt(num, ' ', 5));
							_intNumMain.ChangeText(StringEx.ToStringInt(num, ' ', 5));
						}
					}
					_preData = num;
				}
				break;
			case OptionCenterdisplayID.AchivePlus:
			case OptionCenterdisplayID.AchiveMinus1:
			case OptionCenterdisplayID.AchiveMinus2:
				if (OptionCenterdisplayID.AchiveMinus1 == _displayCenter)
				{
					num = (int)Singleton<GamePlayManager>.Instance.GetDecAchivement(_monitorIndex);
				}
				else if (OptionCenterdisplayID.AchiveMinus2 == _displayCenter)
				{
					num = (int)Singleton<GamePlayManager>.Instance.GetDecTheoryAchivement(_monitorIndex);
				}
				else if (OptionCenterdisplayID.AchivePlus == _displayCenter)
				{
					num = (int)Singleton<GamePlayManager>.Instance.GetAchivement(_monitorIndex);
				}
				if (num != _preData)
				{
					string text4 = Convert.ToString(num).PadLeft(7, '0');
					string text5 = string.Format("{0,3}", (num / 10000).ToString());
					string text6 = "." + text4.Substring(3, 4) + "%";
					_floatNum.ChangeText(text5);
					_floatNumMain.ChangeText(text5);
					_floatNumDenomi.ChangeText(text6);
					_floatNumDenomiMain.ChangeText(text6);
					int achiveColor = (int)GetAchiveColor(num);
					_floatNumMain.SetColor(CommonScriptable.GetColorSetting().GameAchiveColor[achiveColor]);
					_floatNumDenomiMain.SetColor(CommonScriptable.GetColorSetting().GameAchiveColor[achiveColor]);
					_achiveTiele.color = CommonScriptable.GetColorSetting().GameAchiveColor[achiveColor];
				}
				_preData = num;
				break;
			case OptionCenterdisplayID.BoarderS:
			case OptionCenterdisplayID.BoarderSS:
			case OptionCenterdisplayID.BoarderSSS:
			case OptionCenterdisplayID.BoarderBest:
				num = ((OptionCenterdisplayID.BoarderS == _displayCenter) ? GameManager.ConvAchiveDecimalToInt(_boarderAchiveS - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff()) : ((OptionCenterdisplayID.BoarderSS == _displayCenter) ? GameManager.ConvAchiveDecimalToInt(_boarderAchiveSS - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff()) : ((OptionCenterdisplayID.BoarderSSS == _displayCenter) ? GameManager.ConvAchiveDecimalToInt(_boarderAchiveSSS - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff()) : GameManager.ConvAchiveDecimalToInt(101.0m - _boarderAchiveBest - Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).GetTheoryAchivementDiff()))));
				if (num != _preData)
				{
					if (num < 0)
					{
						_title.gameObject.SetActive(value: false);
					}
					else
					{
						string text = Convert.ToString(num).PadLeft(7, '0');
						string text2 = string.Format("{0,3}", (num / 10000).ToString());
						string text3 = "." + text.Substring(3, 4) + "%";
						_title.gameObject.SetActive(value: true);
						_floatNum.ChangeText(text2);
						_floatNumMain.ChangeText(text2);
						_floatNumDenomi.ChangeText(text3);
						_floatNumDenomiMain.ChangeText(text3);
					}
				}
				_preData = num;
				break;
			}
			switch (_displayChain)
			{
			case OptionDispchainID.Achievement:
			{
				decimal num4 = default(decimal);
				if (_isGhost)
				{
					num4 = Singleton<GamePlayManager>.Instance.GetGhostAchieve(_monitorIndex);
				}
				else if (Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() != 1)
				{
					num4 = Singleton<GamePlayManager>.Instance.GetVsAchieve(_monitorIndex);
				}
				if (_preAchieveData != num4)
				{
					_dispDelayCount++;
				}
				else
				{
					_dispDelayCount = 0;
				}
				if (_dispDelayCount >= 3)
				{
					_preAchieveData = num4;
					int num5 = (int)(num4 * 10000m);
					string text7 = "";
					string text8 = "";
					string text9 = "";
					if (num5 / 10000 == 0 && num5 < 0)
					{
						text7 = Convert.ToString(-num5).PadLeft(8, '0');
						text8 = "  -0";
						text9 = "." + text7.Substring(4, 4) + "%";
					}
					else
					{
						text7 = Convert.ToString(num5).PadLeft(8, '0');
						text8 = string.Format("{0,4}", (num5 / 10000).ToString());
						text9 = "." + text7.Substring(4, 4) + "%";
					}
					_achieveNum.ChangeText(text8);
					_achieveNumMain.ChangeText(text8);
					_achieveNumDenomi.ChangeText(text9);
					_achieveNumDenomiMain.ChangeText(text9);
					int rankColor = (int)GetRankColor(_preAchieveData);
					_achieveNumMain.SetColor(CommonScriptable.GetColorSetting().GameAchiveColor[rankColor]);
					_achieveNumDenomiMain.SetColor(CommonScriptable.GetColorSetting().GameAchiveColor[rankColor]);
					_dispDelayCount = 0;
				}
				uint vsRank = Singleton<GamePlayManager>.Instance.GetVsRank(_monitorIndex);
				if (_preVsRank != vsRank)
				{
					_dispDelayCountForVs++;
				}
				else
				{
					_dispDelayCountForVs = 0;
				}
				if (_dispDelayCountForVs >= 3)
				{
					_preVsRank = vsRank;
					_achieveRank.ChangeSprite((int)_preVsRank);
					_dispDelayCountForVs = 0;
				}
				break;
			}
			case OptionDispchainID.Sync:
				if (Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() != 1)
				{
					uint chainCount = Singleton<GamePlayManager>.Instance.GetChainCount(_monitorIndex);
					if (_preSyncData != chainCount)
					{
						int num3 = ((chainCount == 0) ? 1 : ((int)Math.Log10((int)chainCount) + 1));
						num3 = _syncCount - num3;
						_syncNum.transform.localPosition = new Vector3(-(_syncXOffset * num3), _syncNum.transform.localPosition.y, _syncNum.transform.localPosition.z);
						_syncNum.ChangeText(StringEx.ToStringInt((int)chainCount, ' ', 4));
						_syncNumMain.ChangeText(StringEx.ToStringInt((int)chainCount, ' ', 4));
						_preSyncData = (int)chainCount;
					}
				}
				break;
			}
			switch (_displayCenter)
			{
			case OptionCenterdisplayID.Combo:
				_animCount++;
				break;
			case OptionCenterdisplayID.AchivePlus:
			case OptionCenterdisplayID.AchiveMinus1:
			case OptionCenterdisplayID.AchiveMinus2:
				if (_animCount >= -1)
				{
					if (_animCount != (int)Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).Achivement)
					{
						_animCount = (int)Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).Achivement;
						if (_animCount % 10 == 0)
						{
							_titleAnim?.Play("In");
						}
					}
				}
				else if (_animCount == -30)
				{
					_titleAnim?.Play("Out");
					_animCount++;
				}
				else
				{
					_animCount++;
				}
				break;
			case OptionCenterdisplayID.DeluxScore:
			case OptionCenterdisplayID.DeluxScoreMinus:
			case OptionCenterdisplayID.BoarderS:
			case OptionCenterdisplayID.BoarderSS:
			case OptionCenterdisplayID.BoarderSSS:
			case OptionCenterdisplayID.BoarderBest:
				if (_animCount == 0)
				{
					_titleAnim?.Play("In");
					_animCount++;
				}
				else if (_animCount == -30)
				{
					_titleAnim?.Play("Out");
					_animCount++;
				}
				else
				{
					_animCount++;
				}
				break;
			}
			if (_isChallenge)
			{
				_life = (int)Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).Life;
				SetLife();
				if (GameManager.IsInGame && !_isCallShutter && _life <= 0)
				{
					Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
					_shutter.gameObject.transform.parent.gameObject.SetActive(value: true);
					_shutter.gameObject.gameObject.SetActive(value: true);
					_shutter.Play("Shutter_01", 0, 0f);
					SoundManager.PlaySE(Cue.SE_GAME_SHUTDOWN, _monitorIndex);
					_isCallShutter = true;
				}
			}
			if (!_isCourse)
			{
				return;
			}
			_life = (int)Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).Life;
			SetCourseLife();
			if (GameManager.IsInGame && !_isCallShutter && _life <= 0 && Singleton<GamePlayManager>.Instance.GetGameScore(_monitorIndex).IsCourseForceFail)
			{
				bool flag = true;
				if (!_isStartLifeZero || (_forceShutter ? true : false))
				{
					Singleton<GamePlayManager>.Instance.SetTrackSkipFrag(_monitorIndex);
					_shutter.gameObject.transform.parent.gameObject.SetActive(value: true);
					_shutter.gameObject.gameObject.SetActive(value: true);
					_shutter.Play("Shutter_01", 0, 0f);
					SoundManager.PlaySE(Cue.SE_GAME_SHUTDOWN, _monitorIndex);
					_isCallShutter = true;
				}
			}
		}

		public static AchiveColorEnum GetAchiveColor(int achive)
		{
			MusicClearrankID clearRank = GameManager.GetClearRank(achive);
			if (clearRank < MusicClearrankID.Rank_A)
			{
				return AchiveColorEnum.UnderRankA;
			}
			if (clearRank < MusicClearrankID.Rank_SSS)
			{
				return AchiveColorEnum.UnderRankSSS;
			}
			return AchiveColorEnum.UpperAP;
		}

		public static OrderColorEnum GetRankColor(decimal achive)
		{
			if (achive >= 0m)
			{
				return OrderColorEnum.VsWin;
			}
			return OrderColorEnum.VsLose;
		}
	}
}
