using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TechArts;

namespace MADRISM
{
	internal class PlayState : TaskObj
	{
		internal enum Fonts
		{
			N0,
			N1,
			N2,
			N3,
			N4,
			N5,
			N6,
			N7,
			N8,
			N9,
			Comma,
			Point,
			Sou,
			Hyou,
			Ka,
			Gaku,
			Yuka,
			Men,
			Seki,
			Setu,
			Kou,
			Suu,
			Hei,
			Kin,
			Tubo,
			Tan,
			Yen,
			M2,
			L,
			D,
			K,
			T,
			MAX
		}

		internal enum Imgs
		{
			Toilet,
			Kitchen,
			Sentakuki,
			Door,
			GameOver,
			MAX
		}

		public enum UnitKind
		{
			Room,
			Toilet,
			Kitchen,
			Door,
			Sentakuki,
			MAX
		}

		public const float STDDRSPD = 0.7f;

		public const float STDDWSPD = 0.012f;

		private const int STDNEXTWAIT = 30;

		private const float GAMESPDFIRST = 0.5f;

		private const float GAMESPDDELTA = 0.00032f;

		private const float GAMESPDLIMIT = 7f;

		public static PlayState core;

		public SoundEffect snd_drop;

		public SoundEffect snd_drop2;

		public SoundEffect snd_miss;

		public SoundEffect snd_rot;

		public SoundEffect snd_vanish;

		public SoundEffect snd_disap;

		public Song bgm_gameover;

		private Song bgm_loop;

		public Texture2D[] fonts;

		private Texture2D[] imgs;

		private Texture2D gridtex;

		private Texture2D pen;

		private Texture2D white;

		internal List<Parts> exist;

		private int count;

		private bool bBGMLoop;

		private bool bGameOver;

		public int nDestroyParts;

		private float score;

		private float dscore;

		private float area;

		private float darea;

		private float dtanka;

		private float GameSpdRate;

		private string replayname;

		private int replaytime;

		private int RoomCount
		{
			get
			{
				int num = 0;
				foreach (Parts item in exist)
				{
					if (item.kind == UnitKind.Room)
					{
						num++;
					}
				}
				return num;
			}
		}

		private int DoorCount
		{
			get
			{
				int num = 0;
				foreach (Parts item in exist)
				{
					if (item.kind == UnitKind.Door)
					{
						num++;
					}
				}
				return num;
			}
		}

		private int KitchenCount
		{
			get
			{
				int num = 0;
				foreach (Parts item in exist)
				{
					if (item.kind == UnitKind.Kitchen)
					{
						num++;
					}
				}
				return num;
			}
		}

		private int ToiletCount
		{
			get
			{
				int num = 0;
				foreach (Parts item in exist)
				{
					if (item.kind == UnitKind.Toilet)
					{
						num++;
					}
				}
				return num;
			}
		}

		public void ReqGameOver()
		{
			bGameOver = true;
		}

		internal List<Parts> InOtherParts(Parts r)
		{
			List<Parts> list = new List<Parts>();
			bool flag = true;
			if (r.kind == UnitKind.Room || r.kind == UnitKind.Door)
			{
				flag = false;
			}
			foreach (Parts item in exist)
			{
				if (item.kind != UnitKind.Room)
				{
					continue;
				}
				if (flag)
				{
					if (item.rect.Contains(r.rect))
					{
						list.Add(item);
					}
				}
				else if (item.rect.Intersects(r.rect))
				{
					list.Add(item);
				}
			}
			return list;
		}

		internal bool InOtherParts(Rectangle r)
		{
			foreach (Parts item in exist)
			{
				if (item.kind == UnitKind.Room && item.rect.Intersects(r))
				{
					return true;
				}
			}
			return false;
		}

		private float radian(float n)
		{
			return n * 3.141596f / 180f;
		}

		private bool InGame()
		{
			return !GlobalState.inAttract;
		}

