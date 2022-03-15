namespace DB
{
	public static class EventModeMusicCountIDEnum
	{
		private static readonly EventModeMusicCountTableRecord[] records = new EventModeMusicCountTableRecord[4]
		{
			new EventModeMusicCountTableRecord(0, "_1", "1", 1),
			new EventModeMusicCountTableRecord(1, "_2", "2", 2),
			new EventModeMusicCountTableRecord(2, "_3", "3", 3),
			new EventModeMusicCountTableRecord(3, "_4", "4", 4)
		};

		public static bool IsActive(this EventModeMusicCountID self)
		{
			if (self >= EventModeMusicCountID._1 && self < EventModeMusicCountID.End)
			{
				return self != EventModeMusicCountID._1;
			}
			return false;
		}

		public static bool IsValid(this EventModeMusicCountID self)
		{
			if (self >= EventModeMusicCountID._1)
			{
				return self < EventModeMusicCountID.End;
			}
			return false;
		}

		public static void Clamp(this EventModeMusicCountID self)
		{
			if (self < EventModeMusicCountID._1)
			{
				self = EventModeMusicCountID._1;
			}
			else if ((int)self >= GetEnd())
			{
				self = (EventModeMusicCountID)GetEnd();
			}
		}

		public static int GetEnd(this EventModeMusicCountID self)
		{
			return GetEnd();
		}

		public static EventModeMusicCountID FindID(string enumName)
		{
			for (EventModeMusicCountID eventModeMusicCountID = EventModeMusicCountID._1; eventModeMusicCountID < EventModeMusicCountID.End; eventModeMusicCountID++)
			{
				if (eventModeMusicCountID.GetEnumName() == enumName)
				{
					return eventModeMusicCountID;
				}
			}
			return EventModeMusicCountID.Invalid;
		}

		public static int GetEnd()
		{
			return records.Length;
		}

		public static int GetEnumValue(this EventModeMusicCountID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumValue;
			}
			return 0;
		}

		public static string GetEnumName(this EventModeMusicCountID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].EnumName;
			}
			return "";
		}

		public static string GetName(this EventModeMusicCountID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Name;
			}
			return "";
		}

		public static int GetTrack(this EventModeMusicCountID self)
		{
			if (self.IsValid())
			{
				return records[(int)self].Track;
			}
			return 0;
		}
	}
}
