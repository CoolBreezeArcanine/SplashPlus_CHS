namespace Comio
{
	public class JvsSyncProtect
	{
		public static bool IsSync(byte data)
		{
			if (data == 224)
			{
				return true;
			}
			return false;
		}

		public static bool IsMarker(byte data)
		{
			if (data == 208)
			{
				return true;
			}
			return false;
		}

		public static bool IsSyncOrMarker(byte data)
		{
			if (IsSync(data) || IsMarker(data))
			{
				return true;
			}
			return false;
		}

		public static void MakeReq(Packet plain, byte dstNodeId, byte commandNo, byte[] data, byte dataSize)
		{
			plain.Clear();
			plain.Add(224);
			plain.Add(dstNodeId);
			plain.Add(1);
			plain.Add((byte)(dataSize + 1));
			plain.Add(commandNo);
			if (dataSize != 0 && data.Length != 0)
			{
				plain.AddRange(data);
			}
			SetSum(plain);
		}

		public static void SetSum(Packet plain)
		{
			if (plain.Count >= 5)
			{
				int num = plain.Count - 2;
				byte b = 0;
				int num2 = 1;
				for (int i = 0; i < num; i++)
				{
					b = (byte)(b + plain[num2++]);
				}
				plain[plain.Count - 1] = b;
			}
		}

		public static bool Encode(Packet encode, Packet plain)
		{
			int num = 5;
			if (plain.Count < num)
			{
				return false;
			}
			if (plain[0] != 224)
			{
				return false;
			}
			encode.Clear();
			encode.Add(224);
			for (int i = 1; i < plain.Count; i++)
			{
				if (IsSyncOrMarker(plain[i]))
				{
					if ((long)(encode.Count + 1) >= 76L)
					{
						return false;
					}
					encode.Add(208);
					encode.Add((byte)(plain[i] - 1));
				}
				else
				{
					if ((long)encode.Count >= 76L)
					{
						return false;
					}
					encode.Add(plain[i]);
				}
			}
			return true;
		}

		public static bool Decode(Packet plain, Packet encode, out bool sumError, out bool middleSync)
		{
			sumError = false;
			middleSync = false;
			if (encode.Count == 0)
			{
				return false;
			}
			if (encode[0] == 224)
			{
				plain.Clear();
				plain.Add(224);
				bool flag = false;
				int num = encode.Count - 1;
				for (int i = 1; i < encode.Count; i++)
				{
					if (IsSync(encode[i]))
					{
						flag = true;
						break;
					}
					if (IsMarker(encode[i]))
					{
						if (num < 2)
						{
							return false;
						}
						plain.Add((byte)(encode[i + 1] + 1));
						i++;
						num--;
					}
					else
					{
						plain.Add(encode[i]);
					}
					num--;
				}
				PacketHeader packetHeader = (PacketHeader)plain;
				if (plain.Count < 4)
				{
					if (flag)
					{
						middleSync = true;
					}
					return false;
				}
				int num2 = 4 + packetHeader.length + 1;
				if (plain.Count < num2)
				{
					if (flag)
					{
						middleSync = true;
					}
					return false;
				}
				if (plain.Count > num2)
				{
					plain.RemoveRange(num2, plain.Count - num2);
				}
				byte b = 0;
				for (int j = 1; j < plain.Count - 1; j++)
				{
					b = (byte)(b + plain[j]);
				}
				if (b != plain[plain.Count - 1])
				{
					sumError = true;
				}
				return true;
			}
			return false;
		}
	}
}
