using System;
using System.IO;
using System.Text.RegularExpressions;

namespace MAI2System
{
	public class IniFile : IDisposable
	{
		private bool _disposed;

		private bool _dirty;

		private IniSection _head;

		private IniSection _tail;

		private string _filename;

		public IniFile(string filename)
		{
			construct(filename);
		}

		~IniFile()
		{
			dispose(disposing: false);
		}

		public void Dispose()
		{
			dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		public string getValue(string section, string variable, string defaultParam)
		{
			string result = defaultParam;
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				IniSection iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null)
				{
					result = iniSection2.value;
					if (result == null)
					{
						result = defaultParam;
					}
					return result;
				}
			}
			setValue(section, variable, defaultParam);
			return result;
		}

		public int setValue(string section, string variable, string newValue)
		{
			_dirty = true;
			IniSection iniSection = findSection(section);
			if (iniSection == null)
			{
				iniSection = _head.addChild(section, null);
			}
			IniSection iniSection2 = findVariable(iniSection, variable);
			if (iniSection2 == null)
			{
				iniSection.addChild(variable, newValue);
			}
			else
			{
				iniSection2.value = newValue;
			}
			return 1;
		}

		public int getValue(string section, string variable, int defaultParam)
		{
			return getIntValue(section, variable, defaultParam);
		}

		public bool setValue(string section, string variable, int newValue)
		{
			return setIntValue(section, variable, newValue);
		}

		public uint getValue(string section, string variable, uint defaultParam)
		{
			return (uint)getIntValue(section, variable, (int)defaultParam);
		}

		public bool setValue(string section, string variable, uint newValue)
		{
			return setIntValue(section, variable, (int)newValue);
		}

		public float getValue(string section, string variable, float defaultParam)
		{
			return getFloatValue(section, variable, defaultParam);
		}

		public bool setValue(string section, string variable, float newValue)
		{
			return setFloatValue(section, variable, newValue);
		}

		public bool getValue(string section, string variable, bool defaultParam)
		{
			return getIntValue(section, variable, defaultParam ? 1 : 0) != 0;
		}

		public bool setValue(string section, string variable, bool newValue)
		{
			return setIntValue(section, variable, newValue ? 1 : 0);
		}

		public int getIntValue(string section, string variable, int defaultParam)
		{
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				IniSection iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null && iniSection2.getIntValue(out var value))
				{
					return value;
				}
			}
			setIntValue(section, variable, defaultParam);
			return defaultParam;
		}

		public bool setIntValue(string section, string variable, int newValue)
		{
			_dirty = true;
			IniSection iniSection = findSection(section);
			if (iniSection == null)
			{
				iniSection = _head.addChild(section, null);
			}
			IniSection iniSection2 = findVariable(iniSection, variable);
			if (iniSection2 == null)
			{
				iniSection2 = iniSection.addChild(variable, null);
			}
			return iniSection2.setIntValue(newValue);
		}

		public float getFloatValue(string section, string variable, float defaultParam)
		{
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				IniSection iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null && iniSection2.getFloatValue(out var value))
				{
					return value;
				}
			}
			setFloatValue(section, variable, defaultParam);
			return defaultParam;
		}

		public bool setFloatValue(string section, string variable, float newValue)
		{
			_dirty = true;
			IniSection iniSection = findSection(section);
			if (iniSection == null)
			{
				iniSection = _head.addChild(section, null);
			}
			IniSection iniSection2 = findVariable(iniSection, variable);
			if (iniSection2 == null)
			{
				iniSection2 = iniSection.addChild(variable, null);
			}
			return iniSection2.setFloatValue(newValue);
		}

		public int deleteSection(string section)
		{
			IniSection iniSection = findSection(section);
			if (iniSection != null)
			{
				iniSection = _head.removeChild(iniSection);
			}
			if (iniSection != null)
			{
				_dirty = true;
				return 1;
			}
			return 0;
		}

		public int deleteVariable(string section, string variable)
		{
			IniSection iniSection = findSection(section);
			IniSection iniSection2 = null;
			if (iniSection != null)
			{
				iniSection2 = findVariable(iniSection, variable);
				if (iniSection2 != null)
				{
					iniSection2 = iniSection.removeChild(iniSection2);
				}
			}
			if (iniSection2 != null)
			{
				_dirty = true;
				return 1;
			}
			return 0;
		}

		public IniSection findSection(string section)
		{
			if (section == null)
			{
				if (_head != null)
				{
					return _head.childHead;
				}
				return null;
			}
			if (_head != null)
			{
				for (IniSection iniSection = _head.childHead; iniSection != null; iniSection = iniSection.next)
				{
					if (iniSection.name == section)
					{
						return iniSection;
					}
				}
			}
			return null;
		}

		public IniSection findVariable(IniSection section, string variable)
		{
			if (section != null)
			{
				for (IniSection iniSection = section.childHead; iniSection != null; iniSection = iniSection.next)
				{
					if (iniSection.name == variable)
					{
						return iniSection;
					}
				}
			}
			return null;
		}

		public void clear()
		{
			_head = null;
			_tail = null;
			new IniSection(ref _head, ref _tail, "Root", null);
		}

		private void construct(string filename)
		{
			_dirty = false;
			_head = null;
			_tail = null;
			if (filename == null)
			{
				return;
			}
			_filename = filename;
			try
			{
				using StreamReader sr = new StreamReader(_filename);
				readFromFile(sr);
			}
			catch (Exception)
			{
				new IniSection(ref _head, ref _tail, "Root", null);
			}
		}

		private void dispose(bool disposing)
		{
			if (!_disposed)
			{
				_ = _dirty;
				if (disposing)
				{
					_head = null;
					_tail = null;
					_filename = null;
				}
				_disposed = true;
			}
		}

		private bool readFromFile(StreamReader sr)
		{
			if (_head != null)
			{
				return false;
			}
			new IniSection(ref _head, ref _tail, "Root", null);
			string text = null;
			while (true)
			{
				if (text == null)
				{
					text = sr.ReadLine();
				}
				if (text == null)
				{
					break;
				}
				Match match = IniSection.RegexSectionDefine.Match(text);
				text = ((!match.Success) ? null : _head.addChild(match.Groups[1].Value, null)?.readSection(sr));
			}
			return true;
		}

		private bool writeToFile(StreamWriter sw)
		{
			for (IniSection iniSection = _head.childHead; iniSection != null; iniSection = iniSection.next)
			{
				iniSection.writeSection(sw);
			}
			return true;
		}
	}
}
