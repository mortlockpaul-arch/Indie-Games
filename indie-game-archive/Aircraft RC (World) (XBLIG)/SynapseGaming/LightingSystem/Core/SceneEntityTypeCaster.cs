using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Rendering;

namespace SynapseGaming.LightingSystem.Core;

/// <summary />
public class SceneEntityTypeCaster : ITypeCaster<SceneEntity>
{
	/// <summary />
	public ISceneObject SceneObject;

	/// <summary />
	public ICollisionObject CollisionObject;

	/// <summary />
	public void Set(SceneEntity entity)
	{
		SceneObject = entity as ISceneObject;
		CollisionObject = entity as ICollisionObject;
	}
}
