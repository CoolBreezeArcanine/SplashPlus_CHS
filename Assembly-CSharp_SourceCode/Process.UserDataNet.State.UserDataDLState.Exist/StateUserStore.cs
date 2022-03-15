using MAI2.Util;
using Manager;
using Net.VO.Mai2;

namespace Process.UserDataNet.State.UserDataDLState.Exist
{
	public class StateUserStore : ProcessState<UserDataDLProcess>
	{
		public override void Init(params object[] args)
		{
			int num = (int)args[0];
			NetUserData netUserData = Singleton<NetDataManager>.Instance.GetNetUserData(num);
			UserData userData = Singleton<UserDataManager>.Instance.GetUserData(num);
			userData.Detail = netUserData.Detail.Export(netUserData.UserId);
			userData.CardList = netUserData.CardList.Export();
			userData.Option = netUserData.Option.Export();
			userData.Extend = netUserData.Extend.Export();
			userData.RatingList = netUserData.Rating.Export();
			userData.PlayRegionList = netUserData.RegionList.Export();
			userData.PlateList = netUserData.ItemLists[ItemKind.Plate].Export();
			userData.TitleList = netUserData.ItemLists[ItemKind.Title].Export();
			userData.IconList = netUserData.ItemLists[ItemKind.Icon].Export();
			userData.PartnerList = netUserData.ItemLists[ItemKind.Partner].Export();
			userData.PresentList = netUserData.ItemLists[ItemKind.Present].Export();
			userData.FrameList = netUserData.ItemLists[ItemKind.Frame].Export();
			userData.TicketList = netUserData.ItemLists[ItemKind.Ticket].Export();
			userData.MusicUnlockList = netUserData.ItemLists[ItemKind.Music].ExportToIntList();
			userData.MusicUnlockMasterList = netUserData.ItemLists[ItemKind.MusicMas].ExportToIntList();
			userData.MusicUnlockReMasterList = netUserData.ItemLists[ItemKind.MusicRem].ExportToIntList();
			userData.MusicUnlockStrongList = netUserData.ItemLists[ItemKind.MusicSrg].ExportToIntList();
			userData.FavoriteIconList = netUserData.FavoriteList[FavoriteKind.Icon].Export();
			userData.FavoritePlateList = netUserData.FavoriteList[FavoriteKind.Plate].Export();
			userData.FavoriteTitleList = netUserData.FavoriteList[FavoriteKind.Title].Export();
			userData.FavoriteCharaList = netUserData.FavoriteList[FavoriteKind.Chara].Export();
			userData.FavoriteFrameList = netUserData.FavoriteList[FavoriteKind.Frame].Export();
			userData.ScoreList = netUserData.MusicDetailList.ExportScoreList();
			userData.PlayRegionList = netUserData.RegionList.Export();
			userData.Option.CheckOverParam();
			Process.ToNextUser();
		}
	}
}
