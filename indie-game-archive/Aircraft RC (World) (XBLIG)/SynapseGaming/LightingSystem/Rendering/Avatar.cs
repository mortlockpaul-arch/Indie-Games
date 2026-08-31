using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using Z;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Avatar implementation that provides properties necessary for avatar
/// rendering.
/// </summary>
public class Avatar : IAvatar, ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject
{
	private const int HCB = 71;

	private UpdateDelegate HC_0002;

	private SubmitRemoveManagerDelegate HC_0012;

	private SubmitRemoveManagerDelegate HCH;

	private TypeDictionary<IManagerService> HC7 = new TypeDictionary<IManagerService>();

	private int HC_0001;

	private string HCw = "";

	private UpdateType HCZ;

	private Matrix HC_000F;

	private BoundingSphere HCy;

	private BoundingBox HC6;

	private BoundingBox HCD;

	private BoundingBox HC_0011;

	private BoundingSphere HCK;

	private BoundingBox HC_0003;

	private IList<Matrix> HCk;

	private AvatarExpression HCs;

	private AvatarRenderer HC_0013;

	private AvatarDescription HCX;

	private ObjectVisibility HCz = ObjectVisibility.RenderedAndCastShadows;

	private static List<Matrix> HCA;

	private ComponentCollection<ISceneEntity> HCc;

	[CompilerGenerated]
	private bool HCY;

	/// <summary>
	/// Dictionary of all managers the object is currently contained in (submitted to).
	///
	/// Managers are accessible by their ManagerType and only one manager of a
	/// particular type can be contained in the dictionary at a time.
	/// </summary>
	public TypeDictionary<IManagerService> ContainingManagers => HC7;

	/// <summary>
	/// Unique id used to identify the object across multiple scene loads / reloads.
	/// </summary>
	public int UniqueId => 0;

	/// <summary>
	/// Indicates the object bounding area spans the entire world and
	/// the object is always visible.
	/// </summary>
	public bool InfiniteBounds => false;

	/// <summary>
	/// Indicates the current move. This value increments each time the object
	/// is moved (when the World transform changes).
	/// </summary>
	public int MoveId => HC_0001;

