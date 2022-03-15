using System;

namespace PartyLink
{
	[Serializable]
	public class Hello : ICommandParam
	{
		private static readonly int MESSAGE_LENGTH = 31;

		public string _message;

		public int _value;

		public int aaaa;

		public Command getCommand()
		{
			return Command.Hello;
		}

		public Hello(string pMessage)
		{
			_value = 1;
			int length = Math.Min(MESSAGE_LENGTH, pMessage.Length);
			_message = pMessage.Substring(0, length);
			aaaa = 0;
		}

		public Hello()
		{
			_value = 1;
			_message = "";
			aaaa = 0;
		}

		public override string ToString()
		{
			string text = "";
			text = text + "mes " + _message + " ";
			text = text + "value " + _value + " ";
			return text + "aaaa " + aaaa + " ";
		}
	}
}
