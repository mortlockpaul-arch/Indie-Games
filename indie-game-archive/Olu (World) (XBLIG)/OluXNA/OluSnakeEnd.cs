using Microsoft.Xna.Framework;

namespace OluXNA;

internal class OluSnakeEnd : Enemy
{
	protected OluSnake parent;

	public OluSnakeEnd(OluSnake _parent)
	{
		parent = _parent;
		hitPoints = 1;
		pathList = parent.pathList.Clone();
		pathList.curPathIndex = 0;
		pathList.ResetCurrent();
	}

	public override Matrix Transformation()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return Matrix.CreateTranslation(getPos());
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void hit(TargetEffectBase toHit)
	{
		base.hit(toHit);
	}

	public override void die()
	{
		base.die();
	}

	public override void act(GameTime gametime)
	{
		base.act(gametime);
	}

	public override void start()
	{
		addCond(new NeverCondition());
		base.start();
	}

	public override string name()
	{
		return "[0lLoup]";
	}
}
