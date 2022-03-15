using System;

namespace PartyLink
{
	public class PacketType
	{
		private byte[] _buffer;

		private int _count;

		public int Count => _count;

		private PacketType()
		{
		}

		public PacketType(int capacity)
		{
			_buffer = new byte[capacity];
			_count = 0;
		}

		public void Clear()
		{
			_count = 0;
		}

		public void ClearAndResize(int size)
		{
			if (_buffer.Length >= size)
			{
				_count = size;
				for (int i = 0; i < _count; i++)
				{
					_buffer[i] = 0;
				}
			}
		}

		public byte[] GetBuffer()
		{
			return _buffer;
		}

		public void ChangeCount(int count)
		{
			if (_buffer.Length >= count)
			{
				_count = count;
			}
		}

		public void AddRange(int size)
		{
			if (_count + size <= _buffer.Length)
			{
				_count += size;
				for (int i = _count - size; i < _count; i++)
				{
					_buffer[i] = 0;
				}
			}
		}

		public void AddRange(byte[] srcBuf)
		{
			if (srcBuf != null && _count + srcBuf.Length <= _buffer.Length)
			{
				for (int i = 0; i < srcBuf.Length; i++)
				{
					_buffer[_count + i] = srcBuf[i];
				}
				_count += srcBuf.Length;
			}
		}

		public void AddByBufferType(BufferType bufferType, int pos, int size)
		{
			if (bufferType != null && pos >= 0 && bufferType.Count > pos && size >= 0 && pos + size <= bufferType.Count && _count + size <= _buffer.Length)
			{
				for (int i = 0; i < size; i++)
				{
					_buffer[_count + i] = bufferType[pos + i];
				}
				_count += size;
			}
		}

		public void Set(byte[] srcBuffer, int pos, int size)
		{
			if (srcBuffer != null && pos >= 0 && size >= 0)
			{
				if (size > srcBuffer.Length)
				{
					size = srcBuffer.Length;
				}
				if (_buffer.Length >= size)
				{
					_count = size;
					Array.Copy(srcBuffer, 0, _buffer, 0, _count);
				}
			}
		}
	}
}
