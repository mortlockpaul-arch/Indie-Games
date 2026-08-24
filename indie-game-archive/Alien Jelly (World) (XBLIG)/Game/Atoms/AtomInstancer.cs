using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomInstancer : Entity3D
{
	public static VertexDeclaration VERTEX_DECLARATION = new VertexDeclaration(new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0), new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1), new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2), new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3), new VertexElement(64, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 4));

	private Matrix[] _matrices;

	private Vector4[] _data;

	public AtomDefinition definition;

	public AtomManager manager;

	public MaxModel model;

	public MaxModelPart part;

	private DynamicVertexBuffer instanceVertexBuffer;

	protected Effect effect;

	protected EffectPass pass;

	protected EffectParameter effectParamFocalLength;

	protected EffectParameter effectParamViewI;

	protected EffectParameter effectParamView;

	protected EffectParameter effectParamProjection;

	protected EffectParameter effectParamMatrices;

	protected EffectParameter effectParamData;

	public EffectParameter effectParamTime;

	public EffectParameter effectParamShadowMatrix;

	public EffectParameter effectParamShadowTexture;

	public EffectParameter effectParamLightView;

	public EffectParameter effectParamLightProjection;

	public bool renderShadows = true;

	public bool renderDepth = true;

	public List<AtomInstanced> atoms;

	public Matrix[] matrices;

	public Vector4[] data;

	public int count;

	public AtomInstancer(AtomManager oManager, AtomDefinition oDefinition)
	{
		manager = oManager;
		definition = oDefinition;
		scene = manager.scene;
		model = AtomCatalog.shapes[oDefinition.shape].model;
		atoms = new List<AtomInstanced>();
		matrices = new Matrix[0];
		data = new Vector4[0];
		count = 0;
		visible = true;
		Init();
	}

	public virtual void Init()
	{
		Load();
		scene.RenderStacks_FromName(definition.renderStack).Add(guid.value, this);
	}

	public override void Dispose()
	{
		atoms.Clear();
		atoms = null;
		matrices = null;
		data = null;
		count = 0;
		part.Dispose();
		base.Dispose();
	}

	public override void Load()
	{
		if (model.modelParts.Count > 0)
		{
			part = model.modelParts[0].Clone();
			part.materialData = definition.surface;
			part.Build();
			effect = part.material.effect;
			pass = part.material.effect.CurrentTechnique.Passes[0];
			scene.lights.SetEffect(ref effect);
			effectParamFocalLength = effect.Parameters["focalLength"];
			effectParamViewI = effect.Parameters["ViewI"];
			effectParamView = effect.Parameters["View"];
			effectParamProjection = effect.Parameters["Projection"];
			effectParamMatrices = effect.Parameters["InstanceTransforms"];
			effectParamData = effect.Parameters["InstanceData"];
			effectParamShadowMatrix = effect.Parameters["ShadowMatrix"];
			effectParamShadowTexture = effect.Parameters["TextureShadow"];
			effectParamLightView = effect.Parameters["LightView"];
			effectParamLightProjection = effect.Parameters["LightProj"];
			if (definition.timed)
			{
				effectParamTime = effect.Parameters["Time"];
			}
			if (manager is PlayAtomManager)
			{
				effect.Parameters["CamCull"].SetValue(value: true);
			}
		}
		base.Load();
	}

	public bool Add(AtomInstanced oAtom)
	{
		bool result = true;
		if (!atoms.Contains(oAtom))
		{
			atoms.Add(oAtom);
			oAtom.instancerIndex = atoms.Count - 1;
			count = atoms.Count;
			ShaderDataAdd();
		}
		else
		{
			result = false;
		}
		return result;
	}

	public void Remove(AtomInstanced oAtom)
	{
		atoms.Remove(oAtom);
		count = atoms.Count;
		if (count > 0)
		{
			ShaderDataPopulate();
		}
	}

	public void ShaderDataPopulate()
	{
		matrices = new Matrix[count];
		data = new Vector4[count];
		for (int i = 0; i < count; i++)
		{
			atoms[i].instancerIndex = i;
			ref Matrix reference = ref matrices[i];
			reference = atoms[i].matrix;
			ref Vector4 reference2 = ref data[i];
			reference2 = atoms[i].data;
		}
		MakeBuffer();
	}

	public void ShaderDataAdd()
	{
		_matrices = new Matrix[count];
		_data = new Vector4[count];
		matrices.CopyTo(_matrices, 0);
		data.CopyTo(_data, 0);
		for (int i = matrices.Length; i < _matrices.Length; i++)
		{
			ref Matrix reference = ref _matrices[i];
			reference = atoms[i].matrix;
			ref Vector4 reference2 = ref _data[i];
			reference2 = atoms[i].data;
		}
		matrices = new Matrix[_matrices.Length];
		data = new Vector4[_matrices.Length];
		_matrices.CopyTo(matrices, 0);
		_data.CopyTo(data, 0);
		MakeBuffer();
	}

	private void MakeBuffer()
	{
		if (instanceVertexBuffer != null)
		{
			instanceVertexBuffer.Dispose();
		}
		instanceVertexBuffer = new DynamicVertexBuffer(GameEngine.Graphics.GraphicsDevice, VERTEX_DECLARATION, count, BufferUsage.WriteOnly);
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible && count > 0 && !effect.IsDisposed)
		{
			GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
			instanceVertexBuffer.SetData(0, matrices, 0, count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
			instanceVertexBuffer.SetData(64, data, 0, count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
			effectParamFocalLength.SetValue(scene.cameras.camera.focalLength);
			effectParamViewI.SetValue(Matrix.Invert(scene.cameras.camera.view));
			effectParamView.SetValue(scene.cameras.camera.view);
			effectParamProjection.SetValue(scene.cameras.camera.projection);
			if (effectParamTime != null)
			{
				effectParamTime.SetValue((float)oGameTime.TotalGameTime.TotalMilliseconds);
			}
			graphicsDevice.SetVertexBuffers(new VertexBufferBinding(part.vertexBuffer, 0, 0), new VertexBufferBinding(instanceVertexBuffer, 0, 1));
			graphicsDevice.Indices = part.indexBuffer;
			pass.Apply();
			graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, part.vertexBuffer.VertexCount, 0, part.triangleCount, count);
		}
	}

	public void RenderEffect(ref Effect oEffect)
	{
		if (visible && count > 0 && !oEffect.IsDisposed)
		{
			GraphicsDevice graphicsDevice = GameEngine.instance.GraphicsDevice;
			instanceVertexBuffer.SetData(0, matrices, 0, count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
			instanceVertexBuffer.SetData(64, data, 0, count, VERTEX_DECLARATION.VertexStride, SetDataOptions.Discard);
			graphicsDevice.SetVertexBuffers(new VertexBufferBinding(part.vertexBuffer, 0, 0), new VertexBufferBinding(instanceVertexBuffer, 0, 1));
			graphicsDevice.Indices = part.indexBuffer;
			oEffect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, part.vertexBuffer.VertexCount, 0, part.triangleCount, count);
		}
	}
}
