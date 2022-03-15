using System;
using System.Xml.Serialization;

namespace Manager.MaiStudio
{
	[Serializable]
	public enum EventInfoType
	{
		[XmlEnum(Name = "0")]
		Normal,
		[XmlEnum(Name = "1")]
		Special
	}
}
