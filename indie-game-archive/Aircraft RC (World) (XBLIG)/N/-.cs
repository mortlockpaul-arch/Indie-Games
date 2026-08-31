using System;
using System.Collections.Generic;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using Z;

namespace N;

internal class _0002 : IDisposable
{
	private class _0001CB : IComparer<_0001C_0002>
	{
		public int Compare(_0001C_0002 a, _0001C_0002 b)
		{
			return b.Section.Height - a.Section.Height;
		}
	}

	private struct _0001C_0002
	{
		public int Index;

		public Rectangle Section;
	}

	private global::Z.Z<int, Rectangle> HCB = new global::Z.Z<int, Rectangle>(128u);

	private RenderTarget2D HC_0002;

	private Rectangle[] HC_0012 = new Rectangle[2];

	private int HCH;

	private int HC7;

	private List<Rectangle> HC_0001 = new List<Rectangle>(8);

	private List<Rectangle> HCw = new List<Rectangle>(8);

	private _0001CB HCZ = new _0001CB();

	private static List<_0001C_0002> HC_000F = new List<_0001C_0002>();

	internal RenderTarget2D RenderTarget => HC_0002;

	internal _0002(GraphicsDevice P_0, int P_1, SurfaceFormat P_2)
	{
		HC_0002 = new RenderTarget2D(P_0, P_1, P_1, mipMap: false, P_2, DepthFormat.Depth24Stencil8, 0, SunBurnCoreSystem.Instance.GetBestRenderTargetUsage());
		int num = P_1 / 2;
		ref Rectangle reference = ref HC_0012[0];
		reference = new Rectangle(0, 0, num, P_1);
		ref Rectangle reference2 = ref HC_0012[1];
		reference2 = new Rectangle(num, 0, num, P_1);
		HCH = P_1 * P_1;
		_7_0018(HC_0012[0]);
		_7_0018(HC_0012[1]);
		R();
	}

	internal void _71()
	{
		HCB.G();
		_7_0018(HC_0012[0]);
		_7_0018(HC_0012[1]);
		R();
	}

	public void Dispose()
	{
		_71();
		F.B._7_0004(ref HC_0002);
	}

	internal bool _7b()
	{
		return _7g() >= HCH;
	}

	private int _7l(Rectangle P_0)
	{
		return Math.Min(P_0.Width, P_0.Height);
	}

	internal int _7g()
	{
		int num = 0;
		for (global::Z.Z<int, Rectangle> z = HCB._0002D(); z != null; z = z.HC_0012)
		{
			foreach (Rectangle item in z.HC7)
			{
				num += item.Width * item.Height;
			}
		}
		return num;
	}

	private void R()
	{
		HC_0001.Clear();
		HCw.Clear();
		HC7 = _7g();
	}

	private void _7W()
	{
		foreach (Rectangle item in HCw)
		{
			_7_000E(item);
		}
		foreach (Rectangle item2 in HC_0001)
		{
			int num = _7l(item2);
			global::Z.Z<int, Rectangle> z = HCB._0002K(num, (uint)num);
			z.HC7.Remove(item2);
		}
		HC_0001.Clear();
		HCw.Clear();
		int num2 = _7g();
		if (num2 != HC7)
		{
			throw new Exception("Unable to rollback shadow cache data.");
		}
	}

	private void _7_0018(Rectangle P_0)
	{
		if (P_0.Width >= 1 && P_0.Height >= 1)
		{
			_7_000E(P_0);
			HC_0001.Add(P_0);
		}
	}

	private void _7_000E(Rectangle P_0)
	{
		if (P_0.Width >= 1 && P_0.Height >= 1)
		{
			int num = _7l(P_0);
			global::Z.Z<int, Rectangle> z = HCB._0002K(num, (uint)num);
			z.HC7.Add(P_0);
		}
	}

	private bool _7d(int P_0, ref Rectangle P_1)
	{
		if (P_0 < 1)
		{
			return false;
		}
		for (global::Z.Z<int, Rectangle> z = HCB._0002K(P_0, (uint)P_0); z != null; z = z.HC_0012)
		{
			for (int i = 0; i < z.HC7.Count; i++)
			{
				Rectangle item = z.HC7[i];
				if (item.Width >= P_0)
				{
					z.HC7.RemoveAt(i);
					HCw.Add(item);
					Rectangle rectangle = default(Rectangle);
					Rectangle rectangle2 = default(Rectangle);
					rectangle.X = item.X + P_0;
					rectangle.Y = item.Y;
					rectangle.Width = item.Width - P_0;
					rectangle.Height = P_0;
					_7_0018(rectangle);
					rectangle2.X = item.X;
					rectangle2.Y = item.Y + P_0;
					rectangle2.Width = item.Width;
					rectangle2.Height = item.Height - P_0;
					_7_0018(rectangle2);
					P_1.X = item.X;
					P_1.Y = item.Y;
					P_1.Width = P_0;
					P_1.Height = P_0;
					return true;
				}
			}
		}
		return false;
	}

	internal bool _7n(List<Rectangle> P_0)
	{
		HC_000F.Clear();
		for (int i = 0; i < P_0.Count; i++)
		{
			_0001C_0002 item = new _0001C_0002
			{
				Index = i,
				Section = P_0[i]
			};
			HC_000F.Add(item);
		}
		HC_000F.Sort(HCZ);
		for (int j = 0; j < HC_000F.Count; j++)
		{
			_0001C_0002 obj = HC_000F[j];
			if (_7d(obj.Section.Height, ref obj.Section))
			{
				P_0[obj.Index] = obj.Section;
				continue;
			}
			_7W();
			return false;
		}
		R();
		return true;
	}
}
