using System;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using PropModel;

namespace EGEngine;

public class VehicleCls : PropModelBase
{
	public const float MaxUpdateDis = 20000f;

	private static Model BrakeDrum;

	private static Matrix[] BrakeDrumTrans;

	public static bool VehicleMenuOpen = false;

	private bool[] Render = new bool[2];

	public float NetworkMessageRecievedTimer;

	public float AttachRequestPending;

	public bool DisplayCanAttachText;

	public PlayerBase[] AttachedPlayer = new PlayerBase[4];

	public VehicleType eType = VehicleType.YellowTruck;

	public ItemCls VehicleItemRef;

	public float maxSpeed = 120f;

	public bool IsOffRoadVehicle;

	public bool EnableSpeedDecay = true;

	public bool InCanAttachDistance;

	public bool isSpawned;

	private float cameraAngle;

	private float tireaAngle;

	private float tireSteer;

	private float speed = 4f;

	private float inReverse = 1f;

	public bool HeadLightOn;

	private Vector3 vposition = Vector3.Zero;

	private Vector3 vdirection = Vector3.UnitZ;

	private Vector3 vright = Vector3.UnitX;

	private Vector3 vup = Vector3.UnitY;

	private int LeftFrontWheelIndex;

	private int RightFrontWheelIndex;

	private int LeftRearWheelIndex;

	private int RightRearWheelIndex;

	private int HeadLightRightIndex;

	private int HeadLightLeftIndex;

	private int matLFWheelIndex;

	private int matRFWheelIndex;

	private int matLRWheelIndex;

	private int matRRWheelIndex;

	private int matHeadLightRightIndex;

	private int matHeadLightLeftIndex;

	private int MainBodyIndex;

	public int RFrontWheelQuality = 100;

	public int LFrontWheelQuality = 100;

	public int RRearWheelQuality = 100;

	public int LRearWheelQuality = 100;

	public float FuelLevel = 100f;

	public int VehicleDamage = 100;

	private float BrakePreasure;

	private float CamPosHeightPitch = 100f;

	private Vector3 tmpPos = Vector3.Zero;

	private Vector3 tmpDir = Vector3.Zero;

	private Vector3 tmpNorm = Vector3.Zero;

	private Vector3 tmpChasisDir = Vector3.Zero;

	private Vector3 tmpChasisRight = Vector3.Zero;

	private Vector3 frontAxlePos = Vector3.Zero;

	private Vector3 rearAxlePos = Vector3.Zero;

	private Vector3[] tirePos = new Vector3[4];

	private Vector3[] tireCurrentPos = new Vector3[4];

	private float[] tireGravity = new float[4];

	private Matrix vMatTrans = Matrix.Identity;

	private Matrix[] matChasis = new Matrix[2];

	private float networkSystem;

	public bool EngineStart;

	public bool EngineRunning;

	public Cue EngineSound;

	public Cue StartSound;

	public Cue DoorSound;

	public Cue CrashSound;

	public string EngineSoundName = "PoliceCharger";

	private float pitchForce;

	private float leanForce;

	private float yDiff;

	private Vector3 nDir = Vector3.Zero;

	private Vector3 nUp = Vector3.Zero;

	private Vector3 chasisY = Vector3.Zero;

	private Vector3 LastPos = Vector3.Zero;

	private Matrix mRight = Matrix.Identity;

	private static BoundingSphere frustumSphere = default(BoundingSphere);

	private static Matrix matVehicleCollision = Matrix.Identity;

	private static Matrix tmpMatCollision = Matrix.Identity;

	private static Matrix invTransformCollision = Matrix.Identity;

	private static Matrix invTransformNoTransCollision = Matrix.Identity;

	private static BoundingSphere invSphereCollision = default(BoundingSphere);

	private static Vector3 tmpRadiusCollision = Vector3.Zero;

	private static Vector3 debugSpotParams = new Vector3(0.6f, 0.96f, 0.975f);

	private static float debugHLHeight = 180f;

	private static float debugHLDirection = 160f;

	private static float debugHLRight = 60f;

	private static float debugHLRadius = 7000f;

	private static int particleIndexSpawn = 0;

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	private static float DispInstructionsTimer = 12f;

	private BoundingSphere collisionSphere = default(BoundingSphere);

	private Vector3 sphereColVector = Vector3.Zero;

	private static Matrix matBrakeDrumDraw = Matrix.Identity;

	private static Matrix matWheelDraw = Matrix.Identity;

	private static Matrix matFrontWheelDraw;

	private static float MaterialReflectScalar = 0.25f;

	private static string msgExit = "Exit";

	private static string msgLights = "Lights";

	private static string msgSteer = "Steer";

	private static string msgForward = "Forward";

	private static string msgBrake = "Brake/Reverse";

	private static string msgCamera = "Camera";

	private static string msgEnter = "Enter";

	private static Color FuelShadColor = Color.White;

	private static Color FuelDispColor = Color.White;

	private static float FuelBlinktimer = 0f;

	private static Vector2 uiPos = Vector2.Zero;

	private static Rectangle uiRec = default(Rectangle);

	public float Speed
	{
		get
		{
			return speed;
		}
		set
		{
			speed = value;
		}
	}

	public float TireSteer
	{
		get
		{
			return tireSteer;
		}
		set
		{
			tireSteer = value;
		}
	}

	public Vector3 Position
	{
		get
		{
			return vposition;
		}
		set
		{
			vposition = value;
		}
	}

	public Vector3 Direction
	{
		get
		{
			return vdirection;
		}
		set
		{
			vdirection = value;
		}
	}

	public float Reverse
	{
		get
		{
			return inReverse;
		}
		set
		{
			inReverse = value;
		}
	}

	public VehicleCls(VehicleType e)
	{
		eType = e;
	}

	public VehicleCls(ItemCls e)
	{
		SetByItemType(e);
	}

	public void SetByItemType(ItemCls e)
	{
		VehicleItemRef = e;
		eType = (VehicleType)e.ItemType;
		Position = e.pos;
	}

