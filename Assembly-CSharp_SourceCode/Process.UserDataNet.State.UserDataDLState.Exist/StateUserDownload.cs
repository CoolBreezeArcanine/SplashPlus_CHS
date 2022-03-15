using System;
using System.Collections.Generic;
using MAI2.Util;
using Manager;
using Net.Packet.Helper;
using Net.Packet.Mai2;
using Net.VO.Mai2;
using UnityEngine;

namespace Process.UserDataNet.State.UserDataDLState.Exist
{
	public class StateUserDownload : ProcessState<UserDataDLProcess>
	{
		protected Queue<Action> _actions;

		protected int _index;

		protected ulong _userId;

		protected NetUserData _dst;

		protected string _accessCode;

		public override void Init(params object[] args)
		{
			_index = (int)args[0];
			_userId = Singleton<UserDataManager>.Instance.GetUserData(_index).Detail.UserID;
			_dst = Singleton<NetDataManager>.Instance.GetNetUserData(_index);
			_accessCode = Singleton<UserDataManager>.Instance.GetUserData(_index).Detail.AccessCode;
			if (_actions == null)
			{
				_actions = new Queue<Action>();
			}
			_actions.Enqueue(GetUserData);
			_actions.Enqueue(GetUserCard);
			_actions.Enqueue(GetUserCharacter);
			_actions.Enqueue(GetUserItemPlate);
			_actions.Enqueue(GetUserItemTitle);
			_actions.Enqueue(GetUserItemPartner);
			_actions.Enqueue(GetUserItemIcon);
			_actions.Enqueue(GetUserPresent);
			_actions.Enqueue(GetUserItemFrame);
			_actions.Enqueue(GetUserItemTicket);
			_actions.Enqueue(GetUserMusicUnlock);
			_actions.Enqueue(GetUserMusicUnlockMaster);
			_actions.Enqueue(GetUserMusicUnlockReMaster);
			_actions.Enqueue(GetUserMusicUnlockStrong);
			_actions.Enqueue(GetUserCourse);
			_actions.Enqueue(GetUserCharge);
			_actions.Enqueue(GetUserFavoriteIcon);
			_actions.Enqueue(GetUserFavoritePlate);
			_actions.Enqueue(GetUserFavoriteTitle);
			_actions.Enqueue(GetUserFavoriteChara);
			_actions.Enqueue(GetUserFavoriteFrame);
			_actions.Enqueue(GetUserGhost);
			_actions.Enqueue(GetUserMap);
			_actions.Enqueue(GetUserLoginBonus);
			_actions.Enqueue(GetUserRegion);
			_actions.Enqueue(GetUserOption);
			_actions.Enqueue(GetUserExtend);
			_actions.Enqueue(GetUserRating);
			_actions.Enqueue(GetUserMusic);
			_actions.Enqueue(GetUserPortrait);
			_actions.Enqueue(GetUserActivity);
			if (Singleton<ScoreRankingManager>.Instance.ScoreRankings.Count != 0)
			{
				_actions.Enqueue(GetUserScoreRanking);
			}
			_actions.Enqueue(GotoNextState);
			_actions.Dequeue()();
		}

