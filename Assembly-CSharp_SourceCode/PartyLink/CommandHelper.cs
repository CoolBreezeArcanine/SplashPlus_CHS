using System;
using Manager.Party.Party;
using UnityEngine;

namespace PartyLink
{
	public static class CommandHelper
	{
		private static readonly Type[] _Type_table = new Type[32]
		{
			null,
			typeof(AdvocateDelivery),
			null,
			null,
			null,
			null,
			typeof(Hello),
			typeof(HeartBeatRequest),
			typeof(HeartBeatResponse),
			typeof(RequestJoin),
			typeof(CancelJoin),
			typeof(ClientPlayInfo),
			typeof(ClientState),
			typeof(UpdateMechaInfo),
			typeof(ResponseMeasure),
			typeof(FinishNews),
			typeof(StartRecruit),
			typeof(FinishRecruit),
			typeof(JoinResult),
			typeof(Kick),
			typeof(RequestMeasure),
			typeof(StartPlay),
			typeof(PartyPlayInfo),
			typeof(PartyMemberInfo),
			typeof(PartyMemberState),
			typeof(StartClientState),
			typeof(Setting.SettingHostAddress),
			typeof(Setting.SettingRequest),
			typeof(Setting.SettingResponse),
			typeof(Advertise.AdvertiseRequest),
			typeof(Advertise.AdvertiseResponse),
			typeof(Advertise.AdvertiseGo)
		};

		private static readonly Func<string, ICommandParam>[] _DeserializerFunc_table = new Func<string, ICommandParam>[32]
		{
			null,
			JsonUtility.FromJson<AdvocateDelivery>,
			null,
			null,
			null,
			null,
			JsonUtility.FromJson<Hello>,
			JsonUtility.FromJson<HeartBeatRequest>,
			JsonUtility.FromJson<HeartBeatResponse>,
			JsonUtility.FromJson<RequestJoin>,
			JsonUtility.FromJson<CancelJoin>,
			JsonUtility.FromJson<ClientPlayInfo>,
			JsonUtility.FromJson<ClientState>,
			JsonUtility.FromJson<UpdateMechaInfo>,
			JsonUtility.FromJson<ResponseMeasure>,
			JsonUtility.FromJson<FinishNews>,
			JsonUtility.FromJson<StartRecruit>,
			JsonUtility.FromJson<FinishRecruit>,
			JsonUtility.FromJson<JoinResult>,
			JsonUtility.FromJson<Kick>,
			JsonUtility.FromJson<RequestMeasure>,
			JsonUtility.FromJson<StartPlay>,
			JsonUtility.FromJson<PartyPlayInfo>,
			JsonUtility.FromJson<PartyMemberInfo>,
			JsonUtility.FromJson<PartyMemberState>,
			JsonUtility.FromJson<StartClientState>,
			JsonUtility.FromJson<Setting.SettingHostAddress>,
			JsonUtility.FromJson<Setting.SettingRequest>,
			JsonUtility.FromJson<Setting.SettingResponse>,
			JsonUtility.FromJson<Advertise.AdvertiseRequest>,
			JsonUtility.FromJson<Advertise.AdvertiseResponse>,
			JsonUtility.FromJson<Advertise.AdvertiseGo>
		};

		private static readonly string[] _Name_table = new string[32]
		{
			"None", "AdvocateDelivery", "Dummy2", "Dummy3", "Dummy4", "Dummy5", "Hello", "HeartBeatRequest", "HeartBeatResponse", "RequestJoin",
			"CancelJoin", "ClientPlayInfo", "ClientState", "UpdateUserInfo", "ResponseMeasure", "FinishNews", "StartRecruit", "FinishRecruit", "JoinResult", "Kick",
			"RequestMeasure", "StartPlay", "PartyPlayInfo", "PartyMemberInfo", "PartyMemberState", "StartClientState", "SettingHostAddress", "SettingRequest", "SettingResponse", "AdvertiseRequest",
			"AdvertiseResponse", "AdvertiseGo"
		};

		public static Type getParamType(this Command command)
		{
			if (command < Command.None || _Type_table.Length <= (int)command)
			{
				return null;
			}
			return _Type_table[(int)command];
		}

		public static Func<string, ICommandParam> getDeserializerFunc(this Command command)
		{
			if (command < Command.None || _DeserializerFunc_table.Length <= (int)command)
			{
				return null;
			}
			return _DeserializerFunc_table[(int)command];
		}

		public static string getCommandName(this Command command)
		{
			if (command < Command.None || _Name_table.Length <= (int)command)
			{
				return "";
			}
			return _Name_table[(int)command];
		}
	}
}
