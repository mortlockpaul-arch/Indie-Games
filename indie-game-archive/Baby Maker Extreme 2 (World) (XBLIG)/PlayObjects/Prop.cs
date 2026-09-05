using System;
using System.Collections.Generic;
using System.Linq;
using BabyMakerExtreme2;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;
using PhysicsHandler;
using PlayObjects.Props;
using Renderer;
using Screens;

namespace PlayObjects;

public class Prop
{
	private delegate void OutfitCreator(PhysicsOutfit outfit);

	private Vector2 m_location;

	private PhysicsOutfit m_physicsOutfit;

	private PropType m_type;

	private bool m_bDisabled;

	private static int sm_iPhysicsIndex = 2;

	private CollisionParticleSpawner m_spawner;

	private List<PropEffector> m_propEffectors;

	public PropType PropType => m_type;

	public Prop(PropType type)
	{
		if (type >= PropType.EASY_GLOW)
		{
			m_spawner = new CollisionParticleSpawner(isVirtualObj: true);
		}
		else
		{
			m_spawner = new CollisionParticleSpawner(isVirtualObj: false);
		}
		m_physicsOutfit = new PhysicsOutfit(sm_iPhysicsIndex);
		sm_iPhysicsIndex++;
		InitOutfit(m_physicsOutfit, type);
		m_type = type;
		m_bDisabled = false;
		CreateEffectors();
	}

	private void InitOutfit(PhysicsOutfit outfit, PropType type)
	{
		new List<List<List<Vector2>>>();
		new List<SpriteInstance>();
		new List<SpriteInstance>();
		new List<MassTypes>();
		OutfitCreator outfitCreator = ((type < PropType.DOG) ? SetCreatorHospital(type) : ((type < PropType.SANITIZER) ? SetCreatorPark(type) : ((type < PropType.EASY_GLOW) ? SetCreatorMall(type) : ((type >= PropType.MAX_TYPE) ? new OutfitCreator(PropGenerator.CreateNurse) : SetCreatorVirtual(type)))));
		outfitCreator(outfit);
		outfit.SetCollisionHandler(CollisionHandler);
	}

	private OutfitCreator SetCreatorHospital(PropType type)
	{
		return type switch
		{
			PropType.BABY => PropGenerator.CreateBaby, 
			PropType.BABY_AVATAR => PropGenerator.CreateBaby, 
			PropType.BABY_MINI => PropGenerator.CreateBabyMini, 
			PropType.BABY_BIG => PropGenerator.CreateBabyBig, 
			PropType.BABY_BALL_LAUNCHER => PropGenerator.CreateBabyBallLauncher, 
			PropType.BABY_FAIRY => PropGenerator.CreateBabyFairy, 
			PropType.BABY_TRIPLE_BOOST => PropGenerator.CreateBabyTripleBoost, 
			PropType.BABY_JETTER => PropGenerator.CreateBabyJetter, 
			PropType.BIRTHMOTHER => PropGenerator.CreateMother, 
			PropType.OTHERMOTHER => PropGenerator.CreateOtherMother, 
			PropType.BABY_MONITOR => PropGenerator.CreateBabyMonitors, 
			PropType.BASONETTE => PropGenerator.CreateBasonette, 
			PropType.BABY_BAG => PropGenerator.CreateBabyBag, 
			PropType.NURSE => PropGenerator.CreateNurse, 
			PropType.TRAY => PropGenerator.CreateOperationCart, 
			PropType.DOCTOR => PropGenerator.CreateDoctor, 
			PropType.BEAR_FATHER => PropGenerator.CreateBearFather, 
			PropType.SLEEPING_FATHER => PropGenerator.CreateSleepingFather, 
			PropType.FLOWER_TABLE => PropGenerator.CreateFlowerTable, 
			PropType.OLD_MAN => PropGenerator.CreateOldMan, 
			PropType.BODYCAST => PropGenerator.CreateBodyCast, 
			PropType.MRI_DOCTOR => PropGenerator.CreateMRIDoctor, 
			PropType.MRI_MACHINE => PropGenerator.CreateMRIMachine, 
			PropType.TV => PropGenerator.CreateTV, 
			PropType.RECEPTION => PropGenerator.CreateReceptionDesk, 
			PropType.CRASHCART => PropGenerator.CreateCrashCart, 
			PropType.CURTAIN => PropGenerator.CreateCurtainPrivacy, 
			PropType.BOX => PropGenerator.CreateBox, 
			PropType.DIRECTOR => PropGenerator.CreateDirector, 
			PropType.LAMP => PropGenerator.CreateLamp, 
			PropType.COATRACK => PropGenerator.CreateCoatRack, 
			PropType.CHAIR_EMPTY => PropGenerator.CreateEmptyWaitChair, 
			PropType.CHAIR_FULL => PropGenerator.CreateFullWaitChair, 
			PropType.CONF_TABLE => PropGenerator.CreateConfTable, 
			PropType.HOUSE_DESK => PropGenerator.CreateHouseDesk, 
			PropType.DIAG_BOARD => PropGenerator.CreateDiagBoard, 
			PropType.DIAPER_PILE => PropGenerator.CreateDiaperPile, 
			PropType.SURGERY_LIGHT => PropGenerator.CreateSurgeryLight, 
			PropType.SURGERY_PATIENT => PropGenerator.CreateSurgeryPatient, 
			PropType.WHEELCHAIR => PropGenerator.CreateWheelChair, 
			PropType.BED_EMPTY => PropGenerator.CreateBedEmpty, 
			PropType.BED_FULL => PropGenerator.CreateBedFull, 
			PropType.CHECKUP_DOC => PropGenerator.CreateCheckupDoc, 
			PropType.CHECKUP_DESK_SKULL => PropGenerator.CreateCheckupDeskSkull, 
			PropType.CHECKUP_DESK_COTTON => PropGenerator.CreateCheckupDeskCotton, 
			PropType.XRAY_FULL => PropGenerator.CreateXRayFull, 
			PropType.XRAY_DOUBLE => PropGenerator.CreateXRayDouble, 
			PropType.BEDSIDE_TABLE => PropGenerator.CreateBedsideTable, 
			PropType.PROCTOL_PATIENT => PropGenerator.CreateProctologyPatient, 
			PropType.HEAD_TRAUMA => PropGenerator.CreateHeadTrauma, 
			PropType.METAL_CABINET => PropGenerator.CreateMetalCabinet, 
			PropType.SHELF_MID => PropGenerator.CreateShelfMid, 
			PropType.SHELF_BIG => PropGenerator.CreateShelfBig, 
			PropType.SINK => PropGenerator.CreateSink, 
			PropType.TISSUES => PropGenerator.CreateTissues, 
			PropType.CHANGING_TABLE => PropGenerator.CreateChangingTable, 
			PropType.TV_WATCHER => PropGenerator.CreateTVWatcher, 
			PropType.BOTTLE_TABLE => PropGenerator.CreateBottleTable, 
			PropType.GLASS_PANEL => PropGenerator.CreateGlassPanel, 
			PropType.AMBULANCE => PropGeneratorPark.CreateAmbulance, 
			PropType.BLOOD_PRESSURE => PropGeneratorMall.CreateBloodPressure, 
			PropType.BIRTH_CURTAIN => PropGeneratorMall.CreateBirthCurtain, 
			_ => PropGenerator.CreateNurse, 
		};
	}

