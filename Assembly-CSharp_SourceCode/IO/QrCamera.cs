using System;
using System.Collections.Generic;
using DB;
using MAI2.Util;
using MAI2System;
using Manager;
using UnityEngine;
using ZXing;
using ZXing.Common;
using ZXing.Multi.QrCode;
using ZXing.QrCode;

namespace IO
{
	public class QrCamera
	{
		private enum BlockEnum
		{
			IdentifyLeft,
			IdentifyRight,
			Left,
			Right,
			End
		}

		private enum QrCardEnum
		{
			Normal,
			Promotion,
			End
		}

		[Serializable]
		public struct Rectangle
		{
			public int X;

			public int Y;

			public int Width;

			public int Height;

			public Rectangle(int x, int y, int w, int h)
			{
				X = x;
				Y = y;
				Width = w;
				Height = h;
			}
		}

		[Serializable]
		public struct Block
		{
			public int TargetDevice;

			public Rectangle Rect;

			public Color32LuminanceSource SourceQr;

			private Result _result;

			public Result Result
			{
				get
				{
					return _result;
				}
				set
				{
					_result = value;
				}
			}
		}

		public const int MaxCameraNum = 2;

		public const int InvalidCameraIndex = -1;

		private static readonly Dictionary<DecodeHintType, object> DecodeHints;

		private static Color32[] _colors;

		private static Color32LuminanceSource _sourceCameraID;

		private QRCodeReader _qrcodeReader;

		private static QRCodeMultiReader _qrcodeMultiReader;

		private const int TotalBytes = 14;

		private readonly byte[] _buffer = new byte[14];

		public static bool[] DummyInsert;

		public static string[] DummyPromotionCode;

		public static string[] DummyPromotionSubCode;

		public static string[] DummyCardID;

		private static readonly int QrWidth;

		private static readonly int QrHeight;

		private static readonly Rectangle RectQrCard;

		private readonly Block[] _block = new Block[2];

		private bool _initialized;

		public static bool IsReady;

		public string message = "";

		public const int Check_NoData = 0;

		public const int Check_Exists = 1;

		public const int Check_Empty = 2;

		public const int Check_Error = 3;

		public const int Check_InvalidExists = 4;

		public string StatusMessage => message;

		public bool IsAvailableCamera(int index)
		{
			if (CameraManager.IsAvailableCameras != null)
			{
				return CameraManager.IsAvailableCameras[index];
			}
			return false;
		}

		private static byte RGBToGray(Color32 color)
		{
			return (byte)((float)(int)color.r * 0.299f + (float)(int)color.g * 0.587f + (float)(int)color.b * 0.114f);
		}

		public bool Exists(int index)
		{
			if (CameraManager.DeviceId[index] == -1)
			{
				return false;
			}
			return CameraManager.IsAvailableCameras[index];
		}

		public bool Warnning(int index)
		{
			if (CameraManager.CameraProcMode == null)
			{
				return false;
			}
			return CameraManager.CameraProcMode[index] == CameraManager.CameraProcEnum.Warn;
		}

		public bool MayBeCodeCamera(int index)
		{
			if (CameraManager.DeviceId[index] == -1)
			{
				return false;
			}
			if (CameraManager.CameraTypes == null)
			{
				return false;
			}
			return CameraManager.CameraTypes[CameraManager.DeviceId[index]] == CameraManager.CameraTypeEnum.MayBeQR;
		}

		static QrCamera()
		{
			DecodeHints = new Dictionary<DecodeHintType, object>
			{
				{
					DecodeHintType.TRY_HARDER,
					true
				},
				{
					DecodeHintType.POSSIBLE_FORMATS,
					new object[1] { BarcodeFormat.QR_CODE }
				},
				{
					DecodeHintType.CHARACTER_SET,
					"ISO-8859-1"
				}
			};
			_colors = new Color32[CameraManager.QrCameraParam.Height * CameraManager.QrCameraParam.Width];
			_sourceCameraID = new Color32LuminanceSource(CameraManager.QrCameraParam.Width, CameraManager.QrCameraParam.Height);
			DummyInsert = new bool[2];
			DummyPromotionCode = new string[2] { "0", "0" };
			DummyPromotionSubCode = new string[2] { "0", "0" };
			DummyCardID = new string[2] { "0", "0" };
			QrWidth = 340;
			QrHeight = CameraManager.QrCameraParam.Height;
			RectQrCard = new Rectangle(CameraManager.QrCameraParam.Width - QrWidth, 0, QrWidth, QrHeight);
			IsReady = false;
		}

		public void Initialize(MonoBehaviour behaviour)
		{
			_initialized = false;
			IsReady = false;
			_qrcodeReader = new QRCodeReader();
			for (int i = 0; i < 2; i++)
			{
				_block[i].Rect = RectQrCard;
				_block[i].SourceQr = new Color32LuminanceSource(_block[i].Rect.Width, _block[i].Rect.Height);
			}
			_initialized = true;
		}

		public void Terminate()
		{
			_sourceCameraID = null;
			_qrcodeReader = null;
			_qrcodeMultiReader = null;
		}

		public void Update()
		{
			if (!_initialized)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (CameraManager.IsAvailableCameras[i] && CameraManager.GetTexture((CameraManager.CameraTypeEnum)i).didUpdateThisFrame)
				{
					CameraManager.GetTexture((CameraManager.CameraTypeEnum)i).GetPixels32(_colors);
					Decode(i, CameraManager.GetTexture((CameraManager.CameraTypeEnum)i).width);
				}
			}
		}

