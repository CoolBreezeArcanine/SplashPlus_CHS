using IO;
using UnityEngine;

namespace Monitor
{
	public class DataSaveMonitor : MonitorBase
	{
		public enum EntryStatus
		{
			None,
			Entry,
			NotEntry,
			End
		}

		[SerializeField]
		private Animator _anim;

		private int _baseLayer;

		private int _outLayer;

		public override void Initialize(int monIndex, bool isActive)
		{
			base.Initialize(monIndex, isActive);
			if (IsActive())
			{
				MechaManager.LedIf[monIndex].ButtonLedReset();
			}
			if (!isActive)
			{
				Main.gameObject.SetActive(value: false);
				Sub.gameObject.SetActive(value: false);
			}
			else
			{
				_baseLayer = _anim.GetLayerIndex("Base Layer");
				_outLayer = _anim.GetLayerIndex("Out Layer");
			}
		}

		public override void ViewUpdate()
		{
		}

		public void AnimStartIn()
		{
			_anim.Play("In", _baseLayer);
		}

		public void AnimStartOut()
		{
			_anim.Play("Out", _outLayer);
		}
	}
}
