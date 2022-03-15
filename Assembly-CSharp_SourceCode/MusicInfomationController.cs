using System;
using MAI2.Util;
using MAI2System;
using Manager;
using Monitor.Game;
using Timeline;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class MusicInfomationController : MonoBehaviour
{
	[SerializeField]
	private GameObject musicJacketRoot;

	[SerializeField]
	private Image musicJacketBgImage;

	[SerializeField]
	[Header("1P or 2P")]
	private GameObject singlePlayObject;

	[SerializeField]
	private GameObject multiPlayObject;

	[SerializeField]
	[Header("2P通常 or 大会モード")]
	private GameObject multiNormalObject;

	[SerializeField]
	private GameObject multiTournamentObject;

	[SerializeField]
	[Header("基本情報")]
	private RawImage musicJacket;

	[SerializeField]
	private CustomTextScroll MusicNameText;

	[SerializeField]
	private Image difficultyImage;

	[SerializeField]
	[Header("難易度レベル")]
	private SpriteCounter _difficultySingle;

	[SerializeField]
	private SpriteCounter _difficultyDouble;

	[SerializeField]
	private Image _levelTextImage;

	[SerializeField]
	[Header("譜面タイプ")]
	private MultipleImage _scoreKind;

	[SerializeField]
	[Header("達成率")]
	private AchievementCounterObject _singleAchievementCounter;

	[SerializeField]
	private AchievementCounterObject _multiAchievementCounter;

	[SerializeField]
	[Header("順位")]
	private MultipleImage rankText;

	[SerializeField]
	[Header("大会順位")]
	private MultipleImage rankTournamentText;

	[SerializeField]
	[Header("大会用 達成率差分整数部")]
	private SpriteCounter _achieveNum;

	private SpriteCounter _achieveNumMain;

	[SerializeField]
	[Header("大会用 達成率差分小数部")]
	private SpriteCounter _achieveNumDenomi;

	private SpriteCounter _achieveNumDenomiMain;

	[SerializeField]
	[Header("シンクロ")]
	private SpriteCounter syncText;

	[SerializeField]
	private SpriteCounter syncTextOutline;

	private RectTransform _rectTransform;

	private int _achieveSignIndex;

	private bool _isPostMinus;

	public RectTransform RectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

	private void Awake()
	{
		_achieveNumMain = _achieveNum.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
		_achieveNumDenomiMain = _achieveNumDenomi.transform.GetChild(0).gameObject.GetComponent<SpriteCounter>();
	}

	public void SetMusicData(MessageMusicData data)
	{
		if (data != null)
		{
			musicJacket.texture = data.Jacket;
			MusicNameText.SetData(data.Name);
			Setlevel(data.LevelID, data.MusicDifficultyID);
			SetScoreKind(data.Kind);
			ResetMusicNameScrollPosition();
			SetDifficulty((MusicDifficultyID)data.MusicDifficultyID);
		}
	}

	public void SetGameData(MessageGamePlayData data)
	{
		SetDiff(data.Diff);
		SetAchievement(data.Score);
		SetRank(data.Rank);
		SetSync(data.Sync);
	}

	public void SetDifficulty(MusicDifficultyID difficulty)
	{
		string text = "";
		text = difficulty switch
		{
			MusicDifficultyID.Basic => "BSC", 
			MusicDifficultyID.Advanced => "ADV", 
			MusicDifficultyID.Expert => "EXP", 
			MusicDifficultyID.Master => "MST", 
			MusicDifficultyID.ReMaster => "MST_Re", 
			_ => "TYI", 
		};
		difficultyImage.sprite = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_UPE_MBase_" + text);
		musicJacketBgImage.sprite = Resources.Load<Sprite>("Process/Common/Sprites/UpperMonitor/UI_UPE_MusicJacket_Base_" + text);
	}

	public void ResetMusicNameScrollPosition()
	{
		MusicNameText.ResetPosition();
	}

	public void SetVisibles(bool isSingle)
	{
		if (isSingle)
		{
			musicJacketRoot.SetActive(value: true);
			singlePlayObject.SetActive(value: true);
			multiPlayObject.SetActive(value: false);
		}
		else
		{
			musicJacketRoot.SetActive(!GameManager.IsEventMode);
			singlePlayObject.SetActive(value: false);
			multiPlayObject.SetActive(value: true);
			multiNormalObject.SetActive(!SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsEventMode);
			multiTournamentObject.SetActive(SingletonStateMachine<AmManager, AmManager.EState>.Instance.Backup.gameSetting.IsEventMode);
		}
	}

	public void Setlevel(int levelID, int difficulty)
	{
		string levelNum = ((MusicLevelID)levelID).GetLevelNum();
		Sprite[] musicLevelSprites = CommonPrefab.GetMusicLevelSprites(difficulty);
		_difficultySingle.SetSpriteSheet(musicLevelSprites);
		_difficultyDouble.SetSpriteSheet(musicLevelSprites);
		_levelTextImage.sprite = musicLevelSprites[14];
		int num = (levelNum.Contains("+") ? (levelNum.Length - 1) : levelNum.Length);
		if (2 <= num)
		{
			_difficultyDouble.gameObject.SetActive(value: true);
			_difficultyDouble.ChangeText(levelNum.PadRight(3));
			_difficultySingle.gameObject.SetActive(value: false);
		}
		else
		{
			_difficultySingle.gameObject.SetActive(value: true);
			_difficultySingle.ChangeText(levelNum.PadRight(3));
			_difficultyDouble.gameObject.SetActive(value: false);
		}
	}

	public void SetScoreKind(ConstParameter.ScoreKind kind)
	{
		switch (kind)
		{
		case ConstParameter.ScoreKind.Deluxe:
			_scoreKind.ChangeSprite(0);
			break;
		case ConstParameter.ScoreKind.Standard:
			_scoreKind.ChangeSprite(1);
			break;
		}
	}

	public void SetAchievement(uint score)
	{
		if (singlePlayObject.activeSelf)
		{
			_singleAchievementCounter.SetAchievement(0u, score);
			_singleAchievementCounter.OnClipTailEnd();
		}
		else if (multiPlayObject.activeSelf)
		{
			_multiAchievementCounter.SetAchievement(0u, score);
			_multiAchievementCounter.OnClipTailEnd();
		}
	}

	public void SetRank(uint order)
	{
		if (multiPlayObject.activeSelf)
		{
			rankText.ChangeSprite((int)order);
			rankTournamentText.ChangeSprite((int)order);
		}
	}

	public void SetSync(uint sync)
	{
		if (multiPlayObject.activeSelf)
		{
			syncText.ChangeText(StringEx.ToStringInt((int)sync, ' ', 4));
			syncTextOutline.ChangeText(StringEx.ToStringInt((int)sync, ' ', 4));
		}
	}

	public void SetDiff(int diff)
	{
		if (!GameManager.IsEventMode)
		{
			return;
		}
		string text = "";
		string text2 = "";
		string text3 = "";
		if (diff / 10000 == 0 && diff < 0)
		{
			text = Convert.ToString(-diff).PadLeft(8, '0');
			text2 = "  -0";
			text3 = "." + text.Substring(4, 4) + "%";
		}
		else
		{
			text = Convert.ToString(diff).PadLeft(8, '0');
			text2 = string.Format("{0,4}", (diff / 10000).ToString());
			text3 = "." + text.Substring(4, 4) + "%";
		}
		_achieveNum.ChangeText(text2);
		bool flag = false;
		for (int i = 0; i < text2.Length; i++)
		{
			if (text2[i] != '-')
			{
				continue;
			}
			flag = true;
			if (i != _achieveSignIndex)
			{
				_achieveNum.FrameList[i].Scale = 0.8f;
				Vector2 relativePosition = _achieveNum.FrameList[i].RelativePosition;
				relativePosition.x -= 0.4f;
				relativePosition.y = 0.6f;
				_achieveNum.FrameList[i].RelativePosition = relativePosition;
				if (0 <= _achieveSignIndex)
				{
					_achieveNum.FrameList[_achieveSignIndex].Scale = 0.6f;
					Vector2 relativePosition2 = _achieveNum.FrameList[_achieveSignIndex].RelativePosition;
					relativePosition2.x += 0.4f;
					relativePosition2.y = -1f;
					_achieveNum.FrameList[_achieveSignIndex].RelativePosition = relativePosition2;
				}
				_achieveSignIndex = i;
			}
			_isPostMinus = true;
			break;
		}
		if (!flag && _isPostMinus)
		{
			_isPostMinus = false;
			_achieveNum.FrameList[_achieveSignIndex].Scale = 0.6f;
			Vector2 relativePosition3 = _achieveNum.FrameList[_achieveSignIndex].RelativePosition;
			relativePosition3.x += 0.4f;
			relativePosition3.y = -1f;
			_achieveNum.FrameList[_achieveSignIndex].RelativePosition = relativePosition3;
			_achieveSignIndex = -1;
		}
		_achieveNumMain.ChangeText(text2);
		_achieveNumDenomi.ChangeText(text3);
		_achieveNumDenomiMain.ChangeText(text3);
		int rankColor = (int)GameAchiveNum.GetRankColor(diff);
		_achieveNumMain.SetColor(CommonScriptable.GetColorSetting().GameGaugeNumColor[rankColor]);
		_achieveNumDenomiMain.SetColor(CommonScriptable.GetColorSetting().GameGaugeNumColor[rankColor]);
	}

	public void ViewUpdate()
	{
		MusicNameText.ViewUpdate();
	}

	public void ResetInfomation()
	{
		musicJacket.texture = null;
		MusicNameText.SetData("");
		_difficultySingle.ChangeText("0 ", "0 ");
		_difficultyDouble.ChangeText("00 ", "#0 ");
		SetAchievement(0u);
		SetRank(1u);
		SetSync(0u);
		SetDiff(0);
		SetVisibles(isSingle: true);
	}
}
