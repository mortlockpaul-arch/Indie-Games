namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface that provides access to an asset's source repository name and file name.
/// </summary>
public interface IContentRepositoryAsset
{
	/// <summary>
	/// Name of the content repository, which contains the asset.
	/// </summary>
	string ContentRepositoryName { get; }

	/// <summary>
	/// Relative path to the file the asset was loaded from.
	/// </summary>
	string SourceAssetFilePath { get; }
}
