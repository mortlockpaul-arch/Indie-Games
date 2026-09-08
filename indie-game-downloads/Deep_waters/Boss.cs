using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Deep_waters;

public class Boss : CutScene
{
	public List<quicktime> QTE;

	public bossState bstate;

	private float timer;

	private float targettimer = 480f;

	private Random random;

	private int qtAnim;

	private int qtSucces;

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

	private string currentqtime;

	private int timerbut;

	private bool isvisible;

	private int levelnumber = 1;

	private string currentsuccess;

	public Vector3 boss;

	public Vector3 head;

	public bool Splash;

	public bool bleed;

	public bool scream;

	private bool spawnsplash;

	private bool screamspawn;

	private bool bleedspawn;

	private bool spawnsplash2;

	public bool Splash2;

	public Boss(PlayerIndex pi, List<Actor> actors, List<Texture2D> buttons, List<quicktime> QTEvents, int qtanim, int qtsuccess, string initialAnim, bool isloop, float animspeed, int leveln)
		: base(actors, initialAnim, isloop, 1f)
	{
		levelnumber = leveln;
		playerindex = pi;
		Buttons_text = buttons;
		qtAnim = qtanim;
		qtSucces = qtsuccess;
		QTE = QTEvents;
		random = new Random();
	}

	public void update(GameTime gameTime)
	{
		boss = Actors[2].animationController.GetBoneAbsoluteTransform("CATRigHub004Bone002").Translation * 0.05f;
		head = Actors[1].animationController.GetBoneAbsoluteTransform("BaseNeck2").Translation * 0.05f;
		if (bstate == bossState.Intro && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Intro"])
		{
			if (levelnumber == 2)
			{
				if (!spawnsplash && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 13, 860))
				{
					Splash = true;
					spawnsplash = true;
				}
				if (!screamspawn && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 13, 860))
				{
					screamspawn = true;
					scream = true;
				}
				if (!spawnsplash2 && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 15, 460))
				{
					Splash2 = true;
					spawnsplash2 = true;
				}
			}
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				changeAnimation("Idle", isloop: true, 1f);
				bstate = bossState.Idle;
				return;
			}
		}
		if (bstate == bossState.Idle)
		{
			screamspawn = false;
			spawnsplash = false;
			bleedspawn = false;
			spawnsplash2 = false;
			if (Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Idle"])
			{
				bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
				bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
				if (!(timer < targettimer))
				{
					int num = random.Next(0, qtAnim + 1);
					currentqtime = "Quicktime" + (int)MathHelper.Clamp(num, 1f, qtAnim);
					changeAnimation(currentqtime, isloop: false, 1f);
					bstate = bossState.Quicktime;
					timer = 0f;
					targettimer = 480 + random.Next(-100, 100);
					if (QTE.Count <= 0)
					{
						return;
					}
					if (QTE[0] == quicktime.single)
					{
						singleButton = random.Next(0, 8);
						singleButton = (int)MathHelper.Clamp(singleButton, 0f, 7f);
					}
					else if (QTE[0] == quicktime.combo)
					{
						int num2 = random.Next(3, 4);
						combobutton = new List<int>();
						positionsb = new List<Vector2>();
						for (int i = 0; i < num2; i++)
						{
							int num3 = random.Next(0, 8);
							num3 = (int)MathHelper.Clamp(num3, 0f, 7f);
							combobutton.Add(num3);
							Vector2 item = new Vector2(640 - num2 / 2 * Buttons_text[0].Width + Buttons_text[0].Width * i, 360 - Buttons_text[0].Height);
							positionsb.Add(item);
						}
					}
					else if (QTE[0] == quicktime.mash)
					{
						singleButton = random.Next(0, 8);
						singleButton = (int)MathHelper.Clamp(singleButton, 0f, 7f);
						mashtarget = 100 + random.Next(100);
					}
					return;
				}
				timer += gameTime.ElapsedGameTime.Milliseconds;
			}
		}
		if (bstate == bossState.Quicktime && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips[currentqtime])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				if (QTE[0] == quicktime.single)
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
				if (QTE[0] == quicktime.combo)
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
				if (QTE[0] == quicktime.mash)
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
					int num4 = random.Next(0, qtAnim + 1);
					currentsuccess = "Success" + (int)MathHelper.Clamp(num4, 1f, qtSucces);
					changeAnimation(currentsuccess, isloop: false, 1f);
					bstate = bossState.Success;
					QTE.Remove(QTE[0]);
					showQuicktime = false;
				}
				else
				{
					changeAnimation("Death", isloop: false, 0.7f);
					bstate = bossState.Death;
					showQuicktime = false;
				}
				return;
			}
			buttonpressed = -1;
			newstate = GamePad.GetState(playerindex);
			if (newstate.IsButtonDown(Buttons.LeftThumbstickUp) && oldstate.IsButtonUp(Buttons.LeftThumbstickUp))
			{
				buttonpressed = 0;
			}
			if (newstate.IsButtonDown(Buttons.LeftThumbstickLeft) && oldstate.IsButtonUp(Buttons.LeftThumbstickLeft))
			{
				buttonpressed = 1;
			}
			if (newstate.IsButtonDown(Buttons.LeftThumbstickRight) && oldstate.IsButtonUp(Buttons.LeftThumbstickRight))
			{
				buttonpressed = 2;
			}
			if (newstate.IsButtonDown(Buttons.LeftThumbstickDown) && oldstate.IsButtonUp(Buttons.LeftThumbstickDown))
			{
				buttonpressed = 3;
			}
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
					if (QTE[0] == quicktime.single && singleButton != -1 && buttonpressed == singleButton)
					{
						singleButton = -1;
					}
					if (QTE[0] == quicktime.combo && buttonpressed == combobutton[0])
					{
						combobutton.Remove(combobutton[0]);
						positionsb = new List<Vector2>();
						for (int j = 0; j < combobutton.Count; j++)
						{
							int num5 = random.Next(0, 4);
							num5 = (int)MathHelper.Clamp(num5, 0f, 3f);
							Vector2 item2 = new Vector2(640 - combobutton.Count / 2 * Buttons_text[0].Width + Buttons_text[0].Width * j, 360 - Buttons_text[0].Height);
							positionsb.Add(item2);
						}
					}
					if (QTE[0] == quicktime.mash && buttonpressed == singleButton)
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
		if (bstate == bossState.Death && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Death"])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (levelnumber == 1)
			{
				if (!spawnsplash && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 1, 3))
				{
					Splash = true;
					spawnsplash = true;
				}
				if (!screamspawn && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 2, 8))
				{
					screamspawn = true;
					scream = true;
				}
			}
			if (levelnumber == 2 && !bleedspawn && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 1, 60))
			{
				bleed = true;
				bleedspawn = true;
			}
			if (Actors[0].animationController.HasFinished)
			{
				gotogameover = true;
				bstate = bossState.Null;
				return;
			}
		}
		if (bstate == bossState.Success && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips[currentsuccess])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				if (QTE.Count > 0)
				{
					changeAnimation("Idle", isloop: true, 1f);
					bstate = bossState.Idle;
				}
				else
				{
					changeAnimation("Win", isloop: false, 1f);
					bstate = bossState.Win;
				}
				return;
			}
			if (levelnumber == 1)
			{
				if (currentsuccess == "Success1")
				{
					if (!spawnsplash && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 0, 400))
					{
						Splash = true;
						spawnsplash = true;
					}
				}
				else if (!spawnsplash && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 0, 200))
				{
					Splash = true;
					spawnsplash = true;
				}
			}
			if (levelnumber == 2 && currentsuccess == "Success1")
			{
				if (!spawnsplash && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 0, 460))
				{
					Splash = true;
					spawnsplash = true;
				}
				if (!spawnsplash2 && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 1, 500))
				{
					Splash2 = true;
					spawnsplash2 = true;
				}
			}
		}
		if (bstate == bossState.Win && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Win"])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				changeAnimation("Nextlevel", isloop: false, 1f);
				bstate = bossState.Nextlevel;
				return;
			}
		}
		if (bstate == bossState.Nextlevel && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Nextlevel"])
		{
			bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
			bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
			if (Actors[0].animationController.HasFinished)
			{
				bstate = bossState.Null;
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
		if (QTE[0] == quicktime.single && singleButton != -1)
		{
			sb.Draw(Buttons_text[singleButton], new Vector2(640 - Buttons_text[singleButton].Width / 2, 360 - Buttons_text[singleButton].Height / 2), Color.White);
		}
		if (QTE[0] == quicktime.combo)
		{
			for (int i = 0; i < combobutton.Count; i++)
			{
				sb.Draw(Buttons_text[combobutton[i]], positionsb[i], Color.White);
			}
		}
		if (QTE[0] != quicktime.mash)
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
