using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using k;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Class that manages render target sections used for shadow mapping.
/// </summary>
public class ShadowMapCache
{
	private class DA_0018
	{
		public SurfaceFormat Format;

		public int TexelBytes;

		public DA_0018(SurfaceFormat format, int texelbytes)
		{
			Format = format;
			TexelBytes = texelbytes;
		}
	}

	internal class DAL
	{
		internal SystemStatistic _3A_0018 = SystemConsole.GetStatistic("Shadow_TotalPages", SystemStatisticCategory.Shadowing);

		internal SystemStatistic _3AL = SystemConsole.GetStatistic("Shadow_TotalMemoryUsage", SystemStatisticCategory.Shadowing);

		internal SystemStatistic _3A_0019 = SystemConsole.GetStatistic("Shadow_ActivePages", SystemStatisticCategory.Shadowing);

		internal SystemStatistic _3A3 = SystemConsole.GetStatistic("Shadow_ActiveMemoryUsage", SystemStatisticCategory.Shadowing);
	}

	private DA_0018[] _3A_0018 = new DA_0018[5]
	{
		new DA_0018(SurfaceFormat.Single, 4),
		new DA_0018(SurfaceFormat.HalfSingle, 2),
		new DA_0018(SurfaceFormat.HalfVector2, 4),
		new DA_0018(SurfaceFormat.Single, 4),
		new DA_0018(SurfaceFormat.Color, 4)
	};

	private SurfaceFormat _3AL = SurfaceFormat.Single;

	private int _3A_0019;

	private int _3A3;

	private int _3A6 = 2048;

	private int _3AD;

	private bool _3A_0017 = true;

	private IGraphicsDeviceService _3A_0003;

	private List<k.L> _3Al = new List<k.L>(8);

	internal DAL _3At = new DAL();

	/// <summary>
	/// Maximum amount of memory the cache is allowed to consume. This is an
	/// approximate value and the cache may use more memory in certain instances.
	/// </summary>
	public int MaxMemoryUsage => _3AD;

	/// <summary>
	/// True when smaller half-float format render targets are preferred. These
	/// formats consume less memory and generally perform better, but have lower
	/// accuracy on directional lights.
	/// </summary>
	public bool PreferHalfFloatTextureFormat => _3A_0017;

	/// <summary>
	/// Size in pixels of each render target (page) in the cache. For a size of 1024
	/// the actual page dimensions are 1024x1024. Small sizes can reduce performance by
	/// fragmenting the shadow maps, and reduce shadow quality by lowering the maximum
	/// resolution of each shadow map section.
	/// </summary>
	public int PageSize => _3A6;

	/// <summary>
	/// Creates a new ShadowMapCache instance.
	/// </summary>
	/// <param name="pagesize">Size in pixels of each render target (page) in the cache.
	/// For a size of 1024 the actual page dimensions are 1024x1024. Small sizes can reduce
	/// performance by fragmenting the shadow maps, and reduce shadow quality by lowering
	/// the maximum resolution of each shadow map section.</param>
	/// <param name="maxmemoryusage">Maximum amount of memory the cache is allowed to consume.
	/// This is an approximate value and the cache may use more memory in certain instances.</param>
	/// <param name="preferhalffloat">True when smaller half-float format render targets are
	/// preferred. These formats consume less memory and generally perform better, but have
	/// lower accuracy on directional lights.</param>
	public ShadowMapCache(int pagesize, int maxmemoryusage, bool preferhalffloat)
	{
		_3A_0003 = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
		Resize(pagesize, maxmemoryusage, preferhalffloat);
	}

