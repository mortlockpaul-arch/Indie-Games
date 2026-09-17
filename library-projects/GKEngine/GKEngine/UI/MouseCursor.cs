using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;

namespace GKEngine.UI;

public class MouseCursor : Sequence
{
	private bool _active;

	public Vector2 offset = new Vector2(0f, 0f);

	public bool active
	{
		get
		{
			return _active;
		}
		set
		{
			_active = value;
			if (_active)
			{
				Update_Add();
			}
		}
	}

	public MouseCursor(Scene oScene, string xAssetBase, int xStart, int xEnd, int xDigits)
		: base(oScene, xAssetBase, xStart, xEnd, xDigits)
	{
	}

	public override void Load()
	{
		base.Load();
		active = true;
	}

	public override void Dispose()
	{
		active = false;
		base.Dispose();
	}

	public void Update_Add()
	{
		if (!GameEngine.instance.updateStack.add.Contains(Update) && !GameEngine.instance.updateStack.stack.Contains(Update))
		{
			GameEngine.instance.updateStack.Add(Update);
		}
	}

	public bool Update(GameTime oGameTime)
	{
		return !_active;
	}
}
