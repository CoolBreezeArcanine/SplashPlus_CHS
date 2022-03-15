using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;
using Manager.MaiStudio;
using Monitor.Common;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Monitor
{
	public class CommonMonitor : MonitorBase
	{
		public enum SkyDaylight
		{
			NightToMorning,
			MorningToEvening,
			EveningToNight,
			MorningNow,
			EveningNow,
			NightNow
		}

		private enum TrackColor
		{
			Blue,
			Green,
			Red
		}

		[SerializeField]
		[Header("基本")]
		private NetIconController _netIconController;

		[SerializeField]
		private CreditController _creditContoroller;

		[SerializeField]
		private OperationInformationController _operationInformationController;

		[SerializeField]
		private MaintenanceInformationController _maintenanceInformationController;

		[SerializeField]
		[Header("最背景コントローラ")]
		private MonitorBackgroundTownController _backgroundController;

		[SerializeField]
		private UserInformationController _userInformation;

		[SerializeField]
		private CharactorSlotController _charactorInfomation;

		[SerializeField]
		private MusicInfomationController _musicInfomation;

		[SerializeField]
		[Header("トラック数")]
		private MultipleImage _trackCountObject;

		[SerializeField]
		private SpriteCounter _trackCountText;

		[SerializeField]
		private SpriteCounter _trackDenominatortText;

		[SerializeField]
		private GameObject _trackMaskImage;

		[SerializeField]
		private SpriteCounter _trackFreedomCountText;

		[SerializeField]
		[Header("トラック数字のカラー")]
		private Color[] _trackColor = new Color[11];

		[SerializeField]
		[Header("アピールフレーム")]
		private GameObject _appealObject;

		[SerializeField]
		private Image _appealImage;

		[SerializeField]
		[Header("RomVersion")]
		private TextMeshProUGUI _romVersionText;

		[SerializeField]
		[Header("ビルドバージョン")]
		private TextMeshProUGUI _buildVersionText;

		[SerializeField]
		[Header("UserID")]
		private TextMeshProUGUI _userIdText;

		[SerializeField]
		[Header("大会モード")]
		private GameObject _eventModeObject;

		[SerializeField]
		[Header("Debugビルドか")]
		private GameObject _developmentBuildText;

		private readonly Dictionary<int, CharacterSlotData> _characterSlotData = new Dictionary<int, CharacterSlotData>();

		public UserInformationController UserInformation => _userInformation;

		public MusicInfomationController MusicInfomation => _musicInfomation;

		public CharactorSlotController CharactorInfomation => _charactorInfomation;

		public override void Initialize(int monIndex, bool active)
		{
			base.Initialize(monIndex, active);
			ResetInfomations();
			CreateCharacterSlotData();
			UserInformation.gameObject.SetActive(value: false);
			MusicInfomation.gameObject.SetActive(value: false);
			CharactorInfomation.gameObject.SetActive(value: false);
			_netIconController.Initialize(monIndex, active);
			_creditContoroller.Initialize(monIndex, active);
			_operationInformationController.Initialize();
			_maintenanceInformationController.Initialize();
			_charactorInfomation.Initialize();
			_backgroundController.Initialize();
			SetTrackCountVisible(active: false);
			Vector3 localPosition = _appealObject.transform.localPosition;
			localPosition.x = ((monIndex == 0) ? 1080f : (-1080f));
			_appealObject.transform.localPosition = localPosition;
			_romVersionText.text = Singleton<SystemConfig>.Instance.config.displayVersionString;
			_eventModeObject.SetActive(GameManager.IsEventMode);
			new FileInfo(Assembly.GetExecutingAssembly().Location);
			_buildVersionText.text = "";
			_userIdText.text = "";
			_developmentBuildText.SetActive(value: false);
		}

		public override void ViewUpdate()
		{
			_musicInfomation.ViewUpdate();
			_netIconController.ViewUpdate();
			_creditContoroller.ViewUpdate();
			_operationInformationController.ViewUpdate();
			_maintenanceInformationController.ViewUpdate();
			_userInformation.UpdateTextScroll();
		}

		private void CreateCharacterSlotData()
		{
			foreach (KeyValuePair<int, MapData> mapData in Singleton<DataManager>.Instance.GetMapDatas())
			{
				int key = mapData.Key;
				if (!_characterSlotData.ContainsKey(key))
				{
					MapColorData mapColorData = Singleton<DataManager>.Instance.GetMapColorData(mapData.Value.ColorId.id);
					_characterSlotData[key] = new CharacterSlotData(key, mapColorData.Color ?? new Color24(), mapColorData.ColorDark ?? new Color24());
				}
			}
		}

		public void ResetInfomations()
		{
			UserInformation.ResetInformation();
			_musicInfomation.ResetInfomation();
			_charactorInfomation.ResetCharactor();
			SetTrackCount(0u, 0u);
			RequestAppealFrame(OptionAppealID.OFF);
		}

		public void SetTrackCountVisible(bool active)
		{
			_trackCountObject.gameObject.SetActive(active);
		}

		public void SetTrackCount(uint currentTrackNum, uint maxTrackNum)
		{
			TrackColor trackColor = TrackColor.Blue;
			if (!GameManager.IsFreedomMode)
			{
				_trackMaskImage.SetActive(value: false);
				_trackCountText.ChangeText(currentTrackNum.ToString());
				_trackDenominatortText.ChangeText(maxTrackNum.ToString());
				trackColor = (maxTrackNum - currentTrackNum) switch
				{
					0u => TrackColor.Red, 
					1u => TrackColor.Green, 
					_ => TrackColor.Blue, 
				};
			}
			else
			{
				_trackMaskImage.SetActive(value: true);
				_trackFreedomCountText.ChangeText(currentTrackNum.ToString().PadLeft(2));
				trackColor = TrackColor.Red;
			}
			_trackCountObject.ChangeSprite((int)trackColor);
			_trackCountText.SetColor(_trackColor[(int)trackColor]);
			_trackDenominatortText.SetColor(_trackColor[(int)trackColor]);
			_trackFreedomCountText.SetColor(_trackColor[(int)trackColor]);
		}

		public void SetCreditMainMonitor(bool dispMainMonitor)
		{
			_creditContoroller.SwitchMainMon(dispMainMonitor);
		}

		public void SetUserIcon(Texture2D texture)
		{
			_userInformation.SetUserIcon(texture);
		}

		public void SetUserTitle(string title, Sprite titleBg)
		{
			_userInformation.SetTitle(title, titleBg);
		}

		public void SetUserNamePlate(Texture2D texture)
		{
			_userInformation.SetNamePlate(texture);
		}

		public void SetUserDispRate(OptionDisprateID dispRate)
		{
			_userInformation.SetDispRate(dispRate);
		}

		public void SetUserDispRating(uint rating)
		{
			_userInformation.SetUserRating(rating);
		}

		public void SetUserRank(uint danibaseId)
		{
			_userInformation.SetRank(danibaseId);
		}

		public void SetPass(Sprite pass)
		{
			_userInformation.SetPass(pass);
		}

		public void SetUserFrame(Texture2D texture, bool isKira, Texture2D kira_source_tex, Texture2D kira_mask_tex)
		{
			_userInformation.SetFrame(texture, isKira, kira_source_tex, kira_mask_tex);
		}

		public void SetCharacterSlot(MessageCharactorInfomationData data)
		{
			_charactorInfomation.SetSlotData(data, _characterSlotData[data.MapKey]);
		}

		public void ResetCharacterSlot(int index)
		{
			_charactorInfomation.ResetCharacter(index);
		}

		public void RequestOperationInformation(OperationInformationController.InformationType type)
		{
			_operationInformationController.RequestInformation(type);
		}

		public void RequestOperationInformation(bool isEnoughCredit, bool isCoinExist, bool isAimeUser, bool isAimeUnavailable, bool isMaintenance)
		{
			bool isCredit = SingletonStateMachine<AmManager, AmManager.EState>.Instance.Credit.IsFreePlay() || isEnoughCredit;
			_operationInformationController.RequestInformation(isCredit, !isCoinExist, isAimeUser, isAimeUnavailable, isMaintenance);
		}

		public void ActivateMaintenanceInformation(bool flag)
		{
			_maintenanceInformationController.Activation(flag);
		}

		public void KillMaintenanceInformation()
		{
			_maintenanceInformationController.ForceKill();
		}

		public void RequestAppealFrame(OptionAppealID appealId)
		{
			if (!Singleton<UserDataManager>.Instance.IsSingleUser())
			{
				appealId = OptionAppealID.OFF;
			}
			if (appealId == OptionAppealID.OFF)
			{
				_appealObject.SetActive(value: false);
				return;
			}
			_appealObject.SetActive(value: true);
			int num = (int)appealId;
			string path = "Process/Common/Sprites/UpperMonitor/Appeal/UI_CMN_Appeal_" + num.ToString("00");
			_appealImage.sprite = Resources.Load<Sprite>(path);
		}

		public void SetMainBackGroundIn()
		{
		}

		public void SetMainBackGroundOut()
		{
		}

		public void SetBackGroundSky(SkyDaylight changeType)
		{
		}

		public void SetBackGroundMap(int mapId)
		{
			_backgroundController.SetType(mapId);
		}

		public void SetBackGroundDisp(bool flag)
		{
			_backgroundController.SetDisp(flag);
		}

		public void SetDebugUserID()
		{
			_userIdText.text = "<color=#008080>UserID:" + Singleton<UserDataManager>.Instance.GetUserData(monitorIndex).Detail.UserID + "</color>";
		}
	}
}
