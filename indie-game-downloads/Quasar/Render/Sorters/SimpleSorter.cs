namespace Quasar.Render.Sorters;

public class SimpleSorter : Sorter
{
	private static SimpleSorter instance;

	public static SimpleSorter Instance => instance;

	public override void Add(Mesh mesh, Transform motion, int material)
	{
		mesh.RenderMaterial(motion, material);
	}

	static SimpleSorter()
	{
		instance = new SimpleSorter();
	}
}