	private OutfitCreator SetCreatorPark(PropType type)
	{
		return type switch
		{
			PropType.DOG => PropGeneratorPark.CreateDog, 
			PropType.PORTAPOTTY => PropGeneratorPark.CreatePortaPotty, 
			PropType.BUSH1 => PropGeneratorPark.CreateBush1, 
			PropType.BUSH2 => PropGeneratorPark.CreateBush2, 
			PropType.BUSH3 => PropGeneratorPark.CreateBush3, 
			PropType.GARBAGE_CAN => PropGeneratorPark.CreateGarbageCan, 
			PropType.FOUNDTAIN_BIG => PropGeneratorPark.CreateFountainBig, 
			PropType.FOUNTAIN_SMALL => PropGeneratorPark.CreateFountainSmall, 
			PropType.BENCH1 => PropGeneratorPark.CreateBench1, 
			PropType.BENCH2 => PropGeneratorPark.CreateBench2, 
			PropType.BENCH3 => PropGeneratorPark.CreateBench3, 
			PropType.PICNICTABLE1 => PropGeneratorPark.CreatePicnicTable1, 
			PropType.PICNICTABLE2FAT => PropGeneratorPark.CreatePicnicTable2Fatty, 
			PropType.PICNICTABLE3 => PropGeneratorPark.CreatePicnicTable3Skinny, 
			PropType.BBQ => PropGeneratorPark.CreateBBQ, 
			PropType.PICNIC_FLOOR => PropGeneratorPark.CreatePicnicFloor, 
			PropType.ICE_CREAM => PropGeneratorPark.CreateIceCream, 
			PropType.STATUE => PropGeneratorPark.CreateStatue, 
			PropType.SLIDE => PropGeneratorPark.CreateSlide, 
			PropType.BOUNCY_HIPPO => PropGeneratorPark.CreateBouncyHippo, 
			PropType.BOUNCY_DRAGON => PropGeneratorPark.CreateBouncyDragon, 
			PropType.SIGN => PropGeneratorPark.CreateSign, 
			PropType.TEETER_TOTTER => PropGeneratorPark.CreateTeeterTotter, 
			PropType.BIKE => PropGeneratorPark.CreateBike, 
			PropType.BLOCK_CINDER => PropGeneratorPark.CreateBlock, 
			PropType.PIPE => PropGeneratorPark.CreatePipe, 
			PropType.JUNGLE_GYM => PropGeneratorPark.CreateJungleGym, 
			PropType.GRASS_CUTTER => PropGeneratorPark.CreateGrassCutter, 
			PropType.CLOWN => PropGeneratorPark.CreateClown, 
			PropType.VOLLEYBALL => PropGeneratorPark.CreateVolleyBall, 
			PropType.PAINTER => PropGeneratorPark.CreatePainter, 
			PropType.TOILET => PropGeneratorPark.CreateToilet, 
			PropType.DUMP_SIGN => PropGeneratorPark.CreateDumpSign, 
			PropType.BOX_GARBAGE => PropGeneratorPark.CreateGarbageBox, 
			PropType.CONTAINER_GARBAGE => PropGeneratorPark.CreateGarbageContainer, 
			PropType.GARBAGE_BAG_PILE => PropGeneratorPark.CreateGarbageBagPile, 
			PropType.TIRE => PropGeneratorPark.CreateTire, 
			PropType.BONE => PropGeneratorPark.CreateBone, 
			PropType.GARBAGE_BAG1 => PropGeneratorPark.CreateGarbageBag1, 
			PropType.GARBAGE_BAG2 => PropGeneratorPark.CreateGarbageBag2, 
			PropType.GARBAGE_PILE => PropGeneratorPark.CreateGarbagePile, 
			PropType.SKATEBOARDER => PropGeneratorPark.CreateSkater, 
			PropType.RUNNER1 => PropGeneratorPark.CreateRunner1, 
			PropType.RUNNER2 => PropGeneratorPark.CreateRunner2, 
			PropType.RAMPUP => PropGeneratorPark.CreateRampUp, 
			PropType.RAMPDOWN => PropGeneratorPark.CreateRampDown, 
			PropType.FEM_RIGHT_BLUE => PropGeneratorPark.CreateFemRightBlue, 
			PropType.GUY_LEFT_BALD => PropGeneratorPark.CreateGuyLeftBald, 
			PropType.PERSON_LEFT_FRO => PropGeneratorPark.CreatePersonLeftFro, 
			PropType.TRIAL_BEAR => PropGeneratorPark.CreateUpsellBear, 
			_ => PropGeneratorPark.CreateDog, 
		};
	}

