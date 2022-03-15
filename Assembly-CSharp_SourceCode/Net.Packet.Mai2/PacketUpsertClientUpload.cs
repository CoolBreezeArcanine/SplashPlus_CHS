using System;
using System.IO;
using Net.VO;
using Net.VO.Mai2;
using UnityEngine;

namespace Net.Packet.Mai2
{
	public class PacketUpsertClientUpload : Packet
	{
		protected internal const int ChunkSize = 10240;

		private readonly Action<int> _onDone;

		private readonly Action<PacketStatus> _onError;

		private readonly byte[] _source;

		private readonly int _chunkLength;

		private int _chunkNum;

		private string _filePath;

		public PacketUpsertClientUpload(string errorLog, byte[] source, Action<int> onDone, Action<PacketStatus> onError = null)
		{
			_onDone = onDone;
			_onError = onError;
			_source = source;
			_filePath = errorLog;
			NetQuery<ClientUploadRequestVO, UpsertResponseVO> netQuery = new NetQuery<ClientUploadRequestVO, UpsertResponseVO>("UpsertClientUploadApi", 0uL);
			VOExtensions.Export(ref netQuery.Request.clientUpload, Path.GetFileName(_filePath));
			_chunkLength = _source.Length / 10240;
			if (_source.Length % 10240 != 0)
			{
				_chunkLength++;
			}
			netQuery.Request.clientUpload.divLength = _chunkLength;
			netQuery.Request.clientUpload.divData = GetBase64String(_chunkNum++);
			Create(netQuery);
		}

		public override PacketState Proc()
		{
			switch (ProcImpl())
			{
			case PacketState.Done:
			{
				NetQuery<ClientUploadRequestVO, UpsertResponseVO> netQuery = base.Query as NetQuery<ClientUploadRequestVO, UpsertResponseVO>;
				if (_chunkNum < _chunkLength)
				{
					netQuery.Request.clientUpload.orderId = netQuery.Response.returnCode;
					netQuery.Request.clientUpload.divNumber = _chunkNum;
					netQuery.Request.clientUpload.divData = GetBase64String(_chunkNum++);
					Create(netQuery);
				}
				else
				{
					try
					{
						File.Delete(_filePath);
					}
					catch
					{
					}
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
