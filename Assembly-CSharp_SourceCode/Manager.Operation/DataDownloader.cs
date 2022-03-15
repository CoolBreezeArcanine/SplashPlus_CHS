using MAI2.Util;
using Net.Packet;

namespace Manager.Operation
{
	public abstract class DataDownloader
	{
		private enum State
		{
			Offline,
			Online,
			Download
		}

		public bool AutomaticDownload = true;

		private readonly Mode<DataDownloader, State> _mode;

		private readonly OnlineCheckInterval _retryInterval;

		private bool _isNetworkTest;

		private bool _isOnline;

		public bool IsOnline => _isOnline;

		public bool WasDownloadSuccessOnce { get; private set; }

		public bool IsBusy => _mode.get() == 2;

		protected DataDownloader()
		{
			_mode = new Mode<DataDownloader, State>(this);
			_mode.set(State.Offline);
			_retryInterval = new OnlineCheckInterval();
			_isNetworkTest = false;
			_isOnline = false;
			WasDownloadSuccessOnce = false;
		}

		public void Execute()
		{
			_mode.update();
		}

		public void Start(bool isNetworkTest)
		{
			if (!IsBusy)
			{
				_isNetworkTest = isNetworkTest;
				if (isNetworkTest)
				{
					_retryInterval.Reset();
				}
				_mode.set(State.Download);
			}
		}

		public void NotifyOnline()
		{
			_isOnline = true;
		}

		public void NotifyOffline()
		{
			_isOnline = false;
			if (_mode.get() == 1)
			{
				_retryInterval.NotifyOffline();
				_mode.set(State.Offline);
			}
		}

		protected abstract void InitPacketList(bool isNetworkTest);

		protected abstract PacketState ProcPacket();

		protected abstract bool NextPacket();

		protected abstract void EndPacket();

		private void Online_Proc()
		{
			if (AutomaticDownload && _retryInterval.IsNeedCheck())
			{
				_mode.set(State.Download);
			}
		}

		private void Offline_Proc()
		{
			if (AutomaticDownload && _retryInterval.IsNeedCheck())
			{
				_mode.set(State.Download);
			}
		}

		private void Download_Init()
		{
			InitializeDownload();
		}

		private void Download_Proc()
		{
			switch (ProcPacket())
			{
			case PacketState.Done:
				if (!NextPacket())
				{
					if (!_isNetworkTest)
					{
						WasDownloadSuccessOnce = true;
					}
					_retryInterval.NotifyOnline();
					TerminateDownload();
					_mode.set(State.Online);
				}
				break;
			case PacketState.Error:
				EndPacket();
				_retryInterval.NotifyOffline();
				TerminateDownload();
				_mode.set(State.Offline);
				break;
			}
		}

		private void InitializeDownload()
		{
			if (_isNetworkTest && !WasDownloadSuccessOnce)
			{
				_isNetworkTest = false;
			}
			InitPacketList(_isNetworkTest);
		}

		private void TerminateDownload()
		{
			_isNetworkTest = false;
		}
	}
}
