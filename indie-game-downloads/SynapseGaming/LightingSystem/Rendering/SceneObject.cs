using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using _0016;
using _0018;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
	private CollisionReactDelegate _3A_0018;

	private CollisionTriggerDelegate _3AL;

	private Matrix[] _3A_0019;

	private bool _3A3 = true;

	private bool _3A6 = true;

	private bool _3AD = true;

	private ObjectVisibility _3A_0017 = ObjectVisibility.RenderedAndCastShadows;

	private ModelAsset _3A_0003 = ModelAsset.Empty;

	private bool _3Al = true;

	private CollisionType _3At;

	private ICollisionMove _3AF;

	private bool _3Ac;

	private ICollisionMaterial _3Ag;

	private float _3AI = 1f;

	private StaticLightingType _3A8;

	private LightMapSize _3AZ = LightMapSize.Size128x128;

	private Vector3 _3Ax;

	private bool _3Aq;

	private bool _3Ab = true;

	private string _3AT = "";

	private RenderableMeshCollection _3Ay;

	private List<RenderableMesh> _3A_0015 = new List<RenderableMesh>(16);

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
			return _3A_0003;
		}
		set
		{
			if (value != null)
			{
				_3A_0003 = value;
			}
			else
			{
				_3A_0003 = ModelAsset.Empty;
			}
			_6q();
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
			return _3Al;
		}
		set
		{
			_3Al = value;
			_CollisionId++;
			if (!_3Al && _3AF != null)
			{
				_3AF.RemoveForces();
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
			return _3At;
		}
		set
		{
			_3At = value;
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
			return _3AF;
		}
		set
		{
			if (value == _3AF)
			{
				return;
			}
			if (_3AF != null)
			{
				foreach (KeyValuePair<Type, IManagerService> item in base.ContainingManagers.Items)
				{
					_3AF.OnRemovedFromManager(item.Value);
				}
			}
			_3AF = value;
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
			return _3Ag;
		}
		set
		{
			_3Ag = value;
			_3Ac = true;
			_CollisionId++;
		}
	}

	/// <summary>
	/// Mass of the object.
	/// </summary>
	[EditorProperty(true, Description = "Mass", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(3, 0.001, 10000.0, 0.1)]
	public float Mass
	{
		get
		{
			return _3AI;
		}
		set
		{
			_3AI = value;
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
			return _3A8;
		}
		set
		{
			if (!_3Aq && value == StaticLightingType.BakedDown)
			{
				_3A8 = StaticLightingType.None;
				return;
			}
			if (_3A8 != value)
			{
				_3Ab = true;
			}
			_3A8 = value;
		}
	}

	/// <summary>
	/// Determines the light map size when generating baked down lighting on the object.
	/// </summary>
	public LightMapSize LightMapSize
	{
		get
		{
			return _3AZ;
		}
		set
		{
			_3AZ = value;
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
			return _3Ax;
		}
		set
		{
			_3Ax = value;
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
			return _3Aq;
		}
		private set
		{
			_3Aq = flag;
			if (!_3Aq && _3A8 == StaticLightingType.BakedDown)
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
			return _3Ab;
		}
		set
		{
			_3Ab = value;
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
			return _3AT;
		}
		set
		{
			_3AT = value;
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
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
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
			return _3A_0017;
		}
		set
		{
			_3A_0017 = value;
			_3A3 = (_3A_0017 & ObjectVisibility.CastShadows) != 0;
			_3A6 = (_3A_0017 & ObjectVisibility.Rendered) != 0;
			_3AD = (_3A_0017 & ObjectVisibility.RenderedInEditor) != ObjectVisibility.None || (_3A_0017 & ObjectVisibility.Rendered) != 0;
		}
	}

	/// <summary>
	/// Determines if the object casts shadows based on the current ObjectVisibility options.
	/// </summary>
	public bool CastShadows => _3A3;

	/// <summary>
	/// Determines if the object is visible based on the current ObjectVisibility options.
	/// </summary>
	public bool Visible => _3A6;

	/// <summary>
	/// Determines if the object is visible in the editor based on the current ObjectVisibility options.
	/// </summary>
	public bool VisibleInEditor => _3AD;

	/// <summary>
	/// Collection of the object's internal mesh parts.
	/// </summary>
	public RenderableMeshCollection RenderableMeshes => _3Ay;

	/// <summary>
	/// Event used to detect when the object collides with another object, or to
	/// override the default reaction behavior between objects.
	/// </summary>
	public event CollisionReactDelegate CollisionReactEvent
	{
		add
		{
			CollisionReactDelegate collisionReactDelegate = _3A_0018;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Combine(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref _3A_0018, value2, collisionReactDelegate2);
			}
			while ((object)collisionReactDelegate != collisionReactDelegate2);
		}
		remove
		{
			CollisionReactDelegate collisionReactDelegate = _3A_0018;
			CollisionReactDelegate collisionReactDelegate2;
			do
			{
				collisionReactDelegate2 = collisionReactDelegate;
				CollisionReactDelegate value2 = (CollisionReactDelegate)Delegate.Remove(collisionReactDelegate2, value);
				collisionReactDelegate = Interlocked.CompareExchange(ref _3A_0018, value2, collisionReactDelegate2);
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
			CollisionTriggerDelegate collisionTriggerDelegate = _3AL;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Combine(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref _3AL, value2, collisionTriggerDelegate2);
			}
			while ((object)collisionTriggerDelegate != collisionTriggerDelegate2);
		}
		remove
		{
			CollisionTriggerDelegate collisionTriggerDelegate = _3AL;
			CollisionTriggerDelegate collisionTriggerDelegate2;
			do
			{
				collisionTriggerDelegate2 = collisionTriggerDelegate;
				CollisionTriggerDelegate value2 = (CollisionTriggerDelegate)Delegate.Remove(collisionTriggerDelegate2, value);
				collisionTriggerDelegate = Interlocked.CompareExchange(ref _3AL, value2, collisionTriggerDelegate2);
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
		_3Ay = new RenderableMeshCollection(_3A_0015);
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
		if (_3A_0003 != null && _3A_0003 != ModelAsset.Empty && _3A_0003.Asset != null)
		{
			sceneObject.ModelAsset = _3A_0003;
		}
		else if (sceneObject is SceneObject sceneObject2)
		{
			for (int i = 0; i < _3Ay.Count; i++)
			{
				sceneObject2.Add(_3Ay[i].Clone());
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
		if (_3AF != null)
		{
			_3AF.OnSubmittedToManager(manager);
		}
	}

	/// <summary>
	/// Called when the object is removed from a manager.
	/// </summary>
	/// <param name="manager"></param>
	public override void OnRemovedFromManager(IManagerService manager)
	{
		base.OnRemovedFromManager(manager);
		if (_3AF != null)
		{
			_3AF.OnRemovedFromManager(manager);
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
		if (_3A_0018 != null)
		{
			_3A_0018(collider, collidee, worldcollisionpoint, ref collisionhandled);
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
		if (_3AL != null)
		{
			_3AL(collider, trigger);
		}
	}

	private void _6q()
	{
		Clear();
		Model asset = _3A_0003.Asset;
		if (asset == null)
		{
			return;
		}
		string modelMeshName = _3A_0003.ModelMeshName;
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
		ContentRepository contentRepository = ContentRepository.Find(_3A_0003.ContentRepositoryName);
		if (contentRepository != null)
		{
			for (int j = 0; j < _3Ay.Count; j++)
			{
				contentRepository.LoadLightMap(_3Ay[j]);
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
		_3A_0015.Add(mesh);
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
		_3A_0015.Remove(mesh);
		CalculateBounds();
		RebuildMeshInfo();
	}

	/// <summary>
	/// Removes all meshes from this object.
	/// </summary>
	public void Clear()
	{
		_3A_0015.Clear();
		_3Ag = null;
		_3Ac = false;
		CalculateBounds();
		RebuildMeshInfo();
	}

	/// <summary>
	/// Called when the mesh list changes.
	/// </summary>
	protected virtual void RebuildMeshInfo()
	{
		if (!_3Ac)
		{
			_3Ag = null;
		}
		CanLightMap = true;
		for (int i = 0; i < _3Ay.Count; i++)
		{
			RenderableMesh renderableMesh = _3Ay[i];
			if (_3Ag == null)
			{
				_3Ag = renderableMesh._3AF as ICollisionMaterial;
			}
			if (!renderableMesh._3A3)
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
		if (_3Ay.Count <= 0)
		{
			objectboundingbox = new BoundingBox(-Vector3.One, Vector3.One);
			objectboundingsphere = new BoundingSphere(Vector3.One, 1f);
			return;
		}
		RenderableMesh renderableMesh = _3Ay[0];
		objectboundingbox = CoreHelper.TransformBoundingBox(renderableMesh._3A_0003, renderableMesh._3A6);
		objectboundingsphere = CoreHelper.TransformBoundingSphereSlow(renderableMesh._3A_0017, renderableMesh._3A6);
		for (int i = 1; i < _3Ay.Count; i++)
		{
			renderableMesh = _3Ay[i];
			BoundingBox additional = CoreHelper.TransformBoundingBox(renderableMesh._3A_0003, renderableMesh._3A6);
			BoundingSphere additional2 = CoreHelper.TransformBoundingSphereSlow(renderableMesh._3A_0017, renderableMesh._3A6);
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
		int hash = _0018._3.Z(mesh.Name);
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
			renderableMesh._3A_0019 = CoreHelper.GetHashCode(base.UniqueId, hash);
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
		for (int i = 0; i < _3Ay.Count; i++)
		{
			_3Ay[i].SetWorldAndWorldToObject(ref world, ref worldtoobj);
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
		r(info);
		string field = string.Empty;
		string field2 = string.Empty;
		string field3 = string.Empty;
		SerializationHelper.DeserializeField(ref field2, info, "ModelFile", usedefault: true);
		SerializationHelper.DeserializeField(ref field3, info, "ModelMeshName", usedefault: true);
		SerializationHelper.DeserializeField(ref field, info, "ContentRepositoryName", usedefault: true);
		_3A8 = _0016._6._61(info);
		SerializationHelper.DeserializeField(ref _3Ax, info, "CustomStaticLightingColor", usedefault: true);
		SerializationHelper.DeserializeEnum(ref _3AZ, info, "LightMapSize", isflag: true);
		SerializationHelper.DeserializeEnum(ref _3A_0017, info, "Visibility", isflag: false);
		SerializationHelper.DeserializeField(ref _3AI, info, "Mass", usedefault: true);
		SerializationHelper.DeserializeField(ref _3Al, info, "AffectedByGravity", usedefault: true);
		SerializationHelper.DeserializeEnum(ref _3At, info, "CollisionType", isflag: false);
		Visibility = _3A_0017;
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
		info.AddValue("StaticLightingType", _3A8);
		info.AddValue("CustomStaticLightingColor", _3Ax);
		info.AddValue("LightMapSize", _3AZ);
		info.AddValue("Visibility", _3A_0017);
		info.AddValue("Mass", _3AI);
		info.AddValue("AffectedByGravity", _3Al);
		info.AddValue("CollisionType", _3At);
	}
}
