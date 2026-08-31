using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using N;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Class that manages render target sections used for shadow mapping.
/// </summary>
public class ShadowMapCache
{
	private class _0001CB
	{
		public SurfaceFormat Format;

		public int TexelBytes;

		public _0001CB(SurfaceFormat format, int texelbytes)
		{
			Format = format;
			TexelBytes = texelbytes;
		}
	}

	internal class _0001C_0002
	{
		internal SystemStatistic HCB = SystemConsole.GetStatistic("Shadow_TotalPages", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HC_0002 = SystemConsole.GetStatistic("Shadow_TotalMemoryUsage", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HC_0012 = SystemConsole.GetStatistic("Shadow_ActivePages", SystemStatisticCategory.Shadowing);

		internal SystemStatistic HCH = SystemConsole.GetStatistic("Shadow_ActiveMemoryUsage", SystemStatisticCategory.Shadowing);
	}

	private _0001CB[] HCB = new _0001CB[5]
	{
		new _0001CB(SurfaceFormat.Single, 4),
		new _0001CB(SurfaceFormat.HalfSingle, 2),
		new _0001CB(SurfaceFormat.HalfVector2, 4),
		new _0001CB(SurfaceFormat.Single, 4),
		new _0001CB(SurfaceFormat.Color, 4)
	};

	private SurfaceFormat HC_0002 = SurfaceFormat.Single;

	private int HC_0012;

	private int HCH;

	private int HC7 = 2048;

	private int HC_0001;

	private bool HCw = true;

	private IGraphicsDeviceService HCZ;

	private List<N._0002> HC_000F = new List<N._0002>(8);

	internal _0001C_0002 HCy = new _0001C_0002();

	/// <summary>
	/// Maximum amount of memory the cache is allowed to consume. This is an
	/// approximate value and the cache may use more memory in certain instances.
	/// </summary>
	public int MaxMemoryUsage => HC_0001;

	/// <summary>
	/// True when smaller half-float format render targets are preferred. These
	/// formats consume less memory and generally perform better, but have lower
	/// accuracy on directional lights.
	/// </summary>
	public bool PreferHalfFloatTextureFormat => HCw;

	/// <summary>
	/// Size in pixels of each render target (page) in the cache. For a size of 1024
	/// the actual page dimensions are 1024x1024. Small sizes can reduce performance by
	/// fragmenting the shadow maps, and reduce shadow quality by lowering the maximum
	/// resolution of each shadow map section.
	/// </summary>
	public int PageSize => HC7;

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
		HCZ = SunBurnCoreSystem.Instance.GraphicsDeviceManager;
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
		HC7 = pagesize;
		HC_0001 = maxmemoryusage;
		HCw = preferhalffloat;
		HCH = 0;
	}

	private void _75()
	{
		if (HCH > 0)
		{
			return;
		}
		GraphicsDeviceSupport graphicsDeviceSupport = SunBurnCoreSystem.Instance.GetGraphicsDeviceSupport();
		int num = 0;
		if (HCw)
		{
			num = 1;
		}
		for (int i = num; i < HCB.Length; i++)
		{
			_0001CB obj = HCB[i];
			if (graphicsDeviceSupport.SurfaceFormat[obj.Format])
			{
				HC_0002 = obj.Format;
				HC_0012 = obj.TexelBytes;
				HCH = HC7 * HC7 * HC_0012;
				break;
			}
		}
		if (HCH >= 1)
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
		_75();
		int num = 0;
		foreach (Rectangle sectionsize in sectionsizes)
		{
			num += sectionsize.Width * sectionsize.Height;
		}
		if (num > HC7 * HC7)
		{
			return null;
		}
		foreach (N._0002 item in HC_000F)
		{
			if (item._7n(sectionsizes))
			{
				_7E(sectionsizes);
				return item.RenderTarget;
			}
			if (item._7b())
			{
				return null;
			}
		}
		if (HC_000F.Count > 0 && (HC_000F.Count + 1) * HCH > HC_0001)
		{
			return null;
		}
		N._0002 obj = new N._0002(HCZ.GraphicsDevice, HC7, HC_0002);
		HC_000F.Add(obj);
		if (obj._7n(sectionsizes))
		{
			_7E(sectionsizes);
			return obj.RenderTarget;
		}
		return null;
	}

	internal float _73()
	{
		if (HC_000F.Count < 1)
		{
			return 0f;
		}
		int num = 0;
		foreach (N._0002 item in HC_000F)
		{
			if (!item._7b())
			{
				num++;
			}
		}
		return (float)num / (float)HC_000F.Count;
	}

	private void _7E(List<Rectangle> P_0)
	{
		HCy.HCB.AccumulationValue = HC_000F.Count;
		HCy.HC_0002.AccumulationValue = HC_000F.Count * HCH;
		HCy.HC_0012.AccumulationValue = 0;
		HCy.HCH.AccumulationValue = 0;
		foreach (N._0002 item in HC_000F)
		{
			if (!item._7b())
			{
				HCy.HC_0012.AccumulationValue++;
				HCy.HCH.AccumulationValue += HCH;
			}
		}
	}

	/// <summary>
	/// Clears all reserved shadow map sections, allowing the sections to be reused
	/// in future shadow maps section requests.
	/// </summary>
	public void ClearReserves()
	{
		foreach (N._0002 item in HC_000F)
		{
			item._71();
		}
	}

	/// <summary>
	/// Disposes any graphics resources used internally by this object, and clears
	/// all reserved shadow map sections. Commonly used during Game.UnloadContent.
	/// </summary>
	public void Unload()
	{
		foreach (N._0002 item in HC_000F)
		{
			item.Dispose();
		}
		HC_000F.Clear();
	}
}
