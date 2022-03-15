using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using AMDaemon.Allnet;

namespace Net
{
	public class NetHttpClient
	{
		public const int DefaultTimeout = 60000;

		public const int BufferSize = 1024;

		public const string Get = "GET";

		public const string Post = "POST";

		public const string ContentTypeJson = "application/json";

		public const string ContentEncodingDeflate = "deflate";

		public const int StateInit = 0;

		public const int StateReady = 1;

		public const int StateRequest = 2;

		public const int StateProcess = 3;

		public const int StateDone = 4;

		public const int StateError = 5;

		protected HttpWebRequest _request;

		protected HttpWebResponse _response;

		protected WaitHandle _waitHandle;

		protected RegisteredWaitHandle _timeoutWaitHandle;

		protected readonly RequestCachePolicy _cachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

		protected Stream _responseStream;

		protected readonly byte[] _buffer = new byte[1024];

		protected readonly MemoryStream _temporaryStream = new MemoryStream(1024);

		protected readonly MemoryStream _memoryStream = new MemoryStream(1024);

		protected int _state;

		protected byte[] _bytes;

		private static byte[][] hostBytes;

		private static bool isTrueDll;

		private const char splitChar = ':';

		private static byte[] thumbBytes;

		public int TimeOutInMSec { get; set; } = 60000;


		public WebExceptionStatus WebException { get; protected set; }

		public string Error { get; protected set; } = string.Empty;


		public int State
		{
			get
			{
				lock (this)
				{
					return _state;
				}
			}
			set
			{
				lock (this)
				{
					_state = value;
				}
			}
		}

		public int HttpStatus { get; protected set; } = -1;


		protected string HttpHeaderEncryption { get; } = Marshal.PtrToStringAnsi(GetHttpEncryptHeader());


		protected string DefaultHttpHeaderEncryption { get; } = Marshal.PtrToStringAnsi(GetHttpEncryptVersion());


		public MemoryStream GetResponse()
		{
			return _memoryStream;
		}

		[DllImport("Cake")]
		private static extern IntPtr GetHttpEncryptHeader();

		[DllImport("Cake")]
		private static extern IntPtr GetHttpEncryptVersion();

		[DllImport("Cake")]
		private static extern bool CheckServerParameter(byte[] inData, byte[] compare);

