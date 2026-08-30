using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TechArts;

namespace MADRISM
{
	internal class Parts : TaskObj
	{
		private const int RepeatTime = 8;

		private const int RepeatTimeFast = 2;

		private const int RepeatTimeToFast = 16;

		public const int UnitSize = 24;

		internal PlayState.UnitKind kind;

		internal int variation;

		private int MoveDistance;

		private bool bDone;

		private bool bDrop;

		private int nDestroy;

		private Vector2 pos;

		private Vector2 posd;

		private Vector2 size;

		private float area;

		private float downspd;

		private float dropspd;

		internal List<Parts> equipments;

		internal List<Parts> roomlink;

		private int Lcnt;

		private int Rcnt;

		private int Ucnt;

		private int Dcnt;

		private int Lrep;

		private int Rrep;

		private int Urep;

		private int Drep;

		public static bool VP_A;

		public static bool VP_Left;

		public static bool VP_Right;

		public static bool VP_Up;

		public static bool VP_Down;

		public static bool VP_RotL;

		public static bool VP_RotR;

		private float angle;

		private float scale;

		private float tangle;

		private float dangle;

		private bool bRL;

		private bool bRR;

		internal Rectangle rect
		{
			get
			{
				int num = Math.Abs((int)angle % 180);
				if (num < 90)
				{
					return new Rectangle((int)(pos.X - size.X / 2f), (int)(pos.Y - size.Y / 2f), (int)size.X, (int)size.Y);
				}
				return new Rectangle((int)(pos.X - size.Y / 2f), (int)(pos.Y - size.X / 2f), (int)size.Y, (int)size.X);
			}
		}

		internal float Area
		{
			get
			{
				return area;
			}
		}

		internal float Jyo
		{
			get
			{
				return area / 1.65f;
			}
		}

		private float radian(float n)
		{
			return n * 3.141596f / 180f;
		}

		private void moveX(int n)
		{
			pos.X += n;
			posd.X += n;
		}

		private void moveY(int n)
		{
			pos.Y += n;
			posd.Y += n;
		}

		private Vector2 setDropPos()
		{
			return new Vector2(624f, 360f);
		}

		private Vector2 setSize()
		{
			if (kind == PlayState.UnitKind.Door)
			{
				return new Vector2(48f, 56f);
			}
			if (kind == PlayState.UnitKind.Toilet)
			{
				return new Vector2(24f, 48f);
			}
			if (kind == PlayState.UnitKind.Kitchen)
			{
				return new Vector2(24f, 72f);
			}
			return new Vector2(GameEngine.core.rnd.Next(4) * 2 * 24 + 96, GameEngine.core.rnd.Next(4) * 2 * 24 + 96);
		}

		private float calcArea()
		{
			return size.X / 24f * (size.Y / 24f) * 0.275f;
		}

		public Parts(PlayState.UnitKind k, float dwspd, Vector2? osize)
		{
			kind = k;
			downspd = dwspd;
			dropspd = 0.7f;
			dangle = (tangle = (angle = 0f));
			scale = 7f;
			if (!osize.HasValue)
			{
				size = setSize();
			}
			else
			{
				size = osize.Value;
			}
			area = calcArea();
			pos = setDropPos();
			posd = default(Vector2);
			bRR = (bRL = false);
			Lcnt = (Rcnt = (Ucnt = (Dcnt = 0)));
			Lrep = (Rrep = (Urep = (Drep = 8)));
			MoveDistance = 24;
			equipments = new List<Parts>();
			roomlink = new List<Parts>();
			nDestroy = -1;
			bDone = false;
			variation = 0;
			VP_A = (VP_Left = (VP_Right = (VP_Up = (VP_Down = (VP_RotL = (VP_RotR = false))))));
		}

		public bool IsDone()
		{
			return bDone;
		}

		public bool IsDrop()
		{
			return bDrop;
		}

		public bool IsActive()
		{
			return !(IsDone() | IsDrop());
		}

		public void Destroy()
		{
			foreach (Parts equipment in equipments)
			{
				equipment.Destroy();
			}
			if (nDestroy < 0)
			{
				nDestroy = GameEngine.core.rnd.Next(15) + 1;
				PlayState.core.nDestroyParts++;
			}
		}

		private bool InGame()
		{
			return !GlobalState.inAttract;
		}

