using DB;
using Monitor.MusicSelect.OtherParts;
using Monitor.MusicSelect.UI;
using TMPro;
using UnityEngine;

namespace Monitor
{
	public class ExtraInfoController : MonoBehaviour
	{
		public enum EnumActive
		{
			Active,
			NonActive,
			End
		}

		public enum EnumChallengeInfoSeq
		{
			Music,
			Difficulty,
			None
		}

		[SerializeField]
		[Header("オリジナルゴーストデータ")]
		private GhostBattleInformarionObject _originalGhostObj;

		[SerializeField]
		[Header("ボスゴーストデータ")]
		private GhostBattleInformarionObject _originalBossObj;

		[SerializeField]
		[Header("チャレンジデータ(曲セレ)")]
		private ChallengeRule _originalChallengeMusicObj;

		[SerializeField]
		[Header("チャレンジデータ(難易度セレ)")]
		private ChallengeRule _originalChallengeDifficultyObj;

		[SerializeField]
		[Header("オトモダチジャンプ表示")]
		private GhostJumpInfo _originalGhostJumpInfo;

		[SerializeField]
		[Header("スコアタルール表示")]
		private ScoreAttackRuleInfo _originalScoreAttackRuleInfo;

		[SerializeField]
		[Header("ゴーストデータのnullポインタ座標")]
		private Transform[] _ghostPosition;

		[SerializeField]
		[Header("ボスデータのnullポインタ座標")]
		private Transform[] _bossPosition;

		[SerializeField]
		[Header("チャレンジ(曲セレ)のnullポインタ座標")]
		private Transform _challengeMusicPosition;

		[SerializeField]
		[Header("チャレンジ(難易度セレ)のnullポインタ座標")]
		private Transform _challengeDifficultyPosition;

		[SerializeField]
		[Header("オトモダチジャンプのnullポインタ座標")]
		private Transform _ghostJumpInfoPosition;

		[SerializeField]
		[Header("スコアタルールのnullポインタ座標")]
		private Transform _scoreAttackRuleInfoPosition;

		[SerializeField]
		[Header("アクティブ非アクティブのオブジェ")]
		private GameObject[] _activeObj;

		[SerializeField]
		[Header("アクティブ非アクティブのボスオブジェ")]
		private GameObject[] _activeBossObj;

		[SerializeField]
		[Header("日付テキスト")]
		private TextMeshProUGUI _playText;

		[SerializeField]
		[Header("ボステキスト")]
		private TextMeshProUGUI _bossText;

		private GhostBattleInformarionObject[] _ghostObj;

		private GhostBattleInformarionObject[] _bossObj;

		private ChallengeRule _challengeInfoMusicObj;

		private ChallengeRule _challengeInfoDifficultyObj;

		private GhostJumpInfo _ghostJumpInfo;

		private ScoreAttackRuleInfo _scoreAttackRuleInfo;

		public GhostBattleInformarionObject[] GhostObj => _ghostObj;

		public GhostBattleInformarionObject[] BossObj => _bossObj;

		public void Initialize()
		{
			_ghostObj = new GhostBattleInformarionObject[2];
			_bossObj = new GhostBattleInformarionObject[2];
			_challengeInfoMusicObj = new ChallengeRule();
			_challengeInfoDifficultyObj = new ChallengeRule();
			for (int i = 0; i < 2; i++)
			{
				_ghostObj[i] = Object.Instantiate(_originalGhostObj, _ghostPosition[i]);
				_ghostObj[i].gameObject.SetActive(value: false);
				_bossObj[i] = Object.Instantiate(_originalBossObj, _bossPosition[i]);
				_bossObj[i].gameObject.SetActive(value: false);
			}
			_challengeInfoMusicObj = Object.Instantiate(_originalChallengeMusicObj, _challengeMusicPosition);
			_challengeInfoMusicObj.gameObject.SetActive(value: false);
			_challengeInfoDifficultyObj = Object.Instantiate(_originalChallengeDifficultyObj, _challengeDifficultyPosition);
			_challengeInfoDifficultyObj.gameObject.SetActive(value: false);
			_ghostJumpInfo = Object.Instantiate(_originalGhostJumpInfo, _ghostJumpInfoPosition);
			_ghostJumpInfo.Initialize();
			_ghostJumpInfo.gameObject.SetActive(value: false);
			_scoreAttackRuleInfo = Object.Instantiate(_originalScoreAttackRuleInfo, _scoreAttackRuleInfoPosition);
			_scoreAttackRuleInfo.Initialize();
			_scoreAttackRuleInfo.gameObject.SetActive(value: false);
		}

