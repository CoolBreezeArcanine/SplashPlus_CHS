using System.IO;
using MAI2.Util;
using Manager;
using UnityEngine;

public class AssetManager
{
	private AssetBundleManager assetBundleManager;

	public const string IconDummyName = "Icon/UI_Icon_000000.png";

	public const string PlateDummyName = "NamePlate/UI_Plate_000000.png";

	public const string PartnerDummyName = "Partner/UI_PartnerIcon_000000.png";

	public const string FrameDummyName = "Frame/UI_Frame_000000.png";

	public const string FrameMaskDummyName = "FrameMask/UI_FrameMask_000000.png";

	public const string FrameThumbDummyName = "Frame_S/UI_Frame_000000_S.png";

	public const string CharaDummyName = "Chara/UI_Chara_000000.png";

	public const string JacketDummyName = "Jacket/UI_Jacket_000000.png";

	public const string JacketThumbDummyName = "Jacket_S/UI_Jacket_000000_S.png";

	public const string RightDummyName = "Rights/UI_Rights_000000.png";

	public const string SilhouetDummyName = "Silhouette/UI_Silhouette_000000.png";

	public const string PhotoFrameDummyName = "PhotoFrame/UI_PhotoFrame_000000.png";

	public const string CardCharaThumbDummyName = "CardChara_S/UI_CardChara_000000_S.png";

	public const string CardBaseThumbDummyName = "CardBase_S/UI_CardBase_0000000_000000_S.png";

	public const string CardFrameThumbDummyName = "CardFrame_S/UI_CardFrame_0000000_S.png";

	private static AssetManager _instance;

	public static void SetInstance(AssetManager asset)
	{
		_instance = asset;
	}

	public static AssetManager Instance()
	{
		return _instance;
	}

