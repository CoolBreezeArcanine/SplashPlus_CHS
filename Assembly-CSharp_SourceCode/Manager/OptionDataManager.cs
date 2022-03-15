using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using AMDaemon;
using MAI2.Util;
using UnityEngine;
using Util;

namespace Manager
{
	public class OptionDataManager : Singleton<OptionDataManager>
	{
		private List<string> _targetDirs = new List<string>();

		private Safe.ReadonlySortedDictionary<string, string> _movies = CreateDummyFileTable<string>();

		private Safe.ReadonlySortedDictionary<string, string> _sounds = CreateDummyFileTable<string>();

		private Safe.ReadonlySortedDictionary<string, string> _abs = CreateDummyFileTable<string>();

		private const string AbFolderName = "AssetBundleImages/";

		private bool _initialized;

		public void CheckStreamingAssets()
		{
			if (!_initialized)
			{
				Singleton<DataManager>.Instance.GetDirs(Application.streamingAssetsPath, AppImage.OptionMountRootPath, ref _targetDirs);
				ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
				CheckMovie(dirs, ref _movies);
				CheckSound(dirs, ref _sounds);
				CheckAssetBundle(dirs, ref _abs);
				_initialized = true;
			}
		}

		private void CheckMovie(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<string, string> movies)
		{
			SortedDictionary<string, string> outSerializes = new SortedDictionary<string, string>();
			CheckData(dirs, "MovieData", "*.dat", ref outSerializes);
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			foreach (KeyValuePair<string, string> item in outSerializes)
			{
				sortedDictionary[item.Key] = item.Value;
			}
			movies = new Safe.ReadonlySortedDictionary<string, string>(sortedDictionary);
		}

		private void CheckSound(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<string, string> sounds)
		{
			SortedDictionary<string, string> outSerializes = new SortedDictionary<string, string>();
			CheckData(dirs, "SoundData", "Mai2.acf", ref outSerializes);
			CheckData(dirs, "SoundData", "*.awb", ref outSerializes);
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			foreach (KeyValuePair<string, string> item in outSerializes)
			{
				sortedDictionary[item.Key] = item.Value;
			}
			sounds = new Safe.ReadonlySortedDictionary<string, string>(sortedDictionary);
		}

		private void CheckAssetBundle(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<string, string> abs)
		{
			SortedDictionary<string, string> outSerializes = new SortedDictionary<string, string>();
			CheckAssetBundleData(dirs, "AssetBundleImages", "*.ab", ref outSerializes);
			CheckAssetBundleData(dirs, "AssetBundleImages", "AssetBundleImages", ref outSerializes);
			SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
			foreach (KeyValuePair<string, string> item in outSerializes)
			{
				sortedDictionary[item.Key] = item.Value;
			}
			abs = new Safe.ReadonlySortedDictionary<string, string>(sortedDictionary);
		}

		public string GetMovieDataPath(string id)
		{
			try
			{
				return _movies[id];
			}
			catch
			{
				return _movies["dummy"];
			}
		}

		public string GetSoundDataPath(string id)
		{
			try
			{
				return _sounds[id];
			}
			catch
			{
				return _sounds["music000008"];
			}
		}

		public string GetAssetBundlePath(string id)
		{
			try
			{
				return _abs[id];
			}
			catch
			{
				return null;
			}
		}

		private static Safe.ReadonlySortedDictionary<string, T> CreateDummyFileTable<T>()
		{
			return new Safe.ReadonlySortedDictionary<string, T>(new SortedDictionary<string, T>());
		}

		private bool CheckData(ReadOnlyCollection<string> dirs, string subDir, string fileName, ref SortedDictionary<string, string> outSerializes)
		{
			bool flag = false;
			try
			{
				foreach (string dir in dirs)
				{
					string path = dir + "/" + subDir + "/";
					if (Directory.Exists(path))
					{
						string[] files = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
						foreach (string path2 in files)
						{
							string directoryName = Path.GetDirectoryName(path2);
							directoryName += "/";
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path2);
							directoryName = directoryName.Replace("\\", "/");
							outSerializes[fileNameWithoutExtension] = directoryName + fileNameWithoutExtension;
						}
					}
				}
			}
			catch
			{
				flag = true;
			}
			if (flag)
			{
				return false;
			}
			return true;
		}

		private bool CheckAssetBundleData(ReadOnlyCollection<string> dirs, string subDir, string fileName, ref SortedDictionary<string, string> outSerializes)
		{
			bool flag = false;
			try
			{
				foreach (string dir in dirs)
				{
					string path = dir + "/" + subDir + "/";
					if (Directory.Exists(path))
					{
						string[] files = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
						foreach (string obj in files)
						{
							string directoryName = Path.GetDirectoryName(obj);
							directoryName += "/";
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(obj);
							string extension = Path.GetExtension(obj);
							string text = obj;
							int count = text.IndexOf("AssetBundleImages/", StringComparison.Ordinal) + "AssetBundleImages/".Length;
							text = text.Remove(0, count);
							directoryName = directoryName.Replace("\\", "/");
							text = text.Replace("\\", "/");
							outSerializes[text] = directoryName + fileNameWithoutExtension + extension;
						}
					}
				}
			}
			catch
			{
				flag = true;
			}
			if (flag)
			{
				return false;
			}
			return true;
		}
	}
}