		public void setGhostPlayTime(long unixTime)
		{
			_playText.text = CommonMessageID.MusicSelectGhostInfo.GetName();
		}

		public void SetGhostActive(bool isActive, bool isBoss, bool isSpecialBoss)
		{
			if (isBoss)
			{
				_activeObj[0].SetActive(value: false);
				_activeObj[1].SetActive(value: false);
				string text = CommonMessageID.ExtraInfoBoss.GetName();
				if (isSpecialBoss)
				{
					text = CommonMessageID.ExtraInfoSpecialBoss.GetName();
				}
				_bossText.text = text;
				_activeBossObj[0].SetActive(isActive);
				_activeBossObj[1].SetActive(!isActive);
			}
			else
			{
				_activeObj[0].SetActive(isActive);
				_activeObj[1].SetActive(!isActive);
				_activeBossObj[0].SetActive(value: false);
				_activeBossObj[1].SetActive(value: false);
			}
			_challengeInfoMusicObj.gameObject.SetActive(value: false);
			_challengeInfoDifficultyObj.gameObject.SetActive(value: false);
			_scoreAttackRuleInfo.gameObject.SetActive(value: false);
		}

		public void SetChallengeParam(int restLife, int difficulty, int relaxDay, bool infoEnable)
		{
			_challengeInfoMusicObj.SetChallengeRule(restLife, difficulty, relaxDay, infoEnable);
			_challengeInfoDifficultyObj.SetChallengeRule(restLife, difficulty, relaxDay, infoEnable);
		}

		public void SetChallengeView(EnumChallengeInfoSeq seq)
		{
			switch (seq)
			{
			case EnumChallengeInfoSeq.Music:
				_challengeInfoMusicObj.gameObject.SetActive(value: true);
				_challengeInfoDifficultyObj.gameObject.SetActive(value: false);
				break;
			case EnumChallengeInfoSeq.Difficulty:
				_challengeInfoMusicObj.gameObject.SetActive(value: false);
				_challengeInfoDifficultyObj.gameObject.SetActive(value: true);
				break;
			case EnumChallengeInfoSeq.None:
				_challengeInfoMusicObj.gameObject.SetActive(value: false);
				_challengeInfoDifficultyObj.gameObject.SetActive(value: false);
				break;
			}
			_activeObj[0].SetActive(value: false);
			_activeObj[1].SetActive(value: false);
			_activeBossObj[0].SetActive(value: false);
			_activeBossObj[1].SetActive(value: false);
		}

		public void SetGhostJump(bool isActive, bool isReturn)
		{
			_ghostJumpInfo.gameObject.SetActive(isActive);
			if (isActive)
			{
				_ghostJumpInfo.SetAnim(GhostJumpInfo.Anim.In, isReturn);
			}
		}

		public void SetScoreAttackRuleInfo(bool isActive)
		{
			_scoreAttackRuleInfo.gameObject.SetActive(isActive);
		}

		public void SetAllHide()
		{
			_activeObj[0].SetActive(value: false);
			_activeObj[1].SetActive(value: false);
			_activeBossObj[0].SetActive(value: false);
			_activeBossObj[1].SetActive(value: false);
			_challengeInfoMusicObj.gameObject.SetActive(value: false);
			_challengeInfoDifficultyObj.gameObject.SetActive(value: false);
			_ghostJumpInfo.gameObject.SetActive(value: false);
			_scoreAttackRuleInfo.gameObject.SetActive(value: false);
		}
	}
}
