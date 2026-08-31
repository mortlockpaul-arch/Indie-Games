using System;
using System.Collections.Generic;
using H;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Audio;

/// <summary>
/// Manages all scene audio emitters and allows querying the scene with
/// a view or bounding box for audio emitters that affect the area
/// (acts as an audio emitters scenegraph).
/// </summary>
public class AudioManager : BaseObjectGraphManager<AudioSource, IAudioManager>, IAudioManager, IUpdatableManager, IManagerService, IQuery<AudioSource>, ISubmit<AudioSource>, ISubmit<IScene>, IWorldRenderableManager, IRenderableManager, IManager, IUnloadable
{
	private struct _0001CB
	{
		internal class _0001CB : IComparer<AudioManager._0001CB>
		{
			public int Compare(AudioManager._0001CB x, AudioManager._0001CB y)
			{
				return y.Weight.CompareTo(x.Weight);
			}
		}

		public AudioSource AudioSource;

		public float Weight;
	}

	private int HCB = 100;

	private int HC_0002 = 200;

	private ISceneState HC_0012;

	private Matrix HCH = Matrix.Identity;

	private BoundingFrustum HC7 = new BoundingFrustum(Matrix.Identity);

	private H.B HC_0001 = new H.B();

	private List<AudioSource> HCw = new List<AudioSource>();

	private List<_0001CB> HCZ = new List<_0001CB>();

	private static _0001CB._0001CB HC_000F = new _0001CB._0001CB();

	/// <summary>
	/// Determines which type this manager is registered under in the
	/// SceneInterface that contains it.
	///
	/// Please note: changing the return value to the ManagerType of
	/// another class will allow this manager to replace it in the
	/// SceneInterface (and provide replacement features and implementation).
	/// </summary>
	public override Type ManagerType => SceneInterface.AudioManagerType;

	/// <summary>
	/// Sets the order this manager is processed relative to other managers
	/// in the SceneInterface. Managers with lower processing order
	/// values are processed first.
	///
	/// In the case of BeginFrameRendering and EndFrameRendering, BeginFrameRendering
	/// is processed in the normal order (lowest value to highest), however
	/// EndFrameRendering is processed in reverse order (highest to lowest) to ensure
	/// the first manager begun is the last one ended (FILO).
	///
	/// For managers that do not require a specific order a value of 100 is recommended.
	/// </summary>
	public override int ManagerProcessOrder
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Maximum number of audio sources that can be played simultaneously.
	/// The number is limited on Xbox to 300, and on WP7 to 64.
	/// </summary>
	public int MaximumAudioSources
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
			if (HC_0002 > 300)
			{
				HC_0002 = 300;
			}
		}
	}

	/// <summary>
	/// Creates a new AudioManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public AudioManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
	}

	/// <summary>
	/// Called during Game.Update() to allow processing at regular intervals.
	/// </summary>
	/// <param name="gametime"></param>
	public override void Update(GameTime gametime)
	{
		HCw.Clear();
		HCZ.Clear();
		Find(HCw, HC7, ObjectFilter.All);
		foreach (AudioSource item2 in HCw)
		{
			bool flag = item2.AudioType == AudioType.Point;
			float radius = item2.Radius;
			float num = item2.Volume;
			if (item2.AudioState == AudioState.Playing && item2.SoundEffect != null && !(num <= 0f) && (!flag || !(radius <= 0f)))
			{
				if (flag)
				{
					float num2 = Vector3.DistanceSquared(item2.Position, HCH.Translation);
					num *= 1f - num2 / (radius * radius);
				}
				if (!(num <= 0f))
				{
					_0001CB item = new _0001CB
					{
						AudioSource = item2,
						Weight = num
					};
					HCZ.Add(item);
				}
			}
		}
		if (HCZ.Count > HC_0002)
		{
			HCZ.Sort(HC_000F);
		}
		HC_0001.HCB = HC_0002;
		HC_0001.q(ref HCH);
		int num3 = Math.Min(HCZ.Count, HC_0002);
		for (int i = 0; i < num3; i++)
		{
			HC_0001.R(HCZ[i].AudioSource);
		}
		HC_0001.F();
		base.Update(gametime);
	}

	/// <summary>
	/// Called when the game begins rendering the current frame.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		HC_0012 = scenestate;
		HCH = scenestate.ViewToWorld;
		HC7.Matrix = scenestate.ViewFrustum.Matrix;
	}

	/// <summary>
	/// Called when the game finishes rendering the current frame.
	/// </summary>
	public void EndFrameRendering()
	{
		foreach (AudioSource item in HCw)
		{
			item.RenderCustomPass(HC_0012);
		}
	}

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="obj"></param>
	public override void Remove(AudioSource obj)
	{
		HC_0001.N(obj);
		base.Remove(obj);
	}

	/// <summary>
	/// Called when the game clears the engine of objects (generally when
	/// clearing the current level / scene and before loading the next one).
	/// </summary>
	public override void Clear()
	{
		HC_0001.u();
		base.Clear();
	}
}
