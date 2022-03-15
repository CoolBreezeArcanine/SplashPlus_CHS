using UnityEngine;

namespace Monitor
{
	public class MonitorBackgroundTownController : MonoBehaviour
	{
		[SerializeField]
		private GameObject _bgPrefab;

		[SerializeField]
		private GameObject _mainCanvas;

		[SerializeField]
		private GameObject _subCanvas;

		private GameObject _bgObject;

		private Animator _bgAnim;

		public void Awake()
		{
			if (!(null != _bgObject))
			{
				_bgObject = Object.Instantiate(_bgPrefab, _mainCanvas.transform);
				_bgAnim = _bgObject.GetComponent<Animator>();
			}
		}

		public virtual void Initialize()
		{
			SetType(0);
		}

		public void SetType(int id)
		{
			Animator bgAnim = _bgAnim;
			if (!(bgAnim == null))
			{
				bgAnim.Play((id == 0) ? "BGColor_Lime" : "BGColor_Lemon");
			}
		}

		public void SetDisp(bool dispflag)
		{
			_bgObject.SetActive(dispflag);
		}
	}
}
