using System.Collections;
using System.Diagnostics;
using AMDaemon;
using DB;
using IO;
using MAI2.Util;
using MAI2System;
using UnityEngine;

namespace Manager
{
	public class CameraManager
	{
		public enum CameraTypeEnum
		{
			QRLeft,
			QRRight,
			Photo,
			None,
			Unknown,
			MayBeQR,
			End
		}

		public enum CameraProcEnum
		{
			None,
			Bad,
			Good,
			Warn,
			End
		}

		public const int MaxCameraNum = 3;

		private const int CamTimeOut = 5000;

		public const float CamLedFlashWait = 3f;

		public static readonly CameraParameter GameCameraParam = new CameraParameter(1280, 960, 30);

		public static readonly CameraParameter QrCameraParam = new CameraParameter(640, 480, 5);

		private static CameraManager _instance;

		public string Message = "";

		private WebCamTexture[] _webcamtex = new WebCamTexture[3];

		private Color32[] _colors;

		private CameraTypeEnum[] _cameraTypes = new CameraTypeEnum[3];

		public static bool IsReady = false;

		public bool[] isAvailableCamera = new bool[3];

		public CameraProcEnum[] cameraProcMode = new CameraProcEnum[3];

		public static string StatusMessage => _instance.Message;

		public static int[] DeviceId { get; private set; } = new int[3];


		public static string[] DeviceName { get; private set; } = new string[3];


		public static bool IsAvailableCamera
		{
			get
			{
				if (_instance != null)
				{
					return _instance.isAvailableCamera[2];
				}
				return false;
			}
		}

		public static bool IsAvailableLeftQrCamera
		{
			get
			{
				if (_instance != null)
				{
					return _instance.isAvailableCamera[0];
				}
				return false;
			}
		}

		public static bool IsAvailableRightQrCamera
		{
			get
			{
				if (_instance != null)
				{
					return _instance.isAvailableCamera[1];
				}
				return false;
			}
		}

		public static bool[] IsAvailableCameras => _instance?.isAvailableCamera;

		public static CameraProcEnum[] CameraProcMode => _instance?.cameraProcMode;

		public static CameraTypeEnum[] CameraTypes => _instance?._cameraTypes;

		public static int ErrorNo
		{
			get
			{
				if (_instance == null)
				{
					return 0;
				}
				int num = 0;
				for (int i = 0; i < _instance._webcamtex.Length; i++)
				{
					if (null != _instance._webcamtex[i])
					{
						num++;
					}
				}
				if (!Singleton<SystemConfig>.Instance.config.IsDummyQrCamera && !Singleton<SystemConfig>.Instance.config.IsDummyPhotoCamera && num < _instance._webcamtex.Length)
				{
					return 3100;
				}
				if (!Singleton<SystemConfig>.Instance.config.IsDummyQrCamera && (!IsAvailableLeftQrCamera || !IsAvailableRightQrCamera))
				{
					Error.Set(3101);
				}
				if (!Singleton<SystemConfig>.Instance.config.IsDummyPhotoCamera && !IsAvailableCamera)
				{
					Error.Set(3102);
				}
				return 0;
			}
		}

		public static int GetDevicesCount()
		{
			if (_instance == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				if (_instance._webcamtex[i] != null)
				{
					num++;
				}
			}
			return num;
		}

		public static bool Exists(int index)
		{
			if (_instance == null)
			{
				return false;
			}
			return _instance._webcamtex[index] != null;
		}

		public static void Initialize(MonoBehaviour behaviour)
		{
			if (_instance == null)
			{
				_instance = new CameraManager();
			}
			Reset();
			QrLedSwitch(on: true);
			behaviour.StopCoroutine(_instance.CameraInitialize());
			behaviour.StartCoroutine(_instance.CameraInitialize());
		}

		private static void QrLedSwitch(bool on)
		{
			if (on)
			{
				LedBlockID.QrLed_1P.SetColor(Color.white);
				LedBlockID.QrLed_2P.SetColor(Color.white);
			}
			else
			{
				LedBlockID.QrLed_1P.SetColor(Color.black);
				LedBlockID.QrLed_2P.SetColor(Color.black);
			}
		}

