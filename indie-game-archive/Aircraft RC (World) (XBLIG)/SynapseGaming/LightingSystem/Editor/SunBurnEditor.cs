using System;
using _000F;
using _6;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Editor;

/// <summary>
/// Adds editor support to SunBurn projects.
/// </summary>
public class SunBurnEditor : IManagerService, IRenderableManager, IUpdatableManager, IManager, IUnloadable
{
	internal delegate void _0001CB(IDisposable resource);

	internal delegate void _0001C_0002(object obj, _000F._0012 updatetype);

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

	private IManagerServiceProvider HCB;

	private int HC_0002 = 25;

	private Keys HC_0012;

	private _6._0002 HCH;

	private _6.B HC7;

	private static _0001CB HC_0001;

	private static _0001CB HCw;

	private static _0001C_0002 HCZ;

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
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
		}
	}

	/// <summary>
	/// Scene interface the manager was created by, or assigned during construction.
	/// </summary>
	public IManagerServiceProvider OwnerSceneInterface => HCB;

	/// <summary>
	/// The assigned key that, when pressed, will be used to launch the in-game editor.
	/// </summary>
	public Keys LaunchKey
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	internal float EditorIconSize
	{
		get
		{
			return HC7.IconScale;
		}
		set
		{
			HC7.IconScale = iconScale;
			HCH.IconScale = iconScale;
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
			return HC7.UserHandledView;
		}
		set
		{
			HC7.UserHandledView = value;
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
		HCB = sceneinterface;
		HCH = new _6._0002();
		HC7 = new _6.B();
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

	private void _0012D()
	{
	}

	/// <summary>
	/// Called when the editor is closed.
	/// </summary>
	protected internal virtual void CloseEditor()
	{
	}

	internal static void _0012_0011(_0001CB P_0)
	{
		HC_0001 = (_0001CB)Delegate.Combine(HC_0001, P_0);
	}

	internal static void _0012K(_0001CB P_0)
	{
		HCw = (_0001CB)Delegate.Combine(HCw, P_0);
	}

	internal static void _0012_0003(_0001C_0002 P_0)
	{
		HCZ = (_0001C_0002)Delegate.Combine(HCZ, P_0);
	}

	internal static void _0012k(_0001CB P_0)
	{
		HC_0001 = (_0001CB)Delegate.Remove(HC_0001, P_0);
	}

	internal static void _0012s(_0001CB P_0)
	{
		HCw = (_0001CB)Delegate.Remove(HCw, P_0);
	}

	internal static void _0012_0013(_0001C_0002 P_0)
	{
		HCZ = (_0001C_0002)Delegate.Remove(HCZ, P_0);
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

	internal static void _0012X(object P_0, _000F._0012 P_1)
	{
	}
}
