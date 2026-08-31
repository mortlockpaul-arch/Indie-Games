using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using B;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using R;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Components;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using SynapseGaming.LightingSystem.Serialization;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Scene object implementation that uses XNA Models, SunBurn MeshData,
/// and raw vertex / index buffers as a source.
/// </summary>
[Serializable]
[EditorCreatedObject]
public class SceneObject : SceneEntity, ISceneObject, ICollisionObject, ISceneEntity, IMovableObject, IWorldBoundingBoxObject, IComponentObject<ISceneEntity>, IEditorCreatedObject<ISceneEntity>, IEditorObject, INamedObject, IEditorRenderableObject
{
	private CollisionReactDelegate HCB;

	private CollisionTriggerDelegate HC_0002;

	private Matrix[] HC_0012;

	private bool HCH = true;

	private bool HC7 = true;

	private bool HC_0001 = true;

	private ObjectVisibility HCw = ObjectVisibility.RenderedAndCastShadows;

	private ModelAsset HCZ = ModelAsset.Empty;

	private bool HC_000F = true;

	private CollisionType HCy;

	private ICollisionMove HC6;

	private bool HCD;

	private ICollisionMaterial HC_0011;

	private float HCK = 1f;

	private StaticLightingType HC_0003;

	private LightMapSize HCk = LightMapSize.Size128x128;

	private Vector3 HCs;

	private bool HC_0013;

	private bool HCX = true;

	private string HCz = "";

	private RenderableMeshCollection HCA;

	private List<RenderableMesh> HCc = new List<RenderableMesh>(16);

