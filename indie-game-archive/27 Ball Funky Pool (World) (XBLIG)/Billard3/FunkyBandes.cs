using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Billard3;

public class FunkyBandes
{
	public class CollisionInfoOneBande
	{
		public Vector2 P0;

		public Vector2 P1;

		public BandeObject Bande;

		public bool Hit;

		public Bandes.Identifiers_4_bandes_orthos BandeID;

		public CollisionInfoOneBande(Vector2 p0, Vector2 p1, BandeObject bande, Bandes.Identifiers_4_bandes_orthos bandeID)
		{
			P0 = p0;
			P1 = p1;
			Bande = bande;
			BandeID = bandeID;
		}

		public bool Test(Vector2 velocity)
		{
			return BandeID switch
			{
				Bandes.Identifiers_4_bandes_orthos.XP => velocity.X < 0f, 
				Bandes.Identifiers_4_bandes_orthos.XM => velocity.X > 0f, 
				Bandes.Identifiers_4_bandes_orthos.ZP => velocity.Y < 0f, 
				Bandes.Identifiers_4_bandes_orthos.ZM => velocity.Y > 0f, 
				_ => throw new NotImplementedException(), 
			};
		}
	}

	public class CollisionInfoFourBande
	{
		public CollisionInfoOneBande[] Data;

		public CollisionInfoFourBande(Rectangle funkyBandeRec)
		{
			Data = new CollisionInfoOneBande[4];
			Data[0] = new CollisionInfoOneBande(new Vector2((float)funkyBandeRec.Right + 0.833333f, (float)funkyBandeRec.Top - 0.833333f), new Vector2((float)funkyBandeRec.Right + 0.833333f, (float)funkyBandeRec.Bottom + 0.833333f), FunkyBandeRejetXP, Bandes.Identifiers_4_bandes_orthos.XP);
			Data[1] = new CollisionInfoOneBande(new Vector2((float)funkyBandeRec.Left - 0.833333f, (float)funkyBandeRec.Top - 0.833333f), new Vector2((float)funkyBandeRec.Left - 0.833333f, (float)funkyBandeRec.Bottom + 0.833333f), FunkyBandeRejetXM, Bandes.Identifiers_4_bandes_orthos.XM);
			Data[2] = new CollisionInfoOneBande(new Vector2((float)funkyBandeRec.Left - 0.833333f, (float)funkyBandeRec.Bottom + 0.833333f), new Vector2((float)funkyBandeRec.Right + 0.833333f, (float)funkyBandeRec.Bottom + 0.833333f), FunkyBandeRejetZP, Bandes.Identifiers_4_bandes_orthos.ZP);
			Data[3] = new CollisionInfoOneBande(new Vector2((float)funkyBandeRec.Left - 0.833333f, (float)funkyBandeRec.Top - 0.833333f), new Vector2((float)funkyBandeRec.Right + 0.833333f, (float)funkyBandeRec.Top - 0.833333f), FunkyBandeRejetZM, Bandes.Identifiers_4_bandes_orthos.ZM);
		}
	}

	public static List<Rectangle> listFunkyBandes = new List<Rectangle>();

	public static List<CollisionInfoFourBande> listCollisionInfo = new List<CollisionInfoFourBande>();

	public static BandeObject FunkyBandeRejetXP = new BandeObject(BandeObject.Id.FUNKY_BANDE_REJET_XP, BandeObject.Type.CUSTOM, Vector2.Zero, Vector2.UnitY, Vector2.UnitX, "Funky Bande Rejet XP");

	public static BandeObject FunkyBandeRejetXM = new BandeObject(BandeObject.Id.FUNKY_BANDE_REJET_XM, BandeObject.Type.CUSTOM, Vector2.Zero, Vector2.UnitY, Vector2.UnitX * -1f, "Funky Bande Rejet XM");

	public static BandeObject FunkyBandeRejetZP = new BandeObject(BandeObject.Id.FUNKY_BANDE_REJET_ZP, BandeObject.Type.CUSTOM, Vector2.Zero, Vector2.UnitX, Vector2.UnitY, "Funky Bande Rejet ZP");

	public static BandeObject FunkyBandeRejetZM = new BandeObject(BandeObject.Id.FUNKY_BANDE_REJET_ZM, BandeObject.Type.CUSTOM, Vector2.Zero, Vector2.UnitX, Vector2.UnitY * -1f, "Funky Bande Rejet ZM");

	public static void Initialize()
	{
		foreach (Rectangle listFunkyBande in listFunkyBandes)
		{
			listCollisionInfo.Add(new CollisionInfoFourBande(listFunkyBande));
		}
	}
}
