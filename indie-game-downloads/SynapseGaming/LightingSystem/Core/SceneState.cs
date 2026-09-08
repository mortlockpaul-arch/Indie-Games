using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides scene, frame, and view specific data to the the lighting system.
/// </summary>
public class SceneState : ISceneState
{
	private FrameBuffers _3A_0018;

	private Matrix _3AL = Matrix.Identity;

	private Matrix _3A_0019 = Matrix.Identity;

	private Matrix _3A3 = Matrix.Identity;

	private Matrix _3A6 = Matrix.Identity;

	private Matrix _3AD = Matrix.Identity;

	private Matrix _3A_0017 = Matrix.Identity;

	private Matrix _3A_0003 = Matrix.Identity;

	private BoundingFrustum _3Al = new BoundingFrustum(Matrix.Identity);

	private bool _3At = true;

	private bool _3AF;

	private bool _3Ac;

	private GameTime _3Ag = new GameTime();

	private int _3AI;

	private ISceneEnvironment _3A8 = _3AZ;

	private static ISceneEnvironment _3AZ = new SceneEnvironment();

	private static ISceneEnvironment _3Ax = new SceneEnvironment();

	/// <summary>
	/// The scene's current view matrix.
	/// </summary>
	public Matrix View => _3AL;

	/// <summary>
	/// The scene's inverse view matrix.
	/// </summary>
	public Matrix ViewToWorld => _3A_0019;

	/// <summary>
	/// The scene's current projection matrix.
	/// </summary>
	public Matrix Projection => _3A3;

	/// <summary>
	/// Non-oblique copy of the scene's projection matrix. If the projection matrix is already non-oblique
	/// both ProjectionNonOblique and Projection are equal.
	/// </summary>
	public Matrix ProjectionNonOblique => _3A6;

	/// <summary>
	/// The scene's inverse projection matrix.
	/// </summary>
	public Matrix ProjectionToView => _3AD;

	/// <summary>
	/// The scene's combined view and projection matrix.
	/// </summary>
	public Matrix ViewProjection => _3A_0017;

	/// <summary>
	/// The scene's combined inverse view and inverse projection matrix.
	/// </summary>
	public Matrix ProjectionToWorld => _3A_0003;

	/// <summary>
	/// The scene's current view frustum.
	/// </summary>
	public BoundingFrustum ViewFrustum => _3Al;

	/// <summary>
	/// Indicates the rendering pass is drawing to the screen (or to a
	/// target copied to the screen).
	/// </summary>
	public bool RenderingToScreen => _3At;

	/// <summary>
	/// Determines if primitive culling mode should be flipped to accommodate
	/// inverted windings caused by mirrored view or projection transforms.
	/// </summary>
	public bool InvertedWindings => _3AF;

	/// <summary>
	/// Indicates the projection is 2D.
	/// </summary>
	public bool OrthographicProjection => _3Ac;

	/// <summary>
	/// The scene's current game time.
	/// </summary>
	public GameTime GameTime => _3Ag;

	/// <summary>
	/// The current frame id.
	/// </summary>
	public int FrameId => _3AI;

	/// <summary>
	/// The scene's current environment.
	/// </summary>
	public ISceneEnvironment Environment => _3A8;

	/// <summary>
	/// Shared buffers used to render the scene.
	/// </summary>
	public FrameBuffers FrameBuffers => _3A_0018;

