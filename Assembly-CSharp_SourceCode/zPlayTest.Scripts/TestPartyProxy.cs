using System;
using System.Linq;
using Manager;
using Manager.Party.Party;
using UnityEngine;

namespace zPlayTest.Scripts
{
	public class TestPartyProxy : MonoBehaviour
	{
		private GameObject _go;

		private void Update()
		{
			if (TestCore.IsReady && Party.Get().IsClientToReady() && !_go)
			{
				_go = new GameObject("GameClient");
				_go.AddComponent<TestGameClient>();
			}
		}

		public void StartRecruit()
		{
			UserData[] array = new UserData[2]
			{
				new UserData(),
				new UserData()
			};
			array[0].Detail.UserID = 10100uL;
			array[1].Detail.UserID = 10200uL;
			array[0].Detail.UserName = Environment.UserName + ":0";
			array[1].Detail.UserName = Environment.UserName + ":1";
			MusicDifficultyID[] fumenDifs = new MusicDifficultyID[2]
			{
				MusicDifficultyID.Basic,
				MusicDifficultyID.Advanced
			};
			Party.Get().StartRecruit(new MechaInfo(array, fumenDifs, 1234), RecruitStance.OnlyFriend);
		}

		public void CancelRecruit()
		{
			Party.Get().CancelBothRecruitJoin();
		}

		public void FinishRecruit()
		{
			Party.Get().FinishRecruit();
		}

		public void Join()
		{
			RecruitInfo recruitInfo = Party.Get().GetRecruitListWithoutMe().FirstOrDefault();
			if (recruitInfo != null)
			{
				UserData[] array = new UserData[2]
				{
					new UserData(),
					new UserData()
				};
				array[0].Detail.UserID = 20100uL;
				array[1].Detail.UserID = 20200uL;
				array[0].Detail.UserName = Environment.UserName + ":0";
				array[1].Detail.UserName = Environment.UserName + ":1";
				MusicDifficultyID[] fumenDifs = new MusicDifficultyID[2]
				{
					MusicDifficultyID.Master,
					MusicDifficultyID.ReMaster
				};
				Party.Get().SelectMusic();
				Party.Get().StartJoin(recruitInfo.IpAddress, new MechaInfo(array, fumenDifs, 1234));
			}
		}

		public void Leave()
		{
			Party.Get().CancelBothRecruitJoin();
		}

		public void FinishSetting()
		{
			Party.Get().FinishSetting();
		}

		public void FinishPlay()
		{
			if (_go != null)
			{
				UnityEngine.Object.Destroy(_go);
			}
			Party.Get().FinishPlay();
		}

		public void FinishNews()
		{
			Party.Get().Wait();
		}
	}
}
