using UI;
using UnityEngine;
using UnityEngine.UI;

public class AdvRankingParts : MonoBehaviour
{
	[SerializeField]
	private MultipleImage _ranking;

	[SerializeField]
	private CustomTextScroll _musicTitle;

	[SerializeField]
	private RawImage _jacket;

	[SerializeField]
	private GameObject _new;

	public void SetMusic(Texture2D tex, string musicName, int rank, bool isNew)
	{
		_ranking.ChangeSprite(rank);
		_jacket.texture = tex;
		_musicTitle.SetData(musicName);
		_new.SetActive(isNew);
	}

	public void ViewUpdate()
	{
		_musicTitle.ViewUpdate();
	}
}
