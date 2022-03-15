using System;
using MAI2.Util;
using Manager;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor.Game
{
	public class GameUpperGaugeCtrl : MonoBehaviour
	{
		private enum GaugeTextColor
		{
			VsWin,
			VsLose,
			Achive
		}

		[SerializeField]
		private GameObject _gaugeImageObj;

		[SerializeField]
		private Image _gaugeColorObj;

		[SerializeField]
		private TextMeshProUGUI _achiveText;

		[SerializeField]
		private MultipleImage _gaugeRankObj;

		[SerializeField]
		private MultipleImage _gaugeVsObj;

		private int _preData;

		private int _monitorIndex;

		private const float MaskRotatePer = -17.4f;

		public void Initialize(int monIndex)
		{
			_monitorIndex = monIndex;
			_preData = -1;
			if (Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() == 1)
			{
				_gaugeRankObj.gameObject.SetActive(value: true);
				_gaugeVsObj.gameObject.SetActive(value: false);
			}
			else
			{
				_gaugeRankObj.gameObject.SetActive(value: false);
				_gaugeVsObj.gameObject.SetActive(value: true);
			}
		}

		public void Execute()
		{
			int achivement = (int)Singleton<GamePlayManager>.Instance.GetAchivement(_monitorIndex);
			MusicClearrankID clearRank = GameManager.GetClearRank(achivement);
			if (Singleton<GamePlayManager>.Instance.GetPlayerIgnoreNpcNum() == 1)
			{
				if (achivement != _preData)
				{
					_gaugeRankObj.ChangeSprite((int)clearRank);
				}
			}
			else
			{
				uint vsRank = Singleton<GamePlayManager>.Instance.GetVsRank(_monitorIndex);
				_gaugeVsObj.ChangeSprite((int)vsRank);
			}
			if (Singleton<GamePlayManager>.Instance.IsGhostFlag())
			{
				int num = Singleton<GamePlayManager>.Instance.GetGhostVsDiff(_monitorIndex);
				if (num != _preData)
				{
					if (num >= 0)
					{
						string text = Convert.ToString(num).PadLeft(7, '0');
						_achiveText.text = string.Format("+{0,3}.{1}%", (num / 10000).ToString(), text.Substring(3, 4));
						_achiveText.color = CommonScriptable.GetColorSetting().GameGaugeNumColor[0];
					}
					else
					{
						num = Math.Abs(num);
						string text2 = Convert.ToString(num).PadLeft(7, '0');
						_achiveText.text = string.Format("-{0,3}.{1}%", (num / 10000).ToString(), text2.Substring(3, 4));
						_achiveText.color = CommonScriptable.GetColorSetting().GameGaugeNumColor[1];
					}
					_preData = num;
				}
			}
			else if (achivement != _preData)
			{
				string text3 = Convert.ToString(achivement).PadLeft(7, '0');
				_achiveText.text = string.Format("{0,3}.{1}%", (achivement / 10000).ToString(), text3.Substring(3, 4));
				_achiveText.color = CommonScriptable.GetColorSetting().GameGaugeNumColor[2];
				_preData = achivement;
			}
			float num2 = -17.4f * ((float)achivement / 1000000f);
			if (float.IsNaN(Math.Abs(num2)) || float.IsInfinity(Math.Abs(num2)))
			{
				num2 = -0.01f;
			}
			_gaugeImageObj.transform.localRotation = Quaternion.AngleAxis(num2, Vector3.forward);
			switch (clearRank)
			{
			case MusicClearrankID.Rank_D:
				_gaugeColorObj.color = CommonScriptable.GetColorSetting().GameGaugeColor[0];
				break;
			case MusicClearrankID.Rank_C:
				_gaugeColorObj.color = CommonScriptable.GetColorSetting().GameGaugeColor[1];
				break;
			case MusicClearrankID.Rank_B:
			case MusicClearrankID.Rank_BB:
			case MusicClearrankID.Rank_BBB:
				_gaugeColorObj.color = CommonScriptable.GetColorSetting().GameGaugeColor[2];
				break;
			case MusicClearrankID.Rank_A:
			case MusicClearrankID.Rank_AA:
			case MusicClearrankID.Rank_AAA:
				_gaugeColorObj.color = CommonScriptable.GetColorSetting().GameGaugeColor[3];
				break;
			case MusicClearrankID.Rank_S:
			case MusicClearrankID.Rank_SP:
			case MusicClearrankID.Rank_SS:
			case MusicClearrankID.Rank_SSP:
				_gaugeColorObj.color = CommonScriptable.GetColorSetting().GameGaugeColor[4];
				break;
			case MusicClearrankID.Rank_SSS:
			case MusicClearrankID.Rank_SSSP:
				_gaugeColorObj.color = CommonScriptable.GetColorSetting().GameGaugeColor[5];
				break;
			}
		}
	}
}
