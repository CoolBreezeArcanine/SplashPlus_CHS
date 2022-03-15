using System;
using System.Xml.Serialization;

namespace Manager.MaiStudio
{
	[Serializable]
	public enum MusicLockType
	{
		[XmlEnum(Name = "0")]
		Unlock,
		[XmlEnum(Name = "1")]
		Lock,
		[XmlEnum(Name = "2")]
		Challenge,
		[XmlEnum(Name = "3")]
		Transmission
	}
}
