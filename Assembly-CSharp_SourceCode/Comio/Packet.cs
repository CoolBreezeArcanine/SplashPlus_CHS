using System.Collections.Generic;
using System.Diagnostics;

namespace Comio
{
	public class Packet
	{
		private List<byte> _buffer;

		public byte this[int index]
		{
			get
			{
				return _buffer[index];
			}
			set
			{
				_buffer[index] = value;
			}
		}

		public int Count => _buffer.Count;

		public Packet()
		{
			_buffer = new List<byte>();
		}

		public Packet(int length)
		{
			_buffer = new List<byte>();
			_buffer.AddRange(new byte[length]);
		}

		public Packet(Packet packet)
		{
			_buffer = packet._buffer;
		}

		public void setBuffer(Packet packet)
		{
			_buffer = packet._buffer;
		}

		public void Add(byte item)
		{
			_buffer.Add(item);
		}

		public void Clear()
		{
			_buffer.Clear();
		}

		public byte[] ToArray()
		{
			return _buffer.ToArray();
		}

		public void AddRange(IEnumerable<byte> collection)
		{
			_buffer.AddRange(collection);
		}

		public void RemoveRange(int index, int count)
		{
			_buffer.RemoveRange(index, count);
		}

		[Conditional("APP_DEBUG")]
		public void dump()
		{
			ComioLog.Log("PACKET DUMP");
			ComioLog.Log(" length : " + Count);
			string text = "";
			foreach (byte item in _buffer)
			{
				text += item.ToString("X2");
			}
			ComioLog.Log(" data   : " + text);
		}
	}
}
