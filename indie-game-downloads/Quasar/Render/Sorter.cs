namespace Quasar.Render;

public abstract class Sorter
{
	public virtual void BeginRender()
	{
	}

	public abstract void Add(Mesh mesh, Transform motion, int material);

	public virtual void EndRender()
	{
	}
}
