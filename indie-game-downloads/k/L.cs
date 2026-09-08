using System;
using System.Collections.Generic;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using p;

namespace k;

internal class L : IDisposable
{
	private class DA_0018 : IComparer<DAL>
	{
		public int Compare(DAL a, DAL b)
		{
			return b.Section.Height - a.Section.Height;
		}
	}

	private struct DAL
	{
		public int Index;

		public Rectangle Section;
	}

	private global::_0003._0003<int, Rectangle> _3A_0018 = new global::_0003._0003<int, Rectangle>(128u);

	private RenderTarget2D _3AL;

	private Rectangle[] _3A_0019 = new Rectangle[2];

	private int _3A3;

	private int _3A6;

	private List<Rectangle> _3AD = new List<Rectangle>(8);

	private List<Rectangle> _3A_0017 = new List<Rectangle>(8);

	private DA_0018 _3A_0003 = new DA_0018();

	private static List<DAL> _3Al = new List<DAL>();

	internal RenderTarget2D RenderTarget => _3AL;

	internal L(GraphicsDevice P_0, int P_1, SurfaceFormat P_2)
	{
		_3AL = new RenderTarget2D(P_0, P_1, P_1, mipMap: false, P_2, DepthFormat.Depth24Stencil8, 0, SunBurnCoreSystem.Instance.GetBestRenderTargetUsage());
		int num = P_1 / 2;
		ref Rectangle reference = ref _3A_0019[0];
		reference = new Rectangle(0, 0, num, P_1);
		ref Rectangle reference2 = ref _3A_0019[1];
		reference2 = new Rectangle(num, 0, num, P_1);
		_3A3 = P_1 * P_1;
		_6N(_3A_0019[0]);
		_6N(_3A_0019[1]);
		_0016();
	}

	internal void _6M()
	{
		_3A_0018.U();
		_6N(_3A_0019[0]);
		_6N(_3A_0019[1]);
		_0016();
	}

	public void Dispose()
	{
		_6M();
		p._0018._6_0006(ref _3AL);
	}

	internal bool _6P()
	{
		return _6e() >= _3A3;
	}

	private int _6w(Rectangle P_0)
	{
		return Math.Min(P_0.Width, P_0.Height);
	}

	internal int _6e()
	{
		int num = 0;
		for (global::_0003._0003<int, Rectangle> obj = _3A_0018.Lc(); obj != null; obj = obj._3A_0019)
		{
			foreach (Rectangle item in obj._3A6)
			{
				num += item.Width * item.Height;
			}
		}
		return num;
	}

	private void _0016()
	{
		_3AD.Clear();
		_3A_0017.Clear();
		_3A6 = _6e();
	}

	private void _6O()
	{
		foreach (Rectangle item in _3A_0017)
		{
			_6m(item);
		}
		foreach (Rectangle item2 in _3AD)
		{
			int num = _6w(item2);
			global::_0003._0003<int, Rectangle> obj = _3A_0018.LI(num, (uint)num);
			obj._3A6.Remove(item2);
		}
		_3AD.Clear();
		_3A_0017.Clear();
		int num2 = _6e();
		if (num2 != _3A6)
		{
			throw new Exception("Unable to rollback shadow cache data.");
		}
	}

	private void _6N(Rectangle P_0)
	{
		if (P_0.Width >= 1 && P_0.Height >= 1)
		{
			_6m(P_0);
			_3AD.Add(P_0);
		}
	}

	private void _6m(Rectangle P_0)
	{
		if (P_0.Width >= 1 && P_0.Height >= 1)
		{
			int num = _6w(P_0);
			global::_0003._0003<int, Rectangle> obj = _3A_0018.LI(num, (uint)num);
			obj._3A6.Add(P_0);
		}
	}

	private bool _6E(int P_0, ref Rectangle P_1)
	{
		if (P_0 < 1)
		{
			return false;
		}
		for (global::_0003._0003<int, Rectangle> obj = _3A_0018.LI(P_0, (uint)P_0); obj != null; obj = obj._3A_0019)
		{
			for (int i = 0; i < obj._3A6.Count; i++)
			{
				Rectangle item = obj._3A6[i];
				if (item.Width >= P_0)
				{
					obj._3A6.RemoveAt(i);
					_3A_0017.Add(item);
					Rectangle rectangle = default(Rectangle);
					Rectangle rectangle2 = default(Rectangle);
					rectangle.X = item.X + P_0;
					rectangle.Y = item.Y;
					rectangle.Width = item.Width - P_0;
					rectangle.Height = P_0;
					_6N(rectangle);
					rectangle2.X = item.X;
					rectangle2.Y = item.Y + P_0;
					rectangle2.Width = item.Width;
					rectangle2.Height = item.Height - P_0;
					_6N(rectangle2);
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

	internal bool _6S(List<Rectangle> P_0)
	{
		_3Al.Clear();
		for (int i = 0; i < P_0.Count; i++)
		{
			DAL item = new DAL
			{
				Index = i,
				Section = P_0[i]
			};
			_3Al.Add(item);
		}
		_3Al.Sort(_3A_0003);
		for (int j = 0; j < _3Al.Count; j++)
		{
			DAL dAL = _3Al[j];
			if (_6E(dAL.Section.Height, ref dAL.Section))
			{
				P_0[dAL.Index] = dAL.Section;
				continue;
			}
			_6O();
			return false;
		}
		_0016();
		return true;
	}
}
