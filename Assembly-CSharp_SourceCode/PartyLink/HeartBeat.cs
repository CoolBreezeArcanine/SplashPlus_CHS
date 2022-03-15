using System;
using DB;

namespace PartyLink
{
	public class HeartBeat
	{
		private int _recvNumber;

		private int _sendNumber;

		private int _timeout;

		private int _interval;

		private DateTime _recvTime;

		private DateTime _sendTime;

		private readonly ConnectSocket _socket;

		private PartyHeartBeatStateID _state;

		private PartyTick _tick;

		private long _sendUsec;

		private long _responseUsec;

		private long _diffUsec;

		private long _sendFrame;

		private long _responseFrame;

		private long _diffFrame;

		private long _currentFrame;

		public HeartBeat(ConnectSocket socket, int interval, int timeout)
		{
			_socket = socket;
			_recvNumber = 0;
			_sendNumber = 0;
			_timeout = timeout;
			_interval = interval;
			_state = PartyHeartBeatStateID.Active;
			_responseUsec = 0L;
			_tick = new PartyTick();
			_sendUsec = 0L;
			_diffUsec = 0L;
			_sendFrame = 0L;
			_responseFrame = 0L;
			_diffFrame = 0L;
			_currentFrame = 0L;
			_sendTime = (_recvTime = DateTime.Now);
		}

		public void update()
		{
			_currentFrame++;
			DateTime now = DateTime.Now;
			if (!_socket.isActive())
			{
				_recvTime = now;
				_sendTime = now;
				if (_state == PartyHeartBeatStateID.Active)
				{
					_state = PartyHeartBeatStateID.Closed;
				}
				return;
			}
			if (_sendTime.AddSeconds(_interval) <= now)
			{
				_socket.sendClass(new HeartBeatRequest());
				_sendTime = now;
				_sendNumber++;
				_sendUsec = _tick.getUsec();
				_sendFrame = _currentFrame;
			}
			if (_recvTime.AddSeconds(_timeout) < now)
			{
				_state = PartyHeartBeatStateID.Timeout;
				_socket.close();
			}
		}

		public void recvRequest(HeartBeatRequest request)
		{
			DateTime dateTime = (_recvTime = DateTime.Now);
			_recvNumber++;
			_socket.sendClass(new HeartBeatResponse());
		}

		public void recvResponse(HeartBeatResponse response)
		{
			_responseUsec = _tick.getUsec();
			_diffUsec = _responseUsec - _sendUsec;
			_responseFrame = _currentFrame;
			_diffFrame = _responseFrame - _sendFrame;
		}

		public bool isActive()
		{
			return _socket.isActive();
		}

		public long getPingUsec()
		{
			return _diffUsec;
		}

		public long getPingFrame()
		{
			return _diffFrame;
		}

		public override string ToString()
		{
			string text = "";
			if (isActive())
			{
				text += "active ";
				text = text + "R " + _recvNumber + " ";
				text = text + "S " + _sendNumber + " ";
				text = string.Concat(text, "RT ", _recvTime, " ");
				text = string.Concat(text, "ST ", _sendTime, " ");
				return text + "Diff " + _diffFrame + "," + _diffUsec + " ";
			}
			return text + "sleep ";
		}
	}
}
