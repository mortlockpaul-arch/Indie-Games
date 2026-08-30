using System;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace Platformer1;

public class Player1
{
	public const float PhysicsScaleDown = 0.2f;

	public const int offset = 2;

	public const float Dist = 3f;

	public const int RagdollScale = 112;

	public const float BreakPoint = 5f;

	public const int Softness = 1;

	public const float BiasFactor = 0.2f;

	public const int AngleJointMaxImpulse = 30;

	private const float WalkerRadius = 1.2f;

	private const float bodyYRadius = 4.4f;

	private const float bodyXRadius = 3f;

	private const float headYRadius = 3f;

	private const float headXRadius = 2.2f;

	private const float leftUpperArmYRadius = 6.6f;

	private const float leftUpperArmXRadius = 1.2f;

	private const float rightUpperArmYRadius = 6.6f;

	private const float rightUpperArmXRadius = 1.2f;

	private const float leftHandYRadius = 1.6f;

	private const float leftHandXRadius = 0.8f;

	private const float rightHandYRadius = 0.8f;

	private const float rightHandXRadius = 0.4f;

	private const float leftArmYRadius = 1.2f;

	private const float leftArmXRadius = 0.6f;

	private const float rightArmYRadius = 1.2f;

	private const float rightArmXRadius = 0.6f;

	private const float leftThighYRadius = 1.6f;

	private const float leftThighXRadius = 1.2f;

	private const float rightThighYRadius = 1.6f;

	private const float rightThighXRadius = 1.2f;

	private const float leftCalfYRadius = 1.6f;

	private const float leftCalfXRadius = 1.2f;

	private const float rightCalfYRadius = 1.6f;

	private const float rightCalfXRadius = 1.2f;

	private const float leftFootYRadius = 0.2f;

	private const float leftFootXRadius = 0.6f;

	private const float rightFootYRadius = 0.2f;

	private const float rightFootXRadius = 0.6f;

	private const int Edges = 10;

	public const int PointValue = 30;

	private const float MoveStickScale = 8000f;

	private const Buttons JumpButton = Buttons.A;

	private bool keyboardState_IsKeyDown_Keys_Left;

	private bool keyboardState_IsKeyDown_Keys_Right;

	private bool keyboardState_IsKeyDown_Keys_OemComma;

	private bool keyboardState_IsKeyDown_Keys_OemPeriod;

	private float gamePadState_Triggers_Left;

	private float gamePadState_Triggers_Right;

	private bool gamePadState_Buttons_X_ButtonState_Pressed;

	private bool gamePadState_Buttons_Y_ButtonState_Pressed;

	private bool gamePadState_DPad_Left_ButtonState_Pressed;

	private float gamePadState_ThumbSticks_Left_X;

	private bool gamePadState_DPad_Right_ButtonState_Pressed;

	private bool gamePadState_DPad_Up_ButtonState_Pressed;

	private bool gamePadState_DPad_Down_ButtonState_Pressed;

	private bool gamePadState_Buttons_A_ButtonState_Pressed;

	private bool gamePadState_Buttons_A_ButtonState_Released;

	private bool gamePadState_Buttons_B_ButtonState_Pressed;

	private bool gamePadState_Buttons_B_ButtonState_Released;

	private float gamePadState_ThumbSticks_Right_Y;

	private float gamePadState_ThumbSticks_Right_X;

	private float gamePadState_ThumbSticks_Left_Y;

	private bool gamePadState_Buttons_RightShoulder_ButtonState_Pressed;

	private bool gamePadState_Buttons_RightShoulder_ButtonState_Pressed_State;

	private bool gamePadState_Buttons_RightShoulder_ButtonState_Released;

	private bool gamePadState_Buttons_LeftShoulder_ButtonState_Pressed;

	private bool gamePadState_Buttons_LeftShoulder_ButtonState_Released;

	private int GroundPlainHeight = 60;

	public Vector2 cameraTransformOld;

	public bool Active = true;

	public float Scaler = 0.6f;

	public float PhysicsScaleUp = 5f;

	public PlatformerGame MainGame;

	public float ArmScaler = 1f;

	public float LegScaler = 2f;

	public float Player_Species;

	public CollisionCategory _collidesWith = CollisionCategory.All;

	public CollisionCategory _collisionCategory = CollisionCategory.Cat2;

	public short CollisionGroup = 1;

	public bool Already_LimpJoints;

	public short CannonBallCollisionGroup = 120;

	public bool Alive = true;

	public bool Dead;

	public bool ReSpawn;

	public bool Spirit_Walking;

	public double Spirit_Walking_Time_OldGameTime;

	public int Spirit_Walking_Time = 4;

	public bool DeadByBounds;

	public bool ClearedForces;

	public int DeadTimer;

	public int DeadTimerMax = 5000;

	public bool Impailed;

	private bool IsImpailed;

	private Fixture ImpailedBody;

	private Fixture ImpailingBody;

	public bool Unconscious;

	public bool BouncePush;

	public bool ForceFieldPush;

	public bool ForceFieldX;

	public Fixture ForceFixB;

	public int Factor = 10;

	public int Unconscious_Time = 10;

	public bool KnockBack;

	public bool KnockedBack;

	private Fixture MaceFixture;

	public bool DirectionLeft;

	public bool DirectionRight = true;

	public bool IsAlive = true;

	public bool Exiting;

	private bool InPauseMode;

	private bool wasContinuePressed;

	public float mass = 1f;

	public float density = 6.0000002E-05f;

	public float LeftArmRotation;

	public float RightArmRotation;

	public bool Respawn;

	public PlayerIndex playerIndex;

	public bool Player1Index;

	public bool Player2Index;

	public bool Player3Index;

	public bool Player4Index;

	private Color playerColor;

	public int PlayerIndexNew;

	private bool wasJumpPressed;

	public SoundEffect SoundHeadPopOff;

	public SoundEffect SoundGotHit;

	public SoundEffect SoundJump;

	public SoundEffect SoundDeadBodyHit;

	public SoundEffect SoundImpailed;

	public SoundEffect SoundWalking;

	public SoundEffectInstance Step;

	public Color BodyColor = Color.White;

	private Color HandLeftColor = Color.White;

	private Color HandRightColor = Color.White;

	private Color ThighLeftColor = Color.White;

	private Color ThighRightColor = Color.White;

	private Color ColorLeftLeg = Color.White;

	private Color ColorRightLeg = Color.White;

	private Color ColorLeftArm = Color.White;

	private Color ColorRightArm = Color.White;

	private Color ColorLeftHand = Color.White;

	private Color ColorRightHand = Color.White;

	public FixedMouseJoint JumpJoint;

	public Vector2 JumpPoint;

	private bool isJumping;

	private bool wasJumping;

	private int JumpTime;

	private int JumpTimeOld;

	private int JumpClock;

	private int JumpDuration = 1;

	private int JumpDurationMax = 20;

	private bool JumpStart;

	private bool JumpPeak;

	public float MaxJointForce = 60f;

	public float MaxJointForce_Old;

	public int MaxHP = 8;

	public float PlayerHPBody;

	public float PlayerHPBodyMax = 250f;

	public float PlayerHPBody_OLD;

	public bool PlayerHPBody_OLD_Hold_First;

	public float PlayerMana;

	private float PhaseShiftManaCost = 1.5f;

	private float FlyManaCost;

	private float ClimbManaCost = 0.35f;

	public float FreezeBallManaCost = 7.5f;

	public float HealOrbManaCost = 1f;

	public float ManaMax = 250f;

	public double ManaTime;

	public float ManaGainRate = 3f;

	public double HpTime;

	public float HpGainRate = 0.5f;

	public Color ManaColor = Color.White;

	public Color LeftUtilityColor = Color.White;

	public Color RightUtilityColor = Color.White;

	public Texture2D LeftHandUtilityBrush;

	public Texture2D RightHandUtilityBrush;

	public float JointForce;

	public double JustBurntTime;

	public RevoluteJoint NeckJoint;

	public AngleJoint NeckAngleJoint;

	public float NeckJointForce;

	public float NeckAngleJointForce;

	public bool LeftArmSevered;

	public bool RightArmSevered;

	public bool LeftLegSevered;

	public bool RightLegSevered;

	public float UtilityIndexLeft;

	public float UtilityIndexRight;

	public float UtilitySubIndex;

	public float UtilityIndexLeftMax = 2f;

	public float UtilityIndexRightMax = 6f;

	public bool UtilityIndexRightDown;

	public bool UtilityIndexLeftDown;

	public bool DpadUp;

	public bool DpadDown;

	public bool ButtonA;

	public bool ButtonB;

	public bool DpadLeft;

	public bool DpadRight;

	public bool Jump = true;

	public float JumpStrength = 1.5f;

	public bool Crouch;

	public bool Crawl;

	public bool PhaseShift;

	public bool Juggernaut;

	public bool GreenFog;

	public float StandStateIndex;

	public bool BState;

	public bool YState;

	public bool BStateToggle;

	public bool YStateToggle;

	public float CrouchBodyAngle = -0.75f;

	public float CrouchNeckAngle = 0.75f;

	public float CrawlBodyAngle = -1.5f;

	public float CrawlNeckAngle = 1f;

	public Texture2D _SightBrush;

	public Texture2D _TelekinisisBrush;

	private bool Fly_All_Out;

	public bool IsFlying;

	public bool IsClawing;

	public bool IsClimbing;

	public bool BounceHit;

	public Vector2 BounceForce;

	public Vector2 BounceForceScaler;

	private bool Climb_All_Out;

	public Vector2 PlayerPosition;

	public Fixture _headBody;

	private Texture2D _headBrush;

	private float HeadMovement;

	private Vector2 _headBrushOrigin;

	public Texture2D _Weapon_Brush;

	public WeldJoint WeaponJoint;

	public AngleJoint WeaponAngleJoint;

	public bool Weapon_Armed;

	public Vector2 Weapon_Shield_Scaler;

	public float WeaponMaceDamage = 5f;

	public Fixture MaceBashedFixture;

	public bool MaceBashed;

	public Fixture Sensor_Fixture;

	public bool Sensed_Something;

	public Fixture _bodyBody;

	private SliderJoint _bodyJoint;

	private FixedAngleJoint _bodyAngleJoint;

	private AngleJoint _bodyAngleLimitingJoint;

	private Texture2D _bodyBrush;

	private Vector2 _bodyBrushOrigin;

	public float bodyLinearVelocity_X;

	public float bodyLinearVelocity_Y;

	public Vector2 _bodyBodyPosition;

	public float BodyLeanAngle = 0.25f;

	public Fixture _walkerBody;

	private RevoluteJoint _walkerJoint;

	private AngleJoint _walkerAngleJoint;

	private FixedAngleJoint _walkerFixedAngleJoint;

	private Texture2D _walkerBrush;

	private Vector2 _walkerBrushOrigin;

	private float Slap_Damage = 0.5f;

	private Fixture _leftUpperArmBody;

	private RevoluteJoint _leftUpperArmJoint;

	private AngleJoint _leftUpperArmAngleJoint;

	private FixedAngleJoint _leftUpperArmFixedAngleJoint;

	private Texture2D _leftUpperArmBrush;

	private AngleJoint _lefttUpperArmAngleLimitingJoint;

	private Vector2 _leftUpperArmBrushOrigin;

	private Fixture _leftHandBody;

	private Fixture _leftHandBody_Claw;

	private RevoluteJoint _leftHandJoint;

	private AngleJoint _leftHandAngleJoint;

	private FixedAngleJoint _leftHandFixedAngleJoint;

	public bool LeftHandIsTouching;

	private RevoluteJoint _leftHandGrabJoint;

	private Fixture _leftHandGrabOtherFixture;

	private short _leftHandGrabOtherFixture_CollisionGroup;

	private bool _leftHandGrabOtherFixture_IgnoreGravity;

	private float _leftHandGrabOtherFixture_Mass;

	private Vector2 _leftHandGrabOtherFixture_Vector2;

	private BodyType _leftHandGrabOtherFixture_BodyType;

	private Texture2D _leftHandBrush;

	private bool LeftShoulderTriggerState;

	private bool LeftShoulderTriggerStateToggle;

	private bool GrabWithLeftHandBool;

	private Vector2 _leftHandBrushOrigin;

	private Vector2 LeftHandForce;

	private Vector2 ForceScalerLeft;

	private Fixture _leftArmBody;

	private SliderJoint _leftArmJoint;

	private AngleJoint _leftArmAngleJoint;

	private Texture2D _leftArmBrush;

	private Vector2 _leftArmBrushOrigin;

	private Fixture _rightUpperArmBody;

	private RevoluteJoint _rightUpperArmJoint;

	private AngleJoint _rightUpperArmAngleJoint;

	private FixedAngleJoint _rightUpperArmFixedAngleJoint;

	private Texture2D _rightUpperArmBrush;

	private AngleJoint _rightUpperArmAngleLimitingJoint;

	private Vector2 _rightUpperArmBrushOrigin;

	private Fixture _rightHandBody;

	private Fixture _rightHandBody_Claw;

	private RevoluteJoint _rightHandJoint;

	private AngleJoint _rightHandAngleJoint;

	private FixedAngleJoint _rightHandFixedAngleJoint;

	public bool RightHandIsTouching;

	private WeldJoint _rightHandGrabJoint;

	private Fixture _rightHandGrabOtherFixture;

	private short _rightHandGrabOtherFixture_CollisionGroup;

	private bool _rightHandGrabOtherFixture_IgnoreGravity;

	private float _rightHandGrabOtherFixture_Mass;

	private Vector2 _rightHandGrabOtherFixture_Vector2;

	private BodyType _rightHandGrabOtherFixture_BodyType;

	private Texture2D _rightHandBrush;

	private bool RightShoulderTriggerState;

	private bool RightShoulderTriggerStateToggle;

	private bool GrabWithRightHandBool;

	private RevoluteJoint Grab_Joint;

	private bool Grab_Joint_ON;

	private Vector2 _rightHandBrushOrigin;

	private Vector2 RightHandForce;

	private Vector2 ForceScalerRight;

	public bool _SightON;

	private bool _bodyBodyGone;

	private bool _leftUpperArmBodyGone;

	private bool _rightUpperArmBodyGone;

	private bool _leftThighBodyGone;

	private bool _rightThighBodyGone;

	private bool _leftHandBodyGone;

	private bool _rightHandBodyGone;

	private Fixture _rightArmBody;

	private SliderJoint _rightArmJoint;

	private AngleJoint _rightArmAngleJoint;

	private Texture2D _rightArmBrush;

	private Vector2 _rightArmBrushOrigin;

	private Fixture _leftThighBody;

	private RevoluteJoint _leftThighJoint;

	private SliderJoint _leftThighSliderJoint;

	public bool LeftFootIsOnGround;

	private AngleJoint _leftThighAngleJoint;

	private Texture2D _leftThighBrush;

	private Vector2 _leftThighBrushOrigin;

	public float _leftThighJointForce;

	public bool _leftThighJointRemoved;

	public float _leftThighAngleJointTargetAngle;

	public Fixture _leftThighBodyPivotBody;

	public RevoluteJoint _leftThighPivotJoint;

	private Fixture _rightThighBody;

	private RevoluteJoint _rightThighJoint;

	private SliderJoint _rightThighSliderJoint;

	public bool RightFootIsOnGround;

	private AngleJoint _rightThighAngleJoint;

	private Texture2D _rightThighBrush;

	private Vector2 _rightThighBrushOrigin;

	public float _rightThighJointForce;

	public bool _rightThighJointRemoved;

	public float _rightThighAngleJointTargetAngle;

	public Fixture _rightThighBodyPivotBody;

	public RevoluteJoint _rightThighPivotJoint;

	private Fixture _leftCalfBody;

	private RevoluteJoint _leftCalfJoint;

	private Texture2D _leftCalfBrush;

	private Vector2 _leftCalfBrushOrigin;

	private RevoluteJoint _leftCalfToWalkerJoint;

	private AngleJoint _leftCalfToThighAngleJoint;

	private Fixture _rightCalfBody;

	private RevoluteJoint _rightCalfJoint;

	private Texture2D _rightCalfBrush;

	private Vector2 _rightCalfBrushOrigin;

	private RevoluteJoint _rightCalfToWalkerJoint;

	private AngleJoint _rightCalfToThighAngleJoint;

	private Fixture _leftFootBody;

	private RevoluteJoint _leftFootJoint;

	private AngleJoint _leftFootAngleJoint;

	private Texture2D _leftFootBrush;

	private Vector2 _leftFootBrushOrigin;

	private Fixture _rightFootBody;

	private RevoluteJoint _rightFootJoint;

	private AngleJoint _rightFootAngleJoint;

	private Texture2D _rightFootBrush;

	private Vector2 _rightFootBrushOrigin;

	private SliderJoint _legSliderJoint;

	public Vector2 _position;

	public Vector2 _PhyPosition;

	private Vector2 _GetPosition;

	private Vector2 PhysicsPosition;

	private int _radius = 100;

	public Color Color;

	public Vector2 PositionOld;

	private Vector2 origin;

	private Vector2 basePosition;

	private float TelekinesisManaCost = 0.75f;

	private int TelekinesisRangeScaler = -2000;

	private int GrabRangeScaler = -10;

	public Fixture TelekinisisBodyHit;

	public Vector2 TelekinisisBodyHit_Point;

	public bool Telekinisis_Try;

	public bool TelekinesisHitSomthing;

	public bool TeleFirstHit;

	private float TelekinisisBodyHitMassOLD;

	private float TelekinisisBodyHitCollisionGroupOLD;

	public float movement;

	public float movementX;

	public Vector2 PlayerMovement;

	public float RunLimit;

	public bool WasSprinting;

	public bool XLock;

	public float movementY;

	private Vector2 velocity;

	public readonly string ParticleEffecstDir = "/Effects/Particle/";

	public ParticleEffect HealEffect;

	public ParticleEffect particleEffectKineticShield;

	public ParticleEffect particleEffectTeleFog;

	public double OldGameTime;

	public double OldUnconsciousTime;

	public double BloodGameTime;

	public ParticleEffect particleEffectBleed;

	public ParticleEffect particleEffectBleeding;

	public ParticleEffect particleEffectUnconcious;

	public ParticleEffect particleEffectBloodSquirting;

	public ParticleEffect particleEffectSpirit;

	public int BleedingTimer;

	public int BleedingTimerMax = 2000;

	private bool WasDustyLeft;

	private bool WasDustyRight;

	public SpriteBatchRenderer renderer;

	public float GrabThrowForceScaler = 10000000f;

	public float GrabStrangleDamage = 0.005f;

	public Fixture[] _CannonBall;

	public Fixture CannonBall;

	public double[] _CannonBallBulletTimer;

	public int CannonBallWaitTime = 300;

	private int CannonBallIndex;

	private Vector2 _CannonBallOrigin;

	private Texture2D _CannonBallTexture;

	public Texture2D _CannonBallSkullTexture;

	public float CannonBallManaCost = 75f;

	public float CannonBallForceScaler = 1E+12f;

	private SoundEffect CannonBallSound;

	private SoundEffect LightningBallSound;

	public Color CannonBallColor;

	public bool[] CannonBallGo;

	public int[] _CannonBallZone_Wait_One;

	private ParticleEffect particleEffectCannonBallEx;

	public Fixture[] _CannonBallZone;

	public bool[] CannonBallDraw;

	public int FireDamage = 10;

	private int FireSpeedX = 75;

	private int FreezeSpeedX = 75;

	public int Oscar_Update_Speed = 1;

	public Fixture[] _IceBall;

	public double[] _IceBallBulletTimer;

	private int IceBallIndex;

	private Vector2 _IceBallOrigin;

	private Texture2D _IceBallTexture;

	public float IceBallForceScaler = 1E+10f;

	public float IceBallManaCost = 7.5f;

	private SoundEffect IceBallSound;

	public Color IceBallColor;

	public bool Freezer;

	public bool Frozen;

	public bool Shocker;

	public bool Shocked;

	public bool Smoking;

	private WeldJoint FreezeJoint1;

	private WeldJoint FreezeJoint2;

	private WeldJoint FreezeJoint3;

	private WeldJoint FreezeJoint4;

	private WeldJoint FreezeJoint5;

	private WeldJoint FreezeJoint6;

	private WeldJoint FreezeJoint7;

	private int DartVibrationDuration = 100;

	private float DartVibrationSpeed = 0.5f;

	private float DartBoneVibrationSpeed = 0.1f;

	private float DartHarpoonVibrationSpeed = 0.1f;

	private float DartEctoBallVibrationSpeed = 0.1f;

	private float DartBallLightningVibrationSpeed = 0.1f;

	private int DartBallLightningVibrationDuration = 500;

	private float DartMaceVibrationSpeed = 0.7f;

	private float DartRockVibrationSpeed = 0.7f;

	private float DartBurrVibrationSpeed = 0.6f;

	private float DartClawVibrationSpeed = 0.9f;

	private float DartTeleVibrationSpeed = 0.6f;

	private float DartGrabVibrationSpeed = 0.4f;

	private int HurtVibrationDuration = 75;

	private float HurtEctoBallVibrationSpeed = 0.9f;

	private float HurtBallLightningVibrationSpeed = 0.6f;

	private int HurtBallLightningVibrationDuration = 500;

	private float HurtMaceVibrationSpeed = 0.4f;

	private float HurtRockVibrationSpeed = 0.4f;

	private float HurtSpikeVibrationSpeed = 0.7f;

	private float HurtBladeVibrationSpeed = 0.8f;

	private float HurtImpailedVibrationSpeed = 0.6f;

	private int HurtImpailedVibrationDuration = 600;

	private float HurtBurrVibrationSpeed = 0.7f;

	private float HurtClawVibrationSpeed = 0.9f;

	private float HurtTeleVibrationSpeed = 0.9f;

	private float HurtGrabVibrationSpeed = 0.4f;

	private float HurtBoneVibrationSpeed = 0.5f;

	public bool RemovedDarts;

	public Fixture[] _DartBone;

	public Fixture[] _DartBone_Tail;

	public double[] _DartBoneBulletTimer;

	private int DartBoneIndex;

	private Vector2 _DartBoneOrigin;

	private Texture2D _DartBoneTexture;

	public float DartBoneForceScaler = 1E+12f;

	public float DartBoneManaCost = 2.5f;

	private SoundEffect DartBoneSound;

	public Color DartBoneColor = Color.White;

	public Texture2D[] DartBoneDecalTexture;

	public float DartBoneDamage = 23f;

	private float BoneDartRepeater;

	private float BoneDartRepeaterMax = 6f;

	private float RockDamage = 20f;

	private bool RockSkin_ON;

	public Fixture[] _DartHarpoon;

	public double[] _DartHarpoonBulletTimer;

	private int DartHarpoonIndex;

	private Vector2 _DartHarpoonOrigin;

	private Texture2D _DartHarpoonTexture;

	public float DartHarpoonForceScaler = 1E+12f;

	public float DartHarpoonManaCost = 20f;

	private SoundEffect DartHarpoonSound;

	public Color DartHarpoonColor = Color.White;

	public Texture2D[] DartHarpoonDecalTexture;

	public float DartHarpoonDamage = 50f;

	private float HarpoonDartRepeater;

	private float HarpoonDartRepeaterMax = 100f;

	public Fixture[] _DartKinetic;

	public Fixture[] _DartKineticZone;

	public double[] _DartKineticBulletTimer;

	private int DartKineticIndex;

	private Vector2 _DartKineticOrigin;

	private Texture2D _DartKineticTexture;

	public float DartKineticForceScaler = 1E+13f;

	public float DartKineticManaCost = 10.5f;

	private SoundEffect DartKineticSound;

	public Color DartKineticColor = Color.White;

	public Texture2D[] DartKineticDecalTexture;

	public float DartKineticDamage = 66f;

	public bool[] KineticGo;

	public Fixture DartKineticDart;

	public bool[] KineticDraw;

	public ParticleEffect particleEffectKineticEx;

	public Fixture _kineticShields;

	public float KineticShieldManaCost = 0.1f;

	public Fixture[] _DartStasis;

	public Fixture[] _DartStasisZone;

	public double[] _DartStasisBulletTimer;

	private int DartStasisIndex;

	private Vector2 _DartStasisOrigin;

	private Texture2D _DartStasisTexture;

	public float DartStasisForceScaler = 1E+13f;

	public float DartStasisManaCost = 10f;

	private SoundEffect DartStasisSound;

	public Color DartStasisColor = Color.White;

	public Texture2D[] DartStasisDecalTexture;

	public float DartStasisDamage = 66f;

	public bool[] StasisGo;

	public Fixture DartStasisDart;

	public bool[] StasisDraw;

	public bool[] BurrGo;

	public ParticleEffect particleEffectStasisEx;

	private float SharpsNeddleDamage = 5f;

	private float RatDamage = 0.1f;

	private RenderTarget2D _bodyDecalRenderer;

	private bool WeldDecal;

	private Fixture FixA;

	private Fixture[] FixA_Burr;

	private Fixture FixB;

	private Level level;

	private Vector2 position;

	public Vector2 Velocity
	{
		get
		{
			return velocity;
		}
		set
		{
			velocity = value;
		}
	}

	public Level Level => level;

	public Vector2 Position
	{
		get
		{
			return position;
		}
		set
		{
			position = value;
		}
	}

	public Player1(Level Mainlevel, PlatformerGame mainGame, Vector2 position, World _world, int i, float spriteSet, Color MyColor)
	{
		_position = position;
		_PhyPosition = new Vector2(position.X * 0.2f, position.Y * 0.2f);
		PlayerHPBody = PlayerHPBodyMax;
		PlayerMana = ManaMax;
		MainGame = mainGame;
		Scaler = 0.6f;
		LegScaler = 0.75f;
		TelekinisisBodyHit_Point = new Vector2(0f, 0f);
		level = Mainlevel;
		PlayerIndexNew = i;
		BounceForce = new Vector2(0f, 0f);
		BounceForceScaler = new Vector2(0f, 0f);
		Weapon_Shield_Scaler = new Vector2(0.6f, 0.6f);
		SoundHeadPopOff = level.SoundHeadPopOff;
		SoundJump = level.SoundJump;
		SoundWalking = level.SoundWalking;
		SoundGotHit = level.SoundGotHit;
		SoundDeadBodyHit = level.SoundDeadBodyHit;
		SoundImpailed = level.SoundImpailed;
		_Weapon_Brush = level._Weapon_Brush;
		if (i == 1)
		{
			particleEffectKineticEx = level.particleEffectKineticEx;
			particleEffectStasisEx = level.particleEffectStasisEx;
			LeftUtilityColor = Color.AliceBlue;
			LeftHandUtilityBrush = level.P1LeftHandUtilityBrush;
			RightUtilityColor = Color.AliceBlue;
			RightHandUtilityBrush = level.P1RightHandUtilityBrush;
			Player_Species = mainGame.Player1Species;
			particleEffectKineticShield = level.particleEffectKineticShield.DeepCopy();
			particleEffectBleed = level.particleEffectBleed.DeepCopy();
			particleEffectBleeding = level.particleEffectBleeding.DeepCopy();
			particleEffectBloodSquirting = level.particleEffectBloodSquirting.DeepCopy();
			particleEffectSpirit = level.particleEffectSpirit.DeepCopy();
			HealEffect = level.HealEffect.DeepCopy();
			particleEffectUnconcious = level.particleEffectUnconcious.DeepCopy();
			particleEffectTeleFog = level.particleEffectTeleFog.DeepCopy();
			renderer = new SpriteBatchRenderer
			{
				GraphicsDeviceService = level.mainGame.graphics
			};
			particleEffectCannonBallEx = level.particleEffectCannonBallEx;
			HealEffect.Initialise();
			HealEffect.LoadContent(level.Content);
			particleEffectKineticShield.Initialise();
			particleEffectKineticShield.LoadContent(level.Content);
			particleEffectKineticEx.Initialise();
			particleEffectKineticEx.LoadContent(level.Content);
			particleEffectCannonBallEx.Initialise();
			particleEffectCannonBallEx.LoadContent(level.Content);
			particleEffectTeleFog.Initialise();
			particleEffectTeleFog.LoadContent(level.Content);
			particleEffectStasisEx.Initialise();
			particleEffectStasisEx.LoadContent(level.Content);
			particleEffectBleed.Initialise();
			particleEffectBleed.LoadContent(level.Content);
			particleEffectBleeding.Initialise();
			particleEffectBleeding.LoadContent(level.Content);
			particleEffectBloodSquirting.Initialise();
			particleEffectBloodSquirting.LoadContent(level.Content);
			particleEffectSpirit.Initialise();
			particleEffectSpirit.LoadContent(level.Content);
			particleEffectUnconcious.Initialise();
			particleEffectUnconcious.LoadContent(level.Content);
			renderer.LoadContent(level.Content);
			_SightBrush = level.P1_SightBrush;
			_TelekinisisBrush = Level.P1_TelekinisisBrush;
			DartBoneSound = level.P1DartBoneSound;
			DartHarpoonSound = level.P1DartHarpoonSound;
			CannonBallSound = level.P1CannonBallSound;
			LightningBallSound = level.P1LightningBallSound;
			_bodyBrush = level.P1_bodyBrush;
			_headBrush = level.P1_headBrush;
			_leftUpperArmBrush = level.P1_leftUpperArmBrush;
			_rightUpperArmBrush = level.P1_rightUpperArmBrush;
			_leftHandBrush = level.P1_leftHandBrush;
			_rightHandBrush = level.P1_rightHandBrush;
			_leftThighBrush = level.P1_leftThighBrush;
			_rightThighBrush = level.P1_rightThighBrush;
		}
		if (mainGame.Player2InGame && i == 2)
		{
			particleEffectKineticEx = level.particleEffectKineticEx;
			particleEffectStasisEx = level.particleEffectStasisEx;
			LeftUtilityColor = Color.AliceBlue;
			LeftHandUtilityBrush = level.P2LeftHandUtilityBrush;
			RightUtilityColor = Color.AliceBlue;
			RightHandUtilityBrush = level.P2RightHandUtilityBrush;
			Player_Species = mainGame.Player2Species;
			particleEffectKineticShield = level.particleEffectKineticShield;
			particleEffectBleed = level.particleEffectBleed.DeepCopy();
			particleEffectBleeding = level.particleEffectBleeding.DeepCopy();
			particleEffectBloodSquirting = level.particleEffectBloodSquirting.DeepCopy();
			particleEffectSpirit = level.particleEffectSpirit.DeepCopy();
			HealEffect = level.HealEffect;
			particleEffectUnconcious = level.particleEffectUnconcious;
			particleEffectTeleFog = level.particleEffectTeleFog;
			renderer = new SpriteBatchRenderer
			{
				GraphicsDeviceService = level.mainGame.graphics
			};
			particleEffectCannonBallEx = level.particleEffectCannonBallEx;
			HealEffect.Initialise();
			HealEffect.LoadContent(level.Content);
			particleEffectKineticShield.Initialise();
			particleEffectKineticShield.LoadContent(level.Content);
			particleEffectKineticEx.Initialise();
			particleEffectKineticEx.LoadContent(level.Content);
			particleEffectCannonBallEx.Initialise();
			particleEffectCannonBallEx.LoadContent(level.Content);
			particleEffectTeleFog.Initialise();
			particleEffectTeleFog.LoadContent(level.Content);
			particleEffectStasisEx.Initialise();
			particleEffectStasisEx.LoadContent(level.Content);
			particleEffectBleed.Initialise();
			particleEffectBleed.LoadContent(level.Content);
			particleEffectBleeding.Initialise();
			particleEffectBleeding.LoadContent(level.Content);
			particleEffectBloodSquirting.Initialise();
			particleEffectBloodSquirting.LoadContent(level.Content);
			particleEffectSpirit.Initialise();
			particleEffectSpirit.LoadContent(level.Content);
			particleEffectUnconcious.Initialise();
			particleEffectUnconcious.LoadContent(level.Content);
			renderer.LoadContent(level.Content);
			_SightBrush = level.P2_SightBrush;
			_TelekinisisBrush = Level.P2_TelekinisisBrush;
			DartBoneSound = level.P2DartBoneSound;
			DartHarpoonSound = level.P2DartHarpoonSound;
			CannonBallSound = level.P2CannonBallSound;
			LightningBallSound = level.P2LightningBallSound;
			_bodyBrush = level.P2_bodyBrush;
			_headBrush = level.P2_headBrush;
			_leftUpperArmBrush = level.P2_leftUpperArmBrush;
			_rightUpperArmBrush = level.P2_rightUpperArmBrush;
			_leftHandBrush = level.P2_leftHandBrush;
			_rightHandBrush = level.P2_rightHandBrush;
			_leftThighBrush = level.P2_leftThighBrush;
			_rightThighBrush = level.P2_rightThighBrush;
		}
		if (mainGame.Player3InGame && i == 3)
		{
			particleEffectKineticEx = level.particleEffectKineticEx;
			particleEffectStasisEx = level.particleEffectStasisEx;
			LeftUtilityColor = Color.AliceBlue;
			LeftHandUtilityBrush = level.P3LeftHandUtilityBrush;
			RightUtilityColor = Color.AliceBlue;
			RightHandUtilityBrush = level.P3RightHandUtilityBrush;
			Player_Species = mainGame.Player3Species;
			particleEffectKineticShield = level.particleEffectKineticShield;
			particleEffectBleed = level.particleEffectBleed.DeepCopy();
			particleEffectBleeding = level.particleEffectBleeding.DeepCopy();
			particleEffectBloodSquirting = level.particleEffectBloodSquirting.DeepCopy();
			particleEffectSpirit = level.particleEffectSpirit.DeepCopy();
			HealEffect = level.HealEffect;
			particleEffectUnconcious = level.particleEffectUnconcious;
			particleEffectTeleFog = level.particleEffectTeleFog;
			renderer = new SpriteBatchRenderer
			{
				GraphicsDeviceService = level.mainGame.graphics
			};
			particleEffectCannonBallEx = level.particleEffectCannonBallEx;
			HealEffect.Initialise();
			HealEffect.LoadContent(level.Content);
			particleEffectKineticShield.Initialise();
			particleEffectKineticShield.LoadContent(level.Content);
			particleEffectKineticEx.Initialise();
			particleEffectKineticEx.LoadContent(level.Content);
			particleEffectCannonBallEx.Initialise();
			particleEffectCannonBallEx.LoadContent(level.Content);
			particleEffectTeleFog.Initialise();
			particleEffectTeleFog.LoadContent(level.Content);
			particleEffectStasisEx.Initialise();
			particleEffectStasisEx.LoadContent(level.Content);
			particleEffectBleed.Initialise();
			particleEffectBleed.LoadContent(level.Content);
			particleEffectBleeding.Initialise();
			particleEffectBleeding.LoadContent(level.Content);
			particleEffectBloodSquirting.Initialise();
			particleEffectBloodSquirting.LoadContent(level.Content);
			particleEffectSpirit.Initialise();
			particleEffectSpirit.LoadContent(level.Content);
			particleEffectUnconcious.Initialise();
			particleEffectUnconcious.LoadContent(level.Content);
			renderer.LoadContent(level.Content);
			_SightBrush = level.P3_SightBrush;
			_TelekinisisBrush = Level.P3_TelekinisisBrush;
			DartBoneSound = level.P3DartBoneSound;
			DartHarpoonSound = level.P3DartHarpoonSound;
			CannonBallSound = level.P3CannonBallSound;
			LightningBallSound = level.P3LightningBallSound;
			_bodyBrush = level.P3_bodyBrush;
			_headBrush = level.P3_headBrush;
			_leftUpperArmBrush = level.P3_leftUpperArmBrush;
			_rightUpperArmBrush = level.P3_rightUpperArmBrush;
			_leftHandBrush = level.P3_leftHandBrush;
			_rightHandBrush = level.P3_rightHandBrush;
			_leftThighBrush = level.P3_leftThighBrush;
			_rightThighBrush = level.P3_rightThighBrush;
		}
		if (mainGame.Player4InGame && i == 4)
		{
			particleEffectKineticEx = level.particleEffectKineticEx;
			particleEffectStasisEx = level.particleEffectStasisEx;
			LeftUtilityColor = Color.AliceBlue;
			LeftHandUtilityBrush = level.P4LeftHandUtilityBrush;
			RightUtilityColor = Color.AliceBlue;
			RightHandUtilityBrush = level.P4RightHandUtilityBrush;
			Player_Species = mainGame.Player4Species;
			particleEffectKineticShield = level.particleEffectKineticShield;
			particleEffectBleed = level.particleEffectBleed.DeepCopy();
			particleEffectBleeding = level.particleEffectBleeding.DeepCopy();
			particleEffectBloodSquirting = level.particleEffectBloodSquirting.DeepCopy();
			particleEffectSpirit = level.particleEffectSpirit.DeepCopy();
			HealEffect = level.HealEffect;
			particleEffectUnconcious = level.particleEffectUnconcious;
			particleEffectTeleFog = level.particleEffectTeleFog;
			renderer = new SpriteBatchRenderer
			{
				GraphicsDeviceService = level.mainGame.graphics
			};
			particleEffectCannonBallEx = level.particleEffectCannonBallEx;
			HealEffect.Initialise();
			HealEffect.LoadContent(level.Content);
			particleEffectKineticShield.Initialise();
			particleEffectKineticShield.LoadContent(level.Content);
			particleEffectKineticEx.Initialise();
			particleEffectKineticEx.LoadContent(level.Content);
			particleEffectCannonBallEx.Initialise();
			particleEffectCannonBallEx.LoadContent(level.Content);
			particleEffectTeleFog.Initialise();
			particleEffectTeleFog.LoadContent(level.Content);
			particleEffectStasisEx.Initialise();
			particleEffectStasisEx.LoadContent(level.Content);
			particleEffectBleed.Initialise();
			particleEffectBleed.LoadContent(level.Content);
			particleEffectBleeding.Initialise();
			particleEffectBleeding.LoadContent(level.Content);
			particleEffectBloodSquirting.Initialise();
			particleEffectBloodSquirting.LoadContent(level.Content);
			particleEffectSpirit.Initialise();
			particleEffectSpirit.LoadContent(level.Content);
			particleEffectUnconcious.Initialise();
			particleEffectUnconcious.LoadContent(level.Content);
			renderer.LoadContent(level.Content);
			_SightBrush = level.P4_SightBrush;
			_TelekinisisBrush = Level.P4_TelekinisisBrush;
			DartBoneSound = level.P4DartBoneSound;
			DartHarpoonSound = level.P4DartHarpoonSound;
			CannonBallSound = level.P4CannonBallSound;
			LightningBallSound = level.P4LightningBallSound;
			_bodyBrush = level.P4_bodyBrush;
			_headBrush = level.P4_headBrush;
			_leftUpperArmBrush = level.P4_leftUpperArmBrush;
			_rightUpperArmBrush = level.P4_rightUpperArmBrush;
			_leftHandBrush = level.P4_leftHandBrush;
			_rightHandBrush = level.P4_rightHandBrush;
			_leftThighBrush = level.P4_leftThighBrush;
			_rightThighBrush = level.P4_rightThighBrush;
		}
		UtilityIndexLeft = 0f;
		UtilityIndexRight = 0f;
		UtilitySubIndex = 0f;
		for (int j = 0; j < MaxHP; j++)
		{
		}
		if (i > 0 && i < 2)
		{
			CannonBallColor = Color.White;
			IceBallColor = Color.DarkGray;
			playerColor = Color.Black;
			playerIndex = PlayerIndex.One;
			Player1Index = true;
			if (Level.FriendlyFireToggle)
			{
				CollisionGroup = 1;
				CannonBallCollisionGroup = 101;
			}
			else
			{
				CollisionGroup = 1;
				CannonBallCollisionGroup = 101;
			}
		}
		if (i > 1 && i < 3)
		{
			CannonBallColor = Color.White;
			IceBallColor = Color.LightSalmon;
			playerColor = Color.Red;
			playerIndex = PlayerIndex.Two;
			Player2Index = true;
			if (Level.FriendlyFireToggle)
			{
				CollisionGroup = 2;
				CannonBallCollisionGroup = 102;
			}
			else
			{
				CollisionGroup = 1;
				CannonBallCollisionGroup = 101;
			}
		}
		if (i > 2 && i < 4)
		{
			CannonBallColor = Color.White;
			IceBallColor = Color.LightGreen;
			playerColor = Color.Green;
			playerIndex = PlayerIndex.Three;
			Player3Index = true;
			if (Level.FriendlyFireToggle)
			{
				CollisionGroup = 3;
				CannonBallCollisionGroup = 1031;
			}
			else
			{
				CollisionGroup = 1;
				CannonBallCollisionGroup = 101;
			}
		}
		if (i > 3 && i < 5)
		{
			CannonBallColor = Color.White;
			IceBallColor = Color.LightYellow;
			playerColor = Color.Yellow;
			playerIndex = PlayerIndex.Four;
			Player4Index = true;
			if (Level.FriendlyFireToggle)
			{
				CollisionGroup = 4;
				CannonBallCollisionGroup = 104;
			}
			else
			{
				CollisionGroup = 1;
				CannonBallCollisionGroup = 101;
			}
		}
		PlayerMovement = new Vector2(0f, 0f);
		Color = MyColor;
		basePosition = position;
		if (Player_Species == 0f)
		{
			Load_Daru(level, _world, i, spriteSet);
		}
		else if (Player_Species == 4f)
		{
			Load_Ernest(level, _world, i, spriteSet);
		}
		else if (Player_Species == 1f)
		{
			Load_Oscar(level, _world, i, spriteSet);
		}
		else if (Player_Species == 2f)
		{
			Load_Rick(level, _world, i, spriteSet);
		}
		else if (Player_Species == 3f)
		{
			Load_Vinny(level, _world, i, spriteSet);
		}
		_bodyBodyPosition = _bodyBody.Body.Position;
	}

	public void Load_Daru(Level level, World _world, int i, float Species)
	{
		_Weapon_Brush = level.Content.Load<Texture2D>("Sprites/0/leftArm");
		PlayerHPBodyMax = 150f;
		PlayerHPBody = 150f;
		HpGainRate *= 2f;
		ManaGainRate = 2f;
		JumpStrength = 2.5f;
		MaxJointForce = 60f;
		Weapon_Shield_Scaler = new Vector2(0.6f, 2.4f);
		KineticShieldManaCost = 0.5f;
		_DartBone = new Fixture[10000];
		_DartBoneBulletTimer = new double[10000];
		_DartHarpoon = new Fixture[10000];
		_DartHarpoonBulletTimer = new double[10000];
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 1000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.UserData = 1000 + i;
		_bodyBody.Body.AngularDamping = 1000f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 1000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.AngularDamping = 1000f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_head));
		Fixture bodyBody3 = _bodyBody;
		bodyBody3.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody3.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_body));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 1000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1000f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 1000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1000f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 1000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 1000f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.4f, 0.8f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 1000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 1000f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 1000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.AngularDamping = 1000f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 1000f;
		_rightThighBody.Body.AngularDamping = 1000f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_Daru_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Daru_rightThigh));
		_kineticShields = FixtureFactory.CreateRectangle(_world, 2.4f, 90f, 1E-06f);
		_kineticShields.Body.Position = _rightHandBody.Body.Position;
		_kineticShields.Body.BodyType = BodyType.Kinematic;
		_kineticShields.Body.IsBullet = true;
		_kineticShields.Friction = 0f;
		_kineticShields.Restitution = 1f;
		_kineticShields.Body.UserData = 10;
		_kineticShields.Body.AngularDamping = 1f;
		_kineticShields.CollisionGroup = CollisionGroup;
		_kineticShields.CollidesWith = CollisionCategory.None;
		Fixture kineticShields = _kineticShields;
		kineticShields.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticShields.OnCollision, new CollisionEventHandler(Shield_OnCollision_Daru));
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
		_bodyAngleJoint.UserData = 1;
		_leftUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
		_leftUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _leftUpperArmBody.Body);
		_rightUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
		_rightUpperArmJoint.MaxMotorTorque = 50000f;
		_rightUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _rightUpperArmBody.Body);
		_leftHandJoint = new RevoluteJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_rightHandJoint = new RevoluteJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_leftThighJoint = new RevoluteJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
		_leftThighJoint.MotorEnabled = true;
		_leftThighJoint.MaxMotorTorque = 100000000f;
		_leftThighJoint.MotorSpeed = 0f;
		_rightThighJoint = new RevoluteJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
		_rightThighJoint.MotorEnabled = true;
		_rightThighJoint.MaxMotorTorque = 100000000f;
		_rightThighJoint.MotorSpeed = 0f;
		_world.AddJoint(NeckJoint);
		_world.AddJoint(NeckAngleJoint);
		_world.AddJoint(_bodyAngleJoint);
		_world.AddJoint(_leftUpperArmJoint);
		_world.AddJoint(_rightUpperArmJoint);
		_world.AddJoint(_leftHandJoint);
		_world.AddJoint(_rightHandJoint);
		_world.AddJoint(_leftThighJoint);
		_world.AddJoint(_rightThighJoint);
	}

	public void Load_Ernest(Level level, World _world, int i, float Species)
	{
		PlayerHPBodyMax = 200f;
		PlayerHPBody = 200f;
		DartBoneManaCost = 10f;
		HpGainRate *= 1.5f;
		ManaGainRate = 1f;
		DartBoneDamage = 1f;
		MaxJointForce = 75f;
		JumpStrength = 2.75f;
		_DartBone = new Fixture[10000];
		_DartBone_Tail = new Fixture[10000];
		_DartBoneBulletTimer = new double[10000];
		BurrGo = new bool[10000];
		FixA_Burr = new Fixture[10000];
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 1000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.UserData = 1000 + i;
		_bodyBody.Body.AngularDamping = 10f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 1000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.AngularDamping = 10f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_head));
		Fixture headBody2 = _headBody;
		headBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(headBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_head));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 1000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 10f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 1000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 10f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 1000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 10f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.4f, 0.8f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 1000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 10f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 1000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.AngularDamping = 10f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 1000f;
		_rightThighBody.Body.AngularDamping = 10f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_Ernest_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Ernest_rightThigh));
		_kineticShields = FixtureFactory.CreateRectangle(_world, 0.6f, 4.2000003f, 1E-06f);
		_kineticShields.Body.Position = _rightHandBody.Body.Position;
		_kineticShields.Body.BodyType = BodyType.Dynamic;
		_kineticShields.Body.IsBullet = true;
		_kineticShields.Friction = 1f;
		_kineticShields.Restitution = 0f;
		_kineticShields.Body.UserData = 233;
		_kineticShields.Body.AngularDamping = 10f;
		_kineticShields.CollisionGroup = CollisionGroup;
		_kineticShields.CollidesWith = CollisionCategory.None;
		Fixture kineticShields = _kineticShields;
		kineticShields.OnCollision = (CollisionEventHandler)Delegate.Combine(kineticShields.OnCollision, new CollisionEventHandler(Weapon_OnCollision_Mace_Ernest));
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
		_bodyAngleJoint.UserData = 1;
		_leftUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
		_leftUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _leftUpperArmBody.Body);
		_rightUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
		_rightUpperArmJoint.MaxMotorTorque = 50000f;
		_rightUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _rightUpperArmBody.Body);
		_leftHandJoint = new RevoluteJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_rightHandJoint = new RevoluteJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_leftThighJoint = new RevoluteJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
		_leftThighJoint.MotorEnabled = true;
		_leftThighJoint.MaxMotorTorque = 100000000f;
		_leftThighJoint.MotorSpeed = 0f;
		_rightThighJoint = new RevoluteJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
		_rightThighJoint.MotorEnabled = true;
		_rightThighJoint.MaxMotorTorque = 100000000f;
		_rightThighJoint.MotorSpeed = 0f;
		_world.AddJoint(NeckJoint);
		_world.AddJoint(NeckAngleJoint);
		_world.AddJoint(_bodyAngleJoint);
		_world.AddJoint(_leftUpperArmJoint);
		_world.AddJoint(_rightUpperArmJoint);
		_world.AddJoint(_leftHandJoint);
		_world.AddJoint(_rightHandJoint);
		_world.AddJoint(_leftThighJoint);
		_world.AddJoint(_rightThighJoint);
	}

	public void Load_Oscar(Level level, World _world, int i, float Species)
	{
		PlayerHPBodyMax = 100f;
		PlayerHPBody = 100f;
		MaxJointForce = 60f;
		HpGainRate *= 2.5f;
		ManaGainRate = 1.5f;
		JumpStrength = 2.5f;
		_CannonBall = new Fixture[10000];
		_CannonBallZone = new Fixture[10000];
		_CannonBallBulletTimer = new double[10000];
		CannonBallDraw = new bool[10000];
		CannonBallGo = new bool[10000];
		_CannonBallZone_Wait_One = new int[10000];
		_DartKinetic = new Fixture[10000];
		_DartKineticZone = new Fixture[10000];
		_DartKineticBulletTimer = new double[10000];
		KineticGo = new bool[10000];
		KineticDraw = new bool[10000];
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 1000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.UserData = 1000 + i;
		_bodyBody.Body.AngularDamping = 1000f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 1000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.AngularDamping = 1000f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_head));
		Fixture bodyBody3 = _bodyBody;
		bodyBody3.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody3.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_body));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 1000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1000f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 1000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1000f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 1000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 1000f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.4f, 0.8f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 1000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 1000f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 1000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.AngularDamping = 1000f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 1000f;
		_rightThighBody.Body.AngularDamping = 1000f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_Oscar_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Oscar_rightThigh));
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
		_bodyAngleJoint.UserData = 1;
		_leftUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
		_leftUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _leftUpperArmBody.Body);
		_rightUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
		_rightUpperArmJoint.MaxMotorTorque = 50000f;
		_rightUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _rightUpperArmBody.Body);
		_leftHandJoint = new RevoluteJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_rightHandJoint = new RevoluteJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_leftThighJoint = new RevoluteJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
		_leftThighJoint.MotorEnabled = true;
		_leftThighJoint.MaxMotorTorque = 100000000f;
		_leftThighJoint.MotorSpeed = 0f;
		_rightThighJoint = new RevoluteJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
		_rightThighJoint.MotorEnabled = true;
		_rightThighJoint.MaxMotorTorque = 100000000f;
		_rightThighJoint.MotorSpeed = 0f;
		_world.AddJoint(NeckJoint);
		_world.AddJoint(NeckAngleJoint);
		_world.AddJoint(_bodyAngleJoint);
		_world.AddJoint(_leftUpperArmJoint);
		_world.AddJoint(_rightUpperArmJoint);
		_world.AddJoint(_leftHandJoint);
		_world.AddJoint(_rightHandJoint);
		_world.AddJoint(_leftThighJoint);
		_world.AddJoint(_rightThighJoint);
	}

	public void Load_Rick(Level level, World _world, int i, float Species)
	{
		PlayerHPBodyMax = 250f;
		PlayerHPBody = 200f;
		MaxJointForce = 150f;
		HpGainRate *= 1f;
		ManaGainRate = 1f;
		JumpStrength = 2.5f;
		BoneDartRepeaterMax = 15f;
		KineticShieldManaCost = 1f;
		TelekinesisManaCost = 0.5f;
		CannonBallManaCost = 20f;
		_CannonBall = new Fixture[10000];
		_CannonBallZone = new Fixture[10000];
		_CannonBallBulletTimer = new double[10000];
		CannonBallDraw = new bool[10000];
		CannonBallGo = new bool[10000];
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 1000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.UserData = 1000 + i;
		_bodyBody.Body.AngularDamping = 1000f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 1000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.AngularDamping = 1000f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_head));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 1000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1000f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 1000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1000f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 1f, 2f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 1000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 1000f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.5f, 4.8f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 1000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 1000f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.All;
		_rightHandBody.CollisionCategories = CollisionCategory.All;
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 1000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.AngularDamping = 1000f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 1000f;
		_rightThighBody.Body.AngularDamping = 1000f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_Rick_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Rick_rightThigh));
		_kineticShields = FixtureFactory.CreateCircle(_world, 3.3f, 1E-05f);
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		_kineticShields.Body.BodyType = BodyType.Kinematic;
		_kineticShields.Body.IsBullet = true;
		_kineticShields.Friction = 0f;
		_kineticShields.Restitution = 0f;
		_kineticShields.Body.UserData = 120;
		_kineticShields.Body.AngularDamping = 1f;
		_kineticShields.CollisionGroup = CollisionGroup;
		_kineticShields.CollidesWith = CollisionCategory.None;
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
		_bodyAngleJoint.UserData = 1;
		_leftUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
		_leftUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _leftUpperArmBody.Body);
		_rightUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
		_rightUpperArmJoint.MaxMotorTorque = 50000f;
		_rightUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _rightUpperArmBody.Body);
		_leftHandJoint = new RevoluteJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_rightHandJoint = new RevoluteJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_leftThighJoint = new RevoluteJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
		_leftThighJoint.MotorEnabled = true;
		_leftThighJoint.MaxMotorTorque = 100000000f;
		_leftThighJoint.MotorSpeed = 0f;
		_rightThighJoint = new RevoluteJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
		_rightThighJoint.MotorEnabled = true;
		_rightThighJoint.MaxMotorTorque = 100000000f;
		_rightThighJoint.MotorSpeed = 0f;
		_world.AddJoint(NeckJoint);
		_world.AddJoint(NeckAngleJoint);
		_world.AddJoint(_bodyAngleJoint);
		_world.AddJoint(_leftUpperArmJoint);
		_world.AddJoint(_rightUpperArmJoint);
		_world.AddJoint(_leftHandJoint);
		_world.AddJoint(_rightHandJoint);
		_world.AddJoint(_leftThighJoint);
		_world.AddJoint(_rightThighJoint);
	}

	public void Load_Vinny(Level level, World _world, int i, float Species)
	{
		PlayerHPBodyMax = 100f;
		PlayerHPBody = 75f;
		MaxJointForce = 90f;
		HpGainRate *= 3f;
		ManaGainRate = 2.25f;
		JumpStrength = 2.5f;
		FlyManaCost = 1f;
		TelekinesisManaCost = 1f;
		KineticShieldManaCost = 0.9f;
		CannonBallManaCost = 0.1f;
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 1000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.UserData = 1000 + i;
		_bodyBody.Body.AngularDamping = 1000f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 1000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.AngularDamping = 1000f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_head));
		Fixture bodyBody3 = _bodyBody;
		bodyBody3.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody3.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_body));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 1000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1000f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 1000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1000f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 2.4f, 4.8f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 1000f;
		_leftHandBody.Body.UserData = 98;
		_leftHandBody.Body.AngularDamping = 1000f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 1.2f, 2.4f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 1000f;
		_rightHandBody.Body.UserData = 98;
		_rightHandBody.Body.AngularDamping = 1000f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightHand));
		_leftHandBody_Claw = FixtureFactory.CreateRectangle(_world, 4.8f, 4.8f, density);
		_leftHandBody_Claw.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody_Claw.Body.BodyType = BodyType.Dynamic;
		_leftHandBody_Claw.Body.SleepingAllowed = true;
		_leftHandBody_Claw.Density = 2E-12f;
		_leftHandBody_Claw.Friction = 10000f;
		_leftHandBody_Claw.Body.UserData = 97;
		_leftHandBody_Claw.Body.AngularDamping = 1f;
		_leftHandBody_Claw.CollisionGroup = CollisionGroup;
		_leftHandBody_Claw.CollidesWith = CollisionCategory.None;
		_leftHandBody_Claw.CollisionCategories = CollisionCategory.None;
		_leftHandBody_Claw.Body.IsBullet = true;
		_leftHandBody_Claw.Body.Active = false;
		Fixture leftHandBody_Claw = _leftHandBody_Claw;
		leftHandBody_Claw.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody_Claw.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftHand_Claw));
		Fixture leftHandBody_Claw2 = _leftHandBody_Claw;
		leftHandBody_Claw2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody_Claw2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftHand_Claw));
		_rightHandBody_Claw = FixtureFactory.CreateRectangle(_world, 4.8f, 4.8f, density);
		_rightHandBody_Claw.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody_Claw.Body.BodyType = BodyType.Dynamic;
		_rightHandBody_Claw.Body.SleepingAllowed = true;
		_rightHandBody_Claw.Density = 2E-12f;
		_rightHandBody_Claw.Friction = 10000f;
		_rightHandBody_Claw.Body.UserData = 97;
		_rightHandBody_Claw.Body.AngularDamping = 1f;
		_rightHandBody_Claw.Body.IsBullet = true;
		_rightHandBody_Claw.Body.Active = false;
		_rightHandBody_Claw.CollisionGroup = CollisionGroup;
		_rightHandBody_Claw.CollidesWith = CollisionCategory.None;
		_rightHandBody_Claw.CollisionCategories = CollisionCategory.None;
		Fixture rightHandBody_Claw = _rightHandBody_Claw;
		rightHandBody_Claw.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody_Claw.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightHand_Claw));
		Fixture rightHandBody_Claw2 = _rightHandBody_Claw;
		rightHandBody_Claw2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody_Claw2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightHand_Claw));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 1000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.AngularDamping = 1000f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 1000f;
		_rightThighBody.Body.AngularDamping = 1000f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightThigh));
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
		_bodyAngleJoint.UserData = 1;
		_leftUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
		_leftUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _leftUpperArmBody.Body);
		_rightUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
		_rightUpperArmJoint.MaxMotorTorque = 50000f;
		_rightUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _rightUpperArmBody.Body);
		_leftHandJoint = new RevoluteJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_leftHandJoint.MotorEnabled = true;
		_leftHandJoint.MaxMotorTorque = 100000000f;
		_leftHandJoint.MotorSpeed = 0f;
		_rightHandJoint = new RevoluteJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_rightHandJoint.MotorEnabled = true;
		_rightHandJoint.MaxMotorTorque = 100000000f;
		_rightHandJoint.MotorSpeed = 0f;
		_leftThighJoint = new RevoluteJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
		_leftThighJoint.MotorEnabled = true;
		_leftThighJoint.MaxMotorTorque = 100000000f;
		_leftThighJoint.MotorSpeed = 0f;
		_rightThighJoint = new RevoluteJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
		_rightThighJoint.MotorEnabled = true;
		_rightThighJoint.MaxMotorTorque = 100000000f;
		_rightThighJoint.MotorSpeed = 0f;
		_world.AddJoint(NeckJoint);
		_world.AddJoint(NeckAngleJoint);
		_world.AddJoint(_bodyAngleJoint);
		_world.AddJoint(_leftUpperArmJoint);
		_world.AddJoint(_rightUpperArmJoint);
		_world.AddJoint(_leftHandJoint);
		_world.AddJoint(_rightHandJoint);
		_world.AddJoint(_leftThighJoint);
		_world.AddJoint(_rightThighJoint);
	}

	public void Load(Level level, World _world, int i, float Species)
	{
		_SightBrush = Level.Content.Load<Texture2D>("Sights/Red");
		_TelekinisisBrush = Level.Content.Load<Texture2D>("LevelBuilder/CenterDot");
		string text = "Sprites/" + Species + "/";
		CannonBallSound = level.Content.Load<SoundEffect>("SoundEffects/explosion");
		_CannonBall = new Fixture[10000];
		_CannonBallZone = new Fixture[10000];
		_CannonBallBulletTimer = new double[10000];
		CannonBallDraw = new bool[10000];
		CannonBallGo = new bool[10000];
		IceBallSound = level.Content.Load<SoundEffect>("SoundEffects/hitpipe");
		_IceBall = new Fixture[10000];
		_IceBallBulletTimer = new double[10000];
		DartBoneSound = level.Content.Load<SoundEffect>("SoundEffects/hitpipe");
		_DartBone = new Fixture[10000];
		_DartBoneBulletTimer = new double[10000];
		DartHarpoonSound = level.Content.Load<SoundEffect>("SoundEffects/hitpipe");
		_DartHarpoon = new Fixture[10000];
		_DartHarpoonBulletTimer = new double[10000];
		_DartKinetic = new Fixture[10000];
		_DartKineticZone = new Fixture[10000];
		_DartKineticBulletTimer = new double[10000];
		KineticGo = new bool[10000];
		KineticDraw = new bool[10000];
		_DartStasis = new Fixture[10000];
		_DartStasisZone = new Fixture[10000];
		_DartStasisBulletTimer = new double[10000];
		StasisGo = new bool[10000];
		StasisDraw = new bool[10000];
		BurrGo = new bool[10000];
		FixA_Burr = new Fixture[10000];
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 1000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.Body.LinearDamping = 0f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrush = Level.Content.Load<Texture2D>(text + "body");
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 1000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.LinearDamping = 0f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrush = Level.Content.Load<Texture2D>(text + "head");
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_head));
		Fixture bodyBody3 = _bodyBody;
		bodyBody3.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody3.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_body));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 1000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrush = Level.Content.Load<Texture2D>(text + "leftArm");
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 1000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrush = Level.Content.Load<Texture2D>(text + "rightArm");
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 1000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 1f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrush = Level.Content.Load<Texture2D>(text + "leftHand");
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.4f, 0.8f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(9.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 1000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 1f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrush = Level.Content.Load<Texture2D>(text + "rightHand");
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 1000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.LinearDamping = 0f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrush = Level.Content.Load<Texture2D>(text + "leftLeg");
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 1000f;
		_rightThighBody.Body.LinearDamping = 0f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrush = Level.Content.Load<Texture2D>(text + "rightLeg");
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_Vinny_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_Vinny_rightThigh));
		_kineticShields = FixtureFactory.CreateCircle(_world, 20f, 1E-05f);
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		_kineticShields.Body.BodyType = BodyType.Kinematic;
		_kineticShields.Body.IsBullet = true;
		_kineticShields.Friction = 0f;
		_kineticShields.Restitution = 0f;
		_kineticShields.Body.UserData = 120;
		_kineticShields.Body.LinearDamping = 0f;
		_kineticShields.CollisionGroup = CollisionGroup;
		_kineticShields.CollidesWith = CollisionCategory.None;
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
		_bodyAngleJoint.UserData = 1;
		_leftUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
		_leftUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _leftUpperArmBody.Body);
		_rightUpperArmJoint = new RevoluteJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
		_rightUpperArmJoint.MaxMotorTorque = 50000f;
		_rightUpperArmAngleJoint = new AngleJoint(_bodyBody.Body, _rightUpperArmBody.Body);
		_leftHandJoint = new RevoluteJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_rightHandJoint = new RevoluteJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		_leftThighJoint = new RevoluteJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
		_leftThighJoint.MotorEnabled = true;
		_leftThighJoint.MaxMotorTorque = 100000000f;
		_leftThighJoint.MotorSpeed = 0f;
		_rightThighJoint = new RevoluteJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
		_rightThighJoint.MotorEnabled = true;
		_rightThighJoint.MaxMotorTorque = 100000000f;
		_rightThighJoint.MotorSpeed = 0f;
		_world.AddJoint(NeckJoint);
		_world.AddJoint(NeckAngleJoint);
		_world.AddJoint(_bodyAngleJoint);
		_world.AddJoint(_leftUpperArmJoint);
		_world.AddJoint(_rightUpperArmJoint);
		_world.AddJoint(_leftHandJoint);
		_world.AddJoint(_rightHandJoint);
		_world.AddJoint(_leftThighJoint);
		_world.AddJoint(_rightThighJoint);
	}

	private bool OnCollision_Daru_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && fixtureB.CollisionGroup != 0)
		{
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 3f;
				SoundImpailed.Play(level.mainGame.Sound_Effect_Volume, 1f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Daru_body(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Daru_head(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					KnockBack = true;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 2f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Daru_head(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Daru_leftHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					MaceFixture = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandLeftColor = Color.Red;
		_leftHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftHandIsTouching = true;
		}
		else
		{
			LeftHandIsTouching = false;
			_leftHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Daru_leftHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandLeftColor = Color.White;
	}

	private bool OnCollision_Daru_rightHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandRightColor = Color.Red;
		_rightHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightHandIsTouching = true;
		}
		else
		{
			RightHandIsTouching = false;
			_rightHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Daru_rightHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandRightColor = Color.White;
	}

	private bool OnCollision_Daru_leftThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighLeftColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				LeftFootIsOnGround = true;
				if (!Dead && !LeftFootIsOnGround)
				{
					LeftFootIsOnGround = true;
				}
			}
			else
			{
				LeftFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Daru_leftThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			WasDustyLeft = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Daru_rightThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighRightColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !RightFootIsOnGround)
				{
					RightFootIsOnGround = true;
				}
			}
			else
			{
				RightFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Daru_rightThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighRightColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			WasDustyRight = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool DartBoneSaw_OnCollision_Daru(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		fixtureA.Body.IgnoreGravity = false;
		fixtureA.Restitution = 0.1f;
		fixtureA.Body.UserData = 1001;
		fixtureA.Body.Mass = 0.1f;
		fixtureA.Density = 2E-07f;
		return true;
	}

	private bool DartHarpoon_OnCollision_Daru(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		FixA = fixtureA;
		FixB = fixtureB;
		fixtureA.Body.IgnoreGravity = false;
		fixtureA.Restitution = 0.1f;
		fixtureA.Body.UserData = (int)fixtureA.Body.UserData + 1;
		return true;
	}

	private bool Shield_OnCollision_Daru(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		return true;
	}

	private bool OnCollision_Ernest_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && fixtureB.CollisionGroup != 0)
		{
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup)
			{
				BounceHit = true;
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Ernest_body(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Ernest_head(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup)
			{
				BounceHit = true;
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 2f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Ernest_head(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Ernest_leftHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0)
			{
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup)
				{
					BounceHit = true;
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandLeftColor = Color.Red;
		_leftHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftHandIsTouching = true;
		}
		else
		{
			LeftHandIsTouching = false;
			_leftHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Ernest_leftHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandLeftColor = Color.White;
	}

	private bool OnCollision_Ernest_rightHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0)
			{
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup)
				{
					BounceHit = true;
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandRightColor = Color.Red;
		_rightHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightHandIsTouching = true;
		}
		else
		{
			RightHandIsTouching = false;
		}
		return true;
	}

	private void OnSeparation_Ernest_rightHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandRightColor = Color.White;
	}

	private bool OnCollision_Ernest_leftThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0)
			{
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup)
				{
					BounceHit = true;
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighLeftColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !LeftFootIsOnGround)
				{
					LeftFootIsOnGround = true;
				}
			}
			else
			{
				LeftFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Ernest_leftThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			WasDustyLeft = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Ernest_rightThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0)
			{
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage * (fixtureB.Body.LinearVelocity.X + fixtureB.Body.LinearVelocity.Y);
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (fixtureB.CollisionGroup != CannonBallCollisionGroup)
				{
					BounceHit = true;
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighRightColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !RightFootIsOnGround)
				{
					RightFootIsOnGround = true;
				}
			}
			else
			{
				RightFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Ernest_rightThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighRightColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			WasDustyRight = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool DartBurr_OnCollision_Ernest(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureB.Body.UserData != 98 && (int)fixtureB.Body.UserData != 99 && (int)fixtureB.Body.UserData != 199 && (int)fixtureB.Body.UserData != 120 && (int)fixtureB.Body.UserData != 121 && (int)fixtureB.Body.UserData != 122 && (int)fixtureA.Body.UserData != 998 && (int)fixtureB.Body.UserData != 9 && (int)fixtureB.Body.UserData != 1 && fixtureB.CollisionGroup != CollisionGroup && fixtureB.Body.BodyType != BodyType.Static)
		{
			FixA_Burr[(int)fixtureA.UserData - 1000] = fixtureA;
			FixB = fixtureB;
			BurrGo[(int)fixtureA.UserData - 1000] = true;
		}
		return true;
	}

	private bool Weapon_OnCollision_Mace_Ernest(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		MaceBashedFixture = fixtureB;
		MaceBashed = true;
		return true;
	}

	private bool Climb_Sensor_OnCollision_Ernest(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureA.Body.UserData != 8)
		{
			Sensor_Fixture = fixtureB;
			Sensed_Something = true;
			return false;
		}
		return true;
	}

	private bool OnCollision_Oscar_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (PhaseShift)
		{
			if (fixtureB != null && (int)fixtureB.Body.UserData != 1)
			{
				contact.Enabled = false;
				return false;
			}
		}
		else if (fixtureB != null && fixtureB.CollisionGroup != 0)
		{
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Oscar_body(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Oscar_head(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (PhaseShift)
		{
			if (fixtureB != null && (int)fixtureB.Body.UserData != 1)
			{
				contact.Enabled = false;
				return false;
			}
		}
		else if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 2f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Oscar_head(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Oscar_leftHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (PhaseShift)
		{
			if (fixtureB != null && (int)fixtureB.Body.UserData != 1)
			{
				contact.Enabled = false;
				return false;
			}
		}
		else
		{
			if (fixtureB != null)
			{
				if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 98)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 97)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage * 6f;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 201)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RatDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 202)
				{
					PlayerHPBody -= Slap_Damage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
				}
				if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldPush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
				if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldX = true;
				}
				if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					BouncePush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
			}
			HandLeftColor = Color.Red;
			_leftHandGrabOtherFixture = fixtureB;
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				LeftHandIsTouching = true;
			}
			else
			{
				LeftHandIsTouching = false;
				_leftHandGrabOtherFixture = null;
			}
		}
		return true;
	}

	private void OnSeparation_Oscar_leftHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandLeftColor = Color.White;
	}

	private bool OnCollision_Oscar_rightHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (PhaseShift)
		{
			if (fixtureB != null && (int)fixtureB.Body.UserData != 1)
			{
				contact.Enabled = false;
				return false;
			}
		}
		else
		{
			if (fixtureB != null)
			{
				if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 98)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 97)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage * 6f;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 201)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RatDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 202)
				{
					PlayerHPBody -= Slap_Damage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
				}
				if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldPush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
				if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldX = true;
				}
				if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					BouncePush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
			}
			HandRightColor = Color.Red;
			_rightHandGrabOtherFixture = fixtureB;
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				RightHandIsTouching = true;
			}
			else
			{
				RightHandIsTouching = false;
				_rightHandGrabOtherFixture = null;
			}
		}
		return true;
	}

	private void OnSeparation_Oscar_rightHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandRightColor = Color.White;
	}

	private bool OnCollision_Oscar_leftThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (PhaseShift)
		{
			if (fixtureB != null && (int)fixtureB.Body.UserData != 1)
			{
				contact.Enabled = false;
				return false;
			}
		}
		else
		{
			if (fixtureB != null)
			{
				if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 98)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 97)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage * 6f;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 201)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RatDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 202)
				{
					PlayerHPBody -= Slap_Damage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
				}
				if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldPush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
				if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldX = true;
				}
				if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					BouncePush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
			}
			ThighLeftColor = Color.Red;
			if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
			{
				if (fixtureB.CollisionGroup != CollisionGroup)
				{
					if (!Dead && !LeftFootIsOnGround)
					{
						LeftFootIsOnGround = true;
					}
				}
				else
				{
					LeftFootIsOnGround = false;
				}
			}
		}
		return true;
	}

	private void OnSeparation_Oscar_leftThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			WasDustyLeft = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Oscar_rightThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (PhaseShift)
		{
			if (fixtureB != null && (int)fixtureB.Body.UserData != 1)
			{
				contact.Enabled = false;
				return false;
			}
		}
		else
		{
			if (fixtureB != null)
			{
				if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
				{
					if ((int)fixtureB.Body.UserData == 120)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= RockDamage;
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 121 && !Frozen)
					{
						Freezer = true;
						Frozen = true;
					}
					if ((int)fixtureB.Body.UserData == 133 && !Shocked)
					{
						Shocker = true;
						Shocked = true;
						level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 122)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartBoneDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 199)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= DartKineticDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
					}
					if ((int)fixtureB.Body.UserData == 233)
					{
						particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
						particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						particleEffectBleed.Trigger(new Vector2(0f, 0f));
						PlayerHPBody -= WeaponMaceDamage;
						SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
						level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
					}
				}
				if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 98)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 97)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= SharpsNeddleDamage * 6f;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 201)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RatDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 202)
				{
					PlayerHPBody -= Slap_Damage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
				}
				if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldPush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
				if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					ForceFieldX = true;
				}
				if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
				{
					ForceFixB = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
					BouncePush = true;
					FixB = fixtureB;
					contact.Enabled = false;
					return false;
				}
			}
			ThighRightColor = Color.Red;
			if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
			{
				if (fixtureB.CollisionGroup != CollisionGroup)
				{
					if (!Dead && !RightFootIsOnGround)
					{
						RightFootIsOnGround = true;
					}
				}
				else
				{
					RightFootIsOnGround = false;
				}
			}
		}
		return true;
	}

	private void OnSeparation_Oscar_rightThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighRightColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			WasDustyRight = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool DartBallLightning_OnCollision_Oscar(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (Alive)
		{
			LightningBallSound.Play(level.mainGame.Sound_Effect_Volume, 0.5f, 0f);
			particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectCannonBallEx[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectCannonBallEx.Trigger(new Vector2(0f, 0f));
			CannonBallGo[(int)fixtureA.UserData - 1000] = true;
			_CannonBallZone_Wait_One[(int)fixtureA.UserData - 1000] = 0;
			CannonBall = fixtureA;
			_CannonBallBulletTimer[(int)fixtureA.UserData - 1000] = 0.0;
		}
		return true;
	}

	private bool DartBallLightning_OnCollision_Oscar_Zone(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		return true;
	}

	private bool DartEctoBall_OnCollision_Oscar(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (Alive && (int)fixtureB.Body.UserData != 1 && (int)fixtureB.Body.UserData != 9 && (int)fixtureB.Body.UserData != 10)
		{
			CannonBallSound.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectKineticEx[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectKineticEx[1].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectKineticEx.Trigger(new Vector2(0f, 0f));
			KineticGo[(int)fixtureA.UserData - 1000] = true;
			DartKineticDart = fixtureA;
		}
		return true;
	}

	private bool DartEctoBall_OnCollision_Oscar_Zone(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		return true;
	}

	private bool OnCollision_Rick_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && fixtureB.CollisionGroup != 0)
		{
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Rick_body(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Rick_head(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 2f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Rick_head(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Rick_leftHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandLeftColor = Color.Red;
		_leftHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftHandIsTouching = true;
		}
		else
		{
			LeftHandIsTouching = false;
			_leftHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Rick_leftHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandLeftColor = Color.White;
	}

	private bool OnCollision_Rick_rightHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandRightColor = Color.Red;
		_rightHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightHandIsTouching = true;
		}
		else
		{
			RightHandIsTouching = false;
			_rightHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Rick_rightHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandRightColor = Color.White;
	}

	private bool OnCollision_Rick_leftThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighLeftColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !LeftFootIsOnGround)
				{
					LeftFootIsOnGround = true;
				}
			}
			else
			{
				LeftFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Rick_leftThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			WasDustyLeft = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Rick_rightThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighRightColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !RightFootIsOnGround)
				{
					RightFootIsOnGround = true;
				}
			}
			else
			{
				RightFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Rick_rightThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighRightColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			WasDustyRight = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool DartRock_OnCollision_Rick(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if ((int)fixtureA.UserData >= 100)
		{
			SoundWalking.Play(level.mainGame.Sound_Effect_Volume, -1f, 0f);
			fixtureA.UserData = (int)fixtureA.UserData / 2;
		}
		else
		{
			fixtureA.Body.UserData = 1;
		}
		return true;
	}

	private bool DartRock_OnCollision_Rick_Zone(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		return true;
	}

	private bool OnCollision_Vinny_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && fixtureB.CollisionGroup != 0)
		{
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 3f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Vinny_body(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Vinny_head(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					KnockBack = true;
					MaceFixture = fixtureB;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				Impailed = true;
				ImpailingBody = fixtureB;
				ImpailedBody = fixtureA;
				Dead = true;
				level.Vibration_Pulse_Left(playerIndex, HurtImpailedVibrationDuration, HurtImpailedVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage * 2f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		return true;
	}

	private void OnSeparation_Vinny_head(Fixture fixtureA, Fixture fixtureB)
	{
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Vinny_leftHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandLeftColor = Color.Red;
		_leftHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftHandIsTouching = true;
		}
		else
		{
			LeftHandIsTouching = false;
			_leftHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Vinny_leftHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandLeftColor = Color.White;
	}

	private bool OnCollision_Vinny_leftHand_Claw(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB.Body != null && fixtureB.Body.UserData != null && (int)fixtureB.Body.UserData == 8)
		{
			particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			PlayerHPBody += SharpsNeddleDamage * 1f;
		}
		return true;
	}

	private void OnSeparation_Vinny_leftHand_Claw(Fixture fixtureA, Fixture fixtureB)
	{
	}

	private bool OnCollision_Vinny_rightHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		HandRightColor = Color.Red;
		_rightHandGrabOtherFixture = fixtureB;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightHandIsTouching = true;
		}
		else
		{
			RightHandIsTouching = false;
			_rightHandGrabOtherFixture = null;
		}
		return true;
	}

	private void OnSeparation_Vinny_rightHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandRightColor = Color.White;
	}

	private bool OnCollision_Vinny_rightHand_Claw(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB.Body != null && fixtureB.Body.UserData != null && (int)fixtureB.Body.UserData == 8)
		{
			particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseImpulse = new Vector2(level.random.Next(-200, 200), level.random.Next(-200, 200));
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			PlayerHPBody += SharpsNeddleDamage * 1f;
		}
		return true;
	}

	private void OnSeparation_Vinny_rightHand_Claw(Fixture fixtureA, Fixture fixtureB)
	{
	}

	private bool OnCollision_Vinny_leftThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighLeftColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !LeftFootIsOnGround)
				{
					LeftFootIsOnGround = true;
				}
			}
			else
			{
				LeftFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Vinny_leftThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			WasDustyLeft = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_Vinny_rightThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				if ((int)fixtureB.Body.UserData == 120)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= RockDamage;
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtRockVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 121 && !Frozen)
				{
					Freezer = true;
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 133 && !Shocked)
				{
					Shocker = true;
					Shocked = true;
					level.Vibration_Pulse_Left(playerIndex, HurtBallLightningVibrationDuration, HurtBallLightningVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartBoneDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtBoneVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= DartKineticDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtEctoBallVibrationSpeed);
				}
				if ((int)fixtureB.Body.UserData == 233)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					PlayerHPBody -= WeaponMaceDamage;
					SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + -0.5f, 0f);
					level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtMaceVibrationSpeed);
				}
			}
			if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 97)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= SharpsNeddleDamage * 6f;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 201)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				PlayerHPBody -= RatDamage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtClawVibrationSpeed);
			}
			if ((int)fixtureB.Body.UserData == 202)
			{
				PlayerHPBody -= Slap_Damage;
				SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, 0f, 0f);
			}
			if ((int)fixtureB.Body.UserData == 300 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldPush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
			if ((int)fixtureB.Body.UserData == 301 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				ForceFieldX = true;
			}
			if ((int)fixtureB.Body.UserData == 302 && fixtureA.Body.Position.X - fixtureB.Body.Position.X > (float)(-Factor) && fixtureB.Body.Position.X - fixtureA.Body.Position.X < (float)Factor && fixtureA.Body.Position.Y - fixtureB.Body.Position.Y > (float)(-Factor) && fixtureB.Body.Position.Y - fixtureA.Body.Position.Y < (float)Factor)
			{
				ForceFixB = fixtureB;
				level.Vibration_Pulse_Left(playerIndex, HurtVibrationDuration, HurtSpikeVibrationSpeed);
				BouncePush = true;
				FixB = fixtureB;
				contact.Enabled = false;
				return false;
			}
		}
		ThighRightColor = Color.Red;
		if ((int)fixtureB.Body.UserData != 999 && (int)fixtureB.Body.UserData != 300 && (int)fixtureB.Body.UserData != 301)
		{
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				if (!Dead && !RightFootIsOnGround)
				{
					RightFootIsOnGround = true;
				}
			}
			else
			{
				RightFootIsOnGround = false;
			}
		}
		return true;
	}

	private void OnSeparation_Vinny_rightThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighRightColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			WasDustyRight = false;
		}
		if (((int)fixtureB.Body.UserData > 989) & ((int)fixtureB.Body.UserData < 1000))
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	public void RemoveAllJoints(World _world)
	{
		if (NeckJoint != null)
		{
			_world.RemoveJoint(NeckJoint);
		}
		if (NeckAngleJoint != null)
		{
			_world.RemoveJoint(NeckAngleJoint);
		}
		if (_bodyAngleJoint != null)
		{
			_world.RemoveJoint(_bodyAngleJoint);
		}
		if (_leftUpperArmJoint != null)
		{
			_world.RemoveJoint(_leftUpperArmJoint);
		}
		if (_rightUpperArmJoint != null)
		{
			_world.RemoveJoint(_rightUpperArmJoint);
		}
		if (_leftHandJoint != null)
		{
			_world.RemoveJoint(_leftHandJoint);
		}
		if (_rightHandJoint != null)
		{
			_world.RemoveJoint(_rightHandJoint);
		}
		if (_leftThighJoint != null)
		{
			_world.RemoveJoint(_leftThighJoint);
		}
		if (_rightThighJoint != null)
		{
			_world.RemoveJoint(_rightThighJoint);
		}
	}

	public void CollisionCatagoryTo99()
	{
		_bodyBody.CollisionGroup = 99;
		_headBody.CollisionGroup = 99;
		_leftUpperArmBody.CollisionGroup = 99;
		_leftThighBody.CollisionGroup = 99;
		_leftHandBody.CollisionGroup = 99;
		_rightUpperArmBody.CollisionGroup = 99;
		_rightThighBody.CollisionGroup = 99;
		_rightHandBody.CollisionGroup = 99;
		_bodyBody.UserData = 20;
		_headBody.UserData = 20;
		_leftUpperArmBody.UserData = 20;
		_leftThighBody.UserData = 20;
		_leftHandBody.UserData = 20;
		_rightUpperArmBody.UserData = 20;
		_rightThighBody.UserData = 20;
		_rightHandBody.UserData = 20;
	}

	public void UserDataToNull()
	{
		_bodyBody.Body.UserData = 1;
		_headBody.Body.UserData = 1;
		_leftUpperArmBody.Body.UserData = 1;
		_leftThighBody.Body.UserData = 1;
		_leftHandBody.Body.UserData = 1;
		_rightUpperArmBody.Body.UserData = 1;
		_rightThighBody.Body.UserData = 1;
		_rightHandBody.Body.UserData = 1;
		_bodyBody.Body.UserData = 1;
		_headBody.Body.UserData = 1;
		_leftUpperArmBody.Body.UserData = 1;
		_leftThighBody.Body.UserData = 1;
		_leftHandBody.Body.UserData = 1;
		_rightUpperArmBody.Body.UserData = 1;
		_rightThighBody.Body.UserData = 1;
		_rightHandBody.Body.UserData = 1;
	}

	public void LimpJoints()
	{
		NeckAngleJoint.Softness = 0.98f;
		level._world.RemoveJoint(_bodyAngleJoint);
		if (_leftThighJoint != null)
		{
			_leftThighJoint.MotorSpeed = 0f;
			_leftThighJoint.MaxMotorTorque = 0f;
		}
		if (_rightThighJoint != null)
		{
			_rightThighJoint.MotorSpeed = 0f;
			_rightThighJoint.MaxMotorTorque = 0f;
		}
	}

	public void ClearForces()
	{
		if (!ClearedForces)
		{
			ClearedForces = true;
			_bodyBody.Body.AngularVelocity = 0f;
			_headBody.Body.AngularVelocity = 0f;
			_leftUpperArmBody.Body.AngularVelocity = 0f;
			_rightUpperArmBody.Body.AngularVelocity = 0f;
			_leftHandBody.Body.AngularVelocity = 0f;
			_rightHandBody.Body.AngularVelocity = 0f;
			_leftThighBody.Body.AngularVelocity = 0f;
			_rightThighBody.Body.AngularVelocity = 0f;
			_bodyBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_headBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_leftHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_rightHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_leftThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
			_rightThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
			PlayerMovement = new Vector2(0f, 0f);
			movementX = 0f;
		}
	}

	public void Clear_X_Forces()
	{
		_bodyBody.Body.LinearVelocity = new Vector2(0f, _bodyBody.Body.LinearVelocity.Y);
		_headBody.Body.LinearVelocity = new Vector2(0f, _headBody.Body.LinearVelocity.Y);
		_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, _leftUpperArmBody.Body.LinearVelocity.Y);
		_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, _rightUpperArmBody.Body.LinearVelocity.Y);
		_leftHandBody.Body.LinearVelocity = new Vector2(0f, _leftHandBody.Body.LinearVelocity.Y);
		_rightHandBody.Body.LinearVelocity = new Vector2(0f, _rightHandBody.Body.LinearVelocity.Y);
		_leftThighBody.Body.LinearVelocity = new Vector2(0f, _leftThighBody.Body.LinearVelocity.Y);
		_rightThighBody.Body.LinearVelocity = new Vector2(0f, _rightThighBody.Body.LinearVelocity.Y);
		PlayerMovement = new Vector2(0f, PlayerMovement.Y);
		movementX = 0f;
		WasSprinting = false;
	}

	public void SoftJoints()
	{
		NeckAngleJoint.Softness = 0.98f;
		if (_bodyAngleJoint != null)
		{
			level._world.RemoveJoint(_bodyAngleJoint);
		}
		if (_leftThighJoint != null)
		{
			_leftThighJoint.MotorSpeed = 0f;
			_leftThighJoint.MaxMotorTorque = 0f;
		}
		if (_rightThighJoint != null)
		{
			_rightThighJoint.MotorSpeed = 0f;
			_rightThighJoint.MaxMotorTorque = 0f;
		}
	}

	public void StiffJoints()
	{
		NeckAngleJoint.Softness = 0f;
		level._world.AddJoint(_bodyAngleJoint);
		if (_leftThighJoint != null)
		{
			_leftThighJoint.MotorSpeed = 0f;
			_leftThighJoint.MaxMotorTorque = 100000000f;
		}
		if (_rightThighJoint != null)
		{
			_rightThighJoint.MotorSpeed = 0f;
			_rightThighJoint.MaxMotorTorque = 100000000f;
		}
	}

	private void BodyPartsManager2()
	{
		if (Dead)
		{
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollidesWith = CollisionCategory.All;
			float num = 20f;
			if (_bodyBody.Body.GetLinearVelocityFromLocalPoint(_bodyBody.Body.Position).Length() < num)
			{
				_bodyBody.Body.Active = false;
			}
			if (_headBody.Body.GetLinearVelocityFromLocalPoint(_headBody.Body.Position).Length() < num)
			{
				_headBody.Body.Awake = false;
			}
			if (_leftUpperArmBody.Body.GetLinearVelocityFromLocalPoint(_leftUpperArmBody.Body.Position).Length() < num)
			{
				_leftUpperArmBody.Body.Active = false;
			}
			if (_rightUpperArmBody.Body.GetLinearVelocityFromLocalPoint(_rightUpperArmBody.Body.Position).Length() < num)
			{
				_rightUpperArmBody.Body.Active = false;
			}
			if (_leftThighBody.Body.GetLinearVelocityFromLocalPoint(_leftThighBody.Body.Position).Length() < num)
			{
				_leftThighBody.Body.Active = false;
			}
			if (_rightThighBody.Body.GetLinearVelocityFromLocalPoint(_rightThighBody.Body.Position).Length() < num)
			{
				_rightThighBody.Body.Active = false;
			}
			if (_leftHandBody.Body.GetLinearVelocityFromLocalPoint(_leftHandBody.Body.Position).Length() < num)
			{
				_leftHandBody.Body.Active = false;
			}
			if (_rightHandBody.Body.GetLinearVelocityFromLocalPoint(_rightHandBody.Body.Position).Length() < num)
			{
				_rightHandBody.Body.Active = false;
			}
		}
	}

	private void BodyPartsManager3(World _world)
	{
		if (Dead)
		{
			if (_leftHandBody.Body != null)
			{
				_leftHandBody.CollidesWith = CollisionCategory.All;
			}
			if (_rightHandBody.Body != null)
			{
				_rightHandBody.CollidesWith = CollisionCategory.All;
			}
			float num = 20f;
			if (!_bodyBodyGone && _bodyBody.Body.GetLinearVelocityFromLocalPoint(_bodyBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_bodyBody.Body);
				_bodyBodyGone = true;
			}
			if (_headBody.Body.GetLinearVelocityFromLocalPoint(_headBody.Body.Position).Length() < num)
			{
				_headBody.Body.Awake = false;
			}
			if (!_leftUpperArmBodyGone && _leftUpperArmBody.Body.GetLinearVelocityFromLocalPoint(_leftUpperArmBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_leftUpperArmBody.Body);
				_leftUpperArmBodyGone = true;
			}
			if (!_rightUpperArmBodyGone && _rightUpperArmBody.Body.GetLinearVelocityFromLocalPoint(_rightUpperArmBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_rightUpperArmBody.Body);
				_rightUpperArmBodyGone = true;
			}
			if (!_leftThighBodyGone && _leftThighBody.Body.GetLinearVelocityFromLocalPoint(_leftThighBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_leftThighBody.Body);
				_leftThighBodyGone = true;
			}
			if (!_rightThighBodyGone && _rightThighBody.Body.GetLinearVelocityFromLocalPoint(_rightThighBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_rightThighBody.Body);
				_rightThighBodyGone = true;
			}
			if (!_leftHandBodyGone && _leftHandBody.Body.GetLinearVelocityFromLocalPoint(_leftHandBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_leftHandBody.Body);
				_leftHandBodyGone = true;
			}
			if (!_rightHandBodyGone && _rightHandBody.Body.GetLinearVelocityFromLocalPoint(_rightHandBody.Body.Position).Length() < num)
			{
				_world.RemoveBody(_rightHandBody.Body);
				_rightHandBodyGone = true;
			}
			Active = false;
		}
	}

	private void CheckJointError()
	{
		MaxJointForce = 60f;
		if (NeckJoint != null && NeckJoint.JointSpeed > MaxJointForce)
		{
			level._world.RemoveJoint(NeckJoint);
			if (NeckAngleJoint != null)
			{
				level._world.RemoveJoint(NeckAngleJoint);
				SoundImpailed.Play(level.mainGame.Sound_Effect_Volume * 5f, 0.5f, 0f);
				SoundHeadPopOff.Play(level.mainGame.Sound_Effect_Volume * 5f, 0f, 0f);
			}
			NeckJoint = null;
		}
		if (_leftUpperArmJoint != null && _leftUpperArmJoint.JointSpeed > MaxJointForce)
		{
			SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
			particleEffectBleed[0].TriggerOffset = _leftUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			LeftArmSevered = true;
			level._world.RemoveJoint(_leftHandJoint);
			level._world.RemoveJoint(_leftUpperArmJoint);
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_leftHandBody.CollisionCategories = CollisionCategory.All;
		}
		if (_rightUpperArmJoint != null && _rightUpperArmJoint.JointSpeed > MaxJointForce)
		{
			SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
			particleEffectBleed[0].TriggerOffset = _rightUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			RightArmSevered = true;
			level._world.RemoveJoint(_rightUpperArmJoint);
			level._world.RemoveJoint(_rightHandJoint);
			_rightHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollisionCategories = CollisionCategory.All;
		}
		if (_leftThighJoint != null && _leftThighJoint.JointSpeed > MaxJointForce)
		{
			SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
			particleEffectBleed[0].TriggerOffset = _leftThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			LeftLegSevered = true;
			level._world.RemoveJoint(_leftThighJoint);
		}
		if (_rightThighJoint != null && _rightThighJoint.JointSpeed > MaxJointForce)
		{
			SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
			particleEffectBleed[0].TriggerOffset = _rightThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			RightLegSevered = true;
			level._world.RemoveJoint(_rightThighJoint);
		}
	}

	private void BodyPartsManager4(GameTime gameTime)
	{
		if (!Dead)
		{
			return;
		}
		CheckJointError();
		_leftHandBody.CollidesWith = CollisionCategory.All;
		_rightHandBody.CollidesWith = CollisionCategory.All;
		LimpJoints();
		_bodyBody.CollisionGroup = 99;
		_headBody.CollisionGroup = 99;
		_leftUpperArmBody.CollisionGroup = 99;
		_rightUpperArmBody.CollisionGroup = 99;
		_leftHandBody.CollisionGroup = 99;
		_rightHandBody.CollisionGroup = 99;
		_leftThighBody.CollisionGroup = 99;
		_rightThighBody.CollisionGroup = 99;
		if (_bodyBody.Body.AngularDamping > 1f)
		{
			_bodyBody.Body.AngularDamping -= 50f;
		}
		if (_bodyBody.Body.LinearDamping > 1f)
		{
			_bodyBody.Body.LinearDamping -= 50f;
		}
		if (_headBody.Body.AngularDamping > 1f)
		{
			_headBody.Body.AngularDamping -= 50f;
		}
		if (_headBody.Body.LinearDamping > 1f)
		{
			_headBody.Body.LinearDamping -= 50f;
		}
		if (NeckJoint == null)
		{
			if (_leftUpperArmJoint != null && _rightUpperArmJoint != null && _leftUpperArmJoint != null && _rightUpperArmJoint != null)
			{
				Vector2 vector = new Vector2(0f, 0f);
				Vector2 vector2 = new Vector2(0f, 0f);
				if (_leftUpperArmJoint != null && _rightUpperArmJoint != null)
				{
					vector = (_leftUpperArmJoint.WorldAnchorA + _rightUpperArmJoint.WorldAnchorA) / 2f;
				}
				if (_leftThighJoint != null && _rightThighJoint != null)
				{
					vector2 = (_leftThighJoint.WorldAnchorA + _rightThighJoint.WorldAnchorA) / 2f;
				}
				particleEffectBloodSquirting[0].TriggerOffset = vector * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBloodSquirting[0].ReleaseImpulse = (vector - vector2) * new Vector2(40f, 40f);
				particleEffectBloodSquirting[1].TriggerOffset = vector * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBloodSquirting[1].ReleaseImpulse = (vector - vector2) * new Vector2(30f, 30f);
				particleEffectBloodSquirting[2].TriggerOffset = vector * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectBloodSquirting[2].ReleaseImpulse = new Vector2(0f, 0f);
				particleEffectBloodSquirting.Trigger(new Vector2(0f, 0f));
			}
		}
		else
		{
			particleEffectBleeding[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
		}
	}

	public void Walking_Control()
	{
		if (_walkerBody.Body.Position.X > _leftThighBodyPivotBody.Body.Position.X)
		{
			_leftThighAngleJoint.TargetAngle = _rightThighAngleJointTargetAngle;
		}
		else
		{
			_leftThighAngleJoint.TargetAngle = _leftThighAngleJointTargetAngle;
		}
		if (_walkerBody.Body.Position.X < _rightThighBodyPivotBody.Body.Position.X)
		{
			_rightThighAngleJoint.TargetAngle = _leftThighAngleJointTargetAngle;
		}
		else
		{
			_rightThighAngleJoint.TargetAngle = _rightThighAngleJointTargetAngle;
		}
	}

	public void UpdateUtilities()
	{
		_ = UtilityIndexLeft;
		_ = 0f;
	}

	public void DestroyAll(World _world)
	{
		RemoveAllJoints(_world);
		if (_bodyBody != null && _bodyBody.Body != null)
		{
			_world.RemoveBody(_bodyBody.Body);
		}
		if (_headBody != null && _headBody.Body != null)
		{
			_world.RemoveBody(_headBody.Body);
		}
		if (_leftUpperArmBody != null && _leftUpperArmBody.Body != null)
		{
			_world.RemoveBody(_leftUpperArmBody.Body);
		}
		if (_rightUpperArmBody != null && _rightUpperArmBody.Body != null)
		{
			_world.RemoveBody(_rightUpperArmBody.Body);
		}
		if (_leftHandBody != null && _leftHandBody.Body != null)
		{
			_world.RemoveBody(_leftHandBody.Body);
		}
		if (_rightHandBody != null && _rightHandBody.Body != null)
		{
			_world.RemoveBody(_rightHandBody.Body);
		}
		if (_leftHandBody_Claw != null && _leftHandBody_Claw.Body != null)
		{
			_world.RemoveBody(_leftHandBody_Claw.Body);
		}
		if (_rightHandBody_Claw != null && _rightHandBody_Claw.Body != null)
		{
			_world.RemoveBody(_rightHandBody_Claw.Body);
		}
		if (_leftThighBody != null && _leftThighBody.Body != null)
		{
			_world.RemoveBody(_leftThighBody.Body);
		}
		if (_rightThighBody != null && _rightThighBody.Body != null)
		{
			_world.RemoveBody(_rightThighBody.Body);
		}
		if (_kineticShields != null && _kineticShields.Body != null)
		{
			_world.RemoveBody(_kineticShields.Body);
		}
		RemoveAllDarts(_world);
	}

	public void RemoveAllDarts(World _world)
	{
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBall != null && _CannonBall[i] != null && _CannonBall[i].Body != null && _CannonBall[i].Body.FixtureList != null)
			{
				_world.RemoveBody(_CannonBall[i].Body);
			}
		}
		for (int j = 0; j < CannonBallIndex; j++)
		{
			if (_CannonBallZone != null && _CannonBallZone[j] != null && _CannonBallZone[j].Body != null && _CannonBallZone[j].Body.FixtureList != null)
			{
				_world.RemoveBody(_CannonBallZone[j].Body);
			}
		}
		for (int k = 0; k < IceBallIndex; k++)
		{
			if (_IceBall[k] != null && _IceBall[k].Body != null && _IceBall[k].Body.FixtureList != null)
			{
				_world.RemoveBody(_IceBall[k].Body);
			}
		}
		for (int l = 0; l < DartBoneIndex; l++)
		{
			if (_DartBone[l] != null && _DartBone[l].Body != null && _DartBone[l].Body.FixtureList != null)
			{
				_world.RemoveBody(_DartBone[l].Body);
			}
		}
		for (int m = 0; m < DartHarpoonIndex; m++)
		{
			if (_DartHarpoon[m] != null && _DartHarpoon[m].Body != null && _DartHarpoon[m].Body.FixtureList != null)
			{
				_world.RemoveBody(_DartHarpoon[m].Body);
			}
		}
		for (int n = 0; n < DartKineticIndex; n++)
		{
			if (_DartKinetic[n] != null && _DartKinetic[n].Body != null && _DartKinetic[n].Body.FixtureList != null)
			{
				_world.RemoveBody(_DartKinetic[n].Body);
			}
		}
		for (int num = 0; num < DartKineticIndex; num++)
		{
			if (_DartKineticZone[num] != null && _DartKineticZone[num].Body != null && _DartKineticZone[num].Body.FixtureList != null)
			{
				_world.RemoveBody(_DartKineticZone[num].Body);
			}
		}
		for (int num2 = 0; num2 < DartStasisIndex; num2++)
		{
			if (_DartStasis[num2] != null && _DartStasis[num2].Body != null && _DartStasis[num2].Body.FixtureList != null)
			{
				_world.RemoveBody(_DartStasis[num2].Body);
			}
		}
		RemovedDarts = true;
	}

	public void Update_Physics_Daru(GameTime gameTime, World _world)
	{
		if (!RemovedDarts)
		{
			for (int i = 0; i < DartBoneIndex; i++)
			{
				if (_DartBoneBulletTimer[i] > 5.0)
				{
					_DartBoneBulletTimer[i]++;
				}
				if (_DartBoneBulletTimer[i] > (double)(175 * Oscar_Update_Speed))
				{
					_DartBoneBulletTimer[i] = 0.0;
					if (_DartBone[i] != null && _DartBone[i].Body != null && _DartBone[i].Body.FixtureList != null)
					{
						_DartBone[i].Body.Awake = true;
						_world.RemoveBody(_DartBone[i].Body);
						_DartBone[i] = null;
					}
				}
			}
			for (int j = 0; j < DartHarpoonIndex; j++)
			{
				if (_DartHarpoonBulletTimer[j] > 5.0)
				{
					_DartHarpoonBulletTimer[j]++;
				}
				if (_DartHarpoonBulletTimer[j] > (double)(750 * Oscar_Update_Speed))
				{
					_DartHarpoonBulletTimer[j] = 0.0;
					if (_DartHarpoon[j] != null && _DartHarpoon[j].Body != null && _DartHarpoon[j].Body.FixtureList != null)
					{
						_DartHarpoon[j].Body.Awake = true;
						_world.RemoveBody(_DartHarpoon[j].Body);
						_DartHarpoon[j] = null;
					}
				}
			}
		}
		if (!DeadByBounds)
		{
			BodyPartsManager4(gameTime);
		}
		if (Dead)
		{
			Shield_OFF_Daru(_world);
			if (Impailed && !IsImpailed)
			{
				SoundImpailed.Play(level.mainGame.Sound_Effect_Volume, 1f, 0f);
				IsImpailed = true;
				WeldJoint joint = new WeldJoint(ImpailedBody.Body, ImpailingBody.Body, new Vector2(0f, 0f), new Vector2(0f, 0f));
				_world.AddJoint(joint);
			}
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollidesWith = CollisionCategory.All;
			LimpJoints();
			ClearForces();
			PlayerHPBody = 0f;
			Active = false;
			StandStateIndex = 0f;
			Crouch = false;
			Crawl = false;
			Jump = true;
			UserDataToNull();
			Unconscious = false;
		}
		if (Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (OldUnconsciousTime + (double)Unconscious_Time < gameTime.TotalGameTime.TotalSeconds)
			{
				Unconscious = false;
				StiffJoints();
			}
			else
			{
				if (_rightHandGrabOtherFixture != null && GrabWithRightHandBool)
				{
					_rightHandGrabOtherFixture = null;
					if (_rightHandGrabJoint != null)
					{
						_world.RemoveJoint(_rightHandGrabJoint);
					}
				}
				Update_Injuries(gameTime, _world);
				UpdateFreeze(gameTime, _world);
				if (Shocker)
				{
					Particle_Shock(gameTime, _world);
				}
				UpdateShock(gameTime, _world);
				if (Freezer)
				{
					Particle_Freeze(gameTime, _world);
				}
			}
		}
		if (Alive & !Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (KnockBack)
			{
				LimpJoints();
				ClearForces();
				Vector2 vector = new Vector2(30000f, 30000f);
				if (MaceFixture != null && MaceFixture.Body != null)
				{
					_bodyBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_headBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_leftUpperArmBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_rightUpperArmBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_leftHandBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_rightHandBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_leftThighBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
					_rightThighBody.Body.ApplyForce(MaceFixture.Body.LinearVelocity * vector);
				}
				KnockBack = false;
				Unconscious = true;
				OldUnconsciousTime = gameTime.TotalGameTime.TotalSeconds;
				Unconscious_Time = 5;
			}
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			Update_Injuries(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (PlayerHPBody >= PlayerHPBodyMax - 10f)
			{
				if (LeftArmSevered)
				{
					LeftArmSevered = false;
					_leftUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1f, 0f);
					_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftUpperArmJoint);
					_leftHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1.25f, 0f);
					_leftHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftHandJoint);
					_leftHandBody.CollidesWith = CollisionCategory.None;
					_leftHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (RightArmSevered)
				{
					RightArmSevered = false;
					_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(1f, 0f);
					_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightUpperArmJoint);
					_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(1.25f, 0f);
					_rightHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightHandJoint);
					_rightHandBody.CollidesWith = CollisionCategory.None;
					_rightHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (LeftLegSevered)
				{
					LeftLegSevered = false;
					_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-0.5f, 1.25f);
					_leftThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftThighJoint);
					LeftFootIsOnGround = false;
				}
				if (RightLegSevered)
				{
					RightLegSevered = false;
					_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(0.5f, 1.25f);
					_rightThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightThighJoint);
					RightFootIsOnGround = false;
				}
			}
		}
		if (Dead)
		{
			Alive = false;
		}
		if (_leftThighJointRemoved && _rightThighJointRemoved)
		{
			RemoveAllJoints(_world);
			Alive = false;
			Dead = true;
			CollisionCatagoryTo99();
		}
		if (!RightFootIsOnGround || !LeftFootIsOnGround || (!GrabWithLeftHandBool && !GrabWithRightHandBool))
		{
			if (bodyLinearVelocity_Y > 100f)
			{
				_bodyAngleJoint.Softness = 500f;
			}
			else
			{
				_bodyAngleJoint.Softness = 0f;
			}
		}
		else
		{
			_bodyAngleJoint.Softness = 0f;
		}
		GetInput2_Physics(_world, gameTime);
	}

	public void Update_Physics_Ernest(GameTime gameTime, World _world)
	{
		if (!RemovedDarts)
		{
			if (MaceBashed)
			{
				float num = 100000f;
				MaceBashedFixture.Body.ApplyForce(new Vector2(MaceBashedFixture.Body.Position.X - _kineticShields.Body.Position.X, MaceBashedFixture.Body.Position.Y - _kineticShields.Body.Position.Y) * new Vector2(num, num));
				MaceBashed = false;
			}
			for (int i = 0; i < DartBoneIndex; i++)
			{
				if (_DartBoneBulletTimer[i] > 5.0)
				{
					_DartBoneBulletTimer[i]++;
				}
				if (_DartBoneBulletTimer[i] > (double)(400 * Oscar_Update_Speed))
				{
					_DartBoneBulletTimer[i] = 0.0;
					if (_DartBone[i] != null && _DartBone[i].Body != null && _DartBone[i].Body.FixtureList != null)
					{
						_world.RemoveBody(_DartBone[i].Body);
						_DartBone[i] = null;
					}
				}
				if (BurrGo[i] && FixA_Burr[i] != null && FixB != null && (int)FixA_Burr[i].Body.UserData != 1999 && (int)FixB.Body.UserData != 1999)
				{
					WeldJoint joint = new WeldJoint(FixA_Burr[i].Body, FixB.Body, FixA_Burr[i].Body.Position - FixB.Body.Position, new Vector2(0f, 0f));
					_world.AddJoint(joint);
					FixA_Burr[i].Body.UserData = 1999;
					FixA_Burr[i].Body.LinearDamping = 2f;
					FixA_Burr[i].Body.AngularDamping = 100f;
					FixA_Burr[i].Body.Mass = 0.001f;
				}
			}
		}
		if (!DeadByBounds)
		{
			BodyPartsManager4(gameTime);
		}
		if (Dead)
		{
			Weapon_OFF_Ernest(_world);
			_kineticShields.Body.Position = _bodyBody.Body.Position;
			_kineticShields.Body.Active = false;
			_kineticShields.CollidesWith = CollisionCategory.None;
			_kineticShields.CollisionCategories = CollisionCategory.None;
			Climb_OFF_Ernest(_world);
			if (_leftThighJoint != null)
			{
				_leftThighJoint.MotorEnabled = false;
			}
			if (_rightThighJoint != null)
			{
				_rightThighJoint.MotorEnabled = false;
			}
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorEnabled = false;
			}
			if (_rightUpperArmJoint != null)
			{
				_rightUpperArmJoint.MotorEnabled = false;
			}
			if (Impailed && !IsImpailed)
			{
				IsImpailed = true;
				WeldJoint joint2 = new WeldJoint(ImpailedBody.Body, ImpailingBody.Body, new Vector2(0f, 0f), new Vector2(0f, 0f));
				_world.AddJoint(joint2);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			if (Frozen)
			{
				Particle_UnFreeze(gameTime, _world);
				RemoveAllJoints(_world);
				Color = Color.CadetBlue;
			}
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollidesWith = CollisionCategory.All;
			LimpJoints();
			ClearForces();
			PlayerHPBody = 0f;
			Active = false;
			StandStateIndex = 0f;
			Crouch = false;
			Crawl = false;
			Jump = true;
			UserDataToNull();
			Unconscious = false;
		}
		if (Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (OldUnconsciousTime + (double)Unconscious_Time < gameTime.TotalGameTime.TotalSeconds)
			{
				Unconscious = false;
				StiffJoints();
			}
			else
			{
				if (_rightHandGrabOtherFixture != null && GrabWithRightHandBool)
				{
					_rightHandGrabOtherFixture = null;
					if (_rightHandGrabJoint != null)
					{
						_world.RemoveJoint(_rightHandGrabJoint);
					}
				}
				Climb_OFF_Ernest(_world);
				Update_Injuries(gameTime, _world);
				UpdateFreeze(gameTime, _world);
				if (Shocker)
				{
					Particle_Shock(gameTime, _world);
				}
				UpdateShock(gameTime, _world);
				if (Freezer)
				{
					Particle_Freeze(gameTime, _world);
				}
			}
		}
		if (Alive & !Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (KnockBack)
			{
				LimpJoints();
				ClearForces();
				Vector2 vector = new Vector2(300f, 300f);
				_bodyBody.Body.ApplyForce((_bodyBody.Body.Position - MaceFixture.Body.Position) * vector);
				_headBody.Body.ApplyForce((_headBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftUpperArmBody.Body.ApplyForce((_leftUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightUpperArmBody.Body.ApplyForce((_rightUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftHandBody.Body.ApplyForce((_leftHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightHandBody.Body.ApplyForce((_rightHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftThighBody.Body.ApplyForce((_leftThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightThighBody.Body.ApplyForce((_rightThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				KnockBack = false;
				Unconscious = true;
				OldUnconsciousTime = gameTime.TotalGameTime.TotalSeconds;
				Unconscious_Time = 5;
			}
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			Update_Injuries(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (PlayerHPBody >= PlayerHPBodyMax - 10f)
			{
				if (LeftArmSevered)
				{
					LeftArmSevered = false;
					_leftUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1f, 0f);
					_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftUpperArmJoint);
					_leftHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1.25f, 0f);
					_leftHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftHandJoint);
					_leftHandBody.CollidesWith = CollisionCategory.None;
					_leftHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (RightArmSevered)
				{
					RightArmSevered = false;
					_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(1f, 0f);
					_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightUpperArmJoint);
					_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(1.25f, 0f);
					_rightHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightHandJoint);
					_rightHandBody.CollidesWith = CollisionCategory.None;
					_rightHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (LeftLegSevered)
				{
					LeftLegSevered = false;
					_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-0.5f, 1.25f);
					_leftThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftThighJoint);
					LeftFootIsOnGround = false;
				}
				if (RightLegSevered)
				{
					RightLegSevered = false;
					_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(0.5f, 1.25f);
					_rightThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightThighJoint);
					RightFootIsOnGround = false;
				}
			}
			if (Crawl)
			{
				if (DirectionLeft)
				{
					_bodyAngleJoint.TargetAngle = CrawlBodyAngle;
					NeckAngleJoint.TargetAngle = CrawlNeckAngle;
				}
				if (DirectionRight)
				{
					_bodyAngleJoint.TargetAngle = 0f - CrawlBodyAngle;
					NeckAngleJoint.TargetAngle = 0f - CrawlNeckAngle;
				}
				if (!DirectionLeft && !DirectionRight)
				{
					_bodyAngleJoint.TargetAngle = CrawlBodyAngle;
					NeckAngleJoint.TargetAngle = CrawlNeckAngle;
				}
			}
			else
			{
				_bodyAngleJoint.TargetAngle = 0f;
				NeckAngleJoint.TargetAngle = 0f;
			}
		}
		if (Dead)
		{
			Alive = false;
		}
		if (_leftThighJointRemoved && _rightThighJointRemoved)
		{
			RemoveAllJoints(_world);
			Alive = false;
			Dead = true;
			CollisionCatagoryTo99();
		}
		if (!RightFootIsOnGround || !LeftFootIsOnGround || (!GrabWithLeftHandBool && !GrabWithRightHandBool))
		{
			if (bodyLinearVelocity_Y > 100f)
			{
				_bodyAngleJoint.Softness = 500f;
			}
			else
			{
				_bodyAngleJoint.Softness = 0f;
			}
		}
		else
		{
			_bodyAngleJoint.Softness = 0f;
		}
		GetInput2_Physics(_world, gameTime);
	}

	public void Update_Physics_Oscar(GameTime gameTime, World _world)
	{
		if (!RemovedDarts)
		{
			if (_DartKinetic != null)
			{
				for (int i = 0; i < DartKineticIndex; i++)
				{
					if (_DartKineticBulletTimer[i] > 5.0)
					{
						_DartKineticBulletTimer[i]++;
					}
					if (_DartKineticBulletTimer[i] > (double)(250 * Oscar_Update_Speed))
					{
						_DartKineticBulletTimer[i] = 0.0;
						if (_DartKinetic[i] != null && _DartKinetic[i].Body != null && _DartKinetic[i].Body.FixtureList != null)
						{
							Fixture obj = _DartKinetic[i];
							obj.OnCollision = (CollisionEventHandler)Delegate.Remove(obj.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar));
							_world.RemoveBody(_DartKinetic[i].Body);
							_DartKinetic[i] = null;
						}
						if (_DartKineticZone[i] != null && _DartKineticZone[i].Body != null && _DartKineticZone[i].Body.FixtureList != null)
						{
							Fixture obj2 = _DartKineticZone[i];
							obj2.OnCollision = (CollisionEventHandler)Delegate.Remove(obj2.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar_Zone));
							_world.RemoveBody(_DartKineticZone[i].Body);
							_DartKineticZone[i] = null;
						}
					}
					if (KineticGo[i])
					{
						if (_DartKinetic[i] != null && _DartKinetic[i].Body != null)
						{
							_DartKinetic[i].Body.Active = false;
							if (_DartKineticZone[i] != null && _DartKineticZone[i].Body != null && _DartKineticZone[i].Body.FixtureList != null)
							{
								_DartKineticZone[i].Body.Position = _DartKinetic[i].Body.Position;
							}
						}
						foreach (Body body in _world.BodyList)
						{
							if (body == null || body == null)
							{
								continue;
							}
							_ = (int)body.UserData;
							if ((int)body.UserData != 13 && body.FixtureList[0].CollisionGroup != CollisionGroup)
							{
								Vector2 point = body.Position;
								float num = 1E+11f;
								if (_DartKineticZone[i] != null && _DartKineticZone[i].TestPoint(ref point))
								{
									body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
									body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
									body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
									body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
								}
							}
						}
						if (_DartKinetic[i] != null && _DartKinetic[i].Body != null && _DartKinetic[i].Body.FixtureList != null)
						{
							Fixture obj3 = _DartKinetic[i];
							obj3.OnCollision = (CollisionEventHandler)Delegate.Remove(obj3.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar));
							_world.RemoveBody(_DartKinetic[i].Body);
							_DartKinetic[i] = null;
						}
						if (_DartKineticZone[i] != null && _DartKineticZone[i].Body != null && _DartKineticZone[i].Body.FixtureList != null)
						{
							Fixture obj4 = _DartKineticZone[i];
							obj4.OnCollision = (CollisionEventHandler)Delegate.Remove(obj4.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar_Zone));
							_world.RemoveBody(_DartKineticZone[i].Body);
							_DartKineticZone[i] = null;
						}
						KineticGo[i] = false;
						KineticDraw[i] = false;
					}
					else if (_DartKineticZone[i] != null && _DartKineticZone[i].Body != null)
					{
						_DartKineticZone[i].Body.Position = new Vector2(0f, 0f);
					}
				}
			}
			for (int j = 0; j < CannonBallIndex; j++)
			{
				if (_CannonBall == null)
				{
					continue;
				}
				if (_CannonBallBulletTimer[j] > 5.0)
				{
					_CannonBallBulletTimer[j]++;
				}
				if (_CannonBallBulletTimer[j] > (double)(250 * Oscar_Update_Speed))
				{
					_CannonBallBulletTimer[j] = 0.0;
					if (_CannonBall[j] != null && _CannonBall[j].Body != null && _CannonBall[j].Body.FixtureList != null)
					{
						Fixture obj5 = _CannonBall[j];
						obj5.OnCollision = (CollisionEventHandler)Delegate.Remove(obj5.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar));
						_world.RemoveBody(_CannonBall[j].Body);
						_CannonBall[j] = null;
					}
					if (_CannonBallZone[j] != null && _CannonBallZone[j].Body != null && _CannonBallZone[j].Body.FixtureList != null)
					{
						Fixture obj6 = _CannonBallZone[j];
						obj6.OnCollision = (CollisionEventHandler)Delegate.Remove(obj6.OnCollision, new CollisionEventHandler(DartBallLightning_OnCollision_Oscar_Zone));
						_world.RemoveBody(_CannonBallZone[j].Body);
						_CannonBallZone[j] = null;
					}
				}
				if (CannonBallGo[j])
				{
					if (_CannonBall[j] != null && _CannonBall[j].Body != null)
					{
						_CannonBall[j].Body.Active = false;
						if (_CannonBallZone[j] != null && _CannonBallZone[j].Body != null && _CannonBallZone[j].Body.FixtureList != null)
						{
							_CannonBallZone[j].Body.Position = _CannonBall[j].Body.Position;
							_CannonBallZone[j].Body.Active = true;
							_CannonBallZone[j].CollidesWith = CollisionCategory.All;
							_CannonBallZone[j].CollisionCategories = CollisionCategory.All;
							_CannonBallZone[j].Body.Awake = true;
						}
					}
					foreach (Body body2 in _world.BodyList)
					{
						if (body2 == null)
						{
							continue;
						}
						_ = (int)body2.UserData;
						if ((int)body2.UserData != 13)
						{
							Vector2 point2 = body2.Position;
							float num2 = 1000f;
							if (_CannonBallZone[j].TestPoint(ref point2))
							{
								body2.ApplyForce(new Vector2(body2.Position.X - CannonBall.Body.Position.X, body2.Position.Y - CannonBall.Body.Position.Y) * new Vector2(num2, num2));
								body2.ApplyForce(new Vector2(body2.Position.X - CannonBall.Body.Position.X, body2.Position.Y - CannonBall.Body.Position.Y) * new Vector2(num2, num2));
								body2.ApplyForce(new Vector2(body2.Position.X - CannonBall.Body.Position.X, body2.Position.Y - CannonBall.Body.Position.Y) * new Vector2(num2, num2));
								body2.ApplyForce(new Vector2(body2.Position.X - CannonBall.Body.Position.X, body2.Position.Y - CannonBall.Body.Position.Y) * new Vector2(num2, num2));
							}
						}
					}
					if (_CannonBall[j] != null && _CannonBall[j].Body != null && _CannonBall[j].Body.FixtureList != null)
					{
						Fixture obj7 = _CannonBall[j];
						obj7.OnCollision = (CollisionEventHandler)Delegate.Remove(obj7.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar));
						_world.RemoveBody(_CannonBall[j].Body);
						_CannonBall[j] = null;
					}
					if (_CannonBallZone[j] != null && _CannonBallZone[j].Body != null && _CannonBallZone[j].Body.FixtureList != null)
					{
						if (_CannonBallZone_Wait_One[j] > 10)
						{
							Fixture obj8 = _CannonBallZone[j];
							obj8.OnCollision = (CollisionEventHandler)Delegate.Remove(obj8.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar_Zone));
							_world.RemoveBody(_CannonBallZone[j].Body);
							_CannonBallZone[j] = null;
							CannonBallGo[j] = false;
						}
						_CannonBallZone_Wait_One[j]++;
					}
					CannonBallDraw[j] = false;
				}
				else if (_CannonBallZone[j] != null && _CannonBallZone[j].Body != null)
				{
					_CannonBallZone[j].Body.Position = new Vector2(0f, 0f);
				}
			}
		}
		if (!DeadByBounds)
		{
			BodyPartsManager4(gameTime);
		}
		if (Dead)
		{
			PhaseShift = false;
			PhaseShift_OFF();
			if (Impailed && !IsImpailed)
			{
				IsImpailed = true;
				WeldJoint joint = new WeldJoint(ImpailedBody.Body, ImpailingBody.Body, new Vector2(0f, 0f), new Vector2(0f, 0f));
				_world.AddJoint(joint);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			if (Frozen)
			{
				Particle_UnFreeze(gameTime, _world);
				RemoveAllJoints(_world);
				Color = Color.CadetBlue;
			}
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollidesWith = CollisionCategory.All;
			LimpJoints();
			ClearForces();
			PlayerHPBody = 0f;
			SlowTime_OFF_Oscar(_world);
			Active = false;
			StandStateIndex = 0f;
			Crouch = false;
			Crawl = false;
			Jump = true;
			UserDataToNull();
			Unconscious = false;
		}
		if (Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (OldUnconsciousTime + (double)Unconscious_Time < gameTime.TotalGameTime.TotalSeconds)
			{
				Unconscious = false;
				StiffJoints();
			}
			else
			{
				if (_rightHandGrabOtherFixture != null && GrabWithRightHandBool)
				{
					_rightHandGrabOtherFixture = null;
					if (_rightHandGrabJoint != null)
					{
						_world.RemoveJoint(_rightHandGrabJoint);
					}
				}
				Update_Injuries(gameTime, _world);
				UpdateFreeze(gameTime, _world);
				if (Shocker)
				{
					Particle_Shock(gameTime, _world);
				}
				UpdateShock(gameTime, _world);
				if (Freezer)
				{
					Particle_Freeze(gameTime, _world);
				}
			}
		}
		if (Alive & !Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (KnockBack)
			{
				LimpJoints();
				ClearForces();
				Vector2 vector = new Vector2(300f, 300f);
				_bodyBody.Body.ApplyForce((_bodyBody.Body.Position - MaceFixture.Body.Position) * vector);
				_headBody.Body.ApplyForce((_headBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftUpperArmBody.Body.ApplyForce((_leftUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightUpperArmBody.Body.ApplyForce((_rightUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftHandBody.Body.ApplyForce((_leftHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightHandBody.Body.ApplyForce((_rightHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftThighBody.Body.ApplyForce((_leftThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightThighBody.Body.ApplyForce((_rightThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				KnockBack = false;
				Unconscious = true;
				OldUnconsciousTime = gameTime.TotalGameTime.TotalSeconds;
				Unconscious_Time = 5;
			}
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			Update_Injuries(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (PlayerHPBody >= PlayerHPBodyMax - 10f)
			{
				if (LeftArmSevered)
				{
					LeftArmSevered = false;
					_leftUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1f, 0f);
					_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftUpperArmJoint);
					_leftHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1.25f, 0f);
					_leftHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftHandJoint);
					_leftHandBody.CollidesWith = CollisionCategory.None;
					_leftHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (RightArmSevered)
				{
					RightArmSevered = false;
					_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(1f, 0f);
					_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightUpperArmJoint);
					_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(1.25f, 0f);
					_rightHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightHandJoint);
					_rightHandBody.CollidesWith = CollisionCategory.None;
					_rightHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (LeftLegSevered)
				{
					LeftLegSevered = false;
					_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-0.5f, 1.25f);
					_leftThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftThighJoint);
					LeftFootIsOnGround = false;
				}
				if (RightLegSevered)
				{
					RightLegSevered = false;
					_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(0.5f, 1.25f);
					_rightThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightThighJoint);
					RightFootIsOnGround = false;
				}
			}
		}
		if (Dead)
		{
			Alive = false;
		}
		if (_leftThighJointRemoved && _rightThighJointRemoved)
		{
			RemoveAllJoints(_world);
			Alive = false;
			Dead = true;
			CollisionCatagoryTo99();
		}
		if (!RightFootIsOnGround || !LeftFootIsOnGround || (!GrabWithLeftHandBool && !GrabWithRightHandBool))
		{
			if (bodyLinearVelocity_Y > 100f)
			{
				_bodyAngleJoint.Softness = 500f;
			}
			else
			{
				_bodyAngleJoint.Softness = 0f;
			}
		}
		else
		{
			_bodyAngleJoint.Softness = 0f;
		}
		GetInput2_Physics(_world, gameTime);
	}

	public void Update_Physics_Rick(GameTime gameTime, World _world)
	{
		if (!RemovedDarts)
		{
			for (int i = 0; i < CannonBallIndex; i++)
			{
				if (_CannonBall == null)
				{
					continue;
				}
				if (_CannonBall[i] != null && !_CannonBall[i].Body.Awake)
				{
					CannonBallGo[i] = true;
				}
				if (CannonBallGo[i])
				{
					if (_CannonBall[i] != null && _CannonBall[i].Body != null && _CannonBall[i].Body.FixtureList != null)
					{
						_world.RemoveBody(_CannonBall[i].Body);
						_CannonBall[i] = null;
					}
					if (_CannonBallZone[i] != null && _CannonBallZone[i].Body != null && _CannonBallZone[i].Body.FixtureList != null)
					{
						_world.RemoveBody(_CannonBallZone[i].Body);
						_CannonBallZone[i] = null;
					}
					CannonBallGo[i] = false;
					CannonBallDraw[i] = false;
				}
				else if (_CannonBallZone[i] != null && _CannonBallZone[i].Body != null)
				{
					_CannonBallZone[i].Body.Position = new Vector2(0f, 0f);
				}
			}
		}
		if (!DeadByBounds)
		{
			BodyPartsManager4(gameTime);
		}
		if (Dead)
		{
			if (Impailed && !IsImpailed)
			{
				IsImpailed = true;
				WeldJoint joint = new WeldJoint(ImpailedBody.Body, ImpailingBody.Body, new Vector2(0f, 0f), new Vector2(0f, 0f));
				_world.AddJoint(joint);
			}
			Grab_OFF_Rick(_world, gameTime);
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			if (Frozen)
			{
				Particle_UnFreeze(gameTime, _world);
				RemoveAllJoints(_world);
				Color = Color.CadetBlue;
			}
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollidesWith = CollisionCategory.All;
			LimpJoints();
			ClearForces();
			PlayerHPBody = 0f;
			RockSkin_OFF_Rick(_world);
			Active = false;
			StandStateIndex = 0f;
			Crouch = false;
			Crawl = false;
			Jump = true;
			UserDataToNull();
			Unconscious = false;
		}
		if (Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (OldUnconsciousTime + (double)Unconscious_Time < gameTime.TotalGameTime.TotalSeconds)
			{
				Unconscious = false;
				StiffJoints();
			}
			else
			{
				if (_rightHandGrabOtherFixture != null && GrabWithRightHandBool)
				{
					_rightHandGrabOtherFixture = null;
					if (_rightHandGrabJoint != null)
					{
						_world.RemoveJoint(_rightHandGrabJoint);
					}
				}
				Update_Injuries(gameTime, _world);
				UpdateFreeze(gameTime, _world);
				if (Shocker)
				{
					Particle_Shock(gameTime, _world);
				}
				UpdateShock(gameTime, _world);
				if (Freezer)
				{
					Particle_Freeze(gameTime, _world);
				}
			}
		}
		if (Alive & !Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			if (KnockBack)
			{
				LimpJoints();
				ClearForces();
				Vector2 vector = new Vector2(300f, 300f);
				_bodyBody.Body.ApplyForce((_bodyBody.Body.Position - MaceFixture.Body.Position) * vector);
				_headBody.Body.ApplyForce((_headBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftUpperArmBody.Body.ApplyForce((_leftUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightUpperArmBody.Body.ApplyForce((_rightUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftHandBody.Body.ApplyForce((_leftHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightHandBody.Body.ApplyForce((_rightHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftThighBody.Body.ApplyForce((_leftThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightThighBody.Body.ApplyForce((_rightThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				KnockBack = false;
				Unconscious = true;
				OldUnconsciousTime = gameTime.TotalGameTime.TotalSeconds;
				Unconscious_Time = 5;
			}
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			Update_Injuries(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (PlayerHPBody >= PlayerHPBodyMax - 10f)
			{
				if (LeftArmSevered)
				{
					LeftArmSevered = false;
					_leftUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1f, 0f);
					_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftUpperArmJoint);
					_leftHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1.25f, 0f);
					_leftHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftHandJoint);
					_leftHandBody.CollidesWith = CollisionCategory.None;
					_leftHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (RightArmSevered)
				{
					RightArmSevered = false;
					_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(1f, 0f);
					_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightUpperArmJoint);
					_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(1.25f, 0f);
					_rightHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightHandJoint);
					_rightHandBody.CollidesWith = CollisionCategory.None;
					_rightHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (LeftLegSevered)
				{
					LeftLegSevered = false;
					_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-0.5f, 1.25f);
					_leftThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftThighJoint);
					LeftFootIsOnGround = false;
				}
				if (RightLegSevered)
				{
					RightLegSevered = false;
					_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(0.5f, 1.25f);
					_rightThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightThighJoint);
					RightFootIsOnGround = false;
				}
			}
		}
		if (Dead)
		{
			Alive = false;
		}
		if (_leftThighJointRemoved && _rightThighJointRemoved)
		{
			RemoveAllJoints(_world);
			Alive = false;
			Dead = true;
			CollisionCatagoryTo99();
		}
		if (!RightFootIsOnGround || !LeftFootIsOnGround || (!GrabWithLeftHandBool && !GrabWithRightHandBool))
		{
			if (bodyLinearVelocity_Y > 100f)
			{
				_bodyAngleJoint.Softness = 500f;
			}
			else
			{
				_bodyAngleJoint.Softness = 0f;
			}
		}
		else
		{
			_bodyAngleJoint.Softness = 0f;
		}
		GetInput2_Physics(_world, gameTime);
	}

	public void Update_Physics_Vinny(GameTime gameTime, World _world)
	{
		BodyPartsManager4(gameTime);
		if (Dead)
		{
			_leftHandBody_Claw.Body.Position = _leftHandBody.Body.Position;
			_rightHandBody_Claw.Body.Position = _rightHandBody.Body.Position;
			Fly_OFF_Vinny(_world);
			Claw_OFF_Vinny(_world);
			IsClawing = false;
			if (Impailed && !IsImpailed)
			{
				IsImpailed = true;
				WeldJoint joint = new WeldJoint(ImpailedBody.Body, ImpailingBody.Body, new Vector2(0f, 0f), new Vector2(0f, 0f));
				_world.AddJoint(joint);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			if (Frozen)
			{
				Particle_UnFreeze(gameTime, _world);
				RemoveAllJoints(_world);
				Color = Color.CadetBlue;
			}
			_leftHandBody.CollidesWith = CollisionCategory.All;
			_rightHandBody.CollidesWith = CollisionCategory.All;
			LimpJoints();
			ClearForces();
			PlayerHPBody = 0f;
			Fly_OFF_Vinny(_world);
			Active = false;
			StandStateIndex = 0f;
			Crouch = false;
			Crawl = false;
			Jump = true;
			UserDataToNull();
			Unconscious = false;
		}
		if (Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			_leftHandBody_Claw.Body.Position = _leftHandBody.Body.Position;
			_rightHandBody_Claw.Body.Position = _rightHandBody.Body.Position;
			Fly_OFF_Vinny(_world);
			Claw_OFF_Vinny(_world);
			if (OldUnconsciousTime + (double)Unconscious_Time < gameTime.TotalGameTime.TotalSeconds)
			{
				Unconscious = false;
				if (!IsFlying)
				{
					StiffJoints();
				}
			}
			else
			{
				if (_rightHandGrabOtherFixture != null && GrabWithRightHandBool)
				{
					_rightHandGrabOtherFixture = null;
					if (_rightHandGrabJoint != null)
					{
						_world.RemoveJoint(_rightHandGrabJoint);
					}
				}
				Update_Injuries(gameTime, _world);
				UpdateFreeze(gameTime, _world);
				if (Shocker)
				{
					Particle_Shock(gameTime, _world);
				}
				UpdateShock(gameTime, _world);
				if (Freezer)
				{
					Particle_Freeze(gameTime, _world);
				}
			}
		}
		if (Alive & !Unconscious)
		{
			if (BouncePush)
			{
				Bounce();
				BouncePush = false;
			}
			if (ForceFieldPush)
			{
				ForcePush();
				ForceFieldPush = false;
			}
			if (ForceFieldX)
			{
				ForceX();
				ForceFieldX = false;
			}
			_leftHandBody_Claw.Body.Position = _leftHandBody.Body.Position;
			_rightHandBody_Claw.Body.Position = _rightHandBody.Body.Position;
			if (KnockBack)
			{
				LimpJoints();
				ClearForces();
				Vector2 vector = new Vector2(300f, 300f);
				_bodyBody.Body.ApplyForce((_bodyBody.Body.Position - MaceFixture.Body.Position) * vector);
				_headBody.Body.ApplyForce((_headBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftUpperArmBody.Body.ApplyForce((_leftUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightUpperArmBody.Body.ApplyForce((_rightUpperArmBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftHandBody.Body.ApplyForce((_leftHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightHandBody.Body.ApplyForce((_rightHandBody.Body.Position - MaceFixture.Body.Position) * vector);
				_leftThighBody.Body.ApplyForce((_leftThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				_rightThighBody.Body.ApplyForce((_rightThighBody.Body.Position - MaceFixture.Body.Position) * vector);
				KnockBack = false;
				Unconscious = true;
				OldUnconsciousTime = gameTime.TotalGameTime.TotalSeconds;
				Unconscious_Time = 5;
			}
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			Update_Injuries(gameTime, _world);
			if (Freezer)
			{
				Particle_Freeze(gameTime, _world);
			}
			UpdateFreeze(gameTime, _world);
			if (Shocker)
			{
				Particle_Shock(gameTime, _world);
			}
			UpdateShock(gameTime, _world);
			if (PlayerHPBody >= PlayerHPBodyMax - 10f)
			{
				if (LeftArmSevered)
				{
					LeftArmSevered = false;
					_leftUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1f, 0f);
					_leftUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftUpperArmJoint);
					_leftHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(-1.25f, 0f);
					_leftHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftHandJoint);
					_leftHandBody.CollidesWith = CollisionCategory.None;
					_leftHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (RightArmSevered)
				{
					RightArmSevered = false;
					_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(1f, 0f);
					_rightUpperArmBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightUpperArmBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightUpperArmJoint);
					_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(1.25f, 0f);
					_rightHandBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightHandBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightHandJoint);
					_rightHandBody.CollidesWith = CollisionCategory.None;
					_rightHandBody.CollisionCategories = CollisionCategory.None;
				}
				if (LeftLegSevered)
				{
					LeftLegSevered = false;
					_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-0.5f, 1.25f);
					_leftThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_leftThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_leftThighJoint);
					LeftFootIsOnGround = false;
				}
				if (RightLegSevered)
				{
					RightLegSevered = false;
					_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(0.5f, 1.25f);
					_rightThighBody.Body.LinearVelocity = new Vector2(0f, 0f);
					_rightThighBody.Body.AngularVelocity = 0f;
					_world.AddJoint(_rightThighJoint);
					RightFootIsOnGround = false;
				}
			}
		}
		if (Dead)
		{
			Alive = false;
		}
		if (_leftThighJointRemoved && _rightThighJointRemoved)
		{
			RemoveAllJoints(_world);
			Alive = false;
			Dead = true;
			CollisionCatagoryTo99();
		}
		if (!RightFootIsOnGround || !LeftFootIsOnGround || (!GrabWithLeftHandBool && !GrabWithRightHandBool))
		{
			if (bodyLinearVelocity_Y > 100f)
			{
				_bodyAngleJoint.Softness = 500f;
			}
			else
			{
				_bodyAngleJoint.Softness = 0f;
			}
		}
		else
		{
			_bodyAngleJoint.Softness = 0f;
		}
		GetInput2_Physics(_world, gameTime);
	}

	public void Update_Physics(GameTime gameTime, World _world)
	{
		if (DeadByBounds)
		{
			_bodyBody.Body.BodyType = BodyType.Static;
			_headBody.Body.BodyType = BodyType.Static;
		}
		if (_headBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _headBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_bodyBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _bodyBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_rightUpperArmBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _rightUpperArmBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_leftUpperArmBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _leftUpperArmBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_rightThighBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _rightThighBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_leftThighBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _leftThighBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_rightHandBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _rightHandBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (_leftHandBody.CollisionGroup != CollisionGroup)
		{
			if (!Already_LimpJoints)
			{
				_ = _leftHandBody.CollisionGroup;
				LimpJoints();
				Already_LimpJoints = true;
			}
		}
		else if (Already_LimpJoints)
		{
			StiffJoints();
			Already_LimpJoints = false;
		}
		if (Already_LimpJoints)
		{
			particleEffectBleeding[0].TriggerOffset = _headBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
			PlayerHPBody -= GrabStrangleDamage;
		}
		if (Player_Species == 0f)
		{
			Update_Physics_Daru(gameTime, _world);
		}
		else if (Player_Species == 4f)
		{
			Update_Physics_Ernest(gameTime, _world);
		}
		else if (Player_Species == 1f)
		{
			for (int i = 0; i < Oscar_Update_Speed; i++)
			{
				Update_Physics_Oscar(gameTime, _world);
			}
		}
		else if (Player_Species == 2f)
		{
			Update_Physics_Rick(gameTime, _world);
		}
		else if (Player_Species == 3f)
		{
			Update_Physics_Vinny(gameTime, _world);
		}
	}

	public void Update_Daru(GameTime gameTime)
	{
		if (Dead)
		{
			DeadTimer++;
			if (DeadTimer > DeadTimerMax)
			{
				DeadTimer = DeadTimerMax;
			}
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectUnconcious.Update(deltaSeconds);
			particleEffectSpirit.Update(deltaSeconds);
			particleEffectTeleFog.Update(deltaSeconds);
		}
		if (Unconscious)
		{
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds2);
			_SightON = false;
			particleEffectUnconcious[0].TriggerOffset = _headBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectUnconcious.Trigger(new Vector2(0f, 0f));
			_ = gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleed.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectTeleFog.Update(deltaSeconds2);
		}
		if (Alive & !Unconscious)
		{
			float deltaSeconds3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectUnconcious.Update(deltaSeconds3);
			particleEffectTeleFog.Update(deltaSeconds3);
			particleEffectBleed.Update(deltaSeconds3);
			particleEffectBleeding.Update(deltaSeconds3);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 0.10000000149011612)
			{
				PlayerMana += ManaGainRate;
				if (PlayerMana > ManaMax)
				{
					PlayerMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			if (gameTime.TotalGameTime.TotalSeconds - HpTime > 0.10000000149011612)
			{
				PlayerHPBody += HpGainRate;
				if (PlayerHPBody > PlayerHPBodyMax)
				{
					PlayerHPBody = PlayerHPBodyMax;
				}
				HpTime = gameTime.TotalGameTime.TotalSeconds;
			}
		}
		GetInput2(gameTime);
		if (Dead)
		{
			Alive = false;
		}
	}

	public void Update_Ernest(GameTime gameTime)
	{
		if (Dead)
		{
			DeadTimer++;
			if (DeadTimer > DeadTimerMax)
			{
				DeadTimer = DeadTimerMax;
			}
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectUnconcious.Update(deltaSeconds);
			particleEffectSpirit.Update(deltaSeconds);
			particleEffectTeleFog.Update(deltaSeconds);
		}
		if (Unconscious)
		{
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds2);
			_SightON = false;
			particleEffectUnconcious[0].TriggerOffset = _headBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectUnconcious.Trigger(new Vector2(0f, 0f));
			_ = gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleed.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectTeleFog.Update(deltaSeconds2);
			HealEffect.Update(deltaSeconds2);
		}
		if (Alive & !Unconscious)
		{
			float deltaSeconds3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectUnconcious.Update(deltaSeconds3);
			particleEffectTeleFog.Update(deltaSeconds3);
			particleEffectBleed.Update(deltaSeconds3);
			particleEffectBleeding.Update(deltaSeconds3);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 0.10000000149011612)
			{
				PlayerMana += ManaGainRate;
				if (PlayerMana > ManaMax)
				{
					PlayerMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			if (gameTime.TotalGameTime.TotalSeconds - HpTime > 0.10000000149011612)
			{
				PlayerHPBody += HpGainRate;
				if (PlayerHPBody > PlayerHPBodyMax)
				{
					PlayerHPBody = PlayerHPBodyMax;
				}
				HpTime = gameTime.TotalGameTime.TotalSeconds;
			}
		}
		GetInput2(gameTime);
		if (Dead)
		{
			Alive = false;
		}
	}

	public void Update_Oscar(GameTime gameTime)
	{
		if (Dead)
		{
			DeadTimer++;
			if (DeadTimer > DeadTimerMax)
			{
				DeadTimer = DeadTimerMax;
			}
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectKineticShield.Update(deltaSeconds);
			particleEffectCannonBallEx.Update(deltaSeconds);
			particleEffectKineticEx.Update(deltaSeconds);
			particleEffectUnconcious.Update(deltaSeconds);
			particleEffectSpirit.Update(deltaSeconds);
			particleEffectTeleFog.Update(deltaSeconds);
			HealEffect.Update(deltaSeconds);
		}
		if (Unconscious)
		{
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectKineticShield.Update(deltaSeconds2);
			particleEffectKineticEx.Update(deltaSeconds2);
			particleEffectBleeding.Update(deltaSeconds2);
			particleEffectCannonBallEx.Update(deltaSeconds2);
			_SightON = false;
			particleEffectUnconcious[0].TriggerOffset = _headBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectUnconcious.Trigger(new Vector2(0f, 0f));
			_ = gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleed.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectTeleFog.Update(deltaSeconds2);
			HealEffect.Update(deltaSeconds2);
		}
		if (Alive & !Unconscious)
		{
			float deltaSeconds3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectKineticShield.Update(deltaSeconds3);
			particleEffectKineticEx.Update(deltaSeconds3);
			particleEffectUnconcious.Update(deltaSeconds3);
			particleEffectTeleFog.Update(deltaSeconds3);
			particleEffectBleed.Update(deltaSeconds3);
			particleEffectBleeding.Update(deltaSeconds3);
			particleEffectCannonBallEx.Update(deltaSeconds3);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 0.10000000149011612)
			{
				PlayerMana += ManaGainRate;
				if (PlayerMana > ManaMax)
				{
					PlayerMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			if (gameTime.TotalGameTime.TotalSeconds - HpTime > 0.10000000149011612)
			{
				PlayerHPBody += HpGainRate;
				if (PlayerHPBody > PlayerHPBodyMax)
				{
					PlayerHPBody = PlayerHPBodyMax;
				}
				HpTime = gameTime.TotalGameTime.TotalSeconds;
			}
		}
		GetInput2(gameTime);
		if (Dead)
		{
			Alive = false;
		}
	}

	public void Update_Rick(GameTime gameTime)
	{
		if (Dead)
		{
			DeadTimer++;
			if (DeadTimer > DeadTimerMax)
			{
				DeadTimer = DeadTimerMax;
			}
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectKineticShield.Update(deltaSeconds);
			particleEffectCannonBallEx.Update(deltaSeconds);
			particleEffectUnconcious.Update(deltaSeconds);
			particleEffectSpirit.Update(deltaSeconds);
			particleEffectTeleFog.Update(deltaSeconds);
			HealEffect.Update(deltaSeconds);
		}
		if (Unconscious)
		{
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectKineticShield.Update(deltaSeconds2);
			particleEffectCannonBallEx.Update(deltaSeconds2);
			particleEffectBleeding.Update(deltaSeconds2);
			_SightON = false;
			particleEffectUnconcious[0].TriggerOffset = _headBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectUnconcious.Trigger(new Vector2(0f, 0f));
			_ = gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleed.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectTeleFog.Update(deltaSeconds2);
			HealEffect.Update(deltaSeconds2);
		}
		if (Alive & !Unconscious)
		{
			float deltaSeconds3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectKineticShield.Update(deltaSeconds3);
			particleEffectCannonBallEx.Update(deltaSeconds3);
			particleEffectUnconcious.Update(deltaSeconds3);
			particleEffectTeleFog.Update(deltaSeconds3);
			particleEffectBleed.Update(deltaSeconds3);
			particleEffectBleeding.Update(deltaSeconds3);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 0.10000000149011612)
			{
				PlayerMana += ManaGainRate;
				if (PlayerMana > ManaMax)
				{
					PlayerMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			if (gameTime.TotalGameTime.TotalSeconds - HpTime > 0.10000000149011612)
			{
				PlayerHPBody += HpGainRate;
				if (PlayerHPBody > PlayerHPBodyMax)
				{
					PlayerHPBody = PlayerHPBodyMax;
				}
				HpTime = gameTime.TotalGameTime.TotalSeconds;
			}
		}
		GetInput2(gameTime);
		if (Dead)
		{
			Alive = false;
		}
	}

	public void Update_Vinny(GameTime gameTime)
	{
		if (Dead)
		{
			DeadTimer++;
			if (DeadTimer > DeadTimerMax)
			{
				DeadTimer = DeadTimerMax;
			}
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectKineticShield.Update(deltaSeconds);
			particleEffectUnconcious.Update(deltaSeconds);
			particleEffectSpirit.Update(deltaSeconds);
			particleEffectTeleFog.Update(deltaSeconds);
			HealEffect.Update(deltaSeconds);
		}
		if (Unconscious)
		{
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectKineticShield.Update(deltaSeconds2);
			particleEffectBleeding.Update(deltaSeconds2);
			_SightON = false;
			particleEffectUnconcious[0].TriggerOffset = _headBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectUnconcious.Trigger(new Vector2(0f, 0f));
			_ = gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleed.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectTeleFog.Update(deltaSeconds2);
			HealEffect.Update(deltaSeconds2);
		}
		if (Alive & !Unconscious)
		{
			if (TelekinesisHitSomthing && TelekinisisBodyHit != null && TelekinisisBodyHit.Body != null)
			{
				particleEffectTeleFog[0].TriggerOffset = TelekinisisBodyHit.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectTeleFog.Trigger(new Vector2(0f, 0f));
			}
			if (IsFlying)
			{
				particleEffectTeleFog[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
				particleEffectTeleFog.Trigger(new Vector2(0f, 0f));
			}
			float deltaSeconds3 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectKineticShield.Update(deltaSeconds3);
			particleEffectUnconcious.Update(deltaSeconds3);
			particleEffectTeleFog.Update(deltaSeconds3);
			particleEffectBleed.Update(deltaSeconds3);
			particleEffectBleeding.Update(deltaSeconds3);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 0.10000000149011612)
			{
				PlayerMana += ManaGainRate;
				if (PlayerMana > ManaMax)
				{
					PlayerMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			if (gameTime.TotalGameTime.TotalSeconds - HpTime > 0.10000000149011612)
			{
				PlayerHPBody += HpGainRate;
				if (PlayerHPBody > PlayerHPBodyMax)
				{
					PlayerHPBody = PlayerHPBodyMax;
				}
				HpTime = gameTime.TotalGameTime.TotalSeconds;
			}
		}
		GetInput2(gameTime);
		if (Dead)
		{
			Alive = false;
		}
	}

	public void Update(GameTime gameTime)
	{
		if (!DeadByBounds)
		{
			UpdateKillZone();
		}
		if (Player_Species == 0f)
		{
			Update_Daru(gameTime);
		}
		else if (Player_Species == 4f)
		{
			Update_Ernest(gameTime);
		}
		else if (Player_Species == 1f)
		{
			Update_Oscar(gameTime);
		}
		else if (Player_Species == 2f)
		{
			Update_Rick(gameTime);
		}
		else if (Player_Species == 3f)
		{
			Update_Vinny(gameTime);
		}
	}

	public void ActiveAll_False(World _world)
	{
		if (_bodyBody != null && _bodyBody.Body != null && _bodyBody.Body.Active)
		{
			_bodyBody.Body.Active = false;
		}
		if (_headBody != null && _headBody.Body != null && _headBody.Body.Active)
		{
			_headBody.Body.Active = false;
		}
		if (_leftUpperArmBody != null && _leftUpperArmBody.Body != null && _leftUpperArmBody.Body.Active)
		{
			_leftUpperArmBody.Body.Active = false;
		}
		if (_rightUpperArmBody != null && _rightUpperArmBody.Body != null && _rightUpperArmBody.Body.Active)
		{
			_rightUpperArmBody.Body.Active = false;
		}
		if (_leftHandBody != null && _leftHandBody.Body != null && _leftHandBody.Body.Active)
		{
			_leftHandBody.Body.Active = false;
		}
		if (_rightHandBody != null && _rightHandBody.Body != null && _rightHandBody.Body.Active)
		{
			_rightHandBody.Body.Active = false;
		}
		if (_leftHandBody_Claw != null && _leftHandBody_Claw.Body != null && _leftHandBody_Claw.Body.Active)
		{
			_leftHandBody_Claw.Body.Active = false;
		}
		if (_rightHandBody_Claw != null && _rightHandBody_Claw.Body != null && _rightHandBody_Claw.Body.Active)
		{
			_rightHandBody_Claw.Body.Active = false;
		}
		if (_leftThighBody != null && _leftThighBody.Body != null && _leftThighBody.Body.Active)
		{
			_leftThighBody.Body.Active = false;
		}
		if (_rightThighBody != null && _rightThighBody.Body != null && _rightThighBody.Body.Active)
		{
			_rightThighBody.Body.Active = false;
		}
		if (_kineticShields != null && _kineticShields.Body != null && _kineticShields.Body.Active)
		{
			_kineticShields.Body.Active = false;
		}
	}

	public void UpdateKillZone()
	{
		if (_bodyBody.Body.Position.X + level.KillBoundsMargin * 0.2f < level.KillBounds_Left_Side * 0.2f)
		{
			level.PlayerPosition_Vec += new Vector2(100f, 0f);
			DeadByBounds = true;
			Dead = true;
		}
		if (_bodyBody.Body.Position.X - level.KillBoundsMargin * 0.2f > level.KillBounds_Right_Side * 0.2f)
		{
			level.PlayerPosition_Vec += new Vector2(-100f, 0f);
			DeadByBounds = true;
			Dead = true;
		}
		_ = _bodyBody.Body.Position.Y + level.KillBoundsMargin * 0.2f;
		_ = level.KillBounds_Upper_Side * 0.2f;
		_ = _bodyBody.Body.Position.Y - level.KillBoundsMargin * 0.2f;
		_ = level.KillBounds_Lower_Side * 0.2f;
	}

	public void Update_Injuries(GameTime gameTime, World _world)
	{
		if (Player_Species == 0f)
		{
			Update_Injuries_Daru(gameTime, _world);
		}
		else if (Player_Species == 4f)
		{
			Update_Injuries_Ernest(gameTime, _world);
		}
		else if (Player_Species == 1f)
		{
			Update_Injuries_Oscar(gameTime, _world);
		}
		else if (Player_Species == 2f)
		{
			Update_Injuries_Rick(gameTime, _world);
		}
		else if (Player_Species == 3f)
		{
			Update_Injuries_Vinny(gameTime, _world);
		}
	}

	public void Update_Injuries_Daru(GameTime gameTime, World _world)
	{
		if (PlayerHPBody < 4f)
		{
			Dead = true;
			LimpJoints();
			PlayerHPBody = 0f;
		}
		for (int i = 0; i < MaxHP; i++)
		{
		}
		if (NeckJoint != null && NeckJoint.JointSpeed > MaxJointForce * 3f)
		{
			SoundGotHit.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0.75f, 0f);
			PlayerHPBody -= (NeckJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			_ = Unconscious;
		}
		if (LeftLegSevered && RightLegSevered && LeftArmSevered && RightArmSevered)
		{
			SoundImpailed.Play(level.mainGame.Sound_Effect_Volume, 1f, 0f);
			PlayerHPBody -= 0.1f;
			particleEffectBleeding[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
			if ((int)_bodyAngleJoint.UserData != 5)
			{
				_bodyAngleJoint.UserData = 5;
				_world.RemoveJoint(_bodyAngleJoint);
			}
		}
		else if ((int)_bodyAngleJoint.UserData == 5)
		{
			_bodyAngleJoint.UserData = 1;
			level._world.AddJoint(_bodyAngleJoint);
		}
		_ = PlayerHPBody;
		_ = PlayerHPBodyMax;
	}

	public void Update_Injuries_Ernest(GameTime gameTime, World _world)
	{
		if (PlayerHPBody < 4f)
		{
			Dead = true;
			LimpJoints();
			PlayerHPBody = 0f;
		}
		for (int i = 0; i < MaxHP; i++)
		{
		}
		if (NeckJoint != null && NeckJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (NeckJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			_ = Unconscious;
		}
		if (_leftUpperArmJoint != null && _leftUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_leftUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _leftUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightUpperArmJoint != null && _rightUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_rightUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _rightUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_leftThighJoint != null && _leftThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_leftThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _leftThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightThighJoint != null && _rightThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_rightThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _rightThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (LeftLegSevered && RightLegSevered && LeftArmSevered && RightArmSevered)
		{
			PlayerHPBody -= 0.1f;
			particleEffectBleeding[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
			if ((int)_bodyAngleJoint.UserData != 5)
			{
				_bodyAngleJoint.UserData = 5;
				_world.RemoveJoint(_bodyAngleJoint);
			}
		}
		else if ((int)_bodyAngleJoint.UserData == 5)
		{
			_bodyAngleJoint.UserData = 1;
			level._world.AddJoint(_bodyAngleJoint);
		}
		_ = PlayerHPBody;
		_ = PlayerHPBodyMax;
	}

	public void Update_Injuries_Oscar(GameTime gameTime, World _world)
	{
		if (PlayerHPBody < 4f)
		{
			Dead = true;
			LimpJoints();
			PlayerHPBody = 0f;
		}
		for (int i = 0; i < MaxHP; i++)
		{
		}
		if (NeckJoint != null && NeckJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (NeckJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			_ = Unconscious;
		}
		if (_leftUpperArmJoint != null && _leftUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_leftUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _leftUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightUpperArmJoint != null && _rightUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_rightUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _rightUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_leftThighJoint != null && _leftThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_leftThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _leftThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightThighJoint != null && _rightThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_rightThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _rightThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (LeftLegSevered && RightLegSevered && LeftArmSevered && RightArmSevered)
		{
			PlayerHPBody -= 0.1f;
			particleEffectBleeding[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
			if ((int)_bodyAngleJoint.UserData != 5)
			{
				_bodyAngleJoint.UserData = 5;
				_world.RemoveJoint(_bodyAngleJoint);
			}
		}
		else if ((int)_bodyAngleJoint.UserData == 5)
		{
			_bodyAngleJoint.UserData = 1;
			level._world.AddJoint(_bodyAngleJoint);
		}
		_ = PlayerHPBody;
		_ = PlayerHPBodyMax;
	}

	public void Update_Injuries_Rick(GameTime gameTime, World _world)
	{
		if (PlayerHPBody < 4f)
		{
			Dead = true;
			LimpJoints();
			PlayerHPBody = 0f;
		}
		for (int i = 0; i < MaxHP; i++)
		{
		}
		if (NeckJoint != null && NeckJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (NeckJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			_ = Unconscious;
		}
		if (_leftUpperArmJoint != null && _leftUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_leftUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _leftUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightUpperArmJoint != null && _rightUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_rightUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _rightUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_leftThighJoint != null && _leftThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_leftThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _leftThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightThighJoint != null && _rightThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_rightThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _rightThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (LeftLegSevered && RightLegSevered && LeftArmSevered && RightArmSevered)
		{
			PlayerHPBody -= 0.1f;
			particleEffectBleeding[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
			if ((int)_bodyAngleJoint.UserData != 5)
			{
				_bodyAngleJoint.UserData = 5;
				_world.RemoveJoint(_bodyAngleJoint);
			}
		}
		else if ((int)_bodyAngleJoint.UserData == 5)
		{
			_bodyAngleJoint.UserData = 1;
			level._world.AddJoint(_bodyAngleJoint);
		}
		_ = PlayerHPBody;
		_ = PlayerHPBodyMax;
	}

	public void Update_Injuries_Vinny(GameTime gameTime, World _world)
	{
		if (PlayerHPBody < 4f)
		{
			Dead = true;
			LimpJoints();
			PlayerHPBody = 0f;
		}
		for (int i = 0; i < MaxHP; i++)
		{
		}
		if (NeckJoint != null && NeckJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (NeckJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
			_ = Unconscious;
		}
		if (_leftUpperArmJoint != null && _leftUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_leftUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _leftUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightUpperArmJoint != null && _rightUpperArmJoint.JointSpeed > MaxJointForce * 2f)
		{
			PlayerHPBody -= (_rightUpperArmJoint.JointSpeed - MaxJointForce) / 20f;
			particleEffectBleed[0].TriggerOffset = _rightUpperArmBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_leftThighJoint != null && _leftThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_leftThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _leftThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (_rightThighJoint != null && _rightThighJoint.JointSpeed > MaxJointForce * 3f)
		{
			PlayerHPBody -= (_rightThighJoint.JointSpeed - MaxJointForce) / 10f;
			particleEffectBleed[0].TriggerOffset = _rightThighBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleed[0].ReleaseSpeed.Variation = 100f;
			particleEffectBleed.Trigger(new Vector2(0f, 0f));
			particleEffectBleed[0].ReleaseSpeed.Variation = 35f;
		}
		if (LeftLegSevered && RightLegSevered && LeftArmSevered && RightArmSevered)
		{
			PlayerHPBody -= 0.1f;
			particleEffectBleeding[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			particleEffectBleeding.Trigger(new Vector2(0f, 0f));
			if ((int)_bodyAngleJoint.UserData != 5)
			{
				_bodyAngleJoint.UserData = 5;
				_world.RemoveJoint(_bodyAngleJoint);
			}
		}
		else if ((int)_bodyAngleJoint.UserData == 5)
		{
			_bodyAngleJoint.UserData = 1;
			level._world.AddJoint(_bodyAngleJoint);
		}
		_ = PlayerHPBody;
		_ = PlayerHPBodyMax;
	}

	public void CheckRespawn()
	{
		GamePadState state = GamePad.GetState(playerIndex);
		Keyboard.GetState();
		if (state.IsButtonDown(Buttons.Y))
		{
			Respawn = true;
		}
		if (Respawn)
		{
			if (Player1Index)
			{
				Level.RespawnPlayer1();
			}
			if (Player2Index)
			{
				Level.RespawnPlayer2();
			}
			Respawn = false;
		}
	}

	public void GrabWithLeftHand(World _world)
	{
		if (_leftHandGrabOtherFixture != null)
		{
			if (_leftHandGrabOtherFixture.Body != null && (int)_leftHandGrabOtherFixture.Body.UserData != 1)
			{
				_leftHandGrabJoint = new RevoluteJoint(_leftHandBody.Body, _leftHandGrabOtherFixture.Body, new Vector2(0f, 0f), _leftHandGrabOtherFixture.Body.GetLocalPoint(_leftHandBody.Body.Position));
				_world.AddJoint(_leftHandGrabJoint);
				GrabWithLeftHandBool = true;
				_leftHandGrabOtherFixture_CollisionGroup = _leftHandGrabOtherFixture.CollisionGroup;
			}
		}
		else
		{
			GrabWithLeftHandBool = false;
		}
	}

	public void GrabWithRightHand(World _world)
	{
		if (_rightHandGrabOtherFixture != null)
		{
			if (_rightHandGrabOtherFixture.Body != null && (int)_rightHandGrabOtherFixture.Body.UserData != 0 && (int)_rightHandGrabOtherFixture.Body.UserData != 1 && (int)_rightHandGrabOtherFixture.Body.UserData != 1 && (int)_rightHandGrabOtherFixture.Body.UserData != 90)
			{
				_rightHandGrabJoint = new WeldJoint(_rightHandBody.Body, _rightHandGrabOtherFixture.Body, new Vector2(0f, 0f), _rightHandGrabOtherFixture.Body.GetLocalPoint(_rightHandBody.Body.Position));
				_world.AddJoint(_rightHandGrabJoint);
				GrabWithRightHandBool = true;
				_rightHandGrabOtherFixture_CollisionGroup = _rightHandGrabOtherFixture.CollisionGroup;
			}
		}
		else
		{
			GrabWithRightHandBool = false;
		}
	}

	private void GetDieInput(World _world)
	{
		if (Keyboard.GetState().IsKeyDown(Keys.Delete))
		{
			Dead = true;
			RemoveAllJoints(_world);
			Dead = true;
			CollisionCatagoryTo99();
		}
	}

	private void GetInput2_Physics_Daru(World _world, GameTime gameTime)
	{
		if (Dead)
		{
			if (!ReSpawn)
			{
				if (!Spirit_Walking && gamePadState_Buttons_Y_ButtonState_Pressed)
				{
					Spirit_Walking = true;
					Spirit_Walking_Time_OldGameTime = gameTime.TotalGameTime.TotalSeconds;
					PlayerPosition = _headBody.Body.Position;
				}
				if (Spirit_Walking)
				{
					float num = 1f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					if (gamePadState_DPad_Left_ButtonState_Pressed)
					{
						num2 += num;
					}
					if (gamePadState_DPad_Right_ButtonState_Pressed)
					{
						num3 += num;
					}
					if (gamePadState_DPad_Up_ButtonState_Pressed)
					{
						num4 += num;
					}
					if (gamePadState_DPad_Down_ButtonState_Pressed)
					{
						num5 += num;
					}
					float num6 = 1f;
					Vector2 vector = new Vector2(0f - num2 + num3, 0f - num4 + num5);
					PlayerPosition = PlayerPosition + vector + new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y) * new Vector2(num6, 0f - num6);
					if (PlayerPosition.X < -2000f * level.MasterScale)
					{
						PlayerPosition.X = -2000f * level.MasterScale;
					}
					if (PlayerPosition.X > 5000f * level.MasterScale)
					{
						PlayerPosition.X = 5000f * level.MasterScale;
					}
					if (PlayerPosition.Y > (float)GroundPlainHeight * level.MasterScale)
					{
						PlayerPosition.Y = (float)GroundPlainHeight * level.MasterScale;
					}
					particleEffectSpirit[0].TriggerOffset = PlayerPosition * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectSpirit.Trigger(new Vector2(0f, 0f));
				}
			}
			if (Spirit_Walking && gameTime.TotalGameTime.TotalSeconds - (double)Spirit_Walking_Time > Spirit_Walking_Time_OldGameTime)
			{
				ReSpawn = true;
			}
		}
		else
		{
			if (Unconscious)
			{
				return;
			}
			Get_Arm_Movements();
			if (!Player1Index && !Player2Index && !Player3Index && !Player4Index)
			{
				return;
			}
			if (!Frozen)
			{
				LeftShoulderTriggerState = gamePadState_Triggers_Left > 0.1f;
				if (!LeftArmSevered)
				{
					if (LeftShoulderTriggerState)
					{
						if (PlayerMana > KineticShieldManaCost)
						{
							Shield_ON_Daru(_world);
							level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartMaceVibrationSpeed);
						}
						else
						{
							Shield_OFF_Daru(_world);
						}
					}
					else
					{
						Shield_OFF_Daru(_world);
					}
					if (gamePadState_Buttons_LeftShoulder_ButtonState_Pressed)
					{
						_ = PlayerMana;
						_ = KineticShieldManaCost;
					}
				}
			}
			if (!RightArmSevered)
			{
				RightShoulderTriggerState = gamePadState_Triggers_Right > 0.1f;
				if (!RightShoulderTriggerStateToggle && RightShoulderTriggerState && PlayerMana > DartHarpoonManaCost)
				{
					CreateHarpoon_Daru(_world);
					level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartHarpoonVibrationSpeed);
				}
				RightShoulderTriggerStateToggle = RightShoulderTriggerState;
				if (gamePadState_Buttons_RightShoulder_ButtonState_Pressed)
				{
					if (BoneDartRepeater > BoneDartRepeaterMax)
					{
						CreateBoneSaw_Daru(_world);
						level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartBoneVibrationSpeed);
						BoneDartRepeater = 0f;
					}
					else
					{
						BoneDartRepeater++;
					}
				}
			}
			else
			{
				_SightON = false;
			}
			if (gamePadState_Buttons_X_ButtonState_Pressed)
			{
				RunLimit = 20f;
				WasSprinting = true;
			}
			else
			{
				if (WasSprinting)
				{
					Clear_X_Forces();
				}
				WasSprinting = false;
				RunLimit = 0.1f;
			}
			if (gamePadState_DPad_Left_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X < -0.5f)
			{
				if (DirectionRight && WasSprinting)
				{
					Clear_X_Forces();
				}
				DirectionLeft = true;
				DirectionRight = false;
				_bodyAngleJoint.TargetAngle = BodyLeanAngle;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = true;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = true;
				}
				if (movementX > (0f - RunLimit) * 0.2f)
				{
					movementX += -0.2f;
				}
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorSpeed = -40f * (0f - movementX);
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorSpeed = -40f * (0f - movementX);
				}
			}
			else if (gamePadState_DPad_Right_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X > 0.5f)
			{
				if (DirectionLeft && WasSprinting)
				{
					Clear_X_Forces();
				}
				DirectionLeft = false;
				DirectionRight = true;
				_bodyAngleJoint.TargetAngle = 0f - BodyLeanAngle;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = true;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = true;
				}
				if (movementX < RunLimit * 0.2f)
				{
					movementX += 0.2f;
				}
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorSpeed = 40f * movementX;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorSpeed = 40f * movementX;
				}
			}
			else
			{
				if (WasSprinting)
				{
					Clear_X_Forces();
				}
				_bodyAngleJoint.TargetAngle = -0.01f;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = false;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = false;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorEnabled = false;
				}
				if (movementX > 1f)
				{
					movementX -= 0.1f;
				}
				else if (movementX < -1f)
				{
					movementX += 0.1f;
				}
				else
				{
					movementX = 0f;
				}
			}
			if ((_leftThighJoint != null) & (_rightThighJoint != null))
			{
				if (LeftFootIsOnGround)
				{
					if (!LeftLegSevered)
					{
						if (gamePadState_Buttons_A_ButtonState_Pressed)
						{
							isJumping = true;
							LeftFootIsOnGround = false;
						}
						if (gamePadState_Buttons_A_ButtonState_Released)
						{
							isJumping = false;
						}
					}
				}
				else if (RightFootIsOnGround && !RightLegSevered)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						isJumping = true;
						RightFootIsOnGround = false;
					}
					if (gamePadState_Buttons_A_ButtonState_Released)
					{
						isJumping = false;
					}
				}
				if (Jump)
				{
					if (!wasJumpPressed && isJumping)
					{
						StandStateIndex = 0f;
						Crouch = false;
						Crawl = false;
						Jump = true;
						SoundJump.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						JumpStart = true;
						float num7 = 3f;
						if (DirectionLeft)
						{
							movementY = -500f * JumpStrength * 0.2f + movementX * num7;
						}
						else if (DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f + (0f - movementX) * num7;
						}
						else if (!DirectionLeft && !DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f * ((0f - movementX) * num7);
						}
					}
					wasJumpPressed = isJumping;
				}
				if (JumpStart)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						JumpDuration++;
						if (JumpDuration > JumpDurationMax)
						{
							JumpDuration = JumpDurationMax;
						}
					}
					else
					{
						JumpDuration = 1;
					}
					JumpTime++;
					if (JumpTime > JumpDuration)
					{
						if (movementY < 1f)
						{
							movementY = 2f;
						}
						Vector2 linearVelocityFromLocalPoint = _bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(0f, 0f));
						movementY /= 0.75f;
						if (linearVelocityFromLocalPoint.Y < 100f)
						{
							JumpStart = false;
							JumpTime = 0;
							movementY = 0f;
						}
					}
				}
			}
			PlayerMovement = new Vector2(movementX * 0.2f, movementY * 0.2f);
			_headBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _headBody.Body.Position);
			_headBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _headBody.Body.Position);
			if (!LeftArmSevered)
			{
				_leftUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftUpperArmBody.Body.Position);
				_leftUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftUpperArmBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightUpperArmBody.Body.Position);
				_rightUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightUpperArmBody.Body.Position);
			}
			if (!LeftLegSevered)
			{
				_leftThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftThighBody.Body.Position);
			}
			if (!RightLegSevered)
			{
				_rightThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightThighBody.Body.Position);
			}
			if (!LeftArmSevered)
			{
				_leftHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftHandBody.Body.Position);
				_leftHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftHandBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightHandBody.Body.Position);
				_rightHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightHandBody.Body.Position);
			}
			PlayerPosition = _headBody.Body.Position;
		}
	}

	private void GetInput2_Physics_Ernest(World _world, GameTime gameTime)
	{
		if (Dead)
		{
			if (!ReSpawn)
			{
				if (!Spirit_Walking && gamePadState_Buttons_Y_ButtonState_Pressed)
				{
					Spirit_Walking = true;
					Spirit_Walking_Time_OldGameTime = gameTime.TotalGameTime.TotalSeconds;
					PlayerPosition = _headBody.Body.Position;
				}
				if (Spirit_Walking)
				{
					float num = 1f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					if (gamePadState_DPad_Left_ButtonState_Pressed)
					{
						num2 += num;
					}
					if (gamePadState_DPad_Right_ButtonState_Pressed)
					{
						num3 += num;
					}
					if (gamePadState_DPad_Up_ButtonState_Pressed)
					{
						num4 += num;
					}
					if (gamePadState_DPad_Down_ButtonState_Pressed)
					{
						num5 += num;
					}
					float num6 = 1f;
					Vector2 vector = new Vector2(0f - num2 + num3, 0f - num4 + num5);
					PlayerPosition = PlayerPosition + vector + new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y) * new Vector2(num6, 0f - num6);
					if (PlayerPosition.X < -2000f * level.MasterScale)
					{
						PlayerPosition.X = -2000f * level.MasterScale;
					}
					if (PlayerPosition.X > 5000f * level.MasterScale)
					{
						PlayerPosition.X = 5000f * level.MasterScale;
					}
					if (PlayerPosition.Y > (float)GroundPlainHeight * level.MasterScale)
					{
						PlayerPosition.Y = (float)GroundPlainHeight * level.MasterScale;
					}
					particleEffectSpirit[0].TriggerOffset = PlayerPosition * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectSpirit.Trigger(new Vector2(0f, 0f));
				}
			}
			if (Spirit_Walking && gameTime.TotalGameTime.TotalSeconds - (double)Spirit_Walking_Time > Spirit_Walking_Time_OldGameTime)
			{
				ReSpawn = true;
			}
		}
		else
		{
			if (Unconscious)
			{
				return;
			}
			Get_Arm_Movements();
			if (!Player1Index && !Player2Index && !Player3Index && !Player4Index)
			{
				return;
			}
			if (!Frozen)
			{
				LeftShoulderTriggerState = gamePadState_Triggers_Left > 0.1f;
				if (gamePadState_Triggers_Left < 0.1f)
				{
					Fly_All_Out = false;
				}
				if (!LeftArmSevered)
				{
					if (LeftShoulderTriggerState)
					{
						if (PlayerMana > ClimbManaCost)
						{
							Climb_ON_Ernest(_world);
						}
						else
						{
							Climb_OFF_Ernest(_world);
						}
					}
					else
					{
						Climb_OFF_Ernest(_world);
					}
					if (gamePadState_Buttons_LeftShoulder_ButtonState_Pressed)
					{
						_ = PlayerMana;
						_ = KineticShieldManaCost;
					}
				}
			}
			if (!RightArmSevered)
			{
				RightShoulderTriggerState = gamePadState_Triggers_Right > 0.1f;
				if (!RightShoulderTriggerStateToggle && RightShoulderTriggerState && PlayerMana > DartBoneManaCost)
				{
					CreateBurr_Ernest(_world);
					level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartBurrVibrationSpeed);
				}
				RightShoulderTriggerStateToggle = RightShoulderTriggerState;
				if (gamePadState_Buttons_RightShoulder_ButtonState_Pressed)
				{
					Weapon_ON_Ernest(_world);
					level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartMaceVibrationSpeed);
				}
				else
				{
					Weapon_OFF_Ernest(_world);
				}
			}
			else
			{
				_SightON = false;
			}
			if (!IsClimbing)
			{
				if (gamePadState_Buttons_X_ButtonState_Pressed)
				{
					WasSprinting = true;
					RunLimit = 20f;
				}
				else
				{
					if (WasSprinting)
					{
						Clear_X_Forces();
					}
					WasSprinting = false;
					RunLimit = 0.1f;
				}
				if (gamePadState_DPad_Left_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X < -0.5f)
				{
					if (DirectionRight)
					{
						Clear_X_Forces();
					}
					DirectionLeft = true;
					DirectionRight = false;
					_bodyAngleJoint.TargetAngle = BodyLeanAngle;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX > (0f - RunLimit) * 0.2f)
					{
						movementX += -0.2f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
				}
				else if (gamePadState_DPad_Right_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X > 0.5f)
				{
					if (DirectionLeft)
					{
						Clear_X_Forces();
					}
					DirectionLeft = false;
					DirectionRight = true;
					_bodyAngleJoint.TargetAngle = 0f - BodyLeanAngle;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX < RunLimit * 0.2f)
					{
						movementX += 0.2f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = 40f * movementX;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = 40f * movementX;
					}
				}
				else
				{
					Clear_X_Forces();
					_bodyAngleJoint.TargetAngle = -0.01f;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = false;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = false;
					}
					if (_leftUpperArmJoint != null)
					{
						_leftUpperArmJoint.MotorEnabled = false;
					}
					if (movementX > 1f)
					{
						movementX -= 0.1f;
					}
					else if (movementX < -1f)
					{
						movementX += 0.1f;
					}
					else
					{
						movementX = 0f;
					}
				}
				if ((_leftThighJoint != null) & (_rightThighJoint != null))
				{
					if (LeftFootIsOnGround)
					{
						if (!LeftLegSevered)
						{
							if (gamePadState_Buttons_A_ButtonState_Pressed)
							{
								isJumping = true;
								LeftFootIsOnGround = false;
							}
							if (gamePadState_Buttons_A_ButtonState_Released)
							{
								isJumping = false;
							}
						}
					}
					else if (RightFootIsOnGround && !RightLegSevered)
					{
						if (gamePadState_Buttons_A_ButtonState_Pressed)
						{
							isJumping = true;
							RightFootIsOnGround = false;
						}
						if (gamePadState_Buttons_A_ButtonState_Released)
						{
							isJumping = false;
						}
					}
					if (Jump)
					{
						if (!wasJumpPressed && isJumping)
						{
							StandStateIndex = 0f;
							Crouch = false;
							Crawl = false;
							Jump = true;
							SoundJump.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
							JumpStart = true;
							float num7 = 3f;
							if (DirectionLeft)
							{
								movementY = -500f * JumpStrength * 0.2f + movementX * num7;
							}
							else if (DirectionRight)
							{
								movementY = -500f * JumpStrength * 0.2f + (0f - movementX) * num7;
							}
							else if (!DirectionLeft && !DirectionRight)
							{
								movementY = -500f * JumpStrength * 0.2f * ((0f - movementX) * num7);
							}
						}
						wasJumpPressed = isJumping;
					}
					if (JumpStart)
					{
						if (gamePadState_Buttons_A_ButtonState_Pressed)
						{
							JumpDuration++;
							if (JumpDuration > JumpDurationMax)
							{
								JumpDuration = JumpDurationMax;
							}
						}
						else
						{
							JumpDuration = 1;
						}
						JumpTime++;
						if (JumpTime > JumpDuration)
						{
							if (movementY < 1f)
							{
								movementY = 2f;
							}
							Vector2 linearVelocityFromLocalPoint = _bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(0f, 0f));
							movementY /= 0.75f;
							if (linearVelocityFromLocalPoint.Y < 100f)
							{
								JumpStart = false;
								JumpTime = 0;
								movementY = 0f;
							}
						}
					}
				}
				PlayerMovement = new Vector2(movementX * 0.2f, movementY * 0.2f);
				_headBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _headBody.Body.Position);
				_headBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _headBody.Body.Position);
				if (!LeftArmSevered)
				{
					_leftUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftUpperArmBody.Body.Position);
					_leftUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftUpperArmBody.Body.Position);
				}
				if (!RightArmSevered)
				{
					_rightUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightUpperArmBody.Body.Position);
					_rightUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightUpperArmBody.Body.Position);
				}
				if (!LeftLegSevered)
				{
					_leftThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftThighBody.Body.Position);
				}
				if (!RightLegSevered)
				{
					_rightThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightThighBody.Body.Position);
				}
				if (!LeftArmSevered)
				{
					_leftHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftHandBody.Body.Position);
					_leftHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftHandBody.Body.Position);
				}
				if (!RightArmSevered)
				{
					_rightHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightHandBody.Body.Position);
					_rightHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightHandBody.Body.Position);
				}
			}
			PlayerPosition = _headBody.Body.Position;
		}
	}

	private void GetInput2_Physics_Oscar(World _world, GameTime gameTime)
	{
		if (Dead)
		{
			if (!ReSpawn)
			{
				if (!Spirit_Walking && gamePadState_Buttons_Y_ButtonState_Pressed)
				{
					Spirit_Walking = true;
					Spirit_Walking_Time_OldGameTime = gameTime.TotalGameTime.TotalSeconds;
					PlayerPosition = _headBody.Body.Position;
				}
				if (Spirit_Walking)
				{
					float num = 1f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					if (gamePadState_DPad_Left_ButtonState_Pressed)
					{
						num2 += num;
					}
					if (gamePadState_DPad_Right_ButtonState_Pressed)
					{
						num3 += num;
					}
					if (gamePadState_DPad_Up_ButtonState_Pressed)
					{
						num4 += num;
					}
					if (gamePadState_DPad_Down_ButtonState_Pressed)
					{
						num5 += num;
					}
					float num6 = 1f;
					Vector2 vector = new Vector2(0f - num2 + num3, 0f - num4 + num5);
					PlayerPosition = PlayerPosition + vector + new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y) * new Vector2(num6, 0f - num6);
					if (PlayerPosition.X < -2000f * level.MasterScale)
					{
						PlayerPosition.X = -2000f * level.MasterScale;
					}
					if (PlayerPosition.X > 5000f * level.MasterScale)
					{
						PlayerPosition.X = 5000f * level.MasterScale;
					}
					if (PlayerPosition.Y > (float)GroundPlainHeight * level.MasterScale)
					{
						PlayerPosition.Y = (float)GroundPlainHeight * level.MasterScale;
					}
					particleEffectSpirit[0].TriggerOffset = PlayerPosition * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectSpirit.Trigger(new Vector2(0f, 0f));
				}
			}
			if (Spirit_Walking && gameTime.TotalGameTime.TotalSeconds - (double)Spirit_Walking_Time > Spirit_Walking_Time_OldGameTime)
			{
				ReSpawn = true;
			}
		}
		else
		{
			if (Unconscious)
			{
				return;
			}
			Get_Arm_Movements();
			if (!Player1Index && !Player2Index && !Player3Index && !Player4Index)
			{
				return;
			}
			if (!Frozen)
			{
				LeftShoulderTriggerState = gamePadState_Triggers_Left > 0.1f;
				if (!LeftArmSevered)
				{
					if (PlayerMana > PhaseShiftManaCost)
					{
						if (LeftShoulderTriggerState)
						{
							PlayerMana -= PhaseShiftManaCost;
							PhaseShift = true;
							PhaseShift_ON();
							playerColor = Color.LightBlue;
							playerColor.A = 1;
						}
						else
						{
							PhaseShift = false;
							PhaseShift_OFF();
							playerColor = Color.White;
						}
					}
					else
					{
						PhaseShift = false;
						PhaseShift_OFF();
						playerColor = Color.White;
					}
					if (gamePadState_Buttons_LeftShoulder_ButtonState_Pressed && !(PlayerMana > KineticShieldManaCost))
					{
					}
				}
				else
				{
					SlowTime_OFF_Oscar(_world);
				}
			}
			if (!RightArmSevered)
			{
				RightShoulderTriggerState = gamePadState_Triggers_Right > 0.1f;
				if (!RightShoulderTriggerStateToggle && RightShoulderTriggerState && PlayerMana > DartKineticManaCost)
				{
					CreateEctoBall_Oscar(_world);
					level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartEctoBallVibrationSpeed);
				}
				RightShoulderTriggerStateToggle = RightShoulderTriggerState;
				if (gamePadState_Buttons_RightShoulder_ButtonState_Pressed && !gamePadState_Buttons_RightShoulder_ButtonState_Pressed_State)
				{
					CreateBallLightning_Oscar(_world);
					level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartBallLightningVibrationSpeed);
				}
				gamePadState_Buttons_RightShoulder_ButtonState_Pressed_State = gamePadState_Buttons_RightShoulder_ButtonState_Pressed;
			}
			else
			{
				_SightON = false;
			}
			if (gamePadState_Buttons_X_ButtonState_Pressed)
			{
				RunLimit = 20f;
				WasSprinting = true;
			}
			else
			{
				if (WasSprinting)
				{
					Clear_X_Forces();
				}
				WasSprinting = false;
				RunLimit = 0.1f;
			}
			if (gamePadState_DPad_Left_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X < -0.5f)
			{
				if (DirectionRight)
				{
					Clear_X_Forces();
				}
				DirectionLeft = true;
				DirectionRight = false;
				if (PhaseShift)
				{
					XLock = true;
					RunLimit = 4f;
					_bodyAngleJoint.TargetAngle = -0.5f;
					_leftThighBody.Body.LinearDamping = 1f;
					_rightThighBody.Body.LinearDamping = 1f;
					_leftUpperArmBody.Body.LinearDamping = 1f;
					_rightUpperArmBody.Body.LinearDamping = 1f;
					if (movementX > (0f - RunLimit) * 0.2f)
					{
						movementX += -0.4f;
					}
				}
				else
				{
					XLock = false;
					_bodyAngleJoint.TargetAngle = BodyLeanAngle;
					_leftThighBody.Body.LinearDamping = 0f;
					_rightThighBody.Body.LinearDamping = 0f;
					_leftUpperArmBody.Body.LinearDamping = 0f;
					_rightUpperArmBody.Body.LinearDamping = 0f;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX > (0f - RunLimit) * 0.2f)
					{
						movementX += -0.2f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
				}
			}
			else if (gamePadState_DPad_Right_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X > 0.5f)
			{
				if (DirectionLeft)
				{
					Clear_X_Forces();
				}
				DirectionLeft = false;
				DirectionRight = true;
				if (PhaseShift)
				{
					XLock = true;
					RunLimit = 4f;
					_bodyAngleJoint.TargetAngle = 0.5f;
					_leftThighBody.Body.LinearDamping = 1f;
					_rightThighBody.Body.LinearDamping = 1f;
					_leftUpperArmBody.Body.LinearDamping = 1f;
					_rightUpperArmBody.Body.LinearDamping = 1f;
					if (movementX < RunLimit * 0.2f)
					{
						movementX += 0.05f;
					}
				}
				else
				{
					XLock = false;
					_bodyAngleJoint.TargetAngle = 0f - BodyLeanAngle;
					_leftThighBody.Body.LinearDamping = 0f;
					_rightThighBody.Body.LinearDamping = 0f;
					_leftUpperArmBody.Body.LinearDamping = 0f;
					_rightUpperArmBody.Body.LinearDamping = 0f;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX < RunLimit * 0.2f)
					{
						movementX += 0.2f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = 40f * movementX;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = 40f * movementX;
					}
				}
			}
			else
			{
				Clear_X_Forces();
				_bodyAngleJoint.TargetAngle = -0.01f;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = false;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = false;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorEnabled = false;
				}
				if (movementX > 1f)
				{
					movementX -= 0.1f;
				}
				else if (movementX < -1f)
				{
					movementX += 0.1f;
				}
				else
				{
					movementX = 0f;
				}
			}
			if ((_leftThighJoint != null) & (_rightThighJoint != null))
			{
				if (LeftFootIsOnGround)
				{
					if (!LeftLegSevered)
					{
						if (gamePadState_Buttons_A_ButtonState_Pressed)
						{
							isJumping = true;
							LeftFootIsOnGround = false;
						}
						if (gamePadState_Buttons_A_ButtonState_Released)
						{
							isJumping = false;
						}
					}
				}
				else if (RightFootIsOnGround && !RightLegSevered)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						isJumping = true;
						RightFootIsOnGround = false;
					}
					if (gamePadState_Buttons_A_ButtonState_Released)
					{
						isJumping = false;
					}
				}
				if (Jump)
				{
					if (!wasJumpPressed && isJumping)
					{
						StandStateIndex = 0f;
						Crouch = false;
						Crawl = false;
						Jump = true;
						SoundJump.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						JumpStart = true;
						float num7 = 3f;
						if (DirectionLeft)
						{
							movementY = -500f * JumpStrength * 0.2f + movementX * num7;
						}
						else if (DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f + (0f - movementX) * num7;
						}
						else if (!DirectionLeft && !DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f * ((0f - movementX) * num7);
						}
					}
					wasJumpPressed = isJumping;
				}
				if (JumpStart)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						JumpDuration++;
						if (JumpDuration > JumpDurationMax)
						{
							JumpDuration = JumpDurationMax;
						}
					}
					else
					{
						JumpDuration = 1;
					}
					JumpTime++;
					if (JumpTime > JumpDuration)
					{
						if (movementY < 1f)
						{
							movementY = 2f;
						}
						Vector2 linearVelocityFromLocalPoint = _bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(0f, 0f));
						movementY /= 0.75f;
						if (linearVelocityFromLocalPoint.Y < 100f)
						{
							JumpStart = false;
							JumpTime = 0;
							movementY = 0f;
						}
					}
				}
			}
			if (XLock)
			{
				movementY = 0f;
			}
			PlayerMovement = new Vector2(movementX * 0.2f, movementY * 0.2f);
			_headBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _headBody.Body.Position);
			_headBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _headBody.Body.Position);
			if (!LeftArmSevered)
			{
				_leftUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftUpperArmBody.Body.Position);
				_leftUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftUpperArmBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightUpperArmBody.Body.Position);
				_rightUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightUpperArmBody.Body.Position);
			}
			if (!LeftLegSevered)
			{
				_leftThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftThighBody.Body.Position);
			}
			if (!RightLegSevered)
			{
				_rightThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightThighBody.Body.Position);
			}
			if (!LeftArmSevered)
			{
				_leftHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftHandBody.Body.Position);
				_leftHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftHandBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightHandBody.Body.Position);
				_rightHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightHandBody.Body.Position);
			}
			PlayerPosition = _headBody.Body.Position;
		}
	}

	private void GetInput2_Physics_Rick(World _world, GameTime gameTime)
	{
		if (Dead)
		{
			if (!ReSpawn)
			{
				if (!Spirit_Walking && gamePadState_Buttons_Y_ButtonState_Pressed)
				{
					Spirit_Walking = true;
					Spirit_Walking_Time_OldGameTime = gameTime.TotalGameTime.TotalSeconds;
					PlayerPosition = _headBody.Body.Position;
				}
				if (Spirit_Walking)
				{
					float num = 1f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					if (gamePadState_DPad_Left_ButtonState_Pressed)
					{
						num2 += num;
					}
					if (gamePadState_DPad_Right_ButtonState_Pressed)
					{
						num3 += num;
					}
					if (gamePadState_DPad_Up_ButtonState_Pressed)
					{
						num4 += num;
					}
					if (gamePadState_DPad_Down_ButtonState_Pressed)
					{
						num5 += num;
					}
					float num6 = 1f;
					Vector2 vector = new Vector2(0f - num2 + num3, 0f - num4 + num5);
					PlayerPosition = PlayerPosition + vector + new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y) * new Vector2(num6, 0f - num6);
					if (PlayerPosition.X < -2000f * level.MasterScale)
					{
						PlayerPosition.X = -2000f * level.MasterScale;
					}
					if (PlayerPosition.X > 5000f * level.MasterScale)
					{
						PlayerPosition.X = 5000f * level.MasterScale;
					}
					if (PlayerPosition.Y > (float)GroundPlainHeight * level.MasterScale)
					{
						PlayerPosition.Y = (float)GroundPlainHeight * level.MasterScale;
					}
					particleEffectSpirit[0].TriggerOffset = PlayerPosition * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectSpirit.Trigger(new Vector2(0f, 0f));
				}
			}
			if (Spirit_Walking && gameTime.TotalGameTime.TotalSeconds - (double)Spirit_Walking_Time > Spirit_Walking_Time_OldGameTime)
			{
				ReSpawn = true;
			}
		}
		else
		{
			if (Unconscious)
			{
				return;
			}
			if (TelekinesisHitSomthing)
			{
				Get_Grab_Movements(_world);
			}
			Get_Arm_Movements();
			if (!Player1Index && !Player2Index && !Player3Index && !Player4Index)
			{
				return;
			}
			if (!Frozen)
			{
				LeftShoulderTriggerState = gamePadState_Triggers_Left > 0.1f;
				if (!LeftArmSevered)
				{
					if (LeftShoulderTriggerState)
					{
						if (PlayerMana > KineticShieldManaCost)
						{
							RockSkin_ON_Rick(_world);
						}
						else
						{
							RockSkin_OFF_Rick(_world);
						}
					}
					else
					{
						RockSkin_OFF_Rick(_world);
					}
					if (gamePadState_Buttons_LeftShoulder_ButtonState_Pressed)
					{
						_ = PlayerMana;
						_ = KineticShieldManaCost;
					}
				}
			}
			if (!RightArmSevered)
			{
				RightShoulderTriggerState = gamePadState_Triggers_Right > 0.1f;
				if (RightShoulderTriggerState)
				{
					if (!TelekinesisHitSomthing)
					{
						if (PlayerMana > TelekinesisManaCost)
						{
							Grab_ON_Rick(_world, gameTime);
						}
					}
					else if (PlayerMana > TelekinesisManaCost)
					{
						PlayerMana -= TelekinesisManaCost;
						level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartGrabVibrationSpeed);
					}
					else
					{
						Grab_OFF_Rick(_world, gameTime);
					}
				}
				else
				{
					Grab_OFF_Rick(_world, gameTime);
				}
				if (gamePadState_Buttons_RightShoulder_ButtonState_Pressed)
				{
					if (BoneDartRepeater > BoneDartRepeaterMax)
					{
						CreateRock_Rick(_world);
						level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartBoneVibrationSpeed);
						BoneDartRepeater = 0f;
					}
					else
					{
						BoneDartRepeater++;
					}
				}
				if (!RightShoulderTriggerStateToggle && !RightShoulderTriggerState && _rightHandGrabJoint != null)
				{
					if (_rightHandGrabOtherFixture != null)
					{
						_rightHandGrabOtherFixture.CollisionGroup = _rightHandGrabOtherFixture_CollisionGroup;
					}
					_world.RemoveJoint(_rightHandGrabJoint);
					_rightHandGrabJoint = null;
					GrabWithRightHandBool = false;
					if (_rightHandGrabOtherFixture != null && (int)_rightHandGrabOtherFixture.Body.UserData != 9 && _rightHandGrabOtherFixture.CollisionGroup != 101 && _rightHandGrabOtherFixture.CollisionGroup != 102 && _rightHandGrabOtherFixture.CollisionGroup != 103 && _rightHandGrabOtherFixture.CollisionGroup != 104)
					{
						_rightHandGrabOtherFixture.Body.ApplyLinearImpulse(new Vector2(RightHandForce.X, RightHandForce.Y) * new Vector2(GrabThrowForceScaler, GrabThrowForceScaler), new Vector2(0f, 0f));
					}
				}
				RightShoulderTriggerState = gamePadState_Triggers_Right > 0.1f;
				if (!RightShoulderTriggerState && _rightHandGrabJoint != null)
				{
					if (_rightHandGrabOtherFixture != null)
					{
						_rightHandGrabOtherFixture.CollisionGroup = _rightHandGrabOtherFixture_CollisionGroup;
					}
					_world.RemoveJoint(_rightHandGrabJoint);
					_rightHandGrabJoint = null;
					GrabWithRightHandBool = false;
					if (_rightHandGrabOtherFixture != null && (int)_rightHandGrabOtherFixture.Body.UserData != 9 && _rightHandGrabOtherFixture.CollisionGroup != 101 && _rightHandGrabOtherFixture.CollisionGroup != 102 && _rightHandGrabOtherFixture.CollisionGroup != 103 && _rightHandGrabOtherFixture.CollisionGroup != 104)
					{
						_rightHandGrabOtherFixture.Body.ApplyLinearImpulse(new Vector2(RightHandForce.X, RightHandForce.Y) * new Vector2(GrabThrowForceScaler, GrabThrowForceScaler), new Vector2(0f, 0f));
					}
					_rightHandGrabOtherFixture = null;
				}
			}
			else
			{
				_SightON = false;
			}
			if (gamePadState_Buttons_X_ButtonState_Pressed)
			{
				Juggernaut = true;
				MaxJointForce = 750f;
			}
			else
			{
				Juggernaut = false;
				MaxJointForce = 150f;
			}
			if (gamePadState_DPad_Left_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X < -0.5f)
			{
				if (DirectionRight)
				{
					Clear_X_Forces();
				}
				DirectionLeft = true;
				DirectionRight = false;
				if (Juggernaut)
				{
					RunLimit = 20f;
					_bodyAngleJoint.TargetAngle = -1f;
					_kineticShields.Body.Position = _bodyBody.Body.Position;
					_kineticShields.Body.Active = true;
					_kineticShields.CollidesWith = CollisionCategory.All;
					_kineticShields.CollisionCategories = CollisionCategory.All;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX > (0f - RunLimit) * 0.2f)
					{
						movementX += -0.4f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
					if (_leftUpperArmJoint != null)
					{
						_leftUpperArmJoint.MotorEnabled = true;
					}
					if (_leftUpperArmJoint != null)
					{
						_leftUpperArmJoint.MotorSpeed = -40f * (0f - movementX);
					}
				}
				else
				{
					RunLimit = 0.1f;
					_bodyAngleJoint.TargetAngle = BodyLeanAngle;
					_kineticShields.Body.Position = _bodyBody.Body.Position;
					_kineticShields.Body.Active = false;
					_kineticShields.CollidesWith = CollisionCategory.None;
					_kineticShields.CollisionCategories = CollisionCategory.None;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX > (0f - RunLimit) * 0.2f)
					{
						movementX += -0.2f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = -40f * (0f - movementX);
					}
				}
			}
			else if (gamePadState_DPad_Right_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X > 0.5f)
			{
				if (DirectionLeft)
				{
					Clear_X_Forces();
				}
				DirectionLeft = false;
				DirectionRight = true;
				if (Juggernaut)
				{
					RunLimit = 20f;
					_bodyAngleJoint.TargetAngle = 1f;
					_kineticShields.Body.Position = _bodyBody.Body.Position;
					_kineticShields.Body.Active = true;
					_kineticShields.CollidesWith = CollisionCategory.All;
					_kineticShields.CollisionCategories = CollisionCategory.All;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX < RunLimit * 0.2f)
					{
						movementX += 0.05f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = 40f * movementX;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = 40f * movementX;
					}
					if (_leftUpperArmJoint != null)
					{
						_leftUpperArmJoint.MotorEnabled = true;
					}
					if (_leftUpperArmJoint != null)
					{
						_leftUpperArmJoint.MotorSpeed = 40f * movementX;
					}
				}
				else
				{
					RunLimit = 0.1f;
					_bodyAngleJoint.TargetAngle = 0f - BodyLeanAngle;
					_kineticShields.Body.Position = _bodyBody.Body.Position;
					_kineticShields.Body.Active = false;
					_kineticShields.CollidesWith = CollisionCategory.None;
					_kineticShields.CollisionCategories = CollisionCategory.None;
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorEnabled = true;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorEnabled = true;
					}
					if (movementX < RunLimit * 0.2f)
					{
						movementX += 0.2f;
					}
					if (_leftThighJoint != null)
					{
						_leftThighJoint.MotorSpeed = 40f * movementX;
					}
					if (_rightThighJoint != null)
					{
						_rightThighJoint.MotorSpeed = 40f * movementX;
					}
				}
			}
			else
			{
				Clear_X_Forces();
				_bodyAngleJoint.TargetAngle = -0.01f;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = false;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = false;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorEnabled = false;
				}
				if (movementX > 1f)
				{
					movementX -= 0.1f;
				}
				else if (movementX < -1f)
				{
					movementX += 0.1f;
				}
				else
				{
					movementX = 0f;
				}
			}
			if ((_leftThighJoint != null) & (_rightThighJoint != null))
			{
				if (LeftFootIsOnGround)
				{
					if (!LeftLegSevered)
					{
						if (gamePadState_Buttons_A_ButtonState_Pressed)
						{
							isJumping = true;
							LeftFootIsOnGround = false;
						}
						if (gamePadState_Buttons_A_ButtonState_Released)
						{
							isJumping = false;
						}
					}
				}
				else if (RightFootIsOnGround && !RightLegSevered)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						isJumping = true;
						RightFootIsOnGround = false;
					}
					if (gamePadState_Buttons_A_ButtonState_Released)
					{
						isJumping = false;
					}
				}
				if (Jump)
				{
					if (!wasJumpPressed && isJumping)
					{
						StandStateIndex = 0f;
						Crouch = false;
						Crawl = false;
						Jump = true;
						SoundJump.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						JumpStart = true;
						float num7 = 3f;
						if (DirectionLeft)
						{
							movementY = -500f * JumpStrength * 0.2f + movementX * num7;
						}
						else if (DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f + (0f - movementX) * num7;
						}
						else if (!DirectionLeft && !DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f * ((0f - movementX) * num7);
						}
					}
					wasJumpPressed = isJumping;
				}
				if (JumpStart)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						JumpDuration++;
						if (JumpDuration > JumpDurationMax)
						{
							JumpDuration = JumpDurationMax;
						}
					}
					else
					{
						JumpDuration = 1;
					}
					JumpTime++;
					if (JumpTime > JumpDuration)
					{
						if (movementY < 1f)
						{
							movementY = 2f;
						}
						Vector2 linearVelocityFromLocalPoint = _bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(0f, 0f));
						movementY /= 0.75f;
						if (linearVelocityFromLocalPoint.Y < 100f)
						{
							JumpStart = false;
							JumpTime = 0;
							movementY = 0f;
						}
					}
				}
			}
			PlayerMovement = new Vector2(movementX * 0.2f, movementY * 0.2f);
			_headBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _headBody.Body.Position);
			_headBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _headBody.Body.Position);
			if (!LeftArmSevered)
			{
				_leftUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftUpperArmBody.Body.Position);
				_leftUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftUpperArmBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightUpperArmBody.Body.Position);
				_rightUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightUpperArmBody.Body.Position);
			}
			if (!LeftLegSevered)
			{
				_leftThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftThighBody.Body.Position);
			}
			if (!RightLegSevered)
			{
				_rightThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightThighBody.Body.Position);
			}
			if (!LeftArmSevered)
			{
				_leftHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftHandBody.Body.Position);
				_leftHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftHandBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightHandBody.Body.Position);
				_rightHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightHandBody.Body.Position);
			}
			PlayerPosition = _headBody.Body.Position;
		}
	}

	private void GetInput2_Physics_Vinny(World _world, GameTime gameTime)
	{
		if (Dead)
		{
			if (!ReSpawn)
			{
				if (!Spirit_Walking && gamePadState_Buttons_Y_ButtonState_Pressed)
				{
					Spirit_Walking = true;
					Spirit_Walking_Time_OldGameTime = gameTime.TotalGameTime.TotalSeconds;
					PlayerPosition = _headBody.Body.Position;
				}
				if (Spirit_Walking)
				{
					float num = 1f;
					float num2 = 0f;
					float num3 = 0f;
					float num4 = 0f;
					float num5 = 0f;
					if (gamePadState_DPad_Left_ButtonState_Pressed)
					{
						num2 += num;
					}
					if (gamePadState_DPad_Right_ButtonState_Pressed)
					{
						num3 += num;
					}
					if (gamePadState_DPad_Up_ButtonState_Pressed)
					{
						num4 += num;
					}
					if (gamePadState_DPad_Down_ButtonState_Pressed)
					{
						num5 += num;
					}
					float num6 = 1f;
					Vector2 vector = new Vector2(0f - num2 + num3, 0f - num4 + num5);
					PlayerPosition = PlayerPosition + vector + new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y) * new Vector2(num6, 0f - num6);
					if (PlayerPosition.X < -2000f * level.MasterScale)
					{
						PlayerPosition.X = -2000f * level.MasterScale;
					}
					if (PlayerPosition.X > 6000f * level.MasterScale)
					{
						PlayerPosition.X = 6000f * level.MasterScale;
					}
					if (PlayerPosition.Y > (float)GroundPlainHeight * level.MasterScale)
					{
						PlayerPosition.Y = (float)GroundPlainHeight * level.MasterScale;
					}
					particleEffectSpirit[0].TriggerOffset = PlayerPosition * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
					particleEffectSpirit.Trigger(new Vector2(0f, 0f));
				}
			}
			if (Spirit_Walking && gameTime.TotalGameTime.TotalSeconds - (double)Spirit_Walking_Time > Spirit_Walking_Time_OldGameTime)
			{
				ReSpawn = true;
			}
		}
		else
		{
			if (Unconscious)
			{
				return;
			}
			if (TelekinesisHitSomthing)
			{
				Get_Tele_Movements();
			}
			Get_Arm_Movements();
			if (!Player1Index && !Player2Index && !Player3Index && !Player4Index)
			{
				return;
			}
			if (!Frozen)
			{
				LeftShoulderTriggerState = gamePadState_Triggers_Left > 0.1f;
				if (gamePadState_Triggers_Left < 0.1f)
				{
					Fly_All_Out = false;
				}
				if (!LeftArmSevered)
				{
					if (LeftShoulderTriggerState)
					{
						if (PlayerMana > FlyManaCost)
						{
							Fly_ON_Vinny(_world);
						}
						else
						{
							Fly_OFF_Vinny(_world);
						}
					}
					else
					{
						Fly_OFF_Vinny(_world);
					}
					if (gamePadState_Buttons_LeftShoulder_ButtonState_Pressed)
					{
						_ = PlayerMana;
						_ = KineticShieldManaCost;
					}
				}
			}
			if (!RightArmSevered)
			{
				RightShoulderTriggerState = gamePadState_Triggers_Right > 0.1f;
				if (RightShoulderTriggerState)
				{
					if (!TelekinesisHitSomthing)
					{
						if (PlayerMana > TelekinesisManaCost)
						{
							Telekinesis(_world, gameTime);
						}
					}
					else if (PlayerMana > TelekinesisManaCost)
					{
						PlayerMana -= TelekinesisManaCost;
						level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartTeleVibrationSpeed);
					}
					else
					{
						Telekinesis_Null();
					}
				}
				else
				{
					Telekinesis_Null();
				}
				if (gamePadState_Buttons_RightShoulder_ButtonState_Pressed)
				{
					Claw_ON_Vinny(_world);
					level.Vibration_Pulse_Right(playerIndex, DartVibrationDuration, DartClawVibrationSpeed);
				}
				else
				{
					Claw_OFF_Vinny(_world);
					IsClawing = false;
				}
			}
			else
			{
				_SightON = false;
			}
			if (gamePadState_Buttons_X_ButtonState_Pressed)
			{
				RunLimit = 20f;
				WasSprinting = true;
			}
			else
			{
				if (WasSprinting)
				{
					Clear_X_Forces();
				}
				WasSprinting = false;
				RunLimit = 0.1f;
			}
			if (gamePadState_DPad_Left_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X < -0.5f)
			{
				if (DirectionRight)
				{
					Clear_X_Forces();
				}
				DirectionLeft = true;
				DirectionRight = false;
				_bodyAngleJoint.TargetAngle = BodyLeanAngle;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = true;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = true;
				}
				if (movementX > (0f - RunLimit) * 0.2f)
				{
					movementX += -0.4f;
				}
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorSpeed = -40f * (0f - movementX);
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorSpeed = -40f * (0f - movementX);
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorEnabled = true;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorSpeed = -40f * (0f - movementX);
				}
			}
			else if (gamePadState_DPad_Right_ButtonState_Pressed || gamePadState_ThumbSticks_Left_X > 0.5f)
			{
				if (DirectionLeft)
				{
					Clear_X_Forces();
				}
				DirectionLeft = false;
				DirectionRight = true;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = true;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = true;
				}
				if (movementX < RunLimit * 0.2f)
				{
					movementX += 0.05f;
				}
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorSpeed = 40f * movementX;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorSpeed = 40f * movementX;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorEnabled = true;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorSpeed = 40f * movementX;
				}
			}
			else
			{
				Clear_X_Forces();
				_bodyAngleJoint.TargetAngle = -0.01f;
				if (_leftThighJoint != null)
				{
					_leftThighJoint.MotorEnabled = false;
				}
				if (_rightThighJoint != null)
				{
					_rightThighJoint.MotorEnabled = false;
				}
				if (_leftUpperArmJoint != null)
				{
					_leftUpperArmJoint.MotorEnabled = false;
				}
				if (movementX > 1f)
				{
					movementX -= 0.1f;
				}
				else if (movementX < -1f)
				{
					movementX += 0.1f;
				}
				else
				{
					movementX = 0f;
				}
			}
			if ((_leftThighJoint != null) & (_rightThighJoint != null))
			{
				if (LeftFootIsOnGround)
				{
					if (!LeftLegSevered)
					{
						if (gamePadState_Buttons_A_ButtonState_Pressed)
						{
							isJumping = true;
							LeftFootIsOnGround = false;
						}
						if (gamePadState_Buttons_A_ButtonState_Released)
						{
							isJumping = false;
						}
					}
				}
				else if (RightFootIsOnGround && !RightLegSevered)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						isJumping = true;
						RightFootIsOnGround = false;
					}
					if (gamePadState_Buttons_A_ButtonState_Released)
					{
						isJumping = false;
					}
				}
				if (Jump)
				{
					if (!wasJumpPressed && isJumping)
					{
						StandStateIndex = 0f;
						Crouch = false;
						Crawl = false;
						Jump = true;
						SoundJump.Play(level.mainGame.Sound_Effect_Volume, ((float)level.random.NextDouble() - 0.5f) / 2f + 0f, 0f);
						JumpStart = true;
						float num7 = 3f;
						if (DirectionLeft)
						{
							movementY = -500f * JumpStrength * 0.2f + movementX * num7;
						}
						else if (DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f + (0f - movementX) * num7;
						}
						else if (!DirectionLeft && !DirectionRight)
						{
							movementY = -500f * JumpStrength * 0.2f * ((0f - movementX) * num7);
						}
					}
					wasJumpPressed = isJumping;
				}
				if (JumpStart)
				{
					if (gamePadState_Buttons_A_ButtonState_Pressed)
					{
						JumpDuration++;
						if (JumpDuration > JumpDurationMax)
						{
							JumpDuration = JumpDurationMax;
						}
					}
					else
					{
						JumpDuration = 1;
					}
					JumpTime++;
					if (JumpTime > JumpDuration)
					{
						if (movementY < 1f)
						{
							movementY = 2f;
						}
						Vector2 linearVelocityFromLocalPoint = _bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(0f, 0f));
						movementY /= 0.75f;
						if (linearVelocityFromLocalPoint.Y < 100f)
						{
							JumpStart = false;
							JumpTime = 0;
							movementY = 0f;
						}
					}
				}
			}
			PlayerMovement = new Vector2(movementX * 0.2f, movementY * 0.2f);
			_headBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _headBody.Body.Position);
			_headBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _headBody.Body.Position);
			if (!LeftArmSevered)
			{
				_leftUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftUpperArmBody.Body.Position);
				_leftUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftUpperArmBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightUpperArmBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightUpperArmBody.Body.Position);
				_rightUpperArmBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightUpperArmBody.Body.Position);
			}
			if (!LeftLegSevered)
			{
				_leftThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftThighBody.Body.Position);
			}
			if (!RightLegSevered)
			{
				_rightThighBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightThighBody.Body.Position);
			}
			if (!LeftArmSevered)
			{
				_leftHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _leftHandBody.Body.Position);
				_leftHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _leftHandBody.Body.Position);
			}
			if (!RightArmSevered)
			{
				_rightHandBody.Body.ApplyForce(new Vector2(PlayerMovement.X, 0f), _rightHandBody.Body.Position);
				_rightHandBody.Body.ApplyForce(new Vector2(0f, PlayerMovement.Y), _rightHandBody.Body.Position);
			}
			PlayerPosition = _headBody.Body.Position;
		}
	}

	private void PhaseShift_ON()
	{
		PhaseShift = true;
		_bodyBody.Body.LinearVelocity = new Vector2(_bodyBody.Body.LinearVelocity.X, 0f - _bodyBody.Body.LinearVelocity.Y);
		_headBody.Body.LinearVelocity = new Vector2(_headBody.Body.LinearVelocity.X, 0f - _headBody.Body.LinearVelocity.Y);
		_leftUpperArmBody.Body.LinearVelocity = new Vector2(_leftUpperArmBody.Body.LinearVelocity.X, 0f - _leftUpperArmBody.Body.LinearVelocity.Y);
		_leftThighBody.Body.LinearVelocity = new Vector2(_leftThighBody.Body.LinearVelocity.X, 0f - _leftThighBody.Body.LinearVelocity.Y);
		_leftHandBody.Body.LinearVelocity = new Vector2(_leftHandBody.Body.LinearVelocity.X, 0f - _leftHandBody.Body.LinearVelocity.Y);
		_rightUpperArmBody.Body.LinearVelocity = new Vector2(_rightUpperArmBody.Body.LinearVelocity.X, 0f - _rightUpperArmBody.Body.LinearVelocity.Y);
		_rightThighBody.Body.LinearVelocity = new Vector2(_rightThighBody.Body.LinearVelocity.X, 0f - _rightThighBody.Body.LinearVelocity.Y);
		_rightHandBody.Body.LinearVelocity = new Vector2(_rightHandBody.Body.LinearVelocity.X, 0f - _rightHandBody.Body.LinearVelocity.Y);
		_bodyBody.CollisionCategories = CollisionCategory.None;
		_headBody.CollisionCategories = CollisionCategory.None;
		_leftUpperArmBody.CollisionCategories = CollisionCategory.None;
		_leftThighBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_rightUpperArmBody.CollisionCategories = CollisionCategory.None;
		_rightThighBody.CollisionCategories = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_bodyBody.Body.IgnoreGravity = true;
		_headBody.Body.IgnoreGravity = true;
		_leftUpperArmBody.Body.IgnoreGravity = true;
		_leftThighBody.Body.IgnoreGravity = true;
		_leftHandBody.Body.IgnoreGravity = true;
		_rightUpperArmBody.Body.IgnoreGravity = true;
		_rightThighBody.Body.IgnoreGravity = true;
		_rightHandBody.Body.IgnoreGravity = true;
	}

	private void PhaseShift_OFF()
	{
		PhaseShift = false;
		_bodyBody.CollisionCategories = CollisionCategory.All;
		_headBody.CollisionCategories = CollisionCategory.All;
		_leftUpperArmBody.CollisionCategories = CollisionCategory.All;
		_leftThighBody.CollisionCategories = CollisionCategory.All;
		_leftHandBody.CollisionCategories = CollisionCategory.All;
		_rightUpperArmBody.CollisionCategories = CollisionCategory.All;
		_rightThighBody.CollisionCategories = CollisionCategory.All;
		_rightHandBody.CollisionCategories = CollisionCategory.All;
		_bodyBody.Body.IgnoreGravity = false;
		_headBody.Body.IgnoreGravity = false;
		_leftUpperArmBody.Body.IgnoreGravity = false;
		_leftThighBody.Body.IgnoreGravity = false;
		_leftHandBody.Body.IgnoreGravity = false;
		_rightUpperArmBody.Body.IgnoreGravity = false;
		_rightThighBody.Body.IgnoreGravity = false;
		_rightHandBody.Body.IgnoreGravity = false;
	}

	private void GetInput2_Physics(World _world, GameTime gameTime)
	{
		if (Player_Species == 0f)
		{
			GetInput2_Physics_Daru(_world, gameTime);
		}
		else if (Player_Species == 4f)
		{
			GetInput2_Physics_Ernest(_world, gameTime);
		}
		else if (Player_Species == 1f)
		{
			GetInput2_Physics_Oscar(_world, gameTime);
		}
		else if (Player_Species == 2f)
		{
			GetInput2_Physics_Rick(_world, gameTime);
		}
		else if (Player_Species == 3f)
		{
			GetInput2_Physics_Vinny(_world, gameTime);
		}
	}

	private void GetInput2(GameTime gameTime)
	{
		if (!level.Paused && !level.Exit_Reached_First)
		{
			GamePadState state = GamePad.GetState(playerIndex);
			KeyboardState state2 = Keyboard.GetState();
			keyboardState_IsKeyDown_Keys_Left = state2.IsKeyDown(Keys.Left);
			keyboardState_IsKeyDown_Keys_Right = state2.IsKeyDown(Keys.Right);
			gamePadState_Buttons_RightShoulder_ButtonState_Pressed = state.Buttons.RightShoulder == ButtonState.Pressed;
			gamePadState_Buttons_RightShoulder_ButtonState_Released = state.Buttons.RightShoulder == ButtonState.Released;
			gamePadState_Buttons_LeftShoulder_ButtonState_Pressed = state.Buttons.LeftShoulder == ButtonState.Pressed;
			gamePadState_Buttons_LeftShoulder_ButtonState_Released = state.Buttons.LeftShoulder == ButtonState.Released;
			gamePadState_Triggers_Left = state.Triggers.Left;
			gamePadState_Triggers_Right = state.Triggers.Right;
			gamePadState_Buttons_X_ButtonState_Pressed = state.Buttons.X == ButtonState.Pressed;
			gamePadState_Buttons_Y_ButtonState_Pressed = state.Buttons.Y == ButtonState.Pressed;
			gamePadState_ThumbSticks_Right_X = state.ThumbSticks.Right.X;
			gamePadState_ThumbSticks_Right_Y = state.ThumbSticks.Right.Y;
			gamePadState_ThumbSticks_Left_X = state.ThumbSticks.Left.X;
			gamePadState_ThumbSticks_Left_Y = state.ThumbSticks.Left.Y;
			gamePadState_DPad_Left_ButtonState_Pressed = state.DPad.Left == ButtonState.Pressed;
			gamePadState_DPad_Right_ButtonState_Pressed = state.DPad.Right == ButtonState.Pressed;
			gamePadState_DPad_Up_ButtonState_Pressed = state.DPad.Up == ButtonState.Pressed;
			gamePadState_DPad_Down_ButtonState_Pressed = state.DPad.Down == ButtonState.Pressed;
			gamePadState_Buttons_A_ButtonState_Pressed = state.Buttons.A == ButtonState.Pressed;
			gamePadState_Buttons_A_ButtonState_Released = state.Buttons.A == ButtonState.Released;
			gamePadState_Buttons_B_ButtonState_Pressed = state.Buttons.B == ButtonState.Pressed;
			gamePadState_Buttons_B_ButtonState_Released = state.Buttons.B == ButtonState.Released;
			LeftArmRotation = (float)Math.Atan2(gamePadState_ThumbSticks_Left_Y, gamePadState_ThumbSticks_Left_X);
			RightArmRotation = (float)Math.Atan2(gamePadState_ThumbSticks_Right_Y, gamePadState_ThumbSticks_Right_X);
		}
		if (!Spirit_Walking && level.IsExitRange && !level.Exit_Reached_First)
		{
			if (gamePadState_Buttons_B_ButtonState_Pressed)
			{
				Exiting = true;
			}
			else
			{
				Exiting = false;
			}
		}
	}

	private void GetInput2OLD(GameTime gameTime)
	{
		GamePadState state = GamePad.GetState(playerIndex);
		KeyboardState state2 = Keyboard.GetState();
		keyboardState_IsKeyDown_Keys_Left = state2.IsKeyDown(Keys.Left);
		keyboardState_IsKeyDown_Keys_Right = state2.IsKeyDown(Keys.Right);
		gamePadState_Buttons_RightShoulder_ButtonState_Pressed = state.Buttons.RightShoulder == ButtonState.Pressed;
		gamePadState_Buttons_RightShoulder_ButtonState_Released = state.Buttons.RightShoulder == ButtonState.Released;
		gamePadState_Buttons_LeftShoulder_ButtonState_Pressed = state.Buttons.LeftShoulder == ButtonState.Pressed;
		gamePadState_Buttons_LeftShoulder_ButtonState_Released = state.Buttons.LeftShoulder == ButtonState.Released;
		gamePadState_Triggers_Left = state.Triggers.Left;
		gamePadState_Triggers_Right = state.Triggers.Right;
		gamePadState_Buttons_X_ButtonState_Pressed = state.Buttons.X == ButtonState.Pressed;
		gamePadState_ThumbSticks_Left_X = state.ThumbSticks.Left.X;
		gamePadState_ThumbSticks_Left_X = state.ThumbSticks.Left.X;
		gamePadState_DPad_Left_ButtonState_Pressed = state.DPad.Left == ButtonState.Pressed;
		gamePadState_DPad_Right_ButtonState_Pressed = state.DPad.Right == ButtonState.Pressed;
		gamePadState_Buttons_A_ButtonState_Pressed = state.Buttons.A == ButtonState.Pressed;
		gamePadState_Buttons_A_ButtonState_Released = state.Buttons.A == ButtonState.Released;
		LeftArmRotation = (float)Math.Atan2(gamePadState_ThumbSticks_Left_Y, gamePadState_ThumbSticks_Left_X);
		RightArmRotation = (float)Math.Atan2(gamePadState_ThumbSticks_Right_Y, gamePadState_ThumbSticks_Right_X);
		gamePadState_ThumbSticks_Right_X = state.ThumbSticks.Right.X;
		gamePadState_ThumbSticks_Right_Y = state.ThumbSticks.Right.Y;
		if (level != null)
		{
			if (state2.IsKeyDown(Keys.OemComma))
			{
				level.MasterScale -= 0.005f;
			}
			else if (state2.IsKeyDown(Keys.OemPeriod))
			{
				level.MasterScale += 0.005f;
			}
		}
		if (!Player1Index && !Player2Index && !Player3Index && !Player4Index)
		{
			return;
		}
		if (state.Buttons.RightShoulder == ButtonState.Pressed && !UtilityIndexRightDown)
		{
			if (UtilityIndexRight < 10f)
			{
				UtilityIndexRight++;
			}
			if (UtilityIndexRight > UtilityIndexRightMax)
			{
				UtilityIndexRight = 0f;
			}
			UtilityIndexRightDown = true;
		}
		if (state.Buttons.RightShoulder == ButtonState.Released)
		{
			UtilityIndexRightDown = false;
		}
		if (state.Buttons.LeftShoulder == ButtonState.Pressed && !UtilityIndexLeftDown)
		{
			if (UtilityIndexLeft < 10f)
			{
				UtilityIndexLeft++;
			}
			if (UtilityIndexLeft > UtilityIndexLeftMax)
			{
				UtilityIndexLeft = 0f;
			}
			UtilityIndexLeftDown = true;
		}
		if (state.Buttons.LeftShoulder == ButtonState.Released)
		{
			UtilityIndexLeftDown = false;
		}
		if (UtilityIndexLeft > UtilityIndexLeftMax)
		{
			UtilityIndexLeft = UtilityIndexLeftMax;
		}
		if (UtilityIndexLeft < 0f)
		{
			UtilityIndexLeft = 0f;
		}
		if (UtilityIndexRight > UtilityIndexRightMax)
		{
			UtilityIndexRight = UtilityIndexRightMax;
		}
		if (UtilityIndexRight < 0f)
		{
			UtilityIndexRight = 0f;
		}
		HealEffect[0].TriggerOffset = _bodyBody.Body.Position * PhysicsScaleUp;
		if (UtilityIndexRight == 6f)
		{
			RightUtilityColor = Color.White;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/0");
		}
		if (UtilityIndexRight == 4f)
		{
			RightUtilityColor = Color.White;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/1");
		}
		if (UtilityIndexRight == 3f)
		{
			RightUtilityColor = Color.Cyan;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/2");
		}
		if (UtilityIndexRight == 0f)
		{
			RightUtilityColor = Color.White;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/3");
		}
		if (UtilityIndexRight == 2f)
		{
			RightUtilityColor = Color.Green;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/4");
		}
		if (UtilityIndexRight == 5f)
		{
			RightUtilityColor = Color.Fuchsia;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/5");
		}
		if (UtilityIndexRight == 1f)
		{
			RightUtilityColor = Color.Silver;
			RightHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Right/6");
		}
		if (UtilityIndexLeft == 0f)
		{
			LeftUtilityColor = Color.AliceBlue;
			LeftHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Left/0");
		}
		if (UtilityIndexLeft == 1f)
		{
			LeftUtilityColor = Color.White;
			LeftHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Left/1");
		}
		if (UtilityIndexLeft == 2f)
		{
			LeftUtilityColor = Color.GreenYellow;
			LeftHandUtilityBrush = Level.Content.Load<Texture2D>("Utilities/Left/2");
		}
	}

	public void Telekinesis_NON(World _world, GameTime gameTime)
	{
		TelekinesisRangeScaler = 1000;
		Vector2 TelekinisisBodyHitPoint = default(Vector2);
		ref Vector2 reference = ref TelekinisisBodyHitPoint;
		reference = new Vector2(0f, 0f);
		if (RightArmSevered)
		{
			return;
		}
		if (PlayerMana > TelekinesisManaCost)
		{
			PlayerMana -= TelekinesisManaCost;
			TeleFirstHit = false;
			_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
			{
				Body body = f.Body;
				if (!TeleFirstHit)
				{
					if (f.UserData != (object)1)
					{
						if (f.Body.UserData != (object)1)
						{
							if (f.CollisionGroup == CollisionGroup)
							{
								return -1f;
							}
							if (f.Body.BodyType != BodyType.Static)
							{
								TelekinisisBodyHitPoint = body.Position;
								TelekinesisHitSomthing = true;
								TeleFirstHit = true;
								return fr;
							}
							TeleFirstHit = true;
						}
						else
						{
							TeleFirstHit = true;
						}
					}
					else
					{
						TeleFirstHit = true;
					}
				}
				return -1f;
			}, _rightHandBody.Body.Position, (_rightHandBody.Body.Position - _rightUpperArmBody.Body.Position) * new Vector2(TelekinesisRangeScaler, TelekinesisRangeScaler));
		}
		if (!TelekinesisHitSomthing)
		{
			return;
		}
		foreach (Body body2 in _world.BodyList)
		{
			if (body2.UserData != (object)1 && body2.UserData != (object)0 && body2.UserData != (object)1 && body2.Position == TelekinisisBodyHitPoint)
			{
				TelekinisisBodyHit = body2.FixtureList[0];
				TelekinisisBodyHit.Body.Awake = true;
				TelekinisisBodyHitMassOLD = TelekinisisBodyHit.Body.Mass;
				TelekinisisBodyHit.Body.Mass = 0.0001f;
			}
		}
	}

	public void Telekinesis(World _world, GameTime gameTime)
	{
		TelekinesisRangeScaler = 1000;
		Vector2 TelekinisisBodyHitPoint = default(Vector2);
		ref Vector2 reference = ref TelekinisisBodyHitPoint;
		reference = new Vector2(0f, 0f);
		if (RightArmSevered)
		{
			return;
		}
		if (PlayerMana > TelekinesisManaCost)
		{
			Telekinisis_Try = true;
			_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
			{
				Body body = f.Body;
				if (f.UserData != null)
				{
					if (f.Body.UserData != null)
					{
						if ((int)f.Body.UserData != 1)
						{
							if (f.CollisionGroup != CollisionGroup)
							{
								if (f.Body.BodyType != BodyType.Static)
								{
									if ((int)f.Body.UserData != 90)
									{
										if ((int)f.Body.UserData != 97)
										{
											if ((int)f.Body.UserData != 99)
											{
												if (f.Body.Active)
												{
													if ((int)f.Body.UserData != 10)
													{
														if ((int)f.Body.UserData != 20)
														{
															TelekinisisBodyHitPoint = body.Position;
															TelekinisisBodyHit_Point = p;
															TelekinesisHitSomthing = true;
															return 0f;
														}
														TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
														return fr;
													}
													TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
													return 0f;
												}
												TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
												return fr;
											}
											TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
											return fr;
										}
										TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
										return fr;
									}
									TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
									return fr;
								}
								TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
								return fr;
							}
							TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
							return -1f;
						}
						TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
						return fr;
					}
					TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
					return fr;
				}
				TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
				return fr;
			}, _rightHandBody.Body.Position, _rightHandBody.Body.Position + (_rightHandBody.Body.Position - _rightUpperArmBody.Body.Position) * new Vector2(TelekinesisRangeScaler, TelekinesisRangeScaler));
		}
		if (TelekinesisHitSomthing)
		{
			foreach (Body body2 in _world.BodyList)
			{
				if (body2.UserData != (object)1 && body2.UserData != (object)0 && body2.UserData != (object)1 && body2.Position == TelekinisisBodyHitPoint)
				{
					TelekinisisBodyHit = body2.FixtureList[0];
					TelekinisisBodyHit.Body.Awake = true;
					TelekinisisBodyHitMassOLD = TelekinisisBodyHit.Body.Mass;
					if ((int)body2.UserData != 8)
					{
						TelekinisisBodyHit.Body.Mass = 0.0001f;
					}
				}
			}
		}
		ref Vector2 reference2 = ref TelekinisisBodyHitPoint;
		reference2 = new Vector2(0f, 0f);
	}

	public void Telekinesis_Null()
	{
		if (TelekinisisBodyHit != null)
		{
			TelekinisisBodyHit.Body.Mass = TelekinisisBodyHitMassOLD;
		}
		Telekinisis_Try = false;
		TelekinesisHitSomthing = false;
		if (_rightHandBody != null && _rightHandBody.Body != null)
		{
			TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
		}
		TelekinisisBodyHit = null;
		TelekinisisBodyHitMassOLD = 0f;
	}

	public void CreateBoneSaw_Daru(World _world)
	{
		_DartBoneBulletTimer[DartBoneIndex] = 10.0;
		_DartBone[DartBoneIndex] = FixtureFactory.CreateRectangle(_world, 0.1f, 2.5f, 1f);
		_DartBone[DartBoneIndex].Body.Position = _rightHandBody.Body.Position;
		_DartBone[DartBoneIndex].Body.BodyType = BodyType.Dynamic;
		_DartBone[DartBoneIndex].Body.IsBullet = true;
		_DartBone[DartBoneIndex].Body.IgnoreGravity = true;
		_DartBone[DartBoneIndex].Body.SleepingAllowed = true;
		_DartBone[DartBoneIndex].Density = 0.2f;
		_DartBone[DartBoneIndex].Friction = 0.1f;
		_DartBone[DartBoneIndex].Body.Mass = 10f;
		_DartBone[DartBoneIndex].Restitution = 0.5f;
		_DartBone[DartBoneIndex].Body.UserData = 122;
		_DartBone[DartBoneIndex].Body.LinearDamping = 0f;
		_DartBone[DartBoneIndex].Body.AngularDamping = 0.9f;
		_DartBone[DartBoneIndex].CollisionGroup = CollisionGroup;
		_DartBoneTexture = level._DartBoneSawTexture;
		_DartBoneOrigin = new Vector2(_DartBoneTexture.Width / 2, _DartBoneTexture.Height / 2);
		Fixture obj = _DartBone[DartBoneIndex];
		obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartBoneSaw_OnCollision_Daru));
		_DartBone[DartBoneIndex].Body.Rotation = _rightUpperArmBody.Body.Rotation;
		_DartBone[DartBoneIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(DartBoneForceScaler, DartBoneForceScaler));
		DartBoneIndex++;
	}

	public void CreateHarpoon_Daru(World _world)
	{
		if (PlayerMana > DartHarpoonManaCost)
		{
			PlayerMana -= DartHarpoonManaCost;
			_DartHarpoonBulletTimer[DartHarpoonIndex] = 10.0;
			_DartHarpoon[DartHarpoonIndex] = FixtureFactory.CreateRectangle(_world, 0.1f, 10f, 1f);
			_DartHarpoon[DartHarpoonIndex].Body.Position = _rightHandBody.Body.Position;
			_DartHarpoon[DartHarpoonIndex].Body.BodyType = BodyType.Dynamic;
			_DartHarpoon[DartHarpoonIndex].Body.IsBullet = true;
			_DartHarpoon[DartHarpoonIndex].Body.IgnoreGravity = true;
			_DartHarpoon[DartHarpoonIndex].Body.SleepingAllowed = true;
			_DartHarpoon[DartHarpoonIndex].Density = 0.2f;
			_DartHarpoon[DartHarpoonIndex].Friction = 0.1f;
			_DartHarpoon[DartHarpoonIndex].Body.Mass = 10f;
			_DartHarpoon[DartHarpoonIndex].Restitution = 0.5f;
			_DartHarpoon[DartHarpoonIndex].Body.UserData = 990;
			_DartHarpoon[DartHarpoonIndex].Body.LinearDamping = 0f;
			_DartHarpoon[DartHarpoonIndex].Body.AngularDamping = 0.9f;
			_DartHarpoon[DartHarpoonIndex].CollisionGroup = CollisionGroup;
			_DartHarpoonTexture = level._DartHarpoonTexture;
			_DartHarpoonOrigin = new Vector2(_DartHarpoonTexture.Width / 2, _DartHarpoonTexture.Height / 2);
			Fixture obj = _DartHarpoon[DartHarpoonIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartHarpoon_OnCollision_Daru));
			_DartHarpoon[DartHarpoonIndex].Body.Rotation = _rightUpperArmBody.Body.Rotation;
			_DartHarpoon[DartHarpoonIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(DartHarpoonForceScaler, DartHarpoonForceScaler));
			DartHarpoonIndex++;
		}
	}

	public void Shield_ON_Daru(World _world)
	{
		if (!RightArmSevered)
		{
			if (PlayerMana > KineticShieldManaCost)
			{
				PlayerMana -= KineticShieldManaCost;
				Weapon_Armed = true;
				_kineticShields.Body.Active = true;
				_kineticShields.CollidesWith = CollisionCategory.All;
				_kineticShields.CollisionCategories = CollisionCategory.All;
				_kineticShields.Body.Rotation = _rightUpperArmBody.Body.Rotation + 1.570795f;
				_kineticShields.Body.Position = _rightHandBody.Body.Position + (_rightHandBody.Body.Position - _rightUpperArmBody.Body.Position) * 4f;
			}
			else
			{
				Shield_OFF_Daru(_world);
			}
		}
		else
		{
			Shield_OFF_Daru(_world);
		}
	}

	public void Shield_OFF_Daru(World _world)
	{
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		_kineticShields.Body.Active = false;
		_kineticShields.CollidesWith = CollisionCategory.None;
		_kineticShields.CollisionCategories = CollisionCategory.None;
		Weapon_Armed = false;
	}

	public void Weapon_ON_Ernest(World _world)
	{
		if (!RightArmSevered)
		{
			if (!Weapon_Armed)
			{
				_kineticShields.Body.Rotation = _rightUpperArmBody.Body.Rotation + 3.14159f;
				WeaponJoint = new WeldJoint(_rightUpperArmBody.Body, _kineticShields.Body, new Vector2(0f, 0f), new Vector2(0f, 23.1f));
				_world.AddJoint(WeaponJoint);
				Weapon_Armed = true;
			}
			if (DirectionRight)
			{
				_kineticShields.Body.Active = true;
				_kineticShields.Body.AngularVelocity = _rightHandBody.Body.AngularVelocity;
				_kineticShields.Body.LinearVelocity = _rightHandBody.Body.LinearVelocity;
			}
			else if (DirectionLeft)
			{
				_kineticShields.Body.Active = true;
				_kineticShields.Body.AngularVelocity = _rightHandBody.Body.AngularVelocity;
				_kineticShields.Body.LinearVelocity = _rightHandBody.Body.LinearVelocity;
			}
			else
			{
				_kineticShields.Body.Active = true;
				_kineticShields.Body.AngularVelocity = _rightHandBody.Body.AngularVelocity;
				_kineticShields.Body.LinearVelocity = _rightHandBody.Body.LinearVelocity;
			}
			_kineticShields.CollidesWith = CollisionCategory.All;
			_kineticShields.CollisionCategories = CollisionCategory.All;
		}
		else
		{
			Weapon_OFF_Ernest(_world);
		}
	}

	public void Weapon_OFF_Ernest(World _world)
	{
		if (WeaponJoint != null)
		{
			_kineticShields.Body.Position = _bodyBody.Body.Position;
			_kineticShields.Body.Active = false;
			_kineticShields.CollidesWith = CollisionCategory.None;
			_kineticShields.CollisionCategories = CollisionCategory.None;
			_world.RemoveJoint(WeaponJoint);
		}
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		_kineticShields.Body.Active = false;
		_kineticShields.CollidesWith = CollisionCategory.None;
		_kineticShields.CollisionCategories = CollisionCategory.None;
		Weapon_Armed = false;
	}

	public void CreateBurr_Ernest(World _world)
	{
		if (PlayerMana > DartBoneManaCost)
		{
			PlayerMana -= DartBoneManaCost;
			_DartBoneBulletTimer[DartBoneIndex] = 10.0;
			_DartBone[DartBoneIndex] = FixtureFactory.CreateRectangle(_world, 0.1f, 6f, 0.0001f);
			_DartBone[DartBoneIndex].Body.Position = _rightHandBody.Body.Position;
			_DartBone[DartBoneIndex].Body.BodyType = BodyType.Dynamic;
			_DartBone[DartBoneIndex].Body.IsBullet = true;
			_DartBone[DartBoneIndex].Body.IgnoreGravity = false;
			_DartBone[DartBoneIndex].Body.SleepingAllowed = true;
			_DartBone[DartBoneIndex].Density = 0.2f;
			_DartBone[DartBoneIndex].Friction = 0.1f;
			_DartBone[DartBoneIndex].UserData = DartBoneIndex + 1000;
			_DartBone[DartBoneIndex].Restitution = 0.5f;
			_DartBone[DartBoneIndex].Body.UserData = 202;
			_DartBone[DartBoneIndex].Body.LinearDamping = 0f;
			_DartBone[DartBoneIndex].Body.AngularDamping = 0.9f;
			_DartBone[DartBoneIndex].CollisionGroup = CollisionGroup;
			_DartBoneTexture = level._DartBurrTexture;
			_DartBoneOrigin = new Vector2(_DartBoneTexture.Width / 2, _DartBoneTexture.Height / 2);
			Fixture obj = _DartBone[DartBoneIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartBurr_OnCollision_Ernest));
			_DartBone[DartBoneIndex].Body.Rotation = _rightUpperArmBody.Body.Rotation;
			_DartBone[DartBoneIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(DartBoneForceScaler, DartBoneForceScaler));
			_DartBone[DartBoneIndex].Body.ApplyTorque(200f);
			DartBoneIndex++;
		}
	}

	public void Climb_ON_Ernest(World _world)
	{
		if (Climb_All_Out)
		{
			return;
		}
		if (PlayerMana > ClimbManaCost)
		{
			PlayerMana -= ClimbManaCost;
			MaxJointForce = 150f;
			if (_leftThighJoint != null)
			{
				_leftThighJoint.MotorSpeed = 0f;
				_leftThighJoint.MaxMotorTorque = 0f;
			}
			if (_rightThighJoint != null)
			{
				_rightThighJoint.MotorSpeed = 0f;
				_rightThighJoint.MaxMotorTorque = 0f;
			}
			_bodyBody.Restitution = 1f;
			_headBody.Restitution = 1f;
			_leftUpperArmBody.Restitution = 1f;
			_leftHandBody.Restitution = 1f;
			_leftThighBody.Restitution = 1f;
			_rightUpperArmBody.Restitution = 1f;
			_rightHandBody.Restitution = 1f;
			_rightThighBody.Restitution = 1f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
			_leftUpperArmBody.Body.LinearDamping = 0f;
			_leftHandBody.Body.LinearDamping = 0f;
			_leftThighBody.Body.LinearDamping = 0f;
			_rightUpperArmBody.Body.LinearDamping = 0f;
			_rightHandBody.Body.LinearDamping = 0f;
			_rightThighBody.Body.LinearDamping = 0f;
			_bodyBody.Body.IgnoreGravity = true;
			_headBody.Body.IgnoreGravity = true;
			_leftUpperArmBody.Body.IgnoreGravity = true;
			_leftHandBody.Body.IgnoreGravity = true;
			_leftThighBody.Body.IgnoreGravity = true;
			_rightUpperArmBody.Body.IgnoreGravity = true;
			_rightHandBody.Body.IgnoreGravity = true;
			_rightThighBody.Body.IgnoreGravity = true;
			if (BounceHit)
			{
				BounceForce = new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y - gamePadState_ThumbSticks_Left_Y * 2f);
				BounceForceScaler = new Vector2(50f, 50f);
				BounceForce *= BounceForceScaler;
				BounceHit = false;
			}
			_bodyBody.Body.ApplyForce(BounceForce, _bodyBody.Body.Position);
			_headBody.Body.ApplyForce(BounceForce, _headBody.Body.Position);
			_leftUpperArmBody.Body.ApplyForce(BounceForce, _leftUpperArmBody.Body.Position);
			_leftHandBody.Body.ApplyForce(BounceForce, _leftHandBody.Body.Position);
			_leftThighBody.Body.ApplyForce(BounceForce, _leftThighBody.Body.Position);
			_rightUpperArmBody.Body.ApplyForce(BounceForce, _rightUpperArmBody.Body.Position);
			_rightHandBody.Body.ApplyForce(BounceForce, _rightHandBody.Body.Position);
			_rightThighBody.Body.ApplyForce(BounceForce, _rightThighBody.Body.Position);
			IsClimbing = true;
			NeckAngleJoint.Softness = 0.98f;
			_bodyAngleJoint.Softness = 0.99f;
		}
		else
		{
			Climb_OFF_Ernest(_world);
			Climb_All_Out = true;
		}
	}

	public void Climb_OFF_Ernest(World _world)
	{
		if (IsClimbing)
		{
			_bodyBody.Restitution = 0f;
			_headBody.Restitution = 0f;
			_leftUpperArmBody.Restitution = 0f;
			_leftHandBody.Restitution = 0f;
			_leftThighBody.Restitution = 0f;
			_rightUpperArmBody.Restitution = 0f;
			_rightHandBody.Restitution = 0f;
			_rightThighBody.Restitution = 0f;
			_bodyBody.Body.IgnoreGravity = false;
			_headBody.Body.IgnoreGravity = false;
			_leftUpperArmBody.Body.IgnoreGravity = false;
			_leftHandBody.Body.IgnoreGravity = false;
			_leftThighBody.Body.IgnoreGravity = false;
			_rightUpperArmBody.Body.IgnoreGravity = false;
			_rightHandBody.Body.IgnoreGravity = false;
			_rightThighBody.Body.IgnoreGravity = false;
			MaxJointForce = 75f;
			Clear_X_Forces();
			NeckAngleJoint.Softness = 0f;
			_bodyAngleJoint.Softness = 0f;
			BounceHit = false;
			IsClimbing = false;
			if (_leftThighJoint != null)
			{
				_leftThighJoint.MotorSpeed = 0f;
				_leftThighJoint.MaxMotorTorque = 100000000f;
			}
			if (_rightThighJoint != null)
			{
				_rightThighJoint.MotorSpeed = 0f;
				_rightThighJoint.MaxMotorTorque = 100000000f;
			}
		}
		new Vector2(1f, 1f);
	}

	public void SlowTime_ON_Oscar(World _world)
	{
		if (!LeftArmSevered && PlayerMana > KineticShieldManaCost)
		{
			PlayerMana -= KineticShieldManaCost;
			Oscar_Update_Speed = 4;
		}
	}

	public void SlowTime_OFF_Oscar(World _world)
	{
		Oscar_Update_Speed = 1;
	}

	public void CreateBallLightning_Oscar(World _world)
	{
		_CannonBallZone[CannonBallIndex] = FixtureFactory.CreateCircle(_world, 30f, 1E-06f);
		_CannonBallZone[CannonBallIndex].Body.Position = Position * 0.2f;
		_CannonBallZone[CannonBallIndex].Body.BodyType = BodyType.Dynamic;
		_CannonBallZone[CannonBallIndex].Body.UserData = 133;
		_CannonBallZone[CannonBallIndex].UserData = 20;
		_CannonBallZone[CannonBallIndex].Body.Active = false;
		_CannonBallZone[CannonBallIndex].Body.IsBullet = false;
		Fixture obj = _CannonBallZone[CannonBallIndex];
		obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartBallLightning_OnCollision_Oscar_Zone));
		_CannonBallZone[CannonBallIndex].CollisionGroup = CollisionGroup;
		CannonBallDraw[CannonBallIndex] = true;
		_CannonBallBulletTimer[CannonBallIndex] = 10.0;
		_CannonBall[CannonBallIndex] = FixtureFactory.CreateEllipse(_world, 3f, 3.5f, 10, 10f);
		_CannonBall[CannonBallIndex].Body.Position = _rightHandBody.Body.Position;
		_CannonBall[CannonBallIndex].Body.BodyType = BodyType.Dynamic;
		_CannonBall[CannonBallIndex].Body.IsBullet = true;
		_CannonBall[CannonBallIndex].Body.SleepingAllowed = true;
		_CannonBall[CannonBallIndex].Density = 20000f;
		_CannonBall[CannonBallIndex].Friction = 0.9f;
		_CannonBall[CannonBallIndex].Restitution = 0.5f;
		_CannonBall[CannonBallIndex].Body.UserData = 120;
		_CannonBall[CannonBallIndex].Body.LinearDamping = 0f;
		_CannonBall[CannonBallIndex].UserData = CannonBallIndex + 1000;
		_CannonBall[CannonBallIndex].CollisionGroup = CollisionGroup;
		_CannonBall[CannonBallIndex].CollisionCategories = CollisionCategory.Cat27;
		_CannonBallTexture = level._DartLightningBallTexture;
		_CannonBallOrigin = new Vector2(_CannonBallTexture.Width / 2, _CannonBallTexture.Height / 2);
		Fixture obj2 = _CannonBall[CannonBallIndex];
		obj2.OnCollision = (CollisionEventHandler)Delegate.Combine(obj2.OnCollision, new CollisionEventHandler(DartBallLightning_OnCollision_Oscar));
		_CannonBall[CannonBallIndex].Body.ApplyTorque(-10f);
		_CannonBall[CannonBallIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(CannonBallForceScaler, CannonBallForceScaler));
		int num = 5000;
		if (DirectionRight)
		{
			_CannonBall[CannonBallIndex].Body.ApplyAngularImpulse(num);
		}
		else if (DirectionLeft)
		{
			_CannonBall[CannonBallIndex].Body.ApplyAngularImpulse(num);
		}
		else
		{
			_CannonBall[CannonBallIndex].Body.ApplyAngularImpulse(num);
		}
		CannonBallIndex++;
	}

	public void CreateEctoBall_Oscar(World _world)
	{
		if (PlayerMana > DartKineticManaCost)
		{
			PlayerMana -= DartKineticManaCost;
			_DartKineticBulletTimer[DartKineticIndex] = 10.0;
			KineticDraw[DartKineticIndex] = true;
			_DartKineticZone[DartKineticIndex] = FixtureFactory.CreateCircle(_world, 10f, 1E-06f);
			_DartKineticZone[DartKineticIndex].Body.Position = Position * 0.2f;
			_DartKineticZone[DartKineticIndex].Body.BodyType = BodyType.Dynamic;
			_DartKineticZone[DartKineticIndex].Body.UserData = 99;
			_DartKineticZone[DartKineticIndex].UserData = 20;
			_DartKineticZone[DartKineticIndex].Body.Active = false;
			_DartKineticZone[DartKineticIndex].IsSensor = true;
			Fixture obj = _DartKineticZone[DartKineticIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar_Zone));
			_DartKineticZone[DartKineticIndex].CollisionGroup = CollisionGroup;
			_DartKinetic[DartKineticIndex] = FixtureFactory.CreateRectangle(_world, 0.001f, 0.1f, 1E-09f);
			_DartKinetic[DartKineticIndex].Body.Position = _rightHandBody.Body.Position;
			_DartKinetic[DartKineticIndex].Body.BodyType = BodyType.Dynamic;
			_DartKinetic[DartKineticIndex].Body.IsBullet = true;
			_DartKinetic[DartKineticIndex].Body.IgnoreGravity = true;
			_DartKinetic[DartKineticIndex].Density = 2E-11f;
			_DartKinetic[DartKineticIndex].Friction = 0f;
			_DartKinetic[DartKineticIndex].Body.Mass = 0.01f;
			_DartKinetic[DartKineticIndex].Restitution = 0.8f;
			_DartKinetic[DartKineticIndex].Body.UserData = 199;
			_DartKinetic[DartKineticIndex].UserData = DartKineticIndex + 1000;
			_DartKinetic[DartKineticIndex].Body.LinearDamping = 0f;
			_DartKinetic[DartKineticIndex].CollisionGroup = CollisionGroup;
			_DartKineticTexture = level._DartKineticTexture;
			_DartKineticOrigin = new Vector2(_DartKineticTexture.Width / 2, _DartKineticTexture.Height / 2);
			Fixture obj2 = _DartKinetic[DartKineticIndex];
			obj2.OnCollision = (CollisionEventHandler)Delegate.Combine(obj2.OnCollision, new CollisionEventHandler(DartEctoBall_OnCollision_Oscar));
			_DartKinetic[DartKineticIndex].Body.Rotation = _rightUpperArmBody.Body.Rotation;
			_DartKinetic[DartKineticIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(DartKineticForceScaler, DartKineticForceScaler));
			DartKineticIndex++;
		}
	}

	public void Grab_ON_Rick(World _world, GameTime gameTime)
	{
		TelekinesisRangeScaler = 1000;
		Vector2 TelekinisisBodyHitPoint = default(Vector2);
		ref Vector2 reference = ref TelekinisisBodyHitPoint;
		reference = new Vector2(0f, 0f);
		if (RightArmSevered)
		{
			return;
		}
		if (PlayerMana > TelekinesisManaCost)
		{
			Telekinisis_Try = true;
			_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
			{
				Body body = f.Body;
				if (f.UserData != null && f.Body.UserData != null && (int)f.Body.UserData != 1 && f.CollisionGroup != CollisionGroup && f.Body.BodyType != BodyType.Static && (int)f.Body.UserData != 90 && (int)f.Body.UserData != 97 && (int)f.Body.UserData != 99 && (int)f.Body.UserData != 20)
				{
					TelekinisisBodyHitPoint = body.Position;
					TelekinesisHitSomthing = true;
					return 0f;
				}
				return -1f;
			}, _rightHandBody.Body.Position, _rightHandBody.Body.Position - (_rightHandBody.Body.Position - _rightUpperArmBody.Body.Position) * new Vector2(GrabRangeScaler, GrabRangeScaler));
		}
		if (TelekinesisHitSomthing)
		{
			foreach (Body body2 in _world.BodyList)
			{
				if (body2.UserData != (object)1 && body2.UserData != (object)0 && body2.UserData != (object)1 && body2.Position == TelekinisisBodyHitPoint)
				{
					TelekinisisBodyHit = body2.FixtureList[0];
					TelekinisisBodyHit.Body.Awake = true;
					TelekinisisBodyHitMassOLD = TelekinisisBodyHit.Body.Mass;
					TelekinisisBodyHit.Body.Mass = 0.0001f;
					TelekinisisBodyHitCollisionGroupOLD = TelekinisisBodyHit.CollisionGroup;
					TelekinisisBodyHit.CollisionGroup = CollisionGroup;
				}
			}
		}
		ref Vector2 reference2 = ref TelekinisisBodyHitPoint;
		reference2 = new Vector2(0f, 0f);
	}

	public void Grab_OFF_Rick(World _world, GameTime gameTime)
	{
		if (TelekinisisBodyHit != null)
		{
			TelekinisisBodyHit.Body.Mass = TelekinisisBodyHitMassOLD;
			TelekinisisBodyHit.CollisionGroup = (short)TelekinisisBodyHitCollisionGroupOLD;
			if (TelekinisisBodyHit != null)
			{
				TelekinisisBodyHit.Body.ApplyForce(_rightHandBody.Body.Position - (_rightHandBody.Body.Position - _rightUpperArmBody.Body.Position) * new Vector2(GrabThrowForceScaler, GrabThrowForceScaler), new Vector2(0f, 0f));
			}
		}
		Telekinisis_Try = false;
		TelekinesisHitSomthing = false;
		if (_rightHandBody != null && _rightHandBody.Body != null)
		{
			TelekinisisBodyHit_Point = _rightHandBody.Body.Position;
		}
		if (Grab_Joint != null)
		{
			_world.RemoveJoint(Grab_Joint);
			Grab_Joint = null;
		}
		TelekinisisBodyHit = null;
		TelekinisisBodyHitMassOLD = 0f;
		TelekinisisBodyHitCollisionGroupOLD = 0f;
		Grab_Joint_ON = false;
	}

	public void Slap_It_ON_Rick(World _world, GameTime gameTime)
	{
		if (LeftArmSevered)
		{
			return;
		}
		_leftHandBody.Body.UserData = 202;
		_leftUpperArmJoint.MotorEnabled = true;
		_leftUpperArmJoint.MaxMotorTorque = 100000000f;
		if (DirectionLeft)
		{
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorEnabled = true;
			}
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorSpeed = -400f;
			}
		}
		else if (DirectionRight)
		{
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorEnabled = true;
			}
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorSpeed = 400f;
			}
		}
		else
		{
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorEnabled = true;
			}
			if (_leftUpperArmJoint != null)
			{
				_leftUpperArmJoint.MotorSpeed = -400f;
			}
		}
	}

	public void Slap_It_OFF_Rick(World _world, GameTime gameTime)
	{
		_leftHandBody.Body.UserData = 8;
		if (_leftUpperArmJoint != null)
		{
			_leftUpperArmJoint.MotorEnabled = false;
		}
		if (_leftUpperArmJoint != null)
		{
			_leftUpperArmJoint.MotorSpeed = 0f;
		}
	}

	public void Get_Grab_Movements(World _world)
	{
		if (!RightArmSevered && !Grab_Joint_ON && TelekinisisBodyHit != null && TelekinisisBodyHit.Body != null && _rightHandBody != null && _rightHandBody.Body != null && Grab_Joint == null)
		{
			Grab_Joint = new RevoluteJoint(_rightHandBody.Body, TelekinisisBodyHit.Body, new Vector2(0f, 0f), new Vector2(0f, 0f));
			_world.AddJoint(Grab_Joint);
			Grab_Joint_ON = true;
		}
	}

	public void RockSkin_ON_Rick(World _world)
	{
		if (!Juggernaut && !LeftArmSevered && PlayerMana > KineticShieldManaCost)
		{
			PlayerMana -= KineticShieldManaCost;
			RunLimit = 1f;
			RockSkin_ON = true;
			_bodyBody.Body.AngularDamping = 500f;
			_headBody.Body.AngularDamping = 500f;
			_leftUpperArmBody.Body.AngularDamping = 500f;
			_leftThighBody.Body.AngularDamping = 500f;
			_leftHandBody.Body.AngularDamping = 500f;
			_rightUpperArmBody.Body.AngularDamping = 500f;
			_rightThighBody.Body.AngularDamping = 500f;
			_rightHandBody.Body.AngularDamping = 500f;
			if (!PlayerHPBody_OLD_Hold_First)
			{
				PlayerHPBody_OLD = PlayerHPBody;
				PlayerHPBody_OLD_Hold_First = true;
			}
			PlayerHPBody = PlayerHPBody_OLD;
		}
	}

	public void RockSkin_OFF_Rick(World _world)
	{
		RunLimit = 0.1f;
		Color = Color.White;
		ColorLeftHand = Color.White;
		ColorLeftArm = Color.White;
		ColorLeftLeg = Color.White;
		ColorRightHand = Color.White;
		ColorRightArm = Color.White;
		ColorRightLeg = Color.White;
		RockSkin_ON = false;
		PlayerHPBody_OLD_Hold_First = false;
	}

	public void CreateRock_Rick(World _world)
	{
		_CannonBallZone[CannonBallIndex] = FixtureFactory.CreateCircle(_world, 30f, 1E-06f);
		_CannonBallZone[CannonBallIndex].Body.Position = Position * 0.2f;
		_CannonBallZone[CannonBallIndex].Body.BodyType = BodyType.Dynamic;
		_CannonBallZone[CannonBallIndex].Body.UserData = 99;
		_CannonBallZone[CannonBallIndex].UserData = 20;
		_CannonBallZone[CannonBallIndex].Body.Active = false;
		_CannonBallZone[CannonBallIndex].IsSensor = true;
		Fixture obj = _CannonBallZone[CannonBallIndex];
		obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartRock_OnCollision_Rick_Zone));
		CannonBallDraw[CannonBallIndex] = true;
		_CannonBallBulletTimer[CannonBallIndex] = 10.0;
		_CannonBall[CannonBallIndex] = FixtureFactory.CreateEllipse(_world, 3f, 3.5f, 10, 1f);
		_CannonBall[CannonBallIndex].Body.Position = _rightHandBody.Body.Position;
		_CannonBall[CannonBallIndex].Body.BodyType = BodyType.Dynamic;
		_CannonBall[CannonBallIndex].Body.IsBullet = true;
		_CannonBall[CannonBallIndex].Body.SleepingAllowed = true;
		_CannonBall[CannonBallIndex].Density = 20000f;
		_CannonBall[CannonBallIndex].Friction = 1f;
		_CannonBall[CannonBallIndex].Restitution = 0f;
		_CannonBall[CannonBallIndex].Body.UserData = 120;
		_CannonBall[CannonBallIndex].Body.LinearDamping = 0f;
		_CannonBall[CannonBallIndex].UserData = CannonBallIndex + 1000;
		_CannonBall[CannonBallIndex].CollisionGroup = CollisionGroup;
		_CannonBall[CannonBallIndex].CollisionCategories = CollisionCategory.Cat27;
		_CannonBallTexture = level._DartRockBallTexture;
		_CannonBallOrigin = new Vector2(_CannonBallTexture.Width / 2, _CannonBallTexture.Height / 2);
		Fixture obj2 = _CannonBall[CannonBallIndex];
		obj2.OnCollision = (CollisionEventHandler)Delegate.Combine(obj2.OnCollision, new CollisionEventHandler(DartRock_OnCollision_Rick));
		_CannonBall[CannonBallIndex].Body.ApplyTorque(-10f);
		_CannonBall[CannonBallIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(CannonBallForceScaler, CannonBallForceScaler));
		int num = 5000;
		if (DirectionRight)
		{
			_CannonBall[CannonBallIndex].Body.ApplyAngularImpulse(num);
		}
		else if (DirectionLeft)
		{
			_CannonBall[CannonBallIndex].Body.ApplyAngularImpulse(num);
		}
		else
		{
			_CannonBall[CannonBallIndex].Body.ApplyAngularImpulse(num);
		}
		CannonBallIndex++;
	}

	public void Fly_ON_Vinny(World _world)
	{
		if (Fly_All_Out)
		{
			return;
		}
		if (PlayerMana > FlyManaCost)
		{
			PlayerMana -= FlyManaCost;
			Vector2 vector = new Vector2(gamePadState_ThumbSticks_Left_X, gamePadState_ThumbSticks_Left_Y - gamePadState_ThumbSticks_Left_Y * 2f);
			Vector2 vector2 = new Vector2(5f, 5f);
			vector = -vector * -vector2;
			_headBody.Body.ApplyForce(vector);
			NeckAngleJoint.Softness = 0.98f;
			_bodyAngleJoint.Softness = 0.99f;
			if (_leftThighJoint != null)
			{
				_leftThighJoint.MotorSpeed = 0f;
				_leftThighJoint.MaxMotorTorque = 0f;
			}
			if (_rightThighJoint != null)
			{
				_rightThighJoint.MotorSpeed = 0f;
				_rightThighJoint.MaxMotorTorque = 0f;
			}
			_bodyBody.Body.IgnoreGravity = true;
			_headBody.Body.IgnoreGravity = true;
			_leftUpperArmBody.Body.IgnoreGravity = true;
			_leftThighBody.Body.IgnoreGravity = true;
			_leftHandBody.Body.IgnoreGravity = true;
			_rightUpperArmBody.Body.IgnoreGravity = true;
			_rightThighBody.Body.IgnoreGravity = true;
			_rightHandBody.Body.IgnoreGravity = true;
			IsFlying = true;
		}
		else
		{
			Fly_OFF_Vinny(_world);
			Fly_All_Out = true;
		}
	}

	public void Fly_OFF_Vinny(World _world)
	{
		if (IsFlying)
		{
			NeckAngleJoint.Softness = 0f;
			_bodyAngleJoint.Softness = 0f;
			if (_leftThighJoint != null)
			{
				_leftThighJoint.MotorSpeed = 0f;
				_leftThighJoint.MaxMotorTorque = 100000000f;
			}
			if (_rightThighJoint != null)
			{
				_rightThighJoint.MotorSpeed = 0f;
				_rightThighJoint.MaxMotorTorque = 100000000f;
			}
			_bodyBody.Body.IgnoreGravity = false;
			_headBody.Body.IgnoreGravity = false;
			_leftUpperArmBody.Body.IgnoreGravity = false;
			_leftThighBody.Body.IgnoreGravity = false;
			_leftHandBody.Body.IgnoreGravity = false;
			_rightUpperArmBody.Body.IgnoreGravity = false;
			_rightThighBody.Body.IgnoreGravity = false;
			_rightHandBody.Body.IgnoreGravity = false;
			IsFlying = false;
		}
	}

	public void Claw_ON_Vinny(World _world)
	{
		_leftHandBody_Claw.Body.Active = true;
		_rightHandBody_Claw.Body.Active = true;
		_leftHandBody_Claw.CollidesWith = CollisionCategory.All;
		_rightHandBody_Claw.CollidesWith = CollisionCategory.All;
		_leftHandBody_Claw.CollisionCategories = CollisionCategory.All;
		_rightHandBody_Claw.CollisionCategories = CollisionCategory.All;
		if (DirectionLeft)
		{
			_leftHandBody_Claw.Body.Rotation++;
			_rightHandBody_Claw.Body.Rotation++;
		}
		else if (DirectionRight)
		{
			_leftHandBody_Claw.Body.Rotation--;
			_rightHandBody_Claw.Body.Rotation--;
		}
		else
		{
			_leftHandBody_Claw.Body.Rotation++;
			_rightHandBody_Claw.Body.Rotation++;
		}
		IsClawing = true;
	}

	public void Claw_OFF_Vinny(World _world)
	{
		if (!IsClawing)
		{
			_leftHandBody_Claw.Body.Active = false;
			_rightHandBody_Claw.Body.Active = false;
			_leftHandBody_Claw.CollidesWith = CollisionCategory.None;
			_rightHandBody_Claw.CollidesWith = CollisionCategory.None;
			_leftHandBody_Claw.CollisionCategories = CollisionCategory.None;
			_rightHandBody_Claw.CollisionCategories = CollisionCategory.None;
		}
	}

	public void Particle_Freeze(GameTime gametime, World _world)
	{
		OldGameTime = gametime.TotalGameTime.TotalSeconds;
		FreezeJoint1 = new WeldJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		if (!LeftArmSevered)
		{
			FreezeJoint2 = new WeldJoint(_bodyBody.Body, _leftUpperArmBody.Body, new Vector2(-2f, -2.2f), new Vector2(0f, -3.3f));
			_world.AddJoint(FreezeJoint2);
			FreezeJoint2.CollideConnected = false;
			ColorLeftArm = Color.CadetBlue;
			ColorLeftHand = Color.CadetBlue;
		}
		if (!RightArmSevered)
		{
			FreezeJoint3 = new WeldJoint(_bodyBody.Body, _rightUpperArmBody.Body, new Vector2(2f, -2.2f), new Vector2(0f, -3.3f));
			_world.AddJoint(FreezeJoint3);
			FreezeJoint3.CollideConnected = false;
			ColorRightArm = Color.CadetBlue;
			ColorRightHand = Color.CadetBlue;
		}
		if (!LeftLegSevered)
		{
			FreezeJoint4 = new WeldJoint(_bodyBody.Body, _leftThighBody.Body, new Vector2(-2f, 2.9f), new Vector2(0f, -5.2000003f));
			FreezeJoint4.CollideConnected = false;
			_world.AddJoint(FreezeJoint4);
			ColorLeftLeg = Color.CadetBlue;
		}
		if (!RightLegSevered)
		{
			FreezeJoint5 = new WeldJoint(_bodyBody.Body, _rightThighBody.Body, new Vector2(2f, 2.9f), new Vector2(0f, -5.2000003f));
			FreezeJoint5.CollideConnected = false;
			_world.AddJoint(FreezeJoint5);
			ColorRightLeg = Color.CadetBlue;
		}
		FreezeJoint6 = new WeldJoint(_leftUpperArmBody.Body, _leftHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		FreezeJoint7 = new WeldJoint(_rightUpperArmBody.Body, _rightHandBody.Body, new Vector2(0f, 1.3000001f), new Vector2(0f, 0f));
		FreezeJoint1.CollideConnected = false;
		FreezeJoint6.CollideConnected = false;
		FreezeJoint7.CollideConnected = false;
		_world.AddJoint(FreezeJoint1);
		_world.AddJoint(FreezeJoint6);
		_world.AddJoint(FreezeJoint7);
		Color = Color.CadetBlue;
		Freezer = false;
		Frozen = true;
	}

	public void Particle_UnFreeze(GameTime gameTime, World _world)
	{
		if (Frozen)
		{
			if (FreezeJoint1 != null)
			{
				_world.RemoveJoint(FreezeJoint1);
			}
			if (FreezeJoint2 != null)
			{
				_world.RemoveJoint(FreezeJoint2);
			}
			if (FreezeJoint3 != null)
			{
				_world.RemoveJoint(FreezeJoint3);
			}
			if (FreezeJoint4 != null)
			{
				_world.RemoveJoint(FreezeJoint4);
			}
			if (FreezeJoint5 != null)
			{
				_world.RemoveJoint(FreezeJoint5);
			}
			if (FreezeJoint6 != null)
			{
				_world.RemoveJoint(FreezeJoint6);
			}
			if (FreezeJoint7 != null)
			{
				_world.RemoveJoint(FreezeJoint7);
			}
			Color = Color.White;
			ColorLeftHand = Color.White;
			ColorLeftArm = Color.White;
			ColorLeftLeg = Color.White;
			ColorRightHand = Color.White;
			ColorRightArm = Color.White;
			ColorRightLeg = Color.White;
			OldGameTime = 0.0;
			Frozen = false;
		}
	}

	public void Particle_Shock(GameTime gametime, World _world)
	{
		OldGameTime = gametime.TotalGameTime.TotalSeconds;
		LimpJoints();
		Unconscious = true;
		Smoking = true;
		OldUnconsciousTime = gametime.TotalGameTime.TotalSeconds;
		Color = Color.LightSteelBlue;
		ColorLeftHand = Color.LightSteelBlue;
		ColorLeftArm = Color.LightSteelBlue;
		ColorLeftLeg = Color.LightSteelBlue;
		ColorRightHand = Color.LightSteelBlue;
		ColorRightArm = Color.LightSteelBlue;
		ColorRightLeg = Color.LightSteelBlue;
		Shocker = false;
		Shocked = true;
	}

	public void Particle_UnShock(GameTime gameTime, World _world)
	{
		if (Shocked)
		{
			Smoking = false;
			Unconscious = false;
			Color = Color.White;
			ColorLeftHand = Color.White;
			ColorLeftArm = Color.White;
			ColorLeftLeg = Color.White;
			ColorRightHand = Color.White;
			ColorRightArm = Color.White;
			ColorRightLeg = Color.White;
			OldGameTime = 0.0;
			Shocked = false;
		}
	}

	private void Bounce()
	{
		Vector2 vector = new Vector2(0f, -10000f);
		float num = (float)Math.Cos((float)ForceFixB.UserData);
		float num2 = (float)Math.Sin((float)ForceFixB.UserData);
		vector = new Vector2(vector.X * num - vector.Y * num2, vector.X * num2 + vector.Y * num);
		_headBody.Body.LinearVelocity += vector;
		_bodyBody.Body.LinearVelocity += vector;
		_leftUpperArmBody.Body.LinearVelocity += vector;
		_leftHandBody.Body.LinearVelocity += vector;
		_leftThighBody.Body.LinearVelocity += vector;
		_rightUpperArmBody.Body.LinearVelocity += vector;
		_rightHandBody.Body.LinearVelocity += vector;
		_rightThighBody.Body.LinearVelocity += vector;
	}

	private void ForcePush()
	{
		Vector2 vector = new Vector2(0f, -20f);
		float num = (float)Math.Cos((float)ForceFixB.UserData);
		float num2 = (float)Math.Sin((float)ForceFixB.UserData);
		vector = new Vector2(vector.X * num - vector.Y * num2, vector.X * num2 + vector.Y * num);
		_headBody.Body.ApplyForce(vector);
	}

	private void ForceX()
	{
		Vector2 vector = new Vector2(10000000f, 10000000f);
		_headBody.Body.ApplyForce(_headBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_bodyBody.Body.ApplyForce(_bodyBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_leftHandBody.Body.ApplyForce(_leftHandBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_leftUpperArmBody.Body.ApplyForce(_leftUpperArmBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_leftThighBody.Body.ApplyForce(_leftThighBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_rightHandBody.Body.ApplyForce(_rightHandBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_rightUpperArmBody.Body.ApplyForce(_rightUpperArmBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		_rightThighBody.Body.ApplyForce(_rightThighBody.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		if (_leftHandBody_Claw != null)
		{
			_leftHandBody_Claw.Body.ApplyForce(_leftHandBody_Claw.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		}
		if (_rightHandBody_Claw != null)
		{
			_rightHandBody_Claw.Body.ApplyForce(_rightHandBody_Claw.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		}
		if (_kineticShields != null)
		{
			_kineticShields.Body.ApplyForce(_kineticShields.Body.LinearVelocity * vector, ForceFixB.Body.WorldCenter);
		}
	}

	private void UpdateFreeze(GameTime gametime, World _world)
	{
		if (Frozen)
		{
			int num = 10;
			if (gametime.TotalGameTime.TotalSeconds - (double)num > OldGameTime)
			{
				Particle_UnFreeze(gametime, _world);
			}
		}
	}

	private void UpdateShock(GameTime gametime, World _world)
	{
		if (Shocked)
		{
			int num = 10;
			Color = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			ColorLeftHand = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			ColorLeftArm = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			ColorLeftLeg = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			ColorRightHand = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			ColorRightArm = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			ColorRightLeg = new Color((byte)level.random.Next(100, 150), 100, (byte)level.random.Next(150, 255));
			if (gametime.TotalGameTime.TotalSeconds - (double)num > OldGameTime)
			{
				Particle_UnShock(gametime, _world);
			}
		}
	}

	public void Get_Arm_Movements()
	{
		if (!RightArmSevered)
		{
			RightHandForce = new Vector2(gamePadState_ThumbSticks_Right_X - gamePadState_ThumbSticks_Right_X * 2f, gamePadState_ThumbSticks_Right_Y);
			if (RightHandForce.Length() > 0.1f)
			{
				_rightHandBody.Body.AngularDamping = 100f;
				_rightUpperArmBody.Body.AngularDamping = 100f;
				_SightON = true;
			}
			else
			{
				_rightHandBody.Body.AngularDamping = 0f;
				_rightUpperArmBody.Body.AngularDamping = 0f;
				_SightON = false;
			}
			ForceScalerRight = new Vector2(10f, 10f);
			RightHandForce = -RightHandForce * ForceScalerRight;
			_rightHandBody.Body.ApplyForce(new Vector2(RightHandForce.X, RightHandForce.Y));
			_bodyBody.Body.ApplyForce(new Vector2(RightHandForce.X - RightHandForce.X * 2f, RightHandForce.Y - RightHandForce.Y * 2f));
		}
		if (LeftArmSevered)
		{
			return;
		}
		if (LeftArmRotation > -1E-07f)
		{
			if (!(LeftArmRotation < 1E-07f))
			{
				LeftArmRotation -= (float)Math.PI * 2f;
				LeftArmRotation = 4.712389f - LeftArmRotation;
			}
		}
		else
		{
			LeftArmRotation -= (float)Math.PI * 2f;
			LeftArmRotation = 4.712389f - LeftArmRotation;
		}
	}

	public void Get_Tele_Movements()
	{
		if (!RightArmSevered)
		{
			RightHandForce = new Vector2(gamePadState_ThumbSticks_Right_X - gamePadState_ThumbSticks_Right_X * 2f, gamePadState_ThumbSticks_Right_Y);
			ForceScalerRight = new Vector2(1000f, 1000f);
			RightHandForce = -RightHandForce * ForceScalerRight;
			TelekinisisBodyHit.Body.ApplyForce(new Vector2(RightHandForce.X, RightHandForce.Y), TelekinisisBodyHit_Point);
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game, Matrix cameraTransform)
	{
		BlendState blendState = BlendState.AlphaBlend;
		if (PhaseShift)
		{
			blendState = BlendState.Additive;
		}
		if (RockSkin_ON)
		{
			Draw_Rock_Skin(gameTime, spriteBatch, game, cameraTransform, blendState);
			return;
		}
		if (PhaseShift)
		{
			Draw_Ghost_Walk(gameTime, spriteBatch, game, cameraTransform, blendState);
			return;
		}
		spriteBatch.Begin(SpriteSortMode.Immediate, blendState, null, null, null, null, cameraTransform);
		if (!DeadByBounds)
		{
			if (Dead)
			{
				if (Player1Index)
				{
					for (int i = 0; i < game.P1FlairOld.Length; i++)
					{
						_ = game.P1FlairOld_Tag[i];
						if (game.P1FlairOld_Tag[i] == 7)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[i], 150 * i, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int j = 0; j < game.P2FlairOld.Length; j++)
					{
						_ = game.P2FlairOld_Tag[j];
						if (game.P2FlairOld_Tag[j] == 7)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[j], 150 * j, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int k = 0; k < game.P3FlairOld.Length; k++)
					{
						_ = game.P3FlairOld_Tag[k];
						if (game.P3FlairOld_Tag[k] == 7)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[k], 150 * k, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int l = 0; l < game.P4FlairOld.Length; l++)
					{
						_ = game.P4FlairOld_Tag[l];
						if (game.P4FlairOld_Tag[l] == 7)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[l], 150 * l, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (!_rightThighBodyGone)
				{
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
				}
				if (Weapon_Armed)
				{
					spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
				}
				if (!_rightUpperArmBodyGone)
				{
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
				}
				if (!_rightHandBodyGone)
				{
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
				if (!_bodyBodyGone)
				{
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
				}
				if (Player1Index)
				{
					for (int m = 0; m < game.P1FlairOld.Length; m++)
					{
						_ = game.P1FlairOld_Tag[m];
						if (game.P1FlairOld_Tag[m] == 6)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[m], 150 * m, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int n = 0; n < game.P2FlairOld.Length; n++)
					{
						_ = game.P2FlairOld_Tag[n];
						if (game.P2FlairOld_Tag[n] == 6)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[n], 150 * n, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int num = 0; num < game.P3FlairOld.Length; num++)
					{
						_ = game.P3FlairOld_Tag[num];
						if (game.P3FlairOld_Tag[num] == 6)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num], 150 * num, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int num2 = 0; num2 < game.P4FlairOld.Length; num2++)
					{
						_ = game.P4FlairOld_Tag[num2];
						if (game.P4FlairOld_Tag[num2] == 6)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num2], 150 * num2, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (_headBody.Body != null)
				{
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
				}
				if (Player1Index)
				{
					for (int num3 = 0; num3 < game.P1FlairOld.Length; num3++)
					{
						_ = game.P1FlairOld_Tag[num3];
						if (game.P1FlairOld_Tag[num3] == 0)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 1)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 2)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 3)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 4)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 5)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int num4 = 0; num4 < game.P2FlairOld.Length; num4++)
					{
						_ = game.P2FlairOld_Tag[num4];
						if (game.P2FlairOld_Tag[num4] == 0)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 1)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 2)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 3)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 4)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 5)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int num5 = 0; num5 < game.P3FlairOld.Length; num5++)
					{
						_ = game.P3FlairOld_Tag[num5];
						if (game.P3FlairOld_Tag[num5] == 0)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 1)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 2)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 3)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 4)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 5)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int num6 = 0; num6 < game.P4FlairOld.Length; num6++)
					{
						_ = game.P4FlairOld_Tag[num6];
						if (game.P4FlairOld_Tag[num6] == 0)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 1)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 2)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 3)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 4)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 5)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (!_leftThighBodyGone)
				{
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
				}
				if (!_leftUpperArmBodyGone)
				{
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
				}
				if (!_leftHandBodyGone)
				{
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftHandBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
			}
			if (Active)
			{
				for (int num7 = 0; num7 < DartBoneIndex; num7++)
				{
					if (_DartBone[num7] != null && _DartBone[num7].Body != null && _DartBoneBulletTimer[num7] > 5.0)
					{
						spriteBatch.Draw(_DartBoneTexture, _DartBone[num7].Body.Position * PhysicsScaleUp, null, DartBoneColor, _DartBone[num7].Body.Rotation, _DartBoneOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num8 = 0; num8 < DartHarpoonIndex; num8++)
				{
					if (_DartHarpoon[num8] != null && _DartHarpoon[num8].Body != null && _DartHarpoonBulletTimer[num8] > 5.0)
					{
						spriteBatch.Draw(_DartHarpoonTexture, _DartHarpoon[num8].Body.Position * PhysicsScaleUp, null, DartHarpoonColor, _DartHarpoon[num8].Body.Rotation, _DartHarpoonOrigin, 0.75f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num9 = 0; num9 < DartKineticIndex; num9++)
				{
					if (KineticDraw[num9] && _DartKinetic[num9] != null && _DartKinetic[num9].Body != null && _DartKineticBulletTimer[num9] > 5.0)
					{
						spriteBatch.Draw(_DartKineticTexture, _DartKinetic[num9].Body.Position * PhysicsScaleUp, null, DartKineticColor, _DartKinetic[num9].Body.Rotation, _DartKineticOrigin, 0.25f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num10 = 0; num10 < CannonBallIndex; num10++)
				{
					if (CannonBallDraw[num10] && _CannonBall != null && _CannonBall[num10] != null && _CannonBall[num10].Body != null)
					{
						spriteBatch.Draw(_CannonBallTexture, _CannonBall[num10].Body.Position * PhysicsScaleUp, null, Color.White, _CannonBall[num10].Body.Rotation, _CannonBallOrigin, 1f, SpriteEffects.FlipVertically, 1f);
						CannonBallColor = Color.White;
					}
				}
				for (int num11 = 0; num11 < DartStasisIndex; num11++)
				{
					if (StasisDraw[num11] && _DartStasis[num11] != null && _DartStasis[num11].Body != null && _DartStasisBulletTimer[num11] > 5.0)
					{
						spriteBatch.Draw(_DartStasisTexture, _DartStasis[num11].Body.Position * PhysicsScaleUp, null, DartStasisColor, _DartStasis[num11].Body.Rotation, _DartStasisOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
					}
				}
				if (DirectionRight)
				{
					if (Player1Index)
					{
						for (int num12 = 0; num12 < game.P1FlairOld.Length; num12++)
						{
							_ = game.P1FlairOld_Tag[num12];
							if (game.P1FlairOld_Tag[num12] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num12], 150 * num12, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num13 = 0; num13 < game.P2FlairOld.Length; num13++)
						{
							_ = game.P2FlairOld_Tag[num13];
							if (game.P2FlairOld_Tag[num13] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num13], 150 * num13, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num14 = 0; num14 < game.P3FlairOld.Length; num14++)
						{
							_ = game.P3FlairOld_Tag[num14];
							if (game.P3FlairOld_Tag[num14] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num14], 150 * num14, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num15 = 0; num15 < game.P4FlairOld.Length; num15++)
						{
							_ = game.P4FlairOld_Tag[num15];
							if (game.P4FlairOld_Tag[num15] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num15], 150 * num15, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (!_rightHandBodyGone && _SightON)
					{
						spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num16 = 0; num16 < game.P1FlairOld.Length; num16++)
						{
							_ = game.P1FlairOld_Tag[num16];
							if (game.P1FlairOld_Tag[num16] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num16], 150 * num16, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num17 = 0; num17 < game.P2FlairOld.Length; num17++)
						{
							_ = game.P2FlairOld_Tag[num17];
							if (game.P2FlairOld_Tag[num17] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num17], 150 * num17, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num18 = 0; num18 < game.P3FlairOld.Length; num18++)
						{
							_ = game.P3FlairOld_Tag[num18];
							if (game.P3FlairOld_Tag[num18] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num18], 150 * num18, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num19 = 0; num19 < game.P4FlairOld.Length; num19++)
						{
							_ = game.P4FlairOld_Tag[num19];
							if (game.P4FlairOld_Tag[num19] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num19], 150 * num19, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num20 = 0; num20 < game.P1FlairOld.Length; num20++)
						{
							_ = game.P1FlairOld_Tag[num20];
							if (game.P1FlairOld_Tag[num20] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num21 = 0; num21 < game.P2FlairOld.Length; num21++)
						{
							_ = game.P2FlairOld_Tag[num21];
							if (game.P2FlairOld_Tag[num21] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num22 = 0; num22 < game.P3FlairOld.Length; num22++)
						{
							_ = game.P3FlairOld_Tag[num22];
							if (game.P3FlairOld_Tag[num22] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num23 = 0; num23 < game.P4FlairOld.Length; num23++)
						{
							_ = game.P4FlairOld_Tag[num23];
							if (game.P4FlairOld_Tag[num23] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
				if (DirectionLeft)
				{
					if (Player1Index)
					{
						for (int num24 = 0; num24 < game.P1FlairOld.Length; num24++)
						{
							_ = game.P1FlairOld_Tag[num24];
							if (game.P1FlairOld_Tag[num24] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num24], 150 * num24, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 57500f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num25 = 0; num25 < game.P2FlairOld.Length; num25++)
						{
							_ = game.P2FlairOld_Tag[num25];
							if (game.P2FlairOld_Tag[num25] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num25], 150 * num25, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num26 = 0; num26 < game.P3FlairOld.Length; num26++)
						{
							_ = game.P3FlairOld_Tag[num26];
							if (game.P3FlairOld_Tag[num26] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num26], 150 * num26, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num27 = 0; num27 < game.P4FlairOld.Length; num27++)
						{
							_ = game.P4FlairOld_Tag[num27];
							if (game.P4FlairOld_Tag[num27] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num27], 150 * num27, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipHorizontally, 1f);
					if (Player1Index)
					{
						for (int num28 = 0; num28 < game.P1FlairOld.Length; num28++)
						{
							_ = game.P1FlairOld_Tag[num28];
							if (game.P1FlairOld_Tag[num28] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num28], 150 * num28, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num29 = 0; num29 < game.P2FlairOld.Length; num29++)
						{
							_ = game.P2FlairOld_Tag[num29];
							if (game.P2FlairOld_Tag[num29] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num29], 150 * num29, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num30 = 0; num30 < game.P3FlairOld.Length; num30++)
						{
							_ = game.P3FlairOld_Tag[num30];
							if (game.P3FlairOld_Tag[num30] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num30], 150 * num30, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num31 = 0; num31 < game.P4FlairOld.Length; num31++)
						{
							_ = game.P4FlairOld_Tag[num31];
							if (game.P4FlairOld_Tag[num31] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num31], 150 * num31, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 3f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.FlipHorizontally, 1f);
					if (Player1Index)
					{
						for (int num32 = 0; num32 < game.P1FlairOld.Length; num32++)
						{
							_ = game.P1FlairOld_Tag[num32];
							if (game.P1FlairOld_Tag[num32] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num33 = 0; num33 < game.P2FlairOld.Length; num33++)
						{
							_ = game.P2FlairOld_Tag[num33];
							if (game.P2FlairOld_Tag[num33] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num34 = 0; num34 < game.P3FlairOld.Length; num34++)
						{
							_ = game.P3FlairOld_Tag[num34];
							if (game.P3FlairOld_Tag[num34] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num35 = 0; num35 < game.P4FlairOld.Length; num35++)
						{
							_ = game.P4FlairOld_Tag[num35];
							if (game.P4FlairOld_Tag[num35] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (!_rightHandBodyGone && _SightON)
					{
						spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
				}
				if (!DirectionLeft && !DirectionRight)
				{
					if (Player1Index)
					{
						for (int num36 = 0; num36 < game.P1FlairOld.Length; num36++)
						{
							_ = game.P1FlairOld_Tag[num36];
							if (game.P1FlairOld_Tag[num36] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num36], 150 * num36, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num37 = 0; num37 < game.P2FlairOld.Length; num37++)
						{
							_ = game.P2FlairOld_Tag[num37];
							if (game.P2FlairOld_Tag[num37] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num37], 150 * num37, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num38 = 0; num38 < game.P3FlairOld.Length; num38++)
						{
							_ = game.P3FlairOld_Tag[num38];
							if (game.P3FlairOld_Tag[num38] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num38], 150 * num38, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num39 = 0; num39 < game.P4FlairOld.Length; num39++)
						{
							_ = game.P4FlairOld_Tag[num39];
							if (game.P4FlairOld_Tag[num39] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num39], 150 * num39, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (!_rightHandBodyGone)
					{
						if (_SightON)
						{
							spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
						}
						if (TelekinesisHitSomthing)
						{
							Vector2 value = _rightHandBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
							Vector2 value2 = TelekinisisBodyHit.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
							Color color = new Color(150, (byte)level.random.Next(150, 255), 150);
							float rotation = (float)Math.Atan2(value2.Y - value.Y, value2.X - value.X);
							float x = Vector2.Distance(value, value2);
							Texture2D texture2D = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
							texture2D.SetData(new Color[1] { Color.White });
							spriteBatch.Draw(texture2D, value, null, color, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, 0f);
						}
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num40 = 0; num40 < game.P1FlairOld.Length; num40++)
						{
							_ = game.P1FlairOld_Tag[num40];
							if (game.P1FlairOld_Tag[num40] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num40], 150 * num40, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num41 = 0; num41 < game.P2FlairOld.Length; num41++)
						{
							_ = game.P2FlairOld_Tag[num41];
							if (game.P2FlairOld_Tag[num41] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num41], 150 * num41, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num42 = 0; num42 < game.P3FlairOld.Length; num42++)
						{
							_ = game.P3FlairOld_Tag[num42];
							if (game.P3FlairOld_Tag[num42] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num42], 150 * num42, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num43 = 0; num43 < game.P4FlairOld.Length; num43++)
						{
							_ = game.P4FlairOld_Tag[num43];
							if (game.P4FlairOld_Tag[num43] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num43], 150 * num43, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num44 = 0; num44 < game.P1FlairOld.Length; num44++)
						{
							_ = game.P1FlairOld_Tag[num44];
							if (game.P1FlairOld_Tag[num44] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num45 = 0; num45 < game.P2FlairOld.Length; num45++)
						{
							_ = game.P2FlairOld_Tag[num45];
							if (game.P2FlairOld_Tag[num45] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num46 = 0; num46 < game.P3FlairOld.Length; num46++)
						{
							_ = game.P3FlairOld_Tag[num46];
							if (game.P3FlairOld_Tag[num46] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num47 = 0; num47 < game.P4FlairOld.Length; num47++)
						{
							_ = game.P4FlairOld_Tag[num47];
							if (game.P4FlairOld_Tag[num47] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
			}
		}
		spriteBatch.End();
	}

	public void Draw_Rock_Skin(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game, Matrix cameraTransform, BlendState Blender)
	{
		spriteBatch.Begin(SpriteSortMode.Immediate, Blender, null, null, null, null, cameraTransform);
		Color = Color.DarkGreen;
		ColorLeftArm = Color.DarkGreen;
		ColorLeftLeg = Color.DarkGreen;
		ColorLeftHand = Color.DarkGreen;
		ColorRightArm = Color.DarkGreen;
		ColorRightHand = Color.DarkGreen;
		ColorRightLeg = Color.DarkGreen;
		if (!DeadByBounds)
		{
			if (Dead)
			{
				if (Player1Index)
				{
					for (int i = 0; i < game.P1FlairOld.Length; i++)
					{
						_ = game.P1FlairOld_Tag[i];
						if (game.P1FlairOld_Tag[i] == 7)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[i], 150 * i, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int j = 0; j < game.P2FlairOld.Length; j++)
					{
						_ = game.P2FlairOld_Tag[j];
						if (game.P2FlairOld_Tag[j] == 7)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[j], 150 * j, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int k = 0; k < game.P3FlairOld.Length; k++)
					{
						_ = game.P3FlairOld_Tag[k];
						if (game.P3FlairOld_Tag[k] == 7)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[k], 150 * k, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int l = 0; l < game.P4FlairOld.Length; l++)
					{
						_ = game.P4FlairOld_Tag[l];
						if (game.P4FlairOld_Tag[l] == 7)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[l], 150 * l, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (!_rightThighBodyGone)
				{
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
				}
				if (Weapon_Armed)
				{
					spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
				}
				if (!_rightUpperArmBodyGone)
				{
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
				}
				if (!_rightHandBodyGone)
				{
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
				if (!_bodyBodyGone)
				{
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
				}
				if (Player1Index)
				{
					for (int m = 0; m < game.P1FlairOld.Length; m++)
					{
						_ = game.P1FlairOld_Tag[m];
						if (game.P1FlairOld_Tag[m] == 6)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[m], 150 * m, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int n = 0; n < game.P2FlairOld.Length; n++)
					{
						_ = game.P2FlairOld_Tag[n];
						if (game.P2FlairOld_Tag[n] == 6)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[n], 150 * n, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int num = 0; num < game.P3FlairOld.Length; num++)
					{
						_ = game.P3FlairOld_Tag[num];
						if (game.P3FlairOld_Tag[num] == 6)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num], 150 * num, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int num2 = 0; num2 < game.P4FlairOld.Length; num2++)
					{
						_ = game.P4FlairOld_Tag[num2];
						if (game.P4FlairOld_Tag[num2] == 6)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num2], 150 * num2, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (_headBody.Body != null)
				{
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
				}
				if (Player1Index)
				{
					for (int num3 = 0; num3 < game.P1FlairOld.Length; num3++)
					{
						_ = game.P1FlairOld_Tag[num3];
						if (game.P1FlairOld_Tag[num3] == 0)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 1)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 2)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 3)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 4)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 5)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int num4 = 0; num4 < game.P2FlairOld.Length; num4++)
					{
						_ = game.P2FlairOld_Tag[num4];
						if (game.P2FlairOld_Tag[num4] == 0)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 1)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 2)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 3)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 4)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 5)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int num5 = 0; num5 < game.P3FlairOld.Length; num5++)
					{
						_ = game.P3FlairOld_Tag[num5];
						if (game.P3FlairOld_Tag[num5] == 0)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 1)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 2)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 3)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 4)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 5)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int num6 = 0; num6 < game.P4FlairOld.Length; num6++)
					{
						_ = game.P4FlairOld_Tag[num6];
						if (game.P4FlairOld_Tag[num6] == 0)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 1)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 2)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 3)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 4)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 5)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (!_leftThighBodyGone)
				{
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
				}
				if (!_leftUpperArmBodyGone)
				{
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
				}
				if (!_leftHandBodyGone)
				{
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftHandBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
			}
			if (Active)
			{
				for (int num7 = 0; num7 < DartBoneIndex; num7++)
				{
					if (_DartBone[num7] != null && _DartBone[num7].Body != null && _DartBoneBulletTimer[num7] > 5.0)
					{
						spriteBatch.Draw(_DartBoneTexture, _DartBone[num7].Body.Position * PhysicsScaleUp, null, DartBoneColor, _DartBone[num7].Body.Rotation, _DartBoneOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num8 = 0; num8 < DartHarpoonIndex; num8++)
				{
					if (_DartHarpoon[num8] != null && _DartHarpoon[num8].Body != null && _DartHarpoonBulletTimer[num8] > 5.0)
					{
						spriteBatch.Draw(_DartHarpoonTexture, _DartHarpoon[num8].Body.Position * PhysicsScaleUp, null, DartHarpoonColor, _DartHarpoon[num8].Body.Rotation, _DartHarpoonOrigin, 0.75f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num9 = 0; num9 < DartKineticIndex; num9++)
				{
					if (KineticDraw[num9] && _DartKinetic[num9] != null && _DartKinetic[num9].Body != null && _DartKineticBulletTimer[num9] > 5.0)
					{
						spriteBatch.Draw(_DartKineticTexture, _DartKinetic[num9].Body.Position * PhysicsScaleUp, null, DartKineticColor, _DartKinetic[num9].Body.Rotation, _DartKineticOrigin, 0.25f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num10 = 0; num10 < CannonBallIndex; num10++)
				{
					if (CannonBallDraw[num10] && _CannonBall != null && _CannonBall[num10] != null && _CannonBall[num10].Body != null)
					{
						spriteBatch.Draw(_CannonBallTexture, _CannonBall[num10].Body.Position * PhysicsScaleUp, null, Color.White, _CannonBall[num10].Body.Rotation, _CannonBallOrigin, 1f, SpriteEffects.FlipVertically, 1f);
						CannonBallColor = Color.White;
					}
				}
				for (int num11 = 0; num11 < DartStasisIndex; num11++)
				{
					if (StasisDraw[num11] && _DartStasis[num11] != null && _DartStasis[num11].Body != null && _DartStasisBulletTimer[num11] > 5.0)
					{
						spriteBatch.Draw(_DartStasisTexture, _DartStasis[num11].Body.Position * PhysicsScaleUp, null, DartStasisColor, _DartStasis[num11].Body.Rotation, _DartStasisOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
					}
				}
				if (DirectionRight)
				{
					if (Player1Index)
					{
						for (int num12 = 0; num12 < game.P1FlairOld.Length; num12++)
						{
							_ = game.P1FlairOld_Tag[num12];
							if (game.P1FlairOld_Tag[num12] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num12], 150 * num12, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num13 = 0; num13 < game.P2FlairOld.Length; num13++)
						{
							_ = game.P2FlairOld_Tag[num13];
							if (game.P2FlairOld_Tag[num13] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num13], 150 * num13, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num14 = 0; num14 < game.P3FlairOld.Length; num14++)
						{
							_ = game.P3FlairOld_Tag[num14];
							if (game.P3FlairOld_Tag[num14] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num14], 150 * num14, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num15 = 0; num15 < game.P4FlairOld.Length; num15++)
						{
							_ = game.P4FlairOld_Tag[num15];
							if (game.P4FlairOld_Tag[num15] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num15], 150 * num15, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (!_rightHandBodyGone && _SightON)
					{
						spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num16 = 0; num16 < game.P1FlairOld.Length; num16++)
						{
							_ = game.P1FlairOld_Tag[num16];
							if (game.P1FlairOld_Tag[num16] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num16], 150 * num16, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num17 = 0; num17 < game.P2FlairOld.Length; num17++)
						{
							_ = game.P2FlairOld_Tag[num17];
							if (game.P2FlairOld_Tag[num17] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num17], 150 * num17, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num18 = 0; num18 < game.P3FlairOld.Length; num18++)
						{
							_ = game.P3FlairOld_Tag[num18];
							if (game.P3FlairOld_Tag[num18] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num18], 150 * num18, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num19 = 0; num19 < game.P4FlairOld.Length; num19++)
						{
							_ = game.P4FlairOld_Tag[num19];
							if (game.P4FlairOld_Tag[num19] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num19], 150 * num19, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num20 = 0; num20 < game.P1FlairOld.Length; num20++)
						{
							_ = game.P1FlairOld_Tag[num20];
							if (game.P1FlairOld_Tag[num20] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num21 = 0; num21 < game.P2FlairOld.Length; num21++)
						{
							_ = game.P2FlairOld_Tag[num21];
							if (game.P2FlairOld_Tag[num21] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num22 = 0; num22 < game.P3FlairOld.Length; num22++)
						{
							_ = game.P3FlairOld_Tag[num22];
							if (game.P3FlairOld_Tag[num22] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num23 = 0; num23 < game.P4FlairOld.Length; num23++)
						{
							_ = game.P4FlairOld_Tag[num23];
							if (game.P4FlairOld_Tag[num23] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
				if (DirectionLeft)
				{
					if (Player1Index)
					{
						for (int num24 = 0; num24 < game.P1FlairOld.Length; num24++)
						{
							_ = game.P1FlairOld_Tag[num24];
							if (game.P1FlairOld_Tag[num24] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num24], 150 * num24, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 57500f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num25 = 0; num25 < game.P2FlairOld.Length; num25++)
						{
							_ = game.P2FlairOld_Tag[num25];
							if (game.P2FlairOld_Tag[num25] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num25], 150 * num25, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num26 = 0; num26 < game.P3FlairOld.Length; num26++)
						{
							_ = game.P3FlairOld_Tag[num26];
							if (game.P3FlairOld_Tag[num26] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num26], 150 * num26, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num27 = 0; num27 < game.P4FlairOld.Length; num27++)
						{
							_ = game.P4FlairOld_Tag[num27];
							if (game.P4FlairOld_Tag[num27] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num27], 150 * num27, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipHorizontally, 1f);
					if (Player1Index)
					{
						for (int num28 = 0; num28 < game.P1FlairOld.Length; num28++)
						{
							_ = game.P1FlairOld_Tag[num28];
							if (game.P1FlairOld_Tag[num28] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num28], 150 * num28, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num29 = 0; num29 < game.P2FlairOld.Length; num29++)
						{
							_ = game.P2FlairOld_Tag[num29];
							if (game.P2FlairOld_Tag[num29] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num29], 150 * num29, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num30 = 0; num30 < game.P3FlairOld.Length; num30++)
						{
							_ = game.P3FlairOld_Tag[num30];
							if (game.P3FlairOld_Tag[num30] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num30], 150 * num30, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num31 = 0; num31 < game.P4FlairOld.Length; num31++)
						{
							_ = game.P4FlairOld_Tag[num31];
							if (game.P4FlairOld_Tag[num31] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num31], 150 * num31, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 3f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.FlipHorizontally, 1f);
					if (Player1Index)
					{
						for (int num32 = 0; num32 < game.P1FlairOld.Length; num32++)
						{
							_ = game.P1FlairOld_Tag[num32];
							if (game.P1FlairOld_Tag[num32] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num33 = 0; num33 < game.P2FlairOld.Length; num33++)
						{
							_ = game.P2FlairOld_Tag[num33];
							if (game.P2FlairOld_Tag[num33] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num34 = 0; num34 < game.P3FlairOld.Length; num34++)
						{
							_ = game.P3FlairOld_Tag[num34];
							if (game.P3FlairOld_Tag[num34] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num35 = 0; num35 < game.P4FlairOld.Length; num35++)
						{
							_ = game.P4FlairOld_Tag[num35];
							if (game.P4FlairOld_Tag[num35] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (!_rightHandBodyGone && _SightON)
					{
						spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
				}
				if (!DirectionLeft && !DirectionRight)
				{
					if (Player1Index)
					{
						for (int num36 = 0; num36 < game.P1FlairOld.Length; num36++)
						{
							_ = game.P1FlairOld_Tag[num36];
							if (game.P1FlairOld_Tag[num36] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num36], 150 * num36, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num37 = 0; num37 < game.P2FlairOld.Length; num37++)
						{
							_ = game.P2FlairOld_Tag[num37];
							if (game.P2FlairOld_Tag[num37] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num37], 150 * num37, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num38 = 0; num38 < game.P3FlairOld.Length; num38++)
						{
							_ = game.P3FlairOld_Tag[num38];
							if (game.P3FlairOld_Tag[num38] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num38], 150 * num38, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num39 = 0; num39 < game.P4FlairOld.Length; num39++)
						{
							_ = game.P4FlairOld_Tag[num39];
							if (game.P4FlairOld_Tag[num39] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num39], 150 * num39, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (!_rightHandBodyGone)
					{
						if (_SightON)
						{
							spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
						}
						if (TelekinesisHitSomthing)
						{
							Vector2 value = _rightHandBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
							Vector2 value2 = TelekinisisBodyHit.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
							Color color = new Color(150, (byte)level.random.Next(150, 255), 150);
							float rotation = (float)Math.Atan2(value2.Y - value.Y, value2.X - value.X);
							float x = Vector2.Distance(value, value2);
							Texture2D texture2D = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
							texture2D.SetData(new Color[1] { Color.White });
							spriteBatch.Draw(texture2D, value, null, color, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, 0f);
						}
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num40 = 0; num40 < game.P1FlairOld.Length; num40++)
						{
							_ = game.P1FlairOld_Tag[num40];
							if (game.P1FlairOld_Tag[num40] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num40], 150 * num40, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num41 = 0; num41 < game.P2FlairOld.Length; num41++)
						{
							_ = game.P2FlairOld_Tag[num41];
							if (game.P2FlairOld_Tag[num41] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num41], 150 * num41, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num42 = 0; num42 < game.P3FlairOld.Length; num42++)
						{
							_ = game.P3FlairOld_Tag[num42];
							if (game.P3FlairOld_Tag[num42] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num42], 150 * num42, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num43 = 0; num43 < game.P4FlairOld.Length; num43++)
						{
							_ = game.P4FlairOld_Tag[num43];
							if (game.P4FlairOld_Tag[num43] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num43], 150 * num43, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num44 = 0; num44 < game.P1FlairOld.Length; num44++)
						{
							_ = game.P1FlairOld_Tag[num44];
							if (game.P1FlairOld_Tag[num44] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num45 = 0; num45 < game.P2FlairOld.Length; num45++)
						{
							_ = game.P2FlairOld_Tag[num45];
							if (game.P2FlairOld_Tag[num45] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num46 = 0; num46 < game.P3FlairOld.Length; num46++)
						{
							_ = game.P3FlairOld_Tag[num46];
							if (game.P3FlairOld_Tag[num46] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num47 = 0; num47 < game.P4FlairOld.Length; num47++)
						{
							_ = game.P4FlairOld_Tag[num47];
							if (game.P4FlairOld_Tag[num47] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
			}
		}
		spriteBatch.End();
	}

	public void Draw_Ghost_Walk(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game, Matrix cameraTransform, BlendState Blender)
	{
		spriteBatch.Begin(SpriteSortMode.Immediate, Blender, null, null, null, null, cameraTransform);
		if (!DeadByBounds)
		{
			if (Dead)
			{
				if (Player1Index)
				{
					for (int i = 0; i < game.P1FlairOld.Length; i++)
					{
						_ = game.P1FlairOld_Tag[i];
						if (game.P1FlairOld_Tag[i] == 7)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[i], 150 * i, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int j = 0; j < game.P2FlairOld.Length; j++)
					{
						_ = game.P2FlairOld_Tag[j];
						if (game.P2FlairOld_Tag[j] == 7)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[j], 150 * j, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int k = 0; k < game.P3FlairOld.Length; k++)
					{
						_ = game.P3FlairOld_Tag[k];
						if (game.P3FlairOld_Tag[k] == 7)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[k], 150 * k, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int l = 0; l < game.P4FlairOld.Length; l++)
					{
						_ = game.P4FlairOld_Tag[l];
						if (game.P4FlairOld_Tag[l] == 7)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[l], 150 * l, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (!_rightThighBodyGone)
				{
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
				}
				if (Weapon_Armed)
				{
					spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
				}
				if (!_rightUpperArmBodyGone)
				{
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
				}
				if (!_rightHandBodyGone)
				{
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
				if (!_bodyBodyGone)
				{
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
				}
				if (Player1Index)
				{
					for (int m = 0; m < game.P1FlairOld.Length; m++)
					{
						_ = game.P1FlairOld_Tag[m];
						if (game.P1FlairOld_Tag[m] == 6)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[m], 150 * m, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int n = 0; n < game.P2FlairOld.Length; n++)
					{
						_ = game.P2FlairOld_Tag[n];
						if (game.P2FlairOld_Tag[n] == 6)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[n], 150 * n, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int num = 0; num < game.P3FlairOld.Length; num++)
					{
						_ = game.P3FlairOld_Tag[num];
						if (game.P3FlairOld_Tag[num] == 6)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num], 150 * num, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int num2 = 0; num2 < game.P4FlairOld.Length; num2++)
					{
						_ = game.P4FlairOld_Tag[num2];
						if (game.P4FlairOld_Tag[num2] == 6)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num2], 150 * num2, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (_headBody.Body != null)
				{
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
				}
				if (Player1Index)
				{
					for (int num3 = 0; num3 < game.P1FlairOld.Length; num3++)
					{
						_ = game.P1FlairOld_Tag[num3];
						if (game.P1FlairOld_Tag[num3] == 0)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 1)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 2)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 3)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 4)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P1FlairOld_Tag[num3] == 5)
						{
							spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num3], 150 * num3, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player2Index)
				{
					for (int num4 = 0; num4 < game.P2FlairOld.Length; num4++)
					{
						_ = game.P2FlairOld_Tag[num4];
						if (game.P2FlairOld_Tag[num4] == 0)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 1)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 2)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 3)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 4)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P2FlairOld_Tag[num4] == 5)
						{
							spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num4], 150 * num4, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player3Index)
				{
					for (int num5 = 0; num5 < game.P3FlairOld.Length; num5++)
					{
						_ = game.P3FlairOld_Tag[num5];
						if (game.P3FlairOld_Tag[num5] == 0)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 1)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 2)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 3)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 4)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P3FlairOld_Tag[num5] == 5)
						{
							spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num5], 150 * num5, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (Player4Index)
				{
					for (int num6 = 0; num6 < game.P4FlairOld.Length; num6++)
					{
						_ = game.P4FlairOld_Tag[num6];
						if (game.P4FlairOld_Tag[num6] == 0)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 1)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 2)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 3)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 4)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
						else if (game.P4FlairOld_Tag[num6] == 5)
						{
							spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num6], 150 * num6, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
						}
					}
				}
				if (!_leftThighBodyGone)
				{
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
				}
				if (!_leftUpperArmBodyGone)
				{
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
				}
				if (!_leftHandBodyGone)
				{
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftHandBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
			}
			if (Active)
			{
				for (int num7 = 0; num7 < DartBoneIndex; num7++)
				{
					if (_DartBone[num7] != null && _DartBone[num7].Body != null && _DartBoneBulletTimer[num7] > 5.0)
					{
						spriteBatch.Draw(_DartBoneTexture, _DartBone[num7].Body.Position * PhysicsScaleUp, null, DartBoneColor, _DartBone[num7].Body.Rotation, _DartBoneOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num8 = 0; num8 < DartHarpoonIndex; num8++)
				{
					if (_DartHarpoon[num8] != null && _DartHarpoon[num8].Body != null && _DartHarpoonBulletTimer[num8] > 5.0)
					{
						spriteBatch.Draw(_DartHarpoonTexture, _DartHarpoon[num8].Body.Position * PhysicsScaleUp, null, DartHarpoonColor, _DartHarpoon[num8].Body.Rotation, _DartHarpoonOrigin, 0.75f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num9 = 0; num9 < DartKineticIndex; num9++)
				{
					if (KineticDraw[num9] && _DartKinetic[num9] != null && _DartKinetic[num9].Body != null && _DartKineticBulletTimer[num9] > 5.0)
					{
						spriteBatch.Draw(_DartKineticTexture, _DartKinetic[num9].Body.Position * PhysicsScaleUp, null, DartKineticColor, _DartKinetic[num9].Body.Rotation, _DartKineticOrigin, 0.25f, SpriteEffects.FlipVertically, 1f);
					}
				}
				for (int num10 = 0; num10 < CannonBallIndex; num10++)
				{
					if (CannonBallDraw[num10] && _CannonBall != null && _CannonBall[num10] != null && _CannonBall[num10].Body != null)
					{
						spriteBatch.Draw(_CannonBallTexture, _CannonBall[num10].Body.Position * PhysicsScaleUp, null, Color.White, _CannonBall[num10].Body.Rotation, _CannonBallOrigin, 1f, SpriteEffects.FlipVertically, 1f);
						CannonBallColor = Color.White;
					}
				}
				for (int num11 = 0; num11 < DartStasisIndex; num11++)
				{
					if (StasisDraw[num11] && _DartStasis[num11] != null && _DartStasis[num11].Body != null && _DartStasisBulletTimer[num11] > 5.0)
					{
						spriteBatch.Draw(_DartStasisTexture, _DartStasis[num11].Body.Position * PhysicsScaleUp, null, DartStasisColor, _DartStasis[num11].Body.Rotation, _DartStasisOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
					}
				}
				if (DirectionRight)
				{
					if (Player1Index)
					{
						for (int num12 = 0; num12 < game.P1FlairOld.Length; num12++)
						{
							_ = game.P1FlairOld_Tag[num12];
							if (game.P1FlairOld_Tag[num12] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num12], 150 * num12, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num13 = 0; num13 < game.P2FlairOld.Length; num13++)
						{
							_ = game.P2FlairOld_Tag[num13];
							if (game.P2FlairOld_Tag[num13] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num13], 150 * num13, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num14 = 0; num14 < game.P3FlairOld.Length; num14++)
						{
							_ = game.P3FlairOld_Tag[num14];
							if (game.P3FlairOld_Tag[num14] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num14], 150 * num14, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num15 = 0; num15 < game.P4FlairOld.Length; num15++)
						{
							_ = game.P4FlairOld_Tag[num15];
							if (game.P4FlairOld_Tag[num15] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num15], 150 * num15, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (!_rightHandBodyGone && _SightON)
					{
						spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num16 = 0; num16 < game.P1FlairOld.Length; num16++)
						{
							_ = game.P1FlairOld_Tag[num16];
							if (game.P1FlairOld_Tag[num16] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num16], 150 * num16, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num17 = 0; num17 < game.P2FlairOld.Length; num17++)
						{
							_ = game.P2FlairOld_Tag[num17];
							if (game.P2FlairOld_Tag[num17] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num17], 150 * num17, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num18 = 0; num18 < game.P3FlairOld.Length; num18++)
						{
							_ = game.P3FlairOld_Tag[num18];
							if (game.P3FlairOld_Tag[num18] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num18], 150 * num18, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num19 = 0; num19 < game.P4FlairOld.Length; num19++)
						{
							_ = game.P4FlairOld_Tag[num19];
							if (game.P4FlairOld_Tag[num19] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num19], 150 * num19, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num20 = 0; num20 < game.P1FlairOld.Length; num20++)
						{
							_ = game.P1FlairOld_Tag[num20];
							if (game.P1FlairOld_Tag[num20] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num20] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num20], 150 * num20, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num21 = 0; num21 < game.P2FlairOld.Length; num21++)
						{
							_ = game.P2FlairOld_Tag[num21];
							if (game.P2FlairOld_Tag[num21] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num21] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num21], 150 * num21, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num22 = 0; num22 < game.P3FlairOld.Length; num22++)
						{
							_ = game.P3FlairOld_Tag[num22];
							if (game.P3FlairOld_Tag[num22] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num22] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num22], 150 * num22, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num23 = 0; num23 < game.P4FlairOld.Length; num23++)
						{
							_ = game.P4FlairOld_Tag[num23];
							if (game.P4FlairOld_Tag[num23] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num23] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num23], 150 * num23, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
				if (DirectionLeft)
				{
					if (Player1Index)
					{
						for (int num24 = 0; num24 < game.P1FlairOld.Length; num24++)
						{
							_ = game.P1FlairOld_Tag[num24];
							if (game.P1FlairOld_Tag[num24] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num24], 150 * num24, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 57500f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num25 = 0; num25 < game.P2FlairOld.Length; num25++)
						{
							_ = game.P2FlairOld_Tag[num25];
							if (game.P2FlairOld_Tag[num25] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num25], 150 * num25, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num26 = 0; num26 < game.P3FlairOld.Length; num26++)
						{
							_ = game.P3FlairOld_Tag[num26];
							if (game.P3FlairOld_Tag[num26] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num26], 150 * num26, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num27 = 0; num27 < game.P4FlairOld.Length; num27++)
						{
							_ = game.P4FlairOld_Tag[num27];
							if (game.P4FlairOld_Tag[num27] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num27], 150 * num27, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipHorizontally, 1f);
					if (Player1Index)
					{
						for (int num28 = 0; num28 < game.P1FlairOld.Length; num28++)
						{
							_ = game.P1FlairOld_Tag[num28];
							if (game.P1FlairOld_Tag[num28] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num28], 150 * num28, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num29 = 0; num29 < game.P2FlairOld.Length; num29++)
						{
							_ = game.P2FlairOld_Tag[num29];
							if (game.P2FlairOld_Tag[num29] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num29], 150 * num29, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num30 = 0; num30 < game.P3FlairOld.Length; num30++)
						{
							_ = game.P3FlairOld_Tag[num30];
							if (game.P3FlairOld_Tag[num30] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num30], 150 * num30, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num31 = 0; num31 < game.P4FlairOld.Length; num31++)
						{
							_ = game.P4FlairOld_Tag[num31];
							if (game.P4FlairOld_Tag[num31] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num31], 150 * num31, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 3f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.FlipHorizontally, 1f);
					if (Player1Index)
					{
						for (int num32 = 0; num32 < game.P1FlairOld.Length; num32++)
						{
							_ = game.P1FlairOld_Tag[num32];
							if (game.P1FlairOld_Tag[num32] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P1FlairOld_Tag[num32] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num32], 150 * num32, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num33 = 0; num33 < game.P2FlairOld.Length; num33++)
						{
							_ = game.P2FlairOld_Tag[num33];
							if (game.P2FlairOld_Tag[num33] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P2FlairOld_Tag[num33] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num33], 150 * num33, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num34 = 0; num34 < game.P3FlairOld.Length; num34++)
						{
							_ = game.P3FlairOld_Tag[num34];
							if (game.P3FlairOld_Tag[num34] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P3FlairOld_Tag[num34] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num34], 150 * num34, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num35 = 0; num35 < game.P4FlairOld.Length; num35++)
						{
							_ = game.P4FlairOld_Tag[num35];
							if (game.P4FlairOld_Tag[num35] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
							else if (game.P4FlairOld_Tag[num35] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num35], 150 * num35, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.FlipHorizontally, 1f);
							}
						}
					}
					if (!_rightHandBodyGone && _SightON)
					{
						spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.FlipHorizontally, 1f);
					}
				}
				if (!DirectionLeft && !DirectionRight)
				{
					if (Player1Index)
					{
						for (int num36 = 0; num36 < game.P1FlairOld.Length; num36++)
						{
							_ = game.P1FlairOld_Tag[num36];
							if (game.P1FlairOld_Tag[num36] == 7)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num36], 150 * num36, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num37 = 0; num37 < game.P2FlairOld.Length; num37++)
						{
							_ = game.P2FlairOld_Tag[num37];
							if (game.P2FlairOld_Tag[num37] == 7)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num37], 150 * num37, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num38 = 0; num38 < game.P3FlairOld.Length; num38++)
						{
							_ = game.P3FlairOld_Tag[num38];
							if (game.P3FlairOld_Tag[num38] == 7)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num38], 150 * num38, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num39 = 0; num39 < game.P4FlairOld.Length; num39++)
						{
							_ = game.P4FlairOld_Tag[num39];
							if (game.P4FlairOld_Tag[num39] == 7)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num39], 150 * num39, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (!_rightHandBodyGone)
					{
						if (_SightON)
						{
							spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
						}
						if (TelekinesisHitSomthing)
						{
							Vector2 value = _rightHandBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
							Vector2 value2 = TelekinisisBodyHit.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
							Color color = new Color(150, (byte)level.random.Next(150, 255), 150);
							float rotation = (float)Math.Atan2(value2.Y - value.Y, value2.X - value.X);
							float x = Vector2.Distance(value, value2);
							Texture2D texture2D = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
							texture2D.SetData(new Color[1] { Color.White });
							spriteBatch.Draw(texture2D, value, null, color, rotation, Vector2.Zero, new Vector2(x, 1f), SpriteEffects.None, 0f);
						}
					}
					spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, ColorRightLeg, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					if (Weapon_Armed)
					{
						spriteBatch.Draw(_Weapon_Brush, _kineticShields.Body.Position * PhysicsScaleUp, null, ColorRightHand, _kineticShields.Body.Rotation, new Vector2(_Weapon_Brush.Width / 2, _Weapon_Brush.Height / 2), Weapon_Shield_Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorRightArm, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_rightHandBrush, _rightHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _rightHandBody_Claw.Body.Rotation, _rightHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
					spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num40 = 0; num40 < game.P1FlairOld.Length; num40++)
						{
							_ = game.P1FlairOld_Tag[num40];
							if (game.P1FlairOld_Tag[num40] == 6)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num40], 150 * num40, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num41 = 0; num41 < game.P2FlairOld.Length; num41++)
						{
							_ = game.P2FlairOld_Tag[num41];
							if (game.P2FlairOld_Tag[num41] == 6)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num41], 150 * num41, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num42 = 0; num42 < game.P3FlairOld.Length; num42++)
						{
							_ = game.P3FlairOld_Tag[num42];
							if (game.P3FlairOld_Tag[num42] == 6)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num42], 150 * num42, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num43 = 0; num43 < game.P4FlairOld.Length; num43++)
						{
							_ = game.P4FlairOld_Tag[num43];
							if (game.P4FlairOld_Tag[num43] == 6)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _bodyBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num43], 150 * num43, 150, 150), Color, _bodyBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
					if (Player1Index)
					{
						for (int num44 = 0; num44 < game.P1FlairOld.Length; num44++)
						{
							_ = game.P1FlairOld_Tag[num44];
							if (game.P1FlairOld_Tag[num44] == 0)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 1)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 2)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 3)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 4)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P1FlairOld_Tag[num44] == 5)
							{
								spriteBatch.Draw(game.Player1SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P1FlairOld[num44], 150 * num44, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player2Index)
					{
						for (int num45 = 0; num45 < game.P2FlairOld.Length; num45++)
						{
							_ = game.P2FlairOld_Tag[num45];
							if (game.P2FlairOld_Tag[num45] == 0)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 1)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 2)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 3)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 4)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P2FlairOld_Tag[num45] == 5)
							{
								spriteBatch.Draw(game.Player2SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P2FlairOld[num45], 150 * num45, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player3Index)
					{
						for (int num46 = 0; num46 < game.P3FlairOld.Length; num46++)
						{
							_ = game.P3FlairOld_Tag[num46];
							if (game.P3FlairOld_Tag[num46] == 0)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 1)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 2)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 3)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 4)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P3FlairOld_Tag[num46] == 5)
							{
								spriteBatch.Draw(game.Player3SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P3FlairOld[num46], 150 * num46, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					if (Player4Index)
					{
						for (int num47 = 0; num47 < game.P4FlairOld.Length; num47++)
						{
							_ = game.P4FlairOld_Tag[num47];
							if (game.P4FlairOld_Tag[num47] == 0)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 1)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 2)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 3)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 4)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
							else if (game.P4FlairOld_Tag[num47] == 5)
							{
								spriteBatch.Draw(game.Player4SpriteSheet, _headBody.Body.Position * PhysicsScaleUp, new Rectangle(150 * game.P4FlairOld[num47], 150 * num47, 150, 150), Color, _headBody.Body.Rotation, new Vector2(75f, 75f), Scaler * 2f, SpriteEffects.None, 1f);
							}
						}
					}
					spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, ColorLeftLeg, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, ColorLeftArm, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
					spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, ColorLeftHand, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
					if (IsClawing)
					{
						spriteBatch.Draw(_leftHandBrush, _leftHandBody_Claw.Body.Position * PhysicsScaleUp, null, ColorRightHand, _leftHandBody_Claw.Body.Rotation, _leftHandBrushOrigin, Scaler * 1f, SpriteEffects.None, 1f);
					}
				}
			}
		}
		spriteBatch.End();
	}

	public void DrawMagic(SpriteBatch spriteBatch)
	{
		for (int i = 0; i < IceBallIndex; i++)
		{
			if (_IceBall[i] != null && _IceBall[i].Body != null && (int)_IceBall[i].Body.UserData != 999 && _IceBallBulletTimer[i] > 5.0)
			{
				spriteBatch.Draw(_IceBallTexture, _IceBall[i].Body.Position * PhysicsScaleUp, null, IceBallColor, _IceBall[i].Body.Rotation, _IceBallOrigin, 0.5f, SpriteEffects.None, 1f);
			}
		}
	}

	private static Vector2 MoveUp(GameTime gameTime, float speed)
	{
		double num = gameTime.TotalGameTime.TotalSeconds * (double)speed;
		float x = 0f;
		float y = (float)num;
		return new Vector2(x, y);
	}

	public void DrawWiggledLines(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame gamey, Matrix cameraTransform)
	{
		if (Player_Species == 4f && (Telekinisis_Try || TelekinesisHitSomthing))
		{
			gamey.graphics.GraphicsDevice.Textures[1] = gamey.RnRBurnTexture;
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, cameraTransform);
			gamey.graphics.GraphicsDevice.Textures[1] = gamey.RnRBurnTexture;
			gamey.disappearEffect.Parameters["OverlayScroll"].SetValue(MoveUp(gameTime, 0.1f) * 0.25f);
			gamey.disappearEffect.CurrentTechnique.Passes[0].Apply();
			Vector2 value = _rightHandBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			Vector2 value2 = new Vector2(0f, 0f);
			if (!TelekinesisHitSomthing)
			{
				value2 = TelekinisisBodyHit_Point * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			}
			else if (TelekinisisBodyHit != null && TelekinisisBodyHit.Body != null)
			{
				value2 = TelekinisisBodyHit.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp);
			}
			Color color = new Color(150, (byte)level.random.Next(150, 255), 150, 100);
			float rotation = (float)Math.Atan2(value2.Y - value.Y, value2.X - value.X);
			float x = Vector2.Distance(value, value2);
			Texture2D texture2D = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
			texture2D.SetData(new Color[1] { Color.White });
			spriteBatch.Draw(texture2D, value, null, color, rotation, Vector2.Zero, new Vector2(x, 5f), SpriteEffects.None, 0f);
			spriteBatch.End();
		}
	}

	public void DrawParticles(Matrix cameraTransformForParticles, SpriteBatch spriteBatch)
	{
		if (level.Paused)
		{
			return;
		}
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, cameraTransformForParticles);
		if (!DeadByBounds)
		{
			if (!Dead)
			{
				renderer.RenderEffect(HealEffect, spriteBatch);
				renderer.RenderEffect(particleEffectKineticShield, spriteBatch);
				renderer.RenderEffect(particleEffectKineticEx, spriteBatch);
				renderer.RenderEffect(particleEffectCannonBallEx, spriteBatch);
				renderer.RenderEffect(particleEffectStasisEx, spriteBatch);
				renderer.RenderEffect(particleEffectTeleFog, spriteBatch);
				if (Level.Blood)
				{
					renderer.RenderEffect(particleEffectBleed, spriteBatch);
					renderer.RenderEffect(particleEffectBleeding, spriteBatch);
					renderer.RenderEffect(particleEffectBloodSquirting, spriteBatch);
				}
			}
			else if (Unconscious)
			{
				if (Level.Blood)
				{
					renderer.RenderEffect(particleEffectBleeding, spriteBatch);
					renderer.RenderEffect(particleEffectBloodSquirting, spriteBatch);
				}
			}
			else if (Smoking)
			{
				if (Spirit_Walking)
				{
					renderer.RenderEffect(particleEffectSpirit, spriteBatch);
				}
				if (Level.Blood)
				{
					renderer.RenderEffect(particleEffectBleeding, spriteBatch);
					renderer.RenderEffect(particleEffectBloodSquirting, spriteBatch);
				}
			}
			else
			{
				if (Spirit_Walking)
				{
					renderer.RenderEffect(particleEffectSpirit, spriteBatch);
				}
				if (Level.Blood)
				{
					renderer.RenderEffect(particleEffectBleeding, spriteBatch);
					renderer.RenderEffect(particleEffectBloodSquirting, spriteBatch);
				}
			}
		}
		else if (Spirit_Walking)
		{
			renderer.RenderEffect(particleEffectSpirit, spriteBatch);
		}
		spriteBatch.End();
	}
}