	/// <summary>
	/// Provides direct access to the repository name, file name, and model
	/// the scene object was created from. Only valid for serialized scene objects
	/// created via the SunBurn editor.
	/// </summary>
	[EditorProperty(true, Description = "Model File", HorizontalAlignment = true, MajorGrouping = 1, MinorGrouping = 2, ToolTipText = "")]
	public ModelAsset ModelAsset
	{
		get
		{
			return HCZ;
		}
		set
		{
			if (value != null)
			{
				HCZ = value;
			}
			else
			{
				HCZ = ModelAsset.Empty;
			}
			_7X();
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId
	{
		get
		{
			return _CollisionId;
		}
		set
		{
			_CollisionId = value;
		}
	}

	/// <summary>
	/// Determines if gravity will cause the object to fall. For an object to be affected
	/// by gravity its UpdateType must be Automatic and CollisionType must be Collide.
	/// </summary>
	[EditorProperty(true, Description = "Affected By Gravity", MajorGrouping = 6, MinorGrouping = 1, ToolTipText = "")]
	public bool AffectedByGravity
	{
		get
		{
			return HC_000F;
		}
		set
		{
			HC_000F = value;
			_CollisionId++;
			if (!HC_000F && HC6 != null)
			{
				HC6.RemoveForces();
			}
		}
	}

	/// <summary>
	/// Determines how an object interacts with the scene.
	/// </summary>
	[EditorProperty(true, Description = "Collision Type", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 1, ToolTipText = "")]
	public CollisionType CollisionType
	{
		get
		{
			return HCy;
		}
		set
		{
			HCy = value;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Move helper used by this object to determine its momentum, next location, and sweep volume.
	/// </summary>
	public ICollisionMove CollisionMove
	{
		get
		{
			return HC6;
		}
		set
		{
			if (value == HC6)
			{
				return;
			}
			if (HC6 != null)
			{
				foreach (KeyValuePair<Type, IManagerService> item in base.ContainingManagers.Items)
				{
					HC6.OnRemovedFromManager(item.Value);
				}
			}
			HC6 = value;
			if (value == null)
			{
				return;
			}
			foreach (KeyValuePair<Type, IManagerService> item2 in base.ContainingManagers.Items)
			{
				value.OnSubmittedToManager(item2.Value);
			}
		}
	}

	/// <summary>
	/// Default material used when collision surface does not implement material info.
	/// </summary>
	public ICollisionMaterial DefaultCollisionMaterial
	{
		get
		{
			return HC_0011;
		}
		set
		{
			HC_0011 = value;
			HCD = true;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Mass of the object.
	/// </summary>
	[EditorNumberPadOptions(3, 0.001, 10000.0, 0.1)]
	[EditorProperty(true, Description = "Mass", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 2, ToolTipText = "")]
	public float Mass
	{
		get
		{
			return HCK;
		}
		set
		{
			HCK = value;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Determines if an object uses light mapping, approximate lighting, or no lighting
	/// to receive illumination from BakedDown light sources.
	/// </summary>
	[EditorProperty(true, Description = "Static Lighting Type", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 2, ToolTipText = "")]
	public StaticLightingType StaticLightingType
	{
		get
		{
			return HC_0003;
		}
		set
		{
			if (!HC_0013 && value == StaticLightingType.BakedDown)
			{
				HC_0003 = StaticLightingType.None;
				return;
			}
			if (HC_0003 != value)
			{
				HCX = true;
			}
			HC_0003 = value;
		}
	}

	/// <summary>
	/// Determines the light map size when generating baked down lighting on the object.
	/// </summary>
	public LightMapSize LightMapSize
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
	/// Specifies the lighting color used when the StaticLightingType is set to Custom.
	/// </summary>
	[EditorProperty(true, Description = "Custom Static Lighting", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 3, ToolTipText = "", ControlType = ControlType.ColorSelection)]
	public Vector3 CustomStaticLightingColor
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
	/// Indicates the object's meshes are capable of using light maps.
	/// </summary>
	[EditorProperty(false)]
	public bool CanLightMap
	{
		get
		{
			return HC_0013;
		}
		private set
		{
			HC_0013 = flag;
			if (!HC_0013 && HC_0003 == StaticLightingType.BakedDown)
			{
				StaticLightingType = StaticLightingType.None;
			}
		}
	}

	/// <summary>
	/// Indicates the object is rendering without errors.
	/// </summary>
	[EditorProperty(false)]
	public bool Valid
	{
		get
		{
			return HCX;
		}
		set
		{
			HCX = value;
		}
	}

	/// <summary>
	/// Contains any errors that occurred during rendering.
	/// </summary>
	[EditorTextBoxOptions(true, Width = 165)]
	[EditorProperty(true, Description = "Possible Errors", HorizontalAlignment = true, MajorGrouping = 5, MinorGrouping = 3, ToolTipText = "")]
	public string RenderingErrors
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
	/// Array of bone transforms used to form the skeleton's current pose. The array
	/// index of a bone matrix should match the vertex buffer bone index.
	/// </summary>
	public Matrix[] SkinBones
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

	/// <summary>
	/// Defines how the object is rendered.
	///
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders the object and casts shadows from it).
	/// </summary>
	[EditorProperty(true, Description = "Visibility", HorizontalAlignment = true, MajorGrouping = 4, MinorGrouping = 1, ToolTipText = "")]
	[EditorDropDownOptions(165)]
	public ObjectVisibility Visibility
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = value;
			HCH = (HCw & ObjectVisibility.CastShadows) != 0;
			HC7 = (HCw & ObjectVisibility.Rendered) != 0;
			HC_0001 = (HCw & ObjectVisibility.RenderedInEditor) != ObjectVisibility.None || (HCw & ObjectVisibility.Rendered) != 0;
		}
	}

	/// <summary>
	/// Determines if the object casts shadows based on the current ObjectVisibility options.
	/// </summary>
	public bool CastShadows => HCH;

	/// <summary>
	/// Determines if the object is visible based on the current ObjectVisibility options.
	/// </summary>
	public bool Visible => HC7;

	/// <summary>
	/// Determines if the object is visible in the editor based on the current ObjectVisibility options.
	/// </summary>
	public bool VisibleInEditor => HC_0001;

	/// <summary>
	/// Collection of the object's internal mesh parts.
	/// </summary>
	public RenderableMeshCollection RenderableMeshes => HCA;

	/// <summary>
	/// Event used to detect when the object collides with another object, or to
	/// override the default reaction behavior between objects.
	/// </summary>
	public event CollisionReactDelegate CollisionReactEvent
	{
		add
		{
			CollisionReactDelegate collisionReactDelegate = HCB;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Combine(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref HCB, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
		remove
		{
			CollisionReactDelegate collisionReactDelegate = HCB;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Remove(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref HCB, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
	}

	/// <summary>
	/// Event used to detect when another object collides with this object, but only
	/// when this object's CollisionType is set to Trigger.
	///
	/// The event handler can then apply custom trigger code like damage, apply force, and more.
	/// </summary>
	public event CollisionTriggerDelegate CollisionTriggerEvent
	{
		add
		{
			CollisionTriggerDelegate collisionTriggerDelegate = HC_0002;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Combine(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref HC_0002, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
		remove
		{
			CollisionTriggerDelegate collisionTriggerDelegate = HC_0002;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Remove(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref HC_0002, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
	}

	/// <summary>
	/// Default constructor for derived classes that implement their own mesh creation.
	/// </summary>
	public SceneObject()
		: base("", infinitebounds: false)
	{
	}

	/// <summary>
	/// Creates a new SceneObject instance.
	/// </summary>
	/// <param name="name">Custom name for the object.</param>
	/// <param name="infinitebounds">Indicates the object bounding area spans the entire world and
	/// the object is always visible.</param>
	public SceneObject(string name, bool infinitebounds)
		: base(name, infinitebounds)
	{
	}

	/// <summary>
	/// Creates a new SceneObject from mesh data.
	/// </summary>
	/// <param name="meshdata"></param>
	public SceneObject(MeshData meshdata)
		: this(meshdata, "")
	{
	}

	/// <summary>
	/// Creates a new SceneObject from mesh data.
	/// </summary>
	/// <param name="meshdata"></param>
	/// <param name="name">Custom name for the object.</param>
	public SceneObject(MeshData meshdata, string name)
		: base(name, meshdata.InfiniteBounds)
	{
		RenderableMesh renderableMesh = new RenderableMesh();
		renderableMesh.Build(this, meshdata.Effect, meshdata.MeshToObject, meshdata.ObjectSpaceBoundingSphere, meshdata.ObjectSpaceBoundingBox, meshdata.IndexBuffer, meshdata.VertexBuffer, 0, PrimitiveType.TriangleList, meshdata.PrimitiveCount, 0, meshdata.VertexCount, 0, detectskinningandlightmapping: true);
		Add(renderableMesh);
	}

	/// <summary>
	/// Creates a new SceneObject from a user defined vertex buffer.
	/// </summary>
	/// <param name="effect">Effect applied to the mesh during rendering.</param>
	/// <param name="meshboundingsphere">Smallest mesh space bounding sphere that
	/// completely encloses the object.</param>
	/// <param name="meshboundingbox">Smallest mesh space bounding box that
	/// completely encloses the object.</param>
	/// <param name="vertexbuffer">VertexBuffer that contains the mesh geometry.</param>
	/// <param name="vertexstart">Index into the vertex buffer that mesh geometry begins.</param>
	/// <param name="primitivetype">Primitive format the mesh geometry is stored in.</param>
	/// <param name="primitivecount">Number of primitives in the mesh geometry.</param>
	/// <param name="vertexstreamoffset">Offset in bytes from the beginning of the vertex
	/// buffer to start reading data.</param>
	/// <param name="objectspace">Mesh object-space matrix.</param>
	public SceneObject(Effect effect, BoundingSphere meshboundingsphere, BoundingBox meshboundingbox, Matrix objectspace, VertexBuffer vertexbuffer, PrimitiveType primitivetype, int primitivecount, int vertexstart, int vertexstreamoffset)
		: this("", infinitebounds: false, effect, meshboundingsphere, meshboundingbox, objectspace, null, vertexbuffer, 0, primitivetype, primitivecount, 0, 0, vertexstreamoffset)
	{
	}

	/// <summary>
	/// Creates a new SceneObject from a user defined vertex buffer.
	/// </summary>
	/// <param name="name">Custom name for the object.</param>
	/// <param name="infinitebounds">Determines if the object spans an infinite bounding volume.</param>
	/// <param name="effect">Effect applied to the mesh during rendering.</param>
	/// <param name="meshboundingsphere">Smallest mesh space bounding sphere that
	/// completely encloses the object.</param>
	/// <param name="meshboundingbox">Smallest mesh space bounding box that
	/// completely encloses the object.</param>
	/// <param name="vertexbuffer">VertexBuffer that contains the mesh geometry.</param>
	/// <param name="vertexstart">Index into the vertex buffer that mesh geometry begins.</param>
	/// <param name="primitivetype">Primitive format the mesh geometry is stored in.</param>
	/// <param name="primitivecount">Number of primitives in the mesh geometry.</param>
	/// <param name="vertexstreamoffset">Offset in bytes from the beginning of the vertex
	/// buffer to start reading data.</param>
	/// <param name="objectspace">Mesh object-space matrix.</param>
	public SceneObject(string name, bool infinitebounds, Effect effect, BoundingSphere meshboundingsphere, BoundingBox meshboundingbox, Matrix objectspace, VertexBuffer vertexbuffer, PrimitiveType primitivetype, int primitivecount, int vertexstart, int vertexstreamoffset)
		: this(name, infinitebounds, effect, meshboundingsphere, meshboundingbox, objectspace, null, vertexbuffer, 0, primitivetype, primitivecount, 0, 0, vertexstreamoffset)
	{
	}

	/// <summary>
	/// Creates a new SceneObject from a user defined vertex and index buffer.
	/// </summary>
	/// <param name="effect">Effect applied to the mesh during rendering.</param>
	/// <param name="meshboundingsphere">Smallest mesh space bounding sphere that
	/// completely encloses the object.</param>
	/// <param name="meshboundingbox">Smallest mesh space bounding box that
	/// completely encloses the object.</param>
	/// <param name="indexbuffer">IndexBuffer that contains the mesh geometry.</param>
	/// <param name="vertexbuffer">VertexBuffer that contains the mesh geometry.</param>
	/// <param name="indexstart">Index into the index buffer that mesh geometry begins.</param>
	/// <param name="primitivetype">Primitive format the mesh geometry is stored in.</param>
	/// <param name="primitivecount">Number of primitives in the mesh geometry.</param>
	/// <param name="vertexbase">Offset added to each index in the index buffer during rendering.</param>
	/// <param name="vertexcount">Number of vertices in the vertex buffer range required to
	/// draw the mesh.  For instance, a quad rendering vertices at indices (2, 5, 6, 9) requires
	/// a vertex buffer range of 8 vertices (vertices 2 – 9 inclusive).</param>
	/// <param name="vertexstreamoffset">Offset in bytes from the beginning of the vertex
	/// buffer to start reading data.</param>
	/// <param name="objectspace">Mesh object-space matrix.</param>
	public SceneObject(Effect effect, BoundingSphere meshboundingsphere, BoundingBox meshboundingbox, Matrix objectspace, IndexBuffer indexbuffer, VertexBuffer vertexbuffer, int indexstart, PrimitiveType primitivetype, int primitivecount, int vertexbase, int vertexcount, int vertexstreamoffset)
		: this("", infinitebounds: false, effect, meshboundingsphere, meshboundingbox, objectspace, indexbuffer, vertexbuffer, indexstart, primitivetype, primitivecount, vertexbase, vertexcount, vertexstreamoffset)
	{
	}

	/// <summary>
	/// Creates a new SceneObject from a user defined vertex and index buffer.
	/// </summary>
	/// <param name="name">Custom name for the object.</param>
	/// <param name="infinitebounds">Determines if the object spans an infinite bounding volume.</param>
	/// <param name="effect">Effect applied to the mesh during rendering.</param>
	/// <param name="meshboundingsphere">Smallest mesh space bounding sphere that
	/// completely encloses the object.</param>
	/// <param name="meshboundingbox">Smallest mesh space bounding box that
	/// completely encloses the object.</param>
	/// <param name="indexbuffer">IndexBuffer that contains the mesh geometry.</param>
	/// <param name="vertexbuffer">VertexBuffer that contains the mesh geometry.</param>
	/// <param name="indexstart">Index into the index buffer that mesh geometry begins.</param>
	/// <param name="primitivetype">Primitive format the mesh geometry is stored in.</param>
	/// <param name="primitivecount">Number of primitives in the mesh geometry.</param>
	/// <param name="vertexbase">Offset added to each index in the index buffer during rendering.</param>
	/// <param name="vertexcount">Number of vertices in the vertex buffer range required to
	/// draw the mesh.  For instance, a quad rendering vertices at indices (2, 5, 6, 9) requires
	/// a vertex buffer range of 8 vertices (vertices 2 – 9 inclusive).</param>
	/// <param name="vertexstreamoffset">Offset in bytes from the beginning of the vertex
	/// buffer to start reading data.</param>
	/// <param name="objectspace">Mesh object-space matrix.</param>
	public SceneObject(string name, bool infinitebounds, Effect effect, BoundingSphere meshboundingsphere, BoundingBox meshboundingbox, Matrix objectspace, IndexBuffer indexbuffer, VertexBuffer vertexbuffer, int indexstart, PrimitiveType primitivetype, int primitivecount, int vertexbase, int vertexcount, int vertexstreamoffset)
		: base(name, infinitebounds)
	{
		RenderableMesh renderableMesh = new RenderableMesh();
		renderableMesh.Build(this, effect, objectspace, meshboundingsphere, meshboundingbox, indexbuffer, vertexbuffer, indexstart, primitivetype, primitivecount, vertexbase, vertexcount, vertexstreamoffset, detectskinningandlightmapping: true);
		Add(renderableMesh);
	}

	/// <summary>
	/// Creates a new SceneObject constructing RenderableMeshes
	/// from all ModelMeshes within the provided Model.
	/// </summary>
	/// <param name="model"></param>
	public SceneObject(Model model)
		: this(model, model.Root.Name)
	{
	}

	/// <summary>
	/// Creates a new SceneObject constructing RenderableMeshes
	/// from the provided ModelMesh.
	/// </summary>
	/// <param name="mesh"></param>
	public SceneObject(ModelMesh mesh)
		: this(mesh, mesh.ParentBone.Name)
	{
	}

	/// <summary>
	/// Creates a new SceneObject constructing RenderableMeshes
	/// from all ModelMeshes within the provided Model.
	/// </summary>
	/// <param name="model"></param>
	/// <param name="name">Custom name for the object.</param>
	public SceneObject(Model model, string name)
		: base(name, infinitebounds: false)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			AddModelMesh(model.Meshes[i], null);
		}
	}

	/// <summary>
	/// Creates a new SceneObject constructing RenderableMeshes
	/// from the provided ModelMesh.
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="name">Custom name for the object.</param>
	public SceneObject(ModelMesh mesh, string name)
		: base(name, infinitebounds: false)
	{
		AddModelMesh(mesh, null);
	}

	/// <summary>
	/// Creates a new SceneObject constructing RenderableMeshes
	/// from all ModelMeshes within the provided Model.
	/// </summary>
	/// <param name="model"></param>
	/// <param name="overrideeffect">User defined effect used to render the object.</param>
	/// <param name="name">Custom name for the object.</param>
	public SceneObject(Model model, Effect overrideeffect, string name)
		: base(name, infinitebounds: false)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			AddModelMesh(model.Meshes[i], overrideeffect);
		}
	}

	/// <summary>
	/// Creates a new SceneObject constructing RenderableMeshes
	/// from the provided ModelMesh.
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="overrideeffect">User defined effect used to render the object.</param>
	/// <param name="name">Custom name for the object.</param>
	public SceneObject(ModelMesh mesh, Effect overrideeffect, string name)
		: base(name, infinitebounds: false)
	{
		AddModelMesh(mesh, overrideeffect);
	}

	/// <summary>
	/// Initializes the object to default values.
	/// </summary>
	/// <param name="name">Custom name for the object.</param>
	/// <param name="infinitebounds">Indicates the object bounding area spans the entire world and
	/// the object is always visible.</param>
	protected override void Init(string name, bool infinitebounds)
	{
		HCA = new RenderableMeshCollection(HCc);
		base.Init(name, infinitebounds);
	}

	/// <summary>
	/// Deep clones the object including any contained sub-objects and components.
	/// </summary>
	/// <returns></returns>
	public override ISceneEntity Clone()
	{
		ISceneEntity sceneEntity = base.Clone();
		ISceneObject sceneObject = sceneEntity as ISceneObject;
		if (HCZ != null && HCZ != ModelAsset.Empty && HCZ.Asset != null)
		{
			sceneObject.ModelAsset = HCZ;
		}
		else if (sceneObject is SceneObject sceneObject2)
		{
			for (int i = 0; i < HCA.Count; i++)
			{
				sceneObject2.Add(HCA[i].Clone());
			}
		}
		sceneObject.StaticLightingType = StaticLightingType;
		return sceneEntity;
	}

	/// <summary>
	/// Called when the object is submitted to a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnSubmittedToManager(IManagerService manager)
	{
		base.OnSubmittedToManager(manager);
		if (HC6 != null)
		{
			HC6.OnSubmittedToManager(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnRemovedFromManager(IManagerService manager)
	{
		base.OnRemovedFromManager(manager);
		if (HC6 != null)
		{
			HC6.OnRemovedFromManager(manager);
		}
	}

	/// <summary>
	/// Used to trigger the CollisionReactEvent event when two objects collide.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="collidee">The object hit by the moving object.</param>
	/// <param name="worldcollisionpoint">Contains information about the closest collision point to the collider.</param>
	/// <param name="collisionhandled">Determines if the collision was handled by a prior event hander.
	/// If this value is true do NOT process any collision reaction code. If the event handler processes
	/// collision reaction code set this value to true to avoid another handler or SunBurn's built-in
	/// reaction code from processing.</param>
	public virtual void OnCollisionReact(IMovableObject collider, IMovableObject collidee, CollisionPoint worldcollisionpoint, ref bool collisionhandled)
	{
		_Components.OnCollisionReact(collider, collidee, worldcollisionpoint, ref collisionhandled);
		if (HCB != null)
		{
			HCB(collider, collidee, worldcollisionpoint, ref collisionhandled);
		}
	}

	/// <summary>
	/// Used to trigger the CollisionTriggerEvent event when an object passes through or overlaps a trigger.
	/// </summary>
	/// <param name="collider">The moving object.</param>
	/// <param name="trigger">The trigger hit by the moving object.</param>
	public virtual void OnCollisionTrigger(IMovableObject collider, IMovableObject trigger)
	{
		_Components.OnCollisionTrigger(collider, trigger);
		if (HC_0002 != null)
		{
			HC_0002(collider, trigger);
		}
	}

	private void _7X()
	{
		Clear();
		Model asset = HCZ.Asset;
		if (asset == null)
		{
			return;
		}
		string modelMeshName = HCZ.ModelMeshName;
		bool flag = !string.IsNullOrEmpty(modelMeshName);
		for (int i = 0; i < asset.Meshes.Count; i++)
		{
			ModelMesh modelMesh = asset.Meshes[i];
			if (!flag || (!string.IsNullOrEmpty(modelMesh.Name) && !(modelMesh.Name != modelMeshName)))
			{
				AddModelMesh(modelMesh, null);
				if (flag)
				{
					break;
				}
			}
		}
		ContentRepository contentRepository = ContentRepository.Find(HCZ.ContentRepositoryName);
		if (contentRepository != null)
		{
			for (int j = 0; j < HCA.Count; j++)
			{
				contentRepository.LoadLightMap(HCA[j]);
			}
		}
		Matrix world = base.World;
		Matrix worldtoobj = base.WorldToObject;
		CalculateBounds();
		UpdateWorldAndWorldToObject(ref world, ref worldtoobj);
	}

	/// <summary>
	/// Adds a mesh to this object. Automatically recalculates the object bounds.
	/// </summary>
	/// <param name="mesh"></param>
	public void Add(RenderableMesh mesh)
	{
		HCc.Add(mesh);
		mesh.SetWorldAndWorldToObject(base.World, base.WorldToObject);
		RebuildMeshInfo();
		CalculateBounds();
	}

	/// <summary>
	/// Removes a mesh from this object. Automatically recalculates the object bounds.
	/// </summary>
	/// <param name="mesh"></param>
	public void Remove(RenderableMesh mesh)
	{
		HCc.Remove(mesh);
		CalculateBounds();
		RebuildMeshInfo();
	}

	/// <summary>
	/// Removes all meshes from this object.
	/// </summary>
	public void Clear()
	{
		HCc.Clear();
		HC_0011 = null;
		HCD = false;
		CalculateBounds();
		RebuildMeshInfo();
	}

	/// <summary>
	/// Called when the mesh list changes.
	/// </summary>
	protected virtual void RebuildMeshInfo()
	{
		if (!HCD)
		{
			HC_0011 = null;
		}
		CanLightMap = true;
		for (int i = 0; i < HCA.Count; i++)
		{
			RenderableMesh renderableMesh = HCA[i];
			if (HC_0011 == null)
			{
				HC_0011 = renderableMesh.HC6 as ICollisionMaterial;
			}
			if (!renderableMesh.HCH)
			{
				CanLightMap = false;
				break;
			}
		}
	}

	/// <summary>
	/// Calculates the object bounds.
	/// </summary>
	/// <param name="objectboundingbox">Object bounds to update.</param>
	/// <param name="objectboundingsphere">Object bounds to update.</param>
	protected override void CalculateObjectBounds(ref BoundingBox objectboundingbox, ref BoundingSphere objectboundingsphere)
	{
		if (base.InfiniteBounds)
		{
			base.CalculateObjectBounds(ref objectboundingbox, ref objectboundingsphere);
			return;
		}
		if (HCA.Count <= 0)
		{
			objectboundingbox = new BoundingBox(-Vector3.One, Vector3.One);
			objectboundingsphere = new BoundingSphere(Vector3.One, 1f);
			return;
		}
		RenderableMesh renderableMesh = HCA[0];
		objectboundingbox = CoreHelper.TransformBoundingBox(renderableMesh.HCZ, renderableMesh.HC7);
		objectboundingsphere = CoreHelper.TransformBoundingSphereSlow(renderableMesh.HCw, renderableMesh.HC7);
		for (int i = 1; i < HCA.Count; i++)
		{
			renderableMesh = HCA[i];
			BoundingBox additional = CoreHelper.TransformBoundingBox(renderableMesh.HCZ, renderableMesh.HC7);
			BoundingSphere additional2 = CoreHelper.TransformBoundingSphereSlow(renderableMesh.HCw, renderableMesh.HC7);
			objectboundingbox = BoundingBox.CreateMerged(objectboundingbox, additional);
			objectboundingsphere = BoundingSphere.CreateMerged(objectboundingsphere, additional2);
		}
	}

	/// <summary>
	/// Updates the object world bounds based on the current world transform and object space bounds.
	///
	/// NOTE: when implementing custom bounds ensure the hull type (box or sphere) is completely
	/// enclosed by the other bounds type. For instance if the hull type is Box then the bounding
	/// sphere should completely contain the bounding box, and vice-versa. This is critical for
	/// correct collision.
	/// </summary>
	/// <param name="worldboundingbox">World bounds to update.</param>
	/// <param name="worldboundingsphere">World bounds to update.</param>
	/// <param name="alreadymoved">Indicates the object move id is already updated.</param>
	protected override void CalculateWorldBounds(ref BoundingBox worldboundingbox, ref BoundingSphere worldboundingsphere, bool alreadymoved)
	{
		if (base.HullType == HullType.Mesh && base.UpdateType == UpdateType.Automatic)
		{
			base.HullType = HullType.Box;
		}
		base.CalculateWorldBounds(ref worldboundingbox, ref worldboundingsphere, alreadymoved);
		if (base.HullType == HullType.Mesh && !base.InfiniteBounds)
		{
			BoundingBox boundingBox = worldboundingbox;
			float num = worldboundingsphere.Radius * 2f;
			Vector3 vector = worldboundingbox.Max - worldboundingbox.Min;
			if (vector.X < num || vector.Y < num || vector.Z < num)
			{
				worldboundingsphere = BoundingSphere.CreateFromBoundingBox(worldboundingbox);
			}
			else
			{
				worldboundingbox = BoundingBox.CreateFromSphere(worldboundingsphere);
			}
			if (!alreadymoved && !boundingBox.Equals(worldboundingbox))
			{
				base.MoveId++;
			}
		}
	}

	/// <summary>
	/// Converts a ModelMesh into RenderableMeshes and adds them
	/// to this object. Automatically recalculates the object bounds.
	/// </summary>
	/// <param name="mesh"></param>
	/// <param name="overrideeffect">User defined effect used to render
	/// the object. If null the effects contained in the ModelMesh are used.</param>
	public void AddModelMesh(ModelMesh mesh, Effect overrideeffect)
	{
		int hash = B.H.k(mesh.Name);
		for (int i = 0; i < mesh.MeshParts.Count; i++)
		{
			ModelMeshPart modelMeshPart = mesh.MeshParts[i];
			Effect overrideeffect2 = modelMeshPart.Effect;
			if (overrideeffect != null)
			{
				overrideeffect2 = overrideeffect;
			}
			RenderableMesh renderableMesh = new RenderableMesh();
			renderableMesh.Build(this, mesh, modelMeshPart, overrideeffect2);
			renderableMesh.HC_0012 = CoreHelper.GetHashCode(base.UniqueId, hash);
			Add(renderableMesh);
		}
	}

	/// <summary>
	/// Updates the object world space and inverse world space transforms.
	/// Override to perform custom code when the world transform changes.
	/// </summary>
	/// <param name="world">World space transform.</param>
	/// <param name="worldtoobj">Inverse world space transform.</param>
	protected override void UpdateWorldAndWorldToObject(ref Matrix world, ref Matrix worldtoobj)
	{
		for (int i = 0; i < HCA.Count; i++)
		{
			HCA[i].SetWorldAndWorldToObject(ref world, ref worldtoobj);
		}
		base.UpdateWorldAndWorldToObject(ref world, ref worldtoobj);
	}

	/// <summary>
	/// Returns a String that represents the current Object.
	/// </summary>
	/// <returns></returns>
	public override string ToString()
	{
		return CoreHelper.GetDisplayName(this);
	}

	/// <summary>
	/// Helper method that creates a new SceneObject for each
	/// ModelMesh in the provided Model.
	/// </summary>
	/// <param name="model">Source Model object.</param>
	/// <param name="returnobjects">List used to store the created SceneObject objects.</param>
	public static void CreateMeshBasedObjectsFromModel(Model model, IList<SceneObject> returnobjects)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			returnobjects.Add(new SceneObject(model.Meshes[i]));
		}
	}

	/// <summary>
	/// Deserializes object data from the provided SerializationInfo.
	/// </summary>
	/// <param name="info">Contains the serialized object data.</param>
	/// <param name="context"></param>
	public override void SetObjectData(SerializationInfo info, StreamingContext context)
	{
		_7k(info);
		string field = string.Empty;
		string field2 = string.Empty;
		string field3 = string.Empty;
		SerializationHelper.DeserializeField(ref field2, info, "ModelFile", usedefault: true);
		SerializationHelper.DeserializeField(ref field3, info, "ModelMeshName", usedefault: true);
		SerializationHelper.DeserializeField(ref field, info, "ContentRepositoryName", usedefault: true);
		HC_0003 = R._7._7e(info);
		SerializationHelper.DeserializeField(ref HCs, info, "CustomStaticLightingColor", usedefault: true);
		SerializationHelper.DeserializeEnum(ref HCk, info, "LightMapSize", isflag: true);
		SerializationHelper.DeserializeEnum(ref HCw, info, "Visibility", isflag: false);
		SerializationHelper.DeserializeField(ref HCK, info, "Mass", usedefault: true);
		SerializationHelper.DeserializeField(ref HC_000F, info, "AffectedByGravity", usedefault: true);
		SerializationHelper.DeserializeEnum(ref HCy, info, "CollisionType", isflag: false);
		Visibility = HCw;
		ModelAsset = new ModelAsset(field, field2, field3);
		base.Components.SetObjectData(info, context);
	}

	/// <summary>
	/// Serializes object data to the provided SerializationInfo.
	/// </summary>
	/// <param name="info">SerializationInfo to store the serialized data.</param>
	/// <param name="context"></param>
	[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		base.GetObjectData(info, context);
		info.AddValue("ModelFile", ModelAsset.SourceAssetFilePath);
		info.AddValue("ModelMeshName", ModelAsset.ModelMeshName);
		info.AddValue("ContentRepositoryName", ModelAsset.ContentRepositoryName);
		info.AddValue("StaticLightingType", HC_0003);
		info.AddValue("CustomStaticLightingColor", HCs);
		info.AddValue("LightMapSize", HCk);
		info.AddValue("Visibility", HCw);
		info.AddValue("Mass", HCK);
		info.AddValue("AffectedByGravity", HC_000F);
		info.AddValue("CollisionType", HCy);
	}
}
