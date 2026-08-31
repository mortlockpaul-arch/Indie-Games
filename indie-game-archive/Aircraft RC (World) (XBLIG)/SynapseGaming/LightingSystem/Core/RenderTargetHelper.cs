using System;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Core;

/// <summary>
/// Helper class for rendering to a texture. Provides automatic support for rendering
/// reflection and refraction textures, as well as standard render-to-texture.
/// </summary>
public class RenderTargetHelper : IUnloadable
{
	/// <summary>
	/// Type of rendering to perform on the render target.
	/// </summary>
	public enum TargetType
	{
		/// <summary>
		/// Automatically generates a reflection image based on the current view and reflection plane.
		/// </summary>
		Reflection,
		/// <summary>
		/// Automatically generates a refraction image based on the current view and reflection plane.
		/// </summary>
		Refraction,
		/// <summary>
		/// Renders to texture normally based on the current view.
		/// </summary>
		Standard
	}

	private int HCB;

	private int HC_0002;

	private bool HC_0012;

	private SurfaceFormat HCH;

	private int HC7;

	private RenderTargetUsage HC_0001;

	private SceneState HCw = new SceneState();

	private TargetType HCZ = TargetType.Standard;

	private RenderTarget2D HC_000F;

	private Viewport HCy = default(Viewport);

	private ISystemPreferences HC6 = new SystemPreferences();

	private Plane HCD = default(Plane);

	private RenderTargetBinding[] HC_0011;

	private Viewport HCK;

	private RenderTargetBinding[] HC_0003 = new RenderTargetBinding[1];

	/// <summary>
	/// Scene rendering state used to render objects to this RenderTargetHelper. The state values
	/// may be different from those passed into BeginFrameRendering to accommodate reflection and refraction.
	/// </summary>
	public ISceneState SceneState => HCw;

	/// <summary>
	/// Rendering preferences used to render objects to this RenderTargetHelper.
	/// </summary>
	public ISystemPreferences Preferences => HC6;

	/// <summary>
	/// Creates a new RenderTargetHelper instance.
	/// </summary>
	/// <param name="type">Type of rendering to perform on the render target.</param>
	/// <param name="width">Render target width.</param>
	/// <param name="height">Render target height.</param>
	/// <param name="format">Render target format.</param>
	public RenderTargetHelper(TargetType type, int width, int height, SurfaceFormat format)
	{
		HCZ = type;
		HCB = width;
		HC_0002 = height;
		HCH = format;
		HC_0012 = false;
		HC7 = 0;
		HC_0001 = SunBurnCoreSystem.Instance.GetBestRenderTargetUsage();
	}

	/// <summary>
	/// Creates a new RenderTargetHelper instance.
	/// </summary>
	/// <param name="type">Type of rendering to perform on the render target.</param>
	/// <param name="width">Render target width.</param>
	/// <param name="height">Render target height.</param>
	/// <param name="mipmapped">Determines if the render target generates mipmaps.</param>
	/// <param name="format">Render target format.</param>
	/// <param name="multisamplecount">Render target multisample quality.</param>
	/// <param name="usage">Render target usage.</param>
	public RenderTargetHelper(TargetType type, int width, int height, bool mipmapped, SurfaceFormat format, int multisamplecount, RenderTargetUsage usage)
	{
		HCZ = type;
		HCB = width;
		HC_0002 = height;
		HC_0012 = mipmapped;
		HCH = format;
		HC7 = multisamplecount;
		HC_0001 = usage;
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
		HC6 = preferences;
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
		HC6 = new SystemPreferences();
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		Clear();
		F.B._7_0004(ref HC_000F);
	}

	/// <summary>
	/// Gets the texture containing the resulting rendered image.
	/// </summary>
	/// <returns></returns>
	public Texture2D GetTexture()
	{
		return HC_000F;
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		if (HCZ != TargetType.Standard)
		{
			throw new Exception("Non standard targets require a world reflection plane, please use another overload for this method.");
		}
		BeginFrameRendering(scenestate, HCD);
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	/// <param name="worldreflectionplane">World space plane used as the reflection surface.</param>
	public void BeginFrameRendering(ISceneState scenestate, Plane worldreflectionplane)
	{
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		if (HC_000F == null)
		{
			HC_000F = new RenderTarget2D(graphicsDevice, HCB, HC_0002, HC_0012, HCH, DepthFormat.Depth24Stencil8, HC7, HC_0001);
			HCy.X = 0;
			HCy.Y = 0;
			HCy.Width = HCB;
			HCy.Height = HC_0002;
			HCy.MinDepth = 0f;
			HCy.MaxDepth = 1f;
		}
		HC_0011 = graphicsDevice.GetRenderTargets();
		HCK = graphicsDevice.Viewport;
		ref RenderTargetBinding reference = ref HC_0003[0];
		reference = new RenderTargetBinding(HC_000F);
		graphicsDevice.SetRenderTargets(HC_0003);
		graphicsDevice.Viewport = HCy;
		if (HCZ != TargetType.Reflection)
		{
			Matrix projection = scenestate.Projection;
			Matrix projectionoblique = projection;
			if (HCZ != TargetType.Standard)
			{
				projectionoblique = _0002q(projection, scenestate.ProjectionToWorld, worldreflectionplane);
			}
			HCw.BeginFrameRendering(scenestate.View, projection, projectionoblique, scenestate.GameTime, scenestate.Environment, scenestate.FrameBuffers, scenestate.RenderingToScreen);
		}
		else
		{
			Matrix matrix = Matrix.CreateReflection(worldreflectionplane) * scenestate.View;
			Matrix projection2 = scenestate.Projection;
			Matrix matrix2 = Matrix.Invert(matrix * projection2);
			Matrix projectionoblique2 = _0002q(projection2, matrix2, worldreflectionplane);
			HCw.BeginFrameRendering(matrix, projection2, projectionoblique2, scenestate.GameTime, scenestate.Environment, scenestate.FrameBuffers, scenestate.RenderingToScreen);
		}
	}

	private Matrix _0002q(Matrix P_0, Matrix P_1, Plane P_2)
	{
		Matrix.Transpose(ref P_1, out var result);
		Vector4 vector = new Vector4(P_2.Normal, P_2.D);
		Vector4.Transform(ref vector, ref result, out var result2);
		if (result2.W == 0f)
		{
			return P_0;
		}
		if (result2.W > 0f)
		{
			result2 = Vector4.Transform(-vector, result);
		}
		Matrix identity = Matrix.Identity;
		float num = Vector4.Dot(vector2: new Vector4(Math.Sign(result2.X), Math.Sign(result2.Y), 1f, 1f), vector1: result2);
		if (num == 0f)
		{
			return P_0;
		}
		result2 *= 1f / num;
		identity.M13 = result2.X;
		identity.M23 = result2.Y;
		identity.M33 = result2.Z;
		identity.M43 = result2.W;
		return P_0 * identity;
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public void EndFrameRendering()
	{
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		graphicsDevice.SetRenderTargets(HC_0011);
		graphicsDevice.Viewport = HCK;
	}
}
