using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TechArts;

namespace MADRISM
{
	internal class AttractState : TaskObj
	{
		private const int DESCMAX = 5;

		public static Parts nextParts;

		private Texture2D gridtex;

		private Texture2D[] texts;

		private Texture2D[] texttex;

		private Vector2[] textpos;

		private float[] textalp;

		public AttractState()
		{
			gridtex = GameEngine.core.Content.Load<Texture2D>("Sprite/Game/grid");
			texttex = new Texture2D[5];
			textpos = new Vector2[5];
			textalp = new float[5];
			texts = new Texture2D[24];
			for (int i = 1; i < 24; i++)
			{
				texts[i] = GameEngine.core.Content.Load<Texture2D>("Sprite/Attract/" + i.ToString("00"));
			}
			nextParts = null;
		}

		public override void PostUpdate()
		{
			if (GameEngine.core.IsPressed_A_Ctr())
			{
				GlobalState.inDestroy = true;
				GlobalState.inState = false;
				manager.Remove(this);
			}
		}

		public override void Draw()
		{
		}

		public override void Draw2()
		{
			for (int i = 0; i < 5; i++)
			{
				if (texttex[i] != null && textalp[i] > 0f)
				{
					GameEngine.core.spriteBatch.Draw(texttex[i], textpos[i], null, new Color(1f, 1f, 1f, textalp[i]), 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 1f);
				}
			}
		}

