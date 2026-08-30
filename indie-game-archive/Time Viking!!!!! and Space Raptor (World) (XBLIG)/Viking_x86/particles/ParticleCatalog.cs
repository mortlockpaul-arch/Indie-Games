using Viking_x86.particles.debris;
using Viking_x86.particles.evilzaps;
using Viking_x86.particles.explode;
using Viking_x86.particles.pickups;
using Viking_x86.particles.zaps;

namespace Viking_x86.particles;

public class ParticleCatalog
{
	public const int SCRAP = 0;

	public const int MINI_SCRAP = 1;

	public const int EXPLODE = 2;

	public const int DUST = 3;

	public const int ZAP = 4;

	public const int ZAP_GLOW = 5;

	public const int PIXEL = 6;

	public const int PIXEL_DOT = 7;

	public const int ROCKET_SCRAP = 8;

	public const int SPACE_ZAP = 9;

	public const int NEKO_ZAP = 10;

	public const int RED_PIXEL = 11;

	public const int RED_GLOW = 12;

	public const int GOLD = 13;

	public const int GOLD_DOT = 14;

	public const int PICKUP = 15;

	public const int PICKUP_DOT = 16;

	public const int ZAP_BOMB = 17;

	public const int ZAPNEL = 18;

	public const int SCORE = 19;

	public const int SPIT = 20;

	public const int ZAPFLASH = 21;

	public const int SPITDRIP = 22;

	public const int SPITBOMB = 23;

	public const int SPITNEL = 24;

	public static BaseParticle[] particle = new BaseParticle[64];

	public static void Init()
	{
		particle[0] = new Scrap();
		particle[1] = new MiniScrap();
		particle[2] = new Explode();
		particle[3] = new Dust();
		particle[4] = new Zap();
		particle[5] = new ZapGlow();
		particle[6] = new Pixel();
		particle[7] = new PixelDot();
		particle[8] = new RocketScrap();
		particle[9] = new SpaceZap();
		particle[10] = new NekoZap();
		particle[11] = new RedPixel();
		particle[12] = new RedGlow();
		particle[13] = new Gold();
		particle[14] = new GoldDot();
		particle[15] = new Pickup();
		particle[16] = new PickupDot();
		particle[17] = new ZapBomb();
		particle[18] = new Zapnel();
		particle[19] = new Score();
		particle[20] = new Spit();
		particle[21] = new ZapFlash();
		particle[22] = new SpitDrip();
		particle[23] = new SpitBomb();
		particle[24] = new Spitnel();
	}
}
