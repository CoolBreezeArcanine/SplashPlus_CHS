using System;
using System.Xml.Serialization;

namespace Manager.MaiStudio
{
	[Serializable]
	public enum Group
	{
		[XmlEnum(Name = "0")]
		GroupA,
		[XmlEnum(Name = "1")]
		GroupB,
		[XmlEnum(Name = "2")]
		GroupC,
		[XmlEnum(Name = "3")]
		GroupD,
		[XmlEnum(Name = "4")]
		GroupE,
		[XmlEnum(Name = "5")]
		GroupF,
		[XmlEnum(Name = "6")]
		GroupG,
		[XmlEnum(Name = "7")]
		GroupH,
		[XmlEnum(Name = "8")]
		GroupI,
		[XmlEnum(Name = "9")]
		GroupJ,
		[XmlEnum(Name = "10")]
		GroupK,
		[XmlEnum(Name = "11")]
		GroupL
	}
}
