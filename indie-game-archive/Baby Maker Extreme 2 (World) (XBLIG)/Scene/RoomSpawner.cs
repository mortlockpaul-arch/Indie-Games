using System;
using System.Collections.Generic;
using BabyMakerExtreme2;
using Microsoft.Xna.Framework;
using PlayObjects;
using Renderer;

namespace Scene;

public class RoomSpawner
{
	private enum SPAWN_STRATEGIES
	{
		BIRTH_ROOM,
		MATERNITY_WARD1,
		MATERNITY_WARD2,
		MATERNITY_WARD3,
		MATERNITY_WARD4,
		MATERNITY_WARD5,
		TRIAGE1,
		TRIAGE2,
		TRIAGE3,
		BEDS_ROOM1,
		BEDS_ROOM2,
		MRI_ROOM,
		TV_WAITING1,
		TV_WAITING2,
		DIRECTOR,
		DIAGNOSTICS,
		SURGERY,
		ONEONONEDIAG1,
		ONEONONEDIAG2,
		ONEONONEDIAG3,
		GLASS_PANEL,
		MAX_ROOM
	}

	private enum PARK_SPAWN_STRATEGIES
	{
		GENERIC_PARKLANDS,
		GENERIC_PARKLANDS_GLOWERS,
		PLAYGROUND,
		DUMP,
		SKATEPARK,
		TRIAL_BEAR,
		BIRTH_ROOM,
		MAX_ROOM
	}

	private enum MALL_SPAWN_STRATEGIES
	{
		HALL,
		TOY,
		TOY_END,
		JEWELLERY,
		JEWELLERY_END,
		CLOTHING,
		CLOTHING_END,
		COFFEE,
		COFFEE_END,
		ELECTRONICS,
		PEOPLE,
		GIANT_TV_END,
		BIRTH_ROOM,
		MAX_ROOM
	}

	private enum VIRTUAL_SPAWN_STRATEGIES
	{
		NORM_ITEM,
		GLOW_ITEM,
		MAX_ROOM
	}

	private const int ROOM_TYPES = 10;

	private const int ROOM_TYPES_MALL = 6;

	private List<SpawnStrategy> m_spawnStrategiesHospital;

	private List<SpawnStrategy> m_spawnStrategiesPark;

	private List<SpawnStrategy> m_spawnStrategiesMall;

	private List<SpawnStrategy> m_spawnStrategiesVirtual;

	private int m_iLastRoom;

	private int m_iRoomType;

	private bool m_bStartTracker;

	private float m_fTrackStartPos;

	private float m_fTrackCurPos;

	private List<int> m_HospitalRoomTracker;

	private bool m_bSpawnedTrialBear;

