using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using AMDaemon;
using MAI2.Util;
using MAI2System;
using Manager.MaiStudio;
using Manager.MaiStudio.Serialize;
using Net.VO.Mai2;
using UnityEngine;
using Util;

namespace Manager
{
	public class DataManager : Singleton<DataManager>
	{
		private const string _dataDirSearchName = "A???";

		private const string _dataDirSearchNameIsC = "C???";

		private bool _isSuccessSetupConfig;

		private Manager.MaiStudio.Config _config = new Manager.MaiStudio.Config();

		private List<Thread> _threads;

		private int _threaEndCount;

		private int _errorCount;

		private List<string> _targetDirs = new List<string>();

		private static string _defaultPath;

		private static string _optionPath;

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaData> _charas = CreateDummyTable<Manager.MaiStudio.CharaData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaAwakeData> _charaAwakes = CreateDummyTable<Manager.MaiStudio.CharaAwakeData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaGenreData> _charaGenres = CreateDummyTable<Manager.MaiStudio.CharaGenreData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.EventData> _events = CreateDummyTable<Manager.MaiStudio.EventData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicData> _musics = CreateDummyTable<Manager.MaiStudio.MusicData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGenreData> _musicGenres = CreateDummyTable<Manager.MaiStudio.MusicGenreData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGroupData> _musicGroups = CreateDummyTable<Manager.MaiStudio.MusicGroupData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicVersionData> _musicVersions = CreateDummyTable<Manager.MaiStudio.MusicVersionData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicNameSortData> _musicNameSorts = CreateDummyTable<Manager.MaiStudio.MusicNameSortData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicClearRankData> _musicClearRanks = CreateDummyTable<Manager.MaiStudio.MusicClearRankData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicDifficultyData> _musicDifficultys = CreateDummyTable<Manager.MaiStudio.MusicDifficultyData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicLevelData> _musicLevels = CreateDummyTable<Manager.MaiStudio.MusicLevelData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.OldMusicScoreData> _oldMusicScores = CreateDummyTable<Manager.MaiStudio.OldMusicScoreData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ScoreRankingData> _tournamentMusics = CreateDummyTable<Manager.MaiStudio.ScoreRankingData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseData> _courses = CreateDummyTable<Manager.MaiStudio.CourseData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseModeData> _courseModes = CreateDummyTable<Manager.MaiStudio.CourseModeData>();

		private GameRanking[] _musicOffRanking;

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.LoginBonusData> _loginBonuses = CreateDummyTable<Manager.MaiStudio.LoginBonusData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapData> _maps = CreateDummyTable<Manager.MaiStudio.MapData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapColorData> _mapColors = CreateDummyTable<Manager.MaiStudio.MapColorData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTreasureData> _mapTreasures = CreateDummyTable<Manager.MaiStudio.MapTreasureData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapBonusMusicData> _mapBonusMusics = CreateDummyTable<Manager.MaiStudio.MapBonusMusicData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapOtomodachiData> _mapOtomodachis = CreateDummyTable<Manager.MaiStudio.MapOtomodachiData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapSilhouetteData> _mapSilhouettes = CreateDummyTable<Manager.MaiStudio.MapSilhouetteData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTitleData> _mapTitles = CreateDummyTable<Manager.MaiStudio.MapTitleData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ItemMusicData> _itemMusics = CreateDummyTable<Manager.MaiStudio.ItemMusicData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.IconData> _icons = CreateDummyTable<Manager.MaiStudio.IconData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PlateData> _plates = CreateDummyTable<Manager.MaiStudio.PlateData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TitleData> _titles = CreateDummyTable<Manager.MaiStudio.TitleData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PartnerData> _partners = CreateDummyTable<Manager.MaiStudio.PartnerData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.FrameData> _frames = CreateDummyTable<Manager.MaiStudio.FrameData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TicketData> _tickets = CreateDummyTable<Manager.MaiStudio.TicketData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionTypeData> _collectionTypes = CreateDummyTable<Manager.MaiStudio.CollectionTypeData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionGenreData> _collectionGenres = CreateDummyTable<Manager.MaiStudio.CollectionGenreData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PhotoFrameData> _photoFrames = CreateDummyTable<Manager.MaiStudio.PhotoFrameData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.InformationData> _infomations = CreateDummyTable<Manager.MaiStudio.InformationData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeData> _udemaes = CreateDummyTable<Manager.MaiStudio.UdemaeData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ClassData> _classes = CreateDummyTable<Manager.MaiStudio.ClassData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeBossData> _udemaeBosses = CreateDummyTable<Manager.MaiStudio.UdemaeBossData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardData> _cards = CreateDummyTable<Manager.MaiStudio.CardData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardCharaData> _cardCharas = CreateDummyTable<Manager.MaiStudio.CardCharaData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardTypeData> _cardTypes = CreateDummyTable<Manager.MaiStudio.CardTypeData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.HolidayData> _weekdayBonuses = CreateDummyTable<Manager.MaiStudio.HolidayData>();

		private Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ChallengeData> _challenges = CreateDummyTable<Manager.MaiStudio.ChallengeData>();

		private const int InvalidId = -1;

		public void SetDirs(string dataPath, string optionPath)
		{
			_defaultPath = dataPath;
			_optionPath = optionPath;
		}

		public void SetupConfig()
		{
			Manager.MaiStudio.RomConfig romConfig = null;
			Manager.MaiStudio.DataConfig dataConfig = null;
			Manager.MaiStudio.SupportConfig support = null;
			if (Deserialize<Manager.MaiStudio.Serialize.RomConfig>(_defaultPath + "/RomConfig.xml", out var dsr))
			{
				romConfig = (Manager.MaiStudio.RomConfig)dsr;
			}
			if (romConfig != null)
			{
				List<string> list = new List<string>();
				list.Add("DataConfig.xml");
				List<string> filePaths = new List<string>();
				GetFilePaths(_defaultPath, _optionPath, list, ref filePaths);
				foreach (string item in filePaths)
				{
					if (Deserialize<Manager.MaiStudio.Serialize.DataConfig>(item, out var dsr2))
					{
						if (dataConfig == null)
						{
							dataConfig = (Manager.MaiStudio.DataConfig)dsr2;
						}
						else if (dsr2.version.GetCode() >= dataConfig.version.GetCode())
						{
							dataConfig = (Manager.MaiStudio.DataConfig)dsr2;
						}
					}
				}
			}
			_config = new Manager.MaiStudio.Config(romConfig, dataConfig, support);
			_isSuccessSetupConfig = false;
			if (romConfig != null && dataConfig != null)
			{
				_isSuccessSetupConfig = true;
			}
		}

		public bool IsSuccessSetupConfig()
		{
			return _isSuccessSetupConfig;
		}

