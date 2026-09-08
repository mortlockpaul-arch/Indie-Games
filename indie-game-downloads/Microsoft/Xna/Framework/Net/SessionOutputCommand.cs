namespace Microsoft.Xna.Framework.Net;

internal enum SessionOutputCommand
{
	SessionStateChanged = 1,
	GamerJoined,
	GamerLeft,
	ReceivedData,
	UpdateSessionInfo,
	UpdateNetworkStats,
	SessionPropertyChanged,
	GameModeChanged,
	HostChanged
}
