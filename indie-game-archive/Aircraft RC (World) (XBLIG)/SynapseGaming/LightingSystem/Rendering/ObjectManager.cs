using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Rendering;

/// <summary>
/// Manages all scene objects in a mini scenegraph.
/// </summary>
public class ObjectManager : BaseObjectGraphManager<SceneEntity, IObjectManager>, IObjectManager, IWorldRenderableManager, IRenderableManager, IManagerService, IUpdatableManager, IManager, IUnloadable, IQuery<SceneEntity>, ISubmit<SceneEntity>, IQuery<RenderableMesh>, ISubmit<IScene>
{
	private int HCB = 50;

	private List<SceneEntity> HC_0002 = new List<SceneEntity>();

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public override Type ManagerType => SceneInterface.ObjectManagerType;

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
	/// Creates a new ObjectManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	/// <param name="worldboundingbox">The smallest bounding area that completely
	/// contains the scene.  Helps the RenderManager build an optimal scene tree.</param>
	/// <param name="worldtreemaxdepth"></param>
	public ObjectManager(IManagerServiceProvider sceneinterface, BoundingBox worldboundingbox, int worldtreemaxdepth)
		: base(sceneinterface, worldboundingbox, worldtreemaxdepth)
	{
	}

	/// <summary>
	/// Creates a new ObjectManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public ObjectManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
	}

	/// <summary />
	~ObjectManager()
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
		SplashScreen._7z();
	}

	/// <summary>
	/// Updates the object and its contained resources.
	/// </summary>
	/// <param name="gametime"></param>
	public override void Update(GameTime gametime)
	{
		LightingSystemPerformance.Begin("ObjectManager.Update");
		base.Update(gametime);
	}

	private void _7_0003(Effect P_0, Effect P_1)
	{
		HC_0002.Clear();
		Find(HC_0002, ObjectFilter.All);
		foreach (SceneEntity item in HC_0002)
		{
			if (item is ISceneObject sceneObject)
			{
				for (int i = 0; i < sceneObject.RenderableMeshes.Count; i++)
				{
					sceneObject.RenderableMeshes[i].RemapEffect();
				}
			}
		}
	}

	/// <summary>
	/// Helper method that creates and submits a static scene
	/// object using a method layout similar to SunBurn 1.2.x.
	///
	/// NOTE: This method creates a single scene object for an
	/// entire model.  This is ideal for small models such as
	/// props, however large models that represent entire rooms
	/// or scenes need to be split into separate objects per
	/// model mesh using SubmitStaticSceneObjectPerMesh.
	/// </summary>
	/// <param name="model">Source model.</param>
	/// <param name="world">Scene object world transform.</param>
	/// <param name="visibility">Defines how the object is rendered.
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders objects and casts shadows from them).</param>
	public void SubmitStaticSceneObject(Model model, Matrix world, ObjectVisibility visibility)
	{
		SceneObject sceneObject = new SceneObject(model);
		sceneObject.UpdateType = UpdateType.None;
		sceneObject.Visibility = visibility;
		sceneObject.World = world;
		Submit(sceneObject);
	}

	/// <summary>
	/// Helper method that creates and submits a static scene
	/// object using a method layout similar to SunBurn 1.2.x.
	///
	/// NOTE: This method creates a scene object for each mesh
	/// contained in the model.  This is ideal for large models
	/// that represent entire rooms or scenes, however small
	/// models such as props should be contained in a single scene
	/// object using SubmitStaticSceneObject.
	/// </summary>
	/// <param name="model">Source model.</param>
	/// <param name="world">Scene object world transform.</param>
	/// <param name="visibility">Defines how the object is rendered.
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders objects and casts shadows from them).</param>
	public void SubmitStaticSceneObjectPerMesh(Model model, Matrix world, ObjectVisibility visibility)
	{
		int count = model.Meshes.Count;
		for (int i = 0; i < count; i++)
		{
			SubmitStaticSceneObject(model.Meshes[i], world, visibility);
		}
	}

	/// <summary>
	/// Helper method that creates and submits a static scene
	/// object using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="mesh">Source model mesh.</param>
	/// <param name="world">Scene object world transform.</param>
	/// <param name="visibility">Defines how the object is rendered.
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders objects and casts shadows from them).</param>
	public void SubmitStaticSceneObject(ModelMesh mesh, Matrix world, ObjectVisibility visibility)
	{
		SceneObject sceneObject = new SceneObject(mesh);
		sceneObject.UpdateType = UpdateType.None;
		sceneObject.Visibility = visibility;
		sceneObject.World = world;
		Submit(sceneObject);
	}

	/// <summary>
	/// Helper method that creates and submits a static scene
	/// object using a method layout similar to SunBurn 1.2.x.
	///
	/// NOTE: This method creates a single scene object for an
	/// entire model.  This is ideal for small models such as
	/// props, however large models that represent entire rooms
	/// or scenes need to be split into separate objects per
	/// model mesh using SubmitStaticSceneObjectPerMesh.
	/// </summary>
	/// <param name="model">Source model.</param>
	/// <param name="overrideeffect">User defined effect used to render the object.</param>
	/// <param name="world">Scene object world transform.</param>
	/// <param name="visibility">Defines how the object is rendered.
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders objects and casts shadows from them).</param>
	public void SubmitStaticSceneObject(Model model, Effect overrideeffect, Matrix world, ObjectVisibility visibility)
	{
		SceneObject sceneObject = new SceneObject(model, overrideeffect, model.Root.Name);
		sceneObject.UpdateType = UpdateType.None;
		sceneObject.Visibility = visibility;
		sceneObject.World = world;
		Submit(sceneObject);
	}

	/// <summary>
	/// Helper method that creates and submits a static scene
	/// object using a method layout similar to SunBurn 1.2.x.
	///
	/// NOTE: This method creates a scene object for each mesh
	/// contained in the model.  This is ideal for large models
	/// that represent entire rooms or scenes, however small
	/// models such as props should be contained in a single scene
	/// object using SubmitStaticSceneObject.
	/// </summary>
	/// <param name="model">Source model.</param>
	/// <param name="overrideeffect">User defined effect used to render the object.</param>
	/// <param name="world">Scene object world transform.</param>
	/// <param name="visibility">Defines how the object is rendered.
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders objects and casts shadows from them).</param>
	public void SubmitStaticSceneObjectPerMesh(Model model, Effect overrideeffect, Matrix world, ObjectVisibility visibility)
	{
		int count = model.Meshes.Count;
		for (int i = 0; i < count; i++)
		{
			SubmitStaticSceneObject(model.Meshes[i], overrideeffect, world, visibility);
		}
	}

	/// <summary>
	/// Helper method that creates and submits a static scene
	/// object using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="mesh">Source model mesh.</param>
	/// <param name="overrideeffect">User defined effect used to render the object.</param>
	/// <param name="world">Scene object world transform.</param>
	/// <param name="visibility">Defines how the object is rendered.
	/// This enumeration is a Flag, which allows combining multiple values using the
	/// Logical OR operator (example: "ObjectVisibility.Rendered | ObjectVisibility.CastShadows",
	/// both renders objects and casts shadows from them).</param>
	public void SubmitStaticSceneObject(ModelMesh mesh, Effect overrideeffect, Matrix world, ObjectVisibility visibility)
	{
		SceneObject sceneObject = new SceneObject(mesh, overrideeffect, mesh.ParentBone.Name);
		sceneObject.UpdateType = UpdateType.None;
		sceneObject.Visibility = visibility;
		sceneObject.World = world;
		Submit(sceneObject);
	}

	/// <summary>
	/// Finds all contained objects that match a set of filter attributes
	/// and overlap with or are contained in a bounding area.
	/// </summary>
	/// <param name="foundmeshes">List used to store found objects during the query.</param>
	/// <param name="worldbounds">Bounding area used to limit query results.</param>
	/// <param name="objectfilter">Filter used to limit query results to objects with specific attributes.</param>
	public virtual void Find(List<RenderableMesh> foundmeshes, BoundingFrustum worldbounds, ObjectFilter objectfilter)
	{
		HC_0002.Clear();
		Find(HC_0002, worldbounds, objectfilter);
		foreach (SceneEntity item in HC_0002)
		{
			if (item == null)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ISceneObject sceneObject = sceneEntityTypeCaster.SceneObject;
			if (sceneObject != null)
			{
				for (int i = 0; i < sceneObject.RenderableMeshes.Count; i++)
				{
					foundmeshes.Add(sceneObject.RenderableMeshes[i]);
				}
			}
		}
	}

	/// <summary>
	/// Finds all contained objects that match a set of filter attributes
	/// and overlap with or are contained in a bounding area.
	/// </summary>
	/// <param name="foundmeshes">List used to store found objects during the query.</param>
	/// <param name="worldbounds">Bounding area used to limit query results.</param>
	/// <param name="objectfilter">Filter used to limit query results to objects with specific attributes.</param>
	public virtual void Find(List<RenderableMesh> foundmeshes, BoundingBox worldbounds, ObjectFilter objectfilter)
	{
		HC_0002.Clear();
		Find(HC_0002, worldbounds, objectfilter);
		foreach (SceneEntity item in HC_0002)
		{
			if (item == null)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ISceneObject sceneObject = sceneEntityTypeCaster.SceneObject;
			if (sceneObject != null)
			{
				for (int i = 0; i < sceneObject.RenderableMeshes.Count; i++)
				{
					foundmeshes.Add(sceneObject.RenderableMeshes[i]);
				}
			}
		}
	}

	/// <summary>
	/// Finds all contained objects that match a set of filter attributes.
	/// </summary>
	/// <param name="foundmeshes">List used to store found objects during the query.</param>
	/// <param name="objectfilter">Filter used to limit query results to objects with specific attributes.</param>
	public virtual void Find(List<RenderableMesh> foundmeshes, ObjectFilter objectfilter)
	{
		HC_0002.Clear();
		Find(HC_0002, objectfilter);
		foreach (SceneEntity item in HC_0002)
		{
			if (item == null)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ISceneObject sceneObject = sceneEntityTypeCaster.SceneObject;
			if (sceneObject != null)
			{
				for (int i = 0; i < sceneObject.RenderableMeshes.Count; i++)
				{
					foundmeshes.Add(sceneObject.RenderableMeshes[i]);
				}
			}
		}
	}

	/// <summary>
	/// Quickly finds all objects near a bounding area without the overhead of
	/// filtering by object type, checking if objects are enabled, or verifying
	/// containment within the bounds.
	/// </summary>
	/// <param name="foundmeshes">List used to store found objects during the query.</param>
	/// <param name="worldbounds">Bounding area used to limit query results.</param>
	public void FindFast(List<RenderableMesh> foundmeshes, BoundingBox worldbounds)
	{
		HC_0002.Clear();
		FindFast(HC_0002, worldbounds);
		foreach (SceneEntity item in HC_0002)
		{
			if (item == null)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ISceneObject sceneObject = sceneEntityTypeCaster.SceneObject;
			if (sceneObject != null)
			{
				for (int i = 0; i < sceneObject.RenderableMeshes.Count; i++)
				{
					foundmeshes.Add(sceneObject.RenderableMeshes[i]);
				}
			}
		}
	}

	/// <summary>
	/// Quickly finds all objects without the overhead of filtering by object
	/// type or checking if objects are enabled.
	/// </summary>
	/// <param name="foundmeshes">List used to store found objects during the query.</param>
	public void FindFast(List<RenderableMesh> foundmeshes)
	{
		HC_0002.Clear();
		FindFast(HC_0002);
		foreach (SceneEntity item in HC_0002)
		{
			if (item == null)
			{
				continue;
			}
			SceneEntityTypeCaster sceneEntityTypeCaster = OptimizationSystem.SceneEntityTypeCasters.Get(item);
			ISceneObject sceneObject = sceneEntityTypeCaster.SceneObject;
			if (sceneObject != null)
			{
				for (int i = 0; i < sceneObject.RenderableMeshes.Count; i++)
				{
					foundmeshes.Add(sceneObject.RenderableMeshes[i]);
				}
			}
		}
	}
}
