using System;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SynapseGaming.LightingSystem.Core;
using l;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Adds editor support to SunBurn projects.
/// </summary>
public class SunBurnEditor : IManagerService, IRenderableManager, IUpdatableManager, IManager, IUnloadable
{
	internal delegate void DA_0018(IDisposable resource);

	internal delegate void DAL(object obj, l._0019 updatetype);

	/// <summary>
	/// Used to remap effects that are replaced in editor.
	/// </summary>
	/// <param name="currenteffect">The effect to replace.</param>
	/// <param name="neweffect">The new effect.</param>
	public delegate void EffectReplaceDelegate(Effect currenteffect, Effect neweffect);

	/// <summary>
	/// Used to reload all scene assets when requested by the editor.
	/// </summary>
	public delegate void ReloadAssetsDelegate();

	private IManagerServiceProvider _3A_0018;

	private int _3AL = 25;

	private Keys _3A_0019;

	private F.L _3A3;

	private F._0018 _3A6;

	private static DA_0018 _3AD;

	private static DA_0018 _3A_0017;

	private static DAL _3A_0003;

	/// <summary>
	/// Used to remap effects that are replaced in editor.
	/// </summary>
	public static EffectReplaceDelegate ReplaceEffect;

	/// <summary>
	/// Used to reload all scene assets when requested by the editor.
	/// </summary>
	public static ReloadAssetsDelegate ReloadAssets;

	internal static bool EditorAttachedStatic => false;

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public Type ManagerType => SceneInterface.EditorType;

	/// <summary>
	/// Sets the order this manager is processed relative to other managers
	/// in the IManagerServiceProvider. Managers with lower processing order
	/// values are processed first.
	///
	/// In the case of BeginFrameRendering and EndFrameRendering, BeginFrameRendering
	/// is processed in the normal order (lowest order value to highest), however
	/// EndFrameRendering is processed in reverse order (highest to lowest) to ensure
	/// the first manager begun is the last one ended (FILO).
	/// </summary>
	public int ManagerProcessOrder
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => _3A_0018;

	/// <summary>
	/// The assigned key that, when pressed, will be used to launch the in-game editor.
	/// </summary>
	public Keys LaunchKey
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
		}
	}

	internal float EditorIconSize
	{
		get
		{
			return _3A6.IconScale;
		}
		set
		{
			_3A6.IconScale = iconScale;
			_3A3.IconScale = iconScale;
		}
	}

	/// <summary>
	/// Determines if user defined code handles in editor camera movement.
	/// If so only object selection and object movement is processed.
	/// </summary>
	public bool UserHandledView
	{
		get
		{
			return _3A6.UserHandledView;
		}
		set
		{
			_3A6.UserHandledView = value;
		}
	}

	/// <summary>
	/// Allows specific processing when the editor attached. Commonly used for editor specific input processing.
	/// </summary>
	public bool EditorAttached => false;

	/// <summary>
	/// Allows specific processing when the game window has input focus, not the editor's controls.
	/// </summary>
	public bool GameHasFocus => true;

	/// <summary>
	/// Creates a LightingSystemEditor instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public SunBurnEditor(IManagerServiceProvider sceneinterface)
	{
		_3A_0018 = sceneinterface;
		_3A3 = new F.L();
		_3A6 = new F._0018();
	}

	/// <summary />
	~SunBurnEditor()
	{
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public virtual void ApplyPreferences(ISystemPreferences preferences)
	{
	}

	/// <summary>
	/// Processes in editor input control, object selection, and camera movement.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
	}

	/// <summary>
	/// Sets up the object prior to rendering.
	/// </summary>
	/// <param name="scenestate"></param>
	public virtual void BeginFrameRendering(ISceneState scenestate)
	{
	}

	/// <summary>
	/// Finalizes rendering.
	/// </summary>
	public virtual void EndFrameRendering()
	{
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public virtual void Unload()
	{
	}

	/// <summary>
	/// Removes resources managed by this object. Commonly used while clearing the scene.
	/// </summary>
	public virtual void Clear()
	{
	}

	/// <summary>
	/// Opens the SunBurn editor manually.
	/// </summary>
	public virtual void LaunchEditor()
	{
	}

	private void _0019F()
	{
	}

	/// <summary>
	/// Called when the editor is closed.
	/// </summary>
	protected internal virtual void CloseEditor()
	{
	}

	internal static void _0019c(DA_0018 P_0)
	{
		_3AD = (DA_0018)Delegate.Combine(_3AD, P_0);
	}

	internal static void _0019g(DA_0018 P_0)
	{
		_3A_0017 = (DA_0018)Delegate.Combine(_3A_0017, P_0);
	}

	internal static void _0019I(DAL P_0)
	{
		_3A_0003 = (DAL)Delegate.Combine(_3A_0003, P_0);
	}

	internal static void _00198(DA_0018 P_0)
	{
		_3AD = (DA_0018)Delegate.Remove(_3AD, P_0);
	}

	internal static void _0019Z(DA_0018 P_0)
	{
		_3A_0017 = (DA_0018)Delegate.Remove(_3A_0017, P_0);
	}

	internal static void _0019x(DAL P_0)
	{
		_3A_0003 = (DAL)Delegate.Remove(_3A_0003, P_0);
	}

	/// <summary>
	/// Register delegate used to reload scene assets when requested by the editor.
	/// </summary>
	/// <param name="del"></param>
	public static void RegisterOnReplaceEffect(EffectReplaceDelegate del)
	{
	}

	/// <summary>
	/// Unregister delegate used to reload scene assets when requested by the editor.
	/// </summary>
	/// <param name="del"></param>
	public static void UnregisterOnReplaceEffect(EffectReplaceDelegate del)
	{
	}

	/// <summary>
	/// Call to start tracking user defined resources in the editor.
	/// </summary>
	/// <param name="resource"></param>
	public static void OnCreateResource(IDisposable resource)
	{
	}

	/// <summary>
	/// Call to stop tracking user defined resources in the editor.
	/// </summary>
	/// <param name="resource"></param>
	public static void OnDisposeResource(IDisposable resource)
	{
	}

	internal static void _0019q(object P_0, l._0019 P_1)
	{
	}
}