		public override IEnumerator<int> Update()
		{
			Rectangle sf = GameEngine.core.SafeArea;
			sf.X += 8;
			for (int i = 0; i < 30; i++)
			{
				yield return 0;
			}
			int left = sf.X;
			int bottom = sf.Y + sf.Height;
			int linespc = (int)((float)texts[2].Height * 1.5f);
			for (int j = 0; j < 60; j++)
			{
				yield return 0;
			}
			texttex[0] = texts[1];
			textpos[0] = new Vector2(left, bottom - texts[1].Height - 16);
			textalp[0] = 0f;
			yield return 0;
			for (int k = 0; k < 30; k++)
			{
				textalp[0] += 1f / 30f;
				yield return 0;
			}
			textalp[0] = 1f;
			for (int l = 0; l < 180; l++)
			{
				yield return 0;
			}
			nextParts = new Parts(PlayState.UnitKind.Room, 0.0048f, new Vector2(192f, 120f));
			for (int m = 0; m < 60; m++)
			{
				yield return 0;
			}
			for (int n = 0; n < 30; n++)
			{
				textalp[0] -= 1f / 30f;
				yield return 0;
			}
			textalp[0] = 0f;
			for (int num = 0; num < 60; num++)
			{
				yield return 0;
			}
			texttex[0] = texts[2];
			textpos[0] = new Vector2(left, bottom - linespc * 3);
			textalp[0] = 0f;
			yield return 0;
			for (int num2 = 0; num2 < 30; num2++)
			{
				textalp[0] += 1f / 30f;
				yield return 0;
			}
			textalp[0] = 1f;
			Parts.VP_Up = true;
			for (int num3 = 0; num3 < 30; num3++)
			{
				yield return 0;
			}
			Parts.VP_Up = false;
			for (int num4 = 0; num4 < 15; num4++)
			{
				yield return 0;
			}
			Parts.VP_Down = true;
			for (int num5 = 0; num5 < 30; num5++)
			{
				yield return 0;
			}
			Parts.VP_Down = false;
			for (int num6 = 0; num6 < 15; num6++)
			{
				yield return 0;
			}
			Parts.VP_Down = true;
			for (int num7 = 0; num7 < 30; num7++)
			{
				yield return 0;
			}
			Parts.VP_Down = false;
			for (int num8 = 0; num8 < 15; num8++)
			{
				yield return 0;
			}
			Parts.VP_Up = true;
			for (int num9 = 0; num9 < 30; num9++)
			{
				yield return 0;
			}
			Parts.VP_Up = false;
			for (int num10 = 0; num10 < 30; num10++)
			{
				yield return 0;
			}
			Parts.VP_Left = true;
			for (int num11 = 0; num11 < 30; num11++)
			{
				yield return 0;
			}
			Parts.VP_Left = false;
			for (int num12 = 0; num12 < 15; num12++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num13 = 0; num13 < 30; num13++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num14 = 0; num14 < 15; num14++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num15 = 0; num15 < 30; num15++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num16 = 0; num16 < 15; num16++)
			{
				yield return 0;
			}
			Parts.VP_Left = true;
			for (int num17 = 0; num17 < 40; num17++)
			{
				yield return 0;
			}
			Parts.VP_Left = false;
			texttex[1] = texts[3];
			textpos[1] = new Vector2(left, bottom - linespc * 2);
			textalp[1] = 0f;
			yield return 0;
			for (int num18 = 0; num18 < 30; num18++)
			{
				textalp[1] += 1f / 30f;
				yield return 0;
			}
			textalp[1] = 1f;
			for (int num19 = 0; num19 < 60; num19++)
			{
				yield return 0;
			}
			for (int num20 = 0; num20 < 4; num20++)
			{
				Parts.VP_RotL = true;
				for (int num21 = 0; num21 < 8; num21++)
				{
					yield return 0;
				}
				Parts.VP_RotL = false;
				for (int num22 = 0; num22 < 24; num22++)
				{
					yield return 0;
				}
			}
			for (int num23 = 0; num23 < 30; num23++)
			{
				yield return 0;
			}
			for (int num24 = 0; num24 < 4; num24++)
			{
				Parts.VP_RotR = true;
				for (int num25 = 0; num25 < 8; num25++)
				{
					yield return 0;
				}
				Parts.VP_RotR = false;
				for (int num26 = 0; num26 < 24; num26++)
				{
					yield return 0;
				}
			}
			for (int num27 = 0; num27 < 30; num27++)
			{
				yield return 0;
			}
			texttex[2] = texts[4];
			textpos[2] = new Vector2(left, bottom - linespc);
			textalp[2] = 0f;
			yield return 0;
			for (int num28 = 0; num28 < 30; num28++)
			{
				textalp[2] += 1f / 30f;
				yield return 0;
			}
			textalp[2] = 1f;
			Parts.VP_A = true;
			for (int num29 = 0; num29 < 10; num29++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num30 = 0; num30 < 50; num30++)
			{
				yield return 0;
			}
			for (int num31 = 0; num31 < 180; num31++)
			{
				yield return 0;
			}
			for (int num32 = 0; num32 < 30; num32++)
			{
				textalp[0] -= 1f / 30f;
				textalp[1] -= 1f / 30f;
				textalp[2] -= 1f / 30f;
				yield return 0;
			}
			textalp[0] = (textalp[1] = (textalp[2] = 0f));
			for (int num33 = 0; num33 < 60; num33++)
			{
				yield return 0;
			}
			for (int num34 = 0; num34 < 5; num34++)
			{
				texttex[num34] = texts[5 + num34];
				textpos[num34] = new Vector2(left, bottom - linespc * (5 - num34));
				textalp[num34] = 0f;
			}
			textpos[1].X += 5f;
			textpos[3].X += 5f;
			yield return 0;
			for (int num35 = 0; num35 < 30; num35++)
			{
				textalp[0] += 1f / 30f;
				yield return 0;
			}
			textalp[0] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Room, 0.009000001f, new Vector2(192f, 192f));
			for (int num36 = 0; num36 < 30; num36++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num37 = 0; num37 < 18; num37++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num38 = 0; num38 < 30; num38++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num39 = 0; num39 < 10; num39++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num40 = 0; num40 < 20; num40++)
			{
				yield return 0;
			}
			for (int num41 = 0; num41 < 30; num41++)
			{
				textalp[1] += 1f / 30f;
				yield return 0;
			}
			textalp[1] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Door, 0.009000001f, null);
			for (int num42 = 0; num42 < 30; num42++)
			{
				yield return 0;
			}
			Parts.VP_RotR = true;
			for (int num43 = 0; num43 < 10; num43++)
			{
				yield return 0;
			}
			Parts.VP_RotR = false;
			for (int num44 = 0; num44 < 30; num44++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num45 = 0; num45 < 10; num45++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num46 = 0; num46 < 20; num46++)
			{
				yield return 0;
			}
			for (int num47 = 0; num47 < 30; num47++)
			{
				textalp[2] += 1f / 30f;
				yield return 0;
			}
			textalp[2] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Kitchen, 0.009000001f, null);
			for (int num48 = 0; num48 < 30; num48++)
			{
				yield return 0;
			}
			Parts.VP_Left = true;
			for (int num49 = 0; num49 < 24; num49++)
			{
				yield return 0;
			}
			Parts.VP_Left = false;
			for (int num50 = 0; num50 < 30; num50++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num51 = 0; num51 < 10; num51++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num52 = 0; num52 < 20; num52++)
			{
				yield return 0;
			}
			for (int num53 = 0; num53 < 30; num53++)
			{
				textalp[3] += 1f / 30f;
				yield return 0;
			}
			textalp[3] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Toilet, 0.009000001f, null);
			for (int num54 = 0; num54 < 30; num54++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num55 = 0; num55 < 20; num55++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num56 = 0; num56 < 60; num56++)
			{
				yield return 0;
			}
			for (int num57 = 0; num57 < 30; num57++)
			{
				textalp[4] += 1f / 30f;
				yield return 0;
			}
			textalp[4] = 1f;
			Parts.VP_A = true;
			for (int num58 = 0; num58 < 10; num58++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num59 = 0; num59 < 20; num59++)
			{
				yield return 0;
			}
			for (int num60 = 0; num60 < 120; num60++)
			{
				yield return 0;
			}
			for (int num61 = 0; num61 < 30; num61++)
			{
				textalp[0] -= 1f / 30f;
				textalp[1] -= 1f / 30f;
				textalp[2] -= 1f / 30f;
				textalp[3] -= 1f / 30f;
				textalp[4] -= 1f / 30f;
				yield return 0;
			}
			textalp[0] = (textalp[1] = (textalp[2] = (textalp[3] = (textalp[4] = 0f))));
			for (int num62 = 0; num62 < 60; num62++)
			{
				yield return 0;
			}
			for (int num63 = 0; num63 < 5; num63++)
			{
				textpos[num63] = new Vector2(left, bottom - linespc * (5 - num63));
				textalp[num63] = 0f;
			}
			texttex[0] = texts[5];
			texttex[1] = texts[7];
			texttex[2] = texts[12];
			texttex[3] = texts[11];
			texttex[4] = texts[9];
			textpos[2].X += 5f;
			textpos[3].X += 5f;
			yield return 0;
			for (int num64 = 0; num64 < 30; num64++)
			{
				textalp[0] += 1f / 30f;
				yield return 0;
			}
			textalp[0] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Room, 0.009000001f, new Vector2(192f, 192f));
			for (int num65 = 0; num65 < 30; num65++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num66 = 0; num66 < 18; num66++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num67 = 0; num67 < 30; num67++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num68 = 0; num68 < 10; num68++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num69 = 0; num69 < 30; num69++)
			{
				yield return 0;
			}
			nextParts = new Parts(PlayState.UnitKind.Room, 0.009000001f, new Vector2(192f, 192f));
			for (int num70 = 0; num70 < 30; num70++)
			{
				yield return 0;
			}
			Parts.VP_Left = true;
			for (int num71 = 0; num71 < 22; num71++)
			{
				yield return 0;
			}
			Parts.VP_Left = false;
			for (int num72 = 0; num72 < 30; num72++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num73 = 0; num73 < 10; num73++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num74 = 0; num74 < 30; num74++)
			{
				yield return 0;
			}
			for (int num75 = 0; num75 < 30; num75++)
			{
				textalp[1] += 1f / 30f;
				yield return 0;
			}
			textalp[1] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Kitchen, 0.009000001f, null);
			for (int num76 = 0; num76 < 30; num76++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num77 = 0; num77 < 20; num77++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num78 = 0; num78 < 30; num78++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num79 = 0; num79 < 10; num79++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num80 = 0; num80 < 20; num80++)
			{
				yield return 0;
			}
			for (int num81 = 0; num81 < 30; num81++)
			{
				textalp[2] += 1f / 30f;
				yield return 0;
			}
			textalp[2] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Toilet, 0.009000001f, null);
			for (int num82 = 0; num82 < 30; num82++)
			{
				yield return 0;
			}
			Parts.VP_Left = true;
			for (int num83 = 0; num83 < 24; num83++)
			{
				yield return 0;
			}
			Parts.VP_Left = false;
			for (int num84 = 0; num84 < 30; num84++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num85 = 0; num85 < 10; num85++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num86 = 0; num86 < 20; num86++)
			{
				yield return 0;
			}
			for (int num87 = 0; num87 < 30; num87++)
			{
				textalp[3] += 1f / 30f;
				yield return 0;
			}
			textalp[3] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Door, 0.009000001f, null);
			for (int num88 = 0; num88 < 30; num88++)
			{
				yield return 0;
			}
			Parts.VP_RotR = true;
			for (int num89 = 0; num89 < 10; num89++)
			{
				yield return 0;
			}
			Parts.VP_RotR = false;
			for (int num90 = 0; num90 < 30; num90++)
			{
				yield return 0;
			}
			for (int num91 = 0; num91 < 30; num91++)
			{
				textalp[4] += 1f / 30f;
				yield return 0;
			}
			textalp[4] = 1f;
			Parts.VP_A = true;
			for (int num92 = 0; num92 < 10; num92++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num93 = 0; num93 < 20; num93++)
			{
				yield return 0;
			}
			for (int num94 = 0; num94 < 120; num94++)
			{
				yield return 0;
			}
			for (int num95 = 0; num95 < 30; num95++)
			{
				textalp[0] -= 1f / 30f;
				textalp[1] -= 1f / 30f;
				textalp[2] -= 1f / 30f;
				textalp[3] -= 1f / 30f;
				textalp[4] -= 1f / 30f;
				yield return 0;
			}
			textalp[0] = (textalp[1] = (textalp[2] = (textalp[3] = (textalp[4] = 0f))));
			for (int num96 = 0; num96 < 60; num96++)
			{
				yield return 0;
			}
			texttex[0] = texts[13];
			textpos[0] = new Vector2(left, bottom - linespc);
			textalp[0] = 0f;
			yield return 0;
			for (int num97 = 0; num97 < 30; num97++)
			{
				textalp[0] += 1f / 30f;
				yield return 0;
			}
			textalp[0] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Room, 0.009000001f, new Vector2(192f, 192f));
			for (int num98 = 0; num98 < 30; num98++)
			{
				yield return 0;
			}
			Parts.VP_Left = true;
			for (int num99 = 0; num99 < 24; num99++)
			{
				yield return 0;
			}
			Parts.VP_Left = false;
			for (int num100 = 0; num100 < 30; num100++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num101 = 0; num101 < 10; num101++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num102 = 0; num102 < 30; num102++)
			{
				yield return 0;
			}
			nextParts = new Parts(PlayState.UnitKind.Door, 0.009000001f, null);
			for (int num103 = 0; num103 < 30; num103++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num104 = 0; num104 < 24; num104++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num105 = 0; num105 < 30; num105++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num106 = 0; num106 < 10; num106++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num107 = 0; num107 < 30; num107++)
			{
				yield return 0;
			}
			nextParts = new Parts(PlayState.UnitKind.Toilet, 0.009000001f, null);
			for (int num108 = 0; num108 < 30; num108++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num109 = 0; num109 < 24; num109++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num110 = 0; num110 < 30; num110++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num111 = 0; num111 < 10; num111++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num112 = 0; num112 < 30; num112++)
			{
				yield return 0;
			}
			nextParts = new Parts(PlayState.UnitKind.Kitchen, 0.009000001f, null);
			for (int num113 = 0; num113 < 30; num113++)
			{
				yield return 0;
			}
			Parts.VP_Right = true;
			for (int num114 = 0; num114 < 24; num114++)
			{
				yield return 0;
			}
			Parts.VP_Right = false;
			for (int num115 = 0; num115 < 30; num115++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num116 = 0; num116 < 10; num116++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num117 = 0; num117 < 30; num117++)
			{
				yield return 0;
			}
			for (int num118 = 0; num118 < 30; num118++)
			{
				textalp[0] -= 1f / 30f;
				yield return 0;
			}
			textalp[0] = 0f;
			for (int num119 = 0; num119 < 60; num119++)
			{
				yield return 0;
			}
			texttex[0] = texts[14];
			textpos[0] = new Vector2(left, bottom - linespc);
			textalp[0] = 0f;
			yield return 0;
			for (int num120 = 0; num120 < 30; num120++)
			{
				textalp[0] += 1f / 30f;
				yield return 0;
			}
			textalp[0] = 1f;
			nextParts = new Parts(PlayState.UnitKind.Room, 0.009000001f, new Vector2(192f, 192f));
			for (int num121 = 0; num121 < 30; num121++)
			{
				yield return 0;
			}
			Parts.VP_Down = true;
			for (int num122 = 0; num122 < 18; num122++)
			{
				yield return 0;
			}
			Parts.VP_Down = false;
			for (int num123 = 0; num123 < 30; num123++)
			{
				yield return 0;
			}
			Parts.VP_A = true;
			for (int num124 = 0; num124 < 10; num124++)
			{
				yield return 0;
			}
			Parts.VP_A = false;
			for (int num125 = 0; num125 < 240; num125++)
			{
				yield return 0;
			}
			for (int num126 = 0; num126 < 30; num126++)
			{
				textalp[0] -= 1f / 30f;
				yield return 0;
			}
			textalp[0] = 0f;
			for (int num127 = 0; num127 < 60; num127++)
			{
				yield return 0;
			}
			for (int num128 = 0; num128 < 15; num128++)
			{
				GameEngine.core.fader.Brightness += 1f / 15f;
				yield return 0;
			}
			GameEngine.core.fader.Brightness = 1f;
			for (int num129 = 0; num129 < 60; num129++)
			{
				GameEngine.core.fader.Brightness = 1f;
				yield return 0;
			}
			GlobalState.inDestroy = true;
			while (GlobalState.inState)
			{
				GameEngine.core.fader.Brightness = 1f;
				yield return 0;
			}
			manager.Remove(this);
		}
	}
}
