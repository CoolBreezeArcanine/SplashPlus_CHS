using System;
using DB;
using MAI2;

namespace PartyLink
{
	public static class DeliveryChecker
	{
		public class InitParam
		{
			public ushort _portNumber;

			public int _sendIntervalSec;

			public int _checkCount;

			public InitParam()
			{
				_portNumber = CPortNumber;
				_sendIntervalSec = c_sendIntervalSec;
				_checkCount = c_checkCount;
			}

			public InitParam(InitParam arg)
			{
				_portNumber = arg._portNumber;
				_sendIntervalSec = arg._sendIntervalSec;
				_checkCount = arg._checkCount;
			}
		}

		public interface IManager
		{
			void initialize();

			void terminate();

			void execute();

			void start(bool isServer);

			bool isError();

			PartyDeliveryCheckerErrorID getErrorID();

			bool isOk();

			void info(ref string os);
		}

		private class Manager : StateMachine<Manager, PartyDeliveryCheckerStateID>, IManager, IDisposable
		{
			private InitParam _initParam;

			private bool _isServer;

			private PartyDeliveryCheckerErrorID _errorID;

			private BroadcastSocket _broadcastSocket;

			private UdpRecvSocket _udpSocket;

			private int _recvDifferentAddressCount;

			private int _recvSameAddressCount;

			private int _sendCount;

			private DateTime _recvTime;

			private DateTime _sendTime;

			private bool _alreadyDisposed;

			private const int MockID = -1;

			public Manager(InitParam param)
				: this(param, 0)
			{
			}

			public Manager(InitParam param, int mockID)
			{
				_broadcastSocket = new BroadcastSocket("DeliveryChecker::BroadCast", -1);
				_udpSocket = new UdpRecvSocket("DeliveryChecker::UDP", -1);
				_recvDifferentAddressCount = 0;
				_recvSameAddressCount = 0;
				_sendCount = 0;
				_recvTime = DateTime.MinValue;
				_sendTime = DateTime.MinValue;
				_initParam = new InitParam(param);
				_isServer = false;
				_errorID = PartyDeliveryCheckerErrorID.None;
				clear();
				setCurrentStateID(PartyDeliveryCheckerStateID.First);
				_udpSocket.setCheckVersion(isCheckVersion: false);
				_udpSocket.registCommand(Command.AdvocateDelivery, recvDeliveryChecker);
			}

			~Manager()
			{
				dispose(disposing: false);
			}

			public void Dispose()
			{
				dispose(disposing: true);
			}

			protected virtual void dispose(bool disposing)
			{
				if (!_alreadyDisposed)
				{
					if (disposing)
					{
						_initParam = null;
						Util.SafeDispose(ref _broadcastSocket);
						Util.SafeDispose(ref _udpSocket);
						GC.SuppressFinalize(this);
					}
					_alreadyDisposed = true;
				}
			}

			public void initialize()
			{
				clear();
				setCurrentStateID(PartyDeliveryCheckerStateID.First);
			}

			public void terminate()
			{
				setCurrentStateID(PartyDeliveryCheckerStateID.Finish);
			}

			public void execute()
			{
				openSocket();
				_udpSocket.recv();
				if (_isServer)
				{
					updateServer();
				}
				updateState(-1f);
			}

			public void start(bool isServer)
			{
				clear();
				_isServer = isServer;
				if (_isServer)
				{
					setCurrentStateID(PartyDeliveryCheckerStateID.ServerCheck);
				}
				else
				{
					setCurrentStateID(PartyDeliveryCheckerStateID.ClientCheck);
				}
			}

			public bool isError()
			{
				return getCurrentStateID() == PartyDeliveryCheckerStateID.Error;
			}

			public bool isOk()
			{
				bool flag = false;
				if (_isServer)
				{
					return getCurrentStateID() == PartyDeliveryCheckerStateID.ServerActive;
				}
				return getCurrentStateID() == PartyDeliveryCheckerStateID.ClientActive;
			}

			public PartyDeliveryCheckerErrorID getErrorID()
			{
				return _errorID;
			}

			public void info(ref string os)
			{
				os = os + " isError " + isError() + "\n";
				os = os + " isOk " + isOk() + "\n";
				os = os + " isSendInterval " + isSendInterval() + "\n";
				os = os + " getErrorID " + getErrorID().GetName() + "\n";
				os += "\n";
				os = os + " _isServer " + _isServer + "\n";
				os = os + " state " + getCurrentStateID().GetName() + "\n";
				os = os + " error " + _errorID.GetName() + "\n";
				os = os + " _recvDifferentAddressCount " + _recvDifferentAddressCount + "\n";
				os = os + " _recvSameAddressCount " + _recvSameAddressCount + "\n";
				os = os + " _sendCount " + _sendCount + "\n";
				os = string.Concat(os, " _recvTime ", _recvTime, "\n");
				os = string.Concat(os, " _sendTime ", _sendTime, "\n");
				os = os + " _initParam  portNumber " + _initParam._portNumber + " sendIntervalSec " + _initParam._sendIntervalSec + " checkCount " + _initParam._checkCount + "\n";
				os += "\n";
				os = string.Concat(os, " _broadcastSocket ", _broadcastSocket, "\n");
				os = string.Concat(os, " _udpSocket ", _udpSocket, "\n");
			}