		public static void Reset()
		{
			IsReady = false;
			if (_instance == null)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				WebCamTexture webCamTexture = _instance._webcamtex[i];
				if (null != webCamTexture)
				{
					webCamTexture.Stop();
					_instance._webcamtex[i] = null;
				}
				DeviceId[i] = -1;
				DeviceName[i] = "";
				_instance._cameraTypes[i] = CameraTypeEnum.None;
				_instance.isAvailableCamera[i] = false;
				_instance.cameraProcMode[i] = CameraProcEnum.None;
			}
		}

		public void Terminate()
		{
			Reset();
		}

		private IEnumerator CameraInitialize()
		{
			Message = "カメラを探しています";
			if (WebCamTexture.devices.Length == 0)
			{
				Message = "接続されているカメラが見つかりませんでした";
				IsReady = true;
				QrLedSwitch(on: false);
				yield break;
			}
			if (WebCamTexture.devices.Length > 3)
			{
				Message = "接続されているカメラが多すぎます";
				IsReady = true;
				QrLedSwitch(on: false);
				yield break;
			}
			yield return new WaitForSeconds(3f);
			int camIndex = 0;
			while (camIndex < WebCamTexture.devices.Length)
			{
				WebCamDevice webCamDevice = WebCamTexture.devices[camIndex];
				DeviceName[camIndex] = webCamDevice.name;
				Message = "カメラチェック:" + camIndex;
				_webcamtex[camIndex] = new WebCamTexture(webCamDevice.name, GameCameraParam.Width, GameCameraParam.Height, GameCameraParam.Fps);
				_webcamtex[camIndex].Play();
				Stopwatch timer = new Stopwatch();
				bool isCamTimeOut2 = false;
				timer.Restart();
				while (!_webcamtex[camIndex].isPlaying)
				{
					if (timer.ElapsedMilliseconds < 5000)
					{
						yield return new WaitForSeconds(1f);
						continue;
					}
					isCamTimeOut2 = true;
					break;
				}
				_webcamtex[camIndex].Stop();
				if (isCamTimeOut2)
				{
					Message = "カメラは見つけたけど再生に失敗";
					_instance._cameraTypes[camIndex] = CameraTypeEnum.Unknown;
				}
				else if (_webcamtex[camIndex].width <= QrCameraParam.Width)
				{
					if (null != _webcamtex[camIndex])
					{
						_webcamtex[camIndex].Stop();
						_webcamtex[camIndex] = null;
					}
					_webcamtex[camIndex] = new WebCamTexture(WebCamTexture.devices[camIndex].name, QrCameraParam.Width, QrCameraParam.Height, QrCameraParam.Fps);
					WebCamTexture wcamTex = _webcamtex[camIndex];
					wcamTex.Stop();
					wcamTex.Play();
					timer.Reset();
					timer.Start();
					isCamTimeOut2 = false;
					while (!wcamTex.isPlaying)
					{
						if (timer.ElapsedMilliseconds < 5000)
						{
							yield return new WaitForSeconds(1f);
							continue;
						}
						isCamTimeOut2 = true;
						break;
					}
					if (isCamTimeOut2)
					{
						Message = "カメラは見つけたけど再生に失敗";
						wcamTex.Stop();
						_instance._cameraTypes[camIndex] = CameraTypeEnum.Unknown;
					}
					else
					{
						timer.Reset();
						timer.Start();
						int DecodeTimeoutSec = 5000;
						int findQrCamera = -1;
						while (timer.ElapsedMilliseconds < DecodeTimeoutSec)
						{
							if (wcamTex.didUpdateThisFrame)
							{
								wcamTex.GetPixels32(_colors);
								findQrCamera = QrCamera.IdentCheckDecode(wcamTex);
								if (findQrCamera != -1)
								{
									break;
								}
							}
							yield return null;
						}
						wcamTex.Stop();
						if (-1 != findQrCamera)
						{
							_cameraTypes[camIndex] = ((findQrCamera != 0) ? CameraTypeEnum.QRRight : CameraTypeEnum.QRLeft);
							DeviceId[(int)_cameraTypes[camIndex]] = camIndex;
							isAvailableCamera[(int)_cameraTypes[camIndex]] = true;
							cameraProcMode[(int)_cameraTypes[camIndex]] = CameraProcEnum.Good;
						}
						else
						{
							_cameraTypes[camIndex] = CameraTypeEnum.MayBeQR;
						}
					}
				}
				else
				{
					_instance._cameraTypes[camIndex] = CameraTypeEnum.Photo;
					isAvailableCamera[(int)_cameraTypes[camIndex]] = true;
					cameraProcMode[(int)_cameraTypes[camIndex]] = CameraProcEnum.Good;
					DeviceId[2] = camIndex;
				}
				int num = camIndex + 1;
				camIndex = num;
			}
			if (DeviceId[0] == -1 && DeviceId[1] == -1)
			{
				int num2 = 0;
				for (int i = 0; i < WebCamTexture.devices.Length; i++)
				{
					if (_instance._cameraTypes[i] == CameraTypeEnum.Unknown || _instance._cameraTypes[i] == CameraTypeEnum.MayBeQR)
					{
						DeviceId[num2] = i;
						num2++;
					}
				}
			}
			else if (DeviceId[0] == -1)
			{
				for (int j = 0; j < WebCamTexture.devices.Length; j++)
				{
					if (_instance._cameraTypes[j] == CameraTypeEnum.MayBeQR)
					{
						_instance._cameraTypes[j] = CameraTypeEnum.QRLeft;
						DeviceId[0] = j;
						isAvailableCamera[(int)_cameraTypes[j]] = true;
						cameraProcMode[(int)_cameraTypes[j]] = CameraProcEnum.Warn;
						break;
					}
					if (_instance._cameraTypes[j] == CameraTypeEnum.Unknown)
					{
						_instance._cameraTypes[j] = CameraTypeEnum.QRLeft;
						DeviceId[0] = j;
						break;
					}
				}
			}
			else if (DeviceId[1] == -1)
			{
				for (int k = 0; k < WebCamTexture.devices.Length; k++)
				{
					if (_instance._cameraTypes[k] == CameraTypeEnum.MayBeQR)
					{
						_instance._cameraTypes[k] = CameraTypeEnum.QRRight;
						DeviceId[1] = k;
						isAvailableCamera[(int)_cameraTypes[k]] = true;
						cameraProcMode[(int)_cameraTypes[k]] = CameraProcEnum.Warn;
						break;
					}
					if (_instance._cameraTypes[k] == CameraTypeEnum.Unknown)
					{
						_instance._cameraTypes[k] = CameraTypeEnum.QRRight;
						DeviceId[1] = k;
						break;
					}
				}
			}
			IsReady = true;
			QrLedSwitch(on: false);
		}

