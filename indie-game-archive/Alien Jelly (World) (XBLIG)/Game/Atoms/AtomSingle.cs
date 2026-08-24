using System.Collections.Generic;
using GKEngine;
using GKEngine.Cameras;
using GKEngine.Entities;
using Game.Grids;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomSingle : Atom, IGridable, IRenderable
{
	protected MaxModel model;

	protected bool useMaterials = true;

	public EffectParameter effectParamShadowMatrix;

	public EffectParameter effectParamShadowTexture;

	public EffectParameter effectParamLightView;

	public EffectParameter effectParamLightProjection;

	protected List<EffectParameter> effectData;

	protected int effectDataCount;

	public bool renderShadows = true;

	public bool renderDepth = true;

	public AtomSingle(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
		data.Y = ((!oDefinition.camCull) ? 1 : 0);
		Load();
	}

	public virtual void Load()
	{
		if (definition.clonable)
		{
			model = AtomCatalog.shapes[definition.shape].model.Clone();
		}
		else
		{
			model = AtomCatalog.shapes[definition.shape].model;
		}
		if (useMaterials)
		{
			for (int i = 0; i < model.modelParts.Count; i++)
			{
				model.modelParts[i].materialData = definition.surface;
			}
		}
		else
		{
			LoadManualSurfaces();
		}
		model.Build(this);
		for (int i = 0; i < model.modelParts.Count; i++)
		{
			manager.scene.lights.SetEffect(ref model.modelParts[i].material.effect);
			if (model.modelParts[i].material.effect.Parameters["CamCull"] != null)
			{
				model.modelParts[i].material.effect.Parameters["CamCull"].SetValue(!(manager is PlayAtomManager) || definition.camCull);
			}
		}
		effectData = new List<EffectParameter>();
		for (int i = 0; i < model.modelParts.Count; i++)
		{
			if (model.modelParts[i].material.effect.Parameters["data"] != null)
			{
				effectData.Add(model.modelParts[i].material.effect.Parameters["data"]);
			}
		}
		effectDataCount = effectData.Count;
		SetShadowParams();
		manager.scene.RenderStacks_FromName(definition.renderStack).Add(guid.value, this);
	}

	public override void InitPlay()
	{
		base.InitPlay();
		data.Y = ((!definition.camCull) ? 1 : 0);
	}

	public override void InitBuild()
	{
		base.InitBuild();
		data.Y = 0f;
	}

	protected virtual void LoadManualSurfaces()
	{
	}

	public virtual void Dispose()
	{
		effectParamShadowMatrix = null;
		effectParamShadowTexture = null;
		effectParamLightView = null;
		effectParamLightProjection = null;
		effectData.Clear();
		effectData = null;
		manager.scene.RenderStacks_FromName(definition.renderStack).Remove(guid.value, this);
		if (!definition.clonable)
		{
			model.Dispose();
		}
		model = null;
	}

	public virtual void SetShadowParams()
	{
		effectParamShadowMatrix = model.modelParts[0].material.effect.Parameters["ShadowMatrix"];
		effectParamShadowTexture = model.modelParts[0].material.effect.Parameters["TextureShadow"];
		effectParamLightView = model.modelParts[0].material.effect.Parameters["LightView"];
		effectParamLightProjection = model.modelParts[0].material.effect.Parameters["LightProj"];
	}

	public virtual void Render(GameTime oGameTime)
	{
		if (visible)
		{
			Camera camera = manager.scene.cameras.camera;
			for (int i = 0; i < effectDataCount; i++)
			{
				effectData[i].SetValue(data);
			}
			model.Render(camera);
		}
	}

	public virtual void RenderEffect(ref Effect oEffect)
	{
		if (!visible || model == null)
		{
			return;
		}
		GraphicsDevice graphicsDevice = GameEngine.Graphics.GraphicsDevice;
		for (int i = 0; i < model.modelPartsCount; i++)
		{
			MaxModelPart maxModelPart = model.modelParts[i];
			if (maxModelPart.hasLocal)
			{
				oEffect.Parameters["World"].SetValue(Matrix.Multiply(maxModelPart.local, matrix));
			}
			else
			{
				oEffect.Parameters["World"].SetValue(matrix);
			}
			oEffect.Parameters["data"].SetValue(data);
			graphicsDevice.SetVertexBuffer(maxModelPart.vertexBuffer);
			graphicsDevice.Indices = maxModelPart.indexBuffer;
			EffectPass effectPass = oEffect.CurrentTechnique.Passes[0];
			effectPass.Apply();
			graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, maxModelPart.vertexBuffer.VertexCount, 0, maxModelPart.triangleCount);
		}
	}
}
