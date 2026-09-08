using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Deep_waters;

public class CutSceneAction : CutScene
{
	public quicktime QTE;

	public Action bstate;

	private Random random;

	public bool Success = true;

	public bool gotonextlevel;

	public bool gotogameover;

	private bool showQuicktime;

	public int singleButton;

	public List<int> combobutton;

	public int mashtarget = 100;

	public List<Texture2D> Buttons_text;

	private int buttonpressed = -1;

	private PlayerIndex playerindex;

	private GamePadState newstate;

	private GamePadState oldstate;

	private List<Vector2> positionsb;

	private int timerbut;

	private bool isvisible;

	private string currentsuccess;

	private int levelnumber;

	public CutSceneAction(PlayerIndex pi, List<Actor> actors, List<Texture2D> buttons, quicktime QTEvents, string initialAnim, bool isloop, int leveln)
		: base(actors, initialAnim, isloop, 1f)
	{
		levelnumber = leveln;
		playerindex = pi;
		Buttons_text = buttons;
		QTE = QTEvents;
		random = new Random();
	}

	public void update(GameTime gameTime)
	{
		if (bstate == Action.Intro && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Intro"])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				changeAnimation("QuickTime", isloop: false, 1f);
				bstate = Action.Quicktime;
				if (QTE == quicktime.single)
				{
					singleButton = random.Next(4, 8);
					singleButton = (int)MathHelper.Clamp(singleButton, 4f, 7f);
				}
				else if (QTE == quicktime.combo)
				{
					int num = random.Next(3, 4);
					combobutton = new List<int>();
					positionsb = new List<Vector2>();
					for (int i = 0; i < num; i++)
					{
						int num2 = random.Next(4, 8);
						num2 = (int)MathHelper.Clamp(num2, 4f, 7f);
						combobutton.Add(num2);
						Vector2 item = new Vector2(640 - num / 2 * Buttons_text[0].Width + Buttons_text[0].Width * i, 360 - Buttons_text[0].Height);
						positionsb.Add(item);
					}
				}
				else if (QTE == quicktime.mash)
				{
					singleButton = random.Next(4, 8);
					singleButton = (int)MathHelper.Clamp(singleButton, 4f, 7f);
					mashtarget = 100 + random.Next(100);
				}
				return;
			}
		}
		if (bstate == Action.Quicktime && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["QuickTime"])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				if (QTE == quicktime.single)
				{
					if (singleButton == -1)
					{
						Success = true;
					}
					else
					{
						Success = false;
					}
				}
				if (QTE == quicktime.combo)
				{
					if (combobutton.Count > 0)
					{
						Success = false;
					}
					else
					{
						Success = true;
					}
				}
				if (QTE == quicktime.mash)
				{
					if (mashtarget <= 0)
					{
						mashtarget = 100;
						Success = true;
					}
					else
					{
						Success = false;
					}
				}
				if (Success)
				{
					currentsuccess = "Success";
					changeAnimation(currentsuccess, isloop: false, 1f);
					bstate = Action.Success;
					showQuicktime = false;
				}
				else
				{
					changeAnimation("Death", isloop: false, 1f);
					bstate = Action.Death;
					showQuicktime = false;
				}
				return;
			}
			buttonpressed = -1;
			newstate = GamePad.GetState(playerindex);
			if (newstate.IsButtonDown(Buttons.A) && oldstate.IsButtonUp(Buttons.A))
			{
				buttonpressed = 4;
			}
			if (newstate.IsButtonDown(Buttons.B) && oldstate.IsButtonUp(Buttons.B))
			{
				buttonpressed = 5;
			}
			if (newstate.IsButtonDown(Buttons.X) && oldstate.IsButtonUp(Buttons.X))
			{
				buttonpressed = 6;
			}
			if (newstate.IsButtonDown(Buttons.Y) && oldstate.IsButtonUp(Buttons.Y))
			{
				buttonpressed = 7;
			}
			if (Success)
			{
				if (buttonpressed != -1)
				{
					if (QTE == quicktime.single && singleButton != -1 && buttonpressed == singleButton)
					{
						singleButton = -1;
					}
					if (QTE == quicktime.combo && buttonpressed == combobutton[0])
					{
						combobutton.Remove(combobutton[0]);
						positionsb = new List<Vector2>();
						for (int j = 0; j < combobutton.Count; j++)
						{
							int num3 = random.Next(0, 4);
							num3 = (int)MathHelper.Clamp(num3, 0f, 3f);
							Vector2 item2 = new Vector2(640 - combobutton.Count / 2 * Buttons_text[0].Width + Buttons_text[0].Width * j, 360 - Buttons_text[0].Height);
							positionsb.Add(item2);
						}
					}
					if (QTE == quicktime.mash && buttonpressed == singleButton)
					{
						mashtarget -= 10;
					}
				}
				showQuicktime = true;
			}
			else
			{
				showQuicktime = false;
			}
			oldstate = newstate;
		}
		if (bstate == Action.Death && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Death"])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				gotogameover = true;
				bstate = Action.Null;
				return;
			}
		}
		if (bstate == Action.Success && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips[currentsuccess])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				bstate = Action.Null;
				gotonextlevel = true;
			}
		}
	}

	public void draw(SpriteBatch sb, GameTime gameTime)
	{
		if (!showQuicktime)
		{
			return;
		}
		if (QTE == quicktime.single && singleButton != -1)
		{
			sb.Draw(Buttons_text[singleButton], new Vector2(640 - Buttons_text[singleButton].Width / 2, 360 - Buttons_text[singleButton].Height / 2), Color.White);
		}
		if (QTE == quicktime.combo)
		{
			for (int i = 0; i < combobutton.Count; i++)
			{
				sb.Draw(Buttons_text[combobutton[i]], positionsb[i], Color.White);
			}
		}
		if (QTE != quicktime.mash)
		{
			return;
		}
		if (isvisible)
		{
			if (timerbut < 100)
			{
				timerbut += gameTime.ElapsedGameTime.Milliseconds;
			}
			else
			{
				isvisible = false;
				timerbut = 0;
			}
			sb.Draw(Buttons_text[singleButton], new Vector2(640 - Buttons_text[singleButton].Width / 2, 360 - Buttons_text[singleButton].Height / 2), Color.White);
		}
		else if (timerbut < 160)
		{
			timerbut += gameTime.ElapsedGameTime.Milliseconds;
		}
		else
		{
			isvisible = true;
			timerbut = 0;
		}
	}
}