	public RoomSpawner(SceneObjectSpawner spawner)
	{
		m_bSpawnedTrialBear = !Game1.IsTrial();
		m_iRoomType = 0;
		m_iLastRoom = -1;
		m_fTrackCurPos = 0f;
		m_fTrackStartPos = 0f;
		m_bStartTracker = true;
		m_spawnStrategiesHospital = new List<SpawnStrategy>();
		for (int i = 0; i < 21; i++)
		{
			m_spawnStrategiesHospital.Add(new SpawnStrategy(spawner));
		}
		m_HospitalRoomTracker = new List<int>();
		for (int j = 1; j < 10; j++)
		{
			m_HospitalRoomTracker.Add(j);
		}
		m_spawnStrategiesHospital[0].AddType(PropType.BIRTHMOTHER, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[0].AddType(PropType.BABY_BAG, 100, 1f, 5f, 50f);
		m_spawnStrategiesHospital[0].AddType(PropType.NURSE, 100, 1f, 30f, 50f);
		m_spawnStrategiesHospital[0].AddType(PropType.TRAY, 100, 1f, 30f, 50f);
		m_spawnStrategiesHospital[0].AddType(PropType.BABY_MONITOR, 100, 1f, 40f, 50f);
		m_spawnStrategiesHospital[0].AddType(PropType.BASONETTE, 100, 1f, 10f, 50f);
		m_spawnStrategiesHospital[0].AddType(PropType.BEAR_FATHER, 100, 1f, 140f, 50f);
		m_spawnStrategiesHospital[1].AddType(PropType.OTHERMOTHER, 1, 1f, 100f, 40f);
		m_spawnStrategiesHospital[2].AddType(PropType.BABY_MONITOR, 1, 1f, 60f, 60f);
		m_spawnStrategiesHospital[2].AddType(PropType.BABY_BAG, 1, 1f, 10f, 10f);
		m_spawnStrategiesHospital[3].AddType(PropType.SLEEPING_FATHER, 1, 1f, 100f, 60f);
		m_spawnStrategiesHospital[3].AddType(PropType.BEAR_FATHER, 1, 1f, 100f, 60f);
		m_spawnStrategiesHospital[3].AddType(PropType.CHAIR_EMPTY, 1, 1f, 30f, 30f);
		m_spawnStrategiesHospital[4].AddType(PropType.NURSE, 1, 1f, 100f, 20f);
		m_spawnStrategiesHospital[4].AddType(PropType.DOCTOR, 1, 1f, 100f, 20f);
		m_spawnStrategiesHospital[4].AddType(PropType.TRAY, 1, 1f, 50f, 20f);
		m_spawnStrategiesHospital[5].AddType(PropType.BOTTLE_TABLE, 1, 1f, 10f, 10f);
		m_spawnStrategiesHospital[5].AddType(PropType.TRAY, 1, 1f, 10f, 10f);
		m_spawnStrategiesHospital[5].AddType(PropType.BASONETTE, 2, 1f, 10f, 10f);
		m_spawnStrategiesHospital[5].AddType(PropType.CHANGING_TABLE, 1, 1f, 10f, 10f);
		m_spawnStrategiesHospital[5].AddType(PropType.DIAPER_PILE, 1, 1f, 10f, 10f);
		m_spawnStrategiesHospital[6].AddType(PropType.CHAIR_EMPTY, 100, 1f, 20f, 20f);
		m_spawnStrategiesHospital[6].AddType(PropType.CHAIR_FULL, 100, 1f, 20f, 20f);
		m_spawnStrategiesHospital[7].AddType(PropType.COATRACK, 2, 1f, 30f, 30f);
		m_spawnStrategiesHospital[7].AddType(PropType.CRASHCART, 1, 1f, 100f, 60f);
		m_spawnStrategiesHospital[7].AddType(PropType.HEAD_TRAUMA, 1, 1f, 70f, 50f);
		m_spawnStrategiesHospital[7].AddType(PropType.FLOWER_TABLE, 3, 1f, 50f, 50f);
		m_spawnStrategiesHospital[7].AddType(PropType.LAMP, 1, 1f, 50f, 50f);
		m_spawnStrategiesHospital[7].AddType(PropType.OLD_MAN, 1, 1f, 50f, 50f);
		m_spawnStrategiesHospital[7].AddType(PropType.SHELF_BIG, 1, 1f, 60f, 60f);
		m_spawnStrategiesHospital[7].AddType(PropType.METAL_CABINET, 1, 1f, 50f, 50f);
		m_spawnStrategiesHospital[7].AddType(PropType.TISSUES, 3, 1f, 50f, 50f);
		m_spawnStrategiesHospital[7].AddType(PropType.WHEELCHAIR, 1, 1f, 50f, 50f);
		m_spawnStrategiesHospital[8].AddType(PropType.RECEPTION, 1, 1f, 80f, 80f);
		m_spawnStrategiesHospital[9].AddType(PropType.CURTAIN, 1, 1f, 10f, 10f);
		m_spawnStrategiesHospital[9].AddType(PropType.BED_EMPTY, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[9].AddType(PropType.BED_FULL, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[9].AddType(PropType.BODYCAST, 2, 1f, 10f, 10f);
		m_spawnStrategiesHospital[10].AddType(PropType.BEDSIDE_TABLE, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[10].AddType(PropType.FLOWER_TABLE, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[10].AddType(PropType.TISSUES, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[10].AddType(PropType.DOCTOR, 2, 1f, 10f, 10f);
		m_spawnStrategiesHospital[10].AddType(PropType.NURSE, 2, 1f, 10f, 10f);
		m_spawnStrategiesHospital[11].AddType(PropType.MRI_DOCTOR, 100, 1f, 80f, 10f);
		m_spawnStrategiesHospital[11].AddType(PropType.MRI_MACHINE, 100, 1f, 200f, 70f);
		m_spawnStrategiesHospital[12].AddType(PropType.TV_WATCHER, 100, 1f, 30f, 10f);
		m_spawnStrategiesHospital[12].AddType(PropType.SLEEPING_FATHER, 1, 1f, 30f, 10f);
		m_spawnStrategiesHospital[13].AddType(PropType.TV, 1, 1f, 100f, 70f);
		m_spawnStrategiesHospital[14].AddType(PropType.BOX, 100, 1f, 80f, 10f);
		m_spawnStrategiesHospital[14].AddType(PropType.DIRECTOR, 100, 1f, 30f, 10f);
		m_spawnStrategiesHospital[14].AddType(PropType.COATRACK, 100, 1f, 30f, 10f);
		m_spawnStrategiesHospital[14].AddType(PropType.LAMP, 100, 1f, 30f, 10f);
		m_spawnStrategiesHospital[15].AddType(PropType.CONF_TABLE, 100, 1f, 10f, 40f);
		m_spawnStrategiesHospital[15].AddType(PropType.DIAG_BOARD, 100, 1f, 80f, 10f);
		m_spawnStrategiesHospital[15].AddType(PropType.HOUSE_DESK, 100, 1f, 80f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.SURGERY_LIGHT, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.TRAY, 100, 1f, 10f, 60f);
		m_spawnStrategiesHospital[16].AddType(PropType.SURGERY_PATIENT, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.SURGERY_PATIENT, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.TRAY, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.DOCTOR, 100, 1f, 60f, 70f);
		m_spawnStrategiesHospital[16].AddType(PropType.SINK, 100, 1f, 70f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.SINK, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.SINK, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.COATRACK, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[16].AddType(PropType.COATRACK, 100, 1f, 10f, 10f);
		m_spawnStrategiesHospital[17].AddType(PropType.XRAY_FULL, 1, 1f, 50f, 60f);
		m_spawnStrategiesHospital[17].AddType(PropType.XRAY_DOUBLE, 1, 1f, 50f, 60f);
		m_spawnStrategiesHospital[17].AddType(PropType.PROCTOL_PATIENT, 1, 1f, 100f, 10f);
		m_spawnStrategiesHospital[17].AddType(PropType.HEAD_TRAUMA, 1, 0.6f, 100f, 60f);
		m_spawnStrategiesHospital[18].AddType(PropType.CHECKUP_DOC, 1, 1f, 30f, 20f);
		m_spawnStrategiesHospital[19].AddType(PropType.CHECKUP_DESK_COTTON, 1, 1f, 80f, 40f);
		m_spawnStrategiesHospital[19].AddType(PropType.CHECKUP_DESK_SKULL, 1, 1f, 80f, 40f);
		m_spawnStrategiesHospital[20].AddType(PropType.GLASS_PANEL, 1, 1f, 0f, 0f);
		m_spawnStrategiesPark = new List<SpawnStrategy>();
		for (int k = 0; k < 7; k++)
		{
			m_spawnStrategiesPark.Add(new SpawnStrategy(spawner));
		}
		m_spawnStrategiesPark[6].AddType(PropType.AMBULANCE, 100, 1f, 10f, 10f);
		m_spawnStrategiesPark[6].AddType(PropType.DOCTOR, 100, 1f, 60f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.PORTAPOTTY, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.BUSH1, 3, 1.3f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.BUSH2, 3, 1.3f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.BUSH3, 3, 1.3f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.GARBAGE_CAN, 3, 2.3f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.FOUNDTAIN_BIG, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.FOUNTAIN_SMALL, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.BENCH1, 3, 1.5f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.BENCH2, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.BENCH3, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.PICNICTABLE1, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.PICNICTABLE3, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.FEM_RIGHT_BLUE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.GUY_LEFT_BALD, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.PERSON_LEFT_FRO, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.PURSE_LADY_LEFT, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[0].AddType(PropType.PUNK_PERSON_RIGHT, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.DOG, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.PICNICTABLE2FAT, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.BBQ, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.PICNIC_FLOOR, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.ICE_CREAM, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.STATUE, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.BIKE, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.RUNNER1, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.RUNNER2, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.STATUE, 3, 1f, 100f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.GRASS_CUTTER, 3, 1f, 100f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.CLOWN, 3, 1f, 100f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.VOLLEYBALL, 3, 1f, 100f, 50f);
		m_spawnStrategiesPark[1].AddType(PropType.PAINTER, 3, 1f, 100f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.DUMP_SIGN, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.BLOCK_CINDER, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.GARBAGE_BAG_PILE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.TIRE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.TOILET, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.BOX_GARBAGE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.GARBAGE_BAG2, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.CONTAINER_GARBAGE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.PIPE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.GARBAGE_PILE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.BONE, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[3].AddType(PropType.GARBAGE_BAG1, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[2].AddType(PropType.SLIDE, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[2].AddType(PropType.BOUNCY_HIPPO, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[2].AddType(PropType.BOUNCY_DRAGON, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[2].AddType(PropType.TEETER_TOTTER, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[2].AddType(PropType.JUNGLE_GYM, 3, 1f, 50f, 50f);
		m_spawnStrategiesPark[4].AddType(PropType.RAMPDOWN, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[4].AddType(PropType.SKATEBOARDER, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[4].AddType(PropType.RAMPUP, 1, 1f, 50f, 50f);
		m_spawnStrategiesPark[5].AddType(PropType.TRIAL_BEAR, 1, 1f, 100f, 100f);
		m_spawnStrategiesMall = new List<SpawnStrategy>();
		for (int l = 0; l < 13; l++)
		{
			m_spawnStrategiesMall.Add(new SpawnStrategy(spawner));
		}
		m_spawnStrategiesMall[12].AddType(PropType.BIRTH_CURTAIN, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[12].AddType(PropType.DOCTOR, 1, 1f, 100f, 50f);
		m_spawnStrategiesMall[12].AddType(PropType.BLOOD_PRESSURE, 1, 1f, 150f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.SANITIZER, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.SALE_SIGN, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.MALL_CHAIRS, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.TINY_TREE, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.MALL_FOUNTAIN, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.MALL_SHRUB, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.CELL_BOOTH, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[0].AddType(PropType.ATM, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.TOY_BEAR, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.ROBOT, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.SNAKE_POLE, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.BLOCK_STACK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.TOY_SHELF, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.TRAIN_SET, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.TOY_RACK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[1].AddType(PropType.TRICYCLE, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[2].AddType(PropType.TOY_CASHIER, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[3].AddType(PropType.PEARL_NECKLACE, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[3].AddType(PropType.JEWEL_TABLE1, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[3].AddType(PropType.JEWEL_TABLE2, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[3].AddType(PropType.JEWEL_TABLE3, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[3].AddType(PropType.NECKLACE_RACK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[4].AddType(PropType.JEWELERY_CASHIER, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.CLOTH_TABLE1, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.CLOTH_TABLE2, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.CLOTH_TABLE3, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.WOOD_CHAIR, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.MIRROR, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.DUMMY, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[5].AddType(PropType.SHIRT_RACK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[6].AddType(PropType.CLOTHING_CASHIER, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEECHAIRTABLE1, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEECHAIRTABLE2, 3, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEESTOOLTABLE1, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEESTOOLTABLE2, 3, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEECOUCHTABLE1, 3, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEECOUCHTABLE2, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[7].AddType(PropType.COFFEESHELF, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[8].AddType(PropType.COFFEE_CASHIER, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[8].AddType(PropType.COFFEE_PRESS, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.COMPUTER_DESK1, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.COMPUTER_DESK2, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.CAMERA_DESK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.OLD_COMPUTER_DESK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.FLOPPY_DESK, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.SOFTWARE_SHELF, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.BIG_SCREEN, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[9].AddType(PropType.COMPUTER_CASHIER, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[10].AddType(PropType.FEM_RIGHT_BLUE, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[10].AddType(PropType.GUY_LEFT_BALD, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[10].AddType(PropType.PERSON_LEFT_FRO, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[10].AddType(PropType.PURSE_LADY_LEFT, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[10].AddType(PropType.PUNK_PERSON_RIGHT, 1, 1f, 50f, 50f);
		m_spawnStrategiesMall[11].AddType(PropType.GIANT_TV, 1, 1f, 0f, 500f);
		m_spawnStrategiesVirtual = new List<SpawnStrategy>();
		for (int m = 0; m < 2; m++)
		{
			m_spawnStrategiesVirtual.Add(new SpawnStrategy(spawner));
		}
		m_spawnStrategiesVirtual[1].AddType(PropType.EASY_GLOW, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[1].AddType(PropType.MED_GLOW, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[1].AddType(PropType.HARD_GLOW, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[1].AddType(PropType.VHARD_GLOW, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[0].AddType(PropType.EASY_NORM, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[0].AddType(PropType.MED_NORM, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[0].AddType(PropType.HARD_NORM, 10, 1f, 3f, 3f);
		m_spawnStrategiesVirtual[0].AddType(PropType.VHARD_NORM, 10, 1f, 3f, 3f);
	}

	public float GenerateRoom(int roomType, float startPos)
	{
		switch (roomType)
		{
		case 0:
			startPos += m_spawnStrategiesHospital[0].SpawnFlat(startPos);
			m_spawnStrategiesHospital[0].Reset();
			break;
		case 1:
		{
			startPos += m_spawnStrategiesHospital[1].SpawnEnemies(1, startPos);
			int count2 = (int)SceneRenderer.GetRand(0f, 2f);
			startPos += m_spawnStrategiesHospital[2].SpawnEnemies(count2, startPos);
			if (SceneRenderer.GetRand(0f, 1f) < 0.3f)
			{
				startPos += m_spawnStrategiesHospital[3].SpawnEnemies(1, startPos);
			}
			count2 = (int)SceneRenderer.GetRand(0f, 2f);
			startPos += m_spawnStrategiesHospital[4].SpawnEnemies(count2, startPos);
			count2 = (int)SceneRenderer.GetRand(0f, 3f);
			startPos += m_spawnStrategiesHospital[5].SpawnEnemies(count2, startPos);
			m_spawnStrategiesHospital[1].Reset();
			m_spawnStrategiesHospital[2].Reset();
			m_spawnStrategiesHospital[3].Reset();
			m_spawnStrategiesHospital[4].Reset();
			m_spawnStrategiesHospital[5].Reset();
			break;
		}
		case 2:
		{
			int num = (int)SceneRenderer.GetRand(3f, 6f) * 2 + 1;
			bool flag = SceneRenderer.GetRand(0f, 1f) < 0.5f;
			for (int i = 0; i < num; i++)
			{
				if (i == num / 2)
				{
					startPos += m_spawnStrategiesHospital[8].SpawnEnemies(1, startPos);
					continue;
				}
				if (flag)
				{
					int count = (int)SceneRenderer.GetRand(2f, 6f);
					startPos += m_spawnStrategiesHospital[6].SpawnEnemies(count, startPos);
				}
				else
				{
					startPos += m_spawnStrategiesHospital[7].SpawnEnemies(1, startPos);
				}
				flag = !flag;
			}
			m_spawnStrategiesHospital[6].Reset();
			m_spawnStrategiesHospital[7].Reset();
			m_spawnStrategiesHospital[8].Reset();
			break;
		}
		case 3:
		{
			int num2 = (int)SceneRenderer.GetRand(3f, 7f);
			for (int j = 0; j < num2; j++)
			{
				if (SceneRenderer.GetRand(0f, 1f) < 0.5f)
				{
					startPos += m_spawnStrategiesHospital[9].SpawnEnemies(1, startPos);
					startPos += m_spawnStrategiesHospital[10].SpawnEnemies(1, startPos);
				}
				else
				{
					startPos += m_spawnStrategiesHospital[10].SpawnEnemies(1, startPos);
					startPos += m_spawnStrategiesHospital[9].SpawnEnemies(1, startPos);
				}
				startPos += 300f;
			}
			m_spawnStrategiesHospital[9].Reset();
			m_spawnStrategiesHospital[10].Reset();
			break;
		}
		case 4:
			startPos += m_spawnStrategiesHospital[11].SpawnFlat(startPos);
			m_spawnStrategiesHospital[11].Reset();
			break;
		case 5:
			startPos += m_spawnStrategiesHospital[12].SpawnEnemies((int)SceneRenderer.GetRand(3f, 8f), startPos);
			startPos += m_spawnStrategiesHospital[13].SpawnEnemies(1, startPos);
			m_spawnStrategiesHospital[12].Reset();
			m_spawnStrategiesHospital[13].Reset();
			break;
		case 6:
			startPos += m_spawnStrategiesHospital[14].SpawnFlat(startPos);
			m_spawnStrategiesHospital[14].Reset();
			break;
		case 7:
			startPos += m_spawnStrategiesHospital[15].SpawnFlat(startPos);
			m_spawnStrategiesHospital[15].Reset();
			break;
		case 8:
			startPos += m_spawnStrategiesHospital[16].SpawnFlat(startPos);
			m_spawnStrategiesHospital[16].Reset();
			break;
		default:
			startPos += m_spawnStrategiesHospital[17].SpawnEnemies(1, startPos);
			startPos += m_spawnStrategiesHospital[18].SpawnEnemies(1, startPos);
			startPos += m_spawnStrategiesHospital[19].SpawnEnemies(1, startPos);
			if (m_spawnStrategiesHospital[17].NumLeft <= 1)
			{
				m_spawnStrategiesHospital[17].Reset();
			}
			m_spawnStrategiesHospital[18].Reset();
			m_spawnStrategiesHospital[19].Reset();
			break;
		}
		return startPos;
	}

	public float GenerateParkStart(float startPos)
	{
		startPos += m_spawnStrategiesPark[6].SpawnFlat(startPos);
		m_spawnStrategiesPark[6].Reset();
		return startPos;
	}

	public float GenerateParkRoom(float startPos)
	{
		int num = (int)SceneRenderer.GetRand(0f, 10f);
		if (!m_bSpawnedTrialBear && m_fTrackCurPos - m_fTrackStartPos > 3000f && Game1.IsTrial())
		{
			startPos += m_spawnStrategiesPark[5].SpawnFlat(startPos);
			m_spawnStrategiesPark[5].Reset();
			m_bSpawnedTrialBear = true;
		}
		else
		{
			switch (num)
			{
			case 0:
				startPos += m_spawnStrategiesPark[4].SpawnFlat(startPos);
				m_spawnStrategiesPark[4].Reset();
				break;
			case 1:
				startPos += m_spawnStrategiesPark[2].SpawnFlat(startPos);
				m_spawnStrategiesPark[2].Reset();
				break;
			case 2:
				startPos += m_spawnStrategiesPark[3].SpawnFlat(startPos);
				m_spawnStrategiesPark[3].Reset();
				break;
			default:
			{
				int count = (int)SceneRenderer.GetRand(1f, 4f);
				int count2 = (int)SceneRenderer.GetRand(1f, 4f);
				startPos += m_spawnStrategiesPark[0].SpawnEnemies(count, startPos);
				startPos += m_spawnStrategiesPark[1].SpawnEnemies(1, startPos);
				startPos += m_spawnStrategiesPark[0].SpawnEnemies(count2, startPos);
				m_spawnStrategiesPark[0].Reset();
				m_spawnStrategiesPark[1].Reset();
				break;
			}
			}
		}
		return startPos;
	}

	public float GenerateMallRoom(int roomType, float startPos)
	{
		int num = -1;
		int index;
		switch (roomType)
		{
		case -1:
			startPos += m_spawnStrategiesMall[12].SpawnFlat(startPos);
			m_spawnStrategiesMall[12].Reset();
			return startPos;
		case 0:
			index = 0;
			break;
		case 1:
			index = 1;
			num = 2;
			break;
		case 2:
			index = 3;
			num = 4;
			break;
		case 3:
			index = 5;
			num = 6;
			break;
		case 4:
			index = 7;
			num = 8;
			break;
		default:
			index = 9;
			break;
		}
		int num2 = m_spawnStrategiesMall[index].NumLeft;
		if (roomType == 0 || roomType == 4)
		{
			num2 = 4;
		}
		int num3 = (int)SceneRenderer.GetRand(0f, 3f);
		for (int i = 0; i < num3; i++)
		{
			int num4 = (int)SceneRenderer.GetRand(0f, num2);
			if (num4 > 0)
			{
				startPos += m_spawnStrategiesMall[index].SpawnEnemies(num4, startPos);
				num2 -= num4;
			}
			startPos += m_spawnStrategiesMall[10].SpawnEnemies(1, startPos);
		}
		m_spawnStrategiesMall[10].Reset();
		startPos += m_spawnStrategiesMall[index].SpawnEnemies(num2, startPos);
		m_spawnStrategiesMall[index].Reset();
		if (num >= 0)
		{
			startPos += m_spawnStrategiesMall[num].SpawnFlat(startPos);
			m_spawnStrategiesMall[num].Reset();
		}
		return startPos;
	}

	public float GenerateVirtualRoom(float startPos)
	{
		if (startPos == 0f)
		{
			startPos += 200f;
			m_spawnStrategiesVirtual[1].ChangeWeight(163, 1f);
			m_spawnStrategiesVirtual[1].ChangeWeight(164, 0f);
			m_spawnStrategiesVirtual[1].ChangeWeight(165, 0f);
			m_spawnStrategiesVirtual[1].ChangeWeight(166, 0f);
			startPos += m_spawnStrategiesVirtual[1].SpawnEnemies(1, startPos);
			m_spawnStrategiesVirtual[0].ChangeWeight(167, 1f);
			m_spawnStrategiesVirtual[0].ChangeWeight(168, 0f);
			m_spawnStrategiesVirtual[0].ChangeWeight(169, 0f);
			m_spawnStrategiesVirtual[0].ChangeWeight(170, 0f);
			startPos += 200f;
			startPos += m_spawnStrategiesVirtual[0].SpawnEnemies(1, startPos);
			startPos += 200f;
			startPos += m_spawnStrategiesVirtual[0].SpawnEnemies(1, startPos);
		}
		float num = startPos * 1.4f;
		float weight = 100f + num / 3000f;
		float weight2 = 60f + num / 2000f;
		float weight3 = 30f + num / 1500f;
		float weight4 = 0f + num / 1200f;
		m_spawnStrategiesVirtual[0].ChangeWeight(167, weight);
		m_spawnStrategiesVirtual[0].ChangeWeight(168, weight2);
		m_spawnStrategiesVirtual[0].ChangeWeight(169, weight3);
		m_spawnStrategiesVirtual[0].ChangeWeight(170, weight4);
		m_spawnStrategiesVirtual[1].ChangeWeight(163, weight);
		m_spawnStrategiesVirtual[1].ChangeWeight(164, weight2);
		m_spawnStrategiesVirtual[1].ChangeWeight(165, weight3);
		m_spawnStrategiesVirtual[1].ChangeWeight(166, weight4);
		float num2 = startPos + SceneRenderer.GetRand(30f, 500f) + startPos / 100f;
		startPos += SceneRenderer.GetRand(30f, 300f);
		while (startPos < num2)
		{
			startPos += m_spawnStrategiesVirtual[0].SpawnEnemies(1, startPos);
			startPos += SceneRenderer.GetRand(30f, 300f);
			m_spawnStrategiesVirtual[0].Reset();
		}
		startPos += m_spawnStrategiesVirtual[1].SpawnEnemies(1, startPos);
		m_spawnStrategiesVirtual[1].Reset();
		return startPos;
	}

	public float GenerateRandomRoom(float startPos)
	{
		if (m_bStartTracker)
		{
			m_bStartTracker = false;
			m_fTrackStartPos = startPos;
		}
		if (m_iRoomType == 0)
		{
			int num = 0;
			if (m_HospitalRoomTracker.Count > 0)
			{
				num = m_HospitalRoomTracker[(int)SceneRenderer.GetRand(0f, m_HospitalRoomTracker.Count)];
				m_HospitalRoomTracker.Remove(num);
			}
			else
			{
				do
				{
					num = Math.Min(9, (int)SceneRenderer.GetRand(1f, 12f));
				}
				while (num == m_iLastRoom);
			}
			m_iLastRoom = num;
			m_fTrackCurPos = GenerateRoom(num, startPos);
		}
		else if (m_iRoomType == 1)
		{
			m_fTrackCurPos = GenerateParkRoom(startPos);
		}
		else if (m_iRoomType == 2)
		{
			int num2 = 0;
			if (m_iLastRoom == 0)
			{
				do
				{
					num2 = Math.Min(5, (int)SceneRenderer.GetRand(1f, 6f));
				}
				while (num2 == m_iLastRoom);
				m_iLastRoom = num2;
			}
			else
			{
				m_iLastRoom = 0;
			}
			m_fTrackCurPos = GenerateMallRoom(num2, startPos);
		}
		else
		{
			m_fTrackCurPos = GenerateVirtualRoom(startPos);
		}
		return m_fTrackCurPos;
	}

	public float GenerateGlassPanel(float startPos)
	{
		m_spawnStrategiesHospital[20].SpawnEnemies(1, startPos);
		m_spawnStrategiesHospital[20].Reset();
		return startPos;
	}

	public float GenerateGiantTV(float startPos)
	{
		startPos += m_spawnStrategiesMall[11].SpawnEnemies(1, startPos);
		m_spawnStrategiesMall[11].Reset();
		return startPos;
	}

	public void Reset()
	{
		m_iLastRoom = -1;
		m_HospitalRoomTracker = new List<int>();
		for (int i = 1; i < 10; i++)
		{
			m_HospitalRoomTracker.Add(i);
		}
		m_bStartTracker = true;
		m_fTrackCurPos = 0f;
		m_fTrackStartPos = 0f;
		m_bSpawnedTrialBear = !Game1.IsTrial();
	}

	public Color GetRoomColor()
	{
		if (m_iRoomType == 0 || m_iRoomType == 2)
		{
			switch (m_iLastRoom)
			{
			case 0:
				return Color.Crimson;
			case 1:
				return Color.Chocolate;
			case 2:
				return Color.Violet;
			case 3:
				return Color.Yellow;
			case 4:
				return Color.Tomato;
			case 5:
				return Color.Thistle;
			case 6:
				return Color.RoyalBlue;
			case 7:
				return Color.SaddleBrown;
			case 8:
				return Color.SandyBrown;
			case 9:
				return Color.Wheat;
			}
		}
		else
		{
			if (m_iRoomType == 1)
			{
				return Color.SkyBlue;
			}
			if (m_iRoomType == 3)
			{
				return new Color(37, 49, 40, 1);
			}
		}
		return Color.White;
	}

	public void SetRoomType(int i)
	{
		int iRoomType = m_iRoomType;
		m_iRoomType = i;
		if (m_iRoomType != iRoomType)
		{
			m_bStartTracker = true;
		}
	}

	public bool WantsToSwitch()
	{
		if (m_iRoomType == 0)
		{
			return m_HospitalRoomTracker.Count == 0;
		}
		if (m_iRoomType == 1)
		{
			return (m_fTrackCurPos - m_fTrackStartPos) / 75f > 900f;
		}
		if (m_iRoomType == 2)
		{
			return (m_fTrackCurPos - m_fTrackStartPos) / 75f > 1500f;
		}
		return false;
	}
}
