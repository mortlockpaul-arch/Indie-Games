using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class SphereEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	public float radius;

	public SphereEmitter(string name, ParticleSystem p)
		: base(name, p)
	{
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector3 vector = GameMath.RandomVector3(-1f, 1f);
		vector.Normalize();
		Vector3 point = vector * radius;
		Vector3 direction = vector * speed.Next();
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

	public static SphereEmitter Load(XElement xe, ParticleSystem p)
	{
		SphereEmitter sphereEmitter = new SphereEmitter(XDocHelper.GetAttribute(xe, "name"), p);
		sphereEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		sphereEmitter.radius = XDocHelper.ParseFloatAttribute(xe, "radius");
		sphereEmitter.ParseXml(xe);
		return sphereEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		SphereEmitter sphereEmitter = new SphereEmitter("", particleSystem);
		sphereEmitter.speed = speed.Clone();
		sphereEmitter.radius = radius;
		return sphereEmitter;
	}
}