	private OutfitCreator SetCreatorMall(PropType type)
	{
		return type switch
		{
			PropType.SANITIZER => PropGeneratorMall.CreateSanitizer, 
			PropType.SALE_SIGN => PropGeneratorMall.CreateSaleSign, 
			PropType.MALL_CHAIRS => PropGeneratorMall.CreateMallChairs, 
			PropType.TINY_TREE => PropGeneratorMall.CreateTinyTree, 
			PropType.MALL_SHRUB => PropGeneratorMall.CreateMallShrub, 
			PropType.CELL_BOOTH => PropGeneratorMall.CreateCellBooth, 
			PropType.ATM => PropGeneratorMall.CreateATM, 
			PropType.TOY_BEAR => PropGeneratorMall.CreateToyBear, 
			PropType.ROBOT => PropGeneratorMall.CreateRobot, 
			PropType.SNAKE_POLE => PropGeneratorMall.CreateSnakePole, 
			PropType.MALL_FOUNTAIN => PropGeneratorMall.CreateMallFountain, 
			PropType.BLOCK_STACK => PropGeneratorMall.CreateBlockStack, 
			PropType.TOY_CASHIER => PropGeneratorMall.CreateToyCashier, 
			PropType.TOY_SHELF => PropGeneratorMall.CreateToyShelf, 
			PropType.TRAIN_SET => PropGeneratorMall.CreateTrainSet, 
			PropType.TOY_RACK => PropGeneratorMall.CreateToyRack, 
			PropType.TRICYCLE => PropGeneratorMall.CreateTriCycle, 
			PropType.PEARL_NECKLACE => PropGeneratorMall.CreatePearlNecklace, 
			PropType.JEWEL_TABLE1 => PropGeneratorMall.CreateJewelTable1, 
			PropType.JEWEL_TABLE2 => PropGeneratorMall.CreateJewelTable2, 
			PropType.JEWEL_TABLE3 => PropGeneratorMall.CreateJewelTable3, 
			PropType.JEWELERY_CASHIER => PropGeneratorMall.CreateJeweleryCashier, 
			PropType.NECKLACE_RACK => PropGeneratorMall.CreateNecklaceRack, 
			PropType.CLOTH_TABLE1 => PropGeneratorMall.CreateClothTable1, 
			PropType.CLOTH_TABLE2 => PropGeneratorMall.CreateClothTable2, 
			PropType.CLOTH_TABLE3 => PropGeneratorMall.CreateClothTable3, 
			PropType.WOOD_CHAIR => PropGeneratorMall.CreateWoodChair, 
			PropType.MIRROR => PropGeneratorMall.CreateMirror, 
			PropType.DUMMY => PropGeneratorMall.CreateDummy, 
			PropType.SHIRT_RACK => PropGeneratorMall.CreateShirtRack, 
			PropType.CLOTHING_CASHIER => PropGeneratorMall.CreateClothingCashier, 
			PropType.COFFEECHAIRTABLE1 => PropGeneratorMall.CreateCoffeeChairTable1, 
			PropType.COFFEECHAIRTABLE2 => PropGeneratorMall.CreateCoffeeChairTable2, 
			PropType.COFFEESHELF => PropGeneratorMall.CreateCoffeeShelf, 
			PropType.COFFEESTOOLTABLE1 => PropGeneratorMall.CreateCoffeeStoolTable1, 
			PropType.COFFEESTOOLTABLE2 => PropGeneratorMall.CreateCoffeeStoolTable2, 
			PropType.COFFEECOUCHTABLE1 => PropGeneratorMall.CreateCoffeeCouchTable1, 
			PropType.COFFEECOUCHTABLE2 => PropGeneratorMall.CreateCoffeeCouchTable2, 
			PropType.COFFEE_CASHIER => PropGeneratorMall.CreateCoffeeCashier, 
			PropType.COFFEE_PRESS => PropGeneratorMall.CreateCoffeePress, 
			PropType.COMPUTER_DESK1 => PropGeneratorMall.CreateComputerDesk1, 
			PropType.COMPUTER_DESK2 => PropGeneratorMall.CreateComputerDesk2, 
			PropType.CAMERA_DESK => PropGeneratorMall.CreateCameraDesk, 
			PropType.OLD_COMPUTER_DESK => PropGeneratorMall.CreateOldComputerDesk, 
			PropType.FLOPPY_DESK => PropGeneratorMall.CreateFloppyDesk, 
			PropType.SOFTWARE_SHELF => PropGeneratorMall.CreateSoftwareShelf, 
			PropType.BIG_SCREEN => PropGeneratorMall.CreateBigScreen, 
			PropType.COMPUTER_CASHIER => PropGeneratorMall.CreateComputerCashier, 
			PropType.GIANT_TV => PropGeneratorMall.CreateGiantTV, 
			PropType.PURSE_LADY_LEFT => PropGeneratorMall.CreatePurseLadyLeft, 
			PropType.PUNK_PERSON_RIGHT => PropGeneratorMall.CreatePunkRight, 
			_ => PropGeneratorMall.CreatePunkRight, 
		};
	}

	private OutfitCreator SetCreatorVirtual(PropType type)
	{
		return type switch
		{
			PropType.EASY_GLOW => PropGeneratorVirtual.CreateBigGlow, 
			PropType.EASY_NORM => PropGeneratorVirtual.CreateSmallNorm, 
			PropType.MED_GLOW => PropGeneratorVirtual.CreateMedGlow, 
			PropType.MED_NORM => PropGeneratorVirtual.CreateMedNorm, 
			PropType.HARD_GLOW => PropGeneratorVirtual.CreateSmallGlow, 
			PropType.HARD_NORM => PropGeneratorVirtual.CreateBigNorm, 
			PropType.VHARD_GLOW => PropGeneratorVirtual.CreateSmallestGlow, 
			PropType.VHARD_NORM => PropGeneratorVirtual.CreateBiggestNorm, 
			_ => PropGeneratorVirtual.CreateSmallestGlow, 
		};
	}

	public Prop(Prop clone)
	{
		m_physicsOutfit = new PhysicsOutfit(sm_iPhysicsIndex);
		sm_iPhysicsIndex++;
		InitOutfit(m_physicsOutfit, clone.m_physicsOutfit);
		m_type = clone.m_type;
		m_bDisabled = false;
		CreateEffectors();
	}

