using UnityEngine;

namespace FX
{
	public class FX_Dbg_AutoMotion_StateLoop : MonoBehaviour
	{
		[SerializeField]
		private string _st = "State";

		[SerializeField]
		private int _loop1 = 1;

		[SerializeField]
		private int _loop1_Frame;

		[SerializeField]
		private int _loop2 = 2;

		[SerializeField]
		private int _loop2_Frame = 30;

		[SerializeField]
		private int _loopFrames = 120;

		private int Count;

		private Animator nAnim;

		[SerializeField]
		private int _minStateNo;

		[SerializeField]
		private int _maxStateNo = 3;

		private int _currentStateNo;

		[SerializeField]
		private bool _autoPlay;

		private void Start()
		{
			nAnim = GetComponent<Animator>();
		}

		private void Update()
		{
			float axis = Input.GetAxis("Mouse ScrollWheel");
			if (axis > 0f)
			{
				_currentStateNo--;
				if (_currentStateNo == _minStateNo - 1)
				{
					_currentStateNo = _maxStateNo;
				}
				nAnim.SetInteger(_st, _currentStateNo);
			}
			if (axis < 0f)
			{
				_currentStateNo++;
				if (_currentStateNo == _maxStateNo + 1)
				{
					_currentStateNo = _minStateNo;
				}
				nAnim.SetInteger(_st, _currentStateNo);
			}
			if (_autoPlay)
			{
				if (Count == _loop1_Frame)
				{
					nAnim.SetInteger(_st, _loop1);
				}
				if (Count == _loop2_Frame)
				{
					nAnim.SetInteger(_st, _loop2);
				}
				Count++;
				if (Count > _loopFrames)
				{
					Count = 0;
				}
			}
		}
	}
}
