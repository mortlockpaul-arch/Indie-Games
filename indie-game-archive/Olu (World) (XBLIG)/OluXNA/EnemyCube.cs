using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class EnemyCube : IEffect
{
	public List<PlaneEffectHelper> planeIdent;

	public static PlaneEffect[] pFX;

	public static PlaneEffect[] oluPFX;

	public bool oluMode;

	public EnemyCube()
	{
		planeIdent = new List<PlaneEffectHelper>();
	}

	public static void GenerateFX()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_040f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0443: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_046d: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		int num = 4;
		float velocity = 0.22f;
		float vRandom = 0.8f;
		float sideSpeed = 0.21f;
		float sideSpeedRandom = 0.05f;
		Color yellow = Color.Yellow;
		pFX = new PlaneEffect[10];
		for (int i = 0; i < 5; i++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int j = 0; j < num; j++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, velocity, vRandom, sideSpeed, sideSpeedRandom);
				treeNode.branchTree = false;
				treeNode.setColor(yellow);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference = ref planeEffect.cornerNodes[0];
			reference = new Vector3(0f, 0f, 1f);
			ref Vector3 reference2 = ref planeEffect.cornerNodes[1];
			reference2 = new Vector3(1f, 0f, 1f);
			ref Vector3 reference3 = ref planeEffect.cornerNodes[2];
			reference3 = new Vector3(0f, 0f, 0f);
			ref Vector3 reference4 = ref planeEffect.cornerNodes[3];
			reference4 = new Vector3(1f, 0f, 0f);
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			pFX[i] = planeEffect;
		}
		num = 8;
		velocity = 0.15f;
		vRandom = 0.05f;
		sideSpeed = 0.15f;
		sideSpeedRandom = 0.05f;
		yellow = Color.Red;
		for (int k = 5; k < 10; k++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int l = 0; l < num; l++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, velocity, vRandom, sideSpeed, sideSpeedRandom);
				treeNode.branchTree = false;
				treeNode.setColor(yellow);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference5 = ref planeEffect.cornerNodes[0];
			reference5 = new Vector3(0f, 0f, 1f);
			ref Vector3 reference6 = ref planeEffect.cornerNodes[1];
			reference6 = new Vector3(1f, 0f, 1f);
			ref Vector3 reference7 = ref planeEffect.cornerNodes[2];
			reference7 = new Vector3(0f, 0f, 0f);
			ref Vector3 reference8 = ref planeEffect.cornerNodes[3];
			reference8 = new Vector3(1f, 0f, 0f);
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			pFX[k] = planeEffect;
		}
		num = 4;
		velocity = 0.22f;
		vRandom = 0.8f;
		sideSpeed = 0.21f;
		sideSpeedRandom = 0.05f;
		yellow = Color.LightGreen;
		oluPFX = new PlaneEffect[10];
		for (int m = 0; m < 5; m++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int n = 0; n < num; n++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, velocity, vRandom, sideSpeed, sideSpeedRandom);
				treeNode.branchTree = false;
				treeNode.setColor(yellow);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference9 = ref planeEffect.cornerNodes[0];
			reference9 = new Vector3(0f, 0f, 1f);
			ref Vector3 reference10 = ref planeEffect.cornerNodes[1];
			reference10 = new Vector3(1f, 0f, 1f);
			ref Vector3 reference11 = ref planeEffect.cornerNodes[2];
			reference11 = new Vector3(0f, 0f, 0f);
			ref Vector3 reference12 = ref planeEffect.cornerNodes[3];
			reference12 = new Vector3(1f, 0f, 0f);
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			oluPFX[m] = planeEffect;
		}
		num = 8;
		velocity = 0.15f;
		vRandom = 0.05f;
		sideSpeed = 0.15f;
		sideSpeedRandom = 0.05f;
		yellow = Color.Green;
		for (int num2 = 5; num2 < 10; num2++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int num3 = 0; num3 < num; num3++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, velocity, vRandom, sideSpeed, sideSpeedRandom);
				treeNode.branchTree = false;
				treeNode.setColor(yellow);
				planeEffect.addNode(treeNode);
			}
			ref Vector3 reference13 = ref planeEffect.cornerNodes[0];
			reference13 = new Vector3(0f, 0f, 1f);
			ref Vector3 reference14 = ref planeEffect.cornerNodes[1];
			reference14 = new Vector3(1f, 0f, 1f);
			ref Vector3 reference15 = ref planeEffect.cornerNodes[2];
			reference15 = new Vector3(0f, 0f, 0f);
			ref Vector3 reference16 = ref planeEffect.cornerNodes[3];
			reference16 = new Vector3(1f, 0f, 0f);
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
			oluPFX[num2] = planeEffect;
		}
	}

	public void rotate(GameTime gametime)
	{
		base.rotAngle += base.rotDelta * (float)gametime.ElapsedGameTime.TotalSeconds;
	}

	public override void draw()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().matStack.PushMatrix();
		BaseGame.Get().matStack.ApplyMatrix(Matrix.CreateTranslation(base.pos) * Matrix.CreateFromAxisAngle(Vector3.Normalize(base.rotAxis), base.rotAngle));
		for (int i = 0; i < planeIdent.Count; i++)
		{
			BaseGame.Get().matStack.PushMatrix();
			BaseGame.Get().matStack.ApplyMatrix(planeIdent[i].transform);
			if (!oluMode)
			{
				pFX[planeIdent[i].planeNum].draw();
			}
			else
			{
				oluPFX[planeIdent[i].planeNum].draw();
			}
			BaseGame.Get().matStack.PopMatrix();
		}
		BaseGame.Get().matStack.PopMatrix();
	}

	public void createCube(int size, int offset, int randAmount)
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		planeIdent.Add(new PlaneEffectHelper());
		planeIdent.Add(new PlaneEffectHelper());
		planeIdent.Add(new PlaneEffectHelper());
		planeIdent.Add(new PlaneEffectHelper());
		planeIdent.Add(new PlaneEffectHelper());
		planeIdent.Add(new PlaneEffectHelper());
		Vector3[] array = (Vector3[])(object)new Vector3[4]
		{
			new Vector3((float)(-size) / 2f, (float)size / 2f, (float)(-size) / 2f),
			new Vector3((float)size / 2f, (float)size / 2f, (float)(-size) / 2f),
			new Vector3((float)(-size) / 2f, (float)size / 2f, (float)size / 2f),
			new Vector3((float)size / 2f, (float)size / 2f, (float)size / 2f)
		};
		CreateTransformationMatrix(array[0], array[1], array[2], array[3], out planeIdent[0].transform);
		ref Vector3 reference = ref array[2];
		reference *= -1f;
		ref Vector3 reference2 = ref array[3];
		reference2 *= -1f;
		CreateTransformationMatrix(array[3], array[2], array[0], array[1], out planeIdent[1].transform);
		ref Vector3 reference3 = ref array[0];
		reference3 *= -1f;
		ref Vector3 reference4 = ref array[1];
		reference4 *= -1f;
		CreateTransformationMatrix(array[1], array[0], array[3], array[2], out planeIdent[2].transform);
		ref Vector3 reference5 = ref array[2];
		reference5 *= -1f;
		ref Vector3 reference6 = ref array[3];
		reference6 *= -1f;
		CreateTransformationMatrix(array[0], array[1], array[3], array[2], out planeIdent[3].transform);
		ref Vector3 reference7 = ref array[1];
		reference7 *= -1f;
		ref Vector3 reference8 = ref array[2];
		reference8 *= -1f;
		CreateTransformationMatrix(array[2], array[0], array[1], array[3], out planeIdent[4].transform);
		ref Vector3 reference9 = ref array[0];
		reference9 *= -1f;
		ref Vector3 reference10 = ref array[1];
		reference10 *= -1f;
		ref Vector3 reference11 = ref array[2];
		reference11 *= -1f;
		ref Vector3 reference12 = ref array[3];
		reference12 *= -1f;
		CreateTransformationMatrix(array[1], array[3], array[2], array[0], out planeIdent[5].transform);
		planeIdent[0].planeNum = offset + random.Next(randAmount);
		planeIdent[1].planeNum = offset + random.Next(randAmount);
		planeIdent[2].planeNum = offset + random.Next(randAmount);
		planeIdent[3].planeNum = offset + random.Next(randAmount);
		planeIdent[4].planeNum = offset + random.Next(randAmount);
		planeIdent[5].planeNum = offset + random.Next(randAmount);
	}

	public static void CreateTransformationMatrix(Vector3 a, Vector3 b, Vector3 c, Vector3 d, out Matrix result)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		result = default(Matrix);
		Vector3 val = b - a;
		Vector3 val2 = c - a - (d - b);
		Vector3 val3 = d - b;
		result.M11 = val.X;
		result.M21 = val.Y;
		result.M31 = val.Z;
		result.M41 = 0f;
		result.M12 = val2.X;
		result.M22 = val2.Y;
		result.M32 = val2.Z;
		result.M42 = 0f;
		result.M13 = val3.X;
		result.M23 = val3.Y;
		result.M33 = val3.Z;
		result.M43 = 0f;
		result.M14 = 0f;
		result.M24 = 0f;
		result.M34 = 0f;
		result.M44 = 1f;
		result = Matrix.Transpose(result);
		result = Matrix.Multiply(result, Matrix.CreateTranslation(a));
	}

	public void createCube(int size, int density, float vel, float velRand, float side, float sideRand, Color colorCoord)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0472: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		for (int i = 0; i < 6; i++)
		{
			PlaneEffect planeEffect = new PlaneEffect();
			for (int j = 0; j < density; j++)
			{
				TreeNode treeNode = new TreeNode((float)random.NextDouble(), 0f, 0f, 1, vel, velRand, side, sideRand);
				treeNode.branchTree = false;
				treeNode.setColor(colorCoord);
				planeEffect.addNode(treeNode);
			}
			switch (i)
			{
			case 0:
			{
				ref Vector3 reference21 = ref planeEffect.cornerNodes[0];
				reference21 = new Vector3((float)(-size) / 2f, (float)size / 2f, (float)size / 2f);
				ref Vector3 reference22 = ref planeEffect.cornerNodes[1];
				reference22 = new Vector3((float)size / 2f, (float)size / 2f, (float)size / 2f);
				ref Vector3 reference23 = ref planeEffect.cornerNodes[2];
				reference23 = new Vector3((float)(-size) / 2f, (float)size / 2f, (float)(-size) / 2f);
				ref Vector3 reference24 = ref planeEffect.cornerNodes[3];
				reference24 = new Vector3((float)size / 2f, (float)size / 2f, (float)(-size) / 2f);
				break;
			}
			case 1:
			{
				ref Vector3 reference17 = ref planeEffect.cornerNodes[0];
				reference17 = new Vector3((float)(-size) / 2f, (float)size / 2f, (float)(-size) / 2f);
				ref Vector3 reference18 = ref planeEffect.cornerNodes[1];
				reference18 = new Vector3((float)size / 2f, (float)size / 2f, (float)(-size) / 2f);
				ref Vector3 reference19 = ref planeEffect.cornerNodes[2];
				reference19 = new Vector3((float)(-size) / 2f, (float)(-size) / 2f, (float)(-size) / 2f);
				ref Vector3 reference20 = ref planeEffect.cornerNodes[3];
				reference20 = new Vector3((float)size / 2f, (float)(-size) / 2f, (float)(-size) / 2f);
				break;
			}
			case 2:
			{
				ref Vector3 reference13 = ref planeEffect.cornerNodes[0];
				reference13 = new Vector3((float)(-size) / 2f, (float)(-size) / 2f, (float)size / 2f);
				ref Vector3 reference14 = ref planeEffect.cornerNodes[1];
				reference14 = new Vector3((float)(-size) / 2f, (float)size / 2f, (float)size / 2f);
				ref Vector3 reference15 = ref planeEffect.cornerNodes[2];
				reference15 = new Vector3((float)(-size) / 2f, (float)(-size) / 2f, (float)(-size) / 2f);
				ref Vector3 reference16 = ref planeEffect.cornerNodes[3];
				reference16 = new Vector3((float)(-size) / 2f, (float)size / 2f, (float)(-size) / 2f);
				break;
			}
			case 3:
			{
				ref Vector3 reference9 = ref planeEffect.cornerNodes[0];
				reference9 = new Vector3((float)(-size) / 2f, (float)(-size) / 2f, (float)size / 2f);
				ref Vector3 reference10 = ref planeEffect.cornerNodes[1];
				reference10 = new Vector3((float)size / 2f, (float)(-size) / 2f, (float)size / 2f);
				ref Vector3 reference11 = ref planeEffect.cornerNodes[2];
				reference11 = new Vector3((float)(-size) / 2f, (float)size / 2f, (float)size / 2f);
				ref Vector3 reference12 = ref planeEffect.cornerNodes[3];
				reference12 = new Vector3((float)size / 2f, (float)size / 2f, (float)size / 2f);
				break;
			}
			case 4:
			{
				ref Vector3 reference5 = ref planeEffect.cornerNodes[0];
				reference5 = new Vector3((float)size / 2f, (float)size / 2f, (float)size / 2f);
				ref Vector3 reference6 = ref planeEffect.cornerNodes[1];
				reference6 = new Vector3((float)size / 2f, (float)(-size) / 2f, (float)size / 2f);
				ref Vector3 reference7 = ref planeEffect.cornerNodes[2];
				reference7 = new Vector3((float)size / 2f, (float)size / 2f, (float)(-size) / 2f);
				ref Vector3 reference8 = ref planeEffect.cornerNodes[3];
				reference8 = new Vector3((float)size / 2f, (float)(-size) / 2f, (float)(-size) / 2f);
				break;
			}
			case 5:
			{
				ref Vector3 reference = ref planeEffect.cornerNodes[0];
				reference = new Vector3((float)(-size) / 2f, (float)(-size) / 2f, (float)size / 2f);
				ref Vector3 reference2 = ref planeEffect.cornerNodes[1];
				reference2 = new Vector3((float)size / 2f, (float)(-size) / 2f, (float)size / 2f);
				ref Vector3 reference3 = ref planeEffect.cornerNodes[2];
				reference3 = new Vector3((float)(-size) / 2f, (float)(-size) / 2f, (float)(-size) / 2f);
				ref Vector3 reference4 = ref planeEffect.cornerNodes[3];
				reference4 = new Vector3((float)size / 2f, (float)(-size) / 2f, (float)(-size) / 2f);
				break;
			}
			}
			planeEffect.iteratePlane();
			planeEffect.FinalizeEffect();
		}
	}

	public void dropOff(Vector3 offset, Vector3 _rotAxis, float _rotAngle)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		Random random = new Random();
		if (planeIdent.Count > 0)
		{
			int index = random.Next(0, planeIdent.Count - 1);
			Vector3 val = Vector3.Transform(new Vector3(0.5f, 0.25f, 0.5f), planeIdent[index].transform);
			planeIdent[index].transform = Matrix.Multiply(planeIdent[index].transform, Matrix.CreateTranslation(-val));
			BaseGame.Get().fallFX.Add(new FallingObject((oluMode ? oluPFX : pFX)[planeIdent[index].planeNum], (float)(random.NextDouble() * Math.PI), 0f, Vector3.Normalize(new Vector3((float)(random.NextDouble() - 0.5), (float)(random.NextDouble() - 0.5), (float)(random.NextDouble() - 0.5))), Vector3.Normalize(val) * (5f + 10f * (float)random.NextDouble()), Vector3.Transform(offset + val, Matrix.CreateFromAxisAngle(_rotAxis, _rotAngle)), planeIdent[index].transform));
			planeIdent.RemoveAt(index);
		}
	}
}
