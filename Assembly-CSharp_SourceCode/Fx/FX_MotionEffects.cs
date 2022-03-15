using System.Collections.Generic;
using UnityEngine;

namespace FX
{
	public class FX_MotionEffects : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("1.『pos_fx』に、子階層の pos_fx を\r\n\tドラッグして入れてください。\r\n2.『FX Prefabs』を開いて種類分の個数を指定し、空欄に\r\nエフェクトのPrefabをドラッグして入れてください。\r\n3.エフェクトを発生させたいモーションのフレームに、\r\nAdd event ボタンでイベントを追加してください。\r\n4.3のイベントのインスペクターから、『Function』に\r\nFX_Muzzle()』を設定してください。\r\nエフェクトを複数設定した場合は\r\n『int』に、Element番号を指定してください。")]
		private Transform pos_fx;

		public List<GameObject> FXPrefabs;

		private void Start()
		{
		}

		private void Effect(int Element)
		{
			Object.Instantiate(FXPrefabs[Element]).transform.SetParent(pos_fx.transform, worldPositionStays: false);
		}
	}
}
