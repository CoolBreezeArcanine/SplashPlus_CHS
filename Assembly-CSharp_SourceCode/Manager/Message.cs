namespace Manager
{
	public class Message
	{
		public ProcessType TargetType { get; private set; }

		public uint TargetId { get; private set; }

		public ushort Id { get; private set; }

		public object[] Param { get; private set; }

		public Message(ProcessType targetProcessType, ushort messageId, params object[] param)
		{
			TargetType = targetProcessType;
			TargetId = 0u;
			Id = messageId;
			Param = param;
		}

		public Message(uint targetProcessId, ushort messageId, params object[] param)
		{
			TargetType = ProcessType.Broadcast;
			TargetId = targetProcessId;
			Id = messageId;
			Param = param;
		}
	}
}
