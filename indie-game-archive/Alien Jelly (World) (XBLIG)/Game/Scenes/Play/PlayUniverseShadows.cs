using System.Collections.Generic;
using GKEngine;
using GKEngine.Scenes;
using Game.Atoms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Play;

public class PlayUniverseShadows
{
	public const string PATH_EFFECT_SHADOWMAP_SINGLE = "Content/Effects/Pre/ShadowMap_Single";

	public const string PATH_EFFECT_SHADOWMAP_INSTANCED = "Content/Effects/Pre/ShadowMap_Instanced";

	public const string PATH_EFFECT_SHADOWMAP_QBIT = "Content/Effects/Pre/ShadowMap_QBit";

	public const float SHRINK = 1f;

	public const int TEXTURE_SIZE = 1024;

	public const float LIGHT_DEPTH_MIN = 1000f;

	public const float LIGHT_DEPTH_MAX = 30000f;

	public const float LIGHT_ZOOM = 1.5f;

	public const float LIGHT_DISTANCE_MAX = 1500f;

	public const int DELAY_TICK = 2;

	public static Color COLOR_BLANK = new Color(0, 0, 0, 0);

	private Matrix _light_View;

	private Matrix _light_Proj;

	public PlayUniverse universe;

	public Scene scene;

	public bool active = true;

	private int delayTick;

	private Effect effectShadowInstanced;

	private EffectParameter effectShadowInstanced_LightView;

	private EffectParameter effectShadowInstanced_LightProj;

	private Effect effectShadowSingle;

	private EffectParameter effectShadowSingle_LightView;

	private EffectParameter effectShadowSingle_LightProj;

	private Effect effectShadowQBit;

	private EffectParameter effectShadowQBit_LightView;

	private EffectParameter effectShadowQBit_LightProj;

	public RenderTarget2D target;

	private Matrix matrixTextureBias;

	private DepthStencilState depthStencilState = new DepthStencilState();

	private BlendState blendState = new BlendState();

	private Point _size = new Point(GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);

	public Point size
	{
		get
		{
			return _size;
		}
		set
		{
			_size = value;
			target = new RenderTarget2D(GameEngine.Graphics.GraphicsDevice, (int)((float)_size.X * 1f), (int)((float)_size.Y * 1f), mipMap: false, SurfaceFormat.Single, DepthFormat.Depth24);
			matrixTextureBias = new Matrix(0.5f, 0f, 0f, 0f, 0f, -0.5f, 0f, 0f, 0f, 0f, 1f, 0f, 0.5f + 0.5f / (float)_size.X, 0.5f + 0.5f / (float)_size.Y, 0f, 1f);
		}
	}

	public PlayUniverseShadows(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		scene = oUniverse.scene;
		Load();
	}

	public void Load()
	{
		size = new Point(1024, 1024);
		depthStencilState = new DepthStencilState();
		depthStencilState.DepthBufferEnable = true;
		depthStencilState.DepthBufferWriteEnable = true;
		depthStencilState.DepthBufferFunction = CompareFunction.LessEqual;
		blendState = BlendState.Opaque;
		effectShadowInstanced = GameEngine.SceneContent.Load<Effect>("Content/Effects/Pre/ShadowMap_Instanced").Clone();
		effectShadowSingle = GameEngine.SceneContent.Load<Effect>("Content/Effects/Pre/ShadowMap_Single").Clone();
		effectShadowQBit = GameEngine.SceneContent.Load<Effect>("Content/Effects/Pre/ShadowMap_QBit").Clone();
		effectShadowInstanced_LightView = effectShadowInstanced.Parameters["LightView"];
		effectShadowInstanced_LightProj = effectShadowInstanced.Parameters["LightProj"];
		effectShadowSingle_LightView = effectShadowSingle.Parameters["LightView"];
		effectShadowSingle_LightProj = effectShadowSingle.Parameters["LightProj"];
		effectShadowQBit_LightView = effectShadowQBit.Parameters["LightView"];
		effectShadowQBit_LightProj = effectShadowQBit.Parameters["LightProj"];
		Vector3 position = scene.lights.primary.position;
		_light_View = Matrix.CreateLookAt(position, Vector3.Zero, Vector3.Up);
		_light_Proj = Matrix.CreateOrthographic((float)target.Width * 1.5f, (float)target.Height * 1.5f, 1000f, 30000f);
		foreach (KeyValuePair<string, AtomInstancer> instancer in universe.atoms.instancers)
		{
			if (instancer.Value.effectParamShadowMatrix != null)
			{
				instancer.Value.effectParamShadowMatrix.SetValue(matrixTextureBias);
				instancer.Value.effectParamLightView.SetValue(_light_View);
				instancer.Value.effectParamLightProjection.SetValue(_light_Proj);
			}
		}
		for (int i = 0; i < universe.atoms.lengthSingles; i++)
		{
			if (universe.atoms.singles[i].effectParamShadowMatrix != null)
			{
				AtomSingle atomSingle = universe.atoms.singles[i];
				atomSingle.effectParamShadowMatrix.SetValue(matrixTextureBias);
				atomSingle.effectParamLightView.SetValue(_light_View);
				atomSingle.effectParamLightProjection.SetValue(_light_Proj);
			}
		}
	}

	public void Render(GameTime oGameTime)
	{
		if (active)
		{
			delayTick++;
			delayTick %= 2;
			if (delayTick == 0)
			{
				Render_ShadowMap();
			}
		}
	}

	private void Render_ShadowMap()
	{
		_ = scene.cameras.camera;
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		GameEngine.Graphics.GraphicsDevice.SetRenderTarget(target);
		GameEngine.Graphics.GraphicsDevice.Clear(Color.Red);
		graphicsDevice.BlendState = blendState;
		graphicsDevice.DepthStencilState = depthStencilState;
		effectShadowInstanced_LightView.SetValue(_light_View);
		effectShadowInstanced_LightProj.SetValue(_light_Proj);
		effectShadowSingle_LightView.SetValue(_light_View);
		effectShadowSingle_LightProj.SetValue(_light_Proj);
		effectShadowQBit_LightView.SetValue(_light_View);
		effectShadowQBit_LightProj.SetValue(_light_Proj);
		universe.atoms.RenderShadowEffect(ref effectShadowInstanced, ref effectShadowSingle);
		universe.qbits.RenderEffect(ref effectShadowQBit);
		universe.robots.RenderEffect(ref effectShadowSingle);
		universe.interactables.RenderEffect(ref effectShadowSingle);
		GameEngine.Graphics.GraphicsDevice.SetRenderTarget(null);
		GameEngine.Graphics.GraphicsDevice.SamplerStates[GameMain.REGISTER_SHADOW] = SamplerState.PointClamp;
		GameEngine.Graphics.GraphicsDevice.Textures[GameMain.REGISTER_SHADOW] = universe.shadows.target;
	}

	public void Dispose()
	{
		effectShadowInstanced.Dispose();
		effectShadowSingle.Dispose();
		effectShadowQBit.Dispose();
		target.Dispose();
	}
}
