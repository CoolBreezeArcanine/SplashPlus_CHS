using System;
using UnityEngine;

public static class AnimatorExtensions
{
	public static void PlayendAction(this Animator animator, int layer, int fullPathHash, Action playend)
	{
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(layer);
		if (currentAnimatorStateInfo.fullPathHash == fullPathHash && !(currentAnimatorStateInfo.normalizedTime < 1f))
		{
			playend();
		}
	}

	public static void PlayendAction(this Animator animator, int layer, string fullPath, Action playend)
	{
		animator.PlayendAction(layer, Animator.StringToHash(fullPath), playend);
	}
}
