using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class PlaneDetachEffect : IEffect
{
	private int drawIndex;

	private Vector3 vel;

	private float rotInc;

	private float floor;

	private PlaneDetachColl pdColl;

	private bool belowWater;

	private List<RippleEffect> rEL;

	private FillMode fillMode;

	public int state;

	public PathList pList;

	public int meshNum;

	public int indexNum;

	public PlaneDetachEffect(ref ModelWrapper _model, int meshNum, int indexNum, ref Enemy _enem, PlaneDetachColl _col, ref List<RippleEffect> _rEL, FillMode _fillMode, Matrix _modMatrix)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(ref _model, meshNum, indexNum, ref _enem, _col, _fillMode, _modMatrix);
		rEL = _rEL;
	}

	public PlaneDetachEffect(ref ModelWrapper _model, int meshNum, int indexNum, ref Enemy _enem, PlaneDetachColl _col, ref List<RippleEffect> _rEL, FillMode _fillMode)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector(ref _model, meshNum, indexNum, ref _enem, _col, ref _rEL, _fillMode, Matrix.Identity);
	}

	public PlaneDetachEffect(ref ModelWrapper _model, int meshNum, int indexNum, ref Enemy _enem, PlaneDetachColl _col, ref List<RippleEffect> _rEL)
		: this(ref _model, meshNum, indexNum, ref _enem, _col, ref _rEL, (FillMode)3)
	{
	}

	public PlaneDetachEffect(ref ModelWrapper _model, int meshNum, int indexNum, ref Enemy _enem, PlaneDetachColl _col, FillMode _fillMode, Matrix _modMatrix)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		VertexNormalTex[] array = new VertexNormalTex[3];
		VertexPositionNormalTexture[] array2 = (VertexPositionNormalTexture[])(object)new VertexPositionNormalTexture[3];
		pdColl = _col;
		state = 0;
		Vector3 val = Vector3.Zero;
		Vector3 val2 = Vector3.Zero;
		for (int i = 0; i < 3; i++)
		{
			ref VertexNormalTex reference = ref array[i];
			reference = new VertexNormalTex(_model.vertices[meshNum][_model.indices[meshNum][indexNum + i]]);
			array[i].position = BaseGame.GetVertexPos(ref _model, meshNum, indexNum + i, ref _enem, _modMatrix);
			array[i].normal = BaseGame.GetVertexNorm(ref _model, meshNum, indexNum + i, ref _enem);
			val += array[i].position;
			val2 += array[i].normal;
		}
		val /= 3f;
		val2 /= 3f;
		for (int j = 0; j < 3; j++)
		{
			ref VertexNormalTex reference2 = ref array[j];
			reference2.position -= val;
		}
		for (int k = 0; k < 3; k++)
		{
			array2[k].Position = array[k].position;
			array2[k].Normal = array[k].normal;
			array2[k].TextureCoordinate = array[k].tex;
		}
		base.pos = val;
		floor = Vector3.Transform(Vector3.Zero, _enem.Transformation()).Y - 10f;
		vel = val2 * 2f;
		drawIndex = pdColl.AllocateSpace();
		drawIndex = drawIndex * 3 * VertexPositionNormalTexture.SizeInBytes;
		pdColl.vBuffer.SetData<VertexPositionNormalTexture>(drawIndex, array2, 0, 3, VertexPositionNormalTexture.SizeInBytes);
		belowWater = false;
		rotInc = 0f;
		fillMode = _fillMode;
		this.meshNum = meshNum;
		this.indexNum = indexNum;
	}

	public void act(GameTime gametime)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		if (state == 0)
		{
			float num = (float)gametime.ElapsedGameTime.TotalSeconds;
			base.pos = new Vector3(base.pos.X + vel.X * num, base.pos.Y + vel.Y * num - BaseGame.gravFactor * num * num / 2f, base.pos.Z + vel.Z * num);
			base.rotAngle += rotInc * num;
			ref Vector3 reference = ref vel;
			reference.Y -= BaseGame.gravFactor * num;
			if (rEL != null && !belowWater && base.pos.Y < floor)
			{
				Vector3 start = default(Vector3);
				((Vector3)(ref start))._002Ector(base.pos.X, floor, base.pos.Z);
				RippleEffect rippleEffect = new RippleEffect(start, 0.25f, 0.25f, 0f, 2.5f, 5f, _loop: false, 0f);
				rippleEffect.fxUpdate = BaseGame.GetFogEffect().Parameters;
				belowWater = true;
				rEL.Add(rippleEffect);
				BaseGame.Get().PlayCue("ride");
			}
			if (base.pos.Y < floor - 50f)
			{
				pdColl.fx.Remove(this);
			}
		}
		else
		{
			if (state != 1)
			{
				return;
			}
			if (pList.Update(gametime))
			{
				if (pdColl.eParent != null)
				{
					((Olu)pdColl.eParent).AddFace(meshNum, indexNum);
				}
				pdColl.fx.Remove(this);
			}
			else
			{
				base.pos = pList.curLocation();
			}
		}
	}

	public override void draw()
	{
	}

	public void DrawPlane()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if (base.pos.Y < floor || (rEL == null && (int)fillMode == 2))
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)2;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)2;
		}
		else
		{
			BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
			BaseGame.Get().graphics.GraphicsDevice.RenderState.StencilPass = (StencilOperation)3;
		}
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(base.pos));
		BaseGame.Get().graphics.GraphicsDevice.Vertices[0].SetSource(pdColl.vBuffer, drawIndex, VertexPositionNormalTexture.SizeInBytes);
		BaseGame.Get().graphics.GraphicsDevice.DrawPrimitives((PrimitiveType)4, 0, 1);
		BaseGame.Get().matStack.PopMatrix();
		BaseGame.Get().graphics.GraphicsDevice.RenderState.FillMode = (FillMode)3;
	}
}
