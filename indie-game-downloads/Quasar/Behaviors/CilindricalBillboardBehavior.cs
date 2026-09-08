using Microsoft.Xna.Framework;

namespace Quasar.Behaviors;

public class CilindricalBillboardBehavior : Behavior
{
	public override void DoUpdate(Element element)
	{
		Matrix matrix = Matrix.Invert(element.Transform.Parent);
		matrix.Translation = Vector3.Zero;
		element.Transform.Matrix = Matrix.CreateConstrainedBillboard(element.Transform.Translation, element.Transform.Translation - Scene.CurrentInstance.Camera.Transform.ZVector, element.Transform.YVector, null, -Vector3.UnitZ) * matrix;
	}
}
