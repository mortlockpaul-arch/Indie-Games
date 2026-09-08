using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Graphics;
using Quasar.ContentPipeline;
using Quasar.Global;
using Quasar.Textures;

namespace Quasar.Meshes.Text;

public class BitmapFont : IDisposable
{
	public const string FONTS_DIR = "Fonts/";

	private const string XML_FONT_ELEMENT = "font";

	private const string XML_COMMON_ELEMENT = "common";

	private const string XML_CHARS_ELEMENT = "chars";

	private const string XML_LINEHEIGHT_ATTRIBUTE = "lineHeight";

	private const string XML_BASE_ATTRIBUTE = "base";

	private const string XML_SCALEW_ATTRIBUTE = "scaleW";

	private const string XML_SCALEH_ATTRIBUTE = "scaleH";

	private const string XML_PAGES_ELEMENT = "pages";

	private const string XML_PAGE_ELEMENT = "page";

	private const string XML_FILE_ATTRIBUTE = "file";

	private const string XML_CHAR_ELEMENT = "char";

	private const string XML_ID_ATTRIBUTE = "id";

	private const string XML_X_ATTRIBUTE = "x";

	private const string XML_Y_ATTRIBUTE = "y";

	private const string XML_WIDTH_ATTRIBUTE = "width";

	private const string XML_HEIGHT_ATTRIBUTE = "height";

	private const string XML_XOFFSET_ATTRIBUTE = "xoffset";

	private const string XML_YOFFSET_ATTRIBUTE = "yoffset";

	private const string XML_XADVANCE_ATTRIBUTE = "xadvance";

	private const string XML_ADVANCEADJUST_ATTRIBUTE = "advanceAdjust";

	private const string XML_BASELINEADJUST_ATTRIBUTE = "baselineAdjust";

	private Dictionary<char, CharInfo> charInfo = new Dictionary<char, CharInfo>();

	private FontInfo fontInfo;

	private Texture fontTexture;

	public Dictionary<char, CharInfo> CharInfo => charInfo;

	public FontInfo FontInfo => fontInfo;

	public Texture Texture => fontTexture;

	public BitmapFont(string name)
	{
		processFile(name);
	}

	public int MeasureString(string text)
	{
		int num = 0;
		foreach (char key in text)
		{
			if (charInfo.ContainsKey(key))
			{
				num += charInfo[key].XAdvance;
			}
		}
		return num;
	}

	public int MeasureString(StringBuilder text)
	{
		int num = 0;
		int length = text.Length;
		for (int i = 0; i < length; i++)
		{
			char key = text[i];
			if (charInfo.ContainsKey(key))
			{
				num += charInfo[key].XAdvance;
			}
		}
		return num;
	}

	public void CountCharWidth(int pxMaxWidth, string str, int baseIndex, out int nChars, out int pxWidth)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		nChars = 0;
		pxWidth = 0;
		for (int i = baseIndex; i < str.Length; i++)
		{
			char c = str[i];
			if (c == '\n')
			{
				nChars++;
				break;
			}
			if (!this.charInfo.ContainsKey(c))
			{
				nChars++;
				continue;
			}
			CharInfo charInfo = this.charInfo[c];
			num3 = pxWidth;
			pxWidth += charInfo.XAdvance + fontInfo.AdvanceAdjust;
			nChars++;
			if (char.IsWhiteSpace(c))
			{
				num = nChars;
				num2 = num3;
			}
			if (pxWidth > pxMaxWidth)
			{
				pxWidth = num2;
				if (pxWidth == 0)
				{
					pxWidth = num3;
					nChars--;
				}
				else
				{
					nChars = num;
				}
				if (nChars == 0)
				{
					nChars = 1;
				}
				break;
			}
		}
	}

	public void CountCharWidth(int pxMaxWidth, StringBuilder builder, int baseIndex, out int nChars, out int pxWidth)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		nChars = 0;
		pxWidth = 0;
		for (int i = baseIndex; i < builder.Length; i++)
		{
			char c = builder[i];
			if (c == '\n')
			{
				nChars++;
				break;
			}
			if (!this.charInfo.ContainsKey(c))
			{
				nChars++;
				continue;
			}
			CharInfo charInfo = this.charInfo[c];
			num3 = pxWidth;
			pxWidth += charInfo.XAdvance + fontInfo.AdvanceAdjust;
			nChars++;
			if (char.IsWhiteSpace(c))
			{
				num = nChars;
				num2 = num3;
			}
			if (pxWidth > pxMaxWidth)
			{
				pxWidth = num2;
				if (pxWidth == 0)
				{
					pxWidth = num3;
					nChars--;
				}
				else
				{
					nChars = num;
				}
				if (nChars == 0)
				{
					nChars = 1;
				}
				break;
			}
		}
	}

	private void processFile(string file)
	{
		XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>(file);
		XDocument xdoc = XDocument.Parse(xmlSource.XmlCode);
		LoadFont(xdoc);
	}

	private void LoadFont(XDocument xdoc)
	{
		foreach (XElement item in xdoc.Elements("font"))
		{
			foreach (XElement item2 in item.Elements())
			{
				switch (item2.Name.LocalName)
				{
				case "common":
					LoadCommonInfo(item2);
					break;
				case "pages":
					LoadPage(item2);
					break;
				case "chars":
					LoadCharInfo(item2);
					break;
				}
			}
		}
	}

	private void LoadPage(XElement xe)
	{
		XElement xe2 = xe.Element("page");
		string text = XDocHelper.GetAttribute(xe2, "file").Replace(".png", "");
		fontTexture = TextureManager.LoadTexture("Fonts/" + text);
	}

	private void LoadCommonInfo(XElement xe)
	{
		fontInfo = default(FontInfo);
		fontInfo.LineHeight = XDocHelper.ParseIntAttribute(xe, "lineHeight", 32);
		fontInfo.BaseHeight = XDocHelper.ParseIntAttribute(xe, "base", 16);
		fontInfo.ScaleW = XDocHelper.ParseIntAttribute(xe, "scaleW", 20);
		fontInfo.ScaleH = XDocHelper.ParseIntAttribute(xe, "scaleH", 20);
		fontInfo.AdvanceAdjust = XDocHelper.ParseIntAttribute(xe, "advanceAdjust", 0);
		fontInfo.BaselineAdjust = XDocHelper.ParseIntAttribute(xe, "baselineAdjust", 0);
	}

	private void LoadCharInfo(XElement xe)
	{
		foreach (XElement item in xe.Elements("char"))
		{
			AddChar(item);
		}
	}

	private void AddChar(XElement xe)
	{
		CharInfo value = new CharInfo
		{
			Id = XDocHelper.ParseIntAttribute(xe, "id", 0),
			X = XDocHelper.ParseIntAttribute(xe, "x", 0),
			Y = XDocHelper.ParseIntAttribute(xe, "y", 0),
			Width = XDocHelper.ParseIntAttribute(xe, "width", 0),
			Height = XDocHelper.ParseIntAttribute(xe, "height", 0),
			XOffset = XDocHelper.ParseIntAttribute(xe, "xoffset", 0),
			YOffset = XDocHelper.ParseIntAttribute(xe, "yoffset", 0),
			XAdvance = XDocHelper.ParseIntAttribute(xe, "xadvance", 0)
		};
		charInfo.Add((char)value.Id, value);
	}

	public void Dispose()
	{
		charInfo.Clear();
		fontTexture = null;
		GC.SuppressFinalize(this);
	}

	~BitmapFont()
	{
		Dispose();
	}
}
