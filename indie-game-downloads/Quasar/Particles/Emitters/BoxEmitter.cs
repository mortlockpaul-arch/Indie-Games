using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class BoxEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	public Vector3 size;

	public BoxEmitter(string name, ParticleSystem p)
		: base(name, p)
	{
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector3 point = new Vector3(GameMath.Random.NextFloat((0f - size.X) * 0.5f, size.X * 0.5f), GameMath.Random.NextFloat((0f - size.Y) * 0.5f, size.Y * 0.5f), GameMath.Random.NextFloat((0f - size.Z) * 0.5f, size.Z * 0.5f));
		Vector3 direction = GameMath.VectorSetLength(point, speed.Next());
		direction += baseSpeed.Next();
		transform.TransformPoint(ref point, out point);
		transform.RotateVectorLocal(ref direction, out direction);
		if (rotation != Quaternion.Identity)
		{
			Vector3.Transform(ref point, ref rotation, out point);
			Vector3.Transform(ref direction, ref rotation, out direction);
		}
		point += origin;
		if (ParticleGroup != null)
		{
			return ParticleGroup.SpawnParticle(time, ref point, ref direction);
		}
		return false;
	}

	public static BoxEmitter Load(XElement xe, ParticleSystem p)
	{
		BoxEmitter boxEmitter = new BoxEmitter(XDocHelper.GetAttribute(xe, "name"), p);
		boxEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		boxEmitter.size = XDocHelper.ParseVector3Attribute(xe, "size");
		boxEmitter.ParseXml(xe);
		return boxEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		BoxEmitter boxEmitter = new BoxEmitter("", particleSystem);
		boxEmitter.speed = speed.Clone();
		boxEmitter.size = size;
		return boxEmitter;
	}
}