		private void GetUserData()
		{
			PacketHelper.StartPacket(new PacketGetUserData(_userId, delegate(UserDetail data)
			{
				_dst.Detail = data;
				_dst.Detail.accessCode = _accessCode;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserCard()
		{
			PacketHelper.StartPacket(new PacketGetUserCard(_userId, delegate(UserCard[] data)
			{
				_dst.CardList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserCharacter()
		{
			PacketHelper.StartPacket(new PacketGetUserCharacter(_userId, delegate(UserCharacter[] data)
			{
				_dst.CharacterList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		protected void GetUserItemPlate()
		{
			GetUserItem(ItemKind.Plate);
		}

		protected void GetUserItemTitle()
		{
			GetUserItem(ItemKind.Title);
		}

		protected void GetUserItemIcon()
		{
			GetUserItem(ItemKind.Icon);
		}

		private void GetUserItemPartner()
		{
			GetUserItem(ItemKind.Partner);
		}

		private void GetUserPresent()
		{
			GetUserItem(ItemKind.Present);
		}

		private void GetUserItemFrame()
		{
			GetUserItem(ItemKind.Frame);
		}

		private void GetUserItemTicket()
		{
			GetUserItem(ItemKind.Ticket);
		}

		private void GetUserMusicUnlock()
		{
			GetUserItem(ItemKind.Music);
		}

		private void GetUserMusicUnlockMaster()
		{
			GetUserItem(ItemKind.MusicMas);
		}

		private void GetUserMusicUnlockReMaster()
		{
			GetUserItem(ItemKind.MusicRem);
		}

		private void GetUserMusicUnlockStrong()
		{
			GetUserItem(ItemKind.MusicSrg);
		}

		private void GetUserItem(ItemKind kind)
		{
			PacketHelper.StartPacket(new PacketGetUserItem(_userId, (long)kind, delegate(UserItem[] data)
			{
				_dst.ItemLists[kind] = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserFavoriteIcon()
		{
			PacketHelper.StartPacket(new PacketGetUserFavorite(_userId, 1, delegate(UserFavorite data)
			{
				_dst.FavoriteList[FavoriteKind.Icon] = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserFavoritePlate()
		{
			PacketHelper.StartPacket(new PacketGetUserFavorite(_userId, 2, delegate(UserFavorite data)
			{
				_dst.FavoriteList[FavoriteKind.Plate] = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserFavoriteTitle()
		{
			PacketHelper.StartPacket(new PacketGetUserFavorite(_userId, 3, delegate(UserFavorite data)
			{
				_dst.FavoriteList[FavoriteKind.Title] = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserFavoriteChara()
		{
			PacketHelper.StartPacket(new PacketGetUserFavorite(_userId, 4, delegate(UserFavorite data)
			{
				_dst.FavoriteList[FavoriteKind.Chara] = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserFavoriteFrame()
		{
			PacketHelper.StartPacket(new PacketGetUserFavorite(_userId, 5, delegate(UserFavorite data)
			{
				_dst.FavoriteList[FavoriteKind.Frame] = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserGhost()
		{
			PacketHelper.StartPacket(new PacketGetUserGhost(_userId, delegate(UserGhost[] data)
			{
				_dst.GhostList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserMap()
		{
			PacketHelper.StartPacket(new PacketGetUserMap(_userId, delegate(UserMap[] data)
			{
				_dst.MapList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserLoginBonus()
		{
			PacketHelper.StartPacket(new PacketGetUserLoginBonus(_userId, delegate(Net.VO.Mai2.UserLoginBonus[] data)
			{
				_dst.LoginBonusList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserRegion()
		{
			PacketHelper.StartPacket(new PacketGetUserRegion(_userId, delegate(UserRegion[] data)
			{
				_dst.RegionList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserOption()
		{
			PacketHelper.StartPacket(new PacketGetUserOption(_userId, delegate(UserOption data)
			{
				_dst.Option = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserExtend()
		{
			PacketHelper.StartPacket(new PacketGetUserExtend(_userId, delegate(UserExtend data)
			{
				_dst.Extend = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserScoreRanking()
		{
			PacketHelper.StartPacket(new PacketGetUserScoreRanking(_userId, delegate(UserScoreRanking[] data)
			{
				_dst.ScoreRanking = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserRating()
		{
			PacketHelper.StartPacket(new PacketGetUserRating(_userId, delegate(UserRating data)
			{
				_dst.Rating = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		protected void GetUserMusic()
		{
			PacketHelper.StartPacket(new PacketGetUserMusic(_userId, delegate(UserMusicDetail[] data)
			{
				_dst.MusicDetailList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		protected void GetUserCourse()
		{
			PacketHelper.StartPacket(new PacketGetUserCourse(_userId, delegate(UserCourse[] data)
			{
				_dst.CourseList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		protected void GetUserCharge()
		{
			PacketHelper.StartPacket(new PacketGetUserCharge(_userId, delegate(UserCharge[] data)
			{
				_dst.ChargeList = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserPortrait()
		{
			PacketHelper.StartPacket(new PacketGetUserPortrait(_userId, delegate(UserPortrait[] _, byte[] bytes)
			{
				if (bytes.Length != 0)
				{
					Texture2D texture2D = new Texture2D(256, 256);
					texture2D.LoadImage(bytes);
					GameManager.FaceIconTexture[_index] = texture2D;
				}
				_actions.Dequeue()();
			}, Process.OnError));
		}

		private void GetUserActivity()
		{
			PacketHelper.StartPacket(new PacketGetUserActivity(_userId, delegate(UserActivity data)
			{
				_dst.Activity = data;
				_actions.Dequeue()();
			}, Process.OnError));
		}

		protected void GotoNextState()
		{
			_dst.UserId = _userId;
			Context.NextState(_index);
		}
	}
}
