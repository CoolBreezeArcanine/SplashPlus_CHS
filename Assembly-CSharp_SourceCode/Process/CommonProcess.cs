using DB;
using Manager;
using Monitor;
using Monitor.Common;
using UnityEngine;

namespace Process
{
	public class CommonProcess : ProcessBase
	{
		public enum CommonSequence
		{
			Wait,
			Logo,
			Title,
			FadeOut,
			Release
		}

		public const ushort MessageID_Reset = 20000;

		public const ushort MessageID_Music_SetActive = 20001;

		public const ushort MessageID_Music_SetData = 20002;

		public const ushort MessageID_Music_SetGameData = 20003;

		public const ushort MessageID_User_SetActive = 20010;

		public const ushort MessageID_User_SetData = 20011;

		public const ushort MessageID_User_SetTeamData = 20012;

		public const ushort MessageID_Chara_SetActive = 20020;

		public const ushort MessageID_Chara_SetSlot = 20021;

		public const ushort MessageID_Chara_SetTeamSlot = 20022;

		public const ushort MessageID_Chara_ResetSlot = 20023;

		public const ushort MessageID_Track_SetActive = 20030;

		public const ushort MessageID_Track_SetTrackNum = 20031;

		public const ushort MessageID_GameMode_Set = 20050;

		public const ushort MessageID_CreditMain = 30000;

		public const ushort MessageID_CreditSub = 30001;

		public const ushort MessageID_ReplaceUserIcon = 30002;

		public const ushort MessageID_ReplaceUserTitle = 30003;

		public const ushort MessageID_ReplaceUserNamePlate = 30004;

		public const ushort MessageID_ReplacePass = 30005;

		public const ushort MessageID_ReplaceRateDisp = 30006;

		public const ushort MessageID_ReplaceRating = 30007;

		public const ushort MessageID_ReplaceAppeal = 30008;

		public const ushort MessageID_ReplaceUserFrame = 30009;

		public const ushort MessageID_ReplaceRank = 30010;

		public const ushort MessageID_RequestOperationInformation = 40000;

		public const ushort MessageID_RequestOperationInformationFlags = 40001;

		public const ushort MessageID_ActivateMaintenanceInformationFlags = 45001;

		public const ushort MessageID_KillMaintenanceInformationFlags = 45002;

		public const ushort MessageID_RequestMainBackIn = 50000;

		public const ushort MessageID_RequestMainBackOut = 50001;

		public const ushort MessageID_RequestMainBackSkyChange = 50002;

		public const ushort MessageID_BackgroundTypeSet = 50003;

		public const ushort MessageID_BackgroundDisp = 50004;

		private CommonSequence state;

		private GameObject _leftInstance;

		private GameObject _rightInstance;

		private CommonMonitor[] _monitors;

		public CommonProcess(ProcessDataContainer dataContainer)
			: base(dataContainer, ProcessType.CommonProcess)
		{
		}

		public override void OnAddProcess()
		{
		}

		public override void OnStart()
		{
			GameObject prefs = Resources.Load<GameObject>("Process/Common/CommonProcess");
			_leftInstance = CreateInstanceAndSetParent(prefs, container.LeftMonitor);
			_rightInstance = CreateInstanceAndSetParent(prefs, container.RightMonitor);
			_monitors = new CommonMonitor[2]
			{
				_leftInstance.GetComponent<CommonMonitor>(),
				_rightInstance.GetComponent<CommonMonitor>()
			};
			for (int i = 0; i < _monitors.Length; i++)
			{
				_monitors[i].Initialize(i, active: true);
			}
		}

		public override void OnUpdate()
		{
			if (state != 0)
			{
				_ = 4;
				return;
			}
			CommonMonitor[] monitors = _monitors;
			for (int i = 0; i < monitors.Length; i++)
			{
				monitors[i].ViewUpdate();
			}
		}

		public override void OnLateUpdate()
		{
		}

		public override void OnRelease()
		{
			Object.Destroy(_leftInstance);
			Object.Destroy(_rightInstance);
		}

