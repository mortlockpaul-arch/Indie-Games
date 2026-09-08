using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Provides an asset wrapper for XNA Models containing the source repository name, file name, and direct access
/// to the loaded asset. When visible in the SunBurn editor properties of this type automatically support drag
/// and drop of repository models into the property.
///
/// The content repository provided must be loaded before creating an instance of
/// this class otherwise the asset will fail to load.
/// </summary>
public class ModelAsset : ContentRepositoryAsset<Model>
{
	/// <summary>
	/// Provides an empty ModelAsset which can be used to initialize properties of this type.
	/// </summary>
	public static readonly ModelAsset Empty = new ModelAsset();

	[CompilerGenerated]
	private string _3A_0018;

	/// <summary>
	/// Specific ModelMesh that should be used when constructing objects from this ModelAsset.
	///
	/// If SourceAssetFilePath is valid and this value is empty then the object should use the entire Model.
	/// </summary>
	public string ModelMeshName
	{
		[CompilerGenerated]
		get
		{
			return _3A_0018;
		}
		[CompilerGenerated]
		private set
		{
			_3A_0018 = text;
		}
	}

	private ModelAsset()
	{
		ModelMeshName = string.Empty;
	}

	/// <summary>
	/// Creates a new ModelAsset instance and loads the provided model.
	/// </summary>
	/// <param name="repositoryname">Name of the content repository, which contains the asset.</param>
	/// <param name="sourceassetfilepath">Relative path to the file the asset is loaded from.</param>
	/// <param name="modelmeshname">Specific ModelMesh that should be used when constructing objects from this
	/// ModelAsset. Set to null or empty to use the entire model.</param>
	public ModelAsset(string repositoryname, string sourceassetfilepath, string modelmeshname)
		: base(repositoryname, sourceassetfilepath)
	{
		if (string.IsNullOrEmpty(modelmeshname))
		{
			modelmeshname = string.Empty;
		}
		ModelMeshName = modelmeshname;
	}
}
