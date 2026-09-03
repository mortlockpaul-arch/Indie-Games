using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class TextDisplay2 : Enemy
{
	private bool curLineType;

	private string curText;

	private float progress;

	private float speed;

	private List<string> textBuff;

	private string toDraw;

	private int linesAvail;

	private int lineCount;

	private Vector2 boxStart;

	private Vector2 boxEnd;

	private float introLength;

	private float endLength;

	private float charCountdown;

	private float charMax;

	private float fogStart;

	private bool done;

	public float TransitionLength => introLength;

	public float DisplayProgress => progress;

	public TextDisplay2()
	{
		progress = 0f;
		charMax = BaseGame.BEAT * 0.5f;
		done = false;
		linesAvail = 10;
		lineCount = 0;
	}

	public TextDisplay2(string _text, float _length)
		: this(_text, _length, _requireButton: true)
	{
	}

	public TextDisplay2(string _text, float _length, bool _requireButton)
		: this(_text, _length, _requireButton, 0.2f)
	{
	}

	public TextDisplay2(string _text, float _length, bool _requireButton, float _introLength)
		: this(_text, _length, _requireButton, _introLength, center: false)
	{
	}

	public TextDisplay2(string _text, float _length, bool _requireButton, float _introLength, bool center)
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		this._002Ector();
		textBuff = new List<string>();
		textBuff.AddRange(_text.Replace("\\n", "\n").Split('\n'));
		speed = 1f / _length;
		boxStart = new Vector2(0.5f * (float)BaseGame.WIDTH, 0.5f * (float)BaseGame.HEIGHT);
		boxEnd = boxStart;
		boxStart -= new Vector2(0.3f * (float)BaseGame.WIDTH, 0.25f * (float)BaseGame.HEIGHT);
		boxEnd += boxEnd - boxStart;
		introLength = _introLength;
		LoadLine();
	}

	public TextDisplay2(Dictionary<string, string> attributes, XmlNode node)
		: this(attributes.ContainsKey("text") ? attributes["text"] : "", LevelLoader.GetFloatFromAtt(attributes, "length", -1f))
	{
	}

	public override void draw(GameTime gametime)
	{
	}

	public void BatchDraw(GameTime gametime)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.BigHUDfont, toDraw, boxStart, Color.White, 0f, Vector2.Zero, HUD.textScale, (SpriteEffects)0, 0f);
	}

	public override void act(GameTime gametime)
	{
		if (!exists)
		{
			return;
		}
		BaseGame.Get().channels[8] = fogStart;
		charCountdown -= (float)gametime.ElapsedGameTime.TotalSeconds;
		if (!done)
		{
			if (curText.Length == 0 && textBuff.Count == 0)
			{
				charCountdown += charMax * 6f;
				done = true;
			}
			else if (charCountdown < 0f)
			{
				if (curText.Length == 0)
				{
					LoadLine();
				}
				else if (curLineType)
				{
					BaseGame.Get().PlayCue("hatType");
					toDraw += curText.Substring(0, 1);
					curText = curText.Substring(1);
				}
				else
				{
					toDraw += curText;
					curText = "";
				}
				charCountdown += charMax;
			}
		}
		else if (charCountdown <= 0f)
		{
			leave();
		}
	}

	public void LoadLine()
	{
		if (curLineType)
		{
			BaseGame.Get().PlayCue("clap_2");
		}
		curText = textBuff[0];
		textBuff.RemoveAt(0);
		curLineType = false;
		if (curText[0] == '*')
		{
			curLineType = true;
			curText = curText.Substring(1);
		}
		toDraw += "\n";
		if (!curLineType)
		{
			toDraw += ">";
		}
		lineCount++;
		if (lineCount > linesAvail)
		{
			toDraw = toDraw.Substring(toDraw.IndexOf('\n') + 1);
		}
		if (curLineType)
		{
			charCountdown += charMax * 6f;
		}
		else
		{
			charCountdown += charMax * 2f;
		}
	}

	public override void start()
	{
		base.start();
		fogStart = BaseGame.Get().channels[8];
		BaseGame.Get().tdColl2.tDisplay.Add(this);
	}

	public override string name()
	{
		return "[view 0xCAFE]";
	}

	public override void die()
	{
		BaseGame.Get().tdColl2.tDisplay.Remove(this);
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().tdColl2.tDisplay.Remove(this);
		base.leave();
	}
}
