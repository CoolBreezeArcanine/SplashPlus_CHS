using System;
using Net.VO;
using Net.VO.Mai2;
using UnityEngine;

namespace Net.Packet.Mai2
{
	public class PacketUploadUserPortrait : Packet
	{
		private const int ChunkSize = 10240;

		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly byte[] _source;

		private readonly int _chunkLength;

		private int _chunkNum;

		public PacketUploadUserPortrait(ulong userId, string fname, byte[] source, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_source = source;
			NetQuery<UploadUserPortraitRequestVO, UpsertResponseVO> netQuery = new NetQuery<UploadUserPortraitRequestVO, UpsertResponseVO>("UploadUserPortraitApi", 0uL);
			VOExtensions.ExportIconData(ref netQuery.Request.userPortrait, userId, fname);
			_chunkLength = _source.Length / 10240;
			if (_source.Length % 10240 != 0)
			{
				_chunkLength++;
			}
			netQuery.Request.userPortrait.divLength = _chunkLength;
			netQuery.Request.userPortrait.divData = GetBase64String(_chunkNum++);
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<UploadUserPortraitRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<UploadUserPortraitRequestVO, UpsertResponseVO>;
				if (_chunkNum < _chunkLength)
				{
					netQuery.Request.userPortrait.divNumber = _chunkNum;
					netQuery.Request.userPortrait.divData = GetBase64String(_chunkNum++);
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
	}
}