	public AssetManager(MonoBehaviour monoBehaviour)
	{
		string fullPath = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath, "A000/AssetBundleImages/"));
		assetBundleManager = new AssetBundleManager(fullPath, monoBehaviour);
	}

	public void Initialize()
	{
		assetBundleManager.LoadAllAssetBundle();
	}

	public bool IsDone()
	{
		return assetBundleManager.isDone;
	}

	public float GetProgress()
	{
		return assetBundleManager.GetLoadProgress();
	}

	public string[] GetAllAssetBundlePathNames()
	{
		return assetBundleManager.GetAllAssetBundlePathNames();
	}

	public T LoadAsset<T>(string dataPath) where T : Object
	{
		dataPath = dataPath.ToLower();
		return assetBundleManager.LoadAsset<T>(dataPath);
	}

	public AssetBundleRequest LoadAssetAcync<T>(string dataPath) where T : Object
	{
		dataPath = dataPath.ToLower();
		return assetBundleManager.LoadAssetAsync<T>(dataPath);
	}

	public Texture2D GetJacketTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetMusic(id) == null)
		{
			return GetJacketTexture2D("Jacket/UI_Jacket_000000.png");
		}
		return GetJacketTexture2D(Singleton<DataManager>.Instance.GetMusic(id).jacketFile);
	}

	public Texture2D GetJacketTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			string path = Application.streamingAssetsPath + "/images/" + filename;
			if (File.Exists(path))
			{
				texture2D = new Texture2D(400, 400);
				texture2D.LoadImage(File.ReadAllBytes(path));
				return texture2D;
			}
			texture2D = LoadAsset<Texture2D>("Jacket/UI_Jacket_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetJacketThumbTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetMusic(id) == null)
		{
			return GetJacketThumbTexture2D("Jacket_S/UI_Jacket_000000_S.png");
		}
		return GetJacketThumbTexture2D(Singleton<DataManager>.Instance.GetMusic(id).thumbnailName);
	}

	public Texture2D GetJacketThumbTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			string path = Application.streamingAssetsPath + "/images/" + filename;
			if (File.Exists(path))
			{
				texture2D = new Texture2D(400, 400);
				texture2D.LoadImage(File.ReadAllBytes(path));
				return texture2D;
			}
			texture2D = LoadAsset<Texture2D>("Jacket_S/UI_Jacket_000000_S.png");
		}
		return texture2D;
	}

	public Texture2D GetIconTexture2D(int playerIndex, int id)
	{
		if (id == 10 && playerIndex < 2 && playerIndex >= 0 && null != GameManager.FaceIconTexture[playerIndex])
		{
			return GameManager.FaceIconTexture[playerIndex];
		}
		if (Singleton<DataManager>.Instance.GetIcon(id) == null)
		{
			return GetIconTexture2D("Icon/UI_Icon_000000.png");
		}
		return GetIconTexture2D(Singleton<DataManager>.Instance.GetIcon(id).fileName);
	}

	private Texture2D GetIconTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Icon/UI_Icon_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetPlateTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetPlate(id) == null)
		{
			return GetPlateTexture2D("NamePlate/UI_Plate_000000.png");
		}
		return GetPlateTexture2D(Singleton<DataManager>.Instance.GetPlate(id).fileName);
	}

	public Texture2D GetPlateTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("NamePlate/UI_Plate_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetPartnerTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetPartner(id) == null)
		{
			return GetPartnerTexture2D("Partner/UI_PartnerIcon_000000.png");
		}
		return GetPartnerTexture2D(Singleton<DataManager>.Instance.GetPartner(id).fileName);
	}

	public Texture2D GetPartnerTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Partner/UI_PartnerIcon_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetFrameTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetFrame(id) == null)
		{
			return GetFrameTexture2D("Frame/UI_Frame_000000.png");
		}
		return GetFrameTexture2D(Singleton<DataManager>.Instance.GetFrame(id).fileName);
	}

	public Texture2D GetFrameTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Frame/UI_Frame_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetFrameMaskTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetFrame(id) == null)
		{
			return GetFrameMaskTexture2D("FrameMask/UI_FrameMask_000000.png");
		}
		return GetFrameMaskTexture2D(Singleton<DataManager>.Instance.GetFrame(id).maskName);
	}

	public Texture2D GetFrameMaskTexture2D(string filename)
	{
		Texture2D texture2D = null;
		if (filename.Length > 0)
		{
			texture2D = LoadAsset<Texture2D>(filename);
		}
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("FrameMask/UI_FrameMask_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetFrameThumbTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetFrame(id) == null)
		{
			return GetFrameThumbTexture2D("Frame_S/UI_Frame_000000_S.png");
		}
		return GetFrameThumbTexture2D(Singleton<DataManager>.Instance.GetFrame(id).thumbnailName);
	}

	public Texture2D GetFrameThumbTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Frame_S/UI_Frame_000000_S.png");
		}
		return texture2D;
	}

	public Texture2D GetCharacterTexture2D(int id)
	{
		if (Singleton<DataManager>.Instance.GetChara(id) == null)
		{
			return GetCharacterTexture2D("Chara/UI_Chara_000000.png");
		}
		return GetCharacterTexture2D(Singleton<DataManager>.Instance.GetChara(id).imageFile);
	}

	public Texture2D GetCharacterTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Chara/UI_Chara_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetPhotoFrameTexture2D(int id)
	{
		return GetPhotoFrameTexture2D(Singleton<DataManager>.Instance.GetPhotoFrame(id).imageFile);
	}

	public Texture2D GetPhotoFrameTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("PhotoFrame/UI_PhotoFrame_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetRightTexture2D(int id)
	{
		return GetRightTexture2D(Singleton<DataManager>.Instance.GetMusic(id).rightFile);
	}

	public Texture2D GetRightTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Rights/UI_Rights_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetSilhouetteTexture2D(int id)
	{
		return GetSilhouetteTexture2D(Singleton<DataManager>.Instance.GetMapSilhouetteData(id).fileName);
	}

	public Texture2D GetSilhouetteTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("Silhouette/UI_Silhouette_000000.png");
		}
		return texture2D;
	}

	public Texture2D GetCardCharaTexture2D(int id)
	{
		return GetCardCharaTexture2D(Singleton<DataManager>.Instance.GetCardChara(id).thumbnailName);
	}

	public Texture2D GetCardCharaTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("CardChara_S/UI_CardChara_000000_S.png");
		}
		return texture2D;
	}

	public Texture2D GetCardBaseTexture2D(int id, int effectId)
	{
		string filename = "CardBase_S/UI_CardBase_" + $"{id:D7}_" + $"{effectId:D6}" + "_S.png";
		return GetCardBaseTexture2D(filename);
	}

	private Texture2D GetCardBaseTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("CardBase_S/UI_CardBase_0000000_000000_S.png");
		}
		return texture2D;
	}

	public Texture2D GetCardFrameTexture2D(int id)
	{
		return GetCardFrameTexture2D(Singleton<DataManager>.Instance.GetCardType(id).frameThumbnailName);
	}

	public Texture2D GetCardFrameTexture2D(string filename)
	{
		Texture2D texture2D = LoadAsset<Texture2D>(filename);
		if (texture2D == null)
		{
			texture2D = LoadAsset<Texture2D>("CardFrame_S/UI_CardFrame_0000000_S.png");
		}
		return texture2D;
	}

	public GameObject GetIslandPrefab(int id)
	{
		string dataPath = "Map/Prefab/Island/UI_" + $"{id:D6}" + ".prefab";
		GameObject gameObject = LoadAsset<GameObject>(dataPath);
		if (gameObject == null)
		{
			id = 1;
			dataPath = "Map/Prefab/Island/UI_" + $"{id:D6}" + ".prefab";
			gameObject = LoadAsset<GameObject>(dataPath);
		}
		return gameObject;
	}

	public GameObject GetIslandPrefab(string _filename)
	{
		string dataPath = "Map/Prefab/Island/" + _filename + ".prefab";
		GameObject gameObject = LoadAsset<GameObject>(dataPath);
		_ = gameObject == null;
		return gameObject;
	}

	public Texture2D GetIslandBgTexture(string filename)
	{
		string dataPath = "Map/Sprite/BG" + filename + ".png";
		Texture2D texture2D = LoadAsset<Texture2D>(dataPath);
		_ = texture2D == null;
		return texture2D;
	}

	public GameObject GetNaviCharaPrefab(int id)
	{
		string dataPath = "NaviChara/Prefab/UI_Navichara_" + $"{id:D2}" + ".prefab";
		GameObject gameObject = LoadAsset<GameObject>(dataPath);
		if (gameObject == null)
		{
			id = 1;
			dataPath = "NaviChara/Prefab/UI_Navichara_" + $"{id:D2}" + ".prefab";
			gameObject = LoadAsset<GameObject>(dataPath);
		}
		return gameObject;
	}

	public Texture2D GetNaviCharaIcon(int id)
	{
		string dataPath = "NaviChara/Icon/UI_CMN_NaviChara_Icon_" + $"{id:D2}" + ".png";
		Texture2D texture2D = LoadAsset<Texture2D>(dataPath);
		if (texture2D == null)
		{
			id = 1;
			dataPath = "NaviChara/Icon/UI_CMN_NaviChara_Icon_" + $"{id:D2}" + ".png";
			texture2D = LoadAsset<Texture2D>(dataPath);
		}
		return texture2D;
	}

	public Sprite GetMapBgSprite(int mapId, string filename)
	{
		string dataPath = "Map/Sprite/BG/" + mapId.ToString("000000") + "/" + filename + ".png";
		Sprite sprite = LoadAsset<Sprite>(dataPath);
		if (sprite == null)
		{
			mapId = 999999;
			dataPath = "Map/Sprite/BG/" + mapId.ToString("000000") + "/" + filename + ".png";
			sprite = LoadAsset<Sprite>(dataPath);
		}
		return sprite;
	}

	public Texture2D GetMapBgTexture2D(int mapId, string filename)
	{
		string dataPath = "Map/Sprite/BG/" + mapId.ToString("000000") + "/" + filename + ".png";
		Texture2D texture2D = LoadAsset<Texture2D>(dataPath);
		if (texture2D == null)
		{
			mapId = 1;
			dataPath = "Map/Sprite/BG/" + mapId.ToString("000000") + "/" + filename + ".png";
			texture2D = LoadAsset<Texture2D>(dataPath);
		}
		return texture2D;
	}
}
