using UnityEngine;
using UnityEngine.Video;

public class FX_VideoPlayer_SetCamera : MonoBehaviour
{
	private void Start()
	{
		GetComponent<VideoPlayer>().targetCamera = Camera.main;
	}
}
