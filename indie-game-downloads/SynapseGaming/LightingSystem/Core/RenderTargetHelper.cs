using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using p;

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

	private int _3A_0018;

	private int _3AL;

	private bool _3A_0019;

	private SurfaceFormat _3A3;

	private int _3A6;

	private RenderTargetUsage _3AD;

	private SceneState _3A_0017 = new SceneState();

	private TargetType _3A_0003 = TargetType.Standard;

	private RenderTarget2D _3Al;

	private Viewport _3At = default(Viewport);

	private ISystemPreferences _3AF = new SystemPreferences();

	private Plane _3Ac = default(Plane);

	private RenderTargetBinding[] _3Ag;

	private Viewport _3AI;

	private RenderTargetBinding[] _3A8 = new RenderTargetBinding[1];

	/// <summary>
	/// Scene rendering state used to render objects to this RenderTargetHelper. The state values
	/// may be different from those passed into BeginFrameRendering to accommodate reflection and refraction.
	/// </summary>
	public ISceneState SceneState => _3A_0017;

	/// <summary>
	/// Rendering preferences used to render objects to this RenderTargetHelper.
	/// </summary>
	public ISystemPreferences Preferences => _3AF;

	/// <summary>
	/// Creates a new RenderTargetHelper instance.
	/// </summary>
	/// <param name="type">Type of rendering to perform on the render target.</param>
	/// <param name="width">Render target width.</param>
	/// <param name="height">Render target height.</param>
	/// <param name="format">Render target format.</param>
	public RenderTargetHelper(TargetType type, int width, int height, SurfaceFormat format)
	{
		_3A_0003 = type;
		_3A_0018 = width;
		_3AL = height;
		_3A3 = format;
		_3A_0019 = false;
		_3A6 = 0;
		_3AD = SunBurnCoreSystem.Instance.GetBestRenderTargetUsage();
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
		_3A_0003 = type;
		_3A_0018 = width;
		_3AL = height;
		_3A_0019 = mipmapped;
		_3A3 = format;
		_3A6 = multisamplecount;
		_3AD = usage;
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public void ApplyPreferences(ISystemPreferences preferences)
	{
		_3AF = preferences;
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public void Clear()
	{
		_3AF = new SystemPreferences();
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		Clear();
		p._0018._6_0006(ref _3Al);
	}

	/// <summary>
	/// Gets the texture containing the resulting rendered image.
	/// </summary>
	/// <returns></returns>
	public Texture2D GetTexture()
	{
		return _3Al;
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		if (_3A_0003 != TargetType.Standard)
		{
			throw new Exception("Non standard targets require a world reflection plane, please use another overload for this method.");
		}
		BeginFrameRendering(scenestate, _3Ac);
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	/// <param name="worldreflectionplane">World space plane used as the reflection surface.</param>
	public void BeginFrameRendering(ISceneState scenestate, Plane worldreflectionplane)
	{
		GraphicsDevice graphicsDevice = SunBurnCoreSystem.Instance.GraphicsDeviceManager.GraphicsDevice;
		if (_3Al == null)
		{
			_3Al = new RenderTarget2D(graphicsDevice, _3A_0018, _3AL, _3A_0019, _3A3, DepthFormat.Depth24Stencil8, _3A6, _3AD);
			_3At.X = 0;
			_3At.Y = 0;
			_3At.Width = _3A_0018;
			_3At.Height = _3AL;
			_3At.MinDepth = 0f;
			_3At.MaxDepth = 1f;
		}
		_3Ag = graphicsDevice.GetRenderTargets();
		_3AI = graphicsDevice.Viewport;
		ref RenderTargetBinding reference = ref _3A8[0];
		reference = new RenderTargetBinding(_3Al);
		graphicsDevice.SetRenderTargets(_3A8);
		graphicsDevice.Viewport = _3At;
		if (_3A_0003 != TargetType.Reflection)
		{
			Matrix projection = scenestate.Projection;
			Matrix projectionoblique = projection;
			if (_3A_0003 != TargetType.Standard)
			{
				projectionoblique = L_0010(projection, scenestate.ProjectionToWorld, worldreflectionplane);
			}
			_3A_0017.BeginFrameRendering(scenestate.View, projection, projectionoblique, scenestate.GameTime, scenestate.Environment, scenestate.FrameBuffers, scenestate.RenderingToScreen);
		}
		else
		{
			Matrix matrix = Matrix.CreateReflection(worldreflectionplane) * scenestate.View;
			Matrix projection2 = scenestate.Projection;
			Matrix matrix2 = Matrix.Invert(matrix * projection2);
			Matrix projectionoblique2 = L_0010(projection2, matrix2, worldreflectionplane);
			_3A_0017.BeginFrameRendering(matrix, projection2, projectionoblique2, scenestate.GameTime, scenestate.Environment, scenestate.FrameBuffers, scenestate.RenderingToScreen);
		}
	}

	private Matrix L_0010(Matrix P_0, Matrix P_1, Plane P_2)
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
		graphicsDevice.SetRenderTargets(_3Ag);
		graphicsDevice.Viewport = _3AI;
	}
}