		public static NetHttpClient Create(string url)
		{
			NetHttpClient netHttpClient = new NetHttpClient();
			try
			{
				if (hostBytes == null)
				{
					string[] array = Auth.GameServerHost.Split(':');
					hostBytes = new byte[array.Length][];
					for (int i = 0; i < array.Length; i++)
					{
						hostBytes[i] = Encoding.ASCII.GetBytes(array[i]);
					}
					isTrueDll = true;
				}
				netHttpClient._request = WebRequest.Create(url) as HttpWebRequest;
				netHttpClient._request.CachePolicy = netHttpClient._cachePolicy;
				netHttpClient.State = 1;
				return netHttpClient;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void Dispose()
		{
			ReleaseTimeoutWaitHandle();
			DestroyRequest();
			DestroyReponse();
			_temporaryStream.Close();
			_temporaryStream.Dispose();
			_memoryStream.Close();
			_memoryStream.Dispose();
		}

		public bool Request(byte[] bytes, string userAgent, string method = "POST")
		{
			_request.Method = method;
			_request.ContentType = "application/json";
			_request.UserAgent = (string.IsNullOrEmpty(userAgent) ? string.Empty : userAgent);
			_request.Headers.Add("charset", "UTF-8");
			_request.Headers.Add("X-" + HttpHeaderEncryption, DefaultHttpHeaderEncryption);
			ResetStatus();
			try
			{
				if (bytes != null && bytes.Length != 0)
				{
					_request.Headers.Add(HttpRequestHeader.ContentEncoding, "deflate");
					WriteToRequest(bytes);
					State = 2;
					return true;
				}
				_request.ContentLength = 0L;
				return Request();
			}
			catch (WebException ex)
			{
				SetError(ex.Status, ex.Message, 5, ex.Response as HttpWebResponse);
				return false;
			}
			catch (Exception ex2)
			{
				SetError(WebExceptionStatus.UnknownError, ex2.Message, 5, null);
				return false;
			}
		}

		public bool Request()
		{
			try
			{
				_memoryStream.SetLength(0L);
				ReleaseTimeoutWaitHandle();
				IAsyncResult asyncResult = _request.BeginGetResponse(ResponseCallback, this);
				_waitHandle = asyncResult.AsyncWaitHandle;
				_timeoutWaitHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle, TimeoutCallback, this, TimeOutInMSec, executeOnlyOnce: true);
				State = 3;
				return true;
			}
			catch (WebException ex)
			{
				SetError(ex.Status, ex.Message, 5, ex.Response as HttpWebResponse);
				return false;
			}
			catch (Exception ex2)
			{
				SetError(WebExceptionStatus.UnknownError, ex2.Message, 5, null);
				return false;
			}
		}

		private void DestroyRequest()
		{
			if (_request != null)
			{
				_request.Abort();
				_request = null;
			}
			_bytes = null;
		}

		private void DestroyReponse()
		{
			if (_responseStream != null)
			{
				_responseStream.Close();
				_responseStream.Dispose();
				_responseStream = null;
			}
			if (_response != null)
			{
				HttpStatus = (int)_response.StatusCode;
				_response.Close();
				_response = null;
			}
		}

		private void ResetStatus()
		{
			WebException = WebExceptionStatus.Success;
			HttpStatus = -1;
			Error = string.Empty;
		}

		private void ReleaseTimeoutWaitHandle()
		{
			if (_timeoutWaitHandle != null)
			{
				if (_waitHandle != null)
				{
					_timeoutWaitHandle.Unregister(_waitHandle);
				}
				_timeoutWaitHandle = null;
			}
			if (_waitHandle != null)
			{
				_waitHandle.Close();
				_waitHandle = null;
			}
		}

		private static void TimeoutCallback(object state, bool timedout)
		{
			if (timedout)
			{
				NetHttpClient netHttpClient = state as NetHttpClient;
				if (netHttpClient._request == null)
				{
					netHttpClient.SetError(WebExceptionStatus.UnknownError, string.Empty, 5, null);
				}
				else
				{
					netHttpClient.SetError(WebExceptionStatus.Timeout, string.Empty, 5, null);
				}
			}
		}

		private void SetError(WebExceptionStatus status, string error, int state, HttpWebResponse response)
		{
			WebException = status;
			Error = error;
			ReleaseTimeoutWaitHandle();
			DestroyReponse();
			if (response != null)
			{
				HttpStatus = (int)response.StatusCode;
			}
			State = state;
		}

		private void SetSuccess(int state)
		{
			WebException = WebExceptionStatus.Success;
			Error = string.Empty;
			DestroyReponse();
			State = state;
		}

		private static void RequestCallback(IAsyncResult asynchronousResult)
		{
			NetHttpClient netHttpClient = asynchronousResult.AsyncState as NetHttpClient;
			Stream stream = null;
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				stream = netHttpClient._request.EndGetRequestStream(asynchronousResult);
				netHttpClient._temporaryStream.SetLength(0L);
				using (DeflateStream deflateStream = new DeflateStream(netHttpClient._temporaryStream, CompressionMode.Compress, leaveOpen: true))
				{
					deflateStream.Write(netHttpClient._bytes, 0, netHttpClient._bytes.Length);
					deflateStream.Close();
				}
				uint num = Adler32(netHttpClient._bytes);
				memoryStream.WriteByte(120);
				memoryStream.WriteByte(156);
				netHttpClient._temporaryStream.Position = 0L;
				while (true)
				{
					int num2 = netHttpClient._temporaryStream.Read(netHttpClient._buffer, 0, 1024);
					if (num2 <= 0)
					{
						break;
					}
					memoryStream.Write(netHttpClient._buffer, 0, num2);
				}
				memoryStream.WriteByte((byte)((num >> 24) & 0xFFu));
				memoryStream.WriteByte((byte)((num >> 16) & 0xFFu));
				memoryStream.WriteByte((byte)((num >> 8) & 0xFFu));
				memoryStream.WriteByte((byte)(num & 0xFFu));
				if (memoryStream == null)
				{
					if (!CipherAES.Encrypt(memoryStream.ToArray(), out var encryptData))
					{
						throw new WebException("Encrypt Failed.", WebExceptionStatus.RequestCanceled);
					}
					memoryStream.SetLength(0L);
					memoryStream.Write(encryptData, 0, encryptData.Length);
				}
				byte[] array = memoryStream.ToArray();
				stream.Write(array, 0, array.Length);
				netHttpClient.Request();
			}
			catch (WebException ex)
			{
				netHttpClient.SetError(ex.Status, ex.Message, 5, ex.Response as HttpWebResponse);
			}
			catch (Exception ex2)
			{
				netHttpClient.SetError(WebExceptionStatus.UnknownError, ex2.Message, 5, null);
			}
			finally
			{
				netHttpClient._bytes = null;
				stream?.Close();
			}
		}

