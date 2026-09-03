using Microsoft.Xna.Framework;

namespace OluXNA;

internal class FallingObject
{
	public IEffect effect;

	public Matrix worldMatrix;

	public float rotInc;

	public float rotAngle;

	public Vector3 rotAxis;

	public Vector3 vel;

	public Vector3 pos;

	public FallingObject(IEffect _obj, float _rotInc, float _rotAngle, Vector3 _rotAxis, Vector3 _vel, Vector3 _pos)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		effect = _obj;
		rotInc = _rotInc;
		rotAngle = _rotAngle;
		rotAxis = Vector3.Normalize(_rotAxis);
		vel = _vel;
		pos = _pos;
		worldMatrix = Matrix.Identity;
	}

	public FallingObject(IEffect _obj, float _rotInc, float _rotAngle, Vector3 _rotAxis, Vector3 _vel, Vector3 _pos, Matrix _world)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(_obj, _rotInc, _rotAngle, _rotAxis, _vel, _pos);
		worldMatrix = _world;
	}

	public void Update(GameTime gametime)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)gametime.ElapsedGameTime.TotalSeconds;
		pos = new Vector3(pos.X + vel.X * num, pos.Y + vel.Y * num - BaseGame.gravFactor * num * num / 2f, pos.Z + vel.Z * num);
		rotAngle += rotInc * num;
		ref Vector3 reference = ref vel;
		reference.Y -= BaseGame.gravFactor * num;
	}

	public void draw()
	{
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Colored");
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VertDec;
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateFromAxisAngle(rotAxis, rotAngle) * Matrix.CreateTranslation(pos));
		BaseGame.Get().matStack.ApplyMatrix(worldMatrix);
		effect.draw();
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
	}

	public virtual void setEffect()
	{
		BaseGame.Get().SwitchEffectTechnique("Colored");
	}
}
