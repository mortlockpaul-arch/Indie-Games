using System.Linq;
using BabyMaker;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics;
using PlayerData;
using Renderer;
using Screens;

namespace Scene;

public class Prop
{
	private Vector2 m_location;

	private PhysicsOutfit m_physicsOutfit;

	private PropType m_type;

	private bool m_bDisabled;

	private static int sm_iPhysicsIndex = 2;

	private RevoluteJoint m_babyAttach;

	private int m_iAttachTime;

	private bool m_bWallActivated;

	public PropType PropType => m_type;

	public Prop(PropType type)
	{
		m_physicsOutfit = new PhysicsOutfit(sm_iPhysicsIndex);
		sm_iPhysicsIndex++;
		InitOutfit(m_physicsOutfit, type);
		m_type = type;
		m_bDisabled = false;
		m_babyAttach = null;
		m_iAttachTime = 0;
		m_bWallActivated = false;
	}

	public Prop(Prop clone)
	{
		m_physicsOutfit = new PhysicsOutfit(sm_iPhysicsIndex);
		sm_iPhysicsIndex++;
		InitOutfit(m_physicsOutfit, clone.m_physicsOutfit);
		m_type = clone.m_type;
		m_bDisabled = false;
		m_babyAttach = null;
		m_iAttachTime = 0;
		m_bWallActivated = false;
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
		if (m_babyAttach != null)
		{
			m_iAttachTime += gameTime.ElapsedMilli;
			if (m_iAttachTime > 3000)
			{
				m_babyAttach.Enabled = false;
				PhysicsObjectManager.GetSimulation().Remove(m_babyAttach);
				m_physicsOutfit.ReEnableCollisions();
			}
		}
		if (m_type == PropType.WALL && !m_bWallActivated && Game1.IsTrial() && m_physicsOutfit.GetSprites(0)[0].Position.X < SceneRenderer.GetCameraPosition().X + 300f)
		{
			m_bWallActivated = true;
			new TrialPrompt();
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		m_physicsOutfit.Draw(gameTime);
	}

	public void ResetToLocation(Vector2 v)
	{
		m_physicsOutfit.ResetToPosition(v);
		m_location = v;
		m_bDisabled = false;
		m_bWallActivated = false;
		m_physicsOutfit.DisablePass(-1);
	}

	private void InitOutfit(PhysicsOutfit outfit, PhysicsOutfit clone)
	{
		outfit.Initialize(clone);
		outfit.SetCollisionHandler(CollisionHandler);
	}

	private void InitOutfit(PhysicsOutfit outfit, PropType type)
	{
		switch (type)
		{
		case PropType.MOTHER:
			PropGenerator.CreateBirthMother(outfit);
			outfit.SetDepth(1000f);
			outfit.SetStatic();
			break;
		case PropType.DOCTOR:
			PropGenerator.CreateDoctor(outfit);
			break;
		case PropType.DOC_STAND:
			PropGenerator.CreateDocStand(outfit);
			break;
		case PropType.OLD_COMP:
			PropGenerator.CreateOldComputer(outfit);
			break;
		case PropType.WAIT_CHAIR:
			PropGenerator.CreateWaitChair(outfit);
			break;
		case PropType.WAIT_CHAIR_PERSON:
			PropGenerator.CreateFilledWaitChair(outfit);
			break;
		case PropType.TABLE_SM_PAMPHLET:
			PropGenerator.CreateSmallTable(outfit);
			break;
		case PropType.RECEPTION_DESK:
			PropGenerator.CreateReceptionDesk(outfit);
			break;
		case PropType.ELDERLY:
			PropGenerator.CreateElderly(outfit);
			break;
		case PropType.CRUTCHES:
			PropGenerator.CreateCrutches(outfit);
			break;
		case PropType.METAL_CABINET:
			PropGenerator.CreateMetalCabinet(outfit);
			break;
		case PropType.GURNEY:
			PropGenerator.CreateGurney(outfit);
			break;
		case PropType.CABINET:
			PropGenerator.CreateCabinet(outfit);
			break;
		case PropType.LUNCHLADY:
			PropGenerator.CreateLunchLady(outfit);
			break;
		case PropType.CRAB_MEAL:
			PropGenerator.CreateCrabMeal(outfit);
			break;
		case PropType.BUFFET:
			PropGenerator.CreateBuffet(outfit);
			break;
		case PropType.GARBAGE:
			PropGenerator.CreateGarbageCan(outfit);
			break;
		case PropType.BED_PATIENT:
			PropGenerator.CreateFilledBed(outfit);
			break;
		case PropType.BEDSIDE_TABLE:
			PropGenerator.CreateBedsideTable(outfit);
			break;
		case PropType.WALL:
			PropGenerator.CreateWall(outfit);
			outfit.SetStatic();
			break;
		case PropType.BEDSIDE_TABLE_FLOWER:
			PropGenerator.CreateBedsideTableWithFlower(outfit);
			break;
		case PropType.LARGE_FOOD_TABLE:
			PropGenerator.CreateLargeFoodTable(outfit);
			break;
		case PropType.SMALL_FOOD_TABLE:
			PropGenerator.CreateSmallFoodTable(outfit);
			break;
		case PropType.BED_EMPTY:
			PropGenerator.CreateEmptyBed(outfit);
			break;
		case PropType.FOOD_TRAY:
			PropGenerator.CreateFoodTray(outfit);
			break;
		case PropType.BYPASS_MACHINE:
			PropGenerator.CreateBypass(outfit);
			break;
		case PropType.XRAY:
			PropGenerator.CreateXRay(outfit);
			break;
		case PropType.SURGERY_LIGHT:
			PropGenerator.CreateSurgeryLight(outfit);
			break;
		case PropType.SURGEON:
			PropGenerator.CreateSurgeryDoctor(outfit);
			break;
		case PropType.SURGERY_PATIENT:
			PropGenerator.CreateSurgeryPatient(outfit);
			break;
		case PropType.ARMLESS_GIRL:
			PropGenerator.CreateArmless(outfit);
			break;
		case PropType.LIMB_TABLE:
			PropGenerator.CreateLimbTable(outfit);
			break;
		case PropType.PHYSIO_BARS:
			PropGenerator.CreatePhysiotherapyBars(outfit);
			break;
		case PropType.WHEELCHAIR:
			PropGenerator.CreateWheelchair(outfit);
			break;
		case PropType.LAB_MICROSCOPE_TABLE:
			PropGenerator.CreateMicroscopeTable(outfit);
			break;
		case PropType.LAB_PILL_TABLE:
			PropGenerator.CreatePillsTable(outfit);
			break;
		case PropType.LAB_GEN_TABLE1:
			PropGenerator.CreateLabTable1(outfit);
			break;
		case PropType.LAB_CHAIR:
			PropGenerator.CreateLabChair(outfit);
			break;
		case PropType.LAB_CABINET:
			PropGenerator.CreateLabCabinet(outfit);
			break;
		case PropType.SURGEON_MONITOR:
			PropGenerator.CreateSurgeryScreens(outfit);
			break;
		case PropType.SINK:
			PropGenerator.CreateSink(outfit);
			break;
		case PropType.COAT_STAND:
			PropGenerator.CreateCoatStand(outfit);
			break;
		case PropType.SURGERY_CABINET:
			PropGenerator.CreateSurgeryCabinet(outfit);
			break;
		case PropType.DEAD_BODY:
			PropGenerator.CreateDeadBody(outfit);
			break;
		case PropType.DEAD_SHELF1:
			PropGenerator.CreateDeadShelves1(outfit);
			break;
		case PropType.DEAD_SHELF2:
			PropGenerator.CreateDeadShelves2(outfit);
			break;
		case PropType.EXAM_BED_EMPTY:
			PropGenerator.CreateExamBedEmpty(outfit);
			break;
		case PropType.EXAM_BED_FULL:
			PropGenerator.CreateExamBedFull(outfit);
			break;
		case PropType.DOC_EXAMINER:
			PropGenerator.CreateDocExaminer(outfit);
			break;
		case PropType.EXAM_DESK:
			PropGenerator.CreateExamDesk(outfit);
			break;
		case PropType.SKELETON:
			PropGenerator.CreateSkeleton(outfit);
			break;
		default:
			PropGenerator.CreateDoctor(outfit);
			break;
		}
		outfit.SetCollisionHandler(CollisionHandler);
	}

	public bool CollisionHandler(Geom geometry1, Geom geometry2, ContactList contactList)
	{
		if ((m_type != PropType.WALL || !Game1.IsTrial()) && geometry2.CollisionCategories == PhysicsObjectManager.PlayerCollisionGroup())
		{
			if (!m_bDisabled)
			{
				Player player = PhysicsObjectManager.GetPlayer(geometry2);
				player.SaveFrameData(m_type);
				m_bDisabled = true;
				m_physicsOutfit.DisablePass(1);
				for (int i = 0; i < 30; i++)
				{
					Color color = new Color(SceneRenderer.GetRand(0f, 1f), SceneRenderer.GetRand(0f, 1f), SceneRenderer.GetRand(0f, 1f));
					ParticleManager.GetParticle().Initialize(GenericParticleImgs.GetParticle(), contactList.First().Position, SceneRenderer.GetRand(-2f, -1f), (int)SceneRenderer.GetRand(1300f, 3000f), new Vector2(SceneRenderer.GetRand(-500f, 500f), SceneRenderer.GetRand(-500f, 0f)), fadesOut: true, color, color, 20f, 200f, additive: false, new Vector2(0f, 500f), SceneRenderer.GetRand(0f, 3.14f), 0f, default(Vector2));
				}
				if (m_type == PropType.CRAB_MEAL)
				{
					m_babyAttach = JointFactory.Instance.CreateRevoluteJoint(PhysicsObjectManager.GetSimulation(), geometry1.Body, geometry2.Body, contactList[0].Position);
					m_iAttachTime = 0;
					m_physicsOutfit.LightenGroup(geometry1.CollisionGroup);
				}
				PropSoundPlayer.ActivateSound();
				return true;
			}
			return false;
		}
		return true;
	}

	public void UpdateEnabled()
	{
		m_physicsOutfit.UpdateEnabled();
		if (m_type == PropType.WALL && !Game1.IsTrial())
		{
			m_physicsOutfit.RemoveStatic();
		}
	}
}
