namespace Quasar.Render.Sorters;

public struct ZSortItem(float yPos, Mesh mesh, Transform transform, int material, Material.Priority priority)
{
	public float yPos = yPos;

	public Mesh mesh = mesh;

	public Transform transform = transform;

	public int material = material;

	public Material.Priority priority = priority;
}
