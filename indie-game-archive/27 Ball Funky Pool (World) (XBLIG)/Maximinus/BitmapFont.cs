using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class BitmapFont
{
	public enum TextAlignment
	{
		Left,
		Center,
		Right
	}

	public struct SaveStateInfo
	{
		public bool fKern;

		public float fpDepth;

		public TextAlignment align;

		public Vector2 vPen;

		public Color color;
	}

	private enum GlyphFlags
	{
		None,
		ForceWhite
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct GlyphInfo
	{
	}

	[StructLayout(LayoutKind.Sequential, Size = 1)]
	private struct BitmapInfo
	{
	}

	private static Dictionary<string, BitmapFont> m_dictBitmapFonts = new Dictionary<string, BitmapFont>();

	public int LineHeight;

	public BitmapFont(string strFontFilename)
	{
		throw new Exception("xml not found on xbox");
	}

	public BitmapFont(ContentManager cm, string strFontFilename)
	{
		throw new Exception("xml not found on xbox");
	}

	public void SpriteBatchOverride(SpriteBatch sb)
	{
	}

	public void Reset(GraphicsDevice d)
	{
	}

	public void DrawString(Vector2 v, Color c, string s)
	{
	}

	public int MeasureStringWidth(string s)
	{
		return 0;
	}
}