		internal PlayState(string repname, int reptime)
		{
			core = this;
			replayname = repname;
			replaytime = reptime;
			GameEngine.core.fader.Brightness = 1f;
			GameSpdRate = 0.5f;
			exist = new List<Parts>();
			pen = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/Pen");
			white = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/White");
			fonts = new Texture2D[32];
			fonts[0] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_0");
			fonts[1] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_1");
			fonts[2] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_2");
			fonts[3] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_3");
			fonts[4] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_4");
			fonts[5] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_5");
			fonts[6] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_6");
			fonts[7] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_7");
			fonts[8] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_8");
			fonts[9] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_9");
			fonts[10] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_comma");
			fonts[11] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_point");
			fonts[26] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_yen");
			fonts[27] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_m2");
			fonts[12] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_sou");
			fonts[13] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_hyou");
			fonts[14] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_ka");
			fonts[15] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_gaku");
			fonts[16] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_yuka");
			fonts[17] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_men");
			fonts[18] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_seki");
			fonts[19] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_setu");
			fonts[20] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_kou");
			fonts[21] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_suu");
			fonts[22] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_hei");
			fonts[23] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_kin");
			fonts[24] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_tubo");
			fonts[25] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_tan");
			fonts[28] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_L");
			fonts[29] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_D");
			fonts[30] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_K");
			fonts[31] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/font_T");
			imgs = new Texture2D[5];
			imgs[0] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/Toilet");
			imgs[1] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/Kitchen");
			imgs[2] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/Sentakuki");
			imgs[3] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/Door");
			imgs[4] = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/GameOver_logo");
			gridtex = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/grid");
			snd_drop = GameEngine.core.Content.Load<SoundEffect>("SE/drop");
			snd_drop2 = GameEngine.core.Content.Load<SoundEffect>("SE/drop2");
			snd_miss = GameEngine.core.Content.Load<SoundEffect>("SE/miss");
			snd_rot = GameEngine.core.Content.Load<SoundEffect>("SE/rot");
			snd_vanish = GameEngine.core.Content.Load<SoundEffect>("SE/vanish");
			snd_disap = GameEngine.core.Content.Load<SoundEffect>("SE/disap");
			bgm_loop = GameEngine.core.Content.Load<Song>("Sound/bgm_loop");
			bgm_gameover = GameEngine.core.Content.Load<Song>(InGame() ? "Sound/gameover_ne" : "Sound/gameover");
			if (InGame())
			{
				MediaPlayer.IsRepeating = false;
				MediaPlayer.Play(GameEngine.core.Content.Load<Song>("Sound/bgm_intro"));
			}
			GameEngine.core.fader.WithBGM = false;
			area = (score = 0f);
			darea = (dscore = (dtanka = 0f));
			bGameOver = false;
			nDestroyParts = 0;
		}

		private void drawLine(Texture2D img, Vector2 pos, Vector2 lw, float rangle, byte alpha)
		{
			Color color = new Color(byte.MaxValue, 0, 0, alpha);
			GameEngine.core.spriteBatch.Draw(img, pos, null, color, rangle, new Vector2(0f, 0f), new Vector2(lw.X / 16f, lw.Y / 16f), SpriteEffects.None, 1f);
		}

		private void drawBox(Texture2D img, Vector2 pos, Vector3 wht, float rangle, float scl, byte alpha, int area)
		{
			wht *= scl;
			if ((area & 1) == 1)
			{
				float x = wht.X;
				wht.X = wht.Y;
				wht.Y = x;
				rangle = 0f;
			}
			if (area == 2)
			{
				rangle = 0f;
			}
			Vector2 vector = new Vector2(wht.X / 2f, wht.Y / 2f);
			float z = wht.Z;
			Matrix matrix = Matrix.CreateFromAxisAngle(new Vector3(0f, 0f, 1f), rangle);
			Vector2 position = new Vector2(0f - vector.X, 0f - vector.Y);
			Vector2 position2 = new Vector2(0f - vector.X, 0f - vector.Y + z);
			Vector2 position3 = new Vector2(vector.X, 0f - vector.Y);
			Vector2 position4 = new Vector2(0f - vector.X, vector.Y);
			position = Vector2.Transform(position, matrix) + pos;
			position2 = Vector2.Transform(position2, matrix) + pos;
			position3 = Vector2.Transform(position3, matrix) + pos;
			position4 = Vector2.Transform(position4, matrix) + pos;
			drawLine(img, position, new Vector2(wht.X, z), rangle, alpha);
			drawLine(img, position2, new Vector2(z, wht.Y - z), rangle, alpha);
			drawLine(img, position4, new Vector2(wht.X, z), rangle, alpha);
			drawLine(img, position3, new Vector2(z, wht.Y + z), rangle, alpha);
		}

