using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using MAI2.Util;
using Manager;
using UnityEngine;

namespace Net.Packet
{
	public abstract class Packet
	{
		protected WebExceptionStatus WebExceptionStatus;

		protected NetHttpClient Client;

		protected int RetryCount;

		protected float Time;

		protected string BaseUrl;

		public PacketState State { get; protected set; }

		public PacketStatus Status { get; protected set; }

		public int HttpStatus { get; protected set; } = -1;


		public WebExceptionStatus WebException => WebExceptionStatus;

		public string ErrorString { get; protected set; } = string.Empty;


		public INetQuery Query { get; protected set; }

		public static string ToString(PacketStatus status)
		{
			return status switch
			{
				PacketStatus.Ok => "成功", 
				PacketStatus.ErrorCreate => "ソケット生成エラー", 
				PacketStatus.ErrorEcodeRequest => "要求メッセージエンコードエラー", 
				PacketStatus.ErrorDecodeResponse => "応答メッセージデコードエラー", 
				PacketStatus.ErrorTimeout => "タイムアウト", 
				PacketStatus.ErrorConnection => "接続エラー", 
				PacketStatus.ErrorInternal => "内部システムエラー", 
				PacketStatus.ErrorHttpStatus => "HTTPエラー", 
				_ => string.Empty, 
			};
		}

		public abstract PacketState Proc();

		protected string GetServerUrl()
		{
			return Singleton<OperationManager>.Instance.GetBaseUri();
		}

		[DllImport("Cake")]
		private static extern int GetObfuscatorLength(byte[] inData, int length);

		[DllImport("Cake")]
		private static extern bool Obfuscator(string api, byte[] outData, int length);

		protected bool Create(INetQuery query, string url = null)
		{
			BaseUrl = url ?? GetServerUrl();
			Query = query;
			Client = NetHttpClient.Create(BaseUrl + Obfuscator(query.Api));
			RetryCount = 0;
			Time = 0f;
			if (Client != null)
			{
				Client.TimeOutInMSec = NetConfig.TimeOutInMSec;
				State = PacketState.Ready;
				Status = PacketStatus.Ok;
				ProcImpl();
				return true;
			}
			State = PacketState.Error;
			Status = PacketStatus.ErrorCreate;
			return false;
		}

		protected bool Reset()
		{
			Client = NetHttpClient.Create(BaseUrl + Obfuscator(Query.Api));
			Time = 0f;
			if (Client != null)
			{
				Client.TimeOutInMSec = NetConfig.TimeOutInMSec;
				State = PacketState.Ready;
				Status = PacketStatus.Ok;
				ProcImpl();
				return true;
			}
			State = PacketState.Error;
			Status = PacketStatus.ErrorCreate;
			return false;
		}

		protected PacketState ProcImpl()
		{
			PacketState state = State;
			switch (State)
			{
			case PacketState.Ready:
				try
				{
					string request = Query.GetRequest();
					if (!Client.Request(Encoding.UTF8.GetBytes(request), NetPacketUtil.GetUserAgent(Query)))
					{
						ProcResult();
					}
					else
					{
						State = PacketState.Process;
					}
				}
				catch
				{
					State = PacketState.Error;
					Status = PacketStatus.ErrorEcodeRequest;
				}
				break;
			case PacketState.Process:
				switch (Client.State)
				{
				case 4:
				{
					byte[] bytes = Client.GetResponse().ToArray();
					string @string = Encoding.UTF8.GetString(bytes);
					try
					{
						if (Query.SetResponse(@string))
						{
							State = PacketState.Done;
							Status = PacketStatus.Ok;
						}
						else
						{
							State = PacketState.Error;
							Status = PacketStatus.ErrorDecodeResponse;
						}
					}
					catch
					{
						State = PacketState.Error;
						Status = PacketStatus.ErrorDecodeResponse;
					}
					Clear();
					break;
				}
				case 5:
					ProcResult();
					break;
				}
				break;
			case PacketState.RetryWait:
				Time += UnityEngine.Time.deltaTime;
				if (NetConfig.RetryWaitInSec <= Time)
				{
					Reset();
				}
				break;
			}
			if (state != State && (State == PacketState.Done || State == PacketState.Error))
			{
				Singleton<OperationManager>.Instance.OnPacketFinish(Status);
			}
			return State;
		}

		protected void Clear()
		{
			if (Client != null)
			{
				HttpStatus = Client.HttpStatus;
				WebExceptionStatus = Client.WebException;
				ErrorString = Client.Error;
				Client.Dispose();
				Client = null;
			}
		}

		protected void ProcResult()
		{
			Clear();
			switch (WebExceptionStatus)
			{
			case WebExceptionStatus.Success:
				if (HttpStatus == 200)
				{
					State = PacketState.Done;
					Status = PacketStatus.Ok;
				}
				else
				{
					State = PacketState.Error;
					Status = PacketStatus.ErrorHttpStatus;
				}
				return;
			case WebExceptionStatus.NameResolutionFailure:
			case WebExceptionStatus.ConnectFailure:
			case WebExceptionStatus.ReceiveFailure:
			case WebExceptionStatus.SendFailure:
				Status = PacketStatus.ErrorConnection;
				break;
			case WebExceptionStatus.PipelineFailure:
			case WebExceptionStatus.RequestCanceled:
			case WebExceptionStatus.ProtocolError:
			case WebExceptionStatus.ConnectionClosed:
			case WebExceptionStatus.TrustFailure:
			case WebExceptionStatus.SecureChannelFailure:
			case WebExceptionStatus.ServerProtocolViolation:
			case WebExceptionStatus.KeepAliveFailure:
			case WebExceptionStatus.Pending:
				Status = PacketStatus.ErrorInternal;
				break;
			case WebExceptionStatus.Timeout:
				Status = PacketStatus.ErrorTimeout;
				break;
			default:
				Status = PacketStatus.ErrorInternal;
				break;
			}
			if (RetryCount < NetConfig.MaxRetry)
			{
				RetryCount++;
				State = PacketState.RetryWait;
			}
			else
			{
				State = PacketState.Error;
			}
		}

		public static string Obfuscator(string srcStr)
		{
			byte[] array = new byte[GetObfuscatorLength(Encoding.ASCII.GetBytes(srcStr), srcStr.Length)];
			Obfuscator(srcStr, array, srcStr.Length);
			if (srcStr == "")
			{
				return Encoding.ASCII.GetString(array);
			}
			return srcStr;
		}
	}
}
