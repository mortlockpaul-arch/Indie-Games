using GKEngine;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Particles;

public class ParticleEmitter : Entity3D
{
	public enum Mode
	{
		OneShot,
		Loop
	}

	public enum EffectType
	{
		Default,
		Camera
	}

	public const string PATH_MODEL = "./Content/Models/SFX/Particles/Model";

	public static MaxModel MODEL;

	public static string EFFECT_SHADER_COUNT = "count";

	public static string[] EFFECT_PATHS = new string[2] { "./Content/Effects/Particles/Particles_Default", "./Content/Effects/Particles/Particles_Camera" };

	public static VertexDeclaration VERTEX_DECLARATION = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.BlendWeight, 0), new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.BlendWeight, 1), new VertexElement(24, VertexElementFormat.Vector3, VertexElementUsage.BlendWeight, 2));

	private EffectType effectType;

	private Effect effect;

	private EffectPass pass;

	private EffectParameter effectParamWorld;

	private EffectParameter effectParamView;

	private EffectParameter effectParamProjection;

	private EffectParameter effectParamCameraPos;

	private EffectParameter effectParamTime;

	private EffectParameter effectParamDuration;

	private EffectParameter effectParamType;

	private EffectParameter effectParamTween;

	private EffectParameter effectParamPositions;

	private EffectParameter effectParamVelocities;

	private EffectParameter effectParamData;

	private EffectParameter effectParamRotationStart;

	private EffectParameter effectParamRotationEnd;

	private EffectParameter effectParamRotationTween;

	private EffectParameter effectParamScaleStart;

	private EffectParameter effectParamScaleEnd;

	private EffectParameter effectParamScaleTween;

	private EffectParameter effectParamTintStart;

	private EffectParameter effectParamTintEnd;

	private EffectParameter effectParamTintTween;

	private EffectParameter effectParamTextureStart;

	private EffectParameter effectParamTextureEnd;

	private EffectParameter effectParamTextureTween;

	private MaxModelPart part;

	private DynamicVertexBuffer instanceVertexBuffer;

	public ParticleEmitterSchema schema;

	private string renderStack;

	private float time;

	private float duration = 1f;

	public ParticleEmitter(Scene oScene, EffectType xEffectType, string xRenderStack)
	{
		scene = oScene;
		effectType = xEffectType;
		renderStack = xRenderStack;
		visible = false;
		Init();
	}

	public virtual void Init()
	{
		Load();
		scene.RenderStacks_FromName(renderStack).Add(guid.value, this);
	}

	public override void Dispose()
	{
		Stop();
		part.Dispose();
		if (instanceVertexBuffer != null)
		{
			instanceVertexBuffer.Dispose();
		}
		if (effect != null)
		{
			effect.Dispose();
		}
		pass = null;
		effectParamWorld = null;
		effectParamView = null;
		effectParamProjection = null;
		effectParamCameraPos = null;
		effectParamTime = null;
		effectParamDuration = null;
		effectParamType = null;
		effectParamTween = null;
		effectParamPositions = null;
		effectParamVelocities = null;
		effectParamData = null;
		effectParamRotationStart = null;
		effectParamRotationEnd = null;
		effectParamRotationTween = null;
		effectParamScaleStart = null;
		effectParamScaleEnd = null;
		effectParamScaleTween = null;
		effectParamTintStart = null;
		effectParamTintEnd = null;
		effectParamTintTween = null;
		effectParamTextureStart = null;
		effectParamTextureEnd = null;
		effectParamTextureTween = null;
		base.Dispose();
	}

	public override void Load()
	{
		effect = GameEngine.SceneContent.Load<Effect>(EFFECT_PATHS[(int)effectType]).Clone();
		pass = effect.CurrentTechnique.Passes[0];
		part = MODEL.modelParts[0].Clone();
		effectParamWorld = effect.Parameters["World"];
		effectParamView = effect.Parameters["View"];
		effectParamProjection = effect.Parameters["Projection"];
		effectParamCameraPos = effect.Parameters["CameraPos"];
		effectParamType = effect.Parameters["type"];
		effectParamTime = effect.Parameters["time"];
		effectParamDuration = effect.Parameters["duration"];
		effectParamTween = effect.Parameters["tween"];
		effectParamPositions = effect.Parameters["InstancePositions"];
		effectParamVelocities = effect.Parameters["InstanceVelocities"];
		effectParamData = effect.Parameters["InstanceData"];
		effectParamRotationStart = effect.Parameters["rotationStart"];
		effectParamRotationEnd = effect.Parameters["rotationEnd"];
		effectParamRotationTween = effect.Parameters["rotationTween"];
		effectParamScaleStart = effect.Parameters["scaleStart"];
		effectParamScaleEnd = effect.Parameters["scaleEnd"];
		effectParamScaleTween = effect.Parameters["scaleTween"];
		effectParamTintStart = effect.Parameters["tintStart"];
		effectParamTintEnd = effect.Parameters["tintEnd"];
		effectParamTintTween = effect.Parameters["tintTween"];
		effectParamTextureStart = effect.Parameters["TextureDiffuse0"];
		effectParamTextureEnd = effect.Parameters["TextureDiffuse1"];
		effectParamTextureTween = effect.Parameters["textureTween"];
		base.Load();
	}

	public bool Update(GameTime oGameTime)
	{
		bool result = !visible;
		if (visible)
		{
			time += (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			if (schema.mode == Mode.OneShot && time >= duration)
			{
				Stop();
			}
		}
		return result;
	}

	public void Start(float xDuration, ParticleEmitterSchema oSchema)
	{
		time = 0f;
		duration = xDuration;
		SetSchema(oSchema);
		visible = true;
		if (!GameEngine.instance.updateStack.stack.Contains(Update))
		{
			GameEngine.instance.updateStack.Add(Update);
		}
	}

	public void SetSchema(ParticleEmitterSchema oSchema)
	{
		schema = oSchema;
		if (instanceVertexBuffer != null)
		{
			instanceVertexBuffer.Dispose();
		}
		instanceVertexBuffer = new DynamicVertexBuffer(GameEngine.Graphics.GraphicsDevice, VERTEX_DECLARATION, schema.count, BufferUsage.WriteOnly);
		effectParamType.SetValue((int)schema.mode);
		effectParamDuration.SetValue(duration);
		effectParamTween.SetValue(schema.tween);
		effectParamRotationStart.SetValue(schema.rotationStart);
		effectParamRotationEnd.SetValue(schema.rotationEnd);
		effectParamRotationTween.SetValue(schema.rotationTween);
		effectParamScaleStart.SetValue(schema.scaleStart);
		effectParamScaleEnd.SetValue(schema.scaleEnd);
		effectParamScaleTween.SetValue(schema.scaleTween);
		effectParamTintStart.SetValue(schema.tintStart.ToVector4());
		effectParamTintEnd.SetValue(schema.tintEnd.ToVector4());
		effectParamTintTween.SetValue(schema.tintTween);
		effectParamTextureStart.SetValue(schema.textureStart);
		effectParamTextureEnd.SetValue(schema.textureEnd);
		effectParamTextureTween.SetValue(schema.textureTween);
		if (effectType == EffectType.Camera)
		{
			effectParamCameraPos.SetValue(scene.cameras.camera.position);
		}
		instanceVertexBuffer.SetData(0, schema.positions, 0, schema.count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
		instanceVertexBuffer.SetData(12, schema.deltas, 0, schema.count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
		instanceVertexBuffer.SetData(24, schema.data, 0, schema.count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
	}

	public void Stop()
	{
		visible = false;
	}

	private void StackFlushFloat(ref float[] aStack, float xValue)
	{
		for (int i = 0; i < aStack.Length; i++)
		{
			aStack[i] = xValue;
		}
	}

	public override void Render(GameTime oGameTime)
	{
		GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
		if (visible && !effect.IsDisposed)
		{
			effectParamWorld.SetValue(matrix);
			effectParamView.SetValue(scene.cameras.camera.view);
			effectParamProjection.SetValue(scene.cameras.camera.projection);
			effectParamTime.SetValue(time);
			if (effect.Parameters["CameraPos"] != null)
			{
				effect.Parameters["CameraPos"].SetValue(scene.cameras.camera.position);
			}
			graphicsDevice.SetVertexBuffers(new VertexBufferBinding(part.vertexBuffer, 0, 0), new VertexBufferBinding(instanceVertexBuffer, 0, 1));
			graphicsDevice.Indices = part.indexBuffer;
			pass.Apply();
			graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, part.vertexBuffer.VertexCount, 0, part.triangleCount, schema.count);
		}
	}

	public static void Initialize()
	{
		MODEL = GameEngine.SceneContent.Load<MaxModel>("./Content/Models/SFX/Particles/Model");
	}
}
