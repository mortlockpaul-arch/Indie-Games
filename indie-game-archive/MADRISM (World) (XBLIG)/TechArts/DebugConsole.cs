using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TechArts
{
	public class DebugConsole
	{
		private SpriteBatch sb;

		private SpriteFont sf;

		private List<string> strs;

		private Vector2 opos;

		private Color col;

		private int lines;

		private int linespc;

		public DebugConsole(SpriteBatch b, SpriteFont f, Vector2 p, int l, Color c)
		{
			sb = b;
			sf = f;
			opos = p;
			lines = l;
			linespc = f.LineSpacing;
			col = c;
			strs = new List<string>();
		}

		public DebugConsole(SpriteBatch b, SpriteFont f, Vector2 p, int l, Color c, int spc)
		{
			sb = b;
			sf = f;
			opos = p;
			lines = l;
			linespc = spc;
			col = c;
			strs = new List<string>();
		}

		public void Clear()
		{
			strs.Clear();
		}

		public void LineFeed(int n)
		{
			for (int i = 0; i < n; i++)
			{
				strs.Add("");
			}
		}

		public void Puts(string s)
		{
			strs.Add(s);
			if (strs.Count > lines)
			{
				strs.RemoveAt(0);
			}
		}

		public void Append(string s)
		{
			string text = strs[strs.Count - 1];
			strs.RemoveAt(strs.Count - 1);
			Puts(text + s);
		}

		public void Draw()
		{
			Vector2 position = opos;
			foreach (string str in strs)
			{
				sb.DrawString(sf, str, position, col);
				position.Y += linespc;
			}
		}
	}
}
