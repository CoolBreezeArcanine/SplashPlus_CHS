using MAI2.Util;
using Manager;
using Process;
using UI.DaisyChainList;
using UnityEngine;

namespace Monitor.MusicSelect.ChainList
{
	public class GenreSelectChainList : MusicSelectDaisyChainList
	{
		public override void AdvancedInitialize(IMusicSelectProcess selectProcess, AssetManager manager, int index)
		{
			base.AdvancedInitialize(selectProcess, manager, index);
		}

		public override void SetScrollCard(bool isVisible)
		{
			if (isVisible)
			{
				GenreChainCardObject genreChainCardObject = (GenreChainCardObject)ScrollChainCard;
				genreChainCardObject.ChangeSize(isMainActive: true);
				SetObjectData(genreChainCardObject, 0);
			}
			base.SetScrollCard(isVisible);
		}

		public bool SetObjectData(GenreChainCardObject card, int index, int difficulty = -1)
		{
			MusicSelectProcess.GenreSelectData genreSelectData = SelectProcess.GetGenreSelectData(MonitorIndex, index);
			if (SelectProcess.GetCategoryName(MonitorIndex, index) == Singleton<DataManager>.Instance.GetMusicGenre(198).genreName)
			{
				genreSelectData = null;
			}
			bool result = false;
			if (genreSelectData != null)
			{
				Sprite tabSprite = SelectProcess.GetTabSprite(genreSelectData);
				GenreChainCardObject.GenreCardPattern cardPattern = ((Singleton<UserDataManager>.Instance.GetUserData(MonitorIndex).Detail.CardType != 0) ? GenreChainCardObject.GenreCardPattern.Extend : GenreChainCardObject.GenreCardPattern.Default);
				card.SetCardPattern(cardPattern);
				card.SetChainActive(isActive: true);
				card.TotalMusicNum = genreSelectData.totalMusicNum;
				card.categoryID = genreSelectData.categoryID;
				card.SetFCAPNum(genreSelectData.fcNum, genreSelectData.fcpNum, genreSelectData.apNum, genreSelectData.appNum, genreSelectData.fsNum, genreSelectData.fsdNum);
				card.SetRankNum(genreSelectData.GetOverRankNum(MusicClearrankID.Rank_A), genreSelectData.GetOverRankNum(MusicClearrankID.Rank_S), genreSelectData.GetOverRankNum(MusicClearrankID.Rank_SP), genreSelectData.GetOverRankNum(MusicClearrankID.Rank_SS), genreSelectData.GetOverRankNum(MusicClearrankID.Rank_SSP), genreSelectData.GetOverRankNum(MusicClearrankID.Rank_SSS), genreSelectData.GetOverRankNum(MusicClearrankID.Rank_SSSP), genreSelectData.isExtra);
				MusicClearrankID rank = MusicClearrankID.Rank_D;
				MusicSelectMonitor.FcapIconEnum fcapIconEnum = MusicSelectMonitor.FcapIconEnum.None;
				fcapIconEnum = ((genreSelectData.totalMusicNum == genreSelectData.appNum) ? MusicSelectMonitor.FcapIconEnum.App : ((genreSelectData.totalMusicNum == genreSelectData.apNum) ? MusicSelectMonitor.FcapIconEnum.Ap : ((genreSelectData.totalMusicNum == genreSelectData.fcpNum) ? MusicSelectMonitor.FcapIconEnum.Fcp : ((genreSelectData.totalMusicNum == genreSelectData.fcNum) ? MusicSelectMonitor.FcapIconEnum.Fc : MusicSelectMonitor.FcapIconEnum.None))));
				for (int num = 13; num >= 0; num--)
				{
					if (genreSelectData.totalMusicNum == genreSelectData.GetOverRankNum((MusicClearrankID)num))
					{
						rank = (MusicClearrankID)num;
						break;
					}
				}
				Sprite[] compIconSprite = SelectProcess.GetCompIconSprite(rank, fcapIconEnum);
				if (genreSelectData.isExtra)
				{
					card.SetIconJacket(null, null, null);
				}
				else
				{
					card.SetIconJacket(compIconSprite[1], compIconSprite[2], compIconSprite[0]);
				}
				if (tabSprite != null)
				{
					card.SetJacket(tabSprite);
				}
				result = true;
			}
			else
			{
				Remove(card);
			}
			return result;
		}

		public void DeployGenreList(bool isAnimation = true)
		{
			RemoveAll();
			int num = -4;
			for (int i = 0; i < 9; i++)
			{
				int diffIndex = -4 + i;
				SelectProcess.IsGenreBoundary(diffIndex, out var _);
				GenreChainCardObject chain = GetChain<GenreChainCardObject>();
				if (SetObjectData(chain, num))
				{
					chain.ChangeSize(i == 4);
					SetSpot(i, chain);
				}
				num++;
			}
			Positioning(isImmediate: true, isAnimation);
			base.IsListEnable = true;
		}

		public void StopAnimatoinAll()
		{
			for (int i = 0; i < GetCardListCount(); i++)
			{
				GenreChainCardObject card = GetCard<GenreChainCardObject>(i);
				if (card != null)
				{
					card.ResetChain();
				}
			}
		}

		protected override void Next(int targetIndex, Direction direction)
		{
			int index = ((direction != Direction.Right) ? 8 : 0);
			GenreChainCardObject chain = GetChain<GenreChainCardObject>();
			if (SetObjectData(chain, targetIndex))
			{
				chain.ChangeSize(isMainActive: false);
				SetSpot(index, chain);
			}
		}
	}
}
