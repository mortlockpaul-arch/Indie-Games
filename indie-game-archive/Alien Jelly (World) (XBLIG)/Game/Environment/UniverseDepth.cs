using GKEngine;
using GKEngine.Cameras;
using GKEngine.Core;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Environment;

public class UniverseDepth
{
	public const string PATH_EFFECT_SINGLE = "Content/Effects/Pre/Depth_Single";

	public const string PATH_EFFECT_INSTANCED = "Content/Effects/Pre/Depth_Instanced";

	public const float SHRINK = 1f;

	public const float NEAR = 100f;

	public const float FAR = 1000f;

	public Scene scene;

	public EffectParameter targetEffectParam;

	public bool active = true;

	protected Effect effectInstanced;

	protected EffectParameter effectInstanced_View;

	protected EffectParameter effectInstanced_Proj;

	protected EffectParameter effectInstanced_FocalLength;

	protected Effect effectSingle;

	protected EffectParameter effectSingle_View;

	protected EffectParameter effectSingle_Proj;

	protected EffectParameter effectSingle_FocalLength;

	public RenderTarget2D target;

	private DepthStencilState depthStencilState = new DepthStencilState();

	private BlendState blendState = new BlendState();

	protected Point _size = new Point(GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);

	public Point size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			target = new RenderTarget2D(GameEngine.Graphics.GraphicsDevice, (int)((float)_size.X * 1f), (int)((float)_size.Y * 1f), mipMap: false, SurfaceFormat.Color, DepthFormat.Depth24);
		}
	}

	public UniverseDepth(Scene oScene)
	{
		scene = oScene;
		targetEffectParam = null;
		Load();
	}

	public virtual void Load()
	{
		size = new Point(GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);
		depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferEnable = true;
		depthStencilState.DepthBufferWriteEnable = true;
		depthStencilState.DepthBufferFunction = CompareFunction.LessEqual;
		blendState = BlendState.Opaque;
		effectInstanced = GameEngine.SceneContent.Load<Effect>("Content/Effects/Pre/Depth_Instanced").Clone();
		effectSingle = GameEngine.SceneContent.Load<Effect>("Content/Effects/Pre/Depth_Single").Clone();
		effectInstanced.Parameters["near"].SetValue(100f);
		effectInstanced.Parameters["far"].SetValue(1000f);
		effectSingle.Parameters["near"].SetValue(100f);
		effectSingle.Parameters["far"].SetValue(1000f);
		effectInstanced_View = effectInstanced.Parameters["View"];
		effectInstanced_Proj = effectInstanced.Parameters["Proj"];
		effectInstanced_FocalLength = effectInstanced.Parameters["focalLength"];
		effectSingle_View = effectSingle.Parameters["View"];
		effectSingle_Proj = effectSingle.Parameters["Proj"];
		effectSingle_FocalLength = effectSingle.Parameters["focalLength"];
	}

	public void Render(GameTime oGameTime)
	{
		if (active)
		{
			Render_Map();
		}
	}

	private void Render_Map()
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		GameEngine.Graphics.GraphicsDevice.SetRenderTarget(target);
		GameEngine.Graphics.GraphicsDevice.Clear(Renderer.COLOR_BLANK);
		graphicsDevice.BlendState = blendState;
		graphicsDevice.DepthStencilState = depthStencilState;
		Render_SetParams();
		Render_DrawItems();
		GameEngine.Graphics.GraphicsDevice.SetRenderTarget(null);
		targetEffectParam.SetValue(target);
	}

	protected virtual void Render_SetParams()
	{
		Camera camera = scene.cameras.camera;
		effectInstanced_View.SetValue(camera.view);
		effectInstanced_Proj.SetValue(camera.projection);
		effectInstanced_FocalLength.SetValue(camera.focalLength);
		effectSingle_View.SetValue(camera.view);
		effectSingle_Proj.SetValue(camera.projection);
		effectSingle_FocalLength.SetValue(camera.focalLength);
	}

	protected virtual void Render_DrawItems()
	{
	}

	public virtual void Dispose()
	{
		effectInstanced.Dispose();
		effectSingle.Dispose();
		target.Dispose();
	}
}
