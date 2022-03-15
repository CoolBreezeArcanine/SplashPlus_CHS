using System;
using UnityEngine;

namespace Net.VO
{
	public class VOSerializer : ICloneable
	{
		public string Serialize()
		{
			try
			{
				return JsonUtility.ToJson(this);
			}
			catch (Exception)
			{
				return "";
			}
		}

		public void Deserialize(string str)
		{
			try
			{
				JsonUtility.FromJsonOverwrite(str, this);
			}
			catch (Exception)
			{
			}
		}

		public object Clone()
		{
			return MemberwiseClone();
		}
	}
}
