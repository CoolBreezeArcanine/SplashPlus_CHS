using UnityEngine;

namespace CustomNote
{
	[AddComponentMenu("CustomNote")]
	public class CustomNote : MonoBehaviour
	{
		[SerializeField]
		private string _noteText;

		private void Reset()
		{
		}
	}
}
