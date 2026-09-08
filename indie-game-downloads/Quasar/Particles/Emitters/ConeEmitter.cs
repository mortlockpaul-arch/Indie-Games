using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class ConeEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	private Vector3 dirAngle;

	private float angle;

	public float Angle
	{
		get
		{
			return angle;
		}
		set
		{
			angle = value;
			float num = (float)Math.Sin(value);
			dirAngle = new Vector3(num, num, 0f - (float)Math.Cos(value));
		}
	}

	public ConeEmitter(string name, ParticleSystem p, float angle)
		: base(name, p)
	{
		Angle = angle;
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector2 vector = GameMath.RandomPointInCircle();
		Vector3 v = new Vector3(dirAngle.X * vector.X, dirAngle.Y * vector.Y, dirAngle.Z);
		GameMath.VectorSetLength(ref v, speed.Next(), out v);
		Vector3 point = Vector3.Zero;
		Vector3 direction = v + baseSpeed.Next();
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

	public static ConeEmitter Load(XElement xe, ParticleSystem p)
	{
		ConeEmitter coneEmitter = new ConeEmitter(XDocHelper.GetAttribute(xe, "name"), p, XDocHelper.ParseFloatAttribute(xe, "angle") * ((float)Math.PI / 180f));
		coneEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		coneEmitter.ParseXml(xe);
		return coneEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		ConeEmitter coneEmitter = new ConeEmitter("", particleSystem, angle);
		coneEmitter.speed = speed.Clone();
		return coneEmitter;
	}
}
