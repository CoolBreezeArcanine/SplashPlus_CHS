using System.Linq;
using System.Reflection;
using MAI2.Util;
using Manager;
using Net.Packet;
using Net.Packet.Mai2;

namespace Net
{
	public static class NetPacketUtil
	{
		public static string GetUserAgent(INetQuery query)
		{
			if (query.UserId != 0L)
			{
				return $"{Net.Packet.Packet.Obfuscator(query.Api)}#{query.UserId}";
			}
			return Net.Packet.Packet.Obfuscator(query.Api) + "#" + NetConfig.ClientId;
		}

		public static void ForcedUserLogout()
		{
			if (Singleton<OperationManager>.Instance.NetIconStatus != OperationManager.NetStatus.Online)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(i);
				if (userData.IsEntry && !UserID.IsGuest(userData.Detail.UserID))
				{
					new PacketUserLogout(userData.Detail.UserID, delegate
					{
					}).Proc();
				}
			}
		}

		public static void ForcedUserLogout(int id)
		{
			if (Singleton<OperationManager>.Instance.NetIconStatus == OperationManager.NetStatus.Online && id >= 0 && id < 2)
			{
				UserData userData = Singleton<UserDataManager>.Instance.GetUserData(id);
				if (userData.IsEntry && !UserID.IsGuest(userData.Detail.UserID))
				{
					new PacketUserLogout(userData.Detail.UserID, delegate
					{
					}).Proc();
				}
			}
		}

		public static void CopyMember<TDst, TSrc>(ref TDst dst, TSrc src)
		{
			var first = from f in src.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
				where f.FieldType == typeof(char) || f.FieldType == typeof(byte) || f.FieldType == typeof(short) || f.FieldType == typeof(ushort) || f.FieldType == typeof(int) || f.FieldType == typeof(uint) || f.FieldType == typeof(long) || f.FieldType == typeof(ulong) || f.FieldType == typeof(float) || f.FieldType == typeof(double) || f.FieldType == typeof(bool) || f.FieldType == typeof(string) || f.FieldType.IsEnum
				select new
				{
					name = char.ToUpper(f.Name[0]) + f.Name.Substring(1),
					type = f.FieldType,
					data = f.GetValue(src)
				};
			var second = from f in src.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
				where f.PropertyType == typeof(char) || f.PropertyType == typeof(byte) || f.PropertyType == typeof(short) || f.PropertyType == typeof(ushort) || f.PropertyType == typeof(int) || f.PropertyType == typeof(uint) || f.PropertyType == typeof(long) || f.PropertyType == typeof(ulong) || f.PropertyType == typeof(float) || f.PropertyType == typeof(double) || f.PropertyType == typeof(bool) || f.PropertyType == typeof(string) || f.PropertyType.IsEnum
				select f into p
				select new
				{
					name = char.ToUpper(p.Name[0]) + p.Name.Substring(1),
					type = p.PropertyType,
					data = p.GetValue(src)
				};
			var list = first.Concat(second).ToList();
			object obj = dst;
			var source = (from f in dst.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
				select new
				{
					name = char.ToUpper(f.Name[0]) + f.Name.Substring(1),
					info = f
				}).ToArray();
			var source2 = (from p in dst.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
				select new
				{
					name = char.ToUpper(p.Name[0]) + p.Name.Substring(1),
					info = p
				}).ToArray();
			foreach (var s in list)
			{
				source.FirstOrDefault(f => f.name == s.name && f.info.FieldType == s.type)?.info.SetValue(obj, s.data);
				source2.FirstOrDefault(f => f.name == s.name && f.info.PropertyType == s.type)?.info.SetValue(obj, s.data);
			}
			dst = (TDst)obj;
		}
	}
}
