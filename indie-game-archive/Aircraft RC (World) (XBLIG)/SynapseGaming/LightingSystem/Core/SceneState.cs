using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Provides scene, frame, and view specific data to the the lighting system.
/// </summary>
public class SceneState : ISceneState
{
	private FrameBuffers HCB;

	private Matrix HC_0002 = Matrix.Identity;

	private Matrix HC_0012 = Matrix.Identity;

	private Matrix HCH = Matrix.Identity;

	private Matrix HC7 = Matrix.Identity;

	private Matrix HC_0001 = Matrix.Identity;

	private Matrix HCw = Matrix.Identity;

	private Matrix HCZ = Matrix.Identity;

	private BoundingFrustum HC_000F = new BoundingFrustum(Matrix.Identity);

	private bool HCy = true;

	private bool HC6;

	private bool HCD;

	private GameTime HC_0011 = new GameTime();

	private int HCK;

	private ISceneEnvironment HC_0003 = HCk;

	private static ISceneEnvironment HCk = new SceneEnvironment();

	private static ISceneEnvironment HCs = new SceneEnvironment();

	/// <summary>
	/// The scene's current view matrix.
	/// </summary>
	public Matrix View => HC_0002;

	/// <summary>
	/// The scene's inverse view matrix.
	/// </summary>
	public Matrix ViewToWorld => HC_0012;

	/// <summary>
	/// The scene's current projection matrix.
	/// </summary>
	public Matrix Projection => HCH;

	/// <summary>
	/// Non-oblique copy of the scene's projection matrix. If the projection matrix is already non-oblique
	/// both ProjectionNonOblique and Projection are equal.
	/// </summary>
	public Matrix ProjectionNonOblique => HC7;

	/// <summary>
	/// The scene's inverse projection matrix.
	/// </summary>
	public Matrix ProjectionToView => HC_0001;

	/// <summary>
	/// The scene's combined view and projection matrix.
	/// </summary>
	public Matrix ViewProjection => HCw;

	/// <summary>
	/// The scene's combined inverse view and inverse projection matrix.
	/// </summary>
	public Matrix ProjectionToWorld => HCZ;

	/// <summary>
	/// The scene's current view frustum.
	/// </summary>
	public BoundingFrustum ViewFrustum => HC_000F;

	/// <summary>
	/// Indicates the rendering pass is drawing to the screen (or to a
	/// target copied to the screen).
	/// </summary>
	public bool RenderingToScreen => HCy;

	/// <summary>
	/// Determines if primitive culling mode should be flipped to accommodate
	/// inverted windings caused by mirrored view or projection transforms.
	/// </summary>
	public bool InvertedWindings => HC6;

	/// <summary>
	/// Indicates the projection is 2D.
	/// </summary>
	public bool OrthographicProjection => HCD;

	/// <summary>
	/// The scene's current game time.
	/// </summary>
	public GameTime GameTime => HC_0011;

	/// <summary>
	/// The current frame id.
	/// </summary>
	public int FrameId => HCK;

	/// <summary>
	/// The scene's current environment.
	/// </summary>
	public ISceneEnvironment Environment => HC_0003;

	/// <summary>
	/// Shared buffers used to render the scene.
	/// </summary>
	public FrameBuffers FrameBuffers => HCB;

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
		ISceneEnvironment sceneEnvironment = ((environment == null) ? HCs : environment);
		sceneEnvironment.ShadowCasterDistance = num2;
		sceneEnvironment.ShadowFadeEndDistance = num2;
		sceneEnvironment.ShadowFadeStartDistance = num2;
		sceneEnvironment.VisibleDistance = num2;
		BeginFrameRendering(view, projection, gametime, sceneEnvironment, framebuffers, renderingtoscreen);
		HCD = true;
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
		SplashScreen._7z();
		HC_0002 = view;
		HC_0012 = Matrix.Invert(view);
		HCH = projectionoblique;
		HC_0001 = Matrix.Invert(projectionoblique);
		HC7 = projection;
		HCw = view * projectionoblique;
		HCZ = HC_0001 * HC_0012;
		HC_0011 = gametime;
		HCy = renderingtoscreen;
		HC6 = HCw.Determinant() >= 0f;
		HCD = false;
		HCB = framebuffers;
		HCB.BeginFrameRendering(this);
		if (environment != null)
		{
			HC_0003 = environment;
		}
		else
		{
			HC_0003 = HCk;
		}
		HC_000F.Matrix = view * projection;
		HCK++;
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
		HCB.EndFrameRendering();
	}

	/// <summary />
	public void ApplyEditorUpdate(Matrix view, Matrix viewtoworld, Matrix projection)
	{
		BeginFrameRendering(view, projection, HC_0011, HC_0003, HCB, HCy);
	}
}