	/// <summary>
	/// The object's current name.
	///
	/// Important note: Name can be changed at any time, HOWEVER managers
	/// will only see the change after removing and resubmitting the object.
	/// </summary>
	public string Name
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
		}
	}

	/// <summary>
	/// Determines if objects receive update events from the engine and are tracked
	/// by the scenegraph.
	///
	/// Automatic update events are necessary to be affected by gravity, for
	/// components, and for the scenegraph to track moving objects.  Objects without
	/// Automatic update events can still move, however the containing scenegraph
	/// (ObjectManager or LightManager) must be notified using Manager.Move(object).
	/// </summary>
	public UpdateType UpdateType
	{
		get
		{
			return HCZ;
		}
		set
		{
			HCZ = value;
		}
	}

	/// <summary>
	/// World space transform of the object.
	/// </summary>
	public Matrix World
	{
		get
		{
			return HC_000F;
		}
		set
		{
			if (!HC_000F.Equals(value))
			{
				HC_000F = value;
				HC_0001++;
				Hx();
			}
		}
	}

	/// <summary>
	/// Array of bone transforms for the skeleton's current pose. The matrix index is the
	/// same as the bone order used by the avatar.
	/// </summary>
	public IList<Matrix> SkinBones
	{
		get
		{
			return HCk;
		}
		set
		{
			HCk = value;
		}
	}

	/// <summary>
	/// The current avatar facial expression.
	/// </summary>
	public AvatarExpression Expression
	{
		get
		{
			return HCs;
		}
		set
		{
			HCs = value;
		}
	}

	/// <summary>
	/// Defines how the avatar is rendered.
	///
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders the avatar and casts shadows from it).
	/// </summary>
	public ObjectVisibility Visibility
	{
		get
		{
			return HCz;
		}
		set
		{
			HCz = value;
		}
	}

	/// <summary>
	/// Determines the bounds used in object culling and collision.
	/// </summary>
	public HullType HullType
	{
		get
		{
			return HullType.Box;
		}
		set
		{
		}
	}

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	public BoundingSphere ObjectBoundingSphere => HCy;

	/// <summary>
	/// Object space bounding area of the object.
	/// </summary>
	public BoundingBox ObjectBoundingBox => HC6;

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	public BoundingBox WorldBoundingBox => HC_0011;

	/// <summary>
	/// World space bounding area of the object.
	/// </summary>
	public BoundingSphere WorldBoundingSphere => HCK;

	/// <summary>
	/// Extended world space bounding area of the object. This area is roughly twice the size
	/// to accommodate avatar animations that fall outside the normal bounds.
	/// </summary>
	public BoundingBox WorldBoundingBoxProxy => HC_0003;

	/// <summary>
	/// AvatarRenderer used to render the avatar.
	/// </summary>
	public AvatarRenderer Renderer => HC_0013;

	/// <summary>
	/// Description of the avatar size, clothing, features, and more.
	/// </summary>
	public AvatarDescription Description => HCX;

	/// <summary>
	/// Determines if the avatar casts shadows base on the current ObjectVisibility options.
	/// </summary>
	public bool CastShadows => (HCz & ObjectVisibility.CastShadows) != 0;

	/// <summary>
	/// Determines if the avatar is visible base on the current ObjectVisibility options.
	/// </summary>
	public bool Visible => (HCz & ObjectVisibility.Rendered) != 0;

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return HCY;
		}
		[CompilerGenerated]
		set
		{
			HCY = value;
		}
	}

	/// <summary>
	/// Container that stores, manages, and updates the object's components.
	/// </summary>
	public ComponentCollection<ISceneEntity> Components => HCc;

	/// <summary>
	/// Event used to update the object at regular intervals. This and all
	/// events are only called on dynamic objects.
	/// </summary>
	public event UpdateDelegate UpdateEvent
	{
		add
		{
			UpdateDelegate updateDelegate = HC_0002;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Combine(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref HC_0002, value2, updateDelegate2);
			}
			while ((object)updateDelegate != updateDelegate2);
		}
		remove
		{
			UpdateDelegate updateDelegate = HC_0002;
			UpdateDelegate updateDelegate2;
			do
			{
				updateDelegate2 = updateDelegate;
				UpdateDelegate value2 = (UpdateDelegate)Delegate.Remove(updateDelegate2, value);
				updateDelegate = Interlocked.CompareExchange(ref HC_0002, value2, updateDelegate2);
			}
			while ((object)updateDelegate != updateDelegate2);
		}
	}

	/// <summary>
	/// Event used to determine when the object is submitted to a manager.
	/// </summary>
	public event SubmitRemoveManagerDelegate SubmittedToManagerEvent
	{
		add
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HC_0012;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HC_0012, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HC_0012;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HC_0012, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
	}

	/// <summary>
	/// Event used to determine when the object is removed from a manager.
	/// </summary>
	public event SubmitRemoveManagerDelegate RemovedFromManagerEvent
	{
		add
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HCH;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Combine(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HCH, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
		remove
		{
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate = HCH;
			SubmitRemoveManagerDelegate submitRemoveManagerDelegate2;
			do
			{
				submitRemoveManagerDelegate2 = submitRemoveManagerDelegate;
				SubmitRemoveManagerDelegate value2 = (SubmitRemoveManagerDelegate)Delegate.Remove(submitRemoveManagerDelegate2, value);
				submitRemoveManagerDelegate = Interlocked.CompareExchange(ref HCH, value2, submitRemoveManagerDelegate2);
			}
			while ((object)submitRemoveManagerDelegate != submitRemoveManagerDelegate2);
		}
	}

	static Avatar()
	{
		HCA = new List<Matrix>();
		for (int i = 0; i < 71; i++)
		{
			HCA.Add(Matrix.Identity);
		}
	}

	/// <summary>
	/// Creates a new Avatar instance.
	/// </summary>
	/// <param name="avatarrenderer">AvatarRenderer used to render the avatar.</param>
	/// <param name="description">Description of the avatar size, clothing, features, and more.</param>
	public Avatar(AvatarRenderer avatarrenderer, AvatarDescription description)
	{
		HCc = new ComponentCollection<ISceneEntity>(this);
		HCk = HCA;
		HC_000F = Matrix.Identity;
		SetRendererAndDescription(avatarrenderer, description);
		Hx();
	}

	/// <summary>
	/// Changes both the renderer and description used by the avatar.
	/// </summary>
	/// <param name="avatarrenderer">AvatarRenderer used to render the avatar.</param>
	/// <param name="description">Description of the avatar size, clothing, features, and more.</param>
	public void SetRendererAndDescription(AvatarRenderer avatarrenderer, AvatarDescription description)
	{
		HC_0013 = avatarrenderer;
		HCX = description;
		HC6 = new BoundingBox(new Vector3(-0.5f, 0f, -0.5f), new Vector3(0.5f, HCX.Height, 0.5f));
		HCy = BoundingSphere.CreateFromBoundingBox(HC6);
		HCD = CoreHelper.TransformBoundingBox(HC6, Matrix.CreateScale(2f));
		Hx();
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public virtual ISceneEntity Clone()
	{
		Avatar avatar = new Avatar(HC_0013, HCX);
		Z._7._0002w(this, avatar);
		foreach (IComponent<ISceneEntity> component in HCc.Components)
		{
			avatar.Components.Add(component.Clone());
		}
		return avatar;
	}

	/// <summary>
	/// Updates the object using the provided game time.
	/// </summary>
	/// <param name="gametime"></param>
	public virtual void Update(GameTime gametime)
	{
		HCc.OnUpdate(gametime);
		if (HC_0002 != null)
		{
			HC_0002(this, gametime);
		}
	}

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnSubmittedToManager(IManagerService manager)
	{
		HC7.Add(manager.ManagerType, manager);
		HCc.OnSubmittedToManager(manager);
		if (HC_0012 != null)
		{
			HC_0012(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public virtual void OnRemovedFromManager(IManagerService manager)
	{
		HCc.OnRemovedFromManager(manager);
		if (HCH != null)
		{
			HCH(manager);
		}
		HC7.Remove(manager.ManagerType);
	}

	/// <summary>
	/// Called when the object is created in the SunBurn editor.
	/// </summary>
	public virtual void OnCreatedInEditor()
	{
	}

	/// <summary>
	/// Sets both the avatar bone transforms and expression using an AvatarAnimation object.
	/// </summary>
	/// <param name="animation"></param>
	public void ApplyAnimation(IAvatarAnimation animation)
	{
		HCk = animation.BoneTransforms;
		HCs = animation.Expression;
	}

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	/// </summary>
	/// <param name="world">World space transform of the object.</param>
	/// <param name="worldtoobj">Inverse world space transform of the object.</param>
	public void SetWorldAndWorldToObject(Matrix world, Matrix worldtoobj)
	{
		World = world;
	}

	private void Hx()
	{
		HC_0011 = CoreHelper.TransformBoundingBox(HC6, HC_000F);
		HCK = BoundingSphere.CreateFromBoundingBox(HC_0011);
		HC_0003 = CoreHelper.TransformBoundingBox(HCD, HC_000F);
	}

	/// <summary>
	/// Implements a custom rendering pass. The pass occurs after scene rendering completes, but before post processing.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	public virtual void RenderCustomPass(ISceneState scenestate)
	{
	}

	/// <summary>
	/// Implements rendering of in-editor icons and helpers.
	///
	/// This method is called twice per-frame: once with scene depth clipping enable, and once with it disabled.
	/// </summary>
	/// <param name="scenestate">Current state used to render the scene.</param>
	/// <param name="renderhelper">Helper used to draw lines associated with the object. Only calling Submit() is
	/// supported in this method, using other methods may affect rendering of lines drawn by other objects.</param>
	/// <param name="highlighted">Indicates if the object is currently highlighted by the editor.</param>
	/// <param name="selected">Indicates if the object is currently selected by the editor.</param>
	/// <param name="sceneoccludedpass">Indicates if the current rendering pass depth clips with the scene.
	/// If so rendered icons and helpers are occluded by scene objects.</param>
	public virtual void RenderEditorIcon(ISceneState scenestate, BoundingBoxRenderHelper renderhelper, bool highlighted, bool selected, bool sceneoccludedpass)
	{
	}
}
