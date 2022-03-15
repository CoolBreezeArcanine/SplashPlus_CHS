using MAI2.Util;
using Manager;
using Net.VO.Mai2;

namespace Process.UserDataNet.State.UserDataDLState.Inherit
{
	public class StateUserInheritStore : ProcessState<UserDataDLProcess>
	{
		public override void Init(params object[] args)
		{
			int num = (int)args[0];
			NetUserData netUserData = Singleton<NetDataManager>.Instance.GetNetUserData(num);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(num);
			userData.PlateList = netUserData.ItemLists[ItemKind.Plate].Export();
			userData.TitleList = netUserData.ItemLists[ItemKind.Title].Export();
			userData.IconList = netUserData.ItemLists[ItemKind.Icon].Export();
			userData.ScoreList = netUserData.MusicDetailList.ExportScoreList();
			Process.ToNextUser();
		}
	}
}