		private bool IsPressed_A()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_A();
			}
			return VP_A;
		}

		private bool IsPressed_Left()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_Left();
			}
			return VP_Left;
		}

		private bool IsPressed_Right()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_Right();
			}
			return VP_Right;
		}

		private bool IsPressed_Up()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_Up();
			}
			return VP_Up;
		}

		private bool IsPressed_Down()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_Down();
			}
			return VP_Down;
		}

		private bool IsPressed_RotL()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_RotL();
			}
			return VP_RotL;
		}

		private bool IsPressed_RotR()
		{
			if (InGame())
			{
				return GameEngine.core.IsPressed_RotR();
			}
			return VP_RotR;
		}

		public override IEnumerator<int> Update()
		{
			if (InGame())
			{
				while (GameEngine.core.IsPressed_A())
				{
					yield return 0;
				}
			}
			while (IsActive())
			{
				if (IsPressed_A())
				{
					bDrop = true;
					PlayState.core.snd_drop.Play();
					break;
				}
				if (IsPressed_Left())
				{
					if (Lcnt % Lrep == 0)
					{
						moveX(-MoveDistance);
					}
					Lcnt++;
					if (Lcnt == 16)
					{
						Lrep = 2;
					}
				}
				else
				{
					Lcnt = 0;
					Lrep = 8;
				}
				if (IsPressed_Right())
				{
					if (Rcnt % Rrep == 0)
					{
						moveX(MoveDistance);
					}
					Rcnt++;
					if (Rcnt == 16)
					{
						Rrep = 2;
					}
				}
				else
				{
					Rcnt = 0;
					Rrep = 8;
				}
				if (IsPressed_Up())
				{
					if (Ucnt % Urep == 0)
					{
						moveY(-MoveDistance);
					}
					Ucnt++;
					if (Ucnt == 16)
					{
						Urep = 2;
					}
				}
				else
				{
					Ucnt = 0;
					Urep = 8;
				}
				if (IsPressed_Down())
				{
					if (Dcnt % Drep == 0)
					{
						moveY(MoveDistance);
					}
					Dcnt++;
					if (Dcnt == 16)
					{
						Drep = 2;
					}
				}
				else
				{
					Dcnt = 0;
					Drep = 8;
				}
				if (dangle == 0f)
				{
					if (IsPressed_RotL() & !bRL)
					{
						tangle = angle - 90f;
						dangle = -15f;
						bRL = true;
						PlayState.core.snd_rot.Play();
					}
					else if (IsPressed_RotR() & !bRR)
					{
						tangle = angle + 90f;
						dangle = 15f;
						PlayState.core.snd_rot.Play();
						bRR = true;
					}
				}
				if (bRL)
				{
					bRL = IsPressed_RotL();
				}
				if (bRR)
				{
					bRR = IsPressed_RotR();
				}
				yield return 0;
			}
			while (true)
			{
				yield return 0;
			}
		}

		private bool InOtherEquipments(Parts p, Parts q)
		{
			foreach (Parts equipment in p.equipments)
			{
				if (equipment.rect.Intersects(q.rect))
				{
					return true;
				}
			}
			return false;
		}

		private bool DirectionCheck(Parts r0, Parts r1, ref int dir)
		{
			int shogen = GetShogen(angle);
			float num = r1.pos.X - r0.pos.X;
			float num2 = r1.pos.Y - r0.pos.Y;
			bool flag = false;
			bool flag2 = false;
			if (num > 0f && r1.pos.X > (float)(r0.rect.X + r0.rect.Width))
			{
				flag = true;
			}
			if (num < 0f && r1.pos.X < (float)r0.rect.X)
			{
				flag = true;
			}
			if (num2 > 0f && r1.pos.Y > (float)(r0.rect.Y + r0.rect.Height))
			{
				flag2 = true;
			}
			if (num2 < 0f && r1.pos.Y < (float)r0.rect.Y)
			{
				flag2 = true;
			}
			dir = 0;
			if (flag)
			{
				if (num < 0f)
				{
					dir |= 1;
				}
				else
				{
					dir |= 2;
				}
			}
			if (flag2)
			{
				if (num2 < 0f)
				{
					dir |= 4;
				}
				else
				{
					dir |= 8;
				}
			}
			if (flag2 && flag)
			{
				return true;
			}
			if (flag2 && (shogen & 1) == 0)
			{
				return true;
			}
			if (flag && (shogen & 1) == 1)
			{
				return true;
			}
			return false;
		}

		private bool Dispose()
		{
			List<Parts> list = PlayState.core.InOtherParts(this);
			switch (kind)
			{
			case PlayState.UnitKind.Room:
				if (list.Count == 0)
				{
					return true;
				}
				return false;
			case PlayState.UnitKind.Toilet:
			case PlayState.UnitKind.Kitchen:
				if (list.Count == 1)
				{
					if (InOtherEquipments(list[0], this))
					{
						return false;
					}
					list[0].equipments.Add(this);
					PlayState.core.CheckDestroy(list[0], pos);
					return true;
				}
				return false;
			case PlayState.UnitKind.Door:
				if (list.Count == 2)
				{
					int dir = 0;
					if (!DirectionCheck(list[0], list[1], ref dir))
					{
						return false;
					}
					if (InOtherEquipments(list[0], this))
					{
						return false;
					}
					if (InOtherEquipments(list[1], this))
					{
						return false;
					}
					list[0].equipments.Add(this);
					list[1].equipments.Add(this);
					list[0].roomlink.Add(list[1]);
					list[1].roomlink.Add(list[0]);
					PlayState.core.CheckDestroy(list[0], pos);
					return true;
				}
				return false;
			default:
				return true;
			}
		}

		public void DestroyDirect()
		{
			GameEngine.core.particles.Entry(pos, (kind == PlayState.UnitKind.Room) ? 256 : 64);
			PlayState.core.exist.Remove(this);
			manager.Remove(this);
		}

		public override void PostUpdate()
		{
			if (!IsDone())
			{
				GameEngine.core.rnd.Next();
				int num = (int)((size.X > size.Y) ? size.X : size.Y) / 2;
				Rectangle safeArea = GameEngine.core.SafeArea;
				safeArea.X += 32;
				safeArea.Y += 32;
				safeArea.Width -= 64;
				safeArea.Height -= 64;
				if (pos.X - (float)safeArea.X < (float)(-num))
				{
					pos.X -= posd.X;
				}
				if (pos.X > (float)(num + (safeArea.X + safeArea.Width)))
				{
					pos.X -= posd.X;
				}
				if (pos.Y - (float)safeArea.Y < (float)(-num))
				{
					pos.Y -= posd.Y;
				}
				if (pos.Y > (float)(num + (safeArea.Y + safeArea.Height)))
				{
					pos.Y -= posd.Y;
				}
				scale -= downspd;
				if (IsDrop())
				{
					scale -= dropspd;
				}
				angle += dangle;
				if (dangle > 0f)
				{
					if (angle >= tangle)
					{
						dangle = 0f;
						angle = tangle;
					}
				}
				else if (dangle < 0f && angle <= tangle)
				{
					dangle = 0f;
					angle = tangle;
				}
				if (scale <= 1f)
				{
					scale = 1f;
					angle = tangle;
					dangle = 0f;
					bDone = true;
					if (!Dispose())
					{
						if (!bDrop)
						{
							PlayState.core.snd_drop.Play();
						}
						GameEngine.core.particles.Entry(pos, (kind == PlayState.UnitKind.Room) ? 512 : 32);
						manager.Remove(this);
						if (kind == PlayState.UnitKind.Room)
						{
							PlayState.core.snd_miss.Play();
							PlayState.core.ReqGameOver();
						}
						return;
					}
					PlayState.core.snd_drop2.Play();
					PlayState.core.exist.Add(this);
				}
			}
			posd.X = 0f;
			posd.Y = 0f;
			if (nDestroy >= 0 && --nDestroy == 0)
			{
				PlayState.core.nDestroyParts--;
				DestroyDirect();
			}
			if (!GlobalState.inState)
			{
				manager.Remove(this);
			}
		}

		private int GetShogen(float ang)
		{
			int num = 0;
			switch ((int)ang % 360)
			{
			case 90:
				return 1;
			case 180:
				return 2;
			case 270:
				return 3;
			default:
				return 0;
			}
		}

		public override void Draw()
		{
			byte alpha = byte.MaxValue;
			if (IsActive())
			{
				alpha = 32;
			}
			int shogen = GetShogen(angle);
			PlayState.core.DrawRoom(kind, pos, size, radian(angle), scale, alpha, shogen & 3);
			Rectangle r = rect;
			if (kind == PlayState.UnitKind.Room)
			{
				GameEngine.core.FillRect(r, new Color(48, 48, 48, 16));
			}
			if (IsActive())
			{
				if (GameEngine.core.vcount % 30 >= 10)
				{
					PlayState.core.DrawRoom(kind, pos, size, radian(angle), 1f, 72, shogen & 3);
				}
			}
			else if (scale > 1f)
			{
				PlayState.core.DrawRoom(kind, pos, size, radian(angle), 1f, 72, shogen & 3);
			}
		}
	}
}
