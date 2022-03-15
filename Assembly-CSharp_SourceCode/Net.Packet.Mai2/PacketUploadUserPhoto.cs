using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using MAI2.Util;
using MAI2System;
using Manager;
using Net.VO;
using Net.VO.Mai2;
using UnityEngine;

namespace Net.Packet.Mai2
{
	public class PacketUploadUserPhoto : Packet
	{
		private const int ChunkSize = 10240;

		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly byte[] _source;

		private readonly int _chunkLength;

		private int _chunkNum;

		public PacketUploadUserPhoto(int index, ulong userId, int trackNo, byte[] source, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_source = source;
			NetQuery<UserPhotoRequestVO, UpsertResponseVO> netQuery = new NetQuery<UserPhotoRequestVO, UpsertResponseVO>("UploadUserPhotoApi", 0uL);
			VOExtensions.ExportPhotoData(ref netQuery.Request.userPhoto, userId, Singleton<NetDataManager>.Instance.GetLoginVO(index).loginId, trackNo);
			_chunkLength = _source.Length / 10240;
			if (_source.Length % 10240 != 0)
			{
				_chunkLength++;
			}
			netQuery.Request.userPhoto.divLength = _chunkLength;
			netQuery.Request.userPhoto.divData = GetBase64String(_chunkNum++);
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UserPhotoRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<UserPhotoRequestVO, UpsertResponseVO>;
				if (_chunkNum < _chunkLength)
				{
					netQuery.Request.userPhoto.orderId = netQuery.Response.returnCode;
					netQuery.Request.userPhoto.divNumber = _chunkNum;
					netQuery.Request.userPhoto.divData = GetBase64String(_chunkNum++);
					Create(netQuery);
				}
				else
				{
					_onDone(netQuery.Response.returnCode);
				}
				break;
			}
			case PacketState.Error:
				_onError?.Invoke(base.Status);
				break;
			}
			return base.State;
		}

		private string GetBase64String(int index)
		{
			int num = index * 10240;
			int num2 = Mathf.Clamp(_source.Length - num, 0, 10240);
			byte[] array = new byte[num2];
			Buffer.BlockCopy(_source, num, array, 0, num2);
			return Convert.ToBase64String(array);
		}

		[Conditional("_PHOTO_CHECK")]
		private void CheckData(ulong userId)
		{
			if (_chunkLength >= 3)
			{
				string[] datas = Enumerable.Range(0, _chunkLength - 1).Select(GetBase64String).ToArray();
				datas.Skip(1).Any((string i) => i.Length != datas[0].Length);
			}
		}

		[Conditional("_PHOTO_CHECK")]
		private static void WriteData(ulong userId, byte[] data, bool error)
		{
			string text = DateTime.Now.ToString("yyyyMMddHHmmss");
			string errorLogPath = MAI2System.Path.ErrorLogPath;
			File.WriteAllBytes($"{errorLogPath}Photo_{userId}_{text}_{(error ? 1 : 0)}.bin", data);
		}
	}
}