	public void CreateEffectors()
	{
		if (m_physicsOutfit.CanGlow())
		{
			m_physicsOutfit.SetDepth(SceneRenderer.GetRand(10f, 20f));
		}
		else
		{
			m_physicsOutfit.SetDepth(SceneRenderer.GetRand(0f, 10f));
		}
		if (PropType == PropType.BIRTHMOTHER)
		{
			m_physicsOutfit.SetDepth(60f);
		}
		m_propEffectors = new List<PropEffector>();
		if (PropType == PropType.BEAR_FATHER)
		{
			List<PhysicalRepresentation> physicsObjects = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list = new List<PhysicalRepresentation>();
			list.Add(physicsObjects[0]);
			List<Vector2> list2 = new List<Vector2>();
			List<float> list3 = new List<float>();
			for (int i = 0; i < list.Count; i++)
			{
				list2.Add(3f * new Vector2(SceneRenderer.GetRand(200f, 600f), SceneRenderer.GetRand(-100f, 100f)));
				list3.Add(SceneRenderer.GetRand(-1f, 1f));
			}
			List<int> list4 = new List<int>();
			list4.Add(1);
			List<Joint> joints = new List<Joint>();
			m_physicsOutfit.GetJoints(list4, joints);
			m_propEffectors.Add(new PropLauncher(joints, list, list2, list3));
		}
		else if (PropType == PropType.BODYCAST)
		{
			List<PhysicalRepresentation> physicsObjects2 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list5 = new List<PhysicalRepresentation>();
			list5.Add(physicsObjects2[2]);
			list5.Add(physicsObjects2[3]);
			list5.Add(physicsObjects2[4]);
			List<Vector2> list6 = new List<Vector2>();
			List<float> list7 = new List<float>();
			list6.Add(new Vector2(-400f, -500f));
			list7.Add(0f - SceneRenderer.GetRand(1f, 2f));
			list6.Add(new Vector2(300f, -500f));
			list7.Add(SceneRenderer.GetRand(1f, 2f));
			list6.Add(new Vector2(600f, -500f));
			list7.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list8 = new List<int>();
			list8.Add(0);
			list8.Add(1);
			list8.Add(2);
			List<Joint> joints2 = new List<Joint>();
			m_physicsOutfit.GetJoints(list8, joints2);
			m_propEffectors.Add(new PropLauncher(joints2, list5, list6, list7));
		}
		else if (PropType == PropType.CURTAIN)
		{
			List<PhysicalRepresentation> physicsObjects3 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list9 = new List<PhysicalRepresentation>();
			list9.Add(physicsObjects3[1]);
			List<Vector2> list10 = new List<Vector2>();
			List<float> list11 = new List<float>();
			list10.Add(new Vector2(600f, -600f));
			list11.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list12 = new List<int>();
			list12.Add(0);
			List<Joint> joints3 = new List<Joint>();
			m_physicsOutfit.GetJoints(list12, joints3);
			m_propEffectors.Add(new PropLauncher(joints3, list9, list10, list11));
		}
		else if (PropType == PropType.PROCTOL_PATIENT)
		{
			List<PhysicalRepresentation> physicsObjects4 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list13 = new List<PhysicalRepresentation>();
			list13.Add(physicsObjects4[0]);
			list13.Add(physicsObjects4[2]);
			List<Vector2> list14 = new List<Vector2>();
			List<float> list15 = new List<float>();
			list14.Add(2f * new Vector2(400f, -500f));
			list15.Add(SceneRenderer.GetRand(1f, 2f));
			list14.Add(2f * new Vector2(400f, -500f));
			list15.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list16 = new List<int>();
			list16.Add(1);
			list16.Add(2);
			list16.Add(3);
			List<Joint> joints4 = new List<Joint>();
			m_physicsOutfit.GetJoints(list16, joints4);
			m_propEffectors.Add(new PropLauncher(joints4, list13, list14, list15));
		}
		else if (PropType == PropType.XRAY_DOUBLE)
		{
			List<PhysicalRepresentation> physicsObjects5 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list17 = new List<PhysicalRepresentation>();
			list17.Add(physicsObjects5[1]);
			list17.Add(physicsObjects5[2]);
			List<Vector2> list18 = new List<Vector2>();
			List<float> list19 = new List<float>();
			list18.Add(new Vector2(600f, -800f));
			list19.Add(SceneRenderer.GetRand(1f, 2f));
			list18.Add(new Vector2(800f, -500f));
			list19.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list20 = new List<int>();
			list20.Add(0);
			list20.Add(1);
			list20.Add(2);
			List<Joint> joints5 = new List<Joint>();
			m_physicsOutfit.GetJoints(list20, joints5);
			m_propEffectors.Add(new PropLauncher(joints5, list17, list18, list19));
			List<SpriteInstance> list21 = new List<SpriteInstance>();
			list21.Add(TextureContainer.GetSprite("images/spritesheets/hospital/sheet6", new Rectangle(425, 520, 259, 200), default(Vector2), m_physicsOutfit.GetSprites()[0].Depth + 0.001f));
			list21.Last().Origin = new Vector2(0f, 150f);
			List<float> list22 = new List<float>();
			list22.Add(5f);
			AnimatedRenderSprite spr = new AnimatedRenderSprite(list21, list22, repeats: true);
			PhysicalRepresentation objConnect = physicsObjects5[0];
			m_propEffectors.Add(new PropAnimator(spr, objConnect, 0));
		}
		else if (PropType == PropType.XRAY_FULL)
		{
			List<PhysicalRepresentation> physicsObjects6 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list23 = new List<PhysicalRepresentation>();
			list23.Add(physicsObjects6[1]);
			List<Vector2> list24 = new List<Vector2>();
			List<float> list25 = new List<float>();
			list24.Add(2f * new Vector2(600f, -800f));
			list25.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list26 = new List<int>();
			list26.Add(0);
			List<Joint> joints6 = new List<Joint>();
			m_physicsOutfit.GetJoints(list26, joints6);
			m_propEffectors.Add(new PropLauncher(joints6, list23, list24, list25));
			List<SpriteInstance> list27 = new List<SpriteInstance>();
			list27.Add(TextureContainer.GetSprite("images/spritesheets/hospital/sheet6", new Rectangle(60, 520, 259, 200), default(Vector2), m_physicsOutfit.GetSprites()[0].Depth + 0.001f));
			list27.Last().Origin = new Vector2(0f, 150f);
			List<float> list28 = new List<float>();
			list28.Add(5f);
			AnimatedRenderSprite spr2 = new AnimatedRenderSprite(list27, list28, repeats: true);
			PhysicalRepresentation objConnect2 = physicsObjects6[0];
			m_propEffectors.Add(new PropAnimator(spr2, objConnect2, 0));
		}
		else if (PropType == PropType.TV)
		{
			List<SpriteInstance> list29 = new List<SpriteInstance>();
			list29.Add(TextureContainer.GetSprite("images/spritesheets/hospital/sheet3", new Rectangle(32, 783, 160, 150), default(Vector2), m_physicsOutfit.GetSprites()[0].Depth + 0.001f));
			list29.Last().Origin = new Vector2(30f, 152f);
			list29.Add(TextureContainer.GetSprite("images/spritesheets/hospital/sheet3", new Rectangle(288, 779, 160, 150), default(Vector2), m_physicsOutfit.GetSprites()[0].Depth + 0.001f));
			list29.Last().Origin = new Vector2(30f, 152f);
			list29.Add(TextureContainer.GetSprite("images/spritesheets/hospital/sheet3", new Rectangle(544, 775, 160, 150), default(Vector2), m_physicsOutfit.GetSprites()[0].Depth + 0.001f));
			list29.Last().Origin = new Vector2(30f, 152f);
			List<float> list30 = new List<float>();
			list30.Add(0.2f);
			list30.Add(0.4f);
			list30.Add(0.6f);
			AnimatedRenderSprite spr3 = new AnimatedRenderSprite(list29, list30, repeats: true);
			PhysicalRepresentation objConnect3 = m_physicsOutfit.GetPhysicsObjects()[0];
			m_propEffectors.Add(new PropAnimator(spr3, objConnect3, 0));
		}
		else if (PropType == PropType.OTHERMOTHER)
		{
			List<PhysicalRepresentation> physicsObjects7 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list31 = new List<PhysicalRepresentation>();
			list31.Add(physicsObjects7[2]);
			list31.Add(physicsObjects7[3]);
			list31.Add(physicsObjects7[4]);
			list31.Add(physicsObjects7[5]);
			List<Vector2> list32 = new List<Vector2>();
			List<float> list33 = new List<float>();
			for (int j = 0; j < list31.Count; j++)
			{
				list32.Add(2f * new Vector2(400f, -300f));
				list33.Add(0f);
			}
			List<int> list34 = new List<int>();
			list34.Add(0);
			List<Joint> joints7 = new List<Joint>();
			m_physicsOutfit.GetJoints(list34, joints7);
			m_propEffectors.Add(new PropLauncher(joints7, list31, list32, list33));
		}
		else if (PropType == PropType.TISSUES)
		{
			List<ParticleEmitter> list35 = new List<ParticleEmitter>();
			List<SpriteImage> list36 = new List<SpriteImage>();
			list36.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet3", new Rectangle(928, 29, 69, 52)));
			list36.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet3", new Rectangle(916, 103, 89, 62)));
			list36.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet3", new Rectangle(911, 187, 88, 69)));
			for (int k = 0; k < list36.Count; k++)
			{
				list35.Add(new ParticleEmitter(list36[k], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 500, 1500, 600f, 900f, 3.6415927f, 1.5f, default(Vector2), 80f, 100f, 130f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(3, list35));
		}
		else if (PropType == PropType.FLOWER_TABLE)
		{
			List<ParticleEmitter> list37 = new List<ParticleEmitter>();
			List<SpriteImage> list38 = new List<SpriteImage>();
			list38.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet2", new Rectangle(497, 230, 18, 18)));
			list38.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet2", new Rectangle(481, 251, 14, 14)));
			list38.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet2", new Rectangle(521, 246, 17, 17)));
			list38.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet2", new Rectangle(504, 269, 16, 16)));
			list38.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet2", new Rectangle(483, 286, 18, 18)));
			list38.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet2", new Rectangle(522, 289, 19, 19)));
			for (int l = 0; l < list38.Count; l++)
			{
				list37.Add(new ParticleEmitter(list38[l], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 500, 1500, 600f, 900f, 3.6415927f, 1.5f, default(Vector2), 25f, 25f, 30f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(10, list37));
		}
		else if (PropType == PropType.DIRECTOR)
		{
			List<ParticleEmitter> list39 = new List<ParticleEmitter>();
			List<SpriteImage> list40 = new List<SpriteImage>();
			list40.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet3", new Rectangle(928, 29, 69, 52)));
			list40.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet3", new Rectangle(916, 103, 89, 62)));
			list40.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet3", new Rectangle(911, 187, 88, 69)));
			for (int m = 0; m < list40.Count; m++)
			{
				list39.Add(new ParticleEmitter(list40[m], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 500, 1500, 600f, 900f, 3.6415927f, 1.5f, default(Vector2), 80f, 100f, 130f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(3, list39));
		}
		else if (PropType == PropType.SURGERY_PATIENT)
		{
			List<ParticleEmitter> list41 = new List<ParticleEmitter>();
			List<SpriteImage> list42 = new List<SpriteImage>();
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(126, 805, 42, 36)));
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(28, 844, 35, 34)));
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(72, 845, 32, 32)));
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(108, 866, 34, 33)));
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(17, 943, 33, 33)));
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(49, 961, 63, 43)));
			list42.Add(TextureContainer.GetImage("images/Spritesheets/hospital/sheet6", new Rectangle(131, 962, 31, 31)));
			for (int n = 0; n < list42.Count; n++)
			{
				list41.Add(new ParticleEmitter(list42[n], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 500, 1500, 600f, 900f, 4.08407f, 2f, default(Vector2), 20f, 50f, 80f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(5, list41));
		}
		else if (PropType == PropType.TRIAL_BEAR)
		{
			m_physicsOutfit.SetForcedStatic(0);
			m_propEffectors.Add(new TrialUpseller(m_physicsOutfit.GetPhysicsObjects()[0]));
		}
		else if (PropType == PropType.GLASS_PANEL)
		{
			m_physicsOutfit.SetForcedStatic(0);
			m_propEffectors.Add(new GlassBreaker(m_physicsOutfit));
		}
		else if (PropType == PropType.GIANT_TV)
		{
			m_propEffectors.Add(new VirtualTransitioner());
		}
		else if (PropType == PropType.GRASS_CUTTER)
		{
			List<PhysicalRepresentation> physicsObjects8 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list43 = new List<PhysicalRepresentation>();
			list43.Add(physicsObjects8[1]);
			list43.Add(physicsObjects8[2]);
			list43.Add(physicsObjects8[3]);
			List<Vector2> list44 = new List<Vector2>();
			List<float> list45 = new List<float>();
			list44.Add(0.5f * new Vector2(400f, -500f));
			list45.Add(SceneRenderer.GetRand(1f, 2f));
			list44.Add(0.5f * new Vector2(400f, -500f));
			list45.Add(SceneRenderer.GetRand(1f, 2f));
			list44.Add(0.5f * new Vector2(400f, -500f));
			list45.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list46 = new List<int>();
			list46.Add(0);
			List<Joint> joints8 = new List<Joint>();
			m_physicsOutfit.GetJoints(list46, joints8);
			m_propEffectors.Add(new PropLauncher(joints8, list43, list44, list45));
		}
		else if (PropType == PropType.SKATEBOARDER)
		{
			List<PhysicalRepresentation> physicsObjects9 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list47 = new List<PhysicalRepresentation>();
			list47.Add(physicsObjects9[1]);
			list47.Add(physicsObjects9[2]);
			list47.Add(physicsObjects9[3]);
			list47.Add(physicsObjects9[4]);
			List<Vector2> list48 = new List<Vector2>();
			List<float> list49 = new List<float>();
			list48.Add(0.5f * new Vector2(400f, -500f));
			list49.Add(SceneRenderer.GetRand(1f, 2f));
			list48.Add(0.5f * new Vector2(400f, -500f));
			list49.Add(SceneRenderer.GetRand(1f, 2f));
			list48.Add(0.5f * new Vector2(400f, -500f));
			list49.Add(SceneRenderer.GetRand(1f, 2f));
			list48.Add(0.5f * new Vector2(400f, -500f));
			list49.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> indexes = new List<int>();
			List<Joint> joints9 = new List<Joint>();
			m_physicsOutfit.GetJoints(indexes, joints9);
			m_propEffectors.Add(new PropLauncher(joints9, list47, list48, list49));
		}
		else if (PropType == PropType.BIKE)
		{
			List<PhysicalRepresentation> physicsObjects10 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list50 = new List<PhysicalRepresentation>();
			list50.Add(physicsObjects10[1]);
			list50.Add(physicsObjects10[2]);
			list50.Add(physicsObjects10[3]);
			List<Vector2> list51 = new List<Vector2>();
			List<float> list52 = new List<float>();
			list51.Add(0.5f * new Vector2(400f, -500f));
			list52.Add(SceneRenderer.GetRand(1f, 2f));
			list51.Add(0.5f * new Vector2(400f, -500f));
			list52.Add(SceneRenderer.GetRand(1f, 2f));
			list51.Add(0.5f * new Vector2(400f, -500f));
			list52.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> list53 = new List<int>();
			list53.Add(0);
			List<Joint> joints10 = new List<Joint>();
			m_physicsOutfit.GetJoints(list53, joints10);
			m_propEffectors.Add(new PropLauncher(joints10, list50, list51, list52));
		}
		else if (PropType == PropType.PICNIC_FLOOR)
		{
			List<PhysicalRepresentation> physicsObjects11 = m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> list54 = new List<PhysicalRepresentation>();
			list54.Add(physicsObjects11[8]);
			list54.Add(physicsObjects11[9]);
			list54.Add(physicsObjects11[10]);
			list54.Add(physicsObjects11[11]);
			list54.Add(physicsObjects11[12]);
			List<Vector2> list55 = new List<Vector2>();
			List<float> list56 = new List<float>();
			list55.Add(new Vector2(SceneRenderer.GetRand(-200f, 200f), SceneRenderer.GetRand(0f, -130f)));
			list56.Add(SceneRenderer.GetRand(1f, 2f));
			list55.Add(new Vector2(SceneRenderer.GetRand(-200f, 200f), SceneRenderer.GetRand(0f, -130f)));
			list56.Add(SceneRenderer.GetRand(1f, 2f));
			list55.Add(new Vector2(SceneRenderer.GetRand(-200f, 200f), SceneRenderer.GetRand(0f, -130f)));
			list56.Add(SceneRenderer.GetRand(1f, 2f));
			list55.Add(new Vector2(SceneRenderer.GetRand(-200f, 200f), SceneRenderer.GetRand(0f, -130f)));
			list56.Add(SceneRenderer.GetRand(1f, 2f));
			list55.Add(new Vector2(SceneRenderer.GetRand(-200f, 200f), SceneRenderer.GetRand(0f, -130f)));
			list56.Add(SceneRenderer.GetRand(1f, 2f));
			List<int> indexes2 = new List<int>();
			List<Joint> joints11 = new List<Joint>();
			m_physicsOutfit.GetJoints(indexes2, joints11);
			m_propEffectors.Add(new PropLauncher(joints11, list54, list55, list56));
		}
		else if (PropType == PropType.DOG)
		{
			m_physicsOutfit.GetPhysicsObjects();
			List<PhysicalRepresentation> bodies = new List<PhysicalRepresentation>();
			List<Vector2> vel = new List<Vector2>();
			List<float> spins = new List<float>();
			List<int> list57 = new List<int>();
			list57.Add(2);
			List<Joint> joints12 = new List<Joint>();
			m_physicsOutfit.GetJoints(list57, joints12);
			m_propEffectors.Add(new PropLauncher(joints12, bodies, vel, spins));
		}
		else if (PropType == PropType.ICE_CREAM)
		{
			List<ParticleEmitter> list58 = new List<ParticleEmitter>();
			List<SpriteImage> list59 = new List<SpriteImage>();
			list59.Add(TextureContainer.GetImage("images/particles", new Rectangle(5, 8, 37, 72)));
			list59.Add(TextureContainer.GetImage("images/particles", new Rectangle(55, 8, 37, 72)));
			list59.Add(TextureContainer.GetImage("images/particles", new Rectangle(104, 8, 37, 72)));
			list59.Add(TextureContainer.GetImage("images/particles", new Rectangle(150, 8, 37, 72)));
			list59.Add(TextureContainer.GetImage("images/particles", new Rectangle(199, 8, 37, 72)));
			list59.Add(TextureContainer.GetImage("images/particles", new Rectangle(250, 8, 37, 72)));
			for (int num = 0; num < list59.Count; num++)
			{
				list58.Add(new ParticleEmitter(list59[num], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 3000, 3000, 600f, 900f, 4.08407f, 2f, default(Vector2), 40f, 40f, 40f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(2, list58));
		}
		else if (PropType == PropType.CLOWN)
		{
			List<ParticleEmitter> list60 = new List<ParticleEmitter>();
			List<SpriteImage> list61 = new List<SpriteImage>();
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(308, 15, 40, 40)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(362, 8, 38, 42)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(411, 14, 40, 40)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(464, 19, 34, 33)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(302, 69, 44, 41)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(357, 52, 44, 52)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(413, 60, 39, 43)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(464, 64, 45, 45)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(304, 118, 44, 40)));
			list61.Add(TextureContainer.GetImage("images/particles", new Rectangle(363, 110, 40, 47)));
			for (int num2 = 0; num2 < list61.Count; num2++)
			{
				list60.Add(new ParticleEmitter(list61[num2], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 3000, 3000, 600f, 900f, 4.08407f, 2f, default(Vector2), 60f, 80f, 100f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(1, list60));
		}
		else if (PropType == PropType.PAINTER)
		{
			List<ParticleEmitter> list62 = new List<ParticleEmitter>();
			List<SpriteImage> list63 = new List<SpriteImage>();
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(311, 195, 45, 29)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(362, 191, 33, 32)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(406, 181, 34, 40)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(452, 194, 31, 28)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(298, 230, 24, 23)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(361, 230, 22, 23)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(406, 232, 23, 24)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(446, 234, 24, 27)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(325, 252, 29, 27)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(390, 260, 22, 23)));
			list63.Add(TextureContainer.GetImage("images/particles", new Rectangle(438, 267, 24, 25)));
			for (int num3 = 0; num3 < list63.Count; num3++)
			{
				list62.Add(new ParticleEmitter(list63[num3], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 3000, 3000, 600f, 900f, 3.6415927f, 1.5f, default(Vector2), 20f, 40f, 60f, (float)Math.PI * 2f, 1000));
			}
			m_propEffectors.Add(new PropParticleSpawner(1, list62));
		}
		else
		{
			if (PropType == PropType.BBQ)
			{
				return;
			}
			if (PropType == PropType.PIPE)
			{
				List<PhysicalRepresentation> physicsObjects12 = m_physicsOutfit.GetPhysicsObjects();
				List<PhysicalRepresentation> list64 = new List<PhysicalRepresentation>();
				list64.Add(physicsObjects12[1]);
				List<Vector2> list65 = new List<Vector2>();
				List<float> list66 = new List<float>();
				list65.Add(new Vector2(400f, -500f));
				list66.Add(SceneRenderer.GetRand(1f, 2f));
				List<int> list67 = new List<int>();
				list67.Add(0);
				list67.Add(1);
				List<Joint> joints13 = new List<Joint>();
				m_physicsOutfit.GetJoints(list67, joints13);
				m_propEffectors.Add(new PropLauncher(joints13, list64, list65, list66));
				List<ParticleEmitter> list68 = new List<ParticleEmitter>();
				list64 = new List<PhysicalRepresentation>();
				List<Vector2> list69 = new List<Vector2>();
				list68.Add(new ParticleEmitter(TextureContainer.GetImage("images/particles", new Rectangle(465, 321, 29, 29)), default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 2000, 2000, 400f, 600f, 0f, 0.4f, default(Vector2), 40f, 70f, 90f, (float)Math.PI * 2f, 10));
				list68.Add(new ParticleEmitter(TextureContainer.GetImage("images/particles", new Rectangle(465, 321, 29, 29)), default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 2000, 2000, 400f, 600f, (float)Math.PI, 0.4f, default(Vector2), 40f, 70f, 90f, (float)Math.PI * 2f, 10));
				list64.Add(m_physicsOutfit.GetPhysicsObjects()[1]);
				list64.Add(m_physicsOutfit.GetPhysicsObjects()[1]);
				list69.Add(new Vector2(-70f, -250f));
				list69.Add(new Vector2(160f, -250f));
				m_propEffectors.Add(new PropParticleStream(list68, list64, list69, startsActive: false));
			}
			else if (PropType == PropType.VOLLEYBALL)
			{
				List<PhysicalRepresentation> physicsObjects13 = m_physicsOutfit.GetPhysicsObjects();
				new List<PhysicalRepresentation>();
				m_propEffectors.Add(new PropObjectVelocitizer(physicsObjects13[7], new Vector2(-300f, -300f)));
			}
			else if (PropType == PropType.FLOPPY_DESK)
			{
				List<PhysicalRepresentation> physicsObjects14 = m_physicsOutfit.GetPhysicsObjects();
				List<PhysicalRepresentation> list70 = new List<PhysicalRepresentation>();
				list70.Add(physicsObjects14[1]);
				list70.Add(physicsObjects14[2]);
				list70.Add(physicsObjects14[3]);
				list70.Add(physicsObjects14[4]);
				list70.Add(physicsObjects14[5]);
				list70.Add(physicsObjects14[6]);
				List<Vector2> list71 = new List<Vector2>();
				List<float> list72 = new List<float>();
				for (int num4 = 0; num4 < 6; num4++)
				{
					list71.Add(new Vector2(SceneRenderer.GetRand(-300f, 300f), SceneRenderer.GetRand(-300f, 0f)));
					list72.Add(SceneRenderer.GetRand(1f, 2f));
				}
				List<int> indexes3 = new List<int>();
				List<Joint> joints14 = new List<Joint>();
				m_physicsOutfit.GetJoints(indexes3, joints14);
				m_propEffectors.Add(new PropLauncher(joints14, list70, list71, list72));
			}
			else if (PropType == PropType.CELL_BOOTH)
			{
				List<PhysicalRepresentation> physicsObjects15 = m_physicsOutfit.GetPhysicsObjects();
				List<PhysicalRepresentation> list73 = new List<PhysicalRepresentation>();
				list73.Add(physicsObjects15[4]);
				list73.Add(physicsObjects15[5]);
				list73.Add(physicsObjects15[6]);
				list73.Add(physicsObjects15[7]);
				list73.Add(physicsObjects15[8]);
				list73.Add(physicsObjects15[9]);
				List<Vector2> list74 = new List<Vector2>();
				List<float> list75 = new List<float>();
				for (int num5 = 0; num5 < 6; num5++)
				{
					list74.Add(new Vector2(2f * SceneRenderer.GetRand(-100f, 100f), SceneRenderer.GetRand(-300f, 0f)));
					list75.Add(SceneRenderer.GetRand(1f, 2f));
				}
				List<int> list76 = new List<int>();
				list76.Add(3);
				list76.Add(4);
				list76.Add(5);
				list76.Add(6);
				List<Joint> joints15 = new List<Joint>();
				m_physicsOutfit.GetJoints(list76, joints15);
				m_propEffectors.Add(new PropLauncher(joints15, list73, list74, list75));
			}
			else if (PropType == PropType.ATM)
			{
				List<ParticleEmitter> list77 = new List<ParticleEmitter>();
				List<SpriteImage> list78 = new List<SpriteImage>();
				list78.Add(TextureContainer.GetImage("images/particles", new Rectangle(22, 124, 52, 35)));
				for (int num6 = 0; num6 < list78.Count; num6++)
				{
					list77.Add(new ParticleEmitter(list78[num6], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 3000, 3000, 600f, 900f, 4.08407f, 2f, default(Vector2), 50f, 80f, 80f, (float)Math.PI * 2f, 1000));
				}
				m_propEffectors.Add(new PropParticleSpawner(14, list77));
			}
			else if (PropType == PropType.PEARL_NECKLACE)
			{
				List<ParticleEmitter> list79 = new List<ParticleEmitter>();
				List<SpriteImage> list80 = new List<SpriteImage>();
				list80.Add(TextureContainer.GetImage("images/particles", new Rectangle(229, 127, 29, 30)));
				for (int num7 = 0; num7 < list80.Count; num7++)
				{
					list79.Add(new ParticleEmitter(list80[num7], default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 3000, 3000, 600f, 900f, 4.08407f, 2f, default(Vector2), 30f, 30f, 30f, (float)Math.PI * 2f, 1000));
				}
				m_propEffectors.Add(new PropParticleSpawner(7, list79));
			}
			else
			{
				if (PropType == PropType.COFFEECHAIRTABLE2 || PropType == PropType.COFFEECOUCHTABLE2 || PropType == PropType.COFFEESTOOLTABLE2)
				{
					return;
				}
				if (PropType == PropType.CAMERA_DESK)
				{
					List<ParticleEmitter> list81 = new List<ParticleEmitter>();
					List<SpriteImage> list82 = new List<SpriteImage>();
					list82.Add(TextureContainer.GetImage("images/particle"));
					for (int num8 = 0; num8 < list82.Count; num8++)
					{
						list81.Add(new ParticleEmitter(list82[num8], default(Vector2), 1000f, fades: true, additive: true, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 0f), 0.1f, default(Vector2), 1000, 1000, 0f, 0f, 4.08407f, 2f, default(Vector2), 2000f, 2000f, 2000f, (float)Math.PI * 2f, 1000));
					}
					m_propEffectors.Add(new PropParticleSpawner(7, list81));
				}
				else if (PropType == PropType.MALL_FOUNTAIN)
				{
					List<ParticleEmitter> list83 = new List<ParticleEmitter>();
					List<PhysicalRepresentation> list84 = new List<PhysicalRepresentation>();
					List<Vector2> list85 = new List<Vector2>();
					list83.Add(new ParticleEmitter(TextureContainer.GetImage("images/particles", new Rectangle(465, 321, 29, 29)), default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 2000, 2000, 400f, 600f, -(float)Math.PI / 2f, 0.2f, default(Vector2), 40f, 70f, 90f, (float)Math.PI * 2f, 20));
					list83.Add(new ParticleEmitter(TextureContainer.GetImage("images/particles", new Rectangle(465, 321, 29, 29)), default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 2000, 2000, 400f, 500f, -(float)Math.PI / 2f, 0.2f, default(Vector2), 40f, 70f, 90f, (float)Math.PI * 2f, 20));
					list83.Add(new ParticleEmitter(TextureContainer.GetImage("images/particles", new Rectangle(465, 321, 29, 29)), default(Vector2), 0f, fades: true, additive: false, Color.White, Color.White, Color.White, Color.White, new Vector2(0f, 500f), 0.1f, default(Vector2), 2000, 2000, 400f, 500f, -(float)Math.PI / 2f, 0.2f, default(Vector2), 40f, 70f, 90f, (float)Math.PI * 2f, 20));
					list84.Add(m_physicsOutfit.GetPhysicsObjects()[0]);
					list84.Add(m_physicsOutfit.GetPhysicsObjects()[0]);
					list84.Add(m_physicsOutfit.GetPhysicsObjects()[0]);
					list85.Add(new Vector2(0f, 0f));
					list85.Add(new Vector2(-150f, 0f));
					list85.Add(new Vector2(150f, 0f));
					m_propEffectors.Add(new PropParticleStream(list83, list84, list85, startsActive: true));
				}
				else if ((PropType == PropType.EASY_GLOW || PropType == PropType.MED_GLOW || PropType == PropType.HARD_GLOW || PropType == PropType.VHARD_GLOW) && m_physicsOutfit.GetSprites()[0].SurfaceScale.Y < 200f)
				{
					m_propEffectors.Add(new VirtualFloater(m_physicsOutfit.GetPhysicsObjects()[0]));
				}
			}
		}
	}

	public PhysicsOutfit GetOutfit()
	{
		return m_physicsOutfit;
	}

	public void SetDepth(float f)
	{
		m_physicsOutfit.SetDepth(f);
	}

	public void Update(TimeTracker gameTime)
	{
		m_physicsOutfit.Update(gameTime);
		for (int i = 0; i < m_propEffectors.Count; i++)
		{
			m_propEffectors[i].Update(gameTime);
		}
		if (!m_physicsOutfit.IsGlowing)
		{
			m_spawner.Update(gameTime);
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		m_physicsOutfit.Draw(gameTime);
		for (int i = 0; i < m_propEffectors.Count; i++)
		{
			m_propEffectors[i].Draw(gameTime);
		}
		if (!m_physicsOutfit.IsGlowing)
		{
			m_spawner.Draw(gameTime);
		}
	}

	public void ResetToLocation(Vector2 v)
	{
		for (int i = 0; i < m_propEffectors.Count; i++)
		{
			m_propEffectors[i].Reset();
		}
		m_physicsOutfit.ResetToPosition(v);
		m_location = v;
		m_bDisabled = false;
		m_physicsOutfit.IsGlowing = true;
	}

	private void InitOutfit(PhysicsOutfit outfit, PhysicsOutfit clone)
	{
		outfit.Initialize(clone);
		outfit.SetCollisionHandler(CollisionHandler);
	}

	public bool CollisionHandler(Fixture f1, Fixture f2, Contact contactList)
	{
		if (f2.CollisionFilter.CollisionCategories == PhysicsObjectManager.PlayerCollisionGroup() && (PropType != PropType.TRIAL_BEAR || !Game1.IsTrial()))
		{
			if (!m_bDisabled)
			{
				if (PropType == PropType.TRIAL_BEAR)
				{
					m_physicsOutfit.RemoveForcedStatic(0);
				}
				Player player = (Player)PhysicsObjectManager.GetPlayer(f2.Body);
				player.SaveFrameData(m_type);
				m_bDisabled = true;
				m_physicsOutfit.IsGlowing = false;
				contactList.GetWorldManifold(out var _, out var points);
				Vector2 pos = points[0];
				pos *= 100f;
				m_spawner.Initialize(pos);
				for (int i = 0; i < m_propEffectors.Count; i++)
				{
					m_propEffectors[i].CollisionResponse(player, pos);
				}
				SoundEffects.PlayCollisionSound();
				return true;
			}
			return false;
		}
		return true;
	}

	public void UpdateEnabled()
	{
		m_physicsOutfit.UpdateEnabled();
	}
}
