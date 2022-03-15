using System;
using System.Collections;
using UnityEngine;

public class GemPedestalController : MonoBehaviour
{
	private readonly int Loop = Animator.StringToHash("Loop");

	private readonly int Loop_Blank = Animator.StringToHash("Loop_Blank");

	private readonly int Hash_Get_Gem = Animator.StringToHash("Get_Gem");

	private Action _getGemExitAction;

	private int _index;

	[SerializeField]
	[Header("宝石台座")]
	private RectTransform[] gemPedestalPositions;

	private Animator[] _gemAnimators;

	public void GemGenerate(GameObject smallGem)
	{
		_gemAnimators = new Animator[gemPedestalPositions.Length];
		for (int i = 0; i < gemPedestalPositions.Length; i++)
		{
			_gemAnimators[i] = UnityEngine.Object.Instantiate(smallGem, gemPedestalPositions[i].transform).GetComponent<Animator>();
			_gemAnimators[i].Play(Loop_Blank, 0, 0f);
		}
	}

	public void SetGemActivate(int activeCount)
	{
		if (_gemAnimators == null)
		{
			return;
		}
		for (int i = 0; i < _gemAnimators.Length; i++)
		{
			if (i < activeCount)
			{
				_gemAnimators[i].Play(Loop, 0, 0f);
			}
		}
	}

	public void GemGet(int index, Action exit)
	{
		_index = index;
		StartCoroutine(GemGetCoroutine(exit));
	}

	public void BreakGem(int index, Action exit)
	{
		_index = index;
		StartCoroutine(BreakGem(exit));
	}

	public void ClassUp()
	{
		if (_gemAnimators != null)
		{
			for (int i = 0; i < _gemAnimators.Length; i++)
			{
				_gemAnimators[i].Play("ClassUp_Gem");
			}
		}
	}

	private IEnumerator GemGetCoroutine(Action exit)
	{
		if (_gemAnimators != null)
		{
			_gemAnimators[_index].Play(Hash_Get_Gem, 0, 0f);
			yield return new WaitForSeconds(_gemAnimators[_index].GetCurrentAnimatorStateInfo(0).length);
			_gemAnimators[_index].Play(Loop, 0, 0f);
			exit();
		}
	}

	private IEnumerator BreakGem(Action exit)
	{
		if (_gemAnimators != null)
		{
			_gemAnimators[_index].Play("Break_Gem", 0, 0f);
			yield return null;
			exit();
			yield return new WaitForSeconds(_gemAnimators[_index].GetCurrentAnimatorStateInfo(0).length);
		}
	}

	public void Dispose()
	{
		if (_gemAnimators != null)
		{
			for (int i = 0; i < _gemAnimators.Length; i++)
			{
				UnityEngine.Object.Destroy(_gemAnimators[i].gameObject);
			}
		}
	}
}
