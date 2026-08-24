using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Dialogs;

public class DialogMenuOption
{
	public delegate void OptionDelegate(DialogMenu menu, DialogMenuOption me);

	private const float SHADOW_OFFSET_Y = 3f;

	private const float SCALE_DEFAULT = 0.7f;

	private const float SCALE_SELECTED = 1f;

	private Color COLOR_SELECTED = new Color(146, 214, 20);

	private Color COLOR_SELECTED_SHADOW = new Color(67, 126, 59);

	private Color COLOR_DEFAULT = new Color(221, 18, 73);

	private Color COLOR_DEFAULT_SHADOW = new Color(118, 18, 55);

	private string title;

	public Dialog.DialogDelegate action;

	public object data;

	public bool selected;

	public bool hasHorizontal;

	public bool autoCloseDialog = true;

	public OptionDelegate show;

	public bool deactivated;

	private float _X;

	private float _Y;

	private float _scale = 1f;

	public SpriteString stringTitle;

	public SpriteString stringTitleShadow;

	public SpriteFont font;

	public float X
	{
		get
		{
			return _X;
		}
		set
		{
			_X = value;
			stringTitle.X = _X;
			stringTitleShadow.X = _X;
		}
	}

	public float Y
	{
		get
		{
			return _Y;
		}
		set
		{
			_Y = value;
			stringTitle.Y = _Y;
			stringTitleShadow.Y = _Y + 3f;
		}
	}

	public float scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			stringTitle.scale.X = _scale;
			stringTitle.scale.Y = _scale;
			stringTitleShadow.scale.X = _scale;
			stringTitleShadow.scale.Y = _scale;
		}
	}

	public bool visible
	{
		set
		{
			stringTitle.visible = value;
			stringTitleShadow.visible = value;
		}
	}

	public byte alpha
	{
		set
		{
			stringTitle.color.A = value;
			stringTitleShadow.color.A = value;
		}
	}

	public DialogMenuOption(string xTitle, Dialog.DialogDelegate oAction)
	{
		title = xTitle;
		action = oAction;
	}

	public DialogMenuOption(string xTitle, Dialog.DialogDelegate oAction, object oData)
	{
		title = xTitle;
		action = oAction;
		data = oData;
	}

	public DialogMenuOption(string xTitle, Dialog.DialogDelegate oAction, object oData, SpriteFont oFont)
	{
		title = xTitle;
		action = oAction;
		data = oData;
		font = oFont;
	}

	public void Load(DialogManager oManager)
	{
		if (font == null)
		{
			font = oManager.fontKA_30;
		}
		Load(oManager.spriteManager, font);
	}

	public void Load(SpriteManager oSpriteManager, SpriteFont oFont)
	{
		stringTitleShadow = new SpriteString(oSpriteManager, oFont, title, 0f);
		stringTitle = new SpriteString(oSpriteManager, oFont, title, 0f);
		stringTitle.origin = new Vector2(stringTitle.width * 0.5f, stringTitle.height * 0.5f);
		stringTitleShadow.origin = new Vector2(stringTitleShadow.width * 0.5f, stringTitleShadow.height * 0.5f);
	}

	public void Dispose()
	{
		stringTitle.Dispose();
		stringTitleShadow.Dispose();
		action = null;
		data = null;
	}

	public void SetState(bool xSelected)
	{
		if (xSelected)
		{
			stringTitle.color = COLOR_SELECTED;
			stringTitleShadow.color = COLOR_SELECTED_SHADOW;
			scale = 1f;
		}
		else if (selected)
		{
			stringTitle.color = COLOR_SELECTED;
			stringTitleShadow.color = COLOR_SELECTED_SHADOW;
			scale = 1f;
		}
		else
		{
			stringTitle.color = COLOR_DEFAULT;
			stringTitleShadow.color = COLOR_DEFAULT_SHADOW;
			scale = 0.7f;
		}
		if (deactivated)
		{
			stringTitle.color.R /= 3;
			stringTitle.color.G /= 3;
			stringTitle.color.B /= 3;
			stringTitle.color.A /= 3;
			stringTitleShadow.color.R /= 3;
			stringTitleShadow.color.G /= 3;
			stringTitleShadow.color.B /= 3;
			stringTitleShadow.color.A /= 3;
		}
	}

	public void SetTitle(string xTitle)
	{
		title = xTitle;
		stringTitle.SetText(title);
		stringTitleShadow.SetText(title);
	}
}
