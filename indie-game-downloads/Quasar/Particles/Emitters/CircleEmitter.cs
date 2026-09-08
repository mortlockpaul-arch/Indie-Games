using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class CircleEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	public float radius;

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

	public CircleEmitter(string name, ParticleSystem p, float angle)
		: base(name, p)
	{
		Angle = angle;
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector2 vector = GameMath.VectorFromAngle(GameMath.RandomAngle());
		Vector3 v = new Vector3(dirAngle.X * vector.X, dirAngle.Y * vector.Y, dirAngle.Z);
		GameMath.VectorSetLength(ref v, speed.Next(), out v);
		Vector3 point = new Vector3(vector * radius, 0f);
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

	public static CircleEmitter Load(XElement xe, ParticleSystem p)
	{
		CircleEmitter circleEmitter = new CircleEmitter(XDocHelper.GetAttribute(xe, "name"), p, XDocHelper.ParseFloatAttribute(xe, "angle") * ((float)Math.PI / 180f));
		circleEmitter.radius = XDocHelper.ParseFloatAttribute(xe, "radius");
		circleEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		circleEmitter.ParseXml(xe);
		return circleEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		CircleEmitter circleEmitter = new CircleEmitter("", particleSystem, angle);
		circleEmitter.radius = radius;
		circleEmitter.speed = speed.Clone();
		return circleEmitter;
	}
}
