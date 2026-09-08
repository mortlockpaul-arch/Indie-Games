using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Utils;

namespace Quasar.Particles.Emitters;

public class RectangleEmitter : ParticleEmitter
{
	public FuzzyValue<float> speed;

	public float width;

	public float height;

	private Vector3 dirAngleW;

	private Vector3 dirAngleH;

	private float angleW;

	private float angleH;

	public float AngleW
	{
		get
		{
			return angleW;
		}
		set
		{
			angleW = value;
			float num = (float)Math.Sin(value);
			dirAngleW = new Vector3(num, num, 0f - (float)Math.Cos(value));
		}
	}

	public float AngleH
	{
		get
		{
			return angleH;
		}
		set
		{
			angleH = value;
			float num = (float)Math.Sin(value);
			dirAngleH = new Vector3(num, num, 0f - (float)Math.Cos(value));
		}
	}

	public RectangleEmitter(string name, ParticleSystem p)
		: base(name, p)
	{
		AngleW = 0f;
		AngleH = 0f;
	}

	protected override bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector2 vector = GameMath.RandomVector2(-0.5f, 0.5f);
		Vector3 v = new Vector3(dirAngleW.X * vector.X, dirAngleH.Y * vector.Y, dirAngleW.Z * Math.Abs(vector.X) + dirAngleH.Z * Math.Abs(vector.Y));
		if (GameMath.ZeroLength(v))
		{
			v = dirAngleW * speed.Next();
		}
		else
		{
			GameMath.VectorSetLength(ref v, speed.Next(), out v);
		}
		Vector3 point = new Vector3(vector.X * width, vector.Y * height, 0f);
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

	public static RectangleEmitter Load(XElement xe, ParticleSystem p)
	{
		RectangleEmitter rectangleEmitter = new RectangleEmitter(XDocHelper.GetAttribute(xe, "name"), p);
		rectangleEmitter.AngleH = XDocHelper.ParseFloatAttribute(xe, "angleH") * ((float)Math.PI / 180f);
		rectangleEmitter.AngleW = XDocHelper.ParseFloatAttribute(xe, "angleW") * ((float)Math.PI / 180f);
		rectangleEmitter.width = XDocHelper.ParseFloatAttribute(xe, "width");
		rectangleEmitter.height = XDocHelper.ParseFloatAttribute(xe, "height");
		rectangleEmitter.speed = FuzzyValue.ParseXmlFloat(xe, "Speed");
		rectangleEmitter.ParseXml(xe);
		return rectangleEmitter;
	}

	protected override ParticleEmitter CloneEmitter()
	{
		RectangleEmitter rectangleEmitter = new RectangleEmitter("", particleSystem);
		rectangleEmitter.AngleW = AngleW;
		rectangleEmitter.AngleH = AngleH;
		rectangleEmitter.width = width;
		rectangleEmitter.height = height;
		rectangleEmitter.speed = speed.Clone();
		return rectangleEmitter;
	}
}
