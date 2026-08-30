using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Kobingo.Xna.Library.Graphics;

public class Sprite
{
	[CompilerGenerated]
	private Vector2 _003CCenter_003Ek__BackingField;

	public Texture2D Texture { get; private set; }

	public Vector2 Center
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CCenter_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			_003CCenter_003Ek__BackingField = value;
		}
	}

	private Color[] Pixels { get; set; }

	public Sprite(Texture2D texture)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		if (texture == null)
		{
			throw new ArgumentNullException("texture");
		}
		Texture = texture;
		Pixels = (Color[])(object)new Color[Texture.Width * Texture.Height];
		Texture.GetData<Color>(Pixels);
		Center = new Vector2((float)Texture.Width / 2f, (float)Texture.Height / 2f);
	}

	public static bool IsIntersecting(Sprite spriteA, Matrix transformationA, Sprite spriteB, Matrix transformationB, byte minAlphaValue)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Matrix val = transformationA * Matrix.Invert(transformationB);
		Vector2 val2 = Vector2.TransformNormal(Vector2.UnitX, val);
		Vector2 val3 = Vector2.TransformNormal(Vector2.UnitY, val);
		Vector2 val4 = Vector2.Transform(Vector2.Zero, val);
		for (int i = 0; i < spriteA.Texture.Height; i++)
		{
			Vector2 val5 = val4;
			for (int j = 0; j < spriteA.Texture.Width; j++)
			{
				int num = (int)Math.Round(val5.X);
				int num2 = (int)Math.Round(val5.Y);
				if (0 <= num && num < spriteB.Texture.Width && 0 <= num2 && num2 < spriteB.Texture.Height)
				{
					Color val6 = spriteA.Pixels[j + i * spriteA.Texture.Width];
					Color val7 = spriteB.Pixels[num + num2 * spriteB.Texture.Width];
					if (((Color)(ref val6)).A > minAlphaValue && ((Color)(ref val7)).A > minAlphaValue)
					{
						return true;
					}
				}
				val5 += val2;
			}
			val4 += val3;
		}
		return false;
	}

	public static Rectangle GetBoundingRectangle(Rectangle rectangle, Matrix transform)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = new Vector2((float)((Rectangle)(ref rectangle)).Left, (float)((Rectangle)(ref rectangle)).Top);
		Vector2 val2 = new Vector2((float)((Rectangle)(ref rectangle)).Right, (float)((Rectangle)(ref rectangle)).Top);
		Vector2 val3 = new Vector2((float)((Rectangle)(ref rectangle)).Left, (float)((Rectangle)(ref rectangle)).Bottom);
		Vector2 val4 = new Vector2((float)((Rectangle)(ref rectangle)).Right, (float)((Rectangle)(ref rectangle)).Bottom);
		Vector2.Transform(ref val, ref transform, ref val);
		Vector2.Transform(ref val2, ref transform, ref val2);
		Vector2.Transform(ref val3, ref transform, ref val3);
		Vector2.Transform(ref val4, ref transform, ref val4);
		Vector2 val5 = Vector2.Min(Vector2.Min(val, val2), Vector2.Min(val3, val4));
		Vector2 val6 = Vector2.Max(Vector2.Max(val, val2), Vector2.Max(val3, val4));
		return new Rectangle((int)val5.X, (int)val5.Y, (int)(val6.X - val5.X), (int)(val6.Y - val5.Y));
	}
}
