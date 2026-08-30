using Microsoft.Xna.Framework;

namespace Billard3;

public class Bandes
{
	public enum Identifiers_4_bandes_orthos
	{
		XM,
		XP,
		ZM,
		ZP
	}

	public static Vector2 pointCollisionTrouCentralExterieur = new Vector2(0.5238433f, 29.167f);

	public static Vector2 pointCollisionTrouCentralInterieur = new Vector2(0.2716407f, 9.990462f);

	public static Vector2 pointCollisionTrouCentralMilieu = new Vector2(0.3433734f, 9.756135f);

	public static BandeObject bandeCorner_XPZP_Largeur = new BandeObject(BandeObject.Id.CORNER_XP_ZP_LARGEUR, BandeObject.Type.TROU_CORNER, new Vector2(30f, 28.271f), new Vector2(30.833f, 28.96f), Trous.trouXPZP.pos, "bande Corner XPZP Largeur");

	public static BandeObject bandeCorner_XPZP_Longueur = new BandeObject(BandeObject.Id.CORNER_XP_ZP_LONGUEUR, BandeObject.Type.TROU_CORNER, new Vector2(28.271f, 30f), new Vector2(28.96f, 30.833f), Trous.trouXPZP.pos, "bande Corner XPZP Longueur");

	public static BandeObject bandeCorner_XPZM_Largeur = new BandeObject(BandeObject.Id.CORNER_XP_ZM_LARGEUR, BandeObject.Type.TROU_CORNER, new Vector2(30f, -28.271f), new Vector2(30.833f, -28.96f), Trous.trouXPZM.pos, "bande Corner XPZM Largeur");

	public static BandeObject bandeCorner_XPZM_Longueur = new BandeObject(BandeObject.Id.CORNER_XP_ZM_LONGUEUR, BandeObject.Type.TROU_CORNER, new Vector2(28.271f, -30f), new Vector2(28.96f, -30.833f), Trous.trouXPZM.pos, "bande Corner XPZM Longueur");

	public static BandeObject bandeCorner_XMZP_Largeur = new BandeObject(BandeObject.Id.CORNER_XM_ZP_LARGEUR, BandeObject.Type.TROU_CORNER, new Vector2(-30f, 28.271f), new Vector2(-30.833f, 28.96f), Trous.trouXMZP.pos, "bande Corner XMZP Largeur");

	public static BandeObject bandeCorner_XMZP_Longueur = new BandeObject(BandeObject.Id.CORNER_XM_ZP_LONGUEUR, BandeObject.Type.TROU_CORNER, new Vector2(-28.271f, 30f), new Vector2(-28.96f, 30.833f), Trous.trouXMZP.pos, "bande Corner XMZP Longueur");

	public static BandeObject bandeCorner_XMZM_Largeur = new BandeObject(BandeObject.Id.CORNER_XM_ZM_LARGEUR, BandeObject.Type.TROU_CORNER, new Vector2(-30f, -28.271f), new Vector2(-30.833f, -28.96f), Trous.trouXMZM.pos, "bande Corner XMZM Largeur");

	public static BandeObject bandeCorner_XMZM_Longueur = new BandeObject(BandeObject.Id.CORNER_XM_ZM_LONGUEUR, BandeObject.Type.TROU_CORNER, new Vector2(-28.271f, -30f), new Vector2(-28.96f, -30.833f), Trous.trouXMZM.pos, "bande Corner XMZM Longueur");

	public static BandeObject bandeTrouCentralXPZP = new BandeObject(BandeObject.Id.CENTRAL_XP_ZP, BandeObject.Type.TROU_CENTRAL, new Vector2(1.239f, 30f), new Vector2(0.885f, 30.833f), Trous.trouX0ZP.pos, "bande trou CENTRAL XP ZP");

	public static BandeObject bandeTrouCentralXMZP = new BandeObject(BandeObject.Id.CENTRAL_XM_ZP, BandeObject.Type.TROU_CENTRAL, new Vector2(-1.239f, 30f), new Vector2(-0.885f, 30.833f), Trous.trouX0ZP.pos, "bande trou CENTRAL XM ZP");

	public static BandeObject bandeTrouCentralXPZM = new BandeObject(BandeObject.Id.CENTRAL_XP_ZM, BandeObject.Type.TROU_CENTRAL, new Vector2(1.239f, -30f), new Vector2(0.885f, -30.833f), Trous.trouX0ZM.pos, "bande trou CENTRAL XP ZM");

	public static BandeObject bandeTrouCentralXMZM = new BandeObject(BandeObject.Id.CENTRAL_XM_ZM, BandeObject.Type.TROU_CENTRAL, new Vector2(-1.239f, -30f), new Vector2(-0.885f, -30.833f), Trous.trouX0ZM.pos, "bande trou CENTRAL XM ZM");

	public static BandeObject bandeOrthoZ_XP = new BandeObject(BandeObject.Id.ORTHO_Z_XP, BandeObject.Type.ORTHO_Z, new Vector2(30f, 28.271f), new Vector2(30f, -28.271f), new Vector2(0f, 0f), "bande ORTHO Z XP");

	public static BandeObject bandeOrthoZ_XM = new BandeObject(BandeObject.Id.ORTHO_Z_XM, BandeObject.Type.ORTHO_Z, new Vector2(-30f, 28.271f), new Vector2(-30f, -28.271f), new Vector2(0f, 0f), "bande ORTHO Z XM");

	public static BandeObject bandeOrthoX_ZP = new BandeObject(BandeObject.Id.ORTHO_X_ZP, BandeObject.Type.ORTHO_X, new Vector2(28.271f, 30f), new Vector2(-28.271f, 30f), new Vector2(0f, 0f), "bande ORTHO X ZP");

	public static BandeObject bandeOrthoX_ZM = new BandeObject(BandeObject.Id.ORTHO_X_ZM, BandeObject.Type.ORTHO_X, new Vector2(28.271f, -30f), new Vector2(-28.271f, -30f), new Vector2(0f, 0f), "bande ORTHO X ZM");

	public static BandeObject specialCollisionTrou = new BandeObject(BandeObject.Id.CUSTOM, BandeObject.Type.COLLISION_AVEC_TROU, Vector2.Zero, Vector2.Zero, Vector2.Zero, "special collision trou");
}
