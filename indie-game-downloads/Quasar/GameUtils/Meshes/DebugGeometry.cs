using Microsoft.Xna.Framework;
using Quasar.Meshes;
using Quasar.Shaders;

namespace Quasar.GameUtils.Meshes;

public static class DebugGeometry
{
	private static InstancedMesh debugMesh;

	private static Transform motion;

	static DebugGeometry()
	{
		motion = new Transform();
		debugMesh = new InstancedMesh("Cube", 64);
		debugMesh.Shader = ShaderManager.Shaders["DebugGeom"];
	}

	public static void Clear()
	{
		debugMesh.ClearInstances();
	}

	public static void AddCube(Vector3 position, Vector3 size)
	{
		Matrix data = Matrix.CreateScale(size) * Matrix.CreateTranslation(position);
		debugMesh.AddInstance(ref data, checkCull: true);
	}

	public static void Render()
	{
		debugMesh.UpdateData();
		debugMesh.Render(motion);
	}
}
