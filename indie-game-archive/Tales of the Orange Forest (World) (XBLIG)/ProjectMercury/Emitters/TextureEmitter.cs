#define DEBUG
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectMercury.Emitters;

[Obsolete("Replaced by MaskEmitter")]
public class TextureEmitter : Emitter
{
	private Matrix ScaleMatrix;

	private Vector2 TextureOrigin;

	private Vector2[] PixelOffsets;

	private Vector3[] PixelColours;

	private Texture2D _texture;

	public float Threshold;

	public float Scale
	{
		get
		{
			return ScaleMatrix.M11;
		}
		set
		{
			ScaleMatrix = Matrix.CreateScale(value);
		}
	}

	public Texture2D Texture
	{
		get
		{
			return _texture;
		}
		set
		{
			Guard.ArgumentNull("Texture", value);
			if (Texture != value)
			{
				_texture = value;
				CalculateEmissionPoints();
			}
		}
	}

	public bool ApplyPixelColours { get; set; }

	public TextureEmitter()
	{
		Scale = 1f;
		PixelOffsets = new Vector2[0];
		Threshold = 0.5f;
	}

	private void CalculateEmissionPoints()
	{
		if (Texture == null)
		{
			TextureOrigin = Vector2.Zero;
			Array.Resize(ref PixelOffsets, 0);
			Array.Resize(ref PixelColours, 0);
			return;
		}
		TextureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);
		List<Vector2> list = new List<Vector2>();
		List<Vector3> list2 = new List<Vector3>();
		Color[] array = new Color[Texture.Width * Texture.Height];
		Texture.GetData(array);
		int num = 0;
		byte b = Convert.ToByte(Threshold * 255f);
		for (int i = 0; i < Texture.Width; i++)
		{
			for (int j = 0; j < Texture.Height; j++)
			{
				int num2 = Texture.Width * j + i;
				if (array[num2].A >= b)
				{
					list.Add(new Vector2
					{
						X = (float)i - TextureOrigin.X,
						Y = (float)j - TextureOrigin.Y
					});
					list2.Add(array[num2].ToVector3());
					num++;
				}
			}
		}
		PixelOffsets = list.ToArray();
		PixelColours = list2.ToArray();
	}

	public override Emitter DeepCopy()
	{
		TextureEmitter textureEmitter = new TextureEmitter();
		textureEmitter.ApplyPixelColours = ApplyPixelColours;
		textureEmitter.Scale = Scale;
		textureEmitter.Texture = Texture;
		textureEmitter.Threshold = Threshold;
		Emitter emitter = textureEmitter;
		CopyBaseFields(emitter);
		return emitter;
	}

	protected override void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		int num = RandomHelper.NextInt(PixelOffsets.Length);
		offset = PixelOffsets[num];
		offset.X *= Scale;
		offset.Y *= Scale;
		if (ApplyPixelColours)
		{
			ReleaseColour = PixelColours[num];
		}
		force = RandomHelper.NextUnitVector();
	}
}
