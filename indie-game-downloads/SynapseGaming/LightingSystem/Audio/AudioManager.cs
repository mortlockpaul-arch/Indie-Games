using System;
using System.Collections.Generic;
using _3;
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
	private struct DA_0018
	{
		internal class DA_0018 : IComparer<AudioManager.DA_0018>
		{
			public int Compare(AudioManager.DA_0018 x, AudioManager.DA_0018 y)
			{
				return y.Weight.CompareTo(x.Weight);
			}
		}

		public AudioSource AudioSource;

		public float Weight;
	}

	private int _3A_0018 = 100;

	private int _3AL = 200;

	private ISceneState _3A_0019;

	private Matrix _3A3 = Matrix.Identity;

	private BoundingFrustum _3A6 = new BoundingFrustum(Matrix.Identity);

	private _3._0018 _3AD = new _3._0018();

	private List<AudioSource> _3A_0017 = new List<AudioSource>();

	private List<DA_0018> _3A_0003 = new List<DA_0018>();

	private static DA_0018.DA_0018 _3Al = new DA_0018.DA_0018();

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
			return _3A_0018;
		}
		set
		{
			_3A_0018 = value;
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
			return _3AL;
		}
		set
		{
			_3AL = value;
			if (_3AL > 300)
			{
				_3AL = 300;
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
		_3A_0017.Clear();
		_3A_0003.Clear();
		Find(_3A_0017, _3A6, ObjectFilter.All);
		foreach (AudioSource item2 in _3A_0017)
		{
			bool flag = item2.AudioType == AudioType.Point;
			float radius = item2.Radius;
			float num = item2.Volume;
			if (item2.AudioState == AudioState.Playing && item2.SoundEffect != null && !(num <= 0f) && (!flag || !(radius <= 0f)))
			{
				if (flag)
				{
					float num2 = Vector3.DistanceSquared(item2.Position, _3A3.Translation);
					num *= 1f - num2 / (radius * radius);
				}
				if (!(num <= 0f))
				{
					DA_0018 item = new DA_0018
					{
						AudioSource = item2,
						Weight = num
					};
					_3A_0003.Add(item);
				}
			}
		}
		if (_3A_0003.Count > _3AL)
		{
			_3A_0003.Sort(_3Al);
		}
		_3AD._3A_0018 = _3AL;
		_3AD._0010(ref _3A3);
		int num3 = Math.Min(_3A_0003.Count, _3AL);
		for (int i = 0; i < num3; i++)
		{
			_3AD._0016(_3A_0003[i].AudioSource);
		}
		_3AD.p();
		base.Update(gametime);
	}

	/// <summary>
	/// Called when the game begins rendering the current frame.
	/// </summary>
	/// <param name="scenestate"></param>
	public void BeginFrameRendering(ISceneState scenestate)
	{
		_3A_0019 = scenestate;
		_3A3 = scenestate.ViewToWorld;
		_3A6.Matrix = scenestate.ViewFrustum.Matrix;
	}

	/// <summary>
	/// Called when the game finishes rendering the current frame.
	/// </summary>
	public void EndFrameRendering()
	{
		foreach (AudioSource item in _3A_0017)
		{
			item.RenderCustomPass(_3A_0019);
		}
	}

	/// <summary>
	/// Removes an object from the container.
	/// </summary>
	/// <param name="obj"></param>
	public override void Remove(AudioSource obj)
	{
		_3AD.k(obj);
		base.Remove(obj);
	}

	/// <summary>
	/// Called when the game clears the engine of objects (generally when
	/// clearing the current level / scene and before loading the next one).
	/// </summary>
	public override void Clear()
	{
		_3AD.X();
		base.Clear();
	}
}