		public Manager.MaiStudio.Config GetConfig()
		{
			return _config;
		}

		public void Load()
		{
			if (_threads != null)
			{
				return;
			}
			_errorCount = 0;
			_threaEndCount = 0;
			if (!GetDirs(_defaultPath, _optionPath, ref _targetDirs))
			{
				return;
			}
			try
			{
				_threads = new List<Thread>();
				AddLoadTask();
				foreach (Thread thread in _threads)
				{
					thread.Start();
				}
			}
			catch
			{
				Interlocked.Increment(ref _errorCount);
			}
		}

		public bool IsLoaded()
		{
			bool result = false;
			if (_threads != null && _threaEndCount >= _threads.Count)
			{
				result = true;
			}
			return result;
		}

		public float GetLoadedTaskPer()
		{
			if (_threads != null)
			{
				return (float)_threaEndCount / (float)_threads.Count;
			}
			return 1f;
		}

		public bool IsError()
		{
			bool result = false;
			if (_errorCount > 0)
			{
				result = true;
			}
			return result;
		}

		public void GetFilePaths(string streamingAssetsDir, string optDir, List<string> targets, ref List<string> filePaths)
		{
			filePaths.Clear();
			List<string> dirs = new List<string>();
			if (!GetDirs(streamingAssetsDir, optDir, ref dirs))
			{
				return;
			}
			foreach (string item in dirs)
			{
				foreach (string target in targets)
				{
					string text = item + "/" + target;
					if (File.Exists(text))
					{
						filePaths.Add(text);
					}
				}
			}
		}

		public bool GetDirs(string streamingAssetsDir, string optDir, ref List<string> dirs)
		{
			bool flag = false;
			dirs.Clear();
			try
			{
				if (Directory.Exists(streamingAssetsDir))
				{
					string[] directories = Directory.GetDirectories(streamingAssetsDir, "A???", SearchOption.TopDirectoryOnly);
					if (directories != null)
					{
						string[] array = directories;
						for (int i = 0; i < array.Length; i++)
						{
							string text = array[i].Replace("\\", "/");
							if (IsTargetDirName(text))
							{
								dirs.Add(text);
							}
						}
						dirs.Sort();
					}
				}
				if (Directory.Exists(optDir))
				{
					string[] directories2 = Directory.GetDirectories(optDir, "C???", SearchOption.TopDirectoryOnly);
					if (directories2 != null)
					{
						List<string> list = new List<string>();
						string[] array = directories2;
						for (int i = 0; i < array.Length; i++)
						{
							string text2 = array[i].Replace("\\", "/");
							if (IsTargetDirName(text2, isOpt: true))
							{
								list.Add(text2);
							}
						}
						list.Sort();
						dirs.AddRange(list);
					}
				}
				flag = true;
			}
			catch
			{
				flag = false;
			}
			if (dirs.Count == 0)
			{
				flag = false;
			}
			return flag;
		}

		private bool IsTargetDirName(string path, bool isOpt = false)
		{
			bool result = false;
			if (Directory.Exists(path))
			{
				string fileName = System.IO.Path.GetFileName(path);
				string text = "";
				text = (isOpt ? "C???" : "A???");
				if (fileName.Length >= 2 && text.Length == fileName.Length && text[0] == fileName[0] && int.TryParse(fileName.Substring(1), out var _))
				{
					result = true;
				}
			}
			return result;
		}

		private bool LoadSort(ReadOnlyCollection<string> dirs, string subDir, string fileName, ref Manager.MaiStudio.Serialize.SerializeSortData sortData)
		{
			bool flag = false;
			try
			{
				foreach (string dir in dirs)
				{
					string path = dir + "/" + subDir + "/";
					if (!Directory.Exists(path))
					{
						continue;
					}
					string[] files = Directory.GetFiles(path, fileName, SearchOption.TopDirectoryOnly);
					for (int i = 0; i < files.Length; i++)
					{
						if (Deserialize<Manager.MaiStudio.Serialize.SerializeSortData>(files[i], out var dsr))
						{
							sortData = dsr;
						}
						else
						{
							flag = true;
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
				Interlocked.Increment(ref _errorCount);
				return false;
			}
			return true;
		}

		private bool LoadData<T>(ReadOnlyCollection<string> dirs, string subDir, string fileName, ref SortedDictionary<int, T> outSerializes) where T : SerializeBase, ISerialize, new()
		{
			bool flag = false;
			try
			{
				foreach (string dir in dirs)
				{
					string path = dir + "/" + subDir + "/";
					if (!Directory.Exists(path))
					{
						continue;
					}
					string[] files = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
					foreach (string text in files)
					{
						if (Deserialize<T>(text, out var dsr))
						{
							string directoryName = System.IO.Path.GetDirectoryName(text);
							directoryName += "/";
							dsr.AddPath(directoryName.Replace("\\", "/"));
							outSerializes[dsr.GetID()] = dsr;
						}
						else
						{
							flag = true;
						}
					}
				}
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, T> outSerialize in outSerializes)
				{
					if (outSerialize.Value.IsDisable())
					{
						list.Add(outSerialize.Key);
					}
				}
				foreach (int item in list)
				{
					outSerializes.Remove(item);
				}
			}
			catch
			{
				flag = true;
			}
			if (flag)
			{
				Interlocked.Increment(ref _errorCount);
				return false;
			}
			return true;
		}

		private void SetPriority<T>(Manager.MaiStudio.Serialize.SerializeSortData sort, ref SortedDictionary<int, T> list) where T : ISerialize
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			int num = 0;
			foreach (Manager.MaiStudio.Serialize.StringID sort2 in sort.SortList)
			{
				Manager.MaiStudio.StringID stringID = (Manager.MaiStudio.StringID)sort2;
				dictionary[stringID.id] = num;
				num++;
			}
			foreach (KeyValuePair<int, T> item in list)
			{
				int priority = 0;
				if (dictionary.ContainsKey(item.Key))
				{
					priority = dictionary[item.Key];
				}
				item.Value.SetPriority(priority);
			}
		}

		private static bool Deserialize<T>(string filePath, out T dsr) where T : new()
		{
			bool flag = false;
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				xmlDocument.Load(filePath);
				if (xmlDocument.DocumentElement == null)
				{
					throw new Exception("doc.DocumentElement == null");
				}
				XmlNodeReader xmlReader = new XmlNodeReader(xmlDocument.DocumentElement);
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
				dsr = (T)xmlSerializer.Deserialize(xmlReader);
				return true;
			}
			catch (Exception)
			{
				dsr = new T();
				return false;
			}
		}

		private static Safe.ReadonlySortedDictionary<int, T> CreateDummyTable<T>()
		{
			return new Safe.ReadonlySortedDictionary<int, T>(new SortedDictionary<int, T>());
		}

