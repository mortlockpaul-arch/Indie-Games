using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectMercury.Emitters;

public sealed class MaskEmitter : Emitter
{
	private byte[][] _mask;

	private float _threshold;

	public byte[][] Mask
	{
		get
		{
			return _mask;
		}
		set
		{
			_mask = value;
			RecalculateMaskHits();
			Width = Mask.Length;
			Height = Mask[0].Length;
		}
	}

	public float Threshold
	{
		get
		{
			return _threshold;
		}
		set
		{
			_threshold = value;
			if (Mask != null)
			{
				RecalculateMaskHits();
			}
		}
	}

	public float Width { get; set; }

	public float Height { get; set; }

	public string MaskTextureContentPath { get; set; }

	private Vector2[] MaskHits { get; set; }

	private void RecalculateMaskHits()
	{
		int num = Mask.Length;
		int num2 = Mask[0].Length;
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				byte b = Mask[i][j];
				if ((float)(int)b / 255f >= Threshold)
				{
					list.Add(new Vector2
					{
						X = (float)i / (float)num - 0.5f,
						Y = (float)j / (float)num2 - 0.5f
					});
				}
			}
		}
		MaskHits = list.ToArray();
	}

	public void ApplyMaskTexture(Texture2D maskTexture)
	{
		byte[][] array = new byte[maskTexture.Width][];
		for (int i = 0; i < maskTexture.Height; i++)
		{
			array[i] = new byte[maskTexture.Height];
		}
		for (int j = 0; j < maskTexture.Width; j++)
		{
			for (int k = 0; k < maskTexture.Height; k++)
			{
				Rectangle value = new Rectangle(j, k, 1, 1);
				Color[] array2 = new Color[1];
				maskTexture.GetData(0, value, array2, 0, 1);
				Color color = array2[0];
				int num = color.R + color.G + color.B;
				array[j][k] = (byte)(num / 3);
			}
		}
		Mask = array;
	}

	public override void LoadContent(ContentManager content)
	{
		base.LoadContent(content);
		if (MaskTextureContentPath != null)
		{
			Texture2D maskTexture = content.Load<Texture2D>(MaskTextureContentPath);
			ApplyMaskTexture(maskTexture);
		}
	}

	public override Emitter DeepCopy()
	{
		MaskEmitter maskEmitter = new MaskEmitter();
		maskEmitter.Mask = (byte[][])Mask.Clone();
		maskEmitter.Threshold = Threshold;
		maskEmitter.Width = Width;
		maskEmitter.Height = Height;
		MaskEmitter maskEmitter2 = maskEmitter;
		CopyBaseFields(maskEmitter2);
		return maskEmitter2;
	}

	protected override void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		force = RandomHelper.NextUnitVector();
		offset = RandomHelper.ChooseOne(MaskHits);
		offset.X *= Width;
		offset.Y *= Height;
	}
}