		private static void ResponseCallback(IAsyncResult asynchronousResult)
		{
			NetHttpClient netHttpClient = asynchronousResult.AsyncState as NetHttpClient;
			try
			{
				HttpWebRequest request = netHttpClient._request;
				netHttpClient._response = request.EndGetResponse(asynchronousResult) as HttpWebResponse;
				netHttpClient._responseStream = netHttpClient._response.GetResponseStream();
				netHttpClient._temporaryStream.SetLength(0L);
				netHttpClient._responseStream.BeginRead(netHttpClient._buffer, 0, 1024, ReadCallback, netHttpClient);
			}
			catch (WebException ex)
			{
				netHttpClient.SetError(ex.Status, ex.Message, 5, ex.Response as HttpWebResponse);
			}
			catch (Exception ex2)
			{
				netHttpClient.SetError(WebExceptionStatus.UnknownError, ex2.Message, 5, null);
			}
		}

		private static void ReadCallback(IAsyncResult asynchronousResult)
		{
			NetHttpClient netHttpClient = asynchronousResult.AsyncState as NetHttpClient;
			try
			{
				Stream responseStream = netHttpClient._responseStream;
				int num = responseStream.EndRead(asynchronousResult);
				if (0 < num)
				{
					netHttpClient._temporaryStream.Write(netHttpClient._buffer, 0, num);
					responseStream.BeginRead(netHttpClient._buffer, 0, 1024, ReadCallback, netHttpClient);
				}
				else if (asynchronousResult.IsCompleted)
				{
					if (netHttpClient == null)
					{
						netHttpClient.Decrypt();
					}
					netHttpClient.Decompress();
					netHttpClient.SetSuccess(4);
				}
			}
			catch (WebException ex)
			{
				netHttpClient.SetError(ex.Status, ex.Message, 5, ex.Response as HttpWebResponse);
			}
			catch (Exception ex2)
			{
				netHttpClient.SetError(WebExceptionStatus.UnknownError, ex2.Message, 5, null);
			}
		}

		private void WriteToRequest(byte[] bytes)
		{
			try
			{
				_bytes = bytes;
				_request.BeginGetRequestStream(RequestCallback, this);
			}
			catch
			{
				_bytes = null;
				throw;
			}
		}

		private void Decompress()
		{
			_memoryStream.SetLength(0L);
			if (_temporaryStream.Length < 6)
			{
				return;
			}
			_temporaryStream.Position = 2L;
			_temporaryStream.SetLength(_temporaryStream.Length - 4);
			using (DeflateStream deflateStream = new DeflateStream(_temporaryStream, CompressionMode.Decompress, leaveOpen: true))
			{
				while (true)
				{
					int num = deflateStream.Read(_buffer, 0, 1024);
					if (num <= 0)
					{
						break;
					}
					_memoryStream.Write(_buffer, 0, num);
				}
				deflateStream.Close();
			}
			_memoryStream.Seek(0L, SeekOrigin.Begin);
			_temporaryStream.Seek(0L, SeekOrigin.Begin);
			_temporaryStream.SetLength(0L);
		}

		private void Decrypt()
		{
			using MemoryStream memoryStream = new MemoryStream();
			_temporaryStream.Seek(0L, SeekOrigin.Begin);
			_temporaryStream.CopyTo(memoryStream);
			if (CipherAES.Decrypt(memoryStream.ToArray(), out var plainData))
			{
				_temporaryStream.SetLength(0L);
				_temporaryStream.Write(plainData, 0, plainData.Length);
				_temporaryStream.Seek(0L, SeekOrigin.Begin);
				return;
			}
			throw new WebException("Decrypt Failed.", WebExceptionStatus.ReceiveFailure);
		}

		private static uint Adler32(byte[] data)
		{
			uint num = 1u;
			uint num2 = 0u;
			int num3 = data.Length;
			for (int i = 0; i < num3; i++)
			{
				num = (num + data[i]) % 65521u;
				num2 = (num2 + num) % 65521u;
			}
			return (num2 << 16) + num;
		}

		public static bool CheckServerHash(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			try
			{
				string certHashString = certificate.GetCertHashString();
				for (int i = 0; i < thumbBytes.Length; i++)
				{
					thumbBytes[i] = Convert.ToByte(certHashString.Substring(i * 2, 2), 16);
				}
				if (-1 != certificate.Subject.IndexOf("CN=*.sic-rd1.jp"))
				{
					byte[][] array = hostBytes;
					for (int j = 0; j < array.Length; j++)
					{
						if (CheckServerParameter(array[j], thumbBytes))
						{
							return true;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return false;
		}

		static NetHttpClient()
		{
			hostBytes = null;
			isTrueDll = false;
			thumbBytes = new byte[20];
		}
	}
}
