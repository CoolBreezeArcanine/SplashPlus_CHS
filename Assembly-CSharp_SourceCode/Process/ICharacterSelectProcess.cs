using System.Collections.Generic;

namespace Process
{
	public interface ICharacterSelectProcess
	{
		CharacterData GetSlotData(int playerIndex, int slotIndex);

		bool IsSelectedCharacter(int playerIndex, int id);

		bool IsLeaderCharacter(int playerIndex, int id);

		CharacterData GetUserCharaData(int playerIndex, int diffIndex);

		bool IsChangeCharacterCategory(int playerIndex, int diffIndex, out int overCount);

		string GetCharacterCategoryName(int playerIndex, int diffIndex);

		CharacterMapColorData GetMapColorData(int colorId);

		int GetCurrentCharacterListMax(int playerIndex);

		int GetCurrentCharacterIndex(int playerIndex);

		int GetCurrentCategoryIndex(int playerIndex);

		bool IsMatchColor(int playerIndex, int ID);

		List<CharacterTabData> GetTabDataList(int playerIndex);

		CharacterSelectProces.ArrowDirection GetArrowDirection(int playerIndex, int checkDistance);

		void StartCountUp(int playerIndex);

		CharacterData GetNewcomerData(int playerIndex);

		bool IsNewcomerModeSlotActive(int playerIndex, int slotIndex);

		bool IsLockedSlot(int playerIndex, int slotIndex);
	}
}
