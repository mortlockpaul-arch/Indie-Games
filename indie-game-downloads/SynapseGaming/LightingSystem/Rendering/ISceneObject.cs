using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Interface used by objects contained within the IObjectManager
/// manager service.
///
/// In many cases these object are renderable, however non-renderable
/// objects can also use this interface and be stored within the
/// IObjectManager manager service.
/// </summary>
public interface ISceneObject : ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject
{
	/// <summary>
	/// Provides direct access to the repository name, file name, and model
	/// the scene object was created from. Only valid for serialized scene objects
	/// created via the SunBurn editor.
	/// </summary>
	ModelAsset ModelAsset { get; set; }

	/// <summary>
	/// Determines if an object uses light mapping, approximate lighting, or no lighting
	/// to receive illumination from BakedDown light sources.
	/// </summary>
	StaticLightingType StaticLightingType { get; set; }

	/// <summary>
	/// Determines the light map size when generating baked down lighting on the object.
	/// </summary>
	LightMapSize LightMapSize { get; set; }

	/// <summary>
	/// Specifies the lighting color used when the StaticLightingType is set to Custom.
	/// </summary>
	Vector3 CustomStaticLightingColor { get; set; }

	/// <summary>
	/// Indicates the object's meshes are capable of using light maps.
	/// </summary>
	bool CanLightMap { get; }

	/// <summary>
	/// Indicates the object is rendering without errors.
	/// </summary>
	bool Valid { get; set; }

	/// <summary>
	/// Contains any errors that occurred during rendering.
	/// </summary>
	string RenderingErrors { get; set; }

	/// <summary>
	/// Determines if the object casts shadows based on the current ObjectVisibility options.
	/// </summary>
	bool CastShadows { get; }

	/// <summary>
	/// Determines if the object is visible based on the current ObjectVisibility options.
	/// </summary>
	bool Visible { get; }

	/// <summary>
	/// Determines if the object is visible in the editor based on the current ObjectVisibility options.
	/// </summary>
	bool VisibleInEditor { get; }

	/// <summary>
	/// Defines how the object is rendered.
	///
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders the object and casts shadows from it).
	/// </summary>
	ObjectVisibility Visibility { get; set; }

	/// <summary>
	/// Array of bone transforms used to form the skeleton's current pose. The array
	/// index of a bone matrix should match the vertex buffer bone index.
	/// </summary>
	Matrix[] SkinBones { get; set; }

	/// <summary>
	/// Collection of the object's internal mesh parts.
	/// </summary>
	RenderableMeshCollection RenderableMeshes { get; }
}