		public bool Play()
		{
			for (int i = 0; i < 2; i++)
			{
				if (CameraManager.IsAvailableCameras[i])
				{
					CameraManager.GetTexture((CameraManager.CameraTypeEnum)i).Play();
					_block[i].Result = null;
				}
				LedBlockID.QrLed_1P.SetColor(Color.white);
				LedBlockID.QrLed_2P.SetColor(Color.white);
			}
			return true;
		}

		public void Pause()
		{
			for (int i = 0; i < 2; i++)
			{
				if (CameraManager.IsAvailableCameras[i])
				{
					CameraManager.GetTexture((CameraManager.CameraTypeEnum)i).Pause();
				}
			}
			LedBlockID.QrLed_1P.SetColor(Color.black);
			LedBlockID.QrLed_2P.SetColor(Color.black);
		}

		public void Stop()
		{
			for (int i = 0; i < 2; i++)
			{
				if (CameraManager.IsAvailableCameras[i])
				{
					CameraManager.GetTexture((CameraManager.CameraTypeEnum)i).Stop();
				}
			}
			LedBlockID.QrLed_1P.SetColor(Color.black);
			LedBlockID.QrLed_2P.SetColor(Color.black);
		}

		public static int IdentCheckDecode(WebCamTexture texture)
		{
			if (_qrcodeMultiReader == null)
			{
				_qrcodeMultiReader = new QRCodeMultiReader();
			}
			texture.GetPixels32(_colors);
			_sourceCameraID.SetPixels(_colors);
			BinaryBitmap image = new BinaryBitmap(new HybridBinarizer(_sourceCameraID));
			Result[] array = _qrcodeMultiReader.decodeMultiple(image, DecodeHints);
			if (array == null)
			{
				return -1;
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Text;
				if (!string.IsNullOrEmpty(text) && text.StartsWith("SDEZ"))
				{
					text = text.Substring(4);
					int result = -1;
					if (int.TryParse(text, out result))
					{
						return Mathf.Max(0, result - 1);
					}
				}
			}
			return -1;
		}

		private void Decode(int blockIndex, int width)
		{
			try
			{
				for (int i = 0; i < _block[blockIndex].Rect.Height; i++)
				{
					int num = (_block[blockIndex].Rect.Y + i) * width;
					int num2 = i * _block[blockIndex].Rect.Width;
					for (int j = 0; j < _block[blockIndex].Rect.Width; j++)
					{
						int num3 = _block[blockIndex].Rect.X + j;
						_block[blockIndex].SourceQr.Matrix[num2 + j] = RGBToGray(_colors[num + num3]);
					}
				}
				BinaryBitmap image = new BinaryBitmap(new HybridBinarizer(_block[blockIndex].SourceQr));
				_block[blockIndex].Result = _qrcodeReader.decode(image, DecodeHints);
				_qrcodeReader.reset();
			}
			catch (Exception)
			{
			}
		}

		private static int Decode(byte[] bytes, int start, int end)
		{
			int num = 0;
			int num2 = end - 1;
			while (start <= num2)
			{
				num <<= 8;
				num |= bytes[num2];
				num2--;
			}
			return num;
		}

		public int TryTestModeParse(uint gameId, int index)
		{
			if (_block[index].Result == null)
			{
				return 0;
			}
			object value = null;
			if (!_block[index].Result.ResultMetadata.TryGetValue(ResultMetadataType.BYTE_SEGMENTS, out value))
			{
				return 2;
			}
			List<byte[]> list = value as List<byte[]>;
			if (list.Count <= 0 || list[0].Length < 14)
			{
				return 3;
			}
			Array.Copy(list[0], _buffer, 14);
			QRImage.decrypt(_buffer);
			if (Decode(_buffer, 11, 14) != (int)gameId)
			{
				return 4;
			}
			return 1;
		}

		public bool TryParse(out int cardKind, out int promotionCode, out int cardId, out int param, out uint gameId, int index)
		{
			cardKind = 2;
			promotionCode = 0;
			cardId = 0;
			param = 0;
			gameId = 0u;
			if (Singleton<SystemConfig>.Instance.config.IsDummyQrCamera)
			{
				if (!DummyInsert[index])
				{
					return false;
				}
				promotionCode = int.Parse(DummyPromotionCode[index]);
				cardId = int.Parse(DummyCardID[index]);
				param = int.Parse(DummyPromotionSubCode[index]);
			}
			else
			{
				if (_block[index].Result == null)
				{
					return false;
				}
				object value = null;
				if (!_block[index].Result.ResultMetadata.TryGetValue(ResultMetadataType.BYTE_SEGMENTS, out value))
				{
					return false;
				}
				List<byte[]> list = value as List<byte[]>;
				if (list.Count <= 0 || list[0].Length < 14)
				{
					return false;
				}
				Array.Copy(list[0], _buffer, 14);
				QRImage.decrypt(_buffer);
				promotionCode = Decode(_buffer, 0, 4);
				cardId = Decode(_buffer, 4, 7);
				param = Decode(_buffer, 7, 11);
				gameId = (uint)Decode(_buffer, 11, 14);
			}
			cardKind = ((promotionCode != 0) ? 1 : 0);
			return true;
		}
	}
}
