using GKEngine;
using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomModelPart : MaxModelPart
{
	protected int _instanceCount;

	protected Matrix[] _matrices;

	protected Vector4[] _data;

	protected int time;

	public AtomInstancer instancer;

	private int maxInstances;

	protected Effect effect;

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

	private VertexElement[] originalVertexDeclaration;

	private bool vertexDataIsReplicated;

	public AtomModelPart(MaxModelPart oPart, AtomInstancer oInstancer)
	{
		name = oPart.name;
		triangleCount = oPart.triangleCount;
		materialData = oPart.materialData;
		vertexBuffer = oPart.vertexBuffer;
		indexBuffer = oPart.indexBuffer;
		instancer = oInstancer;
	}

	public override void Build()
	{
	}

	protected virtual void Build_EffectParams()
	{
	}

	public override void Dispose()
	{
		_matrices = null;
		originalVertexDeclaration = null;
		base.Dispose();
	}

	public virtual void Render(GameTime oGameTime)
	{
	}

	public virtual void RenderEffect(ref Effect oEffect)
	{
		_ = GameEngine.instance.GraphicsDevice;
		_ = oEffect.IsDisposed;
	}
}
