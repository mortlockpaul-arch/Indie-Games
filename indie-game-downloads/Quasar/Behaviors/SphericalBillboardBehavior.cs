using Microsoft.Xna.Framework;

namespace Quasar.Behaviors;

public class SphericalBillboardBehavior : Behavior
{
	public override void DoUpdate(Element element)
	{
		Matrix matrix = Matrix.Invert(element.Transform.Parent);
		matrix.Translation = Vector3.Zero;
		Vector3 translation = element.Transform.Translation;
		element.Transform.Matrix = Matrix.CreateBillboard(translation, translation - Scene.CurrentInstance.Camera.Transform.ZVector, Scene.CurrentInstance.Camera.Transform.YVector, Scene.CurrentInstance.Camera.Transform.ZVector) * matrix;
		element.Transform.Translation = translation;
	}
}
