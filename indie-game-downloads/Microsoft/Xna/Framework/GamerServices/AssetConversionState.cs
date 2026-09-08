namespace Microsoft.Xna.Framework.GamerServices;

internal enum AssetConversionState
{
	Default,
	Downloading,
	Initialize,
	ConvertAssets,
	DownloadFailed,
	NoAvatar,
	Unavailable,
	Finished
}
