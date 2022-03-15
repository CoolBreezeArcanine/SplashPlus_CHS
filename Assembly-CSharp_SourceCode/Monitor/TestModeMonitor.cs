using Monitor.TestMode.SubSequence;
using UnityEngine;

namespace Monitor
{
	public class TestModeMonitor : MonitorBase
	{
		[SerializeField]
		private GameObject _testModeRootPrefub;

		private TestModePageTop _testModeTop;

		public bool IsExit;

		public TestModePage.Result Result;

		public override void Initialize(int monIndex, bool active)
		{
			if (monIndex == 0)
			{
				base.Initialize(monIndex, active);
				GameObject gameObject = Object.Instantiate(_testModeRootPrefub);
				gameObject.transform.SetParent(Main.transform, worldPositionStays: false);
				_testModeTop = gameObject.GetComponent<TestModePageTop>();
				_testModeTop.SetParent(this);
				IsExit = false;
				Result = TestModePage.Result.GoBack;
			}
		}

		public override void ViewUpdate()
		{
		}
	}
}
