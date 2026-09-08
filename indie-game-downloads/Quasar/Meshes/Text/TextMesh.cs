using System.Text;
using Microsoft.Xna.Framework;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes.Text;

public class TextMesh : UserIndexVertexMesh<VertexPositionUV>
{
	private const int MAX_TEXT_SIZE = 1024;

	private int bufferSize;

	private BitmapFont font;

	private bool dirty;

	private StringBuilder builder;

	private string text;

	private TextDrawProperties drawProperties;

	private int charCount;

	public BitmapFont Font => font;

	public StringBuilder StringBuilder
	{
		get
		{
			dirty = true;
			return builder;
		}
	}

	public bool UsesStringBuilder => builder != null;

	public string Text
	{
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

	public TextDrawProperties DrawProperties
	{
		get
		{
			return drawProperties;
		}
		set
		{
			drawProperties = value;
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

	public HorizontalAlignment Alignment
	{
		get
		{
			return drawProperties.Alignment;
		}
		set
		{
			drawProperties.Alignment = value;
			dirty = true;
		}
	}

	public float ZValue
	{
		get
		{
			return drawProperties.ZValue;
		}
		set
		{
			drawProperties.ZValue = value;
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

	public TextMesh(BitmapFont font)
		: this(font, new TextDrawProperties(Vector2.Zero))
	{
	}

	public TextMesh(BitmapFont font, TextDrawProperties properties)
		: this(font, properties, 1024, useStringBuilder: false)
	{
	}

	public TextMesh(BitmapFont font, TextDrawProperties properties, int maxBufferSize, bool useStringBuilder)
	{
		bufferSize = maxBufferSize;
		drawProperties = properties;
		this.font = font;
		Material material = new Material();
		base.Materials.Add(material);
		material.Texture = font.Texture;
		material.SetForcedAlpha(alpha: true);
		shader = ShaderManager.Shaders["GUI"];
		if (useStringBuilder)
		{
			builder = new StringBuilder(bufferSize);
		}
		initMesh();
		hasSimpleLayout = true;
		DebugColor = new Color(255, 255, 0);
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
		new Vector2(0f, 0f);
		charCount = 0;
		if (Empty)
		{
			base.PrimitiveCount = 0;
			return;
		}
		Vector2 zero = Vector2.Zero;
		if (drawProperties.Alignment != HorizontalAlignment.Left)
		{
			int nChars;
			int pxWidth;
			if (UsesStringBuilder)
			{
				font.CountCharWidth(int.MaxValue, builder, 0, out nChars, out pxWidth);
			}
			else
			{
				font.CountCharWidth(int.MaxValue, text, 0, out nChars, out pxWidth);
			}
			switch (drawProperties.Alignment)
			{
			case HorizontalAlignment.Center:
				zero.X = (float)(-pxWidth) * 0.5f * drawProperties.Scale;
				break;
			case HorizontalAlignment.Right:
				zero.X = (float)(-pxWidth) * drawProperties.Scale;
				break;
			}
		}
		int num = (UsesStringBuilder ? builder.Length : text.Length);
		for (int i = 0; i < num; i++)
		{
			char key = (UsesStringBuilder ? builder[i] : text[i]);
			int num2 = 4 * charCount;
			if (font.CharInfo.ContainsKey(key))
			{
				if (charCount >= bufferSize)
				{
					break;
				}
				CharInfo charInfo = font.CharInfo[key];
				zero.X += (float)charInfo.XOffset * drawProperties.Scale;
				float num3 = zero.Y - (float)charInfo.YOffset * drawProperties.Scale + (float)font.FontInfo.BaselineAdjust * drawProperties.Scale;
				float num4 = (float)charInfo.Height * drawProperties.Scale;
				float num5 = (float)(font.FontInfo.BaseHeight / 2) * drawProperties.Scale;
				if (charInfo.Width != 0 && charInfo.Height != 0)
				{
					verticesBuffer[num2 + 2].UV = new Vector2((float)charInfo.X / (float)font.FontInfo.ScaleW, (float)charInfo.Y / (float)font.FontInfo.ScaleH);
					verticesBuffer[num2 + 3].UV = new Vector2((float)(charInfo.X + charInfo.Width) / (float)font.FontInfo.ScaleW, (float)charInfo.Y / (float)font.FontInfo.ScaleH);
					verticesBuffer[num2].UV = new Vector2((float)charInfo.X / (float)font.FontInfo.ScaleW, (float)(charInfo.Y + charInfo.Height) / (float)font.FontInfo.ScaleH);
					verticesBuffer[num2 + 1].UV = new Vector2((float)(charInfo.X + charInfo.Width) / (float)font.FontInfo.ScaleW, (float)(charInfo.Y + charInfo.Height) / (float)font.FontInfo.ScaleH);
					verticesBuffer[num2].Position = new Vector3(zero.X, num3 + num5 - num4, 0f);
					verticesBuffer[num2 + 1].Position = new Vector3(zero.X + (float)charInfo.Width * drawProperties.Scale, num3 + num5 - num4, 0f);
					verticesBuffer[num2 + 2].Position = new Vector3(zero.X, num3 + num5, 0f);
					verticesBuffer[num2 + 3].Position = new Vector3(zero.X + (float)charInfo.Width * drawProperties.Scale, num3 + num5, 0f);
					verticesBuffer[num2].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
					verticesBuffer[num2 + 1].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
					verticesBuffer[num2 + 2].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
					verticesBuffer[num2 + 3].Position += new Vector3(drawProperties.Offset, drawProperties.ZValue);
				}
				zero.X += (float)(charInfo.XAdvance + font.FontInfo.AdvanceAdjust) * drawProperties.Scale - (float)charInfo.XOffset * drawProperties.Scale;
				charCount++;
			}
		}
		base.PrimitiveCount = charCount * 2;
		dirty = false;
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
