using System.Text;

namespace MAI2.Memory
{
	public struct Chunk
	{
		private int offset_;

		private int size_;

		private byte[] buffer_;

		public int Offset => offset_;

		public int Size => size_;

		public byte[] Buffer => buffer_;

		public bool IsNull => buffer_ == null;

		public Chunk(int offset, int size, byte[] buffer)
		{
			offset_ = offset;
			size_ = size;
			buffer_ = buffer;
		}

		public bool validate(byte[] buffer)
		{
			return buffer == buffer_;
		}

		public sbyte readS8(ref int position)
		{
			int num = offset_ + position;
			position++;
			return (sbyte)buffer_[num];
		}

		public byte readU8(ref int position)
		{
			int num = offset_ + position;
			position++;
			return buffer_[num];
		}

		public short readS16(ref int position)
		{
			int num = offset_ + position;
			position += 2;
			return (short)(buffer_[num] | (buffer_[num + 1] << 8));
		}

		public ushort readU16(ref int position)
		{
			int num = offset_ + position;
			position += 2;
			return (ushort)(buffer_[num] | (buffer_[num + 1] << 8));
		}

		public int readS32(ref int position)
		{
			int num = offset_ + position;
			position += 4;
			return buffer_[num] | (buffer_[num + 1] << 8) | (buffer_[num + 2] << 16) | (buffer_[num + 3] << 24);
		}

		public uint readU32(ref int position)
		{
			int num = offset_ + position;
			position += 4;
			return (uint)(buffer_[num] | (buffer_[num + 1] << 8) | (buffer_[num + 2] << 16) | (buffer_[num + 3] << 24));
		}

		public long readS64(ref int position)
		{
			int num = offset_ + position + 7;
			position += 8;
			long num2 = 0L;
			int num3 = 0;
			while (num3 < 8)
			{
				num2 <<= 8;
				num2 |= buffer_[num];
				num3++;
				num--;
			}
			return num2;
		}

		public ulong readU64(ref int position)
		{
			int num = offset_ + position + 7;
			position += 8;
			ulong num2 = 0uL;
			int num3 = 0;
			while (num3 < 8)
			{
				num2 <<= 8;
				num2 |= buffer_[num];
				num3++;
				num--;
			}
			return num2;
		}

		public bool readBool(ref int position)
		{
			int num = offset_ + position;
			position++;
			if (buffer_[num] != 0)
			{
				return true;
			}
			return false;
		}

		public string readString(ref int position)
		{
			int num = readS32(ref position);
			string @string = Encoding.UTF8.GetString(buffer_, position, num);
			position += num;
			return @string;
		}

		public int writeS8(int position, sbyte x)
		{
			buffer_[offset_ + position] = (byte)x;
			return position + 1;
		}

		public int writeU8(int position, byte x)
		{
			buffer_[offset_ + position] = x;
			return position + 1;
		}

		public int writeS16(int position, short x)
		{
			int num = offset_ + position;
			buffer_[num] = (byte)((uint)x & 0xFFu);
			buffer_[num + 1] = (byte)((uint)(x >> 8) & 0xFFu);
			return position + 2;
		}

		public int writeU16(int position, ushort x)
		{
			int num = offset_ + position;
			buffer_[num] = (byte)(x & 0xFFu);
			buffer_[num + 1] = (byte)((uint)(x >> 8) & 0xFFu);
			return position + 2;
		}

		public int writeS32(int position, int x)
		{
			int num = offset_ + position;
			buffer_[num] = (byte)((uint)x & 0xFFu);
			buffer_[num + 1] = (byte)((uint)(x >> 8) & 0xFFu);
			buffer_[num + 2] = (byte)((uint)(x >> 16) & 0xFFu);
			buffer_[num + 3] = (byte)((uint)(x >> 24) & 0xFFu);
			return position + 4;
		}

		public int writeU32(int position, uint x)
		{
			int num = offset_ + position;
			buffer_[num] = (byte)(x & 0xFFu);
			buffer_[num + 1] = (byte)((x >> 8) & 0xFFu);
			buffer_[num + 2] = (byte)((x >> 16) & 0xFFu);
			buffer_[num + 3] = (byte)((x >> 24) & 0xFFu);
			return position + 4;
		}

		public int writeS64(int position, long x)
		{
			int num = offset_ + position;
			buffer_[num] = (byte)(x & 0xFF);
			buffer_[num + 1] = (byte)((x >> 8) & 0xFF);
			buffer_[num + 2] = (byte)((x >> 16) & 0xFF);
			buffer_[num + 3] = (byte)((x >> 24) & 0xFF);
			buffer_[num + 4] = (byte)((x >> 32) & 0xFF);
			buffer_[num + 5] = (byte)((x >> 40) & 0xFF);
			buffer_[num + 6] = (byte)((x >> 48) & 0xFF);
			buffer_[num + 7] = (byte)((x >> 56) & 0xFF);
			return position + 8;
		}

		public int writeU64(int position, ulong x)
		{
			int num = offset_ + position;
			buffer_[num] = (byte)(x & 0xFF);
			buffer_[num + 1] = (byte)((x >> 8) & 0xFF);
			buffer_[num + 2] = (byte)((x >> 16) & 0xFF);
			buffer_[num + 3] = (byte)((x >> 24) & 0xFF);
			buffer_[num + 4] = (byte)((x >> 32) & 0xFF);
			buffer_[num + 5] = (byte)((x >> 40) & 0xFF);
			buffer_[num + 6] = (byte)((x >> 48) & 0xFF);
			buffer_[num + 7] = (byte)((x >> 56) & 0xFF);
			return position + 8;
		}

		public int writeBool(int position, bool x)
		{
			buffer_[offset_ + position] = (byte)(x ? 1 : 0);
			return position + 1;
		}

		public int writeString(int position, string str)
		{
			int bytes = Encoding.UTF8.GetBytes(str, 0, str.Length, buffer_, offset_ + position + 4);
			writeS32(position, bytes);
			return position + 4 + bytes;
		}
	}
}
