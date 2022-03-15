using System.Collections.Generic;
using System.IO;

namespace Manager
{
	public class NotesRecord
	{
		private List<M2SRecord> _list = new List<M2SRecord>();

		public M2SRecordList _roList;

		public NotesRecord()
		{
			_roList = new M2SRecordList(_list);
		}

		public bool load(string filename)
		{
			FileInfo fileInfo = new FileInfo(filename);
			try
			{
				using StreamReader streamReader = fileInfo.OpenText();
				string text = "";
				while ((text = streamReader.ReadLine()) != null)
				{
					addRecord(text);
				}
			}
			catch
			{
			}
			return _list.Count != 0;
		}

		public bool loadStr(string data)
		{
			if (data == "")
			{
				return false;
			}
			char[] separator = new char[2] { '\r', '\n' };
			string[] array = data.Split(separator);
			foreach (string str in array)
			{
				addRecord(str);
			}
			return _list.Count != 0;
		}

		public bool addRecord(string str)
		{
			M2SRecord m2SRecord = new M2SRecord();
			if (!m2SRecord.init(str))
			{
				return false;
			}
			return addRecord(m2SRecord);
		}

		public bool addRecord(M2SRecord rec)
		{
			if (!rec.getType().isValid())
			{
				return false;
			}
			_list.Add(rec);
			return true;
		}

		public M2SRecordList getList()
		{
			return _roList;
		}
	}
}