	public override void Load(string n)
	{
		EngineStart = false;
		EngineSound = EndGameEngine.SoundBnk.GetCue(EngineSoundName);
		StartSound = EndGameEngine.SoundBnk.GetCue("VehicleStart");
		DoorSound = EndGameEngine.SoundBnk.GetCue("VehicleDoorOpenClose");
		CrashSound = EndGameEngine.SoundBnk.GetCue("ApocZCrash00");
		base.Load(n, 1.085f);
		Matrix identity = Matrix.Identity;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			if (propModel.Meshes[i].Name == "wheelRF")
			{
				RightFrontWheelIndex = i;
				matRFWheelIndex = propModel.Meshes[i].ParentBone.Index;
				ref Vector3 reference = ref tirePos[1];
				reference = (propTransforms[matRFWheelIndex] * identity).Translation;
			}
			if (propModel.Meshes[i].Name == "wheelLF")
			{
				LeftFrontWheelIndex = i;
				matLFWheelIndex = propModel.Meshes[i].ParentBone.Index;
				ref Vector3 reference2 = ref tirePos[0];
				reference2 = (propTransforms[matLFWheelIndex] * identity).Translation;
			}
			if (propModel.Meshes[i].Name == "wheelRR")
			{
				RightRearWheelIndex = i;
				matRRWheelIndex = propModel.Meshes[i].ParentBone.Index;
				ref Vector3 reference3 = ref tirePos[3];
				reference3 = (propTransforms[matRRWheelIndex] * identity).Translation;
			}
			if (propModel.Meshes[i].Name == "wheelLR")
			{
				LeftRearWheelIndex = i;
				matLRWheelIndex = propModel.Meshes[i].ParentBone.Index;
				ref Vector3 reference4 = ref tirePos[2];
				reference4 = (propTransforms[matLRWheelIndex] * identity).Translation;
			}
			if (propModel.Meshes[i].Name == "HeadLightRight")
			{
				HeadLightRightIndex = i;
				matHeadLightRightIndex = propModel.Meshes[i].ParentBone.Index;
			}
			if (propModel.Meshes[i].Name == "HeadLightLeft")
			{
				HeadLightLeftIndex = i;
				matHeadLightLeftIndex = propModel.Meshes[i].ParentBone.Index;
			}
			if (propModel.Meshes[i].Name == "Body")
			{
				MainBodyIndex = i;
			}
		}
		BrakeDrum = EndGameEngine.GameAssetMgr.Load<Model>("models\\vehicles\\BrakeDrum");
		BrakeDrumTrans = new Matrix[BrakeDrum.Bones.Count];
		BrakeDrum.CopyAbsoluteBoneTransformsTo(BrakeDrumTrans);
		for (int j = 0; j < BrakeDrum.Meshes.Count; j++)
		{
			ModelMesh modelMesh = BrakeDrum.Meshes[j];
			modelMesh.Tag = new MeshAttributesParams();
			for (int k = 0; k < modelMesh.MeshParts.Count; k++)
			{
				modelMesh.MeshParts[k].Tag = new PropEffectParams(modelMesh.MeshParts[k].Effect);
			}
		}
		Set();
	}

	public void Set()
	{
		isSpawned = true;
		vMatTrans = Matrix.Identity;
		vMatTrans.Forward = vdirection;
		vMatTrans.Right = Vector3.Cross(vdirection, Vector3.UnitY);
		vMatTrans.Up = Vector3.UnitY;
		vMatTrans.Translation = Vector3.Zero;
		for (int i = 0; i < 4; i++)
		{
			ref Vector3 reference = ref tireCurrentPos[i];
			reference = Vector3.Transform(tirePos[i], vMatTrans);
			tireCurrentPos[i] += vposition;
			tireCurrentPos[i].Y = HeightMapPhysics.GetHeight(ref tireCurrentPos[i]) + 4f;
			tireGravity[i] = 0f;
		}
		vright = tireCurrentPos[3] - tireCurrentPos[2] + (tireCurrentPos[1] - tireCurrentPos[0]);
		vright.Normalize();
		frontAxlePos = tireCurrentPos[0] + (tireCurrentPos[1] - tireCurrentPos[0]) * 0.5f;
		rearAxlePos = tireCurrentPos[2] + (tireCurrentPos[3] - tireCurrentPos[2]) * 0.5f;
		vdirection = frontAxlePos - rearAxlePos;
		vposition = rearAxlePos + vdirection * 0.5f;
		vdirection.Normalize();
		vup = Vector3.Cross(vright, vdirection);
		vup.Normalize();
		vright = Vector3.Cross(vdirection, vup);
		vright.Normalize();
		vMatTrans.Forward = vdirection;
		vMatTrans.Right = vright;
		vMatTrans.Up = vup;
		TransformBoundingSphere(Matrix.Identity);
		networkSystem = (float)EndGameEngine.randGenerator.NextDouble() * 4f;
		ref Matrix reference2 = ref matWorld[0];
		reference2 = vMatTrans;
		ref Matrix reference3 = ref matWorld[1];
		reference3 = vMatTrans;
		ref Matrix reference4 = ref matChasis[0];
		reference4 = vMatTrans;
		ref Matrix reference5 = ref matChasis[1];
		reference5 = vMatTrans;
		HeadLightOn = false;
		LRearWheelQuality = 100;
		LFrontWheelQuality = 100;
		RRearWheelQuality = 100;
		RFrontWheelQuality = 100;
		switch (EndGameEngine.randGenerator.Next(0, 8))
		{
		case 0:
			RFrontWheelQuality = 0;
			break;
		case 1:
			LFrontWheelQuality = 0;
			break;
		case 2:
			RRearWheelQuality = 0;
			break;
		case 3:
			RRearWheelQuality = 0;
			LFrontWheelQuality = 0;
			break;
		case 4:
			RRearWheelQuality = 0;
			RFrontWheelQuality = 0;
			break;
		case 5:
			RFrontWheelQuality = 0;
			RRearWheelQuality = 0;
			LRearWheelQuality = 0;
			break;
		default:
			LRearWheelQuality = 0;
			break;
		}
		UpdateTransform(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 1f, 0, earlyOut: false);
		UpdateTransform(LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value], 1f, 1, earlyOut: false);
	}

	public void GetOutOfTruck(int qIndex, PlayerBase playerRef)
	{
		Vector3 vector = Vector3.Cross(vdirection, Vector3.UnitY);
		playerRef.vecPosition = vposition - vector * 200f;
		Vector3 origin = vposition;
		Vector3 direction = vector * -1f;
		Vector3 hitPos = Vector3.Zero;
		Vector3 hitNorm = Vector3.UnitX;
		if (AIBase.RayCastGeometry(0, ref origin, ref direction, ref hitPos, ref hitNorm) && (origin - hitPos).LengthSquared() < 40000f)
		{
			playerRef.vecPosition = hitPos + vector * 40f;
		}
		playerRef.tmpPrevPosition = playerRef.vecPosition;
		ref Vector3 reference = ref playerRef.vecHeadPosition[qIndex];
		reference = playerRef.vecPosition;
		playerRef.vecDirection = vdirection;
		playerRef.Angles.X = MathHelper.ToDegrees(MyMath.AngleBetweenVectors(Vector3.UnitZ, vdirection) * -1f);
		playerRef.Angles.Y = 0f;
		playerRef.OverrideCamera = false;
		playerRef.OverridePosition = false;
		playerRef.OverrideButtonTriggerRight = false;
		playerRef.OverrideProjection = false;
	}

	public void CollisionBoundingSphere(ref BoundingSphere bs)
	{
		tmpDir = vposition - bs.Center;
		float num = bs.Radius * bs.Radius;
		if (tmpDir.LengthSquared() < num)
		{
			vposition += tmpDir * (1f - tmpDir.LengthSquared() / num);
			tmpDir.Normalize();
			speed -= Math.Abs(Vector3.Dot(vdirection, tmpDir));
		}
	}

	public void CollisionWithOther(VehicleCls other)
	{
		tmpDir = vposition - (other.Position + other.propBoundingSphere[other.MainBodyIndex].Center);
		float num = other.propBoundingSphere[other.MainBodyIndex].Radius * other.propBoundingSphere[other.MainBodyIndex].Radius;
		if (tmpDir.LengthSquared() < num)
		{
			tmpDir *= 1f - tmpDir.LengthSquared() / num;
			vposition += tmpDir * 0.8f;
			other.vposition -= tmpDir * 0.2f;
			tmpDir.Normalize();
			speed -= Math.Abs(Vector3.Dot(vdirection, tmpDir));
			if (IsOccupied())
			{
				PlayCrashSound(speed);
			}
		}
	}

	public void Destroy()
	{
		for (int i = 0; i < 4; i++)
		{
			AttachedPlayer[i] = null;
		}
		isSpawned = false;
		EngineStart = false;
		if (EngineSound != null)
		{
			EngineSound.Dispose();
		}
	}

	public int IsSeatAvailable()
	{
		for (int i = 0; i < 4; i++)
		{
			if (AttachedPlayer[i] == null)
			{
				return i;
			}
		}
		return -1;
	}

	public bool IsSeatAvailable(int vs)
	{
		for (int i = 0; i < 4; i++)
		{
			if (AttachedPlayer[vs] == null)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsOccupied()
	{
		for (int i = 0; i < 4; i++)
		{
			if (AttachedPlayer[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsRemoteDriver(PlayerBase playerRef)
	{
		if (AttachedPlayer[0] != null && AttachedPlayer[0] != playerRef)
		{
			return true;
		}
		return false;
	}

	public void SetAttachmentFlags()
	{
		for (int i = 0; i < 4; i++)
		{
			if (AttachedPlayer[i] != null)
			{
				AttachedPlayer[i].IsAttached0 = true;
			}
		}
	}

	public int PlayerIsAttached(PlayerBase playerRef)
	{
		for (int i = 0; i < 4; i++)
		{
			if (AttachedPlayer[i] == playerRef)
			{
				return i;
			}
		}
		return -1;
	}

	public void DrawSeatsDebug(PlayerBase playerRef)
	{
		if (!VehicleMenuOpen && !playerRef.IsAttached0)
		{
			return;
		}
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		Vector2 zero = Vector2.Zero;
		zero.X = (uiPos.X = viewport.TitleSafeArea.Left);
		zero.Y = 320f;
		Menu.spriteBatch.Begin();
		for (int i = 0; i < 4; i++)
		{
			if (AttachedPlayer[i] != null)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, i + "-" + AttachedPlayer[i].gamerTag, zero, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, i + "-" + AttachedPlayer[i].gamerTag, zero, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			}
			else
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, i + "-Empty", zero, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, i + "-Empty", zero, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			}
			zero.Y += 24f;
		}
		Menu.spriteBatch.End();
	}

	public void Update(float eTime, int qIndex, PlayerBase playerRef, bool canAttach, int vehicleIndex)
	{
		if (!isSpawned)
		{
			return;
		}
		LRearWheelQuality = 100;
		LFrontWheelQuality = 100;
		RRearWheelQuality = 100;
		RFrontWheelQuality = 100;
		AttachRequestPending -= 0.0334f;
		NetworkMessageRecievedTimer += 0.0334f;
		bool inventoryOpen = InventoryCls.InventoryOpen;
		float num = Speed;
		int num2 = ((qIndex == 0) ? 1 : 0);
		LastPos = matWorld[num2].Translation;
		if (!IsOccupied() && EngineSound != null && !EngineSound.IsDisposed && EngineSound.IsPlaying)
		{
			EngineSound.Dispose();
		}
		tmpDir = playerRef.vecPosition - (vposition + propBoundingSphere[MainBodyIndex].Center);
		float num3 = tmpDir.LengthSquared();
		float num4 = propBoundingSphere[MainBodyIndex].Radius * propBoundingSphere[MainBodyIndex].Radius;
		num4 += 6400f;
		InCanAttachDistance = false;
		if (num3 < num4 * 1.2f)
		{
			InCanAttachDistance = true;
		}
		if (!playerRef.IsAttached0 && num3 < num4)
		{
			CollisionData collisionData = ((MeshUserData)propModel.Tag).collisionData;
			matVehicleCollision = vMatTrans;
			matVehicleCollision.Translation = vposition;
			tmpMatCollision = collisionData.transform * matVehicleCollision;
			invTransformCollision = Matrix.Invert(tmpMatCollision);
			invTransformNoTransCollision = invTransformCollision;
			invTransformNoTransCollision.Translation = Vector3.Zero;
			invSphereCollision.Center = playerRef.vecPosition;
			invSphereCollision.Center.Y -= 18f;
			invSphereCollision.Radius = 28f;
			invSphereCollision.Center = Vector3.Transform(invSphereCollision.Center, invTransformCollision);
			tmpRadiusCollision = Vector3.Transform(Vector3.UnitX * invSphereCollision.Radius, invTransformNoTransCollision);
			invSphereCollision.Radius = tmpRadiusCollision.Length();
			PropModelBase.ObjectSpaceUpVector = Vector3.Transform(Vector3.UnitY, invTransformNoTransCollision);
			PropModelBase.ObjectSpaceUpVector.Normalize();
			bool onWalkable = false;
			playerRef.OverrideLevelOutsideCollision = true;
			PropModelBase.RayCastDist = 42f;
			PropModelBase.TestRayCast = true;
			if (SphereCollision(ref invSphereCollision, qIndex, ref onWalkable) || onWalkable)
			{
				invSphereCollision.Center = Vector3.Transform(invSphereCollision.Center, tmpMatCollision);
				invSphereCollision.Center.Y += 16f;
				playerRef.vecPosition = Vector3.Lerp(playerRef.vecPosition, invSphereCollision.Center, 0.85f);
				if (onWalkable)
				{
					playerRef.GravityAccel = 0f;
					playerRef.onWalkable = onWalkable;
				}
				if (Speed > 20f && playerRef.BloodLevel > 0f)
				{
					tmpNorm = Vector3.UnitY;
					AIBase.VehicleKillZombie(ref playerRef.vecPosition, ref tmpNorm);
					float num5 = playerRef.BloodLevel - 100f;
					playerRef.BloodLevel = ((num5 < 0f) ? 0f : num5);
					byte value = ((playerRef.BloodLoss > 0f) ? ((byte)1) : ((byte)0));
					if (EGENetWorkNext.networkSession != null)
					{
						PacketWriter packetWriter = EGENetWorkNext.packetWriter;
						packetWriter.Write((byte)130);
						packetWriter.Write((byte)7);
						packetWriter.Write(playerRef.NetGamerRef.Id);
						packetWriter.Write(100);
						packetWriter.Write(value);
						EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
					}
				}
			}
			PropModelBase.TestRayCast = false;
		}
		if (HeadLightOn)
		{
			Color color = Color.White;
			Vector3 spotDirection = vdirection * -100f;
			spotDirection.Y += 20f;
			spotDirection.Normalize();
			Vector3 position = vposition;
			Vector3 spotParams = debugSpotParams;
			position.Y += debugHLHeight;
			position += vdirection * debugHLDirection;
			LevelBaseMenu.PointLights.AddDynamicSpotLight(ref position, ref spotDirection, ref spotParams, ref color, debugHLRadius, 2000f, qIndex);
		}
		uint alphaMap = HeightMapPhysics.GetAlphaMap(ref vposition);
		bool flag = (alphaMap & 0xFF0000) != 0;
		if (speed > 5f)
		{
			if (((alphaMap >> 8) & 0xFF) > 64)
			{
				particles.SpawnVehicleDust(ref vposition, (!(speed > 10f)) ? 1 : 4);
			}
			else
			{
				particleIndexSpawn++;
				if (particleIndexSpawn > 1)
				{
					particleIndexSpawn = 0;
					particles.SpawnVehicleDust(ref vposition, 1);
				}
			}
		}
		RemotePlayerContact(qIndex);
		if (canAttach)
		{
			int num6 = IsSeatAvailable();
			if (num6 >= 0 && PlayerIsAttached(playerRef) < 0)
			{
				if (AIBase.BlackFadeTimer < 0f && !inventoryOpen && num3 < num4 * 1.2f)
				{
					if (VehicleMenuOpen)
					{
						playerRef.vehicleSeat = num6;
						if (playerRef.currentGamePadState.IsButtonDown(Buttons.DPadUp) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadUp))
						{
							VehicleMenuOpen = false;
						}
						else if (playerRef.currentGamePadState.IsButtonDown(Buttons.X) && playerRef.lastGamePadState.IsButtonUp(Buttons.X))
						{
							if (EGENetWorkNext.networkSession != null)
							{
								if (EGENetWorkNext.networkSession.IsHost)
								{
									playerRef.IsAttached0 = true;
									playerRef.vehicleSeat = num6;
									AttachedPlayer[playerRef.vehicleSeat] = playerRef;
									PlayDoorSound();
									PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
									packetWriter2.Write((byte)114);
									packetWriter2.Write(EGENetWorkNext.networkSession.Host.Id);
									packetWriter2.Write(VehicleItemRef.uid);
									packetWriter2.Write((byte)playerRef.vehicleSeat);
									EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.ReliableInOrder);
									PlayerAttachHardSetCamera(playerRef);
								}
								else if (AttachRequestPending <= 0f)
								{
									playerRef.vehicleSeat = num6;
									PacketWriter packetWriter3 = EGENetWorkNext.packetWriter;
									packetWriter3.Write((byte)113);
									packetWriter3.Write(VehicleItemRef.uid);
									packetWriter3.Write((byte)playerRef.vehicleSeat);
									EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter3, SendDataOptions.ReliableInOrder);
									AttachRequestPending = 5f;
								}
							}
							else
							{
								playerRef.IsAttached0 = true;
								playerRef.vehicleSeat = num6;
								AttachedPlayer[playerRef.vehicleSeat] = playerRef;
								PlayDoorSound();
							}
						}
					}
					else if (playerRef.currentGamePadState.IsButtonDown(Buttons.DPadUp) && playerRef.lastGamePadState.IsButtonUp(Buttons.DPadUp))
					{
						VehicleMenuOpen = true;
					}
					DisplayCanAttachText = true;
					if (AIBase.BlackFadeTimer < 0f && VehicleMenuOpen)
					{
						if ((RFrontWheelQuality == 0 || LFrontWheelQuality == 0 || RRearWheelQuality == 0 || LRearWheelQuality == 0) && playerRef.currentGamePadState.IsButtonDown(Buttons.Y) && playerRef.lastGamePadState.IsButtonUp(Buttons.Y))
						{
							bool flag2 = AIBase.PlayerInventory.HaveItem(1024, 10);
							bool flag3 = AIBase.PlayerInventory.HaveItem(1024, 8);
							if (flag2 && flag3)
							{
								if (RFrontWheelQuality == 0)
								{
									RFrontWheelQuality = 100;
								}
								else if (LFrontWheelQuality == 0)
								{
									LFrontWheelQuality = 100;
								}
								else if (RRearWheelQuality == 0)
								{
									RRearWheelQuality = 100;
								}
								else if (LRearWheelQuality == 0)
								{
									LRearWheelQuality = 100;
								}
								NetworkSendVehicleUpdate(playerRef, vehicleIndex);
								Speed = 1f;
								AIBase.PlayerInventory.DestroyItem(1024, 10);
							}
							else if (!flag3 && !flag2)
							{
								GenericMessages.Add("You Need A Tool Box And A Tire", 8);
							}
							else if (!flag3)
							{
								GenericMessages.Add("You Need A Tool Box", 8);
							}
							else if (!flag2)
							{
								GenericMessages.Add("You Need A Tire", 8);
							}
						}
						if (FuelLevel < 100f && playerRef.currentGamePadState.IsButtonDown(Buttons.B) && playerRef.lastGamePadState.IsButtonUp(Buttons.B))
						{
							if (AIBase.PlayerInventory.HaveItem(1024, 3))
							{
								FuelLevel = 100f;
								NetworkSendVehicleUpdate(playerRef, vehicleIndex);
								AIBase.PlayerInventory.EmptyItem(1024, 3);
							}
							else
							{
								GenericMessages.Add("You Need Fuel Can", 8);
							}
						}
					}
				}
				else
				{
					DisplayCanAttachText = false;
				}
			}
			else if (AIBase.BlackFadeTimer < 0f && !inventoryOpen)
			{
				if (playerRef.currentGamePadState.IsButtonDown(Buttons.X) && playerRef.lastGamePadState.IsButtonUp(Buttons.X))
				{
					playerRef.IsAttached0 = false;
					DetachPlayer(playerRef, qIndex);
					playerRef.currentGamePadState = playerRef.lastGamePadState;
				}
				else
				{
					DisplayCanAttachText = false;
				}
			}
		}
		if (IsRemoteDriver(playerRef))
		{
			tireaAngle -= speed * 0.025f * inReverse;
			vposition += vdirection * speed * inReverse;
			vright = Vector3.Cross(vdirection, Vector3.UnitY);
			vright.Normalize();
			vup = Vector3.Cross(vright, vdirection);
			vup.Normalize();
			vright = Vector3.Cross(vdirection, vup);
			vright.Normalize();
			vMatTrans.Forward = vdirection;
			vMatTrans.Right = vright;
			vMatTrans.Up = vup;
			for (int i = 0; i < 4; i++)
			{
				float y = tireCurrentPos[i].Y;
				ref Vector3 reference = ref tireCurrentPos[i];
				reference = Vector3.Transform(tirePos[i], vMatTrans);
				tireCurrentPos[i].Y = y + vdirection.Y * speed * inReverse;
				tireCurrentPos[i].X += vposition.X;
				tireCurrentPos[i].Z += vposition.Z;
				float num7 = HeightMapPhysics.GetHeight(ref tireCurrentPos[i]) + 4f;
				float? num8 = AIBase.WalkableHeight(ref tireCurrentPos[i], qIndex);
				if (num8.HasValue && num8 > num7)
				{
					num7 = num8.Value;
				}
				if (i == 1 && RFrontWheelQuality == 0)
				{
					num7 -= 16f;
				}
				else if (i == 0 && LFrontWheelQuality == 0)
				{
					num7 -= 16f;
				}
				else if (i == 3 && RRearWheelQuality == 0)
				{
					num7 -= 16f;
				}
				else if (i == 2 && LRearWheelQuality == 0)
				{
					num7 -= 16f;
				}
				if (num7 > tireCurrentPos[i].Y)
				{
					tireCurrentPos[i].Y = num7;
					tireGravity[i] = 0f;
				}
				else if (num7 < tireCurrentPos[i].Y - 2f)
				{
					tireGravity[i] += 1.5f;
					tireGravity[i] = ((tireGravity[i] > 64f) ? 64f : tireGravity[i]);
					tireCurrentPos[i].Y -= tireGravity[i];
				}
			}
			vright = tireCurrentPos[3] - tireCurrentPos[2] + (tireCurrentPos[1] - tireCurrentPos[0]);
			vright.Normalize();
			frontAxlePos = tireCurrentPos[0] + (tireCurrentPos[1] - tireCurrentPos[0]) * 0.5f;
			rearAxlePos = tireCurrentPos[2] + (tireCurrentPos[3] - tireCurrentPos[2]) * 0.5f;
			Vector3 vector = frontAxlePos - rearAxlePos;
			tmpPos = rearAxlePos + vector * 0.5f;
			vposition.Y = MathHelper.Lerp(vposition.Y, tmpPos.Y, 0.25f);
			if (NetworkMessageRecievedTimer > 4f)
			{
				speed *= 0.98f;
				vdirection = Vector3.Lerp(vector * 100f, vdirection * 100f, 1f);
				vdirection.Normalize();
				UpdateCollision(playerRef, qIndex, vehicleIndex, transmitData: false);
			}
			vup = Vector3.Cross(vright, vdirection);
			vup.Normalize();
			vright = Vector3.Cross(vdirection, vup);
			vright.Normalize();
			vMatTrans.Forward = vdirection;
			vMatTrans.Right = vright;
			vMatTrans.Up = vup;
			ref Matrix reference2 = ref matWorld[qIndex];
			reference2 = vMatTrans;
			UpdatePlayerAttachedSound(offRoad: false);
			if ((vposition - playerRef.vecPosition).LengthSquared() > 400000000f)
			{
				Render[qIndex] = false;
				return;
			}
			tmpPos = vposition;
			tmpPos.X -= playerRef.vecHeadPosition[qIndex].X;
			tmpPos.Z -= playerRef.vecHeadPosition[qIndex].Z;
			matWorld[qIndex].Translation = tmpPos;
			pitchForce = (Speed - num) * 3f * inReverse;
			pitchForce = ((pitchForce > 2f) ? 2f : pitchForce);
			pitchForce = ((pitchForce < -2f) ? (-2f) : pitchForce);
			nDir = Vector3.Transform(vdirection, Matrix.CreateFromAxisAngle(vright, MathHelper.ToRadians(pitchForce)));
			tmpChasisDir.X = vdirection.X * 100f;
			tmpChasisDir.Z = vdirection.Z * 100f;
			tmpChasisDir.Y = MathHelper.SmoothStep(tmpChasisDir.Y * 100f, nDir.Y * 100f, 0.25f);
			tmpChasisDir.Normalize();
			leanForce = speed / maxSpeed;
			leanForce = ((leanForce > 0.05f) ? 0.05f : leanForce);
			mRight = Matrix.CreateFromAxisAngle(tmpChasisDir, tireSteer * (1f - speed / maxSpeed) * (0f - leanForce));
			tmpChasisRight = Vector3.SmoothStep(tmpChasisRight, Vector3.Transform(vright, mRight), 0.5f);
			tmpChasisRight.Normalize();
			nUp = Vector3.Cross(tmpChasisRight, tmpChasisDir);
			nUp.Normalize();
			yDiff = MathHelper.SmoothStep(matChasis[qIndex].Translation.Y, tmpPos.Y, 0.25f) - tmpPos.Y;
			yDiff = ((yDiff > 6f) ? 6f : yDiff);
			yDiff = ((yDiff < -6f) ? (-6f) : yDiff);
			chasisY = tmpPos;
			chasisY.Y += yDiff;
			ref Matrix reference3 = ref matChasis[qIndex];
			reference3 = matWorld[qIndex];
			matChasis[qIndex].Forward = tmpChasisDir;
			matChasis[qIndex].Up = nUp;
			matChasis[qIndex].Right = tmpChasisRight;
			matChasis[qIndex].Translation = chasisY;
			TestFrustum(playerRef, qIndex);
			if (PlayerIsAttached(playerRef) >= 0)
			{
				float num9 = playerRef.currentGamePadState.ThumbSticks.Right.Y * 12f * playerRef.InvertY;
				CamPosHeightPitch -= num9;
				CamPosHeightPitch *= 0.99f;
				if (CamPosHeightPitch > 240f)
				{
					CamPosHeightPitch = 240f;
				}
				if (CamPosHeightPitch < -200f)
				{
					CamPosHeightPitch = -200f;
				}
				tmpPos = vposition;
				cameraAngle += playerRef.currentGamePadState.ThumbSticks.Right.X * 0.2f;
				cameraAngle *= 0.9f;
				Vector3 vector2 = tmpPos;
				vector2.Y += CamPosHeightPitch + 280f + speed * 0.3f;
				Matrix matrix = Matrix.CreateRotationY(cameraAngle);
				Vector3 vector3 = Vector3.Transform(vdirection, matrix);
				vector2 += vector3 * (0f - (640f + speed * 0.2f));
				tmpPos.Y += 180f;
				vector3 = tmpPos - vector2;
				vector3.Normalize();
				AIBase.camOverridePos = Vector3.Lerp(AIBase.camOverridePos, vector2, 0.2f);
				AIBase.camOverrideDir = Vector3.Lerp(AIBase.camOverrideDir, vector3, 0.2f);
				playerRef.OverridePos = AIBase.camOverridePos;
				playerRef.OverrideDir = AIBase.camOverrideDir;
				playerRef.OverrideUp = Vector3.Cross(AIBase.camOverrideDir, vright);
				playerRef.OverrideRight = vright;
				playerRef.vecPosition = AIBase.camOverridePos;
				playerRef.vecDirection = AIBase.camOverrideDir;
				playerRef.CameraDirection = AIBase.camOverrideDir;
			}
			return;
		}
		if (AttachedPlayer[playerRef.vehicleSeat] == playerRef && playerRef.vehicleSeat == 0)
		{
			EnableSpeedDecay = true;
			FuelLevel -= 0.001f;
			UpdatePlayerAttachedSound(flag);
			if (EngineSound.IsPlaying)
			{
				EngineRunning = true;
			}
			else
			{
				EngineRunning = false;
			}
			if (flag)
			{
				if (IsOffRoadVehicle)
				{
					if (speed > 60f)
					{
						speed -= 1.4f;
					}
				}
				else if (speed > 28f)
				{
					speed -= 1.75f;
				}
			}
			if (VehicleDamage > 0)
			{
				bool flag4 = false;
				if (AIBase.BlackFadeTimer < 0f && !inventoryOpen)
				{
					if (playerRef.currentGamePadState.IsButtonDown(Buttons.DPadRight) && !playerRef.lastGamePadState.IsButtonDown(Buttons.DPadRight))
					{
						HeadLightOn = !HeadLightOn;
					}
					if (playerRef.currentGamePadState.IsButtonDown(Buttons.RightTrigger))
					{
						if (inReverse < 0f)
						{
							if (BrakePreasure < 7f)
							{
								BrakePreasure += 0.1f;
							}
							speed -= BrakePreasure;
							if (speed < 0f)
							{
								speed = 0f;
								inReverse = 1f;
							}
						}
						else
						{
							flag4 = true;
							BrakePreasure = 0f;
						}
					}
					if (playerRef.currentGamePadState.IsButtonDown(Buttons.LeftTrigger))
					{
						if (inReverse > 0f)
						{
							if (BrakePreasure < 7f)
							{
								BrakePreasure += 0.1f;
							}
							speed -= BrakePreasure;
							if (speed < 0f)
							{
								speed = 0f;
								inReverse = -1f;
							}
						}
						else
						{
							flag4 = true;
							BrakePreasure = 0f;
						}
					}
				}
				if (FuelLevel > 0f && flag4 && EngineRunning)
				{
					FuelLevel -= 0.005f;
					if (flag)
					{
						_ = IsOffRoadVehicle;
						speed += 0.15f;
					}
					else
					{
						if (speed < 5f)
						{
							if (speed > 0.02f)
							{
								speed += speed;
							}
							else
							{
								speed += 0.01f;
							}
						}
						else
						{
							speed += 0.5f;
						}
						if (speed > maxSpeed)
						{
							speed = maxSpeed;
						}
					}
					if (RFrontWheelQuality == 0 || LFrontWheelQuality == 0 || RRearWheelQuality == 0 || LRearWheelQuality == 0)
					{
						speed = ((speed > 4f) ? 4f : speed);
					}
				}
				else
				{
					speed -= 0.3f;
					if (RFrontWheelQuality == 0 || LFrontWheelQuality == 0 || RRearWheelQuality == 0 || LRearWheelQuality == 0)
					{
						speed -= 0.3f;
					}
					if (speed < 0f)
					{
						speed = 0f;
					}
				}
			}
			if (!inventoryOpen)
			{
				tireSteer = MathHelper.Lerp(tireSteer, playerRef.currentGamePadState.ThumbSticks.Left.X, 0.15f);
			}
			if ((double)Math.Abs(speed) > 0.001)
			{
				if (inReverse < 0f && speed > 15f)
				{
					speed = 15f;
				}
				float num10 = Math.Abs(speed) * 0.1f;
				num10 = ((num10 > 3f) ? 3f : num10);
				vMatTrans *= Matrix.CreateFromAxisAngle(vup, MathHelper.ToRadians(tireSteer * inReverse * (0f - num10)));
				vdirection = Vector3.Transform(vdirection, Matrix.CreateFromAxisAngle(vup, MathHelper.ToRadians(tireSteer * inReverse * (0f - num10 * 1.5f))));
			}
			float num11 = playerRef.currentGamePadState.ThumbSticks.Right.Y * 12f * playerRef.InvertY;
			CamPosHeightPitch -= num11;
			CamPosHeightPitch *= 0.99f;
			if (CamPosHeightPitch > 240f)
			{
				CamPosHeightPitch = 240f;
			}
			if (CamPosHeightPitch < -200f)
			{
				CamPosHeightPitch = -200f;
			}
			tmpPos = vposition;
			cameraAngle += playerRef.currentGamePadState.ThumbSticks.Right.X * 0.2f;
			cameraAngle *= 0.9f;
			Vector3 vector4 = tmpPos;
			vector4.Y += CamPosHeightPitch + 280f + speed * 0.3f;
			Matrix matrix2 = Matrix.CreateRotationY(cameraAngle);
			Vector3 vector5 = Vector3.Transform(vdirection, matrix2);
			vector4 += vector5 * (0f - (640f + speed * 0.2f));
			tmpPos.Y += 180f;
			vector5 = tmpPos - vector4;
			vector5.Normalize();
			AIBase.camOverridePos = Vector3.Lerp(AIBase.camOverridePos, vector4, 0.2f);
			AIBase.camOverrideDir = Vector3.Lerp(AIBase.camOverrideDir, vector5, 0.2f);
			playerRef.OverridePos = AIBase.camOverridePos;
			playerRef.OverrideDir = AIBase.camOverrideDir;
			playerRef.OverrideUp = Vector3.Cross(AIBase.camOverrideDir, vright);
			playerRef.OverrideRight = vright;
			playerRef.vecPosition = AIBase.camOverridePos;
			playerRef.vecDirection = AIBase.camOverrideDir;
			playerRef.CameraDirection = AIBase.camOverrideDir;
			UpdateNetwork();
			playerRef.vecPosition = Position;
			VehicleItemRef.pos = Position;
		}
		else
		{
			if (EnableSpeedDecay)
			{
				speed *= 0.98f;
			}
			if (EGENetWorkNext.networkSession != null && EGENetWorkNext.networkSession.IsHost)
			{
				networkSystem += EndGameEngine.currentTimeStep;
				if (networkSystem > 4f)
				{
					networkSystem -= 4f;
					UpdateNetwork();
				}
			}
		}
		UpdateCollision(playerRef, qIndex, vehicleIndex, AttachedPlayer[playerRef.vehicleSeat] == playerRef);
		UpdateTransform(playerRef, num, qIndex, earlyOut: true);
	}

	public void UpdateTransform(PlayerBase playerRef, float previousSpeed, int qIndex, bool earlyOut)
	{
		if ((double)Math.Abs(speed) > 0.001)
		{
			tireaAngle -= speed * 0.025f * inReverse;
			vposition += vdirection * speed * inReverse;
			for (int i = 0; i < 4; i++)
			{
				float y = tireCurrentPos[i].Y;
				ref Vector3 reference = ref tireCurrentPos[i];
				reference = Vector3.Transform(tirePos[i], vMatTrans);
				tireCurrentPos[i].Y = y + vdirection.Y * speed * inReverse;
				tireCurrentPos[i].X += vposition.X;
				tireCurrentPos[i].Z += vposition.Z;
				float num = HeightMapPhysics.GetHeight(ref tireCurrentPos[i]) + 4f;
				float? num2 = AIBase.WalkableHeight(ref tireCurrentPos[i], qIndex);
				if (num2.HasValue && num2 > num)
				{
					num = num2.Value;
				}
				if (i == 1 && RFrontWheelQuality == 0)
				{
					num -= 16f;
				}
				else if (i == 0 && LFrontWheelQuality == 0)
				{
					num -= 16f;
				}
				else if (i == 3 && RRearWheelQuality == 0)
				{
					num -= 16f;
				}
				else if (i == 2 && LRearWheelQuality == 0)
				{
					num -= 16f;
				}
				if (num > tireCurrentPos[i].Y)
				{
					tireCurrentPos[i].Y = num;
					tireGravity[i] = 0f;
				}
				else if (num < tireCurrentPos[i].Y - 2f)
				{
					tireGravity[i] += 1.5f;
					tireGravity[i] = ((tireGravity[i] > 64f) ? 64f : tireGravity[i]);
					tireCurrentPos[i].Y -= tireGravity[i];
				}
			}
			vright = tireCurrentPos[3] - tireCurrentPos[2] + (tireCurrentPos[1] - tireCurrentPos[0]);
			vright.Normalize();
			frontAxlePos = tireCurrentPos[0] + (tireCurrentPos[1] - tireCurrentPos[0]) * 0.5f;
			rearAxlePos = tireCurrentPos[2] + (tireCurrentPos[3] - tireCurrentPos[2]) * 0.5f;
			vdirection = frontAxlePos - rearAxlePos;
			tmpPos = rearAxlePos + vdirection * 0.5f;
			vposition.Y = MathHelper.Lerp(vposition.Y, tmpPos.Y, 0.1f);
			vdirection.Normalize();
			vup = Vector3.Cross(vright, vdirection);
			vup.Normalize();
			vright = Vector3.Cross(vdirection, vup);
			vright.Normalize();
			vMatTrans.Forward = vdirection;
			vMatTrans.Right = vright;
			vMatTrans.Up = vup;
		}
		else
		{
			tmpPos = vposition;
		}
		if (earlyOut && (vposition - playerRef.vecPosition).LengthSquared() > 225000000f)
		{
			Render[qIndex] = false;
			return;
		}
		ref Matrix reference2 = ref matWorld[qIndex];
		reference2 = vMatTrans;
		tmpPos.X -= playerRef.vecHeadPosition[qIndex].X;
		tmpPos.Z -= playerRef.vecHeadPosition[qIndex].Z;
		matWorld[qIndex].Translation = tmpPos;
		pitchForce = (Speed - previousSpeed) * 3f * inReverse;
		pitchForce = ((pitchForce > 2f) ? 2f : pitchForce);
		pitchForce = ((pitchForce < -2f) ? (-2f) : pitchForce);
		nDir = Vector3.Transform(vdirection, Matrix.CreateFromAxisAngle(vright, MathHelper.ToRadians(pitchForce)));
		tmpChasisDir.X = vdirection.X * 100f;
		tmpChasisDir.Z = vdirection.Z * 100f;
		tmpChasisDir.Y = MathHelper.SmoothStep(tmpChasisDir.Y * 100f, nDir.Y * 100f, 0.25f);
		tmpChasisDir.Normalize();
		leanForce = speed / maxSpeed;
		leanForce = ((leanForce > 0.05f) ? 0.05f : leanForce);
		mRight = Matrix.CreateFromAxisAngle(tmpChasisDir, tireSteer * (1f - speed / maxSpeed) * (0f - leanForce));
		tmpChasisRight = Vector3.SmoothStep(tmpChasisRight, Vector3.Transform(vright, mRight), 0.5f);
		tmpChasisRight.Normalize();
		nUp = Vector3.Cross(tmpChasisRight, tmpChasisDir);
		nUp.Normalize();
		yDiff = MathHelper.SmoothStep(matChasis[qIndex].Translation.Y, tmpPos.Y, 0.25f) - tmpPos.Y;
		yDiff = ((yDiff > 6f) ? 6f : yDiff);
		yDiff = ((yDiff < -6f) ? (-6f) : yDiff);
		chasisY = tmpPos;
		chasisY.Y += yDiff;
		ref Matrix reference3 = ref matChasis[qIndex];
		reference3 = matWorld[qIndex];
		matChasis[qIndex].Forward = tmpChasisDir;
		matChasis[qIndex].Up = nUp;
		matChasis[qIndex].Right = tmpChasisRight;
		matChasis[qIndex].Translation = chasisY;
		TestFrustum(playerRef, qIndex);
	}

	private void UpdateCollision(PlayerBase playerRef, int qIndex, int vehicleIndex, bool transmitData)
	{
		if (speed < 0.001f)
		{
			return;
		}
		int treePositions = LevelBaseMenu.tmpTerrainVegitation.GetTreePositions(ref vposition, qIndex, playerRef);
		for (int i = 0; i < treePositions; i++)
		{
			tmpDir = vposition - TerrainVegetation.GetTreePosList[i];
			tmpDir.Y = 0f;
			if (tmpDir.LengthSquared() < 32400f)
			{
				vposition.X += tmpDir.X * (1f - tmpDir.LengthSquared() / 32400f);
				vposition.Z += tmpDir.Z * (1f - tmpDir.LengthSquared() / 32400f);
				tmpDir.Normalize();
				float num = Vector3.Dot(vdirection, tmpDir);
				ApplyTireDamage(num, (int)(speed * 0.15f), ref tmpDir);
				speed -= Math.Abs(num) * 20f;
				speed = ((speed < 0f) ? 0.1f : speed);
				PlayCrashSound(speed);
			}
		}
		collisionSphere.Radius = 80f + speed * 0.6f;
		collisionSphere.Center = vposition + vdirection * 100f;
		if (AIBase.SphereCollision(ref collisionSphere, qIndex, testWalkable: false))
		{
			Vector3 d = collisionSphere.Center - vposition;
			if (d.LengthSquared() > 1f)
			{
				d.Normalize();
				vposition -= d;
				float num2 = Vector3.Dot(vdirection, d) * (speed * 0.5f);
				ApplyTireDamage(num2 * -1f, (int)(num2 * 0.5f), ref d);
				vright = Vector3.Cross(vdirection, Vector3.UnitY);
				speed -= num2;
				vdirection = d;
				vright = Vector3.Cross(vdirection, Vector3.UnitY);
				vright.Normalize();
				vup = Vector3.Cross(vright, vdirection);
				vup.Normalize();
				vright = Vector3.Cross(vdirection, vup);
				vright.Normalize();
				vMatTrans.Forward = vdirection;
				vMatTrans.Right = vright;
				vMatTrans.Up = vup;
				PlayCrashSound(speed);
				if (transmitData)
				{
					NetworkSendVehicleUpdate(playerRef, vehicleIndex);
				}
			}
		}
		collisionSphere.Radius = 100f + speed * 0.6f;
		collisionSphere.Center = vposition - vdirection * 100f;
		if (!AIBase.SphereCollision(ref collisionSphere, qIndex, testWalkable: false))
		{
			return;
		}
		Vector3 d2 = collisionSphere.Center - (vposition - vdirection * 100f);
		if (d2.LengthSquared() > 1f)
		{
			d2.Normalize();
			float num3 = Math.Abs(Vector3.Dot(vdirection, d2)) * (speed * 0.5f);
			ApplyTireDamage(num3, (int)(num3 * 0.5f), ref d2);
			vright = Vector3.Cross(vdirection, Vector3.UnitY);
			speed -= num3;
			vposition = collisionSphere.Center + vdirection * 100f;
			PlayCrashSound(speed);
			if (transmitData)
			{
				NetworkSendVehicleUpdate(playerRef, vehicleIndex);
			}
		}
	}

	public void SendVehicleDataPacket(PacketWriter pWriter, int index, bool isHost)
	{
		pWriter.Write((byte)117);
		pWriter.Write(index);
		pWriter.Write(VehicleItemRef.uid);
		pWriter.Write((byte)RFrontWheelQuality);
		pWriter.Write((byte)LFrontWheelQuality);
		pWriter.Write((byte)RRearWheelQuality);
		pWriter.Write((byte)LRearWheelQuality);
		pWriter.Write((byte)FuelLevel);
		pWriter.Write((byte)VehicleDamage);
		if (isHost)
		{
			EGENetWorkNext.networkSession.LocalGamers[0].SendData(pWriter, SendDataOptions.InOrder);
		}
		else
		{
			EGENetWorkNext.networkSession.LocalGamers[0].SendData(pWriter, SendDataOptions.InOrder, EGENetWorkNext.networkSession.Host);
		}
	}

	public void SendVehicleSpawnPacket(PacketWriter pWriter, int index, byte gamerIdreciepient)
	{
		pWriter.Write((byte)118);
		pWriter.Write(index);
		pWriter.Write(gamerIdreciepient);
		pWriter.Write(VehicleItemRef.uid);
		pWriter.Write((byte)RFrontWheelQuality);
		pWriter.Write((byte)LFrontWheelQuality);
		pWriter.Write((byte)RRearWheelQuality);
		pWriter.Write((byte)LRearWheelQuality);
		pWriter.Write((byte)FuelLevel);
		pWriter.Write((byte)VehicleDamage);
		pWriter.Write((byte)((AttachedPlayer[0] != null) ? AttachedPlayer[0].NetGamerId : 0));
		pWriter.Write((byte)((AttachedPlayer[1] != null) ? AttachedPlayer[1].NetGamerId : 0));
		pWriter.Write((byte)((AttachedPlayer[2] != null) ? AttachedPlayer[2].NetGamerId : 0));
		pWriter.Write((byte)((AttachedPlayer[3] != null) ? AttachedPlayer[3].NetGamerId : 0));
		pWriter.Write(Position);
		pWriter.Write((byte)((inReverse + 1f) * 255f));
		NormalizedByte4 normalizedByte = new NormalizedByte4(Direction.X, Direction.Y, Direction.Z, 0f);
		HalfVector2 halfVector = new HalfVector2(speed, tireSteer);
		pWriter.Write(normalizedByte.PackedValue);
		pWriter.Write(halfVector.PackedValue);
		pWriter.Write((byte)(HeadLightOn ? 1u : 0u));
		EGENetWorkNext.networkSession.LocalGamers[0].SendData(pWriter, SendDataOptions.ReliableInOrder);
	}

	private void TestFrustum(PlayerBase playerRef, int qIndex)
	{
		frustumSphere.Center = matChasis[qIndex].Translation;
		frustumSphere.Radius = 320f;
		ContainmentType result = ContainmentType.Disjoint;
		playerRef.bFrustum[qIndex].Contains(ref frustumSphere, out result);
		if (result == ContainmentType.Contains || result == ContainmentType.Intersects)
		{
			Render[qIndex] = true;
		}
		else
		{
			Render[qIndex] = false;
		}
	}

	public void UpdatePlayerAttachedSound(bool offRoad)
	{
		float num = (vposition - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[0]).LengthSquared();
		if (num < 400000000f)
		{
			num = num / 400000000f * 20000f;
		}
		if (!EngineStart)
		{
			if (!DoorSound.IsPlaying)
			{
				EngineStart = true;
				if (!StartSound.IsDisposed)
				{
					StartSound.Dispose();
				}
				StartSound = EndGameEngine.SoundBnk.GetCue("VehicleStart");
				StartSound.Play();
				StartSound.SetVariable("Distance", num);
			}
		}
		else
		{
			if (!StartSound.IsPlaying && !EngineSound.IsPlaying)
			{
				EngineSound.Dispose();
				if (FuelLevel > 0f)
				{
					EngineRunning = true;
					EngineSound = EndGameEngine.SoundBnk.GetCue(EngineSoundName);
					EngineSound.Play();
				}
			}
			if (FuelLevel <= 0f)
			{
				EngineRunning = false;
				EngineSound.Dispose();
			}
		}
		if (EngineRunning && EngineSound.IsPlaying)
		{
			float num2 = 100f - speed * 1.5f + num;
			float num3 = speed * 1.5f;
			EngineSound.SetVariable("Distance", (num2 > 0f) ? num2 : 0f);
			EngineSound.SetVariable("Pitch", (num3 < 100f) ? num3 : 100f);
		}
		if (DoorSound.IsPlaying)
		{
			DoorSound.SetVariable("Distance", num);
		}
	}

	private void RemotePlayerContact(int qIndex)
	{
		PropModelBase.TestRayCast = true;
		PlayerBase playerBase = null;
		int index = 0;
		while ((playerBase = EGENetWorkNext.NextNetPlayerReference(ref index)) != null)
		{
			if (playerBase.BloodLevel > 0f)
			{
				tmpNorm = Vector3.Zero;
				tmpSphere.Center = playerBase.vecPosition;
				tmpSphere.Radius = 48f;
				if (SphereColision(ref tmpSphere, ref tmpNorm, qIndex))
				{
					Speed *= 0.995f;
					if (Speed < 20f)
					{
						playerBase.vecPosition += tmpNorm * Speed * 1.5f;
					}
				}
			}
			index++;
		}
	}

	public void PlayCrashSound(float s)
	{
		if (LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].SpawnOverRideTimer > 4f && !CrashSound.IsPlaying)
		{
			CrashSound.Dispose();
			CrashSound = EndGameEngine.SoundBnk.GetCue("ApocZCrash00");
			CrashSound.Play();
			CrashSound.SetVariable("Distance", (200f - s * 2f > 0f) ? (200f - s * 2f) : 0f);
		}
	}

	public void PlayDoorSound()
	{
		float num = (vposition - LevelBaseMenu.Players[(int)EndGameEngine.controllingPlayer.Value].vecHeadPosition[0]).LengthSquared();
		if (num < 400000000f)
		{
			num = num / 400000000f * 20000f;
		}
		if (!DoorSound.IsDisposed)
		{
			DoorSound.Dispose();
		}
		DoorSound = EndGameEngine.SoundBnk.GetCue("VehicleDoorOpenClose");
		DoorSound.Play();
		DoorSound.SetVariable("Distance", num);
	}

	public void PlayerAttachHardSetCamera(PlayerBase playerRef)
	{
		DispInstructionsTimer = 12f;
		AIBase.BlackFadeTimer = 1f;
		float num = playerRef.currentGamePadState.ThumbSticks.Left.Y * 4f;
		CamPosHeightPitch -= num;
		CamPosHeightPitch *= 0.99f;
		if (CamPosHeightPitch > 240f)
		{
			CamPosHeightPitch = 240f;
		}
		if (CamPosHeightPitch < -100f)
		{
			CamPosHeightPitch = -100f;
		}
		cameraAngle += playerRef.currentGamePadState.ThumbSticks.Left.X * 0.2f;
		cameraAngle *= 0.9f;
		AIBase.camOverridePos = vposition;
		AIBase.camOverridePos.Y += CamPosHeightPitch + 320f;
		Matrix matrix = Matrix.CreateRotationY(cameraAngle);
		AIBase.camOverrideDir = Vector3.Transform(vdirection, matrix);
		AIBase.camOverridePos += AIBase.camOverrideDir * (0f - (800f + speed));
		tmpPos = vposition;
		tmpPos.Y += 120f;
		AIBase.camOverrideDir = tmpPos - AIBase.camOverridePos;
		AIBase.camOverrideDir.Normalize();
		playerRef.OverridePos = AIBase.camOverridePos;
		playerRef.OverrideDir = AIBase.camOverrideDir;
		playerRef.OverrideUp = Vector3.Cross(AIBase.camOverrideDir, vright);
		playerRef.OverrideRight = vright;
		playerRef.vecPosition = AIBase.camOverridePos;
		playerRef.vecDirection = AIBase.camOverrideDir;
		playerRef.ThirdPersonCamera = false;
	}

	public void ApplyTireDamage(float a, int f, ref Vector3 d)
	{
		if (a < 0f)
		{
			if (Vector3.Dot(d, Vector3.Cross(vdirection, Vector3.Up)) > 0f)
			{
				LFrontWheelQuality -= f;
			}
			else
			{
				RFrontWheelQuality -= f;
			}
		}
		else if (Vector3.Dot(d, Vector3.Cross(vdirection, Vector3.Up)) > 0f)
		{
			LRearWheelQuality -= f;
		}
		else
		{
			RRearWheelQuality -= f;
		}
		LFrontWheelQuality = ((LFrontWheelQuality > 0) ? LFrontWheelQuality : 0);
		RFrontWheelQuality = ((RFrontWheelQuality > 0) ? RFrontWheelQuality : 0);
		LRearWheelQuality = ((LRearWheelQuality > 0) ? LRearWheelQuality : 0);
		RRearWheelQuality = ((RRearWheelQuality > 0) ? RRearWheelQuality : 0);
	}

	private void NetworkSendVehicleUpdate(PlayerBase playerRef, int vehicleIndex)
	{
		if (EGENetWorkNext.networkSession != null)
		{
			PacketWriter packetWriter = EGENetWorkNext.packetWriter;
			SendVehicleDataPacket(packetWriter, vehicleIndex, EGENetWorkNext.networkSession.IsHost);
		}
	}

	private void DoVehicleCollision(int qIndex)
	{
	}

	public bool SphereColision(ref BoundingSphere sphere, ref Vector3 normal, int qIndex)
	{
		float num = sphere.Radius * sphere.Radius;
		sphereColVector = sphere.Center - vposition;
		float num2 = sphereColVector.LengthSquared();
		float num3 = 600f + speed * 0.6f;
		num3 *= num3;
		if (num2 > num3 + num)
		{
			return false;
		}
		sphereColVector = sphere.Center - (vposition + vdirection * 150f);
		num2 = sphereColVector.LengthSquared();
		num3 = 80f + speed * 0.6f;
		num3 *= num3;
		if (num2 < num3 + num)
		{
			normal = sphereColVector;
			normal.Normalize();
			return true;
		}
		sphereColVector = sphere.Center - vposition;
		num2 = sphereColVector.LengthSquared();
		num3 = 100f + speed * 0.6f;
		num3 *= num3;
		if (num2 < num3 + num)
		{
			normal = sphereColVector;
			normal.Normalize();
			return true;
		}
		sphereColVector = sphere.Center - (vposition - vdirection * 100f);
		num2 = sphereColVector.LengthSquared();
		num3 = 100f + speed * 0.6f;
		num3 *= num3;
		if (num2 < num3 + num)
		{
			normal = sphereColVector;
			normal.Normalize();
			return true;
		}
		return false;
	}

	public void DetachPlayer(PlayerBase playerRef, int qIndex)
	{
		AIBase.BlackFadeTimer = 1f;
		AttachedPlayer[playerRef.vehicleSeat] = null;
		DisplayCanAttachText = true;
		VehicleMenuOpen = false;
		playerRef.IsAttached0 = false;
		playerRef.OverrideCamera = false;
		playerRef.OverridePosition = false;
		playerRef.OverrideButtonTriggerRight = false;
		playerRef.OverrideProjection = false;
		GetOutOfTruck(qIndex, playerRef);
		if (EGENetWorkNext.networkSession != null)
		{
			if (EGENetWorkNext.networkSession.IsHost)
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)115);
				packetWriter.Write(EGENetWorkNext.networkSession.Host.Id);
				packetWriter.Write(VehicleItemRef.uid);
				packetWriter.Write((byte)playerRef.vehicleSeat);
				packetWriter.Write((byte)(HeadLightOn ? 1u : 0u));
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.ReliableInOrder);
			}
			else
			{
				PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
				packetWriter2.Write((byte)115);
				packetWriter2.Write(VehicleItemRef.uid);
				packetWriter2.Write((byte)playerRef.vehicleSeat);
				packetWriter2.Write((byte)(HeadLightOn ? 1u : 0u));
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.ReliableInOrder, EGENetWorkNext.networkSession.Host);
			}
		}
		EngineStart = false;
		EngineRunning = false;
		if (!EngineSound.IsDisposed)
		{
			EngineSound.Dispose();
		}
		if (!DoorSound.IsDisposed)
		{
			DoorSound.Dispose();
		}
		PlayDoorSound();
	}

	public void RemoteDetachPlayer(PlayerBase playerRef, int qIndex)
	{
		AttachedPlayer[playerRef.vehicleSeat] = null;
		DisplayCanAttachText = true;
		VehicleMenuOpen = false;
		playerRef.IsAttached0 = false;
		playerRef.OverrideCamera = false;
		playerRef.OverridePosition = false;
		playerRef.OverrideButtonTriggerRight = false;
		playerRef.OverrideProjection = false;
		GetOutOfTruck(qIndex, playerRef);
		EngineStart = false;
		EngineRunning = false;
		if (!EngineSound.IsDisposed)
		{
			EngineSound.Dispose();
		}
		if (!DoorSound.IsDisposed)
		{
			DoorSound.Dispose();
		}
		PlayDoorSound();
	}

	private void UpdateNetwork()
	{
		if (IsOccupied() && isSpawned)
		{
			if (EGENetWorkNext.networkSession != null && EGENetWorkNext.networkSession.IsHost)
			{
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)116);
				packetWriter.Write(EGENetWorkNext.networkSession.Host.Id);
				packetWriter.Write(VehicleItemRef.uid);
				packetWriter.Write(Position);
				packetWriter.Write((byte)((inReverse + 1f) * 255f));
				NormalizedByte4 normalizedByte = new NormalizedByte4(Direction.X, Direction.Y, Direction.Z, 0f);
				HalfVector2 halfVector = new HalfVector2(speed, tireSteer);
				packetWriter.Write(normalizedByte.PackedValue);
				packetWriter.Write(halfVector.PackedValue);
				packetWriter.Write((byte)(HeadLightOn ? 1u : 0u));
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter, SendDataOptions.InOrder);
			}
			else if (EGENetWorkNext.networkSession != null)
			{
				PacketWriter packetWriter2 = EGENetWorkNext.packetWriter;
				packetWriter2.Write((byte)116);
				packetWriter2.Write(VehicleItemRef.uid);
				packetWriter2.Write(Position);
				packetWriter2.Write((byte)((inReverse + 1f) * 255f));
				NormalizedByte4 normalizedByte2 = new NormalizedByte4(Direction.X, Direction.Y, Direction.Z, 0f);
				HalfVector2 halfVector2 = new HalfVector2(speed, tireSteer);
				packetWriter2.Write(normalizedByte2.PackedValue);
				packetWriter2.Write(halfVector2.PackedValue);
				packetWriter2.Write((byte)(HeadLightOn ? 1u : 0u));
				EGENetWorkNext.networkSession.LocalGamers[0].SendData(packetWriter2, SendDataOptions.InOrder, EGENetWorkNext.networkSession.Host);
			}
		}
	}

	public override void Draw(PlayerBase viewer, int qIndex)
	{
		if (!Render[qIndex] || propModel == null)
		{
			return;
		}
		ShaderPass = 0;
		matWheelDraw = Matrix.CreateRotationX(tireaAngle);
		matFrontWheelDraw = Matrix.CreateRotationZ(tireSteer * (1f - speed / maxSpeed) * -0.7f);
		EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			if (i == HeadLightRightIndex || i == HeadLightLeftIndex)
			{
				continue;
			}
			PropModelBase.drawMesh = propModel.Meshes[i];
			if (RFrontWheelQuality == 0 && i == RightFrontWheelIndex)
			{
				matBrakeDrumDraw = matWorld[qIndex];
				matBrakeDrumDraw.Translation = (matWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]).Translation;
				DrawBrakeDrum(viewer, qIndex);
				continue;
			}
			if (LFrontWheelQuality == 0 && i == LeftFrontWheelIndex)
			{
				matBrakeDrumDraw = matWorld[qIndex];
				matBrakeDrumDraw.Translation = (matWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]).Translation;
				DrawBrakeDrum(viewer, qIndex);
				continue;
			}
			if (RRearWheelQuality == 0 && i == RightRearWheelIndex)
			{
				matBrakeDrumDraw = matWorld[qIndex];
				matBrakeDrumDraw.Translation = (matWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]).Translation;
				DrawBrakeDrum(viewer, qIndex);
				continue;
			}
			if (LRearWheelQuality == 0 && i == LeftRearWheelIndex)
			{
				matBrakeDrumDraw = matWorld[qIndex];
				matBrakeDrumDraw.Translation = (matWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]).Translation;
				DrawBrakeDrum(viewer, qIndex);
				continue;
			}
			for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
			{
				PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[j];
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(PropModelBase.drawMeshPart.VertexBuffer, PropModelBase.drawMeshPart.VertexOffset);
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.Indices = PropModelBase.drawMeshPart.IndexBuffer;
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
				if (i == RightFrontWheelIndex || i == LeftFrontWheelIndex)
				{
					((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(matWheelDraw * matFrontWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]);
				}
				else if (i == RightRearWheelIndex || i == LeftRearWheelIndex)
				{
					((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(matWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]);
				}
				else
				{
					((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matChasis[qIndex]);
				}
				float num = ((LevelOutside.DayLightScalar < 0.1f) ? 0.1f : LevelOutside.DayLightScalar);
				PropModelBase.drawMeshPart.Effect.Parameters["MaterialReflectScalar"].SetValue(MaterialReflectScalar * num);
				PropModelBase.drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PropModelBase.drawMeshPart.NumVertices, PropModelBase.drawMeshPart.StartIndex, PropModelBase.drawMeshPart.PrimitiveCount);
			}
		}
	}

	private void DrawBrakeDrum(PlayerBase viewer, int qIndex)
	{
		for (int i = 0; i < BrakeDrum.Meshes.Count; i++)
		{
			PropModelBase.drawMesh = BrakeDrum.Meshes[i];
			for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
			{
				PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[j];
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(PropModelBase.drawMeshPart.VertexBuffer, PropModelBase.drawMeshPart.VertexOffset);
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.Indices = PropModelBase.drawMeshPart.IndexBuffer;
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
				((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(BrakeDrumTrans[PropModelBase.drawMesh.ParentBone.Index] * matBrakeDrumDraw);
				PropModelBase.drawMeshPart.Effect.CurrentTechnique.Passes[ShaderPass].Apply();
				PropModelBase.drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PropModelBase.drawMeshPart.NumVertices, PropModelBase.drawMeshPart.StartIndex, PropModelBase.drawMeshPart.PrimitiveCount);
			}
		}
	}

	public override void DrawAlpha(PlayerBase viewer, int qIndex)
	{
		if (!Render[qIndex] || propModel == null || !HeadLightOn)
		{
			return;
		}
		EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
		EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Additive;
		EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			if (i == HeadLightRightIndex || i == HeadLightLeftIndex)
			{
				PropModelBase.drawMesh = propModel.Meshes[i];
				for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
				{
					PropModelBase.drawMeshPart = PropModelBase.drawMesh.MeshParts[j];
					PropModelBase.drawMeshPart.Effect.GraphicsDevice.SetVertexBuffer(PropModelBase.drawMeshPart.VertexBuffer, PropModelBase.drawMeshPart.VertexOffset);
					PropModelBase.drawMeshPart.Effect.GraphicsDevice.Indices = PropModelBase.drawMeshPart.IndexBuffer;
					((PropEffectParams)PropModelBase.drawMeshPart.Tag).eyePosition.SetValue(viewer.mDataQueue[qIndex].cameraEyePos);
					((PropEffectParams)PropModelBase.drawMeshPart.Tag).matViewProj.SetValue(viewer.mDataQueue[qIndex].viewProj);
					((PropEffectParams)PropModelBase.drawMeshPart.Tag).matWorld.SetValue(propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matChasis[qIndex]);
					PropModelBase.drawMeshPart.Effect.CurrentTechnique.Passes[19].Apply();
					PropModelBase.drawMeshPart.Effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, PropModelBase.drawMeshPart.NumVertices, PropModelBase.drawMeshPart.StartIndex, PropModelBase.drawMeshPart.PrimitiveCount);
				}
			}
		}
	}

	public override void DrawShadowMap(PlayerBase viewer, ref Matrix lighViewProj, ref Vector3 lightPos, int qIndex, bool lod)
	{
		if (!Render[qIndex] || propModel == null)
		{
			return;
		}
		GraphicsDevice graphicsDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		graphicsDevice.BlendState = BlendState.Opaque;
		graphicsDevice.DepthStencilState = DepthStencilState.Default;
		graphicsDevice.RasterizerState = EndGameEngine.RasterCullCW;
		PropModelBase.tmpMatWorld = matWorld[qIndex];
		matWheelDraw = Matrix.CreateRotationX(tireaAngle);
		matFrontWheelDraw = Matrix.CreateRotationZ(tireSteer * (1f - speed / maxSpeed) * -1f);
		for (int i = 0; i < propModel.Meshes.Count; i++)
		{
			if (i == HeadLightRightIndex || i == HeadLightLeftIndex)
			{
				continue;
			}
			PropModelBase.drawMesh = propModel.Meshes[i];
			if (((MeshAttributesParams)PropModelBase.drawMesh.Tag).ObjectType != EnumObjectTypes.Render)
			{
				continue;
			}
			for (int j = 0; j < PropModelBase.drawMesh.MeshParts.Count; j++)
			{
				ModelMeshPart modelMeshPart = PropModelBase.drawMesh.MeshParts[j];
				Effect effect = modelMeshPart.Effect;
				effect.GraphicsDevice.SetVertexBuffer(modelMeshPart.VertexBuffer, modelMeshPart.VertexOffset);
				effect.GraphicsDevice.Indices = modelMeshPart.IndexBuffer;
				((PropEffectParams)modelMeshPart.Tag).eyePosition.SetValue(lightPos);
				((PropEffectParams)modelMeshPart.Tag).matLightViewProj.SetValue(lighViewProj);
				((PropEffectParams)modelMeshPart.Tag).matWorld.SetValue(propTransforms[PropModelBase.drawMesh.ParentBone.Index] * PropModelBase.tmpMatWorld);
				if (i == RightFrontWheelIndex || i == LeftFrontWheelIndex)
				{
					((PropEffectParams)modelMeshPart.Tag).matWorld.SetValue(matWheelDraw * matFrontWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]);
				}
				else if (i == RightRearWheelIndex || i == LeftRearWheelIndex)
				{
					((PropEffectParams)modelMeshPart.Tag).matWorld.SetValue(matWheelDraw * propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matWorld[qIndex]);
				}
				else
				{
					((PropEffectParams)modelMeshPart.Tag).matWorld.SetValue(propTransforms[PropModelBase.drawMesh.ParentBone.Index] * matChasis[qIndex]);
				}
				if (((MeshAttributesParams)PropModelBase.drawMesh.Tag).Opacity == EnumOpacityTypes.AlphaTest)
				{
					effect.CurrentTechnique.Passes[21].Apply();
				}
				else
				{
					effect.CurrentTechnique.Passes[21].Apply();
				}
				effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, modelMeshPart.NumVertices, modelMeshPart.StartIndex, modelMeshPart.PrimitiveCount);
			}
		}
	}

	public override void DrawPost(PlayerBase e, int qIndex)
	{
		if (!Render[qIndex])
		{
			return;
		}
		Viewport viewport = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport;
		if (AttachedPlayer[e.vehicleSeat] == e)
		{
			DispInstructionsTimer -= 0.0334f;
			float num = ((DispInstructionsTimer > 1f) ? 1f : DispInstructionsTimer);
			uiPos.X = viewport.TitleSafeArea.Right - 220;
			uiPos.Y = viewport.TitleSafeArea.Bottom - 220;
			uiRec.X = (int)(uiPos.X - 42f);
			uiRec.Y = (int)(uiPos.Y + 2f);
			uiRec.Width = 34;
			uiRec.Height = 34;
			Menu.spriteBatch.Begin();
			if (DispInstructionsTimer > 0f)
			{
				Color white = Color.White;
				Color black = Color.Black;
				black.A = (byte)(255f * num);
				white.A = (byte)(211f * num);
				white.R = white.A;
				white.G = white.A;
				white.B = white.A;
				Menu.spriteBatch.Draw(Menu.leftStick, uiRec, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgSteer, uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgSteer, uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				uiRec.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.rightTrigger, uiRec, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgForward, uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgForward, uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				uiRec.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.leftTrigger, uiRec, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgBrake, uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgBrake, uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				uiRec.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.rightStick, uiRec, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgCamera, uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgCamera, uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
				uiPos.Y += 36f;
				uiRec.Y = (int)(uiPos.Y + 2f);
				Menu.spriteBatch.Draw(Menu.dpRight, uiRec, white);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgLights, uiPos, black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, msgLights, uiPos, white, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			}
			uiPos.Y = viewport.TitleSafeArea.Bottom - 40;
			uiRec.Y = (int)(uiPos.Y + 2f);
			Menu.spriteBatch.Draw(Menu.xButton, uiRec, Color.White);
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgExit, uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
			Menu.spriteBatch.DrawString(Menu.defaultFont, msgExit, uiPos, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			FuelLevel = ((FuelLevel < 1f) ? 0f : FuelLevel);
			FuelBlinktimer -= 0.05f;
			FuelBlinktimer = ((FuelBlinktimer < 0f) ? 1f : FuelBlinktimer);
			FuelDispColor = Color.LightGray;
			FuelShadColor = Color.Black;
			if (FuelLevel < 1f)
			{
				FuelDispColor = Color.DarkRed;
				FuelDispColor.A = (byte)(FuelBlinktimer * 255f);
				FuelDispColor.R = (byte)(FuelBlinktimer * 140f);
				FuelShadColor.A = (byte)(FuelBlinktimer * 255f);
			}
			else if (FuelLevel < 10f)
			{
				FuelDispColor = Color.DarkRed;
			}
			else if (FuelLevel < 20f)
			{
				FuelDispColor = Color.Yellow;
			}
			uiPos.X = viewport.TitleSafeArea.Left;
			uiPos.Y = viewport.TitleSafeArea.Bottom - 132;
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Fuel  " + (int)FuelLevel, uiPos + new Vector2(2f, 2f), FuelShadColor);
			Menu.spriteBatch.DrawString(Menu.defaultFont, "Fuel  " + (int)FuelLevel, uiPos, FuelDispColor);
			Menu.spriteBatch.End();
		}
		else if (DisplayCanAttachText && !InventoryCls.InventoryOpen)
		{
			Menu.spriteBatch.Begin();
			if (VehicleMenuOpen)
			{
				uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString(msgEnter).X * 0.5f * 1.1f;
				uiPos.Y = viewport.TitleSafeArea.Bottom - 180;
				uiRec.X = (int)(uiPos.X - 56f);
				uiRec.Y = (int)(uiPos.Y + 2f);
				uiRec.Width = 40;
				uiRec.Height = 40;
				Menu.DrawButton(uiRec, Buttons.X, Color.White);
				if (e.vehicleSeat == 0)
				{
					Menu.spriteBatch.DrawString(Menu.defaultFont, msgEnter + " Driver", uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, msgEnter + " Driver", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
				}
				else
				{
					Menu.spriteBatch.DrawString(Menu.defaultFont, msgEnter + " Passenger", uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, msgEnter + " Passenger", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
				}
				if (RFrontWheelQuality == 0 || LFrontWheelQuality == 0 || RRearWheelQuality == 0 || LRearWheelQuality == 0)
				{
					uiPos.Y += 38f;
					uiRec.X = (int)(uiPos.X - 56f);
					uiRec.Y = (int)(uiPos.Y + 2f);
					Menu.DrawButton(uiRec, Buttons.Y, Color.White);
					Menu.spriteBatch.DrawString(Menu.defaultFont, "Repair Tire", uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, "Repair Tire", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
				}
				if (FuelLevel < 100f)
				{
					uiPos.Y += 38f;
					uiRec.X = (int)(uiPos.X - 56f);
					uiRec.Y = (int)(uiPos.Y + 2f);
					Menu.DrawButton(uiRec, Buttons.B, Color.White);
					Menu.spriteBatch.DrawString(Menu.defaultFont, "Refuel", uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
					Menu.spriteBatch.DrawString(Menu.defaultFont, "Refuel", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
				}
				uiPos.Y += 38f;
				uiRec.X = (int)(uiPos.X - 56f);
				uiRec.Y = (int)(uiPos.Y + 2f);
				Menu.DrawButton(uiRec, Buttons.DPadUp, Color.White);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Close Vehicle Menu", uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Close Vehicle Menu", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			}
			else
			{
				uiPos.X = (float)viewport.TitleSafeArea.Center.X - Menu.defaultFont.MeasureString("To Open Vehicle Menu").X * 0.45f;
				uiPos.Y = viewport.TitleSafeArea.Bottom - 100;
				uiRec.X = (int)(uiPos.X - 56f);
				uiRec.Y = (int)(uiPos.Y + 2f);
				uiRec.Width = 40;
				uiRec.Height = 40;
				Menu.DrawButton(uiRec, Buttons.DPadUp, Color.White);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Open Vehicle Menu", uiPos, Color.Black, 0f, new Vector2(-2f, -2f), 1.1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Open Vehicle Menu", uiPos, Color.LightGray, 0f, Vector2.Zero, 1.1f, SpriteEffects.None, 0);
			}
			Menu.spriteBatch.End();
		}
		if (VehicleMenuOpen || e.IsAttached0)
		{
			Vector2 zero = Vector2.Zero;
			zero.X = (uiPos.X = viewport.TitleSafeArea.Left);
			zero.Y = viewport.TitleSafeArea.Bottom - 162;
			Menu.spriteBatch.Begin();
			if (AttachedPlayer[0] != null)
			{
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Driver: " + AttachedPlayer[0].gamerTag, zero, Color.Black, 0f, new Vector2(-2f, -2f), 1f, SpriteEffects.None, 0);
				Menu.spriteBatch.DrawString(Menu.defaultFont, "Driver: " + AttachedPlayer[0].gamerTag, zero, Color.LightGray, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
			}
			Menu.spriteBatch.End();
		}
	}
}
