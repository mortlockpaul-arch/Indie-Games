using Microsoft.Xna.Framework;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Interface that provides a base for objects exposing scene, frame, and view data to the the lighting system.
/// </summary>
public interface ISceneState
{
	/// <summary>
	/// The scene's current view matrix.
	/// </summary>
	Matrix View { get; }

	/// <summary>
	/// The scene's inverse view matrix.
	/// </summary>
	Matrix ViewToWorld { get; }

	/// <summary>
	/// The scene's current projection matrix.
	/// </summary>
	Matrix Projection { get; }

	/// <summary>
	/// Non-oblique copy of the scene's projection matrix. If the projection matrix is already non-oblique
	/// both ProjectionNonOblique and Projection are equal.
	/// </summary>
	Matrix ProjectionNonOblique { get; }

	/// <summary>
	/// The scene's inverse projection matrix.
	/// </summary>
	Matrix ProjectionToView { get; }

	/// <summary>
	/// The scene's combined view and projection matrix.
	/// </summary>
	Matrix ViewProjection { get; }

	/// <summary>
	/// The scene's combined inverse view and inverse projection matrix.
	/// </summary>
	Matrix ProjectionToWorld { get; }

	/// <summary>
	/// The scene's current view frustum.
	/// </summary>
	BoundingFrustum ViewFrustum { get; }

	/// <summary>
	/// Indicates the rendering pass is drawing to the screen (or to a
	/// target copied to the screen).
	/// </summary>
	bool RenderingToScreen { get; }

	/// <summary>
	/// Determines if primitive culling mode should be flipped to accommodate
	/// inverted windings caused by mirrored view or projection transforms.
	/// </summary>
	bool InvertedWindings { get; }

	/// <summary>
	/// Indicates the projection is 2D.
	/// </summary>
	bool OrthographicProjection { get; }

	/// <summary>
	/// The scene's current game time.
	/// </summary>
	GameTime GameTime { get; }

	/// <summary>
	/// The current frame id.
	/// </summary>
	int FrameId { get; }

	/// <summary>
	/// The scene's current environment.
	/// </summary>
	ISceneEnvironment Environment { get; }

	/// <summary>
	/// Shared buffers used to render the scene.
	/// </summary>
	FrameBuffers FrameBuffers { get; }

	/// <summary />
	void ApplyEditorUpdate(Matrix view, Matrix viewtoworld, Matrix projection);
}
