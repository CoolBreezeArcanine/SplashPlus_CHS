using System.Linq;
using Manager.Party.Party;
using UnityEngine;
using UnityEngine.UI;

namespace zPlayTest.Scripts.UI
{
	public class UIMember : MonoBehaviour
	{
		[SerializeField]
		private GameObject _rig;

		[SerializeField]
		private Text _info;

		[SerializeField]
		private GameObject[] _mechas;

		private void Update()
		{
			if (!TestCore.IsReady)
			{
				return;
			}
			MechaInfo[] mechaInfo = Party.Get().GetPartyMemberInfo().MechaInfo;
			int num = mechaInfo.Count((MechaInfo i) => i.IsJoin);
			if (num == 0)
			{
				_rig.SetActive(value: false);
				return;
			}
			_rig.SetActive(value: true);
			_info.text = $"筐体: {num}/2";
			for (int j = 0; j < 2; j++)
			{
				if (mechaInfo[j].IsJoin)
				{
					_mechas[j].SetActive(value: true);
					Text[] componentsInChildren = _mechas[j].GetComponentsInChildren<Text>();
					componentsInChildren[0].text = mechaInfo[j].UserNames[0];
					componentsInChildren[1].text = mechaInfo[j].UserNames[1];
				}
				else
				{
					_mechas[j].SetActive(value: false);
				}
			}
		}
	}
}
