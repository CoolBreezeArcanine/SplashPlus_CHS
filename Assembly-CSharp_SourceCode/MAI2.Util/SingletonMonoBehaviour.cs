using UnityEngine;

namespace MAI2.Util
{
	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		[SerializeField]
		protected bool _dontDestroyOnLoad;

		private static T _instance;

		public static T instance => _instance;

		protected SingletonMonoBehaviour()
		{
		}

		protected void Awake()
		{
			_instance = base.gameObject.GetComponent<T>();
			if (_dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(base.transform.gameObject);
			}
		}

		private void OnDestroy()
		{
			_instance = null;
		}
	}
}
