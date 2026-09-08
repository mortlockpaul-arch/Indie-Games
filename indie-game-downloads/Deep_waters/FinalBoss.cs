using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Deep_waters;

public class FinalBoss : CutScene
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

	public int mashtarget = 50;

	public List<Texture2D> Buttons_text;

	private int buttonpressed = -1;

	private PlayerIndex playerindex;

	private GamePadState newstate;

	private GamePadState oldstate;

	private List<Vector2> positionsb;

	private int actioncount;

	private string currentqtime;

	private int timerbut;

	private bool isvisible;

	private string currentsuccess;

	public bool explosion;

	public bool explosionspawn;

	public bool batt1spawn;

	public bool batt2spawn;

	public bool batt3spawn;

	public bool batt4spawn;

	public bool batt1;

	public bool batt2;

	public bool batt3;

	public bool batt4;

	public Vector3 bombola;

	public Vector3 boss;

	public Vector3 bosship;

	public bool scream;

	public bool Splash2;

	public bool Splash;

	private bool splashtrail;

	private bool spawnsplash;

	private bool screamspawn;

	public FinalBoss(PlayerIndex pi, List<Actor> actors, List<Texture2D> buttons, List<quicktime> QTEvents, string initialAnim, bool isloop, float animspeed)
		: base(actors, initialAnim, isloop, 1f)
	{
		playerindex = pi;
		Buttons_text = buttons;
		QTE = QTEvents;
		random = new Random();
	}

	public void update(GameTime gameTime)
	{
		bombola = Actors[3].animationController.GetBoneAbsoluteTransform("bombola").Translation * 0.05f;
		boss = Actors[2].animationController.GetBoneAbsoluteTransform("CATRigHub004Bone002").Translation * 0.05f;
		bosship = Actors[2].animationController.GetBoneAbsoluteTransform("CATRigHub003").Translation * 0.05f;
		bcpos = Actors[0].animationController.GetBoneAbsoluteTransform("campos");
		bclook = Actors[0].animationController.GetBoneAbsoluteTransform("camlookat");
		_ = Actors[0].animationController.HasFinished;
		if (bstate == bossState.Intro)
		{
			int num = random.Next(0, qtAnim + 1);
			num = (int)MathHelper.Clamp(num, 1f, qtAnim);
			actioncount++;
			currentqtime = "Quicktime" + actioncount;
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
				singleButton = random.Next(4, 8);
				singleButton = (int)MathHelper.Clamp(singleButton, 4f, 7f);
				mashtarget = 100 + random.Next(100);
			}
			return;
		}
		if (bstate == bossState.Success && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips[currentsuccess])
		{
			if (actioncount == 4 && Actors[0].animationController.Time > new TimeSpan(0, 0, 0, 0, 430) && !explosionspawn)
			{
				explosion = true;
				explosionspawn = true;
			}
			if (actioncount == 2 && !splashtrail)
			{
				Splash2 = true;
				splashtrail = true;
			}
			if (actioncount == 3 && !splashtrail)
			{
				Splash2 = true;
				splashtrail = true;
			}
			if (Actors[0].animationController.HasFinished)
			{
				if (QTE.Count > 0)
				{
					int num4 = random.Next(0, qtAnim + 1);
					num4 = (int)MathHelper.Clamp(num4, 1f, qtAnim);
					actioncount++;
					currentqtime = "Quicktime" + actioncount;
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
						int num5 = random.Next(3, 4);
						combobutton = new List<int>();
						positionsb = new List<Vector2>();
						for (int j = 0; j < num5; j++)
						{
							int num6 = random.Next(0, 8);
							num6 = (int)MathHelper.Clamp(num6, 0f, 7f);
							combobutton.Add(num6);
							Vector2 item2 = new Vector2(640 - num5 / 2 * Buttons_text[0].Width + Buttons_text[0].Width * j, 360 - Buttons_text[0].Height);
							positionsb.Add(item2);
						}
					}
					else if (QTE[0] == quicktime.mash)
					{
						singleButton = random.Next(4, 8);
						singleButton = (int)MathHelper.Clamp(singleButton, 4f, 7f);
						mashtarget = 100 + random.Next(100);
					}
				}
				else
				{
					changeAnimation("Ending", isloop: false, 0.75f);
					bstate = bossState.Win;
				}
				return;
			}
		}
		if (bstate == bossState.Quicktime)
		{
			splashtrail = false;
			if (Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips[currentqtime])
			{
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
						int num7 = random.Next(0, qtAnim + 1);
						num7 = (int)MathHelper.Clamp(num7, 1f, qtSucces);
						currentsuccess = "Success" + actioncount;
						changeAnimation(currentsuccess, isloop: false, 1f);
						bstate = bossState.Success;
						QTE.Remove(QTE[0]);
						showQuicktime = false;
					}
					else
					{
						changeAnimation("Death" + actioncount, isloop: false, 1f);
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
						if (QTE[0] == quicktime.combo && combobutton.Count > 0 && buttonpressed == combobutton[0])
						{
							combobutton.Remove(combobutton[0]);
							positionsb = new List<Vector2>();
							for (int k = 0; k < combobutton.Count; k++)
							{
								int num8 = random.Next(0, 4);
								num8 = (int)MathHelper.Clamp(num8, 0f, 3f);
								Vector2 item3 = new Vector2(640 - combobutton.Count / 2 * Buttons_text[0].Width + Buttons_text[0].Width * k, 360 - Buttons_text[0].Height);
								positionsb.Add(item3);
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
		}
		if (bstate == bossState.Death)
		{
			if (actioncount == 1)
			{
				if (!spawnsplash && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 3, 3))
				{
					Splash = true;
					spawnsplash = true;
				}
				if (!screamspawn && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 3, 3))
				{
					screamspawn = true;
					scream = true;
				}
			}
			if (actioncount == 2 && !screamspawn && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 1, 3))
			{
				screamspawn = true;
				scream = true;
			}
			if ((actioncount == 3 || actioncount == 4) && !screamspawn && Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 4, 3))
			{
				screamspawn = true;
				scream = true;
			}
			if (Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Death" + actioncount] && Actors[0].animationController.HasFinished)
			{
				gotogameover = true;
				bstate = bossState.Null;
				return;
			}
		}
		if (bstate == bossState.Win && Actors[0].animationController.AnimationClip == Actors[0].skinnedModel.AnimationClips["Ending"])
		{
			if (Actors[0].animationController.HasFinished)
			{
				bstate = bossState.Nextlevel;
				return;
			}
			if (Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 1, 0) && Actors[0].animationController.Time < new TimeSpan(0, 0, 0, 4, 0))
			{
				batt1 = true;
			}
			if (Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 4, 0) && Actors[0].animationController.Time < new TimeSpan(0, 0, 0, 7, 0))
			{
				batt2 = true;
				batt1 = false;
			}
			if (Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 7, 0) && Actors[0].animationController.Time < new TimeSpan(0, 0, 0, 10, 0))
			{
				batt3 = true;
				batt2 = false;
			}
			if (Actors[0].animationController.Time >= new TimeSpan(0, 0, 0, 10, 0))
			{
				batt3 = false;
			}
		}
		if (bstate == bossState.Nextlevel && Actors[0].animationController.HasFinished)
		{
			bstate = bossState.Null;
			gotonextlevel = true;
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
