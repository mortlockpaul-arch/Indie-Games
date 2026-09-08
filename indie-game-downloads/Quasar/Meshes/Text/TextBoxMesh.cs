using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes.Text;

public class TextBoxMesh : UserIndexVertexMesh<VertexPositionUV>
{
	private const int MAX_TEXT_SIZE = 1024;

	private int bufferSize;

	private BitmapFont font;

	private bool dirty;

	private StringBuilder builder;

	private string text;

	private float zValue;

	private TextBoxDrawProperties drawProperties;

	private int charCount;

	private List<Quasar.Meshes.Text.TextBoxDrawResults> results = new List<Quasar.Meshes.Text.TextBoxDrawResults>();

	public StringBuilder StringBuilder
	{
		get
		{
			dirty = true;
			return builder;
		}
	}

	public bool UsesStringBuilder => builder != null;

	public float ZValue
	{
		get
		{
			return zValue;
		}
		set
		{
			zValue = value;
			dirty = true;
		}
	}

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			if (UsesStringBuilder)
			{
				builder.Length = 0;
				builder.Append(value);
			}
			else
			{
				text = value;
			}
			dirty = true;
		}
	}

	public Vector2 Size
	{
		get
		{
			return drawProperties.Size;
		}
		set
		{
			drawProperties.Size = value;
			dirty = true;
		}
	}

	public Layout2D.LayoutData Layout
	{
		set
		{
			drawProperties.Layout = value;
			dirty = true;
		}
	}

	public float LineSpacing
	{
		get
		{
			return drawProperties.LineSpacing;
		}
		set
		{
			drawProperties.LineSpacing = value;
			dirty = true;
		}
	}

	public Vector2 Offset
	{
		get
		{
			return drawProperties.Offset;
		}
		set
		{
			drawProperties.Offset = value;
			dirty = true;
		}
	}

	public float Scale
	{
		get
		{
			return drawProperties.Scale;
		}
		set
		{
			drawProperties.Scale = value;
			dirty = true;
		}
	}

	public TextBoxDrawProperties DrawProperties
	{
		set
		{
			drawProperties = value;
		}
	}

	public bool Empty
	{
		get
		{
			if (UsesStringBuilder)
			{
				return builder.Length == 0;
			}
			if (text != null)
			{
				return text.Length == 0;
			}
			return true;
		}
	}

	public TextBoxMesh(BitmapFont font, TextBoxDrawProperties properties)
		: this(font, properties, 1024, useStringBuilder: false)
	{
	}

	public TextBoxMesh(BitmapFont font, TextBoxDrawProperties properties, int maxBufferSize, bool useStringBuilder)
	{
		this.font = font;
		drawProperties = properties;
		Material material = new Material();
		base.Materials.Add(material);
		material.Texture = font.Texture;
		material.SetForcedAlpha(alpha: true);
		shader = ShaderManager.Shaders["GUI"];
		bufferSize = maxBufferSize;
		if (useStringBuilder)
		{
			builder = new StringBuilder(maxBufferSize);
		}
		initMesh();
		hasSimpleLayout = true;
		DebugColor = new Color(255, 206, 121);
	}

	private void initMesh()
	{
		initMesh(4 * bufferSize, 6 * bufferSize);
		initializeIndexBuffer();
	}

	private void initializeIndexBuffer()
	{
		for (int i = 0; i < bufferSize; i++)
		{
			int num = i * 6;
			int num2 = i * 4;
			indicesBuffer[num] = (short)num2;
			indicesBuffer[num + 1] = (short)(num2 + 2);
			indicesBuffer[num + 2] = (short)(num2 + 1);
			indicesBuffer[num + 3] = (short)(num2 + 2);
			indicesBuffer[num + 4] = (short)(num2 + 3);
			indicesBuffer[num + 5] = (short)(num2 + 1);
		}
	}

	private void updateData()
	{
		charCount = 0;
		dirty = false;
		if (Empty)
		{
			base.PrimitiveCount = 0;
			return;
		}
		results.Clear();
		int i = 0;
		int nChars;
		for (int num = (UsesStringBuilder ? builder.Length : text.Length); num > i; i += nChars)
		{
			int pxWidth;
			if (UsesStringBuilder)
			{
				font.CountCharWidth((int)(drawProperties.Size.X / drawProperties.Scale), builder, i, out nChars, out pxWidth);
			}
			else
			{
				font.CountCharWidth((int)(drawProperties.Size.X / drawProperties.Scale), text, i, out nChars, out pxWidth);
			}
			results.Add(new Quasar.Meshes.Text.TextBoxDrawResults(i, nChars, pxWidth));
		}
		Vector2 vAt = new Vector2((0f - drawProperties.Size.X) / 2f, drawProperties.Size.Y / 2f);
		int num2 = Math.Max(1, (int)Math.Floor(drawProperties.Size.Y / ((float)font.FontInfo.BaseHeight * drawProperties.Scale * drawProperties.LineSpacing)));
		int num3 = 0;
		foreach (Quasar.Meshes.Text.TextBoxDrawResults result in results)
		{
			if (num3 >= num2)
			{
				break;
			}
			switch (drawProperties.HorizontalAlignment)
			{
			case HorizontalAlignment.Left:
				vAt.X = (0f - drawProperties.Size.X) / 2f;
				break;
			case HorizontalAlignment.Center:
				vAt.X = (0f - drawProperties.Size.X) / 2f + (drawProperties.Size.X - (float)result.pxWidth * drawProperties.Scale) / 2f;
				break;
			case HorizontalAlignment.Right:
				vAt.X = (0f - drawProperties.Size.X) / 2f + (drawProperties.Size.X - (float)result.pxWidth * drawProperties.Scale);
				break;
			}
			switch (drawProperties.VerticalAlignment)
			{
			case VerticalAlignment.Top:
				vAt.Y = drawProperties.Size.Y / 2f - ((float)num3 * LineSpacing * (float)font.FontInfo.BaseHeight + (float)(font.FontInfo.LineHeight - font.FontInfo.BaseHeight)) * drawProperties.Scale;
				break;
			case VerticalAlignment.Center:
				vAt.Y = (float)font.FontInfo.BaseHeight * ((float)Math.Min(results.Count, num2) / 2f - ((float)num3 * LineSpacing + 0.25f)) * drawProperties.Scale;
				break;
			case VerticalAlignment.Bottom:
				vAt.Y = (0f - drawProperties.Size.Y) / 2f + ((float)(results.Count - 1 - num3) * LineSpacing * (float)font.FontInfo.BaseHeight + 0.5f * (float)font.FontInfo.LineHeight) * drawProperties.Scale;
				break;
			}
			addData(vAt, result.baseIndex, result.charCount);
			num3++;
		}
	}

	private void addData(Vector2 vAt, int baseStrIndex, int nChars)
	{
		new Vector2(0f, 0f);
		for (int i = baseStrIndex; i < baseStrIndex + nChars; i++)
		{
			char key = ((!UsesStringBuilder) ? text[i] : builder[i]);
			int num = 4 * charCount;
			if (charCount >= bufferSize)
			{
				break;
			}
			if (font.CharInfo.ContainsKey(key))
			{
				CharInfo charInfo = font.CharInfo[key];
				vAt.X += (float)charInfo.XOffset * drawProperties.Scale;
				float num2 = vAt.Y - (float)charInfo.YOffset * drawProperties.Scale + (float)font.FontInfo.BaselineAdjust * drawProperties.Scale;
				float num3 = (float)charInfo.Height * drawProperties.Scale;
				float num4 = (float)(font.FontInfo.BaseHeight / 2) * drawProperties.Scale;
				if (charInfo.Width != 0 && charInfo.Height != 0)
				{
					verticesBuffer[num + 2].UV = new Vector2((float)charInfo.X / (float)font.FontInfo.ScaleW, (float)charInfo.Y / (float)font.FontInfo.ScaleH);
					verticesBuffer[num + 3].UV = new Vector2((float)(charInfo.X + charInfo.Width) / (float)font.FontInfo.ScaleW, (float)charInfo.Y / (float)font.FontInfo.ScaleH);
					verticesBuffer[num].UV = new Vector2((float)charInfo.X / (float)font.FontInfo.ScaleW, (float)(charInfo.Y + charInfo.Height) / (float)font.FontInfo.ScaleH);
					verticesBuffer[num + 1].UV = new Vector2((float)(charInfo.X + charInfo.Width) / (float)font.FontInfo.ScaleW, (float)(charInfo.Y + charInfo.Height) / (float)font.FontInfo.ScaleH);
					verticesBuffer[num].Position = new Vector3(vAt.X, num2 + num4 - num3, zValue);
					verticesBuffer[num + 1].Position = new Vector3(vAt.X + (float)charInfo.Width * drawProperties.Scale, num2 + num4 - num3, zValue);
					verticesBuffer[num + 2].Position = new Vector3(vAt.X, num2 + num4, zValue);
					verticesBuffer[num + 3].Position = new Vector3(vAt.X + (float)charInfo.Width * drawProperties.Scale, num2 + num4, zValue);
					verticesBuffer[num].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
					verticesBuffer[num + 1].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
					verticesBuffer[num + 2].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
					verticesBuffer[num + 3].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
				}
				vAt.X += (float)(charInfo.XAdvance + font.FontInfo.AdvanceAdjust) * drawProperties.Scale - (float)charInfo.XOffset * drawProperties.Scale;
				charCount++;
			}
		}
		base.PrimitiveCount = charCount * 2;
	}

	public override void Render(Transform motion)
	{
		if (dirty)
		{
			updateData();
		}
		base.Render(motion);
	}

	public override void Dispose()
	{
		font = null;
		base.Dispose();
	}
}
