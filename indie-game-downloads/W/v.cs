using d;
using r;

namespace W;

internal class v : r._6
{
	private struct _00065h(r.h spaceObject, bool shouldAdd)
	{
		public readonly r.h SpaceObject = spaceObject;

		public readonly bool ShouldAdd = shouldAdd;
	}

	private d.h<_00065h> a5h = new d.h<_00065h>();

	private r.a a5b;

	public r.a Space => a5b;

	public v(r.a space)
	{
		Enabled = true;
		a5b = space;
	}

	public void Add(r.h spaceObject)
	{
		a5h.Enqueue(new _00065h(spaceObject, shouldAdd: true));
	}

	public void Remove(r.h spaceObject)
	{
		a5h.Enqueue(new _00065h(spaceObject, shouldAdd: false));
	}

	protected override void UpdateStage()
	{
		_00065h item;
		while (a5h.TryDequeueFirst(out item))
		{
			if (item.ShouldAdd)
			{
				a5b.Add(item.SpaceObject);
			}
			else
			{
				a5b.Remove(item.SpaceObject);
			}
		}
	}
}
