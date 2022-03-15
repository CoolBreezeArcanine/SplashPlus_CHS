using UnityEngine;

namespace Process.CodeRead
{
	public interface ICodeReadProcess
	{
		int[] SelectCardIndexs { get; }

		int GetCardListID(int playerIndex, int diffIndex);

		int GetGenerateCardNum(int playerIndex);

		bool IsDecisionCard(int playerIndex, int index);

		bool IsCardReaded(int playerIndex);

		bool IsUseReadCard(int playerIndex);

		CodeReadProcess.ReadCard GetReadCard(int playerInde, int cardID);

		CodeReadProcess.ReadCard GetCurrentReadedCard(int playerIndex);

		Sprite GetCardCharacter(int playerIndex, int tyepID);
	}
}