		internal void DrawRoom(UnitKind kind, Vector2 pos, Vector2 size, float angle, float scale, byte alpha, int area)
		{
			switch (kind)
			{
			case UnitKind.Room:
				drawBox(pen, pos, new Vector3(size.X, size.Y, 6f), angle, scale, alpha, area);
				break;
			case UnitKind.Toilet:
			{
				scale *= 0.3f;
				Texture2D texture2D4 = imgs[0];
				GameEngine.core.spriteBatch.Draw(texture2D4, pos, null, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha), angle, new Vector2(texture2D4.Width / 2, texture2D4.Height / 2), scale, SpriteEffects.None, 1f);
				break;
			}
			case UnitKind.Kitchen:
			{
				scale *= 0.3f;
				Texture2D texture2D3 = imgs[1];
				GameEngine.core.spriteBatch.Draw(texture2D3, pos, null, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha), angle, new Vector2(texture2D3.Width / 2, texture2D3.Height / 2), scale, SpriteEffects.None, 1f);
				break;
			}
			case UnitKind.Door:
			{
				scale *= 0.3f;
				Texture2D texture2D2 = imgs[3];
				GameEngine.core.spriteBatch.Draw(texture2D2, pos, null, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha), angle, new Vector2(texture2D2.Width / 2, texture2D2.Height / 2), scale, SpriteEffects.None, 1f);
				break;
			}
			case UnitKind.Sentakuki:
			{
				scale *= 0.25f;
				Texture2D texture2D = imgs[2];
				GameEngine.core.spriteBatch.Draw(texture2D, pos, null, new Color(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha), angle, new Vector2(texture2D.Width / 2, texture2D.Height / 2), scale, SpriteEffects.None, 1f);
				break;
			}
			}
		}

		private UnitKind GetKind()
		{
			int num = GameEngine.core.rnd.Next(10);
			int roomCount = RoomCount;
			int doorCount = DoorCount;
			int kitchenCount = KitchenCount;
			int toiletCount = ToiletCount;
			if (roomCount < 2)
			{
				return UnitKind.Room;
			}
			if (roomCount / 2 > doorCount)
			{
				if (num == 1)
				{
					return UnitKind.Kitchen;
				}
				if (num == 0)
				{
					return UnitKind.Toilet;
				}
				if (num >= 7)
				{
					return UnitKind.Room;
				}
				if (num >= 3)
				{
					return UnitKind.Door;
				}
			}
			switch (num)
			{
			case 1:
			case 3:
			case 5:
				return UnitKind.Kitchen;
			case 2:
			case 4:
			case 6:
				return UnitKind.Toilet;
			case 7:
			case 8:
				return UnitKind.Door;
			default:
				return UnitKind.Room;
			}
		}

		private int NextWait()
		{
			return (int)(30f / GameSpdRate);
		}

		private float DownSpeed()
		{
			return 0.012f * GameSpdRate;
		}

		public override IEnumerator<int> Update()
		{
			if (replayname != "")
			{
				GameEngine.core.BeginReplay(replayname);
				manager.Entry(new GameOverLogoProc(imgs[4], GameEngine.core.Content.Load<Texture2D>("Sprite/Title/PushAButton")));
			}
			while (GameEngine.core.IsPressed_A())
			{
				yield return 0;
			}
			for (int i = 0; i < 30; i++)
			{
				yield return 0;
			}
			while (!bGameOver)
			{
				Parts p;
				while (true)
				{
					if (InGame())
					{
						p = new Parts(GetKind(), DownSpeed(), null);
					}
					else
					{
						p = AttractState.nextParts;
						AttractState.nextParts = null;
					}
					if (p != null)
					{
						break;
					}
					yield return 0;
				}
				manager.Entry(p);
				while (!p.IsDone())
				{
					yield return 0;
				}
				if (nDestroyParts > 0)
				{
					while (nDestroyParts > 0)
					{
						yield return 0;
					}
					for (int j = 0; j < 30; j++)
					{
						yield return 0;
					}
				}
				for (int k = 0; k < NextWait(); k++)
				{
					yield return 0;
				}
			}
			manager.Entry(new GameOverProc(imgs[4]));
			while (!GlobalState.inDestroy)
			{
				yield return 0;
			}
			GlobalState.inState = false;
			manager.Remove(this);
		}

		public override void PostUpdate()
		{
			if (!InGame() && GlobalState.inDestroy)
			{
				GlobalState.inState = false;
				manager.Remove(this);
				return;
			}
			if (replaytime > 0)
			{
				if (GameEngine.core.IsPressed_A_Ctr())
				{
					replaytime = 0;
				}
				if (--replaytime <= 0)
				{
					GameEngine.core.EndReplay();
					GlobalState.inState = false;
					manager.Remove(this);
					return;
				}
			}
			if (!bGameOver)
			{
				GameEngine.core.fader.Brightness -= 1f / 60f;
			}
			if (InGame())
			{
				GameSpdRate += 0.00032f;
				if (GameSpdRate >= 7f)
				{
					GameSpdRate = 7f;
				}
			}
			dscore += (score - dscore) / 10f;
			darea += (area - darea) / 10f;
			if (++count >= 300 && !bBGMLoop && !bGameOver && MediaPlayer.State == MediaState.Stopped)
			{
				if (InGame())
				{
					MediaPlayer.IsRepeating = true;
					MediaPlayer.Play(bgm_loop);
				}
				bBGMLoop = true;
			}
		}

		private void CheckEquipments(Parts p, ref int nKitchen, ref int nToilet)
		{
			foreach (Parts equipment in p.equipments)
			{
				if (equipment.kind == UnitKind.Kitchen)
				{
					nKitchen++;
				}
				if (equipment.kind == UnitKind.Toilet)
				{
					nToilet++;
				}
			}
		}

		internal void CheckDestroy(Parts p, Vector2 pos)
		{
			List<Parts> list = new List<Parts>();
			list.Add(p);
			int num = 0;
			do
			{
				Parts parts = list[num];
				foreach (Parts item in parts.roomlink)
				{
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			while (list.Count > ++num);
			int nKitchen = 0;
			int nToilet = 0;
			foreach (Parts item2 in list)
			{
				CheckEquipments(item2, ref nKitchen, ref nToilet);
			}
			if (nKitchen < 1 || nToilet < 1)
			{
				return;
			}
			ScoreAdd(list, pos);
			snd_vanish.Play();
			snd_drop.Play();
			foreach (Parts item3 in list)
			{
				item3.Destroy();
			}
		}

		internal Texture2D GetFont(char n)
		{
			switch (n)
			{
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
				return fonts[n - 48];
			case 'L':
				return fonts[28];
			case 'D':
				return fonts[29];
			case 'T':
				return fonts[31];
			case 'K':
				return fonts[30];
			case '\\':
				return fonts[26];
			case ',':
				return fonts[10];
			default:
				return null;
			}
		}

		internal void ScoreDisp(Vector2 pos, string gradename, float points)
		{
			char[] array = gradename.ToCharArray();
			float num = (float)gradename.Length * 4f;
			num *= -0.5f;
			for (int i = 0; i < gradename.Length; i++)
			{
				Texture2D font = GetFont(array[i]);
				if (font != null)
				{
					manager.Entry(new ScoreDispFont(font, pos, num, 1f));
				}
				num += 4f;
			}
		}

		internal void ScoreAdd(List<Parts> rooms, Vector2 pos)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			bool flag2 = false;
			foreach (Parts room in rooms)
			{
				int nKitchen = 0;
				int nToilet = 0;
				CheckEquipments(room, ref nKitchen, ref nToilet);
				if (nToilet > 0)
				{
					num4 += nToilet;
					if (nKitchen > 0)
					{
						flag2 = true;
					}
				}
				else if (nKitchen > 0)
				{
					if (room.Jyo >= 6f)
					{
						num2++;
					}
					else
					{
						num3 += nKitchen;
					}
				}
				else if (room.Jyo >= 8f && !flag)
				{
					flag = true;
				}
				else
				{
					num++;
				}
			}
			float num5 = 1f;
			string text = num.ToString();
			text += (flag ? "L" : "");
			if (flag2)
			{
				text += "TK";
				num5 *= 0.1f;
				snd_disap.Play();
			}
			else
			{
				text += ((num2 > 0) ? "DK" : "K");
			}
			if (text == "0K")
			{
				text = "1K";
			}
			if (text == "0DK")
			{
				text = "1K";
			}
			if (text == "0LK")
			{
				text = "1K";
			}
			if (text == "0LDK")
			{
				text = "1DK";
			}
			if (text == "0TK")
			{
				text = "TK";
			}
			if (text == "0LTK")
			{
				text = "1TK";
			}
			if (text.EndsWith("DK"))
			{
				num5 *= 1.2f;
			}
			if (text.Contains('L'))
			{
				num5 *= 1.5f;
			}
			if (num >= 3)
			{
				num5 *= 1.5f;
			}
			if (num >= 5)
			{
				num5 *= 2f;
			}
			if (num4 >= 3 || num3 >= 3)
			{
				num5 *= 0.9f;
			}
			float num6 = 0f;
			foreach (Parts room2 in rooms)
			{
				num6 += room2.Area;
			}
			float num7 = num6 / 3.3f * 1500000f * num5;
			ScoreDisp(pos, text, num7);
			area += num6;
			score += num7;
		}

		private void DrawValue(ref Vector2 pos, float n, string f)
		{
			Color col = Color.White;
			string text = n.ToString(f);
			char[] array = text.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					GameEngine.core.DrawSprite(fonts[array[i] - 48], pos, col, 0f, 0.5f, 1f);
					pos.X += 16f;
					break;
				case '.':
					pos.Y += 8f;
					pos.X -= 4f;
					GameEngine.core.DrawSprite(fonts[11], pos, col, 0f, 0.5f, 1f);
					pos.X += 12f;
					pos.Y -= 8f;
					break;
				case ',':
					pos.X -= 4f;
					pos.Y += 8f;
					GameEngine.core.DrawSprite(fonts[10], pos, col, 0f, 0.5f, 1f);
					pos.X += 12f;
					pos.Y -= 8f;
					break;
				}
			}
		}

		public override void Draw2()
		{
			Rectangle safeArea = GameEngine.core.SafeArea;
			Color black = Color.Black;
			float x = safeArea.X + 12 + 6;
			float num = safeArea.Y + 12;
			GameEngine.core.spriteBatch.Draw(gridtex, new Vector2(0f, 0f), new Color(1f, 1f, 1f, 0.3f));
			Vector2 pos = new Vector2(x, num);
			GameEngine.core.DrawSprite(fonts[12], pos, black, 0f, 0.5f, 1f);
			pos.X += 25f;
			GameEngine.core.DrawSprite(fonts[13], pos, black, 0f, 0.5f, 1f);
			pos.X += 24f;
			GameEngine.core.DrawSprite(fonts[14], pos, black, 0f, 0.5f, 1f);
			pos.X += 25f;
			GameEngine.core.DrawSprite(fonts[15], pos, black, 0f, 0.5f, 1f);
			pos.X += 72f;
			pos.Y++;
			GameEngine.core.DrawSprite(fonts[26], pos, black, 0f, 0.5f, 1f);
			pos.X += 20f;
			DrawValue(ref pos, dscore, "N0");
			num += 36f;
			pos = new Vector2(x, num);
			GameEngine.core.DrawSprite(fonts[12], pos, black, 0f, 0.5f, 1f);
			pos.X += 25f;
			GameEngine.core.DrawSprite(fonts[16], pos, black, 0f, 0.5f, 1f);
			pos.X += 25f;
			pos.Y++;
			GameEngine.core.DrawSprite(fonts[17], pos, black, 0f, 0.5f, 1f);
			pos.X += 24f;
			pos.Y--;
			GameEngine.core.DrawSprite(fonts[18], pos, black, 0f, 0.5f, 1f);
			pos.X += 72f;
			DrawValue(ref pos, darea, "N1");
			pos.X += 10f;
			pos.Y -= 0f;
			GameEngine.core.DrawSprite(fonts[27], pos, black, 0f, 0.5f, 1f);
			num += 36f;
			num += 12f;
			pos = new Vector2(x, num);
			GameEngine.core.DrawSprite(fonts[22], pos, black, 0f, 0.5f, 1f);
			pos.X += 25f;
			GameEngine.core.DrawSprite(fonts[23], pos, black, 0f, 0.5f, 1f);
			pos.X += 25f;
			GameEngine.core.DrawSprite(fonts[24], pos, black, 0f, 0.5f, 1f);
			pos.X += 24f;
			GameEngine.core.DrawSprite(fonts[25], pos, black, 0f, 0.5f, 1f);
			pos.X += 24f;
			GameEngine.core.DrawSprite(fonts[14], pos, black, 0f, 0.5f, 1f);
			pos.X += 48f;
			GameEngine.core.DrawSprite(fonts[26], pos, black, 0f, 0.5f, 1f);
			pos.X += 20f;
			float num2 = 0f;
			if (area > 0f)
			{
				num2 = score / (area / 3.3f);
			}
			dtanka += (num2 - dtanka) * 0.1f;
			DrawValue(ref pos, dtanka, "N0");
		}
	}
}
