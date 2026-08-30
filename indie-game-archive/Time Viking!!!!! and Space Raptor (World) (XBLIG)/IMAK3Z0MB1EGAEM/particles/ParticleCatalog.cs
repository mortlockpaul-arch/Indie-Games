using System.Collections.Generic;
using IMAK3Z0MB1EGAEM.particles.particles;

namespace IMAK3Z0MB1EGAEM.particles;

public class ParticleCatalog
{
	public const int PT_BLOOD = 0;

	public const int PT_SHOT = 1;

	public const int PT_MUZZLEFLASH = 2;

	public const int PT_SCORE = 3;

	public const int PT_PIXEL = 4;

	public const int PT_GOO = 5;

	public const int PT_FACETRAIL = 6;

	public const int PT_FACEDIE = 7;

	public const int PT_POWERUP = 8;

	public const int PT_FLAME = 9;

	public const int PT_EXPLODE = 10;

	public const int PT_ROCKET = 11;

	public const int PT_NEUTRON = 12;

	public const int PT_NEUCHUNK = 13;

	public const int PT_LASER = 14;

	public const int PT_GEOBIT = 15;

	public const int PT_DYING = 16;

	public Dictionary<int, BaseParticleDef> catalog;

	public ParticleCatalog()
	{
		catalog = new Dictionary<int, BaseParticleDef>();
		catalog.Add(0, new Blood());
		catalog.Add(1, new Shot());
		catalog.Add(2, new MuzzleFlash());
		catalog.Add(3, new Score());
		catalog.Add(4, new Pixel());
		catalog.Add(5, new Goo());
		catalog.Add(6, new FaceTrail());
		catalog.Add(7, new FaceDie());
		catalog.Add(8, new PowerUp());
		catalog.Add(9, new Flame());
		catalog.Add(10, new Explode());
		catalog.Add(11, new Rocket());
		catalog.Add(12, new Neutron());
		catalog.Add(13, new Neuchunk());
		catalog.Add(14, new Laser());
		catalog.Add(15, new Geobit());
		catalog.Add(16, new Dying());
	}
}
