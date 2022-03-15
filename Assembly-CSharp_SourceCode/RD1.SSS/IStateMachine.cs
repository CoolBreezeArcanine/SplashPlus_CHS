using System.Text;

namespace RD1.SSS
{
	public interface IStateMachine
	{
		bool isStateEnd();

		bool updateState(float deltaTime);

		string getStateName();

		void getStateName(StringBuilder stringBuilder);

		int getStateResult();
	}
}
