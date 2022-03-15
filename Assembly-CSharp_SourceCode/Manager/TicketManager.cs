using System.Collections.Generic;
using System.Linq;
using MAI2.Util;
using Manager.MaiStudio;
using Manager.UserDatas;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using Net.VO.Mai2;

namespace Manager
{
	public class TicketManager : Singleton<TicketManager>
	{
		private const int FREEDOM_GET_TICKET_ID = 11001;

		private const int COURSE_GET_TICKET_ID = 11001;

		private TicketConnectData[] _connectTicketData;

		private int[] _useTicketId;

		private int[] _boughtTicketId;

		public TicketManager()
		{
			_connectTicketData = new TicketConnectData[2];
			_useTicketId = new int[2];
			_boughtTicketId = new int[2];
			for (int i = 0; i < _connectTicketData.Length; i++)
			{
				_connectTicketData[i] = new TicketConnectData();
			}
			Initialize();
		}

		public void Initialize()
		{
			for (int i = 0; i < _connectTicketData.Length; i++)
			{
				_connectTicketData[i].isConnectFinish = false;
				_connectTicketData[i].isError = false;
				_connectTicketData[i].connectTicketId = -1;
				_connectTicketData[i].credit = -1;
				_useTicketId[i] = -1;
				_boughtTicketId[i] = -1;
			}
		}

