using System.Collections.Generic;
using System.IO;
using DB;
using IO;
using MAI2System;
using UnityEngine;

namespace Manager
{
	public class WebCamManager
	{
		public const int GamePhotoMax = 1;

		private static WebCamManager _instance;

		public string Message = "";

		private static readonly GamePhotoConteiner[] gamePhotoBuf = new GamePhotoConteiner[1];

		private static Texture2D _dummyTexture;

		private static readonly List<int> gamePhotoNum = new List<int>();

		public static bool IsReady = false;

		public static string StatusMessage => _instance.Message;

		public static bool IsAvailableCamera()
		{
			return CameraManager.IsAvailableCamera;
		}

		public static void Initialize(MonoBehaviour behaviour)
		{
			if (_instance == null)
			{
				_instance = new WebCamManager();
				_dummyTexture = Resources.Load<Sprite>("Common/Sprites/System/NoPhotoImage").texture;
			}
			_instance.CameraInitialize();
		}

		public static void Reset()
		{
			IsReady = false;
			_ = _instance;
		}

		private void CameraInitialize()
		{
			string dataPath = GetDataPath();
			if (!Directory.Exists(dataPath))
			{
				Directory.CreateDirectory(dataPath);
			}
			string uploadDataPath = GetUploadDataPath();
			if (!Directory.Exists(uploadDataPath))
			{
				Directory.CreateDirectory(uploadDataPath);
			}
			for (int i = 0; i < 1; i++)
			{
				gamePhotoBuf[i] = new GamePhotoConteiner(CameraManager.GameCameraParam.Height, CameraManager.GameCameraParam.Width);
			}
			IsReady = true;
		}

		public static string GetDataPath()
		{
			return MAI2System.Path.PhotoPath;
		}

		public static string GetUploadDataPath()
		{
			return MAI2System.Path.UploadPath;
		}

		public static WebCamTexture GetTexture()
		{
			return CameraManager.GetTexture(CameraManager.CameraTypeEnum.Photo);
		}

		public static bool IsPlaying()
		{
			if (CameraManager.IsAvailableCamera)
			{
				return GetTexture().isPlaying;
			}
			return false;
		}

		public static void Play()
		{
			if (CameraManager.IsAvailableCamera)
			{
				GetTexture().Play();
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_circle, on: true);
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_red, on: true);
			}
		}

		public static void PlayOnly()
		{
			if (CameraManager.IsAvailableCamera)
			{
				GetTexture().Play();
				GetTexture().Pause();
			}
		}

		public static void Pause()
		{
			if (CameraManager.IsAvailableCamera)
			{
				GetTexture().Pause();
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_circle, on: false);
				MechaManager.Jvs.SetOutput(JvsOutputID.camera_led_red, on: false);
			}
		}

		public static void Stop()
		{
			if (CameraManager.IsAvailableCamera)
			{
				GetTexture().Stop();
				MechaManager.Jvs?.SetOutput(JvsOutputID.camera_led_circle, on: false);
				MechaManager.Jvs?.SetOutput(JvsOutputID.camera_led_red, on: false);
			}
		}

		public static Color32[] GetColor32()
		{
			return _instance.Get();
		}

		private Color32[] Get()
		{
			if (CameraManager.IsAvailableCamera)
			{
				return CameraManager.GetColor32(CameraManager.CameraTypeEnum.Photo);
			}
			return null;
		}

		public static void ClearGamePhotoBuffer()
		{
			if (CameraManager.IsAvailableCamera)
			{
				for (int i = 0; i < 1; i++)
				{
					gamePhotoBuf[i].ClearBuffer();
				}
			}
		}

		public static int GetTakePictureNum(int track)
		{
			if (gamePhotoNum.Count <= track)
			{
				return 0;
			}
			if (gamePhotoNum[track] == 0)
			{
				return 0;
			}
			return gamePhotoNum[track];
		}

		public static bool TakeGamePicture(int index)
		{
			if (index >= 1)
			{
				return false;
			}
			if (GetTexture() != null && IsPlaying() && GetTexture().didUpdateThisFrame)
			{
				gamePhotoBuf[index].CopyColor(GetTexture());
				return true;
			}
			return false;
		}

		public static string GetPhotoPath(int trackNumber, int photoIndex)
		{
			return System.IO.Path.Combine(GetDataPath(), "UserGamePhoto_" + trackNumber + "_" + photoIndex + ".jpg");
		}

		public static void OutputPhotoData()
		{
			if (!CameraManager.IsAvailableCamera)
			{
				return;
			}
			gamePhotoNum.Add(0);
			for (int i = 0; i < 1; i++)
			{
				if (!gamePhotoBuf[i].Enable)
				{
					continue;
				}
				gamePhotoNum[gamePhotoNum.Count - 1]++;
				Texture2D texture2D = new Texture2D(CameraManager.GameCameraParam.Width, CameraManager.GameCameraParam.Height, TextureFormat.ARGB32, mipChain: false);
				texture2D.SetPixels32(gamePhotoBuf[i].Colors);
				texture2D.Apply();
				byte[] buffer = texture2D.EncodeToJPG(100);
				using FileStream output = new FileStream(GetPhotoPath((int)GameManager.MusicTrackNumber, i), FileMode.Create, FileAccess.Write);
				using BinaryWriter binaryWriter = new BinaryWriter(output);
				binaryWriter.Write(buffer);
			}
		}

		public static Texture2D GetTakePicture(int trackNumber, int photoIndex)
		{
			string photoPath = MAI2System.Path.PhotoPath;
			string photoPath2 = GetPhotoPath(trackNumber + 1, photoIndex);
			if (Directory.Exists(photoPath) && File.Exists(photoPath2))
			{
				Texture2D texture2D = null;
				using FileStream fileStream = new FileStream(photoPath2, FileMode.Open, FileAccess.Read);
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					byte[] data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
					texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, mipChain: false);
					texture2D.LoadImage(data);
					binaryReader.Close();
				}
				fileStream.Close();
				return texture2D;
			}
			return _dummyTexture;
		}

		public static void ClearnUpFolder()
		{
			gamePhotoNum.Clear();
			ClearnUpFolder(GetDataPath());
		}

		private static void ClearnUpFolder(string dataPath)
		{
			if (Directory.Exists(dataPath))
			{
				string[] files = Directory.GetFiles(dataPath);
				foreach (string path in files)
				{
					File.SetAttributes(path, FileAttributes.Normal);
					File.Delete(path);
				}
				files = Directory.GetDirectories(dataPath);
				for (int i = 0; i < files.Length; i++)
				{
					ClearnUpFolder(files[i]);
				}
			}
		}
	}
}
