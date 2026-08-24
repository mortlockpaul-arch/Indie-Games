using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomInstanced : Atom, IGridable
{
	public AtomInstancer instancer;

	public int instancerIndex;

	public AtomInstanced(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
		SetInstancer();
	}

	public override void SetPosition(Vector3 vPosition)
	{
		base.SetPosition(vPosition);
		ref Matrix reference = ref instancer.matrices[instancerIndex];
		reference = matrix;
	}

	public override void SetRotation(Quaternion qRot)
	{
		base.SetRotation(qRot);
		ref Matrix reference = ref instancer.matrices[instancerIndex];
		reference = matrix;
	}

	private void SetInstancer()
	{
		if (manager.instancers.ContainsKey(definition.name))
		{
			instancer = manager.instancers[definition.name];
		}
		else
		{
			instancer = manager.Instancers_Make(definition);
		}
	}

	public void PopulateInstancer()
	{
		ref Matrix reference = ref instancer.matrices[instancerIndex];
		reference = matrix;
		ref Vector4 reference2 = ref instancer.data[instancerIndex];
		reference2 = data;
	}

	public void AddToInstancer()
	{
		instancer.Add(this);
	}

	public void RemoveFromInstancer()
	{
		instancer.Remove(this);
	}

	public override void Unover()
	{
		base.Unover();
		PopulateInstancer();
	}

	public override void Over()
	{
		base.Over();
		PopulateInstancer();
	}

	public override void Place(int xX, int xY, int xZ)
	{
		base.Place(xX, xY, xZ);
		PopulateInstancer();
	}

	public override void RotateAndUpdate(Quaternion xRotation)
	{
		base.RotateAndUpdate(xRotation);
		PopulateInstancer();
	}
}
