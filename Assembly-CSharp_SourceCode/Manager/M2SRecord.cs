using System.Collections.Generic;

namespace Manager
{
	public class M2SRecord
	{
		private M2sfileRecordID _recID = new M2sfileRecordID();

		private List<string> _str = new List<string>();

		public M2SRecord()
		{
			init("");
		}

		public M2SRecord(string str)
		{
			init(str);
		}

		public bool init(string str)
		{
			_recID = new M2sfileRecordID();
			_str.Clear();
			if (str == "")
			{
				return false;
			}
			char[] trimChars = new char[2] { '\r', '\n' };
			str = str.TrimEnd(trimChars);
			char[] separator = new char[1] { '\t' };
			string[] array = str.Split(separator);
			foreach (string item in array)
			{
				_str.Add(item);
			}
			if (_str.Count == 0)
			{
				return false;
			}
			_recID = FumenUtil.tag2Rec(_str[0]);
			return _recID.isValid();
		}

		public string getBase()
		{
			int num = 0;
			string text = string.Empty;
			foreach (string item in _str)
			{
				text += item;
				if (++num != _str.Count)
				{
					text += "\t";
					continue;
				}
				return text;
			}
			return text;
		}

		public M2sfileRecordID getType()
		{
			return _recID;
		}

		public string getStr(uint index)
		{
			if (index >= _str.Count)
			{
				return "";
			}
			return _str[(int)index];
		}

		public int getS32(uint index)
		{
			if (index >= _str.Count)
			{
				return 0;
			}
			M2sfileParamID paramIDFromRec = FumenUtil.getParamIDFromRec(_recID, (int)index);
			if (!paramIDFromRec.isValid())
			{
				return 0;
			}
			if (!paramIDFromRec.isAvailableInt())
			{
				return 0;
			}
			if (_str[(int)index].Length == 0)
			{
				return 0;
			}
			return int.Parse(_str[(int)index]);
		}

		public float getF32(uint index)
		{
			if (index >= _str.Count)
			{
				return 0f;
			}
			M2sfileParamID paramIDFromRec = FumenUtil.getParamIDFromRec(_recID, (int)index);
			if (!paramIDFromRec.isValid())
			{
				return 0f;
			}
			if (!paramIDFromRec.isAvailableFloat())
			{
				return 0f;
			}
			if (_str[(int)index].Length == 0)
			{
				return 0f;
			}
			return float.Parse(_str[(int)index]);
		}

		public int getBar()
		{
			return getS32(1u);
		}

		public int getGrid()
		{
			return getS32(2u);
		}

		public int getPos()
		{
			return getS32(3u);
		}

		public NoteTypeID.TouchArea getTouchArea()
		{
			return getStr(4u) switch
			{
				"B" => NoteTypeID.TouchArea.B, 
				"C" => NoteTypeID.TouchArea.C, 
				"E" => NoteTypeID.TouchArea.E, 
				_ => NoteTypeID.TouchArea.None, 
			};
		}

		public NoteTypeID.TouchArea getTouchHoldArea()
		{
			return getStr(5u) switch
			{
				"B" => NoteTypeID.TouchArea.B, 
				"C" => NoteTypeID.TouchArea.C, 
				"E" => NoteTypeID.TouchArea.E, 
				_ => NoteTypeID.TouchArea.None, 
			};
		}

		public int getTouchEffect()
		{
			return getS32(5u);
		}

		public NoteTypeID.TouchSize getTouchSize()
		{
			string str = getStr(6u);
			if (!(str == "M1"))
			{
				if (str == "L1")
				{
					return NoteTypeID.TouchSize.L1;
				}
				return NoteTypeID.TouchSize.M1;
			}
			return NoteTypeID.TouchSize.M1;
		}

		public int getTouchHoldEffect()
		{
			return getS32(6u);
		}

		public NoteTypeID.TouchSize getTouchHoldSize()
		{
			string str = getStr(7u);
			if (!(str == "M1"))
			{
				if (str == "L1")
				{
					return NoteTypeID.TouchSize.L1;
				}
				return NoteTypeID.TouchSize.M1;
			}
			return NoteTypeID.TouchSize.M1;
		}

		public int getHoldLen()
		{
			return getS32(4u);
		}

		public int getSlideWaitLen()
		{
			return getS32(4u);
		}

		public int getSlideShootLen()
		{
			return getS32(5u);
		}

		public int getSlideEndPos()
		{
			return getS32(6u);
		}

		public int getTotalNum()
		{
			return getS32(1u);
		}

		public static bool operator <(M2SRecord op1, M2SRecord op2)
		{
			if (op1.getS32(1u) != op2.getS32(1u))
			{
				return op1.getS32(1u) < op2.getS32(1u);
			}
			if (op1.getS32(2u) != op2.getS32(2u))
			{
				return op1.getS32(2u) < op2.getS32(2u);
			}
			if (op1.getS32(3u) != op2.getS32(3u))
			{
				return op1.getS32(3u) < op2.getS32(3u);
			}
			if (op1.getS32(4u) != op2.getS32(4u))
			{
				return op1.getS32(4u) < op2.getS32(4u);
			}
			if (op1.getType().getValue() != op2.getType().getValue())
			{
				return op1.getType().getValue() < op2.getType().getValue();
			}
			if (op1.getS32(5u) != op2.getS32(5u))
			{
				return op1.getS32(5u) < op2.getS32(5u);
			}
			if (op1.getS32(6u) != op2.getS32(6u))
			{
				return op1.getS32(6u) < op2.getS32(6u);
			}
			return false;
		}

		public static bool operator >(M2SRecord op1, M2SRecord op2)
		{
			if (op1.getS32(1u) != op2.getS32(1u))
			{
				return op1.getS32(1u) > op2.getS32(1u);
			}
			if (op1.getS32(2u) != op2.getS32(2u))
			{
				return op1.getS32(2u) > op2.getS32(2u);
			}
			if (op1.getS32(3u) != op2.getS32(3u))
			{
				return op1.getS32(3u) > op2.getS32(3u);
			}
			if (op1.getS32(4u) != op2.getS32(4u))
			{
				return op1.getS32(4u) > op2.getS32(4u);
			}
			if (op1.getType().getValue() != op2.getType().getValue())
			{
				return op1.getType().getValue() > op2.getType().getValue();
			}
			if (op1.getS32(5u) != op2.getS32(5u))
			{
				return op1.getS32(5u) > op2.getS32(5u);
			}
			if (op1.getS32(6u) != op2.getS32(6u))
			{
				return op1.getS32(6u) > op2.getS32(6u);
			}
			return false;
		}

		public static bool operator ==(M2SRecord op1, M2SRecord op2)
		{
			if (op1.getS32(0u) != op2.getS32(0u))
			{
				return false;
			}
			if (op1.getS32(1u) != op2.getS32(1u))
			{
				return false;
			}
			if (op1.getS32(2u) != op2.getS32(2u))
			{
				return false;
			}
			if (op1.getS32(3u) != op2.getS32(3u))
			{
				return false;
			}
			if (op1.getS32(4u) != op2.getS32(4u))
			{
				return false;
			}
			if (op1.getS32(5u) != op2.getS32(5u))
			{
				return false;
			}
			if (op1.getS32(6u) != op2.getS32(6u))
			{
				return false;
			}
			if (op1.getS32(7u) != op2.getS32(7u))
			{
				return false;
			}
			return true;
		}

		public static bool operator !=(M2SRecord op1, M2SRecord op2)
		{
			return !(op1 == op2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (!(obj is M2SRecord m2SRecord))
			{
				return false;
			}
			return m2SRecord == this;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