			public void Execute_First()
			{
			}

			public void Enter_ServerCheck()
			{
				_sendCount = 0;
			}

			public void Execute_ServerCheck()
			{
				if (_initParam._checkCount <= _sendCount)
				{
					setCurrentStateID(PartyDeliveryCheckerStateID.ServerActive);
				}
			}

			public void Execute_ServerActive()
			{
			}

			public void Execute_ClientCheck()
			{
			}

			public void Execute_ClientActive()
			{
			}

			public void Execute_Error()
			{
			}

			public void Execute_Finish()
			{
			}

			public void Execute_End()
			{
			}

			public void Execute_Begin()
			{
			}

			public void Execute_Invalid()
			{
			}

			private void setCurrentStateID(PartyDeliveryCheckerStateID nextState)
			{
				setNextState(nextState);
				updateState(-1f);
			}

			private PartyDeliveryCheckerStateID getCurrentStateID()
			{
				return getCurrentState();
			}

			private void recvDeliveryChecker(Packet packet)
			{
				_recvTime = DateTime.Now;
				AdvocateDelivery param = packet.getParam<AdvocateDelivery>(isCheckVersion: false);
				IpAddress ipAddress = new IpAddress(param._address);
				IpAddress ipAddress2 = new IpAddress(Util.MyIpAddress(-1));
				if (ipAddress != ipAddress2)
				{
					_recvDifferentAddressCount++;
				}
				else
				{
					_recvSameAddressCount++;
				}
				switch (getCurrentStateID())
				{
				case PartyDeliveryCheckerStateID.ServerCheck:
				case PartyDeliveryCheckerStateID.ServerActive:
					if (ipAddress != ipAddress2)
					{
						error(PartyDeliveryCheckerErrorID.Duplicate);
					}
					break;
				case PartyDeliveryCheckerStateID.ClientCheck:
					setCurrentStateID(PartyDeliveryCheckerStateID.ClientActive);
					break;
				}
			}

			private void error(PartyDeliveryCheckerErrorID errorID)
			{
				_errorID = errorID;
				setCurrentStateID(PartyDeliveryCheckerStateID.Error);
			}

			private void clear()
			{
				_recvDifferentAddressCount = 0;
				_recvSameAddressCount = 0;
				_sendCount = 0;
				_errorID = PartyDeliveryCheckerErrorID.None;
				_broadcastSocket.close();
				_udpSocket.close();
			}

			private void sendDeliveryCheckCommand()
			{
				if (_broadcastSocket.isValid())
				{
					AdvocateDelivery advocateDelivery = new AdvocateDelivery(new IpAddress(Util.MyIpAddress(-1)));
					_broadcastSocket.sendClass(advocateDelivery);
					_sendTime = DateTime.Now;
					_sendCount++;
				}
			}

			private void openSocket()
			{
				if (!_broadcastSocket.isValid())
				{
					IpAddress addrTo = new IpAddress(Util.BroadcastAddress());
					_broadcastSocket.open(addrTo, _initParam._portNumber);
				}
				if (!_udpSocket.isValid())
				{
					IpAddress ipAddress = new IpAddress(Util.MyIpAddress(-1));
					if (ipAddress != IpAddress.Zero)
					{
						_udpSocket.open(ipAddress, _initParam._portNumber);
					}
				}
			}

			private void updateServer()
			{
				if (isSendInterval())
				{
					sendDeliveryCheckCommand();
				}
			}

			private bool isSendInterval()
			{
				DateTime now = DateTime.Now;
				if (_sendTime.AddSeconds(_initParam._sendIntervalSec) < now)
				{
					return true;
				}
				return false;
			}
		}

		public static readonly ushort CPortNumber = 50103;

		public static readonly int c_sendIntervalSec = 2;

		public static readonly int c_checkCount = 4;

		private static Manager s_pMgr = null;

		public static void createManager(InitParam param)
		{
			Util.SafeDispose(ref s_pMgr);
			s_pMgr = new Manager(param);
		}

		public static void destroyManager()
		{
			Util.SafeDispose(ref s_pMgr);
		}

		public static IManager get()
		{
			return s_pMgr;
		}

		public static bool isExist()
		{
			if (s_pMgr != null)
			{
				return true;
			}
			return false;
		}
	}
}
