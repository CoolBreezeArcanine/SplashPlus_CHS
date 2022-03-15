using System;
using System.Collections.Generic;
using MAI2.Util;
using Manager;
using Process.UserDataNet.State.UserDataDLState.Exist;

namespace Process.UserDataNet.State.UserDataDLState.Inherit
{
	public class StateInheritDownload : StateUserDownload
	{
		public override void Init(params object[] args)
		{
			_index = (int)args[0];
			_userId = Singleton<UserDataManager>.Instance.GetUserData(_index).Detail.UserID;
			_dst = Singleton<NetDataManager>.Instance.GetNetUserData(_index);
			if (_actions == null)
			{
				_actions = new Queue<Action>();
			}
			_actions.Enqueue(base.GetUserMusic);
			_actions.Enqueue(base.GetUserItemPlate);
			_actions.Enqueue(base.GetUserItemTitle);
			_actions.Enqueue(base.GetUserItemIcon);
			_actions.Enqueue(base.GotoNextState);
			_actions.Dequeue()();
		}
	}
}
