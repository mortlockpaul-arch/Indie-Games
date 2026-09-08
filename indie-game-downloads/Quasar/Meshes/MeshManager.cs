using Quasar.Global;

namespace Quasar.Meshes;

public class MeshManager : Manager<Mesh>
{
	private static MeshManager instance;

	public static MeshManager Meshes
	{
		get
		{
			if (instance == null)
			{
				instance = new MeshManager();
			}
			return instance;
		}
	}

	private MeshManager()
	{
	}

	protected override Mesh createDefaultItem()
	{
		return new Cube();
	}

	protected override Mesh LoadItem(string name)
	{
		return new XMesh(name);
	}
}
