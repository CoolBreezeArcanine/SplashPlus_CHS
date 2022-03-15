using System.IO;
using System.Text.RegularExpressions;

namespace MAI2System
{
	public class IniSection
	{
		public static Regex RegexSectionDefine = new Regex("^\\[(\\w+)\\]");

		public static Regex RegexVariable = new Regex("^(\\w+)\\s*\\=\\s*(\\S+)");

		protected string _variableName;

		protected string _variableValue;

		private IniSection _prev;

		private IniSection _next;

		private IniSection _childHead;

		private IniSection _childTail;

		public string name => _variableName;

		public string value
		{
			get
			{
				return _variableValue;
			}
			set
			{
				_variableValue = value;
			}
		}

		public IniSection childHead => _childHead;

		public IniSection childTail => _childTail;

		public IniSection next => _next;

		public IniSection prev => _prev;

		public IniSection(ref IniSection head, ref IniSection tail, string name, string value)
		{
			_childHead = null;
			_childTail = null;
			if (name != null)
			{
				_variableName = name;
				if (value != null)
				{
					_variableValue = value;
				}
				if (head == null)
				{
					head = this;
					tail = this;
					_next = null;
					_prev = null;
				}
				else
				{
					tail._next = this;
					_prev = tail;
					_next = null;
					tail = this;
				}
			}
		}

		public IniSection addChild(string name, string value)
		{
			if (name != null)
			{
				return new IniSection(ref _childHead, ref _childTail, name, value);
			}
			return null;
		}

		public IniSection removeChild(IniSection child)
		{
			for (IniSection iniSection = _childHead; iniSection != null; iniSection = iniSection.next)
			{
				if (iniSection == child)
				{
					if (child._prev != null)
					{
						child._prev._next = child._next;
					}
					else
					{
						_childHead = child._next;
					}
					if (child._next != null)
					{
						child._next._prev = child._prev;
					}
					else
					{
						_childTail = child._prev;
					}
					child._next = null;
					child._prev = null;
					return child;
				}
			}
			return null;
		}

		public bool getIntValue(out int value)
		{
			return int.TryParse(_variableValue, out value);
		}

		public bool setIntValue(int newValue)
		{
			_variableValue = newValue.ToString();
			return true;
		}

		public bool getFloatValue(out float value)
		{
			return float.TryParse(_variableValue, out value);
		}

		public bool setFloatValue(float newValue)
		{
			_variableValue = newValue.ToString();
			return true;
		}

		public string readSection(StreamReader sr)
		{
			string text;
			while ((text = sr.ReadLine()) != null)
			{
				if (RegexSectionDefine.Match(text).Success)
				{
					return text;
				}
				Match match = RegexVariable.Match(text);
				if (match.Success)
				{
					addChild(match.Groups[1].Value, match.Groups[2].Value);
				}
			}
			return null;
		}

		public int writeSection(StreamWriter sw)
		{
			sw.WriteLine("[" + name + "]");
			for (IniSection iniSection = _childHead; iniSection != null; iniSection = iniSection.next)
			{
				sw.Write(iniSection.name);
				sw.Write(" = ");
				if (iniSection.value != null)
				{
					sw.Write(iniSection.value);
				}
				sw.WriteLine("");
			}
			sw.WriteLine("");
			return 0;
		}
	}
}
