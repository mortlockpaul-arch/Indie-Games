using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class OmniEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	public OmniEmitter(string name, ParticleSystem p)
		: base(name, p)
	{
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector3 point = Vector3.Zero;
		Vector3 direction = (GameMath.RandomVector3() * 2f - Vector3.One) * speed.Next();
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

	public static OmniEmitter Load(XElement xe, ParticleSystem p)
	{
		OmniEmitter omniEmitter = new OmniEmitter(XDocHelper.GetAttribute(xe, "name"), p);
		omniEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		omniEmitter.ParseXml(xe);
		return omniEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		OmniEmitter omniEmitter = new OmniEmitter("", particleSystem);
		omniEmitter.speed = speed.Clone();
		return omniEmitter;
	}
}