		public void Terminate()
		{
			if (_threads == null)
			{
				return;
			}
			foreach (Thread thread in _threads)
			{
				if (thread.IsAlive)
				{
					thread.Abort();
				}
			}
		}

		public void NoteCreateLoad(int musicID, int difficultyID)
		{
			GameManager.StreamingAssetsPath = Application.streamingAssetsPath;
			GetDirs(Application.streamingAssetsPath, AppImage.OptionMountRootPath, ref _targetDirs);
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadMusicGenre(dirs, ref _musicGenres);
			LoadMusicSingle(dirs, ref _musics, musicID);
			LoadUdemae(dirs, ref _udemaes);
			LoadMusicClearRank(dirs, ref _musicClearRanks);
			_threads = new List<Thread>();
		}

		public void LoadMovieCheckData()
		{
			GameManager.StreamingAssetsPath = Application.streamingAssetsPath;
			GetDirs(Application.streamingAssetsPath, AppImage.OptionMountRootPath, ref _targetDirs);
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadMusicGenre(dirs, ref _musicGenres);
			LoadMusicSingle(dirs, ref _musics);
			_threads = new List<Thread>();
		}

		private void AddLoadTask()
		{
			_threads.Add(new Thread(LoadMusic));
			_threads.Add(new Thread(LoadMusicBase));
			_threads.Add(new Thread(LoadTaskChara));
			_threads.Add(new Thread(LoadTaskMap));
			_threads.Add(new Thread(LoadItem));
			_threads.Add(new Thread(LoadItemIcon));
			_threads.Add(new Thread(LoadItemPlate));
			_threads.Add(new Thread(LoadItemFrame));
			_threads.Add(new Thread(LoadItemTicket));
			_threads.Add(new Thread(LoadItemTitle));
			_threads.Add(new Thread(LoadEtc));
			_threads.Add(new Thread(LoadCards));
		}

		private void LoadTaskChara()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadCharaGenre(dirs, ref _charaGenres);
			LoadChara(dirs, ref _charas);
			LoadCharaAwake(dirs, ref _charaAwakes);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadTaskMap()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadMaps(dirs, ref _maps);
			LoadMapColors(dirs, ref _mapColors);
			LoadMapTresures(dirs, ref _mapTreasures);
			LoadMapBonusMusics(dirs, ref _mapBonusMusics);
			LoadMapOtomodachis(dirs, ref _mapOtomodachis);
			LoadMapSilouettes(dirs, ref _mapSilhouettes);
			LoadMapTitles(dirs, ref _mapTitles);
			LoadMapChallenges(dirs, ref _challenges);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadMusic()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadMusicGenre(dirs, ref _musicGenres);
			LoadMusicVersion(dirs, ref _musicVersions);
			LoadMusicNameSort(dirs, ref _musicNameSorts);
			LoadMusicDifficulty(dirs, ref _musicDifficultys);
			LoadMusicClearRank(dirs, ref _musicClearRanks);
			LoadMusicLevel(dirs, ref _musicLevels);
			LoadScoreRanking(dirs, ref _tournamentMusics);
			LoadOfflineMusicRanking(dirs, ref _musicOffRanking);
			LoadMusicGroup(dirs, ref _musicGroups);
			LoadCourse(dirs, ref _courses);
			LoadCourseMode(dirs, ref _courseModes);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadMusicBase()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadMusic(dirs, ref _musics);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadEtc()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadEvent(dirs, ref _events);
			LoadLoginBonus(dirs, ref _loginBonuses);
			LoadPhotoFrame(dirs, ref _photoFrames);
			LoadInformation(dirs, ref _infomations);
			LoadUdemae(dirs, ref _udemaes);
			LoadClass(dirs, ref _classes);
			LoadUdemaeBoss(dirs, ref _udemaeBosses);
			LoadHoliday(dirs, ref _weekdayBonuses);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadCards()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadCard(dirs, ref _cards);
			LoadCardChara(dirs, ref _cardCharas);
			LoadCardType(dirs, ref _cardTypes);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadItem()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadCollectionGenre(dirs, ref _collectionGenres);
			LoadCollectionType(dirs, ref _collectionTypes);
			LoadItemMusic(dirs, ref _itemMusics);
			LoadPartner(dirs, ref _partners);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadItemIcon()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadIcon(dirs, ref _icons);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadItemPlate()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadPlate(dirs, ref _plates);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadItemFrame()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadFrame(dirs, ref _frames);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadItemTicket()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadTicket(dirs, ref _tickets);
			Interlocked.Increment(ref _threaEndCount);
		}

		private void LoadItemTitle()
		{
			ReadOnlyCollection<string> dirs = _targetDirs.AsReadOnly();
			LoadTitle(dirs, ref _titles);
			Interlocked.Increment(ref _threaEndCount);
		}

