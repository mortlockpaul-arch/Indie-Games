using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class Target
{
	public Vector3 pos;

	public Enemy enem;

	public int selected;

	public int hp;

	public int score;

	public FillMode fillMode;

	public Target()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		pos = Vector3.Zero;
		selected = 0;
		hp = 1;
		score = 10;
	}

	public Target(Target other)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		pos = other.pos;
		selected = other.selected;
		enem = other.enem;
		hp = other.hp;
		score = other.score;
		fillMode = other.fillMode;
	}

	~Target()
	{
		Dispose();
	}

	public void Dispose()
	{
	}

	public virtual Vector3 absolutePos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Transform(pos, enem.Transformation());
	}

	public virtual bool Visible()
	{
		return true;
	}
}