	/// <summary>
	/// Resizes shadow maps and memory usage.
	/// </summary>
	/// <param name="pagesize">Size in pixels of each render target (page) in the cache.
	/// For a size of 1024 the actual page dimensions are 1024x1024. Small sizes can reduce
	/// performance by fragmenting the shadow maps, and reduce shadow quality by lowering
	/// the maximum resolution of each shadow map section.</param>
	/// <param name="maxmemoryusage">Maximum amount of memory the cache is allowed to consume.
	/// This is an approximate value and the cache may use more memory in certain instances.</param>
	/// <param name="preferhalffloat">True when smaller half-float format render targets are
	/// preferred. These formats consume less memory and generally perform better, but have
	/// lower accuracy on directional lights.</param>
	public void Resize(int pagesize, int maxmemoryusage, bool preferhalffloat)
	{
		Unload();
		_3A6 = pagesize;
		_3AD = maxmemoryusage;
		_3A_0017 = preferhalffloat;
		_3A3 = 0;
	}

	private void _6Q()
	{
		if (_3A3 > 0)
		{
			return;
		}
		GraphicsDeviceSupport graphicsDeviceSupport = SunBurnCoreSystem.Instance.GetGraphicsDeviceSupport();
		int num = 0;
		if (_3A_0017)
		{
			num = 1;
		}
		for (int i = num; i < _3A_0018.Length; i++)
		{
			DA_0018 obj = _3A_0018[i];
			if (graphicsDeviceSupport.SurfaceFormat[obj.Format])
			{
				_3AL = obj.Format;
				_3A_0019 = obj.TexelBytes;
				_3A3 = _3A6 * _3A6 * _3A_0019;
				break;
			}
		}
		if (_3A3 >= 1)
		{
			return;
		}
		throw new Exception("Unable to find a valid shadow buffer render target format.");
	}

	/// <summary>
	/// Attempts to reserve the requested shadow map sections in a
	/// single render target. If successful the render target is
	/// returned, otherwise null is returned.
	/// </summary>
	/// <param name="sectionsizes"></param>
	/// <returns></returns>
	public RenderTarget2D ReserveSections(List<Rectangle> sectionsizes)
	{
		_6Q();
		int num = 0;
		foreach (Rectangle sectionsize in sectionsizes)
		{
			num += sectionsize.Width * sectionsize.Height;
		}
		if (num > _3A6 * _3A6)
		{
			return null;
		}
		foreach (k.L item in _3Al)
		{
			if (item._6S(sectionsizes))
			{
				_6V(sectionsizes);
				return item.RenderTarget;
			}
			if (item._6P())
			{
				return null;
			}
		}
		if (_3Al.Count > 0 && (_3Al.Count + 1) * _3A3 > _3AD)
		{
			return null;
		}
		k.L l = new k.L(_3A_0003.GraphicsDevice, _3A6, _3AL);
		_3Al.Add(l);
		if (l._6S(sectionsizes))
		{
			_6V(sectionsizes);
			return l.RenderTarget;
		}
		return null;
	}

	internal float _6B()
	{
		if (_3Al.Count < 1)
		{
			return 0f;
		}
		int num = 0;
		foreach (k.L item in _3Al)
		{
			if (!item._6P())
			{
				num++;
			}
		}
		return (float)num / (float)_3Al.Count;
	}

	private void _6V(List<Rectangle> P_0)
	{
		_3At._3A_0018.AccumulationValue = _3Al.Count;
		_3At._3AL.AccumulationValue = _3Al.Count * _3A3;
		_3At._3A_0019.AccumulationValue = 0;
		_3At._3A3.AccumulationValue = 0;
		foreach (k.L item in _3Al)
		{
			if (!item._6P())
			{
				_3At._3A_0019.AccumulationValue++;
				_3At._3A3.AccumulationValue += _3A3;
			}
		}
	}

	/// <summary>
	/// Clears all reserved shadow map sections, allowing the sections to be reused
	/// in future shadow maps section requests.
	/// </summary>
	public void ClearReserves()
	{
		foreach (k.L item in _3Al)
		{
			item._6M();
		}
	}

	/// <summary>
	/// Disposes any graphics resources used internally by this object, and clears
	/// all reserved shadow map sections. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		foreach (k.L item in _3Al)
		{
			item.Dispose();
		}
		_3Al.Clear();
	}
}
