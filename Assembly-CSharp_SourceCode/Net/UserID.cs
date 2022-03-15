namespace Net
{
	public static class UserID
	{
		public static ulong GuestID(uint placeId = 0u)
		{
			return 0x1000000000001uL | ((ulong)(placeId & 0xFFFF) << 32);
		}

		public static bool IsGuest(ulong userId)
		{
			return (userId & 0x1000000000001L) == 281474976710657L;
		}

		public static uint GetPlaceID(ulong userId)
		{
			if (!IsGuest(userId))
			{
				return 0u;
			}
			return (uint)(int)(userId >> 32) & 0xFFFFu;
		}

		public static ulong ToUserID(uint aimeId)
		{
			return aimeId;
		}

		public static uint ToAimeID(ulong userId)
		{
			if (!IsGuest(userId))
			{
				return (uint)userId;
			}
			return 0u;
		}
	}
}
