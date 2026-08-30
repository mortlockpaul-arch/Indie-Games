using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class ModelInstanceCar : ModelInstance
{
	public class StaticData
	{
		public const float Ralenti = 0.11f;

		public const float MaxAccel = 0.0025f;

		public const float GearChangeTime = 0.75f;

		public readonly float Damper;

		public readonly float RollBar;

		public readonly float FrontWheelDiameter;

		public readonly float RearWheelDiameter;

		public readonly float FrontWheelTurnMax;

		public readonly float SteeringWheelTurnMax;

		public readonly float SteeringAngleMax;

		public readonly float BoundingSphereRadius;

		public readonly float CamCockpitAngle;

		public readonly float CamChase1Angle;

		public readonly float MaxSpeed;

		public readonly float BodyHalfHeight;

		public readonly int Gears;

		public readonly List<float> GearRatios;

		public readonly Vector3 CenterYZ0;

		public readonly Vector3 CamCockpitPos;

		public readonly Vector3 CamChase1Pos;

		public readonly Vector3 FrontAxis;

		private readonly Matrix TransformToCenter;

		public CurveRelationship TorqueCurve;

		public readonly Model Model;

		public readonly Drawing3D_V2.BoundingBoxTransformable boundingBoxData;

		public StaticData(Model m, float Damper, float RollBar, float FrontWheelDiameter, float RearWheelDiameter, float FrontWheelTurnMax, float SteeringWheelTurnMax, float SteeringAngleMax, Vector3 CamCockpitPos, float CamCockpitAngle, Vector3 CamChase1Pos, float CamChase1Angle, List<float> GearRatios, CurveRelationship TorqueCurve, float MaxSpeed)
		{
			MaximinusGame.ContentLoadString = "Car Init";
			int num = 0;
			foreach (ModelMesh mesh in m.Meshes)
			{
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
				{
					num += meshPart.NumVertices;
				}
			}
			Model = m;
			ModelMesh modelMesh = m.Meshes[NameMainBody];
			CenterYZ0 = Vector3.Transform(modelMesh.BoundingSphere.Center * Vector3.UnitX, modelMesh.ParentBone.Transform);
			BoundingSphereRadius = Vector3.Transform(Vector3.UnitX * modelMesh.BoundingSphere.Radius, modelMesh.ParentBone.Transform).Length();
			TransformToCenter = Matrix.CreateTranslation(-CenterYZ0);
			foreach (ModelBone bone in m.Bones)
			{
				bone.Transform *= TransformToCenter * Matrix.CreateRotationY((float)Math.PI / 2f);
			}
			this.Damper = Damper;
			this.RollBar = RollBar;
			this.FrontWheelDiameter = FrontWheelDiameter;
			this.RearWheelDiameter = RearWheelDiameter;
			this.FrontWheelTurnMax = FrontWheelTurnMax;
			this.SteeringWheelTurnMax = SteeringWheelTurnMax;
			this.SteeringAngleMax = SteeringAngleMax;
			this.CamCockpitAngle = CamCockpitAngle;
			this.CamChase1Angle = CamChase1Angle;
			this.CamCockpitPos = CamCockpitPos;
			this.CamChase1Pos = CamChase1Pos;
			Gears = GearRatios.Count;
			this.GearRatios = GearRatios;
			this.TorqueCurve = TorqueCurve;
			this.MaxSpeed = MaxSpeed;
			ModelMesh modelMesh2 = m.Meshes[NameFrontLeftWheel];
			FrontAxis = Vector3.UnitX * Vector3.Transform(modelMesh2.BoundingSphere.Center, modelMesh2.ParentBone.Transform).X;
			BodyHalfHeight = Vector3.Transform(modelMesh.BoundingSphere.Center, modelMesh.ParentBone.Transform).Y;
			boundingBoxData = new Drawing3D_V2.BoundingBoxTransformable(m.Meshes[NameMainBody]);
		}

		public int UpShift(int currentGear)
		{
			return Math.Min(currentGear + 1, Gears);
		}

		public int DownShift(int currentGear)
		{
			return Math.Max(currentGear - 1, 1);
		}
	}

	public float WantedSteer;

	public float WantedSteerDigital;

	public float WantedThrottle;

	public float WantedBrakes;

	private Vector2 pitchRollState;

	private float speedRatio;

	private float rpm;

	private float torque;

	private float power;

	private Timer gearChangeTimer;

	private int gear;

	private float realSteer;

	private float realThrottle;

	private float realBrakes;

	private float forcePitch;

	private float forceRoll;

	private float clutch;

	private Vector3 position;

	private float rotationY;

	private Vector2 tyreLoadFront;

	private float slideRotation;

	private float forwardRotation;

	private Matrix orientation;

	private Matrix world;

	private Matrix worldForSuspendedBones;

	private RoadTrack.Area currentArea;

	public static Debug3D.Cross[] DebugVelocity;

	public static Debug3D.Cross DebugPos;

	public static Debug3D.Cross DebugCrossTrack;

	public static Debug3D.Orientation DebugOrientation;

	private Debug3D.LineList boundingBox;

	public StaticData staticData;

	private static readonly string NameWindShield = "WINDSHIELD";

	private static readonly string NameMainBody = "MAINBODY";

	private static readonly string NameLightsRear = "LIGHTS_REAR";

	private static readonly string NameSTWheel = "STWHEEL";

	private static readonly string NameRearWheels = "RWHEELS";

	private static readonly string NameFrontRightWheel = "FRWHEEL";

	private static readonly string NameFrontLeftWheel = "FLWHEEL";

	private static readonly string NameBrakeDiscLeft = "BRAKEDISCL";

	private static readonly string NameBrakeDiscRight = "BRAKEDISCR";

	private static readonly string NameFrame = "FRAME";

	private ModelMesh meshWindShield;

	private ModelMesh meshLightsRear;

	private ModelMesh meshSTWheel;

	private static readonly List<string> CustomBoneNames = new List<string> { NameWindShield, NameLightsRear, NameSTWheel };

	private bool[] isSuspended;

	private static readonly List<string> NonSuspendedMass = new List<string> { NameRearWheels, NameFrontRightWheel, NameFrontLeftWheel, NameBrakeDiscLeft, NameBrakeDiscRight, NameFrame, NameSTWheel };

	private bool boneHasBrakeDiscs;

	private int boneRearWheels;

	private int boneFRWheel;

	private int boneFLWheel;

	private int boneBrakeDiscL;

	private int boneBrakeDiscR;

	private Vector3 positionOnTrack;

	public Vector3 Velocity => orientation.Forward * speedRatio * staticData.MaxSpeed;

	public Matrix Orientation => orientation;

	public float SpeedRatio => speedRatio;

	public float Drag => speedRatio * speedRatio;

	public float Rpm => rpm;

	public float Torque => torque;

	public float Power => power;

	public int Gear => gear;

	public float ForcePitch => forcePitch;

	public float ForceRoll => forceRoll;

	public float Clutch => clutch;

	public float DamperStrengh => Math.Abs(pitchRollState.X) * MathHelper.Lerp(0.5f, 1f, Math.Abs(forcePitch));

	public float RollBarStrengh => (float)Math.Pow(Math.Abs(pitchRollState.Y), 2.0);

	public Vector2 PitchRollState => pitchRollState;

	public float RealSteer => realSteer;

	public float RealThrottle => realThrottle;

	public float RealBrakes => realBrakes;

	public Vector3 Position => position;

	public float TyreLoadFrontLeft => tyreLoadFront.X;

	public float TyreLoadFrontRight => tyreLoadFront.Y;

	public float SlideRotation => slideRotation;

	public bool TyreStallFront
	{
		get
		{
			if (!(TyreLoadFrontLeft > 1f))
			{
				return TyreLoadFrontRight > 1f;
			}
			return true;
		}
	}

	private bool LightRearON => WantedBrakes > 0f;

	private float WheelTurnRotation => RealSteer;

	private float SteeringWheelTurnRotation => RealSteer;

	private bool clutchEngaged => clutch > 0.02f;

	public ModelInstanceCar(StaticData staticData)
		: base(staticData.Model, CustomBoneNames)
	{
		meshWindShield = model.Meshes[NameWindShield];
		meshLightsRear = model.Meshes[NameLightsRear];
		meshSTWheel = model.Meshes[NameSTWheel];
		this.staticData = staticData;
		isSuspended = new bool[model.Bones.Count];
		for (int i = 0; i < model.Bones.Count; i++)
		{
			isSuspended[i] = !NonSuspendedMass.Contains(model.Bones[i].Name);
		}
		boneRearWheels = FindBoneIndex(NameRearWheels);
		boneFRWheel = FindBoneIndex(NameFrontRightWheel);
		boneFLWheel = FindBoneIndex(NameFrontLeftWheel);
		boneHasBrakeDiscs = HasBone(NameBrakeDiscLeft);
		if (boneHasBrakeDiscs)
		{
			boneBrakeDiscL = FindBoneIndex(NameBrakeDiscLeft);
			boneBrakeDiscR = FindBoneIndex(NameBrakeDiscRight);
		}
		boundingBox = new Debug3D.LineList(staticData.boundingBoxData.DrawingData(showDiagonals: true), Color.Red);
		Reset();
	}

	public void Reset()
	{
		pitchRollState = Vector2.Zero;
		speedRatio = 0f;
		rpm = 0f;
		torque = 0f;
		power = 0f;
		gear = 1;
		realSteer = 0f;
		realThrottle = 0f;
		realBrakes = 0f;
		WantedBrakes = 0f;
		WantedThrottle = 0f;
		WantedSteer = 0f;
		WantedSteerDigital = 0f;
		forcePitch = 0f;
		forceRoll = 0f;
		clutch = 0f;
		forwardRotation = 0f;
		positionOnTrack = Vector3.Zero;
		rotationY = 0f;
		position = Vector3.UnitX / 2f + 1.4f * new Vector3(4.211f, 0f, 4.035f);
		gearChangeTimer = new Timer(0.0, 0f);
		gearChangeTimer.Stop();
		tyreLoadFront = Vector2.Zero;
		slideRotation = 0f;
		UpdateOrientation();
	}

	private void UpdateOrientation()
	{
		Vector3 intersectionPoint = Vector3.Zero;
		currentArea = RoadTrack.Instance.FindArea(Position, (currentArea != null) ? currentArea.Id : 0, out intersectionPoint);
		orientation = Matrix.CreateRotationY(rotationY);
		if (currentArea != null)
		{
			position = intersectionPoint;
			positionOnTrack = intersectionPoint;
			orientation.Up = Vector3.Normalize(currentArea.GetNormal(position));
			orientation.Right = Vector3.Normalize(Vector3.Cross(orientation.Forward, orientation.Up));
			orientation.Forward = Vector3.Normalize(Vector3.Cross(orientation.Up, orientation.Right));
		}
		else
		{
			position.Y = 0f;
		}
	}

	public void StartOfFrame()
	{
		WantedThrottle = 0f;
		WantedBrakes = 0f;
		WantedSteer = 0f;
		WantedSteerDigital = 0f;
	}

	private void Shift(bool isUp)
	{
		if (isUp)
		{
			gear = staticData.UpShift(gear);
		}
		else
		{
			gear = staticData.DownShift(gear);
		}
		gearChangeTimer.Reset(MaximinusGame.gameTime, 0.75f);
		clutch = Math.Abs(EngineSpeedFromVehicleSpeed(gear) - rpm);
	}

	private float EngineSpeedFromVehicleSpeed(int gearParameter)
	{
		return MathHelper.Lerp(0.11f, 1f, Utils.clampRatio(speedRatio * staticData.GearRatios[staticData.Gears - 1] / staticData.GearRatios[gearParameter - 1]));
	}

	public void UpdateDebugIndicators()
	{
	}

	public void Update()
	{
		float num = SpeedRatio;
		if (WantedSteerDigital != 0f)
		{
			realSteer = MathHelper.Lerp(WantedSteerDigital, realSteer, 0.95f);
		}
		else
		{
			realSteer = MathHelper.Lerp(WantedSteer, realSteer, 0.85f);
		}
		if (TyreStallFront)
		{
			WantedThrottle = 0f;
		}
		realThrottle = ((WantedBrakes > 0f) ? 0f : WantedThrottle);
		realBrakes = WantedBrakes;
		if (realBrakes > 0f && speedRatio != 0f)
		{
			int num2 = Math.Sign(speedRatio);
			speedRatio -= 0.004f * realBrakes * (float)Math.Sign(speedRatio);
			if (Math.Sign(speedRatio) != num2)
			{
				speedRatio = 0f;
			}
		}
		if (speedRatio > 0f)
		{
			speedRatio -= Drag * 0.0025f;
		}
		rpm = Math.Max(rpm, 0.11f);
		float num3 = EngineSpeedFromVehicleSpeed(gear);
		clutch = Math.Abs(num3 - rpm);
		if (clutchEngaged)
		{
			realThrottle = 0f;
		}
		torque = staticData.TorqueCurve.Evaluate(rpm);
		power = torque;
		if (realThrottle > 0f)
		{
			rpm += power * realThrottle * 0.02f / staticData.GearRatios[gear - 1];
		}
		else
		{
			rpm -= 0.001f;
		}
		if (!clutchEngaged)
		{
			float value = Utils.clampRatio(MathHelper.Lerp(0f, staticData.GearRatios[gear - 1] / staticData.GearRatios[staticData.Gears - 1], (rpm - 0.11f) / 0.89f));
			float num4 = ((realThrottle > 0f) ? 0.25f : 1f);
			rpm = MathHelper.Lerp(rpm, num3, num4);
			speedRatio = MathHelper.Lerp(speedRatio, value, 1f - num4);
		}
		else
		{
			rpm = MathHelper.Lerp(rpm, num3, 0.15f);
		}
		rpm = Utils.clampRatio(rpm);
		if (!clutchEngaged && rpm > staticData.TorqueCurve.MaximumIndex && gear < staticData.Gears)
		{
			float num5 = EngineSpeedFromVehicleSpeed(gear + 1);
			float num6 = staticData.TorqueCurve.Evaluate(num5);
			if (torque * rpm < num6 * num5)
			{
				Shift(isUp: true);
			}
		}
		if (!clutchEngaged && gear > 1)
		{
			float num7 = EngineSpeedFromVehicleSpeed(gear - 1);
			float num8 = staticData.TorqueCurve.Evaluate(num7);
			if (torque * rpm < num8 * num7)
			{
				Shift(isUp: false);
			}
		}
		float num9 = MathHelper.Clamp((SpeedRatio - num) * 1000f * 1f / 0.9f, -1f, 1f);
		if (speedRatio > 0f)
		{
			if (num9 <= 0f)
			{
				num9 = 0f - realBrakes;
			}
		}
		else
		{
			num9 = 0f;
		}
		forcePitch = MathHelper.Lerp(forcePitch, num9, 0.15f);
		forceRoll = realSteer * speedRatio;
		float num10 = Math.Abs(forceRoll);
		tyreLoadFront.X = num10 + MathHelper.Lerp(0f, 1f, (ForceRoll > 0f) ? (0.5f * Math.Abs(forceRoll)) : 0f);
		tyreLoadFront.Y = num10 + MathHelper.Lerp(0f, 1f, (ForceRoll < 0f) ? (0.5f * Math.Abs(forceRoll)) : 0f);
		Update_PositionVelocity_Bones_Steering();
		Update_Collision_Barriers();
		Audio.Instance.engineStates[0].Volume = 0f;
		Audio.Instance.engineStates[0].Pitch = MathHelper.Lerp(-1f, 0.75f, Utils.clampRatio(rpm));
	}

	private void Update_Collision_Barriers()
	{
		if (currentArea == null || speedRatio == 0f)
		{
			return;
		}
		Matrix matrix = staticData.boundingBoxData.OriginalTransform * worldForSuspendedBones;
		foreach (int barrierIndex in currentArea.barrierIndexes)
		{
			if ((barrierIndex % 2 == 0) ? (currentArea.RatioLow < 0.5f) : (currentArea.RatioLow >= 0.5f))
			{
				Matrix matrix2 = RoadTrack.Instance.barrierBox.OriginalTransform * RoadTrack.Instance.BarrierCollisionMatrixes[barrierIndex];
				Plane plane = Plane.Transform(new Plane(Vector3.UnitX, RoadTrack.Instance.barrierBox.OriginalBox.Max.X), matrix2);
				PlaneIntersectionType planeIntersectionType = staticData.boundingBoxData.OriginalBox.Intersects(Plane.Transform(plane, Matrix.Invert(matrix)));
				if (planeIntersectionType == PlaneIntersectionType.Intersecting)
				{
					Utils.DebugOut("COLL");
				}
			}
		}
	}

	private void Update_PositionVelocity_Bones_Steering()
	{
		Compute_Velocity_Position_Orientation();
		forwardRotation = Velocity.Length() * (float)Math.Sign(speedRatio);
		Vector3 additionalRotation = Vector3.UnitY * forwardRotation * 2f / staticData.FrontWheelDiameter;
		AddRotation(boneRearWheels, Vector3.UnitY * forwardRotation * 2f / staticData.RearWheelDiameter);
		AddRotation(boneFRWheel, additionalRotation);
		AddRotation(boneFLWheel, additionalRotation);
		float v = staticData.FrontWheelTurnMax * WheelTurnRotation;
		SetRotationZ(boneFRWheel, v);
		SetRotationZ(boneFLWheel, v);
		if (boneHasBrakeDiscs)
		{
			SetRotationZ(boneBrakeDiscL, v);
			SetRotationZ(boneBrakeDiscR, v);
		}
		pitchRollState.Y += forceRoll * -1f;
		if (pitchRollState.Y != 0f)
		{
			pitchRollState.Y -= staticData.RollBar * RollBarStrengh * (float)Math.Sign(pitchRollState.Y);
		}
		pitchRollState.Y = Math.Max(-1f, Math.Min(1f, pitchRollState.Y));
		pitchRollState.X += forcePitch * 0.5f;
		if (pitchRollState.X != 0f)
		{
			Math.Abs(speedRatio);
			_ = 0.01f;
			pitchRollState.X -= staticData.Damper * DamperStrengh * (float)Math.Sign(pitchRollState.X);
		}
		pitchRollState.X = MathHelper.Clamp(pitchRollState.X, -1f, 1f);
		world = orientation;
		world *= Matrix.CreateTranslation(position);
		if (slideRotation != 0f)
		{
			world = MyMath.MatrixRotationYCenter(staticData.FrontAxis, (float)Math.PI / 5f * slideRotation) * world;
		}
		Matrix matrix = Matrix.CreateRotationZ(pitchRollState.Y * ((float)Math.PI / 4f) / 7f);
		Matrix matrix2 = Matrix.CreateRotationX(pitchRollState.X * ((float)Math.PI / 4f) / 10f);
		worldForSuspendedBones = matrix * matrix2 * world;
		if (RealSteer != 0f && SpeedRatio != 0f)
		{
			rotationY -= staticData.SteeringAngleMax * ((slideRotation == 0f) ? RealSteer : ((float)Math.Sign(0f - slideRotation) * 3f / 4f)) * ((speedRatio < 0.055f) ? (speedRatio / 0.055f) : ((float)Math.Sign(speedRatio)));
		}
	}

	private void Compute_Velocity_Position_Orientation()
	{
		if (realBrakes == 0f && (!(speedRatio > 0f) || !(speedRatio < 0.3f)))
		{
			speedRatio += Orientation.Forward.Y * -1f * 0.01f;
			if (speedRatio < 0f)
			{
				speedRatio = 0f;
			}
		}
		position += Velocity;
		if (speedRatio != 0f)
		{
			UpdateOrientation();
		}
	}

	public override void Draw(Matrix worldPameter, bool useDefaultLighting)
	{
		MaximinusGame.Draw2D.Device.RasterizerState = RasterizerState.CullNone;
		if (currentArea != null)
		{
			currentArea.DrawBarriersBox();
		}
		boundingBox.Draw(worldForSuspendedBones);
		foreach (ModelMesh mesh in model.Meshes)
		{
			if (!isCustomBone[mesh.ParentBone.Index])
			{
				Drawing3D_V2.DrawModelMesh(mesh, transforms[mesh.ParentBone.Index] * (isSuspended[mesh.ParentBone.Index] ? worldForSuspendedBones : world), useDefaultLighting);
			}
		}
		DrawLightsRear(worldForSuspendedBones);
		DrawSteeringWheel(worldForSuspendedBones);
		Drawing3D_V2.DrawModelMesh(meshWindShield, transforms[meshWindShield.ParentBone.Index] * worldForSuspendedBones, useDefaultLighting: true);
	}

	private void DrawSteeringWheel(Matrix world)
	{
		Matrix transform = Matrix.CreateRotationX(staticData.SteeringWheelTurnMax * SteeringWheelTurnRotation) * transforms[meshSTWheel.ParentBone.Index] * world;
		Drawing3D_V2.DrawModelMesh(meshSTWheel, transform, useDefaultLighting: true);
	}

	private void DrawLightsRear(Matrix world)
	{
		foreach (BasicEffect effect in meshLightsRear.Effects)
		{
			Drawing3D_V2.ApplyEffect(effect, transforms[meshLightsRear.ParentBone.Index] * world, useDefaultLighting: true);
			if (LightRearON)
			{
				effect.DiffuseColor = Vector3.UnitX * 0.8f;
				effect.DirectionalLight1.Enabled = false;
				effect.DirectionalLight2.Enabled = false;
				effect.DirectionalLight0.Direction = Orientation.Forward;
				effect.DirectionalLight0.DiffuseColor = Color.Red.ToVector3();
				effect.DirectionalLight0.SpecularColor = Color.Red.ToVector3();
				effect.SpecularColor = Color.Red.ToVector3();
				effect.SpecularPower = 300f;
			}
			else
			{
				effect.DirectionalLight0.DiffuseColor = Color.Black.ToVector3();
				effect.DirectionalLight1.DiffuseColor = Color.Black.ToVector3();
				effect.DirectionalLight1.DiffuseColor = Color.Black.ToVector3();
			}
		}
		meshLightsRear.Draw();
	}

	public void DrawDebugIndicators()
	{
	}
}
