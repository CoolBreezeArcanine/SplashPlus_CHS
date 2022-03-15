using System.Collections.Generic;

namespace PartyLink
{
	public class BufferType : List<byte>
	{
		private BufferType()
		{
		}

		public BufferType(int capacity)
			: base(capacity)
		{
		}

		public void CopyRange(byte[] src, int pos, int size)
		{
			if (src != null && pos >= 0 && src.Length >= pos + size && size >= 0)
			{
				int num = base.Count + size;
				if (base.Capacity < num)
				{
					base.Capacity = num;
				}
				for (int i = pos; i < pos + size; i++)
				{
					Add(src[i]);
				}
			}
		}
	}
}
