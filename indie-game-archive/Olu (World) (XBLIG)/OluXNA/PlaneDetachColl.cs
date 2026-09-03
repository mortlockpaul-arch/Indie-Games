using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class PlaneDetachColl
{
	public List<PlaneDetachEffect> fx;

	public ModelWrapper parent;

	public VertexBuffer vBuffer;

	public int maxSize;

	public int curIndex;

	public Enemy eParent;

	public PlaneDetachColl(ref ModelWrapper _parent)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		maxSize = 12288;
		base._002Ector();
		fx = new List<PlaneDetachEffect>();
		vBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, maxSize * VertexPositionNormalTexture.SizeInBytes, (BufferUsage)8);
		parent = _parent;
	}

	public void AddPlane(ref ModelWrapper _model, int meshNum, int indexNum, Enemy _enem, FillMode _fillMode)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		fx.Add(new PlaneDetachEffect(ref _model, meshNum, indexNum, ref _enem, this, _fillMode, Matrix.Identity));
	}

	public void AddPlane(ref ModelWrapper _model, int meshNum, int indexNum, Enemy _enem, ref List<RippleEffect> rEL)
	{
		fx.Add(new PlaneDetachEffect(ref _model, meshNum, indexNum, ref _enem, this, ref rEL));
	}

	public void AddPlane(ref ModelWrapper _model, int meshNum, int indexNum, Enemy _enem, ref List<RippleEffect> rEL, FillMode _fillMode)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		fx.Add(new PlaneDetachEffect(ref _model, meshNum, indexNum, ref _enem, this, ref rEL, _fillMode));
	}

	public void AddPlane(ref ModelWrapper _model, int meshNum, int indexNum, Enemy _enem, ref List<RippleEffect> rEL, FillMode _fillMode, Matrix modMatrix)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		fx.Add(new PlaneDetachEffect(ref _model, meshNum, indexNum, ref _enem, this, ref rEL, _fillMode, modMatrix));
	}

	public void AddPlanePath(ref ModelWrapper _model, int meshNum, int indexNum, Enemy _enem, FillMode _fillMode, Matrix modMatrix)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		PlaneDetachEffect planeDetachEffect = new PlaneDetachEffect(ref _model, meshNum, indexNum, ref _enem, this, _fillMode, modMatrix);
		planeDetachEffect.pList = BuildPlanePath(planeDetachEffect);
		planeDetachEffect.state = 1;
		fx.Add(planeDetachEffect);
	}

	public PathList BuildPlanePath(PlaneDetachEffect pde)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[2]
		{
			pde.pos,
			BaseGame.Get().playerPos
		};
		Vector3[] array2 = (Vector3[])(object)new Vector3[4];
		for (int i = 0; i <= 1; i++)
		{
			ref Vector3 reference = ref array2[i * 3];
			reference = array[i];
		}
		for (int j = 0; j < 1; j++)
		{
			Vector3 val = array2[(j + 1) * 3] - array2[j * 3];
			val /= 2f;
			Vector3 val2 = Vector3.Normalize(Vector3.Cross(Vector3.Normalize(val), Vector3.Up));
			val2 *= ((Vector3)(ref val)).Length() * (float)BaseGame.Get().r.NextDouble() * 2f;
			val2 = Vector3.Transform(val2, Matrix.CreateFromAxisAngle(Vector3.Normalize(val), MathHelper.ToRadians(360f * (float)BaseGame.Get().r.NextDouble())));
			ref Vector3 reference2 = ref array2[3 * j + 1];
			reference2 = array2[3 * j] + val + val2;
		}
		for (int k = 0; k < 1; k++)
		{
			ref Vector3 reference3 = ref array2[3 * k + 2];
			reference3 = (3f * array2[3 * k + 1] + 2f * array2[3 * k + 3]) / 5f;
			ref Vector3 reference4 = ref array2[3 * k + 1];
			reference4 = (3f * array2[3 * k + 1] + 2f * array2[3 * k]) / 5f;
		}
		PathList pathList = new PathList();
		pathList.SetLoop(-1);
		for (int l = 0; l < 1; l++)
		{
			pathList.Add(new PBezier(array2[l * 3], array2[l * 3 + 1], array2[l * 3 + 2], array2[l * 3 + 3], 0.16f + 0.08f * (float)BaseGame.Get().r.NextDouble(), Vector3.Up, 0f, 0f, 1f, 19, 0f, 0.0, 0.0));
		}
		return pathList;
	}

	public void act(GameTime gametime)
	{
		for (int num = fx.Count - 1; num >= 0; num--)
		{
			fx[num].act(gametime);
		}
	}

	public void draw(GameTime gametime)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().SwitchEffectTechnique("Textured");
		BaseGame.Get().SetUpEffect(ref parent.epc[0], clearEpc: false);
		BaseGame.Get().fogEffect.Parameters["xPose"].SetValue(Matrix.Identity);
		BaseGame.Get().fogEffect.Parameters["usePalette"].SetValue(false);
		BaseGame.Get().fogEffect.Parameters["xDoubleSided"].SetValue(true);
		BaseGame.Get().fogEffect.Parameters["xEnableLighting"].SetValue(true);
		BaseGame.Get().graphics.GraphicsDevice.VertexDeclaration = BaseGame.Get().VPNTDec;
		BaseGame.Get().fogEffect.Begin();
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].Begin();
		foreach (PlaneDetachEffect item in fx)
		{
			item.DrawPlane();
		}
		BaseGame.Get().fogEffect.CurrentTechnique.Passes[0].End();
		BaseGame.Get().fogEffect.End();
	}

	public int AllocateSpace()
	{
		int result = curIndex;
		curIndex += 3;
		if (curIndex >= maxSize / 3)
		{
			curIndex = 0;
		}
		return result;
	}
}
