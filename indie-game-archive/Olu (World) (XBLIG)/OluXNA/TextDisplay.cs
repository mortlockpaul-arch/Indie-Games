using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class TextDisplay : Enemy
{
	private string text;

	private float progress;

	private float speed;

	private Vector2 boxStart;

	private Vector2 boxEnd;

	private static StretchTex windowBox;

	private string buttonText;

	private Vector2 buttonOrigin;

	private Vector2 buttonPos;

	private bool requireButton;

	private float introLength;

	public float TransitionLength => introLength;

	public float DisplayProgress => progress;

	public TextDisplay()
	{
		progress = 0f;
	}

	public TextDisplay(string _text, float _length)
		: this(_text, _length, _requireButton: true)
	{
	}

	public TextDisplay(string _text, float _length, bool _requireButton)
		: this(_text, _length, _requireButton, 0.2f)
	{
	}

	public TextDisplay(string _text, float _length, bool _requireButton, float _introLength)
		: this(_text, _length, _requireButton, _introLength, center: false)
	{
	}

	public TextDisplay(string _text, float _length, bool _requireButton, float _introLength, bool center)
	{
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		text = BaseGame.WrapString(_text.Replace("\\n", "\n"), 0.8f * (float)BaseGame.WIDTH, HUD.textScale, BaseGame.Get().hud.BigHUDfont);
		speed = 1f / _length;
		if (!center)
		{
			boxStart = new Vector2(0.1f * (float)BaseGame.WIDTH, 0.35f * (float)BaseGame.HEIGHT);
			boxEnd = new Vector2(0.9f * (float)BaseGame.WIDTH, boxStart.Y + BaseGame.Get().hud.BigHUDfont.MeasureString(text).Y * HUD.textScale);
			Vector2 val = (boxEnd - boxStart) / 2f;
			ref Vector2 reference = ref boxStart;
			reference.Y -= val.Y;
			ref Vector2 reference2 = ref boxEnd;
			reference2.Y -= val.Y;
		}
		else
		{
			boxStart = new Vector2(0.5f * (float)BaseGame.WIDTH, 0.5f * (float)BaseGame.HEIGHT);
			boxEnd = boxStart;
			boxStart -= BaseGame.Get().hud.BigHUDfont.MeasureString(text) * HUD.textScale / 2f;
			boxEnd += boxEnd - boxStart;
		}
		buttonText = BaseGame.Get().hud.KeyMap[(Buttons)4096];
		buttonPos = boxEnd - new Vector2(35f, -15f);
		buttonOrigin = BaseGame.Get().hud.ControllerFont.MeasureString(buttonText);
		buttonOrigin.X /= 2f;
		buttonOrigin.Y /= 2f;
		requireButton = _requireButton;
		introLength = _introLength;
	}

	public TextDisplay(Dictionary<string, string> attributes, XmlNode node)
		: this(attributes.ContainsKey("text") ? attributes["text"] : "", LevelLoader.GetFloatFromAtt(attributes, "length", -1f))
	{
	}

	public static void LoadGraphics()
	{
		windowBox = new StretchTex();
		windowBox.Initialize(9, 12, 9, 12, "Content\\WindowTex");
	}

	public override void draw(GameTime gametime)
	{
	}

	public void BatchDraw(GameTime gametime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.BigHUDfont, text, boxStart, Color.White, 0f, Vector2.Zero, HUD.textScale, (SpriteEffects)0, 0f);
		if (requireButton)
		{
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.ControllerFont, buttonText, buttonPos, Color.White, 0f, buttonOrigin, HUD.textScale, (SpriteEffects)0, 0f);
		}
	}

	public override void act(GameTime gametime)
	{
		if (exists)
		{
			if (speed > 0f)
			{
				progress += (float)gametime.ElapsedGameTime.TotalSeconds * speed;
			}
			if (progress > 1f - introLength && requireButton)
			{
				progress = 0.999f - introLength;
			}
			if (progress > 1f)
			{
				progress = 1f;
			}
			BaseGame.Get().channels[8] = 0.6f;
			if (progress < introLength)
			{
				BaseGame.Get().channels[8] = progress * 0.6f / introLength;
			}
			if (progress > 1.01f - introLength)
			{
				BaseGame.Get().channels[8] = (1f - progress) * 0.6f / introLength;
			}
			if (progress >= 1f && !requireButton)
			{
				leave();
			}
			if (requireButton && (BaseGame.Get().input.PadPressed((Buttons)4096) || BaseGame.Get().input.KeyPressed((Keys)13)))
			{
				requireButton = false;
				progress = 1.001f - introLength;
			}
		}
	}

	public override void start()
	{
		base.start();
		BaseGame.Get().tdColl.tDisplay.Add(this);
	}

	public override string name()
	{
		return "[view 0xCAFE]";
	}

	public override void die()
	{
		BaseGame.Get().channels[8] = 0f;
		BaseGame.Get().tdColl.tDisplay.Remove(this);
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().channels[8] = 0f;
		BaseGame.Get().tdColl.tDisplay.Remove(this);
		base.leave();
	}
}