		public Manager.MaiStudio.CharaData GetChara(int id)
		{
			try
			{
				return GetCharas()[id];
			}
			catch
			{
				return new Manager.MaiStudio.CharaData();
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaData> GetCharas()
		{
			if (IsLoaded())
			{
				return _charas;
			}
			return CreateDummyTable<Manager.MaiStudio.CharaData>();
		}

		private void LoadChara(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaData> charas)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "chara", "CharaSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.CharaData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CharaData>();
			LoadData(dirs, "chara", "Chara.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CharaData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CharaData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CharaData> item in outSerializes)
			{
				item.Value.imageFile = "Chara/UI_Chara_" + $"{item.Value.name.id:D6}" + ".png";
				item.Value.thumbnailName = "Chara/UI_Chara_" + $"{item.Value.name.id:D6}" + "_S.png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.CharaData)item.Value;
			}
			charas = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaData>(sortedDictionary);
		}

		public Manager.MaiStudio.CharaAwakeData GetCharaAwake(int id)
		{
			try
			{
				return GetCharaAwakes()[id];
			}
			catch
			{
				return new Manager.MaiStudio.CharaAwakeData();
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaAwakeData> GetCharaAwakes()
		{
			if (IsLoaded())
			{
				return _charaAwakes;
			}
			return CreateDummyTable<Manager.MaiStudio.CharaAwakeData>();
		}

		private void LoadCharaAwake(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaAwakeData> charaAwakes)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CharaAwakeData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CharaAwakeData>();
			LoadData(dirs, "charaAwake", "CharaAwake.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CharaAwakeData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CharaAwakeData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CharaAwakeData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.CharaAwakeData)item.Value;
			}
			charaAwakes = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaAwakeData>(sortedDictionary);
		}

		public Manager.MaiStudio.CharaGenreData GetCharaGenre(int id)
		{
			try
			{
				return GetCharaGenres()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaGenreData> GetCharaGenres()
		{
			if (IsLoaded())
			{
				return _charaGenres;
			}
			return CreateDummyTable<Manager.MaiStudio.CharaGenreData>();
		}

		private void LoadCharaGenre(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaGenreData> genre)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "charaGenre", "CharaGenreSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.CharaGenreData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CharaGenreData>();
			LoadData(dirs, "charaGenre", "CharaGenre.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CharaGenreData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CharaGenreData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CharaGenreData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.CharaGenreData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CharaGenreData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicGenreData GetMusicGenre(int id)
		{
			try
			{
				return GetMusicGenres()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGenreData> GetMusicGenres()
		{
			if (IsLoaded())
			{
				return _musicGenres;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicGenreData>();
		}

		private void LoadMusicGenre(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGenreData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicGenreData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicGenreData>();
			LoadData(dirs, "musicGenre", "MusicGenre.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicGenreData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicGenreData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicGenreData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicGenreData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGenreData>(sortedDictionary);
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGroupData> GetMusicGroups()
		{
			if (IsLoaded())
			{
				return _musicGroups;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicGroupData>();
		}

		private void LoadMusicGroup(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGroupData> group)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicGroupData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicGroupData>();
			LoadData(dirs, "musicGroup", "MusicGroup.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicGroupData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicGroupData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicGroupData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicGroupData)item.Value;
			}
			group = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicGroupData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicDifficultyData GetMusicDifficulty(int id)
		{
			try
			{
				return GetMusicDifficultys()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicDifficultyData> GetMusicDifficultys()
		{
			if (IsLoaded())
			{
				return _musicDifficultys;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicDifficultyData>();
		}

		private void LoadMusicDifficulty(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicDifficultyData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicDifficultyData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicDifficultyData>();
			LoadData(dirs, "musicDifficulty", "MusicDifficulty.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicDifficultyData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicDifficultyData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicDifficultyData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicDifficultyData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicDifficultyData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicClearRankData GetMusicClearRank(int id)
		{
			try
			{
				return GetMusicClearRanks()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicClearRankData> GetMusicClearRanks()
		{
			if (IsLoaded())
			{
				return _musicClearRanks;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicClearRankData>();
		}

		private void LoadMusicClearRank(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicClearRankData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicClearRankData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicClearRankData>();
			LoadData(dirs, "musicClearRank", "MusicClearRank.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicClearRankData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicClearRankData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicClearRankData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicClearRankData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicClearRankData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicLevelData GetMusicLevel(int id)
		{
			try
			{
				return GetMusicLevels()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicLevelData> GetMusicLevels()
		{
			if (IsLoaded())
			{
				return _musicLevels;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicLevelData>();
		}

		private void LoadMusicLevel(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicLevelData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicLevelData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicLevelData>();
			LoadData(dirs, "musicLevel", "MusicLevel.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicLevelData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicLevelData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicLevelData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicLevelData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicLevelData>(sortedDictionary);
		}

		public Manager.MaiStudio.ScoreRankingData GetScoreRanking(int id)
		{
			try
			{
				return GetScoreRankings()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ScoreRankingData> GetScoreRankings()
		{
			if (IsLoaded())
			{
				return _tournamentMusics;
			}
			return CreateDummyTable<Manager.MaiStudio.ScoreRankingData>();
		}

		private void LoadScoreRanking(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ScoreRankingData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.ScoreRankingData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.ScoreRankingData>();
			LoadData(dirs, "ScoreRanking", "ScoreRanking.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.ScoreRankingData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.ScoreRankingData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.ScoreRankingData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.ScoreRankingData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ScoreRankingData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicNameSortData GetMusicNameSort(int id)
		{
			try
			{
				return GetMusicNameSorts()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicNameSortData> GetMusicNameSorts()
		{
			if (IsLoaded())
			{
				return _musicNameSorts;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicNameSortData>();
		}

		private void LoadMusicNameSort(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicNameSortData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicNameSortData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicNameSortData>();
			LoadData(dirs, "musicNameSort", "MusicNameSort.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicNameSortData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicNameSortData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicNameSortData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicNameSortData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicNameSortData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicVersionData GetMusicVersion(int id)
		{
			try
			{
				return GetMusicVersions()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicVersionData> GetMusicVersions()
		{
			if (IsLoaded())
			{
				return _musicVersions;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicVersionData>();
		}

		private void LoadMusicVersion(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicVersionData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicVersionData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicVersionData>();
			LoadData(dirs, "musicVersion", "MusicVersion.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicVersionData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicVersionData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicVersionData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicVersionData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicVersionData>(sortedDictionary);
		}

		public Manager.MaiStudio.MusicData GetMusic(int id)
		{
			try
			{
				return GetMusics()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicData> GetMusics()
		{
			if (IsLoaded())
			{
				return _musics;
			}
			return CreateDummyTable<Manager.MaiStudio.MusicData>();
		}

		private void LoadMusic(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicData> musics, int id = -1)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "music", "MusicSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicData>();
			LoadDataCustom(dirs, "music", "Music.xml", ref outSerializes, id);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicData>();
			new NotesReader();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicData> item in outSerializes)
			{
				int num = item.Value.name.id;
				if (num >= 10000 && num < 20000)
				{
					num %= 10000;
				}
				item.Value.jacketFile = "Jacket/UI_Jacket_" + $"{num:D6}" + ".png";
				item.Value.thumbnailName = "Jacket_S/UI_Jacket_" + $"{num:D6}" + "_s.png";
				if (item.Value.rightsInfoName.id != 0 && item.Value.rightsInfoName.id != -1)
				{
					item.Value.rightFile = "Rights/UI_Rights_" + $"{item.Value.rightsInfoName.id:D6}" + ".png";
				}
				else
				{
					item.Value.rightFile = string.Empty;
				}
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicData)item.Value;
			}
			musics = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicData>(sortedDictionary);
		}

		private void LoadMusicSingle(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicData> musics, int id = -1)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "music", "MusicSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.MusicData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MusicData>();
			LoadDataCustom(dirs, "music", "Music.xml", ref outSerializes, id);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MusicData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MusicData>();
			new NotesReader();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MusicData> item in outSerializes)
			{
				int num = item.Value.name.id;
				if (num >= 10000 && num < 20000)
				{
					num %= 10000;
				}
				item.Value.jacketFile = "Jacket/UI_Jacket_" + $"{num:D6}" + ".png";
				item.Value.thumbnailName = "Jacket_S/UI_Jacket_" + $"{num:D6}" + "_s.png";
				if (item.Value.rightsInfoName.id != 0 && item.Value.rightsInfoName.id != -1)
				{
					item.Value.rightFile = "Rights/UI_Rights_" + $"{item.Value.rightsInfoName.id:D6}" + ".png";
				}
				else
				{
					item.Value.rightFile = string.Empty;
				}
				sortedDictionary[item.Key] = (Manager.MaiStudio.MusicData)item.Value;
			}
			musics = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MusicData>(sortedDictionary);
		}

		public GameRanking GetMusicOffRanking(int id)
		{
			try
			{
				return GetMusicOffRankings()[id];
			}
			catch
			{
				return default(GameRanking);
			}
		}

		public GameRanking[] GetMusicOffRankings()
		{
			if (IsLoaded())
			{
				return _musicOffRanking;
			}
			return new GameRanking[0];
		}

		private void LoadOfflineMusicRanking(ReadOnlyCollection<string> dirs, ref GameRanking[] offranking)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "music", "MusicRanking.xml", ref sortData);
			_musicOffRanking = new GameRanking[sortData.SortList.Count];
			for (int i = 0; i < sortData.SortList.Count; i++)
			{
				_musicOffRanking[i] = new GameRanking
				{
					id = sortData.SortList[i].id
				};
			}
		}

		public Manager.MaiStudio.CourseData GetCourse(int id)
		{
			try
			{
				return GetCourses()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseData> GetCourses()
		{
			if (IsLoaded())
			{
				return _courses;
			}
			return CreateDummyTable<Manager.MaiStudio.CourseData>();
		}

		private void LoadCourse(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseData> courses, int id = -1)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CourseData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CourseData>();
			LoadDataCustom(dirs, "course", "Course.xml", ref outSerializes, id);
			SortedDictionary<int, Manager.MaiStudio.CourseData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CourseData>();
			new NotesReader();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CourseData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.CourseData)item.Value;
			}
			courses = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseData>(sortedDictionary);
		}

		public Manager.MaiStudio.CourseModeData GetCourseMode(int id)
		{
			try
			{
				return GetCourseModes()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseModeData> GetCourseModes()
		{
			if (IsLoaded())
			{
				return _courseModes;
			}
			return CreateDummyTable<Manager.MaiStudio.CourseModeData>();
		}

		private void LoadCourseMode(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseModeData> courses, int id = -1)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CourseModeData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CourseModeData>();
			LoadDataCustom(dirs, "courseMode", "CourseMode.xml", ref outSerializes, id);
			SortedDictionary<int, Manager.MaiStudio.CourseModeData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CourseModeData>();
			new NotesReader();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CourseModeData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.CourseModeData)item.Value;
			}
			courses = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CourseModeData>(sortedDictionary);
		}

		public Manager.MaiStudio.EventData GetEvent(int id)
		{
			try
			{
				return GetEvents()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.EventData> GetEvents()
		{
			if (IsLoaded())
			{
				return _events;
			}
			return CreateDummyTable<Manager.MaiStudio.EventData>();
		}

		private void LoadEvent(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.EventData> events)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.EventData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.EventData>();
			LoadData(dirs, "event", "Event.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.EventData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.EventData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.EventData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.EventData)item.Value;
			}
			events = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.EventData>(sortedDictionary);
		}

		public Manager.MaiStudio.LoginBonusData GetLoginBonus(int id)
		{
			try
			{
				return GetLoginBonuses()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.LoginBonusData> GetLoginBonuses()
		{
			if (IsLoaded())
			{
				return _loginBonuses;
			}
			return CreateDummyTable<Manager.MaiStudio.LoginBonusData>();
		}

		private void LoadLoginBonus(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.LoginBonusData> loginBonuses)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.LoginBonusData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.LoginBonusData>();
			LoadData(dirs, "loginBonus", "LoginBonus.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.LoginBonusData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.LoginBonusData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.LoginBonusData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.LoginBonusData)item.Value;
			}
			loginBonuses = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.LoginBonusData>(sortedDictionary);
		}

		public Manager.MaiStudio.ItemMusicData GetItemMusic(int id)
		{
			try
			{
				return GetItemMusics()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ItemMusicData> GetItemMusics()
		{
			if (IsLoaded())
			{
				return _itemMusics;
			}
			return CreateDummyTable<Manager.MaiStudio.ItemMusicData>();
		}

		private void LoadItemMusic(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ItemMusicData> itemMusics)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.ItemMusicData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.ItemMusicData>();
			LoadData(dirs, "itemMusic", "ItemMusic.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.ItemMusicData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.ItemMusicData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.ItemMusicData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.ItemMusicData)item.Value;
			}
			itemMusics = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ItemMusicData>(sortedDictionary);
		}

		public Manager.MaiStudio.IconData GetIcon(int id)
		{
			try
			{
				return GetIcons()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.IconData> GetIcons()
		{
			if (IsLoaded())
			{
				return _icons;
			}
			return CreateDummyTable<Manager.MaiStudio.IconData>();
		}

		private void LoadIcon(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.IconData> icons)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "icon", "IconSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.IconData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.IconData>();
			LoadData(dirs, "icon", "Icon.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.IconData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.IconData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.IconData> item in outSerializes)
			{
				item.Value.fileName = "Icon/UI_Icon_" + $"{item.Value.name.id:D6}" + ".png";
				item.Value.thumbnailName = "Icon/UI_Icon_" + $"{item.Value.name.id:D6}" + ".png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.IconData)item.Value;
			}
			icons = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.IconData>(sortedDictionary);
		}

		public Manager.MaiStudio.PlateData GetPlate(int id)
		{
			try
			{
				return GetPlates()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PlateData> GetPlates()
		{
			if (IsLoaded())
			{
				return _plates;
			}
			return CreateDummyTable<Manager.MaiStudio.PlateData>();
		}

		private void LoadPlate(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PlateData> plates)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "plate", "PlateSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.PlateData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.PlateData>();
			LoadData(dirs, "plate", "Plate.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.PlateData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.PlateData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.PlateData> item in outSerializes)
			{
				item.Value.fileName = "NamePlate/UI_Plate_" + $"{item.Value.name.id:D6}" + ".png";
				item.Value.thumbnailName = "NamePlate/UI_Plate_" + $"{item.Value.name.id:D6}" + ".png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.PlateData)item.Value;
			}
			plates = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PlateData>(sortedDictionary);
		}

		public Manager.MaiStudio.TitleData GetTitle(int id)
		{
			try
			{
				return GetTitles()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TitleData> GetTitles()
		{
			if (IsLoaded())
			{
				return _titles;
			}
			return CreateDummyTable<Manager.MaiStudio.TitleData>();
		}

		private void LoadTitle(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TitleData> titles)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "title", "TitleSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.TitleData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.TitleData>();
			LoadData(dirs, "title", "Title.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.TitleData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.TitleData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.TitleData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.TitleData)item.Value;
			}
			titles = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TitleData>(sortedDictionary);
		}

		public Manager.MaiStudio.PartnerData GetPartner(int id)
		{
			try
			{
				return GetPartners()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PartnerData> GetPartners()
		{
			if (IsLoaded())
			{
				return _partners;
			}
			return CreateDummyTable<Manager.MaiStudio.PartnerData>();
		}

		private void LoadPartner(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PartnerData> partners)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "partner", "PartnerSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.PartnerData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.PartnerData>();
			LoadData(dirs, "partner", "partner.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.PartnerData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.PartnerData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.PartnerData> item in outSerializes)
			{
				item.Value.fileName = "Partner/UI_Partner_" + $"{item.Value.name.id:D6}" + ".png";
				item.Value.thumbnailName = "Partner/UI_Partner_" + $"{item.Value.name.id:D6}" + ".png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.PartnerData)item.Value;
			}
			partners = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PartnerData>(sortedDictionary);
		}

		public Manager.MaiStudio.FrameData GetFrame(int id)
		{
			try
			{
				return GetFrames()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.FrameData> GetFrames()
		{
			if (IsLoaded())
			{
				return _frames;
			}
			return CreateDummyTable<Manager.MaiStudio.FrameData>();
		}

		private void LoadFrame(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.FrameData> frames)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "frame", "FrameSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.FrameData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.FrameData>();
			LoadData(dirs, "frame", "Frame.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.FrameData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.FrameData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.FrameData> item in outSerializes)
			{
				item.Value.fileName = "Frame/UI_Frame_" + $"{item.Value.name.id:D6}" + ".png";
				item.Value.maskName = "FrameMask/UI_FrameMask_" + $"{item.Value.name.id:D6}" + ".png";
				item.Value.thumbnailName = "Frame_S/UI_Frame_" + $"{item.Value.name.id:D6}" + "_S.png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.FrameData)item.Value;
			}
			frames = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.FrameData>(sortedDictionary);
		}

		public Manager.MaiStudio.TicketData GetTicket(int id)
		{
			try
			{
				return GetTickets()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TicketData> GetTickets()
		{
			if (IsLoaded())
			{
				return _tickets;
			}
			return CreateDummyTable<Manager.MaiStudio.TicketData>();
		}

		private void LoadTicket(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TicketData> tickets)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "ticket", "TicketSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.TicketData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.TicketData>();
			LoadData(dirs, "ticket", "Ticket.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.TicketData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.TicketData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.TicketData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.TicketData)item.Value;
			}
			tickets = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.TicketData>(sortedDictionary);
		}

		public Manager.MaiStudio.CollectionTypeData GetCollectionnType(int id)
		{
			try
			{
				return GetCollectionnTypes()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionTypeData> GetCollectionnTypes()
		{
			if (IsLoaded())
			{
				return _collectionTypes;
			}
			return CreateDummyTable<Manager.MaiStudio.CollectionTypeData>();
		}

		private void LoadCollectionType(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionTypeData> genre)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CollectionTypeData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CollectionTypeData>();
			LoadData(dirs, "collectionType", "CollectionType.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CollectionTypeData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CollectionTypeData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CollectionTypeData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.CollectionTypeData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionTypeData>(sortedDictionary);
		}

		public Manager.MaiStudio.CollectionGenreData GetCollectionGenre(int id)
		{
			try
			{
				return GetCollectionGenres()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionGenreData> GetCollectionGenres()
		{
			if (IsLoaded())
			{
				return _collectionGenres;
			}
			return CreateDummyTable<Manager.MaiStudio.CollectionGenreData>();
		}

		private void LoadCollectionGenre(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionGenreData> genre)
		{
			Manager.MaiStudio.Serialize.SerializeSortData sortData = new Manager.MaiStudio.Serialize.SerializeSortData();
			LoadSort(dirs, "collectionGenre", "CollectionGenreSort.xml", ref sortData);
			SortedDictionary<int, Manager.MaiStudio.Serialize.CollectionGenreData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CollectionGenreData>();
			LoadData(dirs, "collectionGenre", "CollectionGenre.xml", ref outSerializes);
			SetPriority(sortData, ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CollectionGenreData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CollectionGenreData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CollectionGenreData> item in outSerializes)
			{
				item.Value.genreNameTwoLine = item.Value.genreNameTwoLine.Replace("\\n", "\n");
				sortedDictionary[item.Key] = (Manager.MaiStudio.CollectionGenreData)item.Value;
			}
			genre = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CollectionGenreData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapData GetMapData(int id)
		{
			try
			{
				return GetMapDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapData> GetMapDatas()
		{
			if (IsLoaded())
			{
				return _maps;
			}
			return CreateDummyTable<Manager.MaiStudio.MapData>();
		}

		private void LoadMaps(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapData> maps)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapData>();
			LoadData(dirs, "map", "Map.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapData)item.Value;
			}
			maps = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapColorData GetMapColorData(int id)
		{
			try
			{
				return GetMapColorDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapColorData> GetMapColorDatas()
		{
			if (IsLoaded())
			{
				return _mapColors;
			}
			return CreateDummyTable<Manager.MaiStudio.MapColorData>();
		}

		private void LoadMapColors(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapColorData> mapcolors)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapColorData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapColorData>();
			LoadData(dirs, "mapColor", "MapColor.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapColorData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapColorData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapColorData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapColorData)item.Value;
			}
			mapcolors = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapColorData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapTreasureData GetMapTreasureData(int id)
		{
			try
			{
				return GetMapTreasureDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTreasureData> GetMapTreasureDatas()
		{
			if (IsLoaded())
			{
				return _mapTreasures;
			}
			return CreateDummyTable<Manager.MaiStudio.MapTreasureData>();
		}

		private void LoadMapTresures(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTreasureData> mapTresures)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapTreasureData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapTreasureData>();
			LoadData(dirs, "mapTreasure", "MapTreasure.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapTreasureData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapTreasureData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapTreasureData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapTreasureData)item.Value;
			}
			mapTresures = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTreasureData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapBonusMusicData GetMapBonusMusicData(int id)
		{
			try
			{
				return GetMapBonusMusicDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapBonusMusicData> GetMapBonusMusicDatas()
		{
			if (IsLoaded())
			{
				return _mapBonusMusics;
			}
			return CreateDummyTable<Manager.MaiStudio.MapBonusMusicData>();
		}

		private void LoadMapBonusMusics(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapBonusMusicData> mapBonusMusics)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapBonusMusicData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapBonusMusicData>();
			LoadData(dirs, "mapBonusMusic", "MapBonusMusic.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapBonusMusicData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapBonusMusicData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapBonusMusicData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapBonusMusicData)item.Value;
			}
			mapBonusMusics = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapBonusMusicData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapOtomodachiData GetMapOtomodachiData(int id)
		{
			try
			{
				return GetMapOtomodachiDats()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapOtomodachiData> GetMapOtomodachiDats()
		{
			if (IsLoaded())
			{
				return _mapOtomodachis;
			}
			return CreateDummyTable<Manager.MaiStudio.MapOtomodachiData>();
		}

		private void LoadMapOtomodachis(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapOtomodachiData> mapOtomodachis)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapOtomodachiData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapOtomodachiData>();
			LoadData(dirs, "mapOtomodachi", "MapOtomodachi.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapOtomodachiData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapOtomodachiData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapOtomodachiData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapOtomodachiData)item.Value;
			}
			mapOtomodachis = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapOtomodachiData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapSilhouetteData GetMapSilhouetteData(int id)
		{
			try
			{
				return GetMapSilhouetteDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapSilhouetteData> GetMapSilhouetteDatas()
		{
			if (IsLoaded())
			{
				return _mapSilhouettes;
			}
			return CreateDummyTable<Manager.MaiStudio.MapSilhouetteData>();
		}

		private void LoadMapSilouettes(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapSilhouetteData> mapSilhouettes)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapSilhouetteData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapSilhouetteData>();
			LoadData(dirs, "mapSilhouette", "MapSilhouette.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapSilhouetteData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapSilhouetteData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapSilhouetteData> item in outSerializes)
			{
				item.Value.fileName = "Silhouette/UI_Silhouette_" + $"{item.Value.name.id:D6}" + ".png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapSilhouetteData)item.Value;
			}
			mapSilhouettes = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapSilhouetteData>(sortedDictionary);
		}

		public Manager.MaiStudio.MapTitleData GetMapTitleData(int id)
		{
			try
			{
				return GetMapTitleDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTitleData> GetMapTitleDatas()
		{
			if (IsLoaded())
			{
				return _mapTitles;
			}
			return CreateDummyTable<Manager.MaiStudio.MapTitleData>();
		}

		private void LoadMapTitles(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTitleData> mapTitles)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.MapTitleData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.MapTitleData>();
			LoadData(dirs, "mapTitle", "MapTitle.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.MapTitleData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.MapTitleData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.MapTitleData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.MapTitleData)item.Value;
			}
			mapTitles = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.MapTitleData>(sortedDictionary);
		}

		public Manager.MaiStudio.ChallengeData GetChallengeData(int id)
		{
			try
			{
				return GetChallengeDatas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ChallengeData> GetChallengeDatas()
		{
			if (IsLoaded())
			{
				return _challenges;
			}
			return CreateDummyTable<Manager.MaiStudio.ChallengeData>();
		}

		private void LoadMapChallenges(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ChallengeData> challenges)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.ChallengeData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.ChallengeData>();
			LoadData(dirs, "Challenge", "Challenge.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.ChallengeData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.ChallengeData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.ChallengeData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.ChallengeData)item.Value;
			}
			challenges = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ChallengeData>(sortedDictionary);
		}

		public Manager.MaiStudio.PhotoFrameData GetPhotoFrame(int id)
		{
			try
			{
				return GetPhotoFrames()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PhotoFrameData> GetPhotoFrames()
		{
			if (IsLoaded())
			{
				return _photoFrames;
			}
			return CreateDummyTable<Manager.MaiStudio.PhotoFrameData>();
		}

		private void LoadPhotoFrame(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PhotoFrameData> photoFrames)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.PhotoFrameData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.PhotoFrameData>();
			LoadData(dirs, "photoframe", "PhotoFrame.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.PhotoFrameData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.PhotoFrameData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.PhotoFrameData> item in outSerializes)
			{
				item.Value.imageFile = "PhotoFrame/UI_PhotoFrame_" + $"{item.Value.name.id:D6}" + ".png";
				sortedDictionary[item.Key] = (Manager.MaiStudio.PhotoFrameData)item.Value;
			}
			photoFrames = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.PhotoFrameData>(sortedDictionary);
		}

		public Manager.MaiStudio.CardData GetCard(int id)
		{
			try
			{
				return GetCards()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardData> GetCards()
		{
			if (IsLoaded())
			{
				return _cards;
			}
			return CreateDummyTable<Manager.MaiStudio.CardData>();
		}

		private void LoadCard(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardData> cards)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CardData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CardData>();
			LoadData(dirs, "card", "Card.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CardData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CardData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CardData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.CardData)item.Value;
			}
			cards = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardData>(sortedDictionary);
		}

		public Manager.MaiStudio.CardCharaData GetCardChara(int id)
		{
			try
			{
				return GetCardCharas()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardCharaData> GetCardCharas()
		{
			if (IsLoaded())
			{
				return _cardCharas;
			}
			return CreateDummyTable<Manager.MaiStudio.CardCharaData>();
		}

		private void LoadCardChara(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardCharaData> cardCharas)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CardCharaData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CardCharaData>();
			LoadData(dirs, "cardChara", "CardChara.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CardCharaData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CardCharaData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CardCharaData> item in outSerializes)
			{
				item.Value.thumbnailName = "CardChara_S/UI_CardChara_" + $"{item.Value.name.id:D6}" + "_S.png";
				item.Value.imageFile = item.Value.thumbnailName;
				sortedDictionary[item.Key] = (Manager.MaiStudio.CardCharaData)item.Value;
			}
			cardCharas = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardCharaData>(sortedDictionary);
		}

		public Manager.MaiStudio.CardTypeData GetCardType(int id)
		{
			try
			{
				return GetCardTypes()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardTypeData> GetCardTypes()
		{
			if (IsLoaded())
			{
				return _cardTypes;
			}
			return CreateDummyTable<Manager.MaiStudio.CardTypeData>();
		}

		private void LoadCardType(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardTypeData> cardTypes)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.CardTypeData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.CardTypeData>();
			LoadData(dirs, "cardType", "CardType.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.CardTypeData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.CardTypeData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.CardTypeData> item in outSerializes)
			{
				item.Value.frameThumbnailName = "CardFrame_S/UI_CardFrame_" + $"{item.Value.name.id:D7}" + "_S.png";
				item.Value.frameImageFile = item.Value.frameThumbnailName;
				sortedDictionary[item.Key] = (Manager.MaiStudio.CardTypeData)item.Value;
			}
			cardTypes = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.CardTypeData>(sortedDictionary);
		}

		public Manager.MaiStudio.InformationData GetInformation(int id)
		{
			try
			{
				return GetInformations()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.InformationData> GetInformations()
		{
			if (IsLoaded())
			{
				return _infomations;
			}
			return CreateDummyTable<Manager.MaiStudio.InformationData>();
		}

		private void LoadInformation(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.InformationData> infomations)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.InformationData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.InformationData>();
			LoadData(dirs, "information", "Information.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.InformationData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.InformationData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.InformationData> item in outSerializes)
			{
				item.Value.disable = !File.Exists(item.Value.fileName.path);
				sortedDictionary[item.Key] = (Manager.MaiStudio.InformationData)item.Value;
			}
			infomations = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.InformationData>(sortedDictionary);
		}

		public Manager.MaiStudio.UdemaeData GetUdemae(int id)
		{
			try
			{
				return GetUdemaes()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeData> GetUdemaes()
		{
			if (IsLoaded())
			{
				return _udemaes;
			}
			return CreateDummyTable<Manager.MaiStudio.UdemaeData>();
		}

		private void LoadUdemae(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeData> infomations)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.UdemaeData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.UdemaeData>();
			LoadData(dirs, "udemae", "Udemae.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.UdemaeData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.UdemaeData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.UdemaeData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.UdemaeData)item.Value;
			}
			infomations = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeData>(sortedDictionary);
		}

		public Manager.MaiStudio.ClassData GetClass(int id)
		{
			try
			{
				return GetClasses()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ClassData> GetClasses()
		{
			if (IsLoaded())
			{
				return _classes;
			}
			return CreateDummyTable<Manager.MaiStudio.ClassData>();
		}

		private void LoadClass(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ClassData> classes)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.ClassData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.ClassData>();
			LoadData(dirs, "class", "Class.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.ClassData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.ClassData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.ClassData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.ClassData)item.Value;
			}
			classes = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.ClassData>(sortedDictionary);
		}

		public Manager.MaiStudio.UdemaeBossData GetUdemaeBoss(int id)
		{
			try
			{
				return GetUdemaeBosses()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeBossData> GetUdemaeBosses()
		{
			if (IsLoaded())
			{
				return _udemaeBosses;
			}
			return CreateDummyTable<Manager.MaiStudio.UdemaeBossData>();
		}

		private void LoadUdemaeBoss(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeBossData> udemaeBosses)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.UdemaeBossData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.UdemaeBossData>();
			LoadData(dirs, "udemaeBoss", "UdemaeBoss.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.UdemaeBossData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.UdemaeBossData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.UdemaeBossData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.UdemaeBossData)item.Value;
			}
			udemaeBosses = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.UdemaeBossData>(sortedDictionary);
		}

		public Manager.MaiStudio.HolidayData GetHoliday(int id)
		{
			try
			{
				return GetHolidayes()[id];
			}
			catch
			{
				return null;
			}
		}

		public Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.HolidayData> GetHolidayes()
		{
			if (IsLoaded())
			{
				return _weekdayBonuses;
			}
			return CreateDummyTable<Manager.MaiStudio.HolidayData>();
		}

		private void LoadHoliday(ReadOnlyCollection<string> dirs, ref Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.HolidayData> Holiday)
		{
			SortedDictionary<int, Manager.MaiStudio.Serialize.HolidayData> outSerializes = new SortedDictionary<int, Manager.MaiStudio.Serialize.HolidayData>();
			LoadData(dirs, "Holiday", "Holiday.xml", ref outSerializes);
			SortedDictionary<int, Manager.MaiStudio.HolidayData> sortedDictionary = new SortedDictionary<int, Manager.MaiStudio.HolidayData>();
			foreach (KeyValuePair<int, Manager.MaiStudio.Serialize.HolidayData> item in outSerializes)
			{
				sortedDictionary[item.Key] = (Manager.MaiStudio.HolidayData)item.Value;
			}
			Holiday = new Safe.ReadonlySortedDictionary<int, Manager.MaiStudio.HolidayData>(sortedDictionary);
		}

		private ReadOnlyDictionary<int, T1> LoadXml<T0, T1>(ReadOnlyCollection<string> dirs, string subdir, string fname, Func<T0, T1> cast) where T0 : SerializeBase, ISerialize, new()where T1 : AccessorBase
		{
			SortedDictionary<int, T0> outSerializes = new SortedDictionary<int, T0>();
			LoadData(dirs, subdir, fname, ref outSerializes);
			return new ReadOnlyDictionary<int, T1>(outSerializes.Select((KeyValuePair<int, T0> i) => new
			{
				k = i.Key,
				v = cast(i.Value)
			}).ToDictionary(x => x.k, x => x.v));
		}

		private bool LoadDataCustom<T>(ReadOnlyCollection<string> dirs, string subDir, string fileName, ref SortedDictionary<int, T> outSerializes, int id = -1) where T : SerializeBase, ISerialize, new()
		{
			if (id != 1)
			{
				return LoadData(dirs, subDir, fileName, ref outSerializes);
			}
			bool flag = false;
			try
			{
				foreach (string dir in dirs)
				{
					string path = dir + "/" + subDir + "/";
					if (!Directory.Exists(path))
					{
						continue;
					}
					string[] files = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
					foreach (string text in files)
					{
						if (Deserialize<T>(text, out var dsr) && dsr.GetID() == id)
						{
							string directoryName = System.IO.Path.GetDirectoryName(text);
							directoryName += "/";
							dsr.AddPath(directoryName.Replace("\\", "/"));
							outSerializes[dsr.GetID()] = dsr;
						}
						else
						{
							flag = true;
						}
					}
				}
				List<int> list = new List<int>();
				foreach (KeyValuePair<int, T> outSerialize in outSerializes)
				{
					if (outSerialize.Value.IsDisable())
					{
						list.Add(outSerialize.Key);
					}
				}
				foreach (int item in list)
				{
					outSerializes.Remove(item);
				}
			}
			catch
			{
				flag = true;
			}
			if (flag)
			{
				Interlocked.Increment(ref _errorCount);
				return false;
			}
			return true;
		}

		public bool ConvertPresentID2Item(long presentId, out int itemId, out ItemKind itemType)
		{
			itemType = (ItemKind)(presentId / ConstParameter.PresentKindConvert);
			itemId = (int)(presentId % ConstParameter.PresentKindConvert);
			return itemType switch
			{
				ItemKind.Icon => GetIcons().ContainsKey(itemId), 
				ItemKind.Plate => GetPlates().ContainsKey(itemId), 
				ItemKind.Title => GetTitles().ContainsKey(itemId), 
				ItemKind.Partner => GetPartners().ContainsKey(itemId), 
				ItemKind.Frame => GetFrames().ContainsKey(itemId), 
				ItemKind.Ticket => GetTickets().ContainsKey(itemId), 
				ItemKind.Music => GetMusics().ContainsKey(itemId), 
				ItemKind.Character => GetCharas().ContainsKey(itemId), 
				_ => false, 
			};
		}
	}
}
