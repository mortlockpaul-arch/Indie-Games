using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TargetEffect
{
	public Target eTarget;

	public Enemy enem;

	public Vector3 pos;

	public FillMode fillMode;

	public bool activated;

	public float countDown;

	public WaitCond wade;

	public int waitBeat;

	public int lockNum;

	public TargetEffect prev;

	public TargetEffect next;

	public static VertexBuffer vBuffer;

	public static int[] offsets;

	public static int[] size;

	public static Texture2D glowTex;

	public static Texture2D glowTex2;

	public static Texture2D timeTex;

	public static void CreateFX()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0407: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0431: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e0: Expected O, but got Unknown
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		List<VertexPositionColor> list = new List<VertexPositionColor>();
		offsets = new int[6];
		size = new int[6];
		float num = (float)Math.Cos(Math.PI / 4.0);
		float num2 = (float)Math.Sin(Math.PI / 4.0);
		list = new List<VertexPositionColor>();
		offsets[0] = 0;
		Color val = default(Color);
		((Color)(ref val))._002Ector(new Vector4(1f, 1f, 1f, 0.3f));
		list.Add(new VertexPositionColor(new Vector3(num, num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(0f - num, num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(0f - num, 0f - num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(num, 0f - num2, 0f), val));
		offsets[1] = list.Count;
		size[0] = offsets[1];
		((Color)(ref val))._002Ector(new Vector4(1f, 1f, 1f, 1f));
		list.Add(new VertexPositionColor(new Vector3(num, num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(0f - num, num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(0f - num, 0f - num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(num, 0f - num2, 0f), val));
		list.Add(new VertexPositionColor(new Vector3(num, num2, 0f), val));
		offsets[2] = list.Count;
		size[1] = offsets[2] - offsets[1];
		for (float num3 = 0f; num3 <= 2.01f; num3 += 0.0625f)
		{
			list.Add(new VertexPositionColor(new Vector3((float)Math.Cos((double)num3 * Math.PI), (float)Math.Sin((double)num3 * Math.PI), 0f), new Color(new Vector4(1f, 1f, 1f, 0.8f))));
		}
		offsets[3] = list.Count;
		size[2] = offsets[3] - offsets[2];
		for (float num4 = 0f; num4 <= 2.01f; num4 += 0.0625f)
		{
			list.Add(new VertexPositionColor(new Vector3((float)Math.Cos((double)num4 * Math.PI), (float)Math.Sin((double)num4 * Math.PI), 0f), new Color(new Vector4(1f, 1f, 1f, 0.6f))));
		}
		offsets[4] = list.Count;
		size[3] = offsets[4] - offsets[3];
		for (float num5 = 0f; num5 <= 2.01f; num5 += 0.0625f)
		{
			list.Add(new VertexPositionColor(new Vector3((float)Math.Cos((double)num5 * Math.PI), (float)Math.Sin((double)num5 * Math.PI), 0f), new Color(new Vector4(1f, 1f, 1f, 0.4f))));
		}
		offsets[5] = list.Count;
		size[4] = offsets[5] - offsets[4];
		Vector3[] array = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(80f, 0f, 0f),
			new Vector3(85f, 0f, 0f),
			default(Vector3),
			default(Vector3)
		};
		ref Vector3 reference = ref array[2];
		reference = Vector3.Transform(array[0], Matrix.CreateRotationZ(MathHelper.ToRadians(60f)));
		ref Vector3 reference2 = ref array[3];
		reference2 = Vector3.Transform(array[1], Matrix.CreateRotationZ(MathHelper.ToRadians(60f)));
		for (int i = 0; i < 6; i++)
		{
			list.Add(new VertexPositionColor(array[0], Color.Red));
			list.Add(new VertexPositionColor(array[1], Color.Red));
			list.Add(new VertexPositionColor(array[2], Color.Red));
			list.Add(new VertexPositionColor(array[1], Color.Red));
			list.Add(new VertexPositionColor(array[2], Color.Red));
			list.Add(new VertexPositionColor(array[3], Color.Red));
			list.Add(new VertexPositionColor(Vector3.Zero, new Color(1f, 0f, 0f, 0.2f)));
			list.Add(new VertexPositionColor(array[0], new Color(1f, 0f, 0f, 0.2f)));
			list.Add(new VertexPositionColor(array[2], new Color(1f, 0f, 0f, 0.2f)));
			for (int j = 0; j < 4; j++)
			{
				ref Vector3 reference3 = ref array[j];
				reference3 = Vector3.Transform(array[j], Matrix.CreateRotationZ(MathHelper.ToRadians(60f)));
			}
		}
		size[5] = list.Count - offsets[5];
		vBuffer = new VertexBuffer(BaseGame.Get().graphics.GraphicsDevice, VertexPositionColor.SizeInBytes * list.Count, (BufferUsage)8);
		vBuffer.SetData<VertexPositionColor>(list.ToArray());
		list.Clear();
		glowTex = BaseGame.Get().content.Load<Texture2D>("Content/glowTex");
		glowTex2 = BaseGame.Get().content.Load<Texture2D>("Content/glowTex2");
	}

	public virtual void Update(GameTime gametime)
	{
	}

	public virtual void Draw()
	{
	}

	public virtual void DrawInBack()
	{
	}
}
