using UnityEngine;

namespace Monitor.Game
{
	public class GamePhotoInfoCtrl : MonoBehaviour
	{
		[SerializeField]
		[Header("写真撮影プレハブ")]
		private GameObject PhotoKumaPrefab;

		private GameObject kumaRootObj;

		private Animator kumaAnim;

		public void Initialize()
		{
			kumaRootObj = Object.Instantiate(PhotoKumaPrefab, base.gameObject.transform);
			kumaRootObj.SetActive(value: false);
			kumaAnim = kumaRootObj.GetComponent<Animator>();
		}

		public void Execute()
		{
		}

		public void Play()
		{
			kumaRootObj.SetActive(value: true);
			kumaAnim.Play("In");
		}
	}
}
