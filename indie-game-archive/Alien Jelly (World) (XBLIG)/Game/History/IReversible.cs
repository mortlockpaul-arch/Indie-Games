using Microsoft.Xna.Framework;

namespace Game.History;

public interface IReversible
{
	void History_Reverse(ref HistoryItem oItem, float xRatio, GameTime oGameTime);

	void History_Set(ref HistoryItemData oItem, HistoryItem.Action oAction);

	bool History_IsNotInteruptable(HistoryItem.Action oAction);

	void History_Event_Reverse_Start(ref HistoryItem oItem);

	void History_Event_Reverse_End(ref HistoryItem oItem);

	void History_Event_ForceClose(ref HistoryItem oItem);

	void History_Event_Resume(ref HistoryItem oItem);

	void History_Event_Lock();

	void History_Event_Unlock();
}
