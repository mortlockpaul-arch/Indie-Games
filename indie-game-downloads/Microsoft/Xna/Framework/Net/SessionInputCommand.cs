namespace Microsoft.Xna.Framework.Net;

internal enum SessionInputCommand
{
	SendData = 1,
	EnableSendVoice,
	SetIsReady,
	StartGame,
	EndGame,
	ResetReady,
	RemoveMachine,
	SetGamerSlots,
	SetJoinInProgress,
	SetHostMigration,
	SetSessionProperty,
	SetGameMode
}
