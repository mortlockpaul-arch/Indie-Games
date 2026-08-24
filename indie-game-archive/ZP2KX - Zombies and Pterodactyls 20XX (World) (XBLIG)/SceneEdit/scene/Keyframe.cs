using System.IO;
using Microsoft.Xna.Framework;

namespace SceneEdit.scene;

public class Keyframe
{
	public string texture;

	public Vector3 loc;

	public float r;

	public float g;

	public float b;

	public float a;

	public float angle;

	public Vector2 scale;

	public float time;

	public bool tween;

	public Keyframe()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		texture = null;
		scale = new Vector2(1f, 1f);
		loc.Z = 1f;
		r = (g = (b = (a = 1f)));
	}

	internal void Write(BinaryWriter writer)
	{
		if (texture == null)
		{
			writer.Write("master");
		}
		else
		{
			writer.Write(texture);
		}
		writer.Write(loc.X);
		writer.Write(loc.Y);
		writer.Write(loc.Z);
		writer.Write(r);
		writer.Write(g);
		writer.Write(b);
		writer.Write(a);
		writer.Write(angle);
		writer.Write(scale.X);
		writer.Write(scale.Y);
		writer.Write(time);
		writer.Write(tween);
	}

	internal void Read(BinaryReader reader)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		texture = reader.ReadString();
		loc = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
		r = reader.ReadSingle();
		g = reader.ReadSingle();
		b = reader.ReadSingle();
		a = reader.ReadSingle();
		angle = reader.ReadSingle();
		scale = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		time = reader.ReadSingle();
		tween = reader.ReadBoolean();
	}
}
