using Microsoft.Xna.Framework;

namespace Quasar.Behaviors;

public class LookAtBehavior : Behavior
{
	private Transform targetTransform;

	private Vector3 upVector;

	public Vector3 UpVector
	{
		get
		{
			return upVector;
		}
		set
		{
			upVector = value;
		}
	}

	public LookAtBehavior(Transform target)
		: this(target, Vector3.Up)
	{
	}

	public LookAtBehavior(Transform target, Vector3 upVector)
	{
		targetTransform = target;
		this.upVector = upVector;
	}

	public override void DoUpdate(Element element)
	{
		element.Transform.LookAt(element.Transform.Translation, targetTransform.WorldTranslation, upVector);
	}
}