		public List<TicketSelectData> GetTicketData(int playerId)
		{
			List<TicketSelectData> list = new List<TicketSelectData>();
			GameCharge[] gameChargeDataList = Singleton<OperationManager>.Instance.GetGameChargeDataList();
			foreach (TicketData item in from t in Singleton<DataManager>.Instance.GetTickets()
				where Singleton<EventManager>.Instance.IsOpenEvent(t.Value.ticketEvent.id)
				select t.Value into x
				orderby x.priority
				select x)
			{
				int ticketId = item.GetID();
				TicketSelectData ticketSelectData = new TicketSelectData();
				if (item.isMoney)
				{
					bool flag = false;
					int creditNum = 0;
					GameCharge[] array = gameChargeDataList;
					for (int i = 0; i < array.Length; i++)
					{
						GameCharge gameCharge = array[i];
						if (ticketId == gameCharge.chargeId)
						{
							flag = true;
							creditNum = gameCharge.price;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
					ticketSelectData.ticketID = ticketId;
					ticketSelectData.creditNum = creditNum;
				}
				else
				{
					ticketSelectData.ticketID = ticketId;
					ticketSelectData.creditNum = item.creditNum;
				}
				ticketSelectData.ticketName = item.detail;
				ticketSelectData.ticketFileName = "Common/Sprites/Ticket/" + item.filename;
				ticketSelectData.ticketFileName_S = "Common/Sprites/Ticket/" + item.filename + "_S";
				ticketSelectData.maxTiceketNum = item.maxCount;
				ticketSelectData.areaPercent = item.areaPercent;
				ticketSelectData.charaMagnification = item.charaMagnification;
				ticketSelectData.ticketKind = (TicketKind)item.ticketKind.id;
				Manager.UserDatas.UserItem userItem = Singleton<UserDataManager>.Instance.GetUserData(playerId).TicketList.Find((Manager.UserDatas.UserItem t) => t.itemId == ticketId);
				if (userItem != null)
				{
					ticketSelectData.ticketNum = userItem.stock;
				}
				else
				{
					ticketSelectData.ticketNum = 0;
				}
				ticketSelectData.expirationUnixTime = Singleton<EventManager>.Instance.GetEventData(item.ticketEvent.id).EndUnixTime;
				if (ticketSelectData.maxTiceketNum == 0 || ticketSelectData.ticketNum != 0)
				{
					list.Add(ticketSelectData);
				}
			}
			return list;
		}

		public int GetUserChargeId(int playerId)
		{
			foreach (Manager.UserDatas.UserCharge charge in Singleton<UserDataManager>.Instance.GetUserData(playerId).ChargeList)
			{
				if (charge.stock > 0)
				{
					_useTicketId[playerId] = charge.chargeId;
					charge.stock--;
					return charge.chargeId;
				}
			}
			return -1;
		}

		public bool UseTicketPrepare(int playerId, int ticketId)
		{
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(ticketId);
			if (ticket != null)
			{
				if (ticket.ticketKind.id == 1)
				{
					GameCharge[] gameChargeDataList = Singleton<OperationManager>.Instance.GetGameChargeDataList();
					for (int i = 0; i < gameChargeDataList.Length; i++)
					{
						GameCharge gameCharge = gameChargeDataList[i];
						if (gameCharge.chargeId == ticketId)
						{
							_connectTicketData[playerId].isConnectFinish = false;
							_connectTicketData[playerId].connectTicketId = ticketId;
							_connectTicketData[playerId].credit = gameCharge.price;
							SetBuyPaidTicketData(playerId, ticketId);
							RequestUserCharge(playerId);
							return true;
						}
					}
				}
				else
				{
					if (ticket.ticketKind.id != 3 && ticket.ticketKind.id != 2)
					{
						_connectTicketData[playerId].isConnectFinish = true;
						_connectTicketData[playerId].connectTicketId = ticketId;
						_connectTicketData[playerId].credit = 0;
						return true;
					}
					Manager.UserDatas.UserItem userItem = Singleton<UserDataManager>.Instance.GetUserData(playerId).TicketList.Find((Manager.UserDatas.UserItem t) => t.itemId == ticketId);
					if (userItem != null && userItem.stock > 0)
					{
						_connectTicketData[playerId].isConnectFinish = true;
						_connectTicketData[playerId].connectTicketId = ticketId;
						_connectTicketData[playerId].credit = 0;
						userItem.stock--;
						return true;
					}
				}
			}
			return false;
		}

		public bool IsNoRequestTicket()
		{
			TicketConnectData[] connectTicketData = _connectTicketData;
			foreach (TicketConnectData ticketConnectData in connectTicketData)
			{
				if (ticketConnectData.connectTicketId != -1 && !ticketConnectData.isConnectFinish)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsFinishServerTicket(int playerId, ref int credit, ref bool isError)
		{
			isError = false;
			credit = -1;
			if (_connectTicketData[playerId].connectTicketId == -1)
			{
				return false;
			}
			if (_connectTicketData[playerId].isConnectFinish)
			{
				credit = _connectTicketData[playerId].credit;
				_useTicketId[playerId] = _connectTicketData[playerId].connectTicketId;
				_boughtTicketId[playerId] = _connectTicketData[playerId].connectTicketId;
				isError = _connectTicketData[playerId].isError;
			}
			return _connectTicketData[playerId].isConnectFinish;
		}

		private void SetBuyPaidTicketData(int playerId, int ticketId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			bool flag = false;
			foreach (Manager.UserDatas.UserCharge charge in userData.ChargeList)
			{
				if (charge.chargeId == ticketId)
				{
					charge.stock++;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				int stockNum = 1;
				string nowDateString = TimeManager.GetNowDateString();
				string dateString = TimeManager.GetDateString(TimeManager.GetNowUnixTime() + 31536000);
				Manager.UserDatas.UserCharge item = new Manager.UserDatas.UserCharge(ticketId, stockNum, nowDateString, dateString);
				userData.ChargeList.Add(item);
			}
		}

		private void RequestUserCharge(int playerId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			ulong userID = userData.Detail.UserID;
			int connectTicketId = _connectTicketData[playerId].connectTicketId;
			PacketHelper.StartPacket(new PacketUploadUserChargelog(userID, userData.ExportEnableUserCharge(connectTicketId), userData.ExportUserChargelog(connectTicketId), delegate(int code)
			{
				if (code == 1)
				{
					RequestSuccess(playerId);
				}
				else
				{
					RequestFail(playerId);
				}
			}, delegate
			{
				RequestFail(playerId);
			}));
		}

		private void RequestSuccess(int playerId)
		{
			_connectTicketData[playerId].isConnectFinish = true;
			foreach (Manager.UserDatas.UserCharge charge in Singleton<UserDataManager>.Instance.GetUserData(playerId).ChargeList)
			{
				if (charge.chargeId == _connectTicketData[playerId].connectTicketId)
				{
					if (charge.stock > 0)
					{
						charge.stock--;
					}
					else
					{
						_connectTicketData[playerId].isError = true;
					}
					break;
				}
			}
		}

		private void RequestFail(int playerId)
		{
			_connectTicketData[playerId].isConnectFinish = true;
			_connectTicketData[playerId].isError = true;
			foreach (Manager.UserDatas.UserCharge charge in Singleton<UserDataManager>.Instance.GetUserData(playerId).ChargeList)
			{
				if (charge.chargeId == _connectTicketData[playerId].connectTicketId && charge.stock > 0)
				{
					charge.stock--;
				}
			}
		}

		public string GetTicketName(int ticketId)
		{
			string result = "";
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(ticketId);
			if (ticket != null)
			{
				result = ticket.detail;
			}
			return result;
		}

		public int GetFreedomPlayTicketId(int playerId)
		{
			Manager.UserDatas.UserItem userItem = Singleton<UserDataManager>.Instance.GetUserData(playerId).TicketList.Find((Manager.UserDatas.UserItem t) => t.itemId == 11001);
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(11001);
			if (ticket != null)
			{
				if (userItem != null)
				{
					if (userItem.stock < ticket.maxCount)
					{
						return 11001;
					}
				}
				else if (ticket.maxCount != 0)
				{
					return 11001;
				}
			}
			return -1;
		}

		public void SaveFreedomTicketGet(int playerId)
		{
			Singleton<UserDataManager>.Instance.GetUserData(playerId).AddCollections(UserData.Collection.Ticket, 11001, addNewGet: true);
		}

		public void checkAndSaveEventTicketGet(int playerId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			long unixTime = TimeManager.GetUnixTime(userData.Detail.PreLastLoginDate);
			foreach (TicketData item in from t in Singleton<DataManager>.Instance.GetTickets()
				where Singleton<EventManager>.Instance.IsOpenEvent(t.Value.ticketEvent.id)
				select t.Value into x
				orderby x.priority
				select x)
			{
				long eventStartUnixTime = Singleton<EventManager>.Instance.GetEventStartUnixTime(item.ticketEvent.id);
				if (unixTime < eventStartUnixTime)
				{
					userData.AddCollections(UserData.Collection.Ticket, item.GetID(), addNewGet: true);
				}
			}
		}

		public int GetCoursePlayTicketId(int playerId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			if (userData.IsGuest())
			{
				return -1;
			}
			string logDateString = TimeManager.GetLogDateString(TimeManager.PlayBaseTime);
			if (TimeManager.GetUnixTime(string.IsNullOrEmpty(userData.Detail.DailyCourseBonusDate) ? TimeManager.GetLogDateString(0L) : userData.Detail.DailyCourseBonusDate) < TimeManager.GetUnixTime(logDateString))
			{
				Manager.UserDatas.UserItem userItem = userData.TicketList.Find((Manager.UserDatas.UserItem t) => t.itemId == 11001);
				TicketData ticket = Singleton<DataManager>.Instance.GetTicket(11001);
				if (ticket != null)
				{
					if (userItem != null)
					{
						if (userItem.stock < ticket.maxCount)
						{
							return 11001;
						}
					}
					else if (ticket.maxCount != 0)
					{
						return 11001;
					}
				}
			}
			return -1;
		}

		public void SaveCourseTicketGet(int playerId)
		{
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(playerId);
			string logDateString = TimeManager.GetLogDateString(TimeManager.PlayBaseTime);
			if (TimeManager.GetUnixTime(string.IsNullOrEmpty(userData.Detail.DailyCourseBonusDate) ? TimeManager.GetLogDateString(0L) : userData.Detail.DailyCourseBonusDate) < TimeManager.GetUnixTime(logDateString))
			{
				userData.Detail.DailyCourseBonusDate = logDateString;
				userData.AddCollections(UserData.Collection.Ticket, 11001, addNewGet: true);
			}
		}

		public int GetBoughtTicketId(int playerId)
		{
			return _boughtTicketId[playerId];
		}

		public int GetUseTicketId(int playerId)
		{
			return _useTicketId[playerId];
		}

		public int GetTicketCredit(int ticketId)
		{
			int result = 0;
			GameCharge[] gameChargeDataList = Singleton<OperationManager>.Instance.GetGameChargeDataList();
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(ticketId);
			if (ticket != null)
			{
				if (ticket.isMoney)
				{
					GameCharge[] array = gameChargeDataList;
					for (int i = 0; i < array.Length; i++)
					{
						GameCharge gameCharge = array[i];
						if (ticketId == gameCharge.chargeId)
						{
							result = gameCharge.price;
							break;
						}
					}
				}
				else
				{
					result = 0;
				}
			}
			return result;
		}

		public string GetGetWindowFileName(int ticketId)
		{
			string result = "";
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(ticketId);
			if (ticket != null)
			{
				result = "Common/Sprites/Ticket/" + ticket.filename;
			}
			return result;
		}

		public string GetCollectionFileName(int ticketId)
		{
			string result = "";
			TicketData ticket = Singleton<DataManager>.Instance.GetTicket(ticketId);
			if (ticket != null)
			{
				result = "Common/Sprites/Ticket/" + ticket.filename + "_S";
			}
			return result;
		}
	}
}
