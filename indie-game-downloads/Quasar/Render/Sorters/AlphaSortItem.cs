namespace Quasar.Render.Sorters;

public struct AlphaSortItem(float distance, Mesh mesh, Transform transform, int material, Material.Priority priority)
{
	public float distance = distance;

	public Mesh mesh = mesh;

	public Transform transform = transform;

	public int material = material;

	public Material.Priority priority = priority;
}