		public static WebCamTexture GetTexture(CameraTypeEnum type)
		{
			if ((int)type > _instance._webcamtex.Length)
			{
				return null;
			}
			if (DeviceId[(int)type] == -1)
			{
				return null;
			}
			return _instance._webcamtex[DeviceId[(int)type]];
		}

		public static bool IsPlayingPhotoCamera()
		{
			if (IsAvailableCamera)
			{
				return GetTexture(CameraTypeEnum.Photo).isPlaying;
			}
			return false;
		}

		public static void PlayPhotoCamera()
		{
			if (IsAvailableCamera)
			{
				GetTexture(CameraTypeEnum.Photo).Play();
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_circle, on: true);
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_red, on: true);
			}
		}

		public static void PlayPhotoOnly()
		{
			if (IsAvailableCamera)
			{
				GetTexture(CameraTypeEnum.Photo).Play();
				GetTexture(CameraTypeEnum.Photo).Pause();
			}
		}

		public static void PausePhoto()
		{
			if (IsAvailableCamera)
			{
				GetTexture(CameraTypeEnum.Photo).Pause();
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_circle, on: false);
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_red, on: false);
			}
		}

		public static void StopPhoto()
		{
			if (IsAvailableCamera)
			{
				GetTexture(CameraTypeEnum.Photo).Stop();
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_circle, on: false);
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_red, on: false);
			}
		}

		public static Color32[] GetColor32(CameraTypeEnum type)
		{
			return _instance.Get(type);
		}

		private Color32[] Get(CameraTypeEnum type)
		{
			if ((int)type > _instance._webcamtex.Length)
			{
				return null;
			}
			if (GetTexture(type) != null)
			{
				_colors = GetTexture(type).GetPixels32();
				return _colors;
			}
			return null;
		}
	}
}
