using System.Collections.Generic;

namespace PartyLink
{
	public class Analyzer
	{
		private class Info
		{
			private CallBackFunction _callBack;

			public uint _count;

			public CallBackFunction CallBack => _callBack;

			public Info()
			{
				_callBack = null;
				_count = 0u;
			}

			public Info(CallBackFunction func)
			{
				_callBack = func;
				_count = 0u;
			}
		}

		private Dictionary<Command, Info> _commandMap;

		private Packet _packet;

		public Analyzer()
		{
			_commandMap = new Dictionary<Command, Info>();
			_packet = new Packet();
		}

		public void analyze(IpAddress address, BufferType buffer, bool isVersionCheck)
		{
			while (_packet.decode(buffer, address))
			{
				buffer.RemoveRange(0, (int)_packet.getEncryptSize());
				if (!isVersionCheck || _packet.isSameVersion())
				{
					procPacketData(_packet);
				}
			}
		}

		private void procPacketData(Packet packet)
		{
			if (packet == null)
			{
				return;
			}
			Command command = packet.getCommand();
			if (_commandMap.TryGetValue(command, out var value))
			{
				if (value.CallBack != null)
				{
					value.CallBack(packet);
				}
				value._count++;
			}
		}

		public void registCommand(Command command, CallBackFunction callback)
		{
			if (!_commandMap.ContainsKey(command))
			{
				_commandMap.Add(command, new Info(callback));
			}
		}

		public bool isRegisted(Command command)
		{
			return _commandMap.ContainsKey(command);
		}

		public uint getRecvCount(Command command)
		{
			if (!_commandMap.ContainsKey(command))
			{
				return 0u;
			}
			return _commandMap[command]._count;
		}

		public override string ToString()
		{
			string text = "";
			foreach (KeyValuePair<Command, Info> item in _commandMap)
			{
				text = text + "  " + item.Key.getCommandName() + " " + item.Value._count + "\n";
			}
			return text;
		}
	}
}
