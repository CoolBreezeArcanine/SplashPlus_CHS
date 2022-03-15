using System.IO.Ports;

namespace Comio
{
	public class ComParam
	{
		public int BaudRate;

		public int DataBits;

		public bool DtrEnable;

		public bool RtsEnable;

		public int ReadTimeout;

		public int WriteTimeout;

		public Parity Parity;

		public StopBits StopBits;

		public Handshake Handshake;
	}
}