		public override object HandleMessage(Message message)
		{
			switch (message.Id)
			{
			case 20000:
			{
				for (int n = 0; n < _monitors.Length; n++)
				{
					_monitors[n].ResetInfomations();
					_monitors[n].UserInformation.gameObject.SetActive(value: false);
					_monitors[n].MusicInfomation.gameObject.SetActive(value: false);
					_monitors[n].CharactorInfomation.gameObject.SetActive(value: false);
					_monitors[n].SetTrackCountVisible(active: false);
				}
				break;
			}
			case 20010:
			{
				int num20 = (int)message.Param[0];
				_monitors[num20].UserInformation.gameObject.SetActive((bool)message.Param[1]);
				break;
			}
			case 20011:
			{
				int num19 = (int)message.Param[0];
				MessageUserInformationData userData = message.Param[1] as MessageUserInformationData;
				_monitors[num19].UserInformation.SetUserData(userData);
				break;
			}
			case 20001:
			{
				int num7 = (int)message.Param[0];
				_monitors[num7].MusicInfomation.gameObject.SetActive((bool)message.Param[1]);
				_monitors[num7].MusicInfomation.SetVisibles((bool)message.Param[2]);
				break;
			}
			case 20002:
			{
				int num6 = (int)message.Param[0];
				MessageMusicData musicData = message.Param[1] as MessageMusicData;
				_monitors[num6].MusicInfomation.SetMusicData(musicData);
				break;
			}
			case 20003:
			{
				int num5 = (int)message.Param[0];
				MessageGamePlayData gameData = message.Param[1] as MessageGamePlayData;
				_monitors[num5].MusicInfomation.SetGameData(gameData);
				break;
			}
			case 20020:
			{
				int num4 = (int)message.Param[0];
				_monitors[num4].CharactorInfomation.gameObject.SetActive((bool)message.Param[1]);
				break;
			}
			case 20021:
			{
				int num3 = (int)message.Param[0];
				MessageCharactorInfomationData characterSlot = message.Param[1] as MessageCharactorInfomationData;
				_monitors[num3].SetCharacterSlot(characterSlot);
				break;
			}
			case 20022:
			{
				int num23 = (int)message.Param[0];
				MessageCharactorInfomationData teamSlotData = message.Param[1] as MessageCharactorInfomationData;
				_monitors[num23].CharactorInfomation.SetTeamSlotData(teamSlotData);
				break;
			}
			case 20023:
			{
				int num22 = (int)message.Param[0];
				_monitors[num22].ResetCharacterSlot((int)message.Param[1]);
				break;
			}
			case 20030:
			{
				int num21 = (int)message.Param[0];
				_monitors[num21].SetTrackCountVisible((bool)message.Param[1]);
				_monitors[num21].SetTrackCount(GameManager.MusicTrackNumber, GameManager.GetMaxTrackCount());
				break;
			}
			case 20031:
			{
				for (int m = 0; m < _monitors.Length; m++)
				{
					_monitors[m].SetTrackCount(GameManager.MusicTrackNumber, GameManager.GetMaxTrackCount());
				}
				break;
			}
			case 30000:
			{
				CommonMonitor[] monitors = _monitors;
				for (int l = 0; l < monitors.Length; l++)
				{
					monitors[l].SetCreditMainMonitor(dispMainMonitor: true);
				}
				break;
			}
			case 30001:
			{
				CommonMonitor[] monitors = _monitors;
				for (int l = 0; l < monitors.Length; l++)
				{
					monitors[l].SetCreditMainMonitor(dispMainMonitor: false);
				}
				break;
			}
			case 30002:
			{
				int num18 = (int)message.Param[0];
				_monitors[num18].SetUserIcon((Texture2D)message.Param[1]);
				break;
			}
			case 30003:
			{
				int num17 = (int)message.Param[0];
				_monitors[num17].SetUserTitle((string)message.Param[1], (Sprite)message.Param[2]);
				break;
			}
			case 30004:
			{
				int num16 = (int)message.Param[0];
				_monitors[num16].SetUserNamePlate((Texture2D)message.Param[1]);
				break;
			}
			case 30005:
			{
				int num15 = (int)message.Param[0];
				_monitors[num15].SetPass((Sprite)message.Param[1]);
				break;
			}
			case 30006:
			{
				int num14 = (int)message.Param[0];
				_monitors[num14].SetUserDispRate((OptionDisprateID)message.Param[1]);
				break;
			}
			case 30007:
			{
				int num13 = (int)message.Param[0];
				_monitors[num13].SetUserDispRating((uint)message.Param[1]);
				break;
			}
			case 30010:
			{
				int num12 = (int)message.Param[0];
				_monitors[num12].SetUserRank((uint)message.Param[1]);
				break;
			}
			case 30008:
			{
				int num11 = (int)message.Param[0];
				_monitors[num11].RequestAppealFrame((OptionAppealID)message.Param[1]);
				break;
			}
			case 30009:
			{
				int num10 = (int)message.Param[0];
				_monitors[num10].SetUserFrame((Texture2D)message.Param[1], (bool)message.Param[2], (Texture2D)message.Param[3], (Texture2D)message.Param[4]);
				break;
			}
			case 40000:
			{
				int num9 = (int)message.Param[0];
				_monitors[num9].RequestOperationInformation((OperationInformationController.InformationType)message.Param[1]);
				break;
			}
			case 40001:
			{
				int num8 = (int)message.Param[0];
				_monitors[num8].RequestOperationInformation((bool)message.Param[1], (bool)message.Param[2], (bool)message.Param[3], (bool)message.Param[4], (bool)message.Param[5]);
				break;
			}
			case 45001:
			{
				bool flag = (bool)message.Param[0];
				CommonMonitor[] monitors = _monitors;
				for (int l = 0; l < monitors.Length; l++)
				{
					monitors[l].ActivateMaintenanceInformation(flag);
				}
				break;
			}
			case 45002:
			{
				CommonMonitor[] monitors = _monitors;
				for (int l = 0; l < monitors.Length; l++)
				{
					monitors[l].KillMaintenanceInformation();
				}
				break;
			}
			case 50000:
			{
				for (int k = 0; k < _monitors.Length; k++)
				{
					_monitors[k].SetMainBackGroundIn();
				}
				break;
			}
			case 50001:
			{
				for (int j = 0; j < _monitors.Length; j++)
				{
					_monitors[j].SetMainBackGroundOut();
				}
				break;
			}
			case 50002:
			{
				for (int i = 0; i < _monitors.Length; i++)
				{
					_monitors[i].SetBackGroundSky((CommonMonitor.SkyDaylight)message.Param[0]);
				}
				break;
			}
			case 50003:
			{
				int num2 = (int)message.Param[0];
				_monitors[num2].SetBackGroundMap((int)message.Param[1]);
				break;
			}
			case 50004:
			{
				int num = (int)message.Param[0];
				_monitors[num].SetBackGroundDisp((bool)message.Param[1]);
				break;
			}
			}
			return null;
		}
	}
}
