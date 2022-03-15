using Monitor.MusicSelect.PartyConnect;
using UnityEngine;

namespace Monitor
{
	public class ConnectMatchingController : MonoBehaviour
	{
		[SerializeField]
		[Header("コネクトインフォのプレハブ")]
		private ConnectInfoObject _originalConnectInfoObj;

		[SerializeField]
		[Header("コネクトインフォの座標")]
		private Transform _connectInfoPosition;

		private ConnectInfoObject _connectInfo;

		public void Initialize()
		{
			_connectInfo = Object.Instantiate(_originalConnectInfoObj, _connectInfoPosition);
			_connectInfo.Init();
		}

		public void SetRecritInfo(Texture2D musicTexture)
		{
			_connectInfo.SetRecritInfo(musicTexture);
		}

		public void SetConnectingInfo(Texture2D[] textureList, string[] playerNumList, string[] playerNameList)
		{
			_connectInfo.SetConnectingInfo(textureList, playerNumList, playerNameList);
		}

		public void SetCancelInfo()
		{
			_connectInfo.SetCancelInfo();
		}

		public void UpdateTIme()
		{
			_connectInfo.UpdateTIme();
		}
	}
}
