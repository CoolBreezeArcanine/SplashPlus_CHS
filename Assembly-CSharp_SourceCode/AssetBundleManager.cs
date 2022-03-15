using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MAI2.Util;
using Manager;
using UnityEngine;

[ProjectPrefs("AssetBundleRootDirectory", "アセットバンドルの対象となるルートディレクトリへのPath", "AssetBundle", typeof(string))]
public class AssetBundleManager
{
	public const string ASSETBUNDLE_ROOT_DIRECTORY = "AssetBundleRootDirectory";

	private const int threadCount = 16;

	public bool isDone;

	private MonoBehaviour coroutineObject;

	private Dictionary<string, AssetBundle> cacheAssetBundleDic;

	private AssetBundleManifest manifest;

	private Queue<string> queue;

	private string directoryPath;

	private int totalNumber;

	private float progress;

	public AssetBundleManager(string assetbundleDirectoryPath, MonoBehaviour coroutineObject)
	{
		this.coroutineObject = coroutineObject;
		cacheAssetBundleDic = new Dictionary<string, AssetBundle>();
		string assetBundlePath = Singleton<OptionDataManager>.Instance.GetAssetBundlePath("AssetBundleImages");
		if (File.Exists(assetBundlePath))
		{
			AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
			manifest = assetBundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
		}
	}

	public float GetLoadProgress()
	{
		return progress;
	}

	public string[] GetAllAssetBundlePathNames()
	{
		if (manifest != null)
		{
			return manifest.GetAllAssetBundles();
		}
		return null;
	}

	public void LoadAllAssetBundle()
	{
		if (manifest != null)
		{
			queue = new Queue<string>(manifest.GetAllAssetBundles());
			totalNumber = queue.Count;
			for (int i = 0; i < 16; i++)
			{
				coroutineObject.StartCoroutine(LoadAssetBundleAsync(queue, OnSucess));
			}
			coroutineObject.StartCoroutine(CheckAllLoadEnded(queue));
			return;
		}
		throw new Exception("Manifestが存在していません");
	}

	private IEnumerator CheckAllLoadEnded(Queue<string> queue)
	{
		yield return new WaitWhile(delegate
		{
			progress = 1f - (float)queue.Count / (float)totalNumber;
			return queue.Count > 0;
		});
		isDone = true;
	}

	private IEnumerator LoadAssetBundleAsync(Queue<string> queue, Action<string, AssetBundle> onSucess)
	{
		while (queue.Count > 0)
		{
			string path = queue.Dequeue();
			AssetBundleCreateRequest requeset = AssetBundle.LoadFromFileAsync(Singleton<OptionDataManager>.Instance.GetAssetBundlePath(path));
			yield return new WaitWhile(() => !requeset.isDone);
			OnSucess(path, requeset.assetBundle);
		}
	}

	private void OnSucess(string name, AssetBundle loadedAssetBundle)
	{
		try
		{
			string text = name.ToLower();
			string extension = Path.GetExtension(text);
			cacheAssetBundleDic.Add(text.Replace(extension, string.Empty), loadedAssetBundle);
		}
		catch
		{
		}
	}

	public T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
	{
		string name = "Assets/" + ProjectPrefs.GetString("AssetBundleRootDirectory") + assetPath;
		string text = assetPath.ToLower();
		string extension = Path.GetExtension(text);
		text = text.Replace(extension, string.Empty);
		if (cacheAssetBundleDic.ContainsKey(text))
		{
			return cacheAssetBundleDic[text].LoadAsset<T>(name);
		}
		return null;
	}

	public AssetBundleRequest LoadAssetAsync<T>(string assetPath) where T : UnityEngine.Object
	{
		string name = "Assets/" + ProjectPrefs.GetString("AssetBundleRootDirectory") + assetPath;
		return cacheAssetBundleDic[assetPath].LoadAssetAsync<T>(name);
	}
}
