using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class LineEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	public float length;

	public LineEmitter(string name, ParticleSystem p)
		: base(name, p)
	{
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		float num = GameMath.Random.NextFloat(-0.5f, 0.5f);
		Vector3 vector = new Vector3(0f, speed.Next(), 0f);
		Vector3 point = new Vector3(0f, 0f, num * length);
		Vector3 direction = vector + baseSpeed.Next();
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

	public static LineEmitter Load(XElement xe, ParticleSystem p)
	{
		LineEmitter lineEmitter = new LineEmitter(XDocHelper.GetAttribute(xe, "name"), p);
		lineEmitter.length = XDocHelper.ParseFloatAttribute(xe, "length");
		lineEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		lineEmitter.ParseXml(xe);
		return lineEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		LineEmitter lineEmitter = new LineEmitter("", particleSystem);
		lineEmitter.length = length;
		lineEmitter.speed = speed.Clone();
		return lineEmitter;
	}
}