	/// <summary>
	/// Sets up the scene state prior to 2D rendering.
	/// </summary>
	/// <param name="viewposition">World space position of the 2D camera.</param>
	/// <param name="viewwidth">Number of world space units visible across the
	/// width of the viewport.</param>
	/// <param name="aspectratio">Aspect ratio of the viewport.</param>
	/// <param name="gametime">Current game time.</param>
	/// <param name="environment">Environment object used while rendering.</param>
	/// <param name="framebuffers">Shared buffers used to render the scene.</param>
	/// <param name="renderingtoscreen">Indicates the rendering pass is drawing
	/// to the screen (or to a target copied to the screen).</param>
	public void BeginFrameRendering(Vector2 viewposition, float viewwidth, float aspectratio, GameTime gametime, ISceneEnvironment environment, FrameBuffers framebuffers, bool renderingtoscreen)
	{
		float num = viewwidth * 2.5f;
		float num2 = num * 10f;
		Matrix view = Matrix.CreateLookAt(new Vector3(viewposition, 0f - num), new Vector3(viewposition, 0f), Vector3.Up);
		Matrix projection = Matrix.CreatePerspective(viewwidth, viewwidth / aspectratio, num, num2);
		ISceneEnvironment sceneEnvironment = ((environment == null) ? _3Ax : environment);
		sceneEnvironment.ShadowCasterDistance = num2;
		sceneEnvironment.ShadowFadeEndDistance = num2;
		sceneEnvironment.ShadowFadeStartDistance = num2;
		sceneEnvironment.VisibleDistance = num2;
		BeginFrameRendering(view, projection, gametime, sceneEnvironment, framebuffers, renderingtoscreen);
		_3Ac = true;
	}

	/// <summary>
	/// Sets up the scene state prior to 3D rendering. Includes support for oblique projection.
	/// </summary>
	/// <param name="view">Camera view matrix.</param>
	/// <param name="projection">Camera projection matrix. Must be a non-oblique version of the projection matrix.</param>
	/// <param name="projectionoblique">Camera projection matrix.</param>
	/// <param name="gametime">Current game time.</param>
	/// <param name="environment">Environment object used while rendering.</param>
	/// <param name="framebuffers">Shared buffers used to render the scene.</param>
	/// <param name="renderingtoscreen">Indicates the rendering pass is drawing
	/// to the screen (or to a target copied to the screen).</param>
	public void BeginFrameRendering(Matrix view, Matrix projection, Matrix projectionoblique, GameTime gametime, ISceneEnvironment environment, FrameBuffers framebuffers, bool renderingtoscreen)
	{
		SplashScreen._6b();
		_3AL = view;
		_3A_0019 = Matrix.Invert(view);
		_3A3 = projectionoblique;
		_3AD = Matrix.Invert(projectionoblique);
		_3A6 = projection;
		_3A_0017 = view * projectionoblique;
		_3A_0003 = _3AD * _3A_0019;
		_3Ag = gametime;
		_3At = renderingtoscreen;
		_3AF = _3A_0017.Determinant() >= 0f;
		_3Ac = false;
		_3A_0018 = framebuffers;
		_3A_0018.BeginFrameRendering(this);
		if (environment != null)
		{
			_3A8 = environment;
		}
		else
		{
			_3A8 = _3AZ;
		}
		_3Al.Matrix = view * projection;
		_3AI++;
	}

	/// <summary>
	/// Sets up the scene state prior to 3D rendering.
	/// </summary>
	/// <param name="view">Camera view matrix.</param>
	/// <param name="projection">Camera projection matrix. Must be a non-oblique version of the projection matrix.</param>
	/// <param name="gametime">Current game time.</param>
	/// <param name="environment">Environment object used while rendering.</param>
	/// <param name="framebuffers">Shared buffers used to render the scene.</param>
	/// <param name="renderingtoscreen">Indicates the rendering pass is drawing
	/// to the screen (or to a target copied to the screen).</param>
	public void BeginFrameRendering(Matrix view, Matrix projection, GameTime gametime, ISceneEnvironment environment, FrameBuffers framebuffers, bool renderingtoscreen)
	{
		BeginFrameRendering(view, projection, projection, gametime, environment, framebuffers, renderingtoscreen);
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
		_3A_0018.EndFrameRendering();
	}

	/// <summary />
	public void ApplyEditorUpdate(Matrix view, Matrix viewtoworld, Matrix projection)
	{
		BeginFrameRendering(view, projection, _3Ag, _3A8, _3A_0018, _3At);
	}
}
