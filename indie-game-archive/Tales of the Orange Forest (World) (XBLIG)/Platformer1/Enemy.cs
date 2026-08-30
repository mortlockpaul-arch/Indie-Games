using System;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectMercury;
using ProjectMercury.Renderers;

namespace Platformer1;

public class Enemy
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

	private const float rightHandYRadius = 1.6f;

	private const float rightHandXRadius = 0.8f;

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

	public Texture2D _hatBrush;

	public Vector2 cameraTransformOld;

	public bool Active = true;

	public float Scaler = 0.0002f;

	public float PhysicsScaleUp = 5f;

	public PlatformerGame MainGame;

	public float ArmScaler = 2f;

	public float LegScaler = 2f;

	public CollisionCategory _collidesWith = CollisionCategory.All;

	public CollisionCategory _collisionCategory = CollisionCategory.Cat2;

	public short CollisionGroup = 5;

	public short CannonBallCollisionGroup = 120;

	public bool Alive = true;

	public bool Dead;

	public bool Unconscious;

	public bool DirectionLeft;

	public bool DirectionRight = true;

	private bool HFlip;

	private bool VFlip;

	private float RunRotLeft;

	private float RunRotRight;

	public bool IsAlive = true;

	public float mass = 1f;

	public float density = 2E-06f;

	public float LeftArmRotation;

	public float RightArmRotation;

	public bool Respawn;

	public float RaycastIndex;

	public Fixture RaycastHitFirst;

	public bool FirstHit = true;

	private Color EnemyColor;

	public int EnemyIndexNew;

	private Vector2 AverageEnemyPosition;

	private bool wasJumpPressed;

	private bool wasJump3Pressed;

	private bool wasJump4Pressed;

	public SoundEffect Running;

	public SoundEffect Stepping;

	public SoundEffectInstance Step;

	private float RunningSound;

	public Color BodyColor = Color.White;

	private Color HandLeftColor = Color.White;

	private Color HandRightColor = Color.White;

	private Color ThighLeftColor = Color.White;

	private Color ThighRightColor = Color.White;

	public FixedMouseJoint JumpJoint;

	public Vector2 JumpPoint;

	private bool isJumping;

	private bool wasJumping;

	private int JumpTime;

	private int JumpTimeOld;

	private int JumpClock;

	private int JumpDuration;

	private bool JumpStart;

	private bool JumpPeak;

	public float MaxJointForce = 60f;

	public int MaxHP = 8;

	public float EnemyHPBody;

	public float EnemyMana;

	public string EnemyType;

	public float FreezeBallManaCost = 7.5f;

	public float HealOrbManaCost = 1f;

	public float ManaMax = 100f;

	public double ManaTime;

	public float ManaGainRate = 12.333f;

	public Color ManaColor = Color.White;

	public float JointForce;

	public double JustBurntTime;

	public RevoluteJoint NeckJoint;

	public AngleJoint NeckAngleJoint;

	public float NeckJointForce;

	public float NeckAngleJointForce;

	public float UtilityIndex;

	public float UtilitySubIndex;

	public float UtilityIndexMax = 5f;

	public bool DpadUp;

	public bool DpadDown;

	public bool ButtonA;

	public bool ButtonB;

	public bool DpadLeft;

	public bool DpadRight;

	public Texture2D _SightBrush;

	public Vector2 Grab;

	public float GrabDist;

	private Vector2 Vec;

	public Fixture _headBody;

	private Texture2D _headBrush;

	private float HeadMovement;

	private Vector2 _headBrushOrigin;

	public int ObjectType;

	public Fixture _bodyBody;

	private SliderJoint _bodyJoint;

	private FixedAngleJoint _bodyAngleJoint;

	private AngleJoint _bodyAngleLimitingJoint;

	public Texture2D _bodyBrush;

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

	private Fixture _leftUpperArmBody;

	private RevoluteJoint _leftUpperArmJoint;

	private AngleJoint _leftUpperArmAngleJoint;

	private FixedAngleJoint _leftUpperArmFixedAngleJoint;

	private Texture2D _leftUpperArmBrush;

	private AngleJoint _lefttUpperArmAngleLimitingJoint;

	private Vector2 _leftUpperArmBrushOrigin;

	private Fixture _leftHandBody;

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

	private bool LeftShoulderButtonState;

	private bool LeftShoulderButtonStateToggle;

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

	private RevoluteJoint _rightHandJoint;

	private AngleJoint _rightHandAngleJoint;

	private FixedAngleJoint _rightHandFixedAngleJoint;

	public bool RightHandIsTouching;

	private RevoluteJoint _rightHandGrabJoint;

	private Fixture _rightHandGrabOtherFixture;

	private short _rightHandGrabOtherFixture_CollisionGroup;

	private bool _rightHandGrabOtherFixture_IgnoreGravity;

	private float _rightHandGrabOtherFixture_Mass;

	private Vector2 _rightHandGrabOtherFixture_Vector2;

	private BodyType _rightHandGrabOtherFixture_BodyType;

	private Texture2D _rightHandBrush;

	private bool RightShoulderButtonState;

	private bool RightShoulderButtonStateToggle;

	private bool GrabWithRightHandBool;

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

	public Fixture LeftFootGrabOther;

	private Vector2 LeftFootGrabPoint;

	private AngleJoint _leftThighAngleJoint;

	private Texture2D _leftThighBrush;

	private Vector2 _leftThighBrushOrigin;

	public float _leftThighJointForce;

	public bool _leftThighJointRemoved;

	public float _leftThighAngleJointTargetAngle;

	public Vector2 _leftThighBodyAvePosition;

	private Fixture _leftThighBody1;

	private RevoluteJoint _leftThighJoint1;

	private SliderJoint _leftThighSliderJoint1;

	public bool LeftFootIsOnGround1;

	public Fixture LeftFootGrabOther1;

	private Vector2 LeftFootGrabPoint1;

	private AngleJoint _leftThighAngleJoint1;

	private Texture2D _leftThighBrush1;

	private Vector2 _leftThighBrushOrigin1;

	public float _leftThighJointForce1;

	public bool _leftThighJointRemoved1;

	public float _leftThighAngleJointTargetAngle1;

	private Fixture _leftThighBody2;

	private RevoluteJoint _leftThighJoint2;

	private SliderJoint _leftThighSliderJoint2;

	public bool LeftFootIsOnGround2;

	public Fixture LeftFootGrabOther2;

	private Vector2 LeftFootGrabPoint2;

	private AngleJoint _leftThighAngleJoint2;

	private Texture2D _leftThighBrush2;

	private Vector2 _leftThighBrushOrigin2;

	public float _leftThighJointForce2;

	public bool _leftThighJointRemoved2;

	public float _leftThighAngleJointTargetAngle2;

	public Fixture _leftThighBodyPivotBody;

	public RevoluteJoint _leftThighPivotJoint;

	private Fixture _rightThighBody;

	private RevoluteJoint _rightThighJoint;

	private SliderJoint _rightThighSliderJoint;

	public bool RightFootIsOnGround;

	public Fixture RightFootGrabOther;

	private Vector2 RightFootGrabPoint;

	private AngleJoint _rightThighAngleJoint;

	private Texture2D _rightThighBrush;

	private Vector2 _rightThighBrushOrigin;

	public float _rightThighJointForce;

	public bool _rightThighJointRemoved;

	public float _rightThighAngleJointTargetAngle;

	public Vector2 _rightThighBodyAvePosition;

	private Fixture _rightThighBody1;

	private RevoluteJoint _rightThighJoint1;

	private SliderJoint _rightThighSliderJoint1;

	public bool RightFootIsOnGround1;

	public Fixture RightFootGrabOther1;

	private Vector2 RightFootGrabPoint1;

	private AngleJoint _rightThighAngleJoint1;

	private Texture2D _rightThighBrush1;

	private Vector2 _rightThighBrushOrigin1;

	public float _rightThighJointForce1;

	public bool _rightThighJointRemoved1;

	public float _rightThighAngleJointTargetAngle1;

	private Fixture _rightThighBody2;

	private RevoluteJoint _rightThighJoint2;

	private SliderJoint _rightThighSliderJoint2;

	public bool RightFootIsOnGround2;

	public Fixture RightFootGrabOther2;

	private Vector2 RightFootGrabPoint2;

	private AngleJoint _rightThighAngleJoint2;

	private Texture2D _rightThighBrush2;

	private Vector2 _rightThighBrushOrigin2;

	public float _rightThighJointForce2;

	public bool _rightThighJointRemoved2;

	public float _rightThighAngleJointTargetAngle2;

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

	public float movement;

	public float movementX;

	public float RunLimit;

	public float movementY;

	private Vector2 velocity;

	public readonly string ParticleEffecstDir = "/Effects/Particle/";

	public ParticleEffect FireEffectLeft;

	public ParticleEffect FireEffectRight;

	public ParticleEffect FreezeEffectLeft;

	public ParticleEffect FreezeEffectRight;

	public ParticleEffect HealEffect;

	public ParticleEffect particleEffectKineticShield;

	public double OldGameTime;

	public double OldUnconsciousTime;

	public double BloodGameTime;

	public ParticleEffect particleEffectBleed;

	public ParticleEffect particleEffectBleeding;

	public ParticleEffect particleEffectUnconcious;

	public ParticleEffect particleEffectBloodSquirting;

	private bool WasDustyLeft;

	private bool WasDustyRight;

	public Renderer renderer;

	private Texture2D BurntTexture;

	public AABB ParticleBoundingBox;

	public DynamicTree ParticleDynamicTree;

	public float GrabThrowForceScaler = 1000000f;

	public Fixture[] _CannonBall;

	public double[] _CannonBallBulletTimer;

	private int CannonBallIndex;

	private Vector2 _CannonBallOrigin;

	private Texture2D _CannonBallTexture;

	public float CannonBallManaCost = 25f;

	public float CannonBallForceScaler = 1E+12f;

	private SoundEffect CannonBallSound;

	public Color CannonBallColor;

	public int FireDamage = 10;

	private int FireSpeedX = 75;

	private int FreezeSpeedX = 75;

	public Fixture[] _IceBall;

	public double[] _IceBallBulletTimer;

	private int IceBallIndex;

	private Vector2 _IceBallOrigin;

	private Texture2D _IceBallTexture;

	public float IceBallForceScaler = 1E+10f;

	public float IceBallManaCost = 7.5f;

	private SoundEffect IceBallSound;

	public Color IceBallColor;

	public bool Frozen;

	public Fixture[] _DartBone;

	public double[] _DartBoneBulletTimer;

	private int DartBoneIndex;

	private Vector2 _DartBoneOrigin;

	private Texture2D _DartBoneTexture;

	public float DartBoneForceScaler = 1E+12f;

	public float DartBoneManaCost = 2.5f;

	private SoundEffect DartBoneSound;

	public Color DartBoneColor = Color.White;

	public Texture2D[] DartBoneDecalTexture;

	public float DartBoneDamage = 7f;

	public Fixture _DartKinetic;

	public Fixture _DartKineticZone;

	public double _DartKineticBulletTimer;

	private int DartKineticIndex;

	private Vector2 _DartKineticOrigin;

	private Texture2D _DartKineticTexture;

	public float DartKineticForceScaler = 1E+13f;

	public float DartKineticManaCost = 75f;

	private SoundEffect DartKineticSound;

	public Color DartKineticColor = Color.White;

	public Texture2D[] DartKineticDecalTexture;

	public float DartKineticDamage = 66f;

	public bool KineticGo;

	public Fixture DartKineticDart;

	public bool KineticDraw;

	public ParticleEffect particleEffectKineticEx;

	public Fixture _kineticShields;

	public float KineticShieldManaCost = 1f;

	private float SharpsNeddleDamage = 5f;

	private RenderTarget2D _bodyDecalRenderer;

	private bool WeldDecal;

	private Fixture FixA;

	private Fixture FixB;

	private Level level;

	private ContentManager content;

	private Vector2 position;

	public int LevelDataIndex;

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

	public ContentManager Content => content;

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

	public Enemy(ContentManager content, Level MainLevel, PlatformerGame mainGame, Vector2 position, World physicsSimulator, string EnemyType, float rot, int LevelDataIndex)
	{
		_position = position + new Vector2(-100f, -100f);
		_PhyPosition = new Vector2((-100f + position.X) * 0.2f, (-100f + position.Y) * 0.2f);
		level = MainLevel;
		this.LevelDataIndex = LevelDataIndex;
		MainGame = mainGame;
		ObjectType = 7;
		this.content = content;
		this.EnemyType = EnemyType;
		switch (EnemyType)
		{
		case "0":
			LoadRat(level, physicsSimulator, EnemyType);
			break;
		case "1":
			LoadBat(level, physicsSimulator, EnemyType);
			break;
		case "2":
			LoadWereLimer(level, physicsSimulator, EnemyType);
			break;
		default:
			LoadRat(level, physicsSimulator, EnemyType);
			break;
		}
	}

	public void LoadRat(Level level, World _world, string EnemyType)
	{
		EnemyHPBody = 10f;
		EnemyMana = ManaMax;
		Scaler = 0.3f;
		LegScaler = 0.5f;
		ArmScaler = 0.75f;
		string text = "Sprites/Enemy/" + EnemyType + "/";
		Alive = true;
		Active = true;
		float num = 8.8f;
		float height = 0.8f;
		float width = 1f;
		float height2 = 0.4f;
		float num2 = 0.8f;
		float num3 = 0.8f;
		_bodyBody = FixtureFactory.CreateRectangle(_world, num, height, density, _PhyPosition);
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-05f * PhysicsScaleUp;
		_bodyBody.Friction = 10000f;
		_bodyBody.Restitution = 0f;
		_bodyBody.Body.UserData = 201;
		_bodyBody.UserData = 90;
		_bodyBody.Body.LinearDamping = 0f;
		_bodyBody.Body.AngularDamping = 10f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrush = content.Load<Texture2D>(text + "body");
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_body));
		_headBody = FixtureFactory.CreateRectangle(_world, width, height2, density, _PhyPosition);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-06f;
		_headBody.Friction = 10000f;
		_headBody.Restitution = 0f;
		_headBody.Body.UserData = 201;
		_headBody.UserData = 90;
		_headBody.Body.LinearDamping = 0f;
		_headBody.CollisionGroup = CollisionGroup;
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		Fixture headBody2 = _headBody;
		headBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(headBody2.OnSeparation, new SeparationEventHandler(OnSeparation_body));
		_leftThighBody1 = FixtureFactory.CreateCircle(_world, num2, density);
		_leftThighBody1.Body.Position = _bodyBody.Body.Position + new Vector2(num, 0f);
		_leftThighBody1.Body.BodyType = BodyType.Dynamic;
		_leftThighBody1.Body.SleepingAllowed = true;
		_leftThighBody1.Density = 2E-06f;
		_leftThighBody1.Friction = 100000f;
		_leftThighBody1.Restitution = 0f;
		_leftThighBody1.Body.UserData = 201;
		_leftThighBody1.UserData = 90;
		_leftThighBody1.Body.LinearDamping = 0f;
		_leftThighBody1.Body.IsBullet = true;
		_leftThighBody1.CollisionGroup = CollisionGroup;
		_leftThighBrush = content.Load<Texture2D>(text + "feet");
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody1;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_leftThigh_Rat));
		Fixture leftThighBody2 = _leftThighBody1;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftThigh_Rat));
		_rightThighBody1 = FixtureFactory.CreateCircle(_world, num3, density);
		_rightThighBody1.Body.Position = _bodyBody.Body.Position + new Vector2(0f - num, 0f);
		_rightThighBody1.Body.BodyType = BodyType.Dynamic;
		_rightThighBody1.Body.SleepingAllowed = true;
		_rightThighBody1.Density = 2E-06f;
		_rightThighBody1.Friction = 100000f;
		_rightThighBody1.Restitution = 0f;
		_rightThighBody1.Body.LinearDamping = 0f;
		_rightThighBody1.Body.UserData = 201;
		_rightThighBody1.UserData = 90;
		_rightThighBody1.Body.IsBullet = true;
		_rightThighBody1.CollisionGroup = CollisionGroup;
		_rightThighBrush = content.Load<Texture2D>(text + "feet");
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody1;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_rightThigh_Rat));
		Fixture rightThighBody2 = _rightThighBody1;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightThigh_Rat));
		_leftThighBody2 = FixtureFactory.CreateCircle(_world, num2, density);
		_leftThighBody2.Body.Position = _bodyBody.Body.Position + new Vector2(num, 0f);
		_leftThighBody2.Body.BodyType = BodyType.Dynamic;
		_leftThighBody2.Body.SleepingAllowed = true;
		_leftThighBody2.Density = 2E-06f;
		_leftThighBody2.Friction = 100000f;
		_leftThighBody2.Restitution = 0f;
		_leftThighBody2.Body.UserData = 201;
		_leftThighBody2.UserData = 90;
		_leftThighBody2.Body.LinearDamping = 0f;
		_leftThighBody2.Body.IsBullet = true;
		_leftThighBody2.CollisionGroup = CollisionGroup;
		_leftThighBrush = content.Load<Texture2D>(text + "feet");
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody3 = _leftThighBody2;
		leftThighBody3.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody3.OnCollision, new CollisionEventHandler(OnCollision_leftThigh_Rat));
		Fixture leftThighBody4 = _leftThighBody2;
		leftThighBody4.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody4.OnSeparation, new SeparationEventHandler(OnSeparation_leftThigh_Rat));
		_rightThighBody2 = FixtureFactory.CreateCircle(_world, num3, density);
		_rightThighBody2.Body.Position = _bodyBody.Body.Position + new Vector2(0f - num, 0f);
		_rightThighBody2.Body.BodyType = BodyType.Dynamic;
		_rightThighBody2.Body.SleepingAllowed = true;
		_rightThighBody2.Density = 2E-06f;
		_rightThighBody2.Friction = 100000f;
		_rightThighBody2.Restitution = 0f;
		_rightThighBody2.Body.LinearDamping = 0f;
		_rightThighBody2.Body.UserData = 201;
		_rightThighBody2.UserData = 90;
		_rightThighBody2.Body.IsBullet = true;
		_rightThighBody2.CollisionGroup = CollisionGroup;
		_rightThighBrush = content.Load<Texture2D>(text + "feet");
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody3 = _rightThighBody2;
		rightThighBody3.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody3.OnCollision, new CollisionEventHandler(OnCollision_rightThigh_Rat));
		Fixture rightThighBody4 = _rightThighBody2;
		rightThighBody4.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody4.OnSeparation, new SeparationEventHandler(OnSeparation_rightThigh_Rat));
		_leftThighJoint1 = new RevoluteJoint(_bodyBody.Body, _leftThighBody1.Body, new Vector2(0f - (num / 2f - 1.5f), num2), new Vector2(0f, 0f));
		_leftThighJoint1.MotorEnabled = true;
		_leftThighJoint1.MaxMotorTorque = 100000000f;
		_leftThighJoint1.MotorSpeed = 0f;
		_leftThighJoint1.CollideConnected = false;
		_rightThighJoint1 = new RevoluteJoint(_bodyBody.Body, _rightThighBody1.Body, new Vector2(num / 2f - 1.5f, num3), new Vector2(0f, 0f));
		_rightThighJoint1.MotorEnabled = true;
		_rightThighJoint1.MaxMotorTorque = 100000000f;
		_rightThighJoint1.MotorSpeed = 0f;
		_rightThighJoint1.CollideConnected = false;
		_leftThighJoint2 = new RevoluteJoint(_bodyBody.Body, _leftThighBody2.Body, new Vector2(0f - (num / 2f - 1.5f), 0f - num2), new Vector2(0f, 0f));
		_leftThighJoint2.MotorEnabled = true;
		_leftThighJoint2.MaxMotorTorque = 100000000f;
		_leftThighJoint2.MotorSpeed = 0f;
		_leftThighJoint2.CollideConnected = false;
		_rightThighJoint2 = new RevoluteJoint(_bodyBody.Body, _rightThighBody2.Body, new Vector2(num / 2f - 1.5f, 0f - num3), new Vector2(0f, 0f));
		_rightThighJoint2.MotorEnabled = true;
		_rightThighJoint2.MaxMotorTorque = 100000000f;
		_rightThighJoint2.MotorSpeed = 0f;
		_rightThighJoint2.CollideConnected = false;
		_world.AddJoint(_leftThighJoint1);
		_world.AddJoint(_rightThighJoint1);
		_world.AddJoint(_leftThighJoint2);
		_world.AddJoint(_rightThighJoint2);
	}

	public void LoadBat(Level level, World _world, string EnemyType)
	{
		EnemyHPBody = 100f;
		EnemyMana = ManaMax;
		Scaler = 0.6f;
		LegScaler = 0.75f;
		ArmScaler = 0.75f;
		_SightBrush = content.Load<Texture2D>("Sights/Red");
		string text = "Sprites/Enemy/" + EnemyType + "/";
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 10000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.Body.LinearDamping = 0f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrush = content.Load<Texture2D>(text + "body");
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 10000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.LinearDamping = 0f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrush = content.Load<Texture2D>(text + "head");
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_head));
		Fixture headBody2 = _headBody;
		headBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(headBody2.OnSeparation, new SeparationEventHandler(OnSeparation_head));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 10000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrush = content.Load<Texture2D>(text + "leftArm");
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 10000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrush = content.Load<Texture2D>(text + "rightArm");
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 10000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 1f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrush = content.Load<Texture2D>(text + "leftHand");
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 10000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 1f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrush = content.Load<Texture2D>(text + "rightHand");
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 10000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.LinearDamping = 0f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrush = content.Load<Texture2D>(text + "leftLeg");
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 10000f;
		_rightThighBody.Body.LinearDamping = 0f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrush = content.Load<Texture2D>(text + "rightLeg");
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightThigh));
		_kineticShields = FixtureFactory.CreateCircle(_world, 20f, 1f);
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		_kineticShields.Body.BodyType = BodyType.Kinematic;
		_kineticShields.Body.IsBullet = true;
		_kineticShields.Friction = 0f;
		_kineticShields.Restitution = 1f;
		_kineticShields.Body.UserData = 120;
		_kineticShields.Body.LinearDamping = 0f;
		_kineticShields.CollisionGroup = CollisionGroup;
		_kineticShields.CollidesWith = CollisionCategory.None;
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
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
		_DartKineticZone = FixtureFactory.CreateCircle(_world, 50f, 1E-06f);
		_DartKineticZone.Body.Position = Position * 0.2f;
		_DartKineticZone.Body.BodyType = BodyType.Dynamic;
		_DartKineticZone.Body.UserData = 99;
		_DartKineticZone.UserData = 20;
		_DartKineticZone.Body.Active = false;
		_DartKineticZone.IsSensor = true;
		Fixture dartKineticZone = _DartKineticZone;
		dartKineticZone.OnCollision = (CollisionEventHandler)Delegate.Combine(dartKineticZone.OnCollision, new CollisionEventHandler(DartKinetic_OnCollision_Zone));
	}

	public void LoadWereLimer(Level level, World _world, string EnemyType)
	{
		EnemyHPBody = 100f;
		EnemyMana = ManaMax;
		LegScaler = 0.75f;
		ArmScaler = 0.75f;
		_SightBrush = content.Load<Texture2D>("Sights/Red");
		string text = "Sprites/Enemy" + EnemyType + "/";
		Alive = true;
		_bodyBody = FixtureFactory.CreateEllipse(_world, 3f, 4.4f, 10, density);
		_bodyBody.Body.Position = _PhyPosition;
		_bodyBody.Body.BodyType = BodyType.Dynamic;
		_bodyBody.Body.SleepingAllowed = true;
		_bodyBody.Density = 1E-07f * PhysicsScaleUp;
		_bodyBody.Friction = 10000f;
		_bodyBody.Body.UserData = 8;
		_bodyBody.Body.LinearDamping = 0f;
		_bodyBody.CollisionGroup = CollisionGroup;
		_bodyBrush = content.Load<Texture2D>(text + "body");
		_bodyBrushOrigin = new Vector2(_bodyBrush.Width / 2, _bodyBrush.Height / 2);
		Fixture bodyBody = _bodyBody;
		bodyBody.OnCollision = (CollisionEventHandler)Delegate.Combine(bodyBody.OnCollision, new CollisionEventHandler(OnCollision_body));
		Fixture bodyBody2 = _bodyBody;
		bodyBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody2.OnSeparation, new SeparationEventHandler(OnSeparation_body));
		_headBody = FixtureFactory.CreateEllipse(_world, 2.2f, 3f, 10, density);
		_headBody.Body.Position = _PhyPosition - new Vector2(0f, 9.400001f);
		_headBody.Body.BodyType = BodyType.Dynamic;
		_headBody.Body.SleepingAllowed = true;
		_headBody.Density = 2E-08f;
		_headBody.Friction = 10000f;
		_headBody.Body.UserData = 8;
		_headBody.Body.LinearDamping = 0f;
		_headBody.CollisionGroup = CollisionGroup;
		_headBrush = content.Load<Texture2D>(text + "head");
		_headBrushOrigin = new Vector2(_headBrush.Width / 2, _headBrush.Height / 2);
		Fixture headBody = _headBody;
		headBody.OnCollision = (CollisionEventHandler)Delegate.Combine(headBody.OnCollision, new CollisionEventHandler(OnCollision_head));
		Fixture bodyBody3 = _bodyBody;
		bodyBody3.OnSeparation = (SeparationEventHandler)Delegate.Combine(bodyBody3.OnSeparation, new SeparationEventHandler(OnSeparation_body));
		_leftUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_leftUpperArmBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 2.2f);
		_leftUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_leftUpperArmBody.Body.SleepingAllowed = true;
		_leftUpperArmBody.Density = 2E-12f;
		_leftUpperArmBody.Friction = 10000f;
		_leftUpperArmBody.Body.UserData = 8;
		_leftUpperArmBody.Body.AngularDamping = 1f;
		_leftUpperArmBody.CollisionGroup = CollisionGroup;
		_leftUpperArmBrush = content.Load<Texture2D>(text + "leftArm");
		_leftUpperArmBrushOrigin = new Vector2(_leftUpperArmBrush.Width / 2, _leftUpperArmBrush.Height / 2);
		Fixture leftUpperArmBody = _leftUpperArmBody;
		leftUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_leftHand));
		Fixture leftUpperArmBody2 = _leftUpperArmBody;
		leftUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftHand));
		_rightUpperArmBody = FixtureFactory.CreateEllipse(_world, 1.2f, 6.6f, 10, density);
		_rightUpperArmBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -2.2f);
		_rightUpperArmBody.Body.BodyType = BodyType.Dynamic;
		_rightUpperArmBody.Body.SleepingAllowed = true;
		_rightUpperArmBody.Density = 2E-12f;
		_rightUpperArmBody.Friction = 10000f;
		_rightUpperArmBody.Body.UserData = 8;
		_rightUpperArmBody.Body.AngularDamping = 1f;
		_rightUpperArmBody.CollisionGroup = CollisionGroup;
		_rightUpperArmBrush = content.Load<Texture2D>(text + "rightArm");
		_rightUpperArmBrushOrigin = new Vector2(_rightUpperArmBrush.Width / 2, _rightUpperArmBrush.Height / 2);
		Fixture rightUpperArmBody = _rightUpperArmBody;
		rightUpperArmBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightUpperArmBody.OnCollision, new CollisionEventHandler(OnCollision_rightHand));
		Fixture rightUpperArmBody2 = _rightUpperArmBody;
		rightUpperArmBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightUpperArmBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightHand));
		_leftHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_leftHandBody.Body.Position = _bodyBody.Body.Position - new Vector2(14.2f, 3.3f);
		_leftHandBody.Body.BodyType = BodyType.Dynamic;
		_leftHandBody.Body.SleepingAllowed = true;
		_leftHandBody.Density = 2E-12f;
		_leftHandBody.Friction = 10000f;
		_leftHandBody.Body.UserData = 8;
		_leftHandBody.Body.AngularDamping = 1f;
		_leftHandBody.CollisionGroup = CollisionGroup;
		_leftHandBody.CollidesWith = CollisionCategory.None;
		_leftHandBody.CollisionCategories = CollisionCategory.None;
		_leftHandBody.Body.IsBullet = true;
		_leftHandBrush = content.Load<Texture2D>(text + "leftHand");
		_leftHandBrushOrigin = new Vector2(_leftHandBrush.Width / 2, _leftHandBrush.Height / 2);
		Fixture leftHandBody = _leftHandBody;
		leftHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftHandBody.OnCollision, new CollisionEventHandler(OnCollision_leftHand));
		Fixture leftHandBody2 = _leftHandBody;
		leftHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftHand));
		_rightHandBody = FixtureFactory.CreateEllipse(_world, 0.8f, 1.6f, 10, density);
		_rightHandBody.Body.Position = _bodyBody.Body.Position + new Vector2(14.2f, -3.3f);
		_rightHandBody.Body.BodyType = BodyType.Dynamic;
		_rightHandBody.Body.SleepingAllowed = true;
		_rightHandBody.Density = 2E-12f;
		_rightHandBody.Friction = 10000f;
		_rightHandBody.Body.UserData = 8;
		_rightHandBody.Body.AngularDamping = 1f;
		_rightHandBody.Body.IsBullet = true;
		_rightHandBody.CollisionGroup = CollisionGroup;
		_rightHandBody.CollidesWith = CollisionCategory.None;
		_rightHandBody.CollisionCategories = CollisionCategory.None;
		_rightHandBrush = content.Load<Texture2D>(text + "rightHand");
		_rightHandBrushOrigin = new Vector2(_rightHandBrush.Width / 2, _rightHandBrush.Height / 2);
		Fixture rightHandBody = _rightHandBody;
		rightHandBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightHandBody.OnCollision, new CollisionEventHandler(OnCollision_rightHand));
		Fixture rightHandBody2 = _rightHandBody;
		rightHandBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightHandBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightHand));
		_leftThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_leftThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(3f, 6f);
		_leftThighBody.Body.Rotation = 1.5f;
		_leftThighBody.Body.BodyType = BodyType.Dynamic;
		_leftThighBody.Body.SleepingAllowed = true;
		_leftThighBody.Density = 2E-09f;
		_leftThighBody.Friction = 10000f;
		_leftThighBody.Body.UserData = 8;
		_leftThighBody.Body.LinearDamping = 0f;
		_leftThighBody.CollisionGroup = CollisionGroup;
		_leftThighBrush = content.Load<Texture2D>(text + "leftLeg");
		_leftThighBrushOrigin = new Vector2(_leftThighBrush.Width / 2, _leftThighBrush.Height / 2 + 15);
		Fixture leftThighBody = _leftThighBody;
		leftThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(leftThighBody.OnCollision, new CollisionEventHandler(OnCollision_leftThigh));
		Fixture leftThighBody2 = _leftThighBody;
		leftThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(leftThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_leftThigh));
		_rightThighBody = FixtureFactory.CreateEllipse(_world, 1.2f, 1.6f, 10, density);
		_rightThighBody.Body.Position = _bodyBody.Body.Position + new Vector2(-3f, 6f);
		_rightThighBody.Body.Rotation = -1.5f;
		_rightThighBody.Body.BodyType = BodyType.Dynamic;
		_rightThighBody.Body.SleepingAllowed = true;
		_rightThighBody.Density = 2E-09f;
		_rightThighBody.Friction = 10000f;
		_rightThighBody.Body.LinearDamping = 0f;
		_rightThighBody.Body.UserData = 8;
		_rightThighBody.CollisionGroup = CollisionGroup;
		_rightThighBrush = content.Load<Texture2D>(text + "rightLeg");
		_rightThighBrushOrigin = new Vector2(_rightThighBrush.Width / 2, _rightThighBrush.Height / 2 + 15);
		Fixture rightThighBody = _rightThighBody;
		rightThighBody.OnCollision = (CollisionEventHandler)Delegate.Combine(rightThighBody.OnCollision, new CollisionEventHandler(OnCollision_rightThigh));
		Fixture rightThighBody2 = _rightThighBody;
		rightThighBody2.OnSeparation = (SeparationEventHandler)Delegate.Combine(rightThighBody2.OnSeparation, new SeparationEventHandler(OnSeparation_rightThigh));
		_kineticShields = FixtureFactory.CreateCircle(_world, 20f, 1f);
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		_kineticShields.Body.BodyType = BodyType.Kinematic;
		_kineticShields.Body.IsBullet = true;
		_kineticShields.Friction = 0f;
		_kineticShields.Restitution = 1f;
		_kineticShields.Body.UserData = 120;
		_kineticShields.Body.LinearDamping = 0f;
		_kineticShields.CollisionGroup = CollisionGroup;
		_kineticShields.CollidesWith = CollisionCategory.None;
		NeckJoint = new RevoluteJoint(_bodyBody.Body, _headBody.Body, new Vector2(0f, -4.4f), new Vector2(0f, 3f));
		NeckAngleJoint = new AngleJoint(_bodyBody.Body, _headBody.Body);
		_bodyAngleJoint = new FixedAngleJoint(_bodyBody.Body);
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
		_DartKineticZone = FixtureFactory.CreateCircle(_world, 50f, 1E-06f);
		_DartKineticZone.Body.Position = Position * 0.2f;
		_DartKineticZone.Body.BodyType = BodyType.Dynamic;
		_DartKineticZone.Body.UserData = 99;
		_DartKineticZone.UserData = 20;
		_DartKineticZone.Body.Active = false;
		_DartKineticZone.IsSensor = true;
		Fixture dartKineticZone = _DartKineticZone;
		dartKineticZone.OnCollision = (CollisionEventHandler)Delegate.Combine(dartKineticZone.OnCollision, new CollisionEventHandler(DartKinetic_OnCollision_Zone));
	}

	private bool OnCollision_body(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null && fixtureB.CollisionGroup != 0)
		{
			if (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130)
			{
				_ = (int)fixtureB.Body.UserData;
				_ = 120;
				if ((int)fixtureB.Body.UserData == 121)
				{
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartBoneDamage;
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartKineticDamage;
				}
			}
			if ((int)fixtureB.Body.UserData == 99)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				_bodyBody.Body.LinearDamping = 10000f;
				_headBody.Body.LinearDamping = 10000f;
				Dead = true;
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
		}
		return true;
	}

	private void OnSeparation_body(Fixture fixtureA, Fixture fixtureB)
	{
		if ((int)fixtureB.Body.UserData == 99)
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_head(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				_ = (int)fixtureB.Body.UserData;
				_ = 120;
				if ((int)fixtureB.Body.UserData == 121)
				{
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartBoneDamage;
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartKineticDamage;
				}
			}
			if ((int)fixtureB.Body.UserData == 99)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				_bodyBody.Body.LinearDamping = 10000f;
				_headBody.Body.LinearDamping = 10000f;
				Dead = true;
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
		}
		return true;
	}

	private void OnSeparation_head(Fixture fixtureA, Fixture fixtureB)
	{
		if ((int)fixtureB.Body.UserData == 99)
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_leftHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				_ = (int)fixtureB.Body.UserData;
				_ = 120;
				if ((int)fixtureB.Body.UserData == 121)
				{
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartBoneDamage;
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartKineticDamage;
				}
			}
			if ((int)fixtureB.Body.UserData == 99)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
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

	private void OnSeparation_leftHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandLeftColor = Color.White;
	}

	private bool OnCollision_rightHand(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				_ = (int)fixtureB.Body.UserData;
				_ = 120;
				if ((int)fixtureB.Body.UserData == 121)
				{
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartBoneDamage;
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartKineticDamage;
				}
			}
			if ((int)fixtureB.Body.UserData == 99)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
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

	private void OnSeparation_rightHand(Fixture fixtureA, Fixture fixtureB)
	{
		HandRightColor = Color.White;
	}

	private bool OnCollision_leftThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				_ = (int)fixtureB.Body.UserData;
				_ = 120;
				if ((int)fixtureB.Body.UserData == 121)
				{
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartBoneDamage;
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartKineticDamage;
				}
			}
			if ((int)fixtureB.Body.UserData == 99)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
		}
		ThighLeftColor = Color.Red;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = true;
		}
		else
		{
			LeftFootIsOnGround = false;
		}
		return true;
	}

	private void OnSeparation_leftThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			WasDustyLeft = false;
		}
		if ((int)fixtureB.Body.UserData == 99)
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_rightThigh(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && (fixtureB.CollisionGroup != CannonBallCollisionGroup || fixtureB.CollisionGroup != 130))
			{
				_ = (int)fixtureB.Body.UserData;
				_ = 120;
				if ((int)fixtureB.Body.UserData == 121)
				{
					Frozen = true;
				}
				if ((int)fixtureB.Body.UserData == 122)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartBoneDamage;
				}
				if ((int)fixtureB.Body.UserData == 199)
				{
					particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
					particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					particleEffectBleed.Trigger(new Vector2(0f, 0f));
					EnemyHPBody -= DartKineticDamage;
				}
			}
			if ((int)fixtureB.Body.UserData == 99)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
			if ((int)fixtureB.Body.UserData == 98)
			{
				particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
				particleEffectBleed[0].ReleaseImpulse = new Vector2(0f, 150f);
				particleEffectBleed.Trigger(new Vector2(0f, 0f));
				EnemyHPBody -= SharpsNeddleDamage;
			}
		}
		ThighRightColor = Color.Red;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = true;
		}
		else
		{
			RightFootIsOnGround = false;
		}
		return true;
	}

	private void OnSeparation_rightThigh(Fixture fixtureA, Fixture fixtureB)
	{
		ThighRightColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			WasDustyRight = false;
		}
		if ((int)fixtureB.Body.UserData == 99)
		{
			_bodyBody.Body.AngularDamping = 0f;
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
		}
	}

	private bool OnCollision_leftThigh_Rat(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && fixtureB.CollisionGroup == CannonBallCollisionGroup)
			{
				_ = fixtureB.CollisionGroup;
				_ = 130;
			}
			_ = (int)fixtureB.Body.UserData;
			_ = 99;
			_ = (int)fixtureB.Body.UserData;
			_ = 98;
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				LeftFootIsOnGround = true;
				new Vector2(0f, 0f);
				for (int i = 0; i < contact.Manifold.PointCount; i++)
				{
					_ = contact.Manifold.Points[i].LocalPoint;
				}
				LeftFootGrabPoint = fixtureB.Body.Position;
				LeftFootGrabOther = fixtureB;
			}
		}
		return true;
	}

	private void OnSeparation_leftThigh_Rat(Fixture fixtureA, Fixture fixtureB)
	{
		ThighLeftColor = Color.White;
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			LeftFootIsOnGround = false;
			LeftFootGrabPoint = new Vector2(0f, 0f);
			LeftFootGrabOther = null;
			WasDustyLeft = false;
		}
		_ = (int)fixtureB.Body.UserData;
		_ = 99;
	}

	private bool OnCollision_rightThigh_Rat(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB != null)
		{
			if (fixtureB.CollisionGroup != 0 && fixtureB.CollisionGroup == CannonBallCollisionGroup)
			{
				_ = fixtureB.CollisionGroup;
				_ = 130;
			}
			_ = (int)fixtureB.Body.UserData;
			_ = 99;
			_ = (int)fixtureB.Body.UserData;
			_ = 98;
			if (fixtureB.CollisionGroup != CollisionGroup)
			{
				RightFootIsOnGround = true;
				new Vector2(0f, 0f);
				for (int i = 0; i < contact.Manifold.PointCount; i++)
				{
					_ = contact.Manifold.Points[i].LocalPoint;
				}
				RightFootGrabPoint = fixtureB.Body.Position;
				RightFootGrabOther = fixtureB;
			}
		}
		return true;
	}

	private void OnSeparation_rightThigh_Rat(Fixture fixtureA, Fixture fixtureB)
	{
		if (fixtureB.CollisionGroup != CollisionGroup)
		{
			RightFootIsOnGround = false;
			RightFootGrabPoint = new Vector2(0f, 0f);
			RightFootGrabOther = null;
			WasDustyRight = false;
		}
		_ = (int)fixtureB.Body.UserData;
		_ = 99;
	}

	private bool CannonBall_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB.Body.Active && fixtureB.CollisionGroup != CollisionGroup)
		{
			if (fixtureA.CollisionGroup != 130)
			{
				CannonBallSound.Play();
			}
			fixtureA.CollidesWith = CollisionCategory.None;
			fixtureA.CollisionGroup = 130;
			fixtureA.Body.UserData = 130;
			fixtureA.Body.Active = false;
		}
		return true;
	}

	private bool IceBall_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (fixtureB.Body.Active && fixtureB.CollisionGroup != CollisionGroup && fixtureA.CollisionGroup != 130)
		{
			fixtureA.Body.UserData = 999;
		}
		return true;
	}

	private bool DartBone_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		WeldDecal = true;
		FixA = fixtureA;
		FixB = fixtureB;
		fixtureA.Body.IgnoreGravity = false;
		fixtureA.Body.UserData = 999;
		return true;
	}

	private bool DartKinetic_OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		CannonBallSound.Play(1f, 1f, 0f);
		particleEffectBleed[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
		particleEffectKineticEx[0].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
		particleEffectKineticEx[1].TriggerOffset = fixtureA.Body.GetWorldPoint(contact.Manifold.Points[0].LocalPoint) * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
		particleEffectKineticEx.Trigger(new Vector2(0f, 0f));
		KineticGo = true;
		DartKineticDart = fixtureA;
		return true;
	}

	private bool DartKinetic_OnCollision_Zone(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		return true;
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

	private void BodyPartsManager4(GameTime gameTime)
	{
		if (Dead)
		{
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
		}
	}

	public void Particle_Freeze(GameTime gametime)
	{
		OldGameTime = gametime.TotalGameTime.TotalSeconds;
		_bodyBody.Body.LinearDamping = 10000f;
		_headBody.Body.LinearDamping = 10000f;
		_leftUpperArmBody.Body.LinearDamping = 10000f;
		_rightUpperArmBody.Body.LinearDamping = 10000f;
		_leftThighBody.Body.LinearDamping = 10000f;
		_rightThighBody.Body.LinearDamping = 10000f;
		_leftHandBody.Body.LinearDamping = 10000f;
		_rightHandBody.Body.LinearDamping = 10000f;
		_bodyBody.Body.AngularDamping = 10000f;
		_headBody.Body.AngularDamping = 10000f;
		_leftUpperArmBody.Body.AngularDamping = 10000f;
		_rightUpperArmBody.Body.AngularDamping = 10000f;
		_leftThighBody.Body.AngularDamping = 10000f;
		_rightThighBody.Body.AngularDamping = 10000f;
		_leftHandBody.Body.AngularDamping = 10000f;
		_rightHandBody.Body.AngularDamping = 10000f;
		Color = Color.CadetBlue;
		Frozen = false;
	}

	private void UpdateFreeze(GameTime gametime)
	{
		int num = 15;
		if (gametime.TotalGameTime.TotalSeconds - OldGameTime > (double)num && gametime.TotalGameTime.TotalSeconds - OldGameTime < (double)(num + 2))
		{
			_bodyBody.Body.LinearDamping = 0f;
			_headBody.Body.LinearDamping = 0f;
			_leftUpperArmBody.Body.LinearDamping = 0f;
			_rightUpperArmBody.Body.LinearDamping = 0f;
			_leftThighBody.Body.LinearDamping = 0f;
			_rightThighBody.Body.LinearDamping = 0f;
			_leftHandBody.Body.LinearDamping = 0f;
			_rightHandBody.Body.LinearDamping = 0f;
			_bodyBody.Body.AngularDamping = 0f;
			_headBody.Body.AngularDamping = 0f;
			_leftUpperArmBody.Body.AngularDamping = 0f;
			_rightUpperArmBody.Body.AngularDamping = 0f;
			_leftThighBody.Body.AngularDamping = 0f;
			_rightThighBody.Body.AngularDamping = 0f;
			_leftHandBody.Body.AngularDamping = 0f;
			_rightHandBody.Body.AngularDamping = 0f;
			Color = Color.White;
			OldGameTime = 0.0;
		}
	}

	public void Boom()
	{
		Vector2 vector = new Vector2(1000000f, 1000000f);
		_bodyBody.Body.ApplyLinearImpulse((_bodyBody.Body.Position + new Vector2(0f, -50f)) * vector, _bodyBody.Body.Position);
		_headBody.Body.ApplyLinearImpulse((_headBody.Body.Position - _bodyBody.Body.Position) * vector, _headBody.Body.Position);
		_leftUpperArmBody.Body.ApplyLinearImpulse((_leftUpperArmBody.Body.Position - _bodyBody.Body.Position) * vector, _leftUpperArmBody.Body.Position);
		_rightUpperArmBody.Body.ApplyLinearImpulse((_rightUpperArmBody.Body.Position - _bodyBody.Body.Position) * vector, _rightUpperArmBody.Body.Position);
		_leftThighBody.Body.ApplyLinearImpulse((_leftThighBody.Body.Position - _bodyBody.Body.Position) * vector, _leftThighBody.Body.Position);
		_rightThighBody.Body.ApplyLinearImpulse((_rightThighBody.Body.Position - _bodyBody.Body.Position) * vector, _rightThighBody.Body.Position);
		_leftHandBody.Body.ApplyLinearImpulse((_leftHandBody.Body.Position - _bodyBody.Body.Position) * vector, _leftHandBody.Body.Position);
		_rightHandBody.Body.ApplyLinearImpulse((_rightHandBody.Body.Position - _bodyBody.Body.Position) * vector, _rightHandBody.Body.Position);
	}

	public void RemoveAll(World _world)
	{
		Active = false;
		if (_bodyBody != null && _bodyBody.Body != null && _bodyBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_bodyBody.Body);
		}
		if (_headBody != null && _headBody.Body != null && _headBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_headBody.Body);
		}
		if (_leftUpperArmBody != null && _leftUpperArmBody.Body != null && _leftUpperArmBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_leftUpperArmBody.Body);
		}
		if (_rightUpperArmBody != null && _rightUpperArmBody.Body != null && _rightUpperArmBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_rightUpperArmBody.Body);
		}
		if (_leftHandBody != null && _leftHandBody.Body != null && _leftHandBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_leftHandBody.Body);
		}
		if (_rightHandBody != null && _rightHandBody.Body != null && _rightHandBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_rightHandBody.Body);
		}
		if (_leftThighBody != null && _leftThighBody.Body != null && _leftThighBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_leftThighBody.Body);
		}
		if (_rightThighBody != null && _rightThighBody.Body != null && _rightThighBody.Body.FixtureList != null)
		{
			_world.RemoveBody(_rightThighBody.Body);
		}
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBall[i] != null && _CannonBall[i].Body != null && _CannonBall[i].Body.FixtureList != null)
			{
				_world.RemoveBody(_CannonBall[i].Body);
			}
		}
		for (int j = 0; j < IceBallIndex; j++)
		{
			if (_IceBall[j] != null && _IceBall[j].Body != null && _IceBall[j].Body.FixtureList != null)
			{
				_world.RemoveBody(_IceBall[j].Body);
			}
		}
		for (int k = 0; k < DartBoneIndex; k++)
		{
			if (_DartBone[k] != null && _DartBone[k].Body != null && _DartBone[k].Body.FixtureList != null)
			{
				_world.RemoveBody(_DartBone[k].Body);
			}
		}
	}

	public void ActiveAll_True(World _world)
	{
		if (_bodyBody != null && _bodyBody.Body != null && !_bodyBody.Body.Active)
		{
			_bodyBody.Body.Active = true;
		}
		if (_headBody != null && _headBody.Body != null && !_headBody.Body.Active)
		{
			_headBody.Body.Active = true;
		}
		if (_leftUpperArmBody != null && _leftUpperArmBody.Body != null && !_leftUpperArmBody.Body.Active)
		{
			_leftUpperArmBody.Body.Active = true;
		}
		if (_rightUpperArmBody != null && _rightUpperArmBody.Body != null && !_rightUpperArmBody.Body.Active)
		{
			_rightUpperArmBody.Body.Active = true;
		}
		if (_leftHandBody != null && _leftHandBody.Body != null && !_leftHandBody.Body.Active)
		{
			_leftHandBody.Body.Active = true;
		}
		if (_rightHandBody != null && _rightHandBody.Body != null && !_rightHandBody.Body.Active)
		{
			_rightHandBody.Body.Active = true;
		}
		if (_leftThighBody != null && _leftThighBody.Body != null && !_leftThighBody.Body.Active)
		{
			_leftThighBody.Body.Active = true;
		}
		if (_rightThighBody != null && _rightThighBody.Body != null && !_rightThighBody.Body.Active)
		{
			_rightThighBody.Body.Active = true;
		}
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBall[i] != null && _CannonBall[i].Body != null && _CannonBall[i].Body.FixtureList != null && !_CannonBall[i].Body.Active)
			{
				_CannonBall[i].Body.Active = true;
			}
		}
		for (int j = 0; j < IceBallIndex; j++)
		{
			if (_IceBall[j] != null && _IceBall[j].Body != null && _IceBall[j].Body.FixtureList != null && !_IceBall[j].Body.Active)
			{
				_IceBall[j].Body.Active = true;
			}
		}
		for (int k = 0; k < DartBoneIndex; k++)
		{
			if (_DartBone[k] != null && _DartBone[k].Body != null && _DartBone[k].Body.FixtureList != null && !_DartBone[k].Body.Active)
			{
				_DartBone[k].Body.Active = true;
			}
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
		if (_leftThighBody != null && _leftThighBody.Body != null && _leftThighBody.Body.Active)
		{
			_leftThighBody.Body.Active = false;
		}
		if (_rightThighBody != null && _rightThighBody.Body != null && _rightThighBody.Body.Active)
		{
			_rightThighBody.Body.Active = false;
		}
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBall[i] != null && _CannonBall[i].Body != null && _CannonBall[i].Body.FixtureList != null && _CannonBall[i].Body.Active)
			{
				_CannonBall[i].Body.Active = false;
			}
		}
		for (int j = 0; j < IceBallIndex; j++)
		{
			if (_IceBall[j] != null && _IceBall[j].Body != null && _IceBall[j].Body.FixtureList != null && _IceBall[j].Body.Active)
			{
				_IceBall[j].Body.Active = false;
			}
		}
		for (int k = 0; k < DartBoneIndex; k++)
		{
			if (_DartBone[k] != null && _DartBone[k].Body != null && _DartBone[k].Body.FixtureList != null && _DartBone[k].Body.Active)
			{
				_DartBone[k].Body.Active = false;
			}
		}
	}

	public void DestroyAll(World _world)
	{
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
		if (_leftThighBody != null && _leftThighBody.Body != null)
		{
			_world.RemoveBody(_leftThighBody.Body);
		}
		if (_rightThighBody != null && _rightThighBody.Body != null)
		{
			_world.RemoveBody(_rightThighBody.Body);
		}
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBall[i] != null && _CannonBall[i].Body != null)
			{
				_world.RemoveBody(_CannonBall[i].Body);
			}
		}
		for (int j = 0; j < IceBallIndex; j++)
		{
			if (_IceBall[j] != null && _IceBall[j].Body != null)
			{
				_world.RemoveBody(_IceBall[j].Body);
			}
		}
		for (int k = 0; k < DartBoneIndex; k++)
		{
			if (_DartBone[k] != null && _DartBone[k].Body != null)
			{
				_world.RemoveBody(_DartBone[k].Body);
			}
		}
	}

	public void Update(GameTime gameTime, World _world)
	{
		if (Active)
		{
			ActiveAll_True(_world);
			switch (EnemyType)
			{
			case "0":
				UpdateRat(gameTime, _world);
				break;
			case "1":
				UpdateBat(gameTime, _world);
				break;
			case "2":
				UpdateWereLimer(gameTime, _world);
				break;
			default:
				UpdateRat(gameTime, _world);
				break;
			}
		}
		else
		{
			ActiveAll_False(_world);
		}
	}

	public void UpdateRat(GameTime gameTime, World _world)
	{
		if (Alive)
		{
			Update_Injuries_Rat(gameTime, _world);
			_leftThighBodyAvePosition = (_leftThighBody1.Body.Position + _leftThighBody2.Body.Position) / 2f;
			_rightThighBodyAvePosition = (_rightThighBody1.Body.Position + _rightThighBody2.Body.Position) / 2f;
			float num = 50f;
			if (_leftThighBodyAvePosition.X < _rightThighBodyAvePosition.X)
			{
				HFlip = false;
			}
			else if (_leftThighBodyAvePosition.X > _rightThighBodyAvePosition.X)
			{
				HFlip = true;
			}
			if (_bodyBody.Body.Rotation - _bodyBody.Body.Revolutions * ((float)Math.PI * 2f) > (float)Math.PI)
			{
				VFlip = true;
			}
			else
			{
				VFlip = false;
			}
			RaySight(_world);
			float num2 = 10000f;
			if (RaycastHitFirst != null)
			{
				Vector2 vector = RaycastHitFirst.Body.Position - _bodyBody.Body.Position;
				float num3 = 1000f;
				if (vector.X < num3)
				{
					if (vector.X > 0f - num3)
					{
						if (vector.Y < num3)
						{
							if (vector.Y > 0f - num3)
							{
								if (RaycastHitFirst.Body.Position.X > _bodyBody.Body.Position.X)
								{
									if (HFlip)
									{
										DirectionRight = false;
										DirectionLeft = true;
										RunRotLeft = num;
										RunRotRight = num + num / 4f;
									}
									else
									{
										DirectionRight = true;
										DirectionLeft = false;
										RunRotLeft = num + num / 4f;
										RunRotRight = num;
									}
									_leftThighJoint1.MotorSpeed = RunRotLeft;
									_rightThighJoint1.MotorSpeed = RunRotRight;
									_leftThighJoint2.MotorSpeed = RunRotLeft;
									_rightThighJoint2.MotorSpeed = RunRotRight;
									if (LeftFootIsOnGround && RightFootIsOnGround)
									{
										Vector2 force = (_leftThighBodyAvePosition - _rightThighBodyAvePosition) / num2;
										_bodyBody.Body.ApplyForce(force);
									}
								}
								else if (RaycastHitFirst.Body.Position.X < _bodyBody.Body.Position.X)
								{
									if (HFlip)
									{
										DirectionRight = true;
										DirectionLeft = false;
										RunRotLeft = num + num / 4f;
										RunRotRight = num;
									}
									else
									{
										DirectionRight = false;
										DirectionLeft = true;
										RunRotLeft = num;
										RunRotRight = num + num / 4f;
									}
									_leftThighJoint1.MotorSpeed = 0f - RunRotLeft;
									_rightThighJoint1.MotorSpeed = 0f - RunRotRight;
									_leftThighJoint2.MotorSpeed = 0f - RunRotLeft;
									_rightThighJoint2.MotorSpeed = 0f - RunRotRight;
									if (RightFootIsOnGround && LeftFootIsOnGround)
									{
										Vector2 force2 = (_leftThighBodyAvePosition - _rightThighBodyAvePosition) / num2;
										_bodyBody.Body.ApplyForce(force2);
									}
								}
							}
							else
							{
								_leftThighJoint1.MotorSpeed = 0f;
								_rightThighJoint1.MotorSpeed = 0f;
								_leftThighJoint2.MotorSpeed = 0f;
								_rightThighJoint2.MotorSpeed = 0f;
							}
						}
						else
						{
							_leftThighJoint1.MotorSpeed = 0f;
							_rightThighJoint1.MotorSpeed = 0f;
							_leftThighJoint2.MotorSpeed = 0f;
							_rightThighJoint2.MotorSpeed = 0f;
						}
					}
					else
					{
						_leftThighJoint1.MotorSpeed = 0f;
						_rightThighJoint1.MotorSpeed = 0f;
						_leftThighJoint2.MotorSpeed = 0f;
						_rightThighJoint2.MotorSpeed = 0f;
					}
				}
				else
				{
					_leftThighJoint1.MotorSpeed = 0f;
					_rightThighJoint1.MotorSpeed = 0f;
					_leftThighJoint2.MotorSpeed = 0f;
					_rightThighJoint2.MotorSpeed = 0f;
				}
			}
			else
			{
				_leftThighJoint1.MotorSpeed = RunRotLeft / 20f;
				_rightThighJoint1.MotorSpeed = RunRotRight / 20f;
				_leftThighJoint2.MotorSpeed = RunRotLeft / 20f;
				_rightThighJoint2.MotorSpeed = RunRotRight / 20f;
				if (LeftFootIsOnGround)
				{
					Vector2 force3 = (_rightThighBodyAvePosition - _leftThighBodyAvePosition) / 5000f;
					_bodyBody.Body.ApplyForce(force3);
				}
			}
			float num4 = 0.8f;
			GrabDist = 0.001f;
			if (HFlip)
			{
				Vec = RayGrab(_world, _leftThighBody1, _leftThighBody2);
				if (Vec != new Vector2(0f, 0f))
				{
					Vector2 vector2 = new Vector2(0f - GrabDist, 0f - GrabDist) / (Vec - _leftThighBodyAvePosition);
					_leftThighBody1.Body.ApplyForce(vector2 / num4);
					Vec = new Vector2(0f, 0f);
				}
				Vec = RayGrab(_world, _leftThighBody1, _leftThighBody2);
				if (Vec != new Vector2(0f, 0f))
				{
					Vector2 vector3 = new Vector2(0f - GrabDist, 0f - GrabDist) / (Vec - _leftThighBodyAvePosition);
					_rightThighBody1.Body.ApplyForce(vector3 / num4);
					Vec = new Vector2(0f, 0f);
				}
			}
			else
			{
				if (LeftFootIsOnGround)
				{
					Vec = RayGrab(_world, _leftThighBody2, _leftThighBody1);
					if (Vec != new Vector2(0f, 0f))
					{
						Vector2 vector4 = new Vector2(GrabDist, GrabDist) / (Vec - _leftThighBodyAvePosition);
						_leftThighBody1.Body.ApplyForce(vector4 / num4);
						Vec = new Vector2(0f, 0f);
					}
				}
				if (RightFootIsOnGround)
				{
					Vec = RayGrab(_world, _leftThighBody2, _leftThighBody1);
					if (Vec != new Vector2(0f, 0f))
					{
						Vector2 vector5 = new Vector2(GrabDist, GrabDist) / (Vec - _leftThighBodyAvePosition);
						_rightThighBody1.Body.ApplyForce(vector5 / num4);
						Vec = new Vector2(0f, 0f);
					}
				}
			}
			num2 = 10000f;
			KeyboardState state = Keyboard.GetState();
			if (state.IsKeyDown(Keys.D))
			{
				DirectionRight = true;
				DirectionLeft = false;
				_leftThighJoint1.MotorSpeed = RunRotLeft;
				_rightThighJoint1.MotorSpeed = RunRotRight;
				_leftThighJoint2.MotorSpeed = RunRotLeft;
				_rightThighJoint2.MotorSpeed = RunRotRight;
				if (LeftFootIsOnGround && RightFootIsOnGround)
				{
					Vector2 force4 = (_leftThighBodyAvePosition - _rightThighBodyAvePosition) / num2;
					_bodyBody.Body.ApplyForce(force4);
				}
			}
			else if (state.IsKeyDown(Keys.A))
			{
				DirectionRight = false;
				DirectionLeft = true;
				_leftThighJoint1.MotorSpeed = 0f - RunRotLeft;
				_rightThighJoint1.MotorSpeed = 0f - RunRotRight;
				_leftThighJoint2.MotorSpeed = 0f - RunRotLeft;
				_rightThighJoint2.MotorSpeed = 0f - RunRotRight;
				if (RightFootIsOnGround && LeftFootIsOnGround)
				{
					Vector2 force5 = (_leftThighBodyAvePosition - _rightThighBodyAvePosition) / num2;
					_bodyBody.Body.ApplyForce(force5);
				}
			}
		}
		else if (Dead)
		{
			EnemyHPBody = 0f;
		}
	}

	public void UpdateBat(GameTime gameTime, World _world)
	{
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBallBulletTimer[i] > 5.0)
			{
				_CannonBallBulletTimer[i]++;
			}
			if (_CannonBallBulletTimer[i] > 750.0)
			{
				_CannonBallBulletTimer[i] = 0.0;
				_world.RemoveBody(_CannonBall[i].Body);
				_CannonBall[i] = null;
			}
		}
		for (int j = 0; j < IceBallIndex; j++)
		{
			if (_IceBallBulletTimer[j] > 5.0)
			{
				_IceBallBulletTimer[j]++;
			}
			if (_IceBallBulletTimer[j] > 250.0)
			{
				_IceBallBulletTimer[j] = 0.0;
				_world.RemoveBody(_IceBall[j].Body);
				_IceBall[j] = null;
			}
		}
		for (int k = 0; k < DartBoneIndex; k++)
		{
			if (_DartBoneBulletTimer[k] > 5.0)
			{
				_DartBoneBulletTimer[k]++;
			}
			if (_DartBoneBulletTimer[k] > 750.0)
			{
				_DartBoneBulletTimer[k] = 0.0;
				if (_DartBone[k].Body != null)
				{
					_world.RemoveBody(_DartBone[k].Body);
					_DartBone[k] = null;
				}
			}
		}
		if (KineticGo)
		{
			_DartKineticZone.Body.Position = DartKineticDart.Body.Position;
			foreach (Body body in _world.BodyList)
			{
				if ((int)body.UserData != 13)
				{
					Vector2 point = body.Position;
					float num = 1E+11f;
					if (_DartKineticZone.TestPoint(ref point))
					{
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
					}
				}
			}
			if (DartKineticDart.Body != null)
			{
				_world.RemoveBody(DartKineticDart.Body);
				DartKineticDart = null;
			}
			DartKineticDart = null;
			KineticGo = false;
			KineticDraw = false;
		}
		else
		{
			_DartKineticZone.Body.Position = new Vector2(0f, 0f);
		}
		BodyPartsManager4(gameTime);
		if (Dead)
		{
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectKineticShield.Update(deltaSeconds);
			particleEffectKineticEx.Update(deltaSeconds);
			HealEffect.Update(deltaSeconds);
			EnemyHPBody = 0f;
			LowerShields(_world);
		}
		if (Alive & !Unconscious)
		{
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			FireEffectLeft.Update(deltaSeconds2);
			FireEffectRight.Update(deltaSeconds2);
			FreezeEffectLeft.Update(deltaSeconds2);
			FreezeEffectRight.Update(deltaSeconds2);
			HealEffect.Update(deltaSeconds2);
			particleEffectKineticShield.Update(deltaSeconds2);
			particleEffectKineticEx.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectBleed.Update(deltaSeconds2);
			ParticleBoundingBox.LowerBound = new Vector2(level.cameraPosition, level.cameraHeightPosition);
			ParticleBoundingBox.UpperBound = new Vector2(level.cameraPosition + level.mainGame.BackBufferWidth, level.cameraHeightPosition + level.mainGame.BackBufferHeight);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 1.0)
			{
				EnemyMana += ManaGainRate;
				if (EnemyMana > ManaMax)
				{
					EnemyMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			Update_Injuries(gameTime);
			UpdateFreeze(gameTime);
			if (Frozen)
			{
				Particle_Freeze(gameTime);
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
	}

	public void UpdateWereLimer(GameTime gameTime, World _world)
	{
		_kineticShields.Body.Position = _bodyBody.Body.Position;
		for (int i = 0; i < CannonBallIndex; i++)
		{
			if (_CannonBallBulletTimer[i] > 5.0)
			{
				_CannonBallBulletTimer[i]++;
			}
			if (_CannonBallBulletTimer[i] > 750.0)
			{
				_CannonBallBulletTimer[i] = 0.0;
				_world.RemoveBody(_CannonBall[i].Body);
				_CannonBall[i] = null;
			}
		}
		for (int j = 0; j < IceBallIndex; j++)
		{
			if (_IceBallBulletTimer[j] > 5.0)
			{
				_IceBallBulletTimer[j]++;
			}
			if (_IceBallBulletTimer[j] > 250.0)
			{
				_IceBallBulletTimer[j] = 0.0;
				_world.RemoveBody(_IceBall[j].Body);
				_IceBall[j] = null;
			}
		}
		for (int k = 0; k < DartBoneIndex; k++)
		{
			if (_DartBoneBulletTimer[k] > 5.0)
			{
				_DartBoneBulletTimer[k]++;
			}
			if (_DartBoneBulletTimer[k] > 750.0)
			{
				_DartBoneBulletTimer[k] = 0.0;
				if (_DartBone[k].Body != null)
				{
					_world.RemoveBody(_DartBone[k].Body);
					_DartBone[k] = null;
				}
			}
		}
		if (KineticGo)
		{
			_DartKineticZone.Body.Position = DartKineticDart.Body.Position;
			foreach (Body body in _world.BodyList)
			{
				if ((int)body.UserData != 13)
				{
					Vector2 point = body.Position;
					float num = 1E+11f;
					if (_DartKineticZone.TestPoint(ref point))
					{
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
						body.ApplyForce(new Vector2(body.Position.X - DartKineticDart.Body.Position.X, body.Position.Y - DartKineticDart.Body.Position.Y) * new Vector2(num, num));
					}
				}
			}
			if (DartKineticDart.Body != null)
			{
				_world.RemoveBody(DartKineticDart.Body);
				DartKineticDart = null;
			}
			DartKineticDart = null;
			KineticGo = false;
			KineticDraw = false;
		}
		else
		{
			_DartKineticZone.Body.Position = new Vector2(0f, 0f);
		}
		BodyPartsManager4(gameTime);
		if (Dead)
		{
			float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
			particleEffectBleeding.Update(deltaSeconds);
			particleEffectBloodSquirting.Update(deltaSeconds);
			particleEffectKineticShield.Update(deltaSeconds);
			particleEffectKineticEx.Update(deltaSeconds);
			HealEffect.Update(deltaSeconds);
			EnemyHPBody = 0f;
			LowerShields(_world);
		}
		if (Alive & !Unconscious)
		{
			_bodyBody.Body.GetLinearVelocityFromLocalPoint(new Vector2(bodyLinearVelocity_X, bodyLinearVelocity_Y));
			_bodyBodyPosition = _bodyBody.Body.Position;
			float deltaSeconds2 = (float)gameTime.ElapsedGameTime.TotalSeconds;
			FireEffectLeft.Update(deltaSeconds2);
			FireEffectRight.Update(deltaSeconds2);
			FreezeEffectLeft.Update(deltaSeconds2);
			FreezeEffectRight.Update(deltaSeconds2);
			HealEffect.Update(deltaSeconds2);
			particleEffectKineticShield.Update(deltaSeconds2);
			particleEffectKineticEx.Update(deltaSeconds2);
			particleEffectUnconcious.Update(deltaSeconds2);
			particleEffectBleed.Update(deltaSeconds2);
			ParticleBoundingBox.LowerBound = new Vector2(level.cameraPosition, level.cameraHeightPosition);
			ParticleBoundingBox.UpperBound = new Vector2(level.cameraPosition + level.mainGame.BackBufferWidth, level.cameraHeightPosition + level.mainGame.BackBufferHeight);
			if (gameTime.TotalGameTime.TotalSeconds - ManaTime > 1.0)
			{
				EnemyMana += ManaGainRate;
				if (EnemyMana > ManaMax)
				{
					EnemyMana = ManaMax;
				}
				ManaTime = gameTime.TotalGameTime.TotalSeconds;
			}
			Update_Injuries(gameTime);
			UpdateFreeze(gameTime);
			if (Frozen)
			{
				Particle_Freeze(gameTime);
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
	}

	public void Update_Injuries_Rat(GameTime gameTime, World _world)
	{
		if (NeckJoint != null)
		{
			if (_leftThighJoint1.JointSpeed > MaxJointForce * 3f)
			{
				EnemyHPBody -= (_leftThighJoint1.JointSpeed - MaxJointForce) / 10f;
			}
			if (_leftThighJoint2.JointSpeed > MaxJointForce * 3f)
			{
				EnemyHPBody -= (_leftThighJoint2.JointSpeed - MaxJointForce) / 10f;
			}
			if (_rightThighJoint1.JointSpeed > MaxJointForce * 3f)
			{
				EnemyHPBody -= (_rightThighJoint1.JointSpeed - MaxJointForce) / 10f;
			}
			if (_rightThighJoint2.JointSpeed > MaxJointForce * 3f)
			{
				EnemyHPBody -= (_rightThighJoint2.JointSpeed - MaxJointForce) / 10f;
			}
		}
		if (EnemyHPBody < 4f)
		{
			Dead = true;
			EnemyHPBody = 0f;
			_bodyBrush = content.Load<Texture2D>("Sprites/Enemy/0/body_Dead");
			Alive = false;
			if (_leftThighBody1 != null && _leftThighBody1.Body != null)
			{
				_world.RemoveBody(_leftThighBody1.Body);
			}
			if (_leftThighBody2 != null && _leftThighBody2.Body != null)
			{
				_world.RemoveBody(_leftThighBody2.Body);
			}
			if (_rightThighBody1 != null && _rightThighBody1.Body != null)
			{
				_world.RemoveBody(_rightThighBody1.Body);
			}
			if (_rightThighBody2 != null && _rightThighBody2.Body != null)
			{
				_world.RemoveBody(_rightThighBody2.Body);
			}
			if (_headBody != null && _headBody.Body != null)
			{
				_world.RemoveBody(_headBody.Body);
			}
			_bodyBody.CollisionGroup = 99;
			_bodyBody.Body.UserData = 1;
			_bodyBody.Body.AngularDamping = 0f;
		}
	}

	public void Update_Injuries(GameTime gameTime)
	{
		if (EnemyHPBody < 4f)
		{
			Dead = true;
			EnemyHPBody = 0f;
		}
	}

	private void RaySight(World _world)
	{
	}

	private Vector2 RayGrab(World _world, Fixture First, Fixture Last)
	{
		_world.RayCast(delegate(Fixture f, Vector2 p, Vector2 n, float fr)
		{
			_ = f.Body;
			if (f != null)
			{
				if ((int)f.Body.UserData == 1 || (int)f.Body.UserData == 8 || (int)f.Body.UserData == 9)
				{
					Grab = p;
				}
				else
				{
					Grab = new Vector2(0f, 0f);
				}
			}
			else
			{
				Grab = new Vector2(0f, 0f);
			}
			return 0f;
		}, First.Body.Position, (First.Body.Position - Last.Body.Position) * new Vector2(GrabDist, GrabDist));
		return Grab;
	}

	public void RaiseShields(World _world)
	{
		if (EnemyMana > KineticShieldManaCost)
		{
			EnemyMana -= KineticShieldManaCost;
			_kineticShields.Body.Active = true;
			particleEffectKineticShield[0].TriggerOffset = _bodyBody.Body.Position * new Vector2(PhysicsScaleUp, PhysicsScaleUp) - new Vector2(level.cameraPosition, level.cameraHeightPosition);
			particleEffectKineticShield.Trigger(new Vector2(0f, 0f));
			_kineticShields.CollidesWith = CollisionCategory.All;
			_kineticShields.CollisionCategories = CollisionCategory.All;
			LeftHandForce = new Vector2(0f, 1f);
			ForceScalerLeft = new Vector2(1f, 1f);
			LeftHandForce = -LeftHandForce * ForceScalerLeft;
			_leftHandBody.Body.ApplyForce(new Vector2(LeftHandForce.X, LeftHandForce.Y));
			_bodyBody.Body.ApplyForce(new Vector2(LeftHandForce.X - LeftHandForce.X * 2f, LeftHandForce.Y - LeftHandForce.Y * 2f));
		}
	}

	public void LowerShields(World _world)
	{
		_kineticShields.Body.Active = false;
		_kineticShields.CollidesWith = CollisionCategory.None;
		_kineticShields.CollisionCategories = CollisionCategory.None;
	}

	public void CreateCannonBallLeft(World _world)
	{
		if (EnemyMana > CannonBallManaCost)
		{
			EnemyMana -= CannonBallManaCost;
			_CannonBallBulletTimer[CannonBallIndex] = 10.0;
			_CannonBall[CannonBallIndex] = FixtureFactory.CreateEllipse(_world, 1f, 1f, 10, 1000000f);
			_CannonBall[CannonBallIndex].Body.Position = _leftHandBody.Body.Position;
			_CannonBall[CannonBallIndex].Body.BodyType = BodyType.Dynamic;
			_CannonBall[CannonBallIndex].Body.SleepingAllowed = true;
			_CannonBall[CannonBallIndex].Body.IsBullet = true;
			_CannonBall[CannonBallIndex].Density = 20000f;
			_CannonBall[CannonBallIndex].Friction = 0f;
			_CannonBall[CannonBallIndex].Restitution = 1f;
			_CannonBall[CannonBallIndex].Body.UserData = 120;
			_CannonBall[CannonBallIndex].Body.LinearDamping = 0f;
			_CannonBall[CannonBallIndex].CollisionGroup = CollisionGroup;
			_CannonBall[CannonBallIndex].CollisionCategories = CollisionCategory.Cat27;
			_CannonBallTexture = content.Load<Texture2D>("Magic/Cannon/CannonBall");
			_CannonBallOrigin = new Vector2(_CannonBallTexture.Width / 2, _CannonBallTexture.Height / 2);
			_CannonBall[CannonBallIndex].Body.ApplyTorque(10f);
			_CannonBall[CannonBallIndex].Body.ApplyForce(new Vector2(LeftHandForce.X, LeftHandForce.Y) * new Vector2(CannonBallForceScaler, CannonBallForceScaler));
			CannonBallIndex++;
		}
	}

	public void CreateCannonBallRight(World _world)
	{
		if (EnemyMana > CannonBallManaCost)
		{
			EnemyMana -= CannonBallManaCost;
			_CannonBallBulletTimer[CannonBallIndex] = 10.0;
			_CannonBall[CannonBallIndex] = FixtureFactory.CreateEllipse(_world, 1f, 1f, 10, 1000000f);
			_CannonBall[CannonBallIndex].Body.Position = _rightHandBody.Body.Position;
			_CannonBall[CannonBallIndex].Body.BodyType = BodyType.Dynamic;
			_CannonBall[CannonBallIndex].Body.IsBullet = true;
			_CannonBall[CannonBallIndex].Body.SleepingAllowed = true;
			_CannonBall[CannonBallIndex].Density = 20000f;
			_CannonBall[CannonBallIndex].Friction = 1f;
			_CannonBall[CannonBallIndex].Body.UserData = 120;
			_CannonBall[CannonBallIndex].Body.LinearDamping = 0f;
			_CannonBall[CannonBallIndex].CollisionGroup = CollisionGroup;
			_CannonBall[CannonBallIndex].CollisionCategories = CollisionCategory.Cat27;
			_CannonBallTexture = content.Load<Texture2D>("Magic/Cannon/CannonBall");
			_CannonBallOrigin = new Vector2(_CannonBallTexture.Width / 2, _CannonBallTexture.Height / 2);
			_CannonBall[CannonBallIndex].Body.ApplyTorque(-10f);
			_CannonBall[CannonBallIndex].Body.ApplyForce(new Vector2(RightHandForce.X, RightHandForce.Y) * new Vector2(CannonBallForceScaler, CannonBallForceScaler));
			CannonBallIndex++;
		}
	}

	public void CreateIceBallLeft(World _world)
	{
		if (EnemyMana > IceBallManaCost)
		{
			EnemyMana -= IceBallManaCost;
			_IceBallBulletTimer[IceBallIndex] = 10.0;
			_IceBall[IceBallIndex] = FixtureFactory.CreateEllipse(_world, 1f, 1f, 10, 1E-06f);
			_IceBall[IceBallIndex].Body.Position = _leftHandBody.Body.Position;
			_IceBall[IceBallIndex].Body.BodyType = BodyType.Dynamic;
			_IceBall[IceBallIndex].Body.SleepingAllowed = true;
			_IceBall[IceBallIndex].Body.IsBullet = true;
			_IceBall[IceBallIndex].Density = 2E-07f;
			_IceBall[IceBallIndex].Friction = 0f;
			_IceBall[IceBallIndex].Restitution = 0.3f;
			_IceBall[IceBallIndex].Body.UserData = 121;
			_IceBall[IceBallIndex].Body.LinearDamping = 0f;
			_IceBall[IceBallIndex].CollisionGroup = CollisionGroup;
			_IceBall[IceBallIndex].CollisionGroup = CollisionGroup;
			_IceBall[IceBallIndex].CollisionCategories = CollisionCategory.Cat27;
			_IceBallTexture = content.Load<Texture2D>("Magic/Ice/IceBall");
			_IceBallOrigin = new Vector2(_IceBallTexture.Width / 2, _IceBallTexture.Height / 2);
			Fixture obj = _IceBall[IceBallIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(IceBall_OnCollision));
			_IceBall[IceBallIndex].Body.ApplyTorque(10f);
			_IceBall[IceBallIndex].Body.ApplyForce(new Vector2(LeftHandForce.X, LeftHandForce.Y) * new Vector2(IceBallForceScaler, IceBallForceScaler));
			IceBallIndex++;
		}
	}

	public void CreateIceBallRight(World _world)
	{
		if (EnemyMana > IceBallManaCost)
		{
			EnemyMana -= IceBallManaCost;
			_IceBallBulletTimer[IceBallIndex] = 10.0;
			_IceBall[IceBallIndex] = FixtureFactory.CreateEllipse(_world, 1f, 1f, 10, 1E-06f);
			_IceBall[IceBallIndex].Body.Position = _rightHandBody.Body.Position;
			_IceBall[IceBallIndex].Body.BodyType = BodyType.Dynamic;
			_IceBall[IceBallIndex].Body.IsBullet = true;
			_IceBall[IceBallIndex].Body.SleepingAllowed = true;
			_IceBall[IceBallIndex].Density = 2E-07f;
			_IceBall[IceBallIndex].Friction = 0f;
			_IceBall[IceBallIndex].Restitution = 0.3f;
			_IceBall[IceBallIndex].Body.UserData = 121;
			_IceBall[IceBallIndex].Body.LinearDamping = 0f;
			_IceBall[IceBallIndex].CollisionGroup = CollisionGroup;
			_IceBall[IceBallIndex].CollisionCategories = CollisionCategory.Cat27;
			_IceBallTexture = content.Load<Texture2D>("Magic/Ice/IceBall");
			_IceBallOrigin = new Vector2(_IceBallTexture.Width / 2, _IceBallTexture.Height / 2);
			Fixture obj = _IceBall[IceBallIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(IceBall_OnCollision));
			_IceBall[IceBallIndex].Body.ApplyTorque(-10f);
			_IceBall[IceBallIndex].Body.ApplyForce(new Vector2(RightHandForce.X, RightHandForce.Y) * new Vector2(IceBallForceScaler, IceBallForceScaler));
			IceBallIndex++;
		}
	}

	public void CreateDartBoneLeft(World _world)
	{
		if (EnemyMana > DartBoneManaCost)
		{
			EnemyMana -= DartBoneManaCost;
			_DartBoneBulletTimer[DartBoneIndex] = 10.0;
			_DartBone[DartBoneIndex] = FixtureFactory.CreateRectangle(_world, 0.001f, 0.1f, 1E-09f);
			_DartBone[DartBoneIndex].Body.Position = _leftHandBody.Body.Position;
			_DartBone[DartBoneIndex].Body.BodyType = BodyType.Dynamic;
			_DartBone[DartBoneIndex].Body.SleepingAllowed = true;
			_DartBone[DartBoneIndex].Body.IsBullet = true;
			_DartBone[DartBoneIndex].Body.IgnoreGravity = true;
			_DartBone[DartBoneIndex].Density = 2E-11f;
			_DartBone[DartBoneIndex].Friction = 1f;
			_DartBone[DartBoneIndex].Body.Mass = 0.001f;
			_DartBone[DartBoneIndex].Restitution = 0f;
			_DartBone[DartBoneIndex].Body.UserData = 122;
			_DartBone[DartBoneIndex].Body.LinearDamping = 0f;
			_DartBone[DartBoneIndex].CollisionGroup = CollisionGroup;
			_DartBoneTexture = content.Load<Texture2D>("Darts/DartBone");
			_DartBoneOrigin = new Vector2(_DartBoneTexture.Width / 2, _DartBoneTexture.Height / 2);
			Fixture obj = _DartBone[DartBoneIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartBone_OnCollision));
			_DartBone[DartBoneIndex].Body.Rotation = _leftUpperArmBody.Body.Rotation;
			_DartBone[DartBoneIndex].Body.ApplyForce(new Vector2(_leftHandBody.Body.Position.X - _leftUpperArmBody.Body.Position.X, _leftHandBody.Body.Position.Y - _leftUpperArmBody.Body.Position.Y) * new Vector2(DartBoneForceScaler, DartBoneForceScaler));
			DartBoneIndex++;
		}
	}

	public void CreateDartBoneRight(World _world)
	{
		if (EnemyMana > DartBoneManaCost)
		{
			EnemyMana -= DartBoneManaCost;
			_DartBoneBulletTimer[DartBoneIndex] = 10.0;
			_DartBone[DartBoneIndex] = FixtureFactory.CreateRectangle(_world, 0.001f, 0.1f, 1E-09f);
			_DartBone[DartBoneIndex].Body.Position = _rightHandBody.Body.Position;
			_DartBone[DartBoneIndex].Body.BodyType = BodyType.Dynamic;
			_DartBone[DartBoneIndex].Body.IsBullet = true;
			_DartBone[DartBoneIndex].Body.IgnoreGravity = true;
			_DartBone[DartBoneIndex].Body.SleepingAllowed = true;
			_DartBone[DartBoneIndex].Density = 2E-11f;
			_DartBone[DartBoneIndex].Friction = 1f;
			_DartBone[DartBoneIndex].Body.Mass = 0.001f;
			_DartBone[DartBoneIndex].Restitution = 0f;
			_DartBone[DartBoneIndex].Body.UserData = 122;
			_DartBone[DartBoneIndex].Body.LinearDamping = 0f;
			_DartBone[DartBoneIndex].CollisionGroup = CollisionGroup;
			_DartBoneTexture = content.Load<Texture2D>("Darts/DartBone");
			_DartBoneOrigin = new Vector2(_DartBoneTexture.Width / 2, _DartBoneTexture.Height / 2);
			Fixture obj = _DartBone[DartBoneIndex];
			obj.OnCollision = (CollisionEventHandler)Delegate.Combine(obj.OnCollision, new CollisionEventHandler(DartBone_OnCollision));
			_DartBone[DartBoneIndex].Body.Rotation = _rightUpperArmBody.Body.Rotation;
			_DartBone[DartBoneIndex].Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(DartBoneForceScaler, DartBoneForceScaler));
			DartBoneIndex++;
		}
	}

	public void CreateDartKineticLeft(World _world)
	{
		if (EnemyMana > DartKineticManaCost)
		{
			EnemyMana -= DartKineticManaCost;
			KineticDraw = true;
			_DartKinetic = FixtureFactory.CreateRectangle(_world, 0.001f, 0.1f, 1E-07f);
			_DartKinetic.Body.Position = _leftHandBody.Body.Position;
			_DartKinetic.Body.BodyType = BodyType.Dynamic;
			_DartKinetic.Body.SleepingAllowed = true;
			_DartKinetic.Body.IsBullet = true;
			_DartKinetic.Body.IgnoreGravity = true;
			_DartKinetic.Density = 2E-09f;
			_DartKinetic.Friction = 1f;
			_DartKinetic.Restitution = 0f;
			_DartKinetic.Body.UserData = 199;
			_DartKinetic.Body.LinearDamping = 0f;
			_DartKinetic.CollisionGroup = CollisionGroup;
			_DartKineticTexture = content.Load<Texture2D>("Darts/DartKinetic");
			_DartKineticOrigin = new Vector2(_DartKineticTexture.Width / 2, _DartKineticTexture.Height / 2);
			Fixture dartKinetic = _DartKinetic;
			dartKinetic.OnCollision = (CollisionEventHandler)Delegate.Combine(dartKinetic.OnCollision, new CollisionEventHandler(DartKinetic_OnCollision));
			_DartKinetic.Body.Rotation = _leftUpperArmBody.Body.Rotation;
			_DartKinetic.Body.ApplyForce(new Vector2(_leftHandBody.Body.Position.X - _leftUpperArmBody.Body.Position.X, _leftHandBody.Body.Position.Y - _leftUpperArmBody.Body.Position.Y) * new Vector2(DartKineticForceScaler, DartKineticForceScaler));
			DartKineticIndex++;
		}
	}

	public void CreateDartKineticRight(World _world)
	{
		if (EnemyMana > DartKineticManaCost)
		{
			EnemyMana -= DartKineticManaCost;
			KineticDraw = true;
			_DartKinetic = FixtureFactory.CreateRectangle(_world, 0.001f, 0.1f, 1E-07f);
			_DartKinetic.Body.Position = _rightHandBody.Body.Position;
			_DartKinetic.Body.BodyType = BodyType.Dynamic;
			_DartKinetic.Body.IsBullet = true;
			_DartKinetic.Body.IgnoreGravity = true;
			_DartKinetic.Body.SleepingAllowed = true;
			_DartKinetic.Density = 2E-09f;
			_DartKinetic.Friction = 1f;
			_DartKinetic.Restitution = 0f;
			_DartKinetic.Body.UserData = 199;
			_DartKinetic.Body.LinearDamping = 0f;
			_DartKinetic.CollisionGroup = CollisionGroup;
			_DartKineticTexture = content.Load<Texture2D>("Darts/DartKinetic");
			_DartKineticOrigin = new Vector2(_DartKineticTexture.Width / 2, _DartKineticTexture.Height / 2);
			Fixture dartKinetic = _DartKinetic;
			dartKinetic.OnCollision = (CollisionEventHandler)Delegate.Combine(dartKinetic.OnCollision, new CollisionEventHandler(DartKinetic_OnCollision));
			_DartKinetic.Body.Rotation = _rightUpperArmBody.Body.Rotation;
			_DartKinetic.Body.ApplyForce(new Vector2(_rightHandBody.Body.Position.X - _rightUpperArmBody.Body.Position.X, _rightHandBody.Body.Position.Y - _rightUpperArmBody.Body.Position.Y) * new Vector2(DartKineticForceScaler, DartKineticForceScaler));
			DartKineticIndex++;
		}
	}

	public void Draw(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game)
	{
		if (Active)
		{
			switch (EnemyType)
			{
			case "0":
				DrawRat(gameTime, spriteBatch, game);
				break;
			case "1":
				DrawBat(gameTime, spriteBatch, game);
				break;
			case "2":
				DrawWereLimer(gameTime, spriteBatch, game);
				break;
			default:
				DrawRat(gameTime, spriteBatch, game);
				break;
			}
		}
	}

	public void DrawRat(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game)
	{
		Color = Color.White;
		if (Dead)
		{
			if (!_bodyBodyGone)
			{
				spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			}
		}
		else
		{
			if (!Alive || !Active)
			{
				return;
			}
			if (DirectionLeft)
			{
				if (HFlip)
				{
					if (!_rightThighBodyGone)
					{
						spriteBatch.Draw(_rightThighBrush, _rightThighBodyAvePosition * PhysicsScaleUp, null, Color, _rightThighBody1.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipVertically, 1f);
					}
					if (!_leftThighBodyGone)
					{
						spriteBatch.Draw(_leftThighBrush, _leftThighBodyAvePosition * PhysicsScaleUp, null, Color, _leftThighBody1.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipVertically, 1f);
					}
					if (!_bodyBodyGone)
					{
						spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipVertically, 1f);
					}
				}
				else
				{
					if (!_rightThighBodyGone)
					{
						spriteBatch.Draw(_rightThighBrush, _rightThighBodyAvePosition * PhysicsScaleUp, null, Color, _rightThighBody1.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					}
					if (!_leftThighBodyGone)
					{
						spriteBatch.Draw(_leftThighBrush, _leftThighBodyAvePosition * PhysicsScaleUp, null, Color, _leftThighBody1.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					}
					if (!_bodyBodyGone)
					{
						spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					}
				}
			}
			else
			{
				if (!DirectionRight)
				{
					return;
				}
				if (HFlip)
				{
					if (!_rightThighBodyGone)
					{
						spriteBatch.Draw(_rightThighBrush, _rightThighBodyAvePosition * PhysicsScaleUp, null, Color, _rightThighBody1.Body.Rotation + (float)Math.PI, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					}
					if (!_leftThighBodyGone)
					{
						spriteBatch.Draw(_leftThighBrush, _leftThighBodyAvePosition * PhysicsScaleUp, null, Color, _leftThighBody1.Body.Rotation + (float)Math.PI, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
					}
					if (!_bodyBodyGone)
					{
						spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation + (float)Math.PI, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
					}
				}
				else
				{
					if (!_rightThighBodyGone)
					{
						spriteBatch.Draw(_rightThighBrush, _rightThighBodyAvePosition * PhysicsScaleUp, null, Color, _rightThighBody1.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					}
					if (!_leftThighBodyGone)
					{
						spriteBatch.Draw(_leftThighBrush, _leftThighBodyAvePosition * PhysicsScaleUp, null, Color, _leftThighBody1.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
					}
					if (!_bodyBodyGone)
					{
						spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipHorizontally, 1f);
					}
				}
			}
		}
	}

	public void DrawBat(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game)
	{
		if (Dead)
		{
			if (!_rightThighBodyGone)
			{
				spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			}
			if (!_rightUpperArmBodyGone)
			{
				spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			}
			if (!_rightHandBodyGone)
			{
				spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightHandBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			}
			if (!_bodyBodyGone)
			{
				spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			}
			if (_headBody.Body != null)
			{
				spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
			}
			if (!_leftThighBodyGone)
			{
				spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			}
			if (!_leftUpperArmBodyGone)
			{
				spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			}
			if (!_leftHandBodyGone)
			{
				spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftHandBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			}
		}
		if (!Active)
		{
			return;
		}
		for (int i = 0; i < DartBoneIndex; i++)
		{
			if (_DartBone[i] != null && _DartBone[i].Body != null && _DartBoneBulletTimer[i] > 5.0)
			{
				spriteBatch.Draw(_DartBoneTexture, _DartBone[i].Body.Position * PhysicsScaleUp, null, DartBoneColor, _DartBone[i].Body.Rotation, _DartBoneOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
			}
		}
		if (KineticDraw && _DartKinetic != null && _DartKinetic.Body != null)
		{
			spriteBatch.Draw(_DartKineticTexture, _DartKinetic.Body.Position * PhysicsScaleUp, null, DartKineticColor, _DartKinetic.Body.Rotation, _DartKineticOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
		}
		if (DirectionRight)
		{
			if (!_rightHandBodyGone && _SightON)
			{
				spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
			}
			spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
		}
		if (DirectionLeft)
		{
			spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.FlipHorizontally, 1f);
			if (!_rightHandBodyGone && _SightON)
			{
				spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
			}
			spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
		}
		if (!DirectionLeft && !DirectionRight)
		{
			if (!_rightHandBodyGone && _SightON)
			{
				spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
			}
			spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
		}
		for (int j = 0; j < CannonBallIndex; j++)
		{
			if (_CannonBall[j] != null && _CannonBall[j].Body != null && _CannonBallBulletTimer[j] > 5.0)
			{
				spriteBatch.Draw(_CannonBallTexture, _CannonBall[j].Body.Position * PhysicsScaleUp, null, CannonBallColor, _CannonBall[j].Body.Rotation, _CannonBallOrigin, 0.1f, SpriteEffects.None, 1f);
			}
		}
	}

	public void DrawWereLimer(GameTime gameTime, SpriteBatch spriteBatch, PlatformerGame game)
	{
		if (Dead)
		{
			if (!_rightThighBodyGone)
			{
				spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			}
			if (!_rightUpperArmBodyGone)
			{
				spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			}
			if (!_rightHandBodyGone)
			{
				spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightHandBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			}
			if (!_bodyBodyGone)
			{
				spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			}
			if (_headBody.Body != null)
			{
				spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
			}
			if (!_leftThighBodyGone)
			{
				spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			}
			if (!_leftUpperArmBodyGone)
			{
				spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			}
			if (!_leftHandBodyGone)
			{
				spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftHandBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			}
		}
		if (!Active)
		{
			return;
		}
		for (int i = 0; i < DartBoneIndex; i++)
		{
			if (_DartBone[i] != null && _DartBone[i].Body != null && _DartBoneBulletTimer[i] > 5.0)
			{
				spriteBatch.Draw(_DartBoneTexture, _DartBone[i].Body.Position * PhysicsScaleUp, null, DartBoneColor, _DartBone[i].Body.Rotation, _DartBoneOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
			}
		}
		if (KineticDraw && _DartKinetic != null && _DartKinetic.Body != null)
		{
			spriteBatch.Draw(_DartKineticTexture, _DartKinetic.Body.Position * PhysicsScaleUp, null, DartKineticColor, _DartKinetic.Body.Rotation, _DartKineticOrigin, 0.5f, SpriteEffects.FlipVertically, 1f);
		}
		if (DirectionRight)
		{
			if (!_rightHandBodyGone && _SightON)
			{
				spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
			}
			spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
		}
		if (DirectionLeft)
		{
			spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.FlipHorizontally, 1f);
			if (!_rightHandBodyGone && _SightON)
			{
				spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
			}
			spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.FlipHorizontally, 1f);
			spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.FlipHorizontally, 1f);
		}
		if (!DirectionLeft && !DirectionRight)
		{
			if (!_rightHandBodyGone && _SightON)
			{
				spriteBatch.Draw(_SightBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, new Vector2(_SightBrush.Width / 2, 0f), Scaler * 2f, SpriteEffects.FlipVertically, 1f);
			}
			spriteBatch.Draw(_rightThighBrush, _rightThighBody.Body.Position * PhysicsScaleUp, null, Color, _rightThighBody.Body.Rotation, _rightThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightUpperArmBrush, _rightUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_rightHandBrush, _rightHandBody.Body.Position * PhysicsScaleUp, null, Color, _rightUpperArmBody.Body.Rotation, _rightHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_bodyBrush, _bodyBody.Body.Position * PhysicsScaleUp, null, Color, _bodyBody.Body.Rotation, _bodyBrushOrigin, Scaler * 1.25f, SpriteEffects.None, 1f);
			spriteBatch.Draw(_headBrush, _headBody.Body.Position * PhysicsScaleUp, null, Color, _headBody.Body.Rotation, _headBrushOrigin, Scaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftThighBrush, _leftThighBody.Body.Position * PhysicsScaleUp, null, Color, _leftThighBody.Body.Rotation, _leftThighBrushOrigin, Scaler * LegScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftUpperArmBrush, _leftUpperArmBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftUpperArmBrushOrigin, Scaler * ArmScaler, SpriteEffects.None, 1f);
			spriteBatch.Draw(_leftHandBrush, _leftHandBody.Body.Position * PhysicsScaleUp, null, Color, _leftUpperArmBody.Body.Rotation, _leftHandBrushOrigin, Scaler / 1.5f, SpriteEffects.None, 1f);
		}
		for (int j = 0; j < CannonBallIndex; j++)
		{
			if (_CannonBall[j] != null && _CannonBall[j].Body != null && _CannonBallBulletTimer[j] > 5.0)
			{
				spriteBatch.Draw(_CannonBallTexture, _CannonBall[j].Body.Position * PhysicsScaleUp, null, CannonBallColor, _CannonBall[j].Body.Rotation, _CannonBallOrigin, 0.1f, SpriteEffects.None, 1f);
			}
		}
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

	public void DrawParticles(Vector2 cameraTransform)
	{
		if (!Dead)
		{
			for (int i = 0; i < FireEffectLeft.Count; i++)
			{
				for (int j = 0; j < FireEffectLeft[i].Particles.Length; j++)
				{
					if (!(FireEffectLeft[i].Particles[j].Position == PositionOld - cameraTransform))
					{
						FireEffectLeft[i].Particles[j].Position = FireEffectLeft[i].Particles[j].Position - (cameraTransform - cameraTransformOld);
					}
				}
			}
			for (int k = 0; k < FireEffectRight.Count; k++)
			{
				for (int l = 0; l < FireEffectRight[k].Particles.Length; l++)
				{
					FireEffectRight[k].Particles[l].Position = FireEffectRight[k].Particles[l].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int m = 0; m < FreezeEffectLeft.Count; m++)
			{
				for (int n = 0; n < FreezeEffectLeft[m].Particles.Length; n++)
				{
					FreezeEffectLeft[m].Particles[n].Position = FreezeEffectLeft[m].Particles[n].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num = 0; num < FreezeEffectRight.Count; num++)
			{
				for (int num2 = 0; num2 < FreezeEffectRight[num].Particles.Length; num2++)
				{
					FreezeEffectRight[num].Particles[num2].Position = FreezeEffectRight[num].Particles[num2].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num3 = 0; num3 < HealEffect.Count; num3++)
			{
				for (int num4 = 0; num4 < HealEffect[num3].Particles.Length; num4++)
				{
					HealEffect[num3].Particles[num4].Position = HealEffect[num3].Particles[num4].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num5 = 0; num5 < particleEffectKineticShield.Count; num5++)
			{
				for (int num6 = 0; num6 < particleEffectKineticShield[num5].Particles.Length; num6++)
				{
					particleEffectKineticShield[num5].Particles[num6].Position = particleEffectKineticShield[num5].Particles[num6].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num7 = 0; num7 < particleEffectKineticEx.Count; num7++)
			{
				for (int num8 = 0; num8 < particleEffectKineticEx[num7].Particles.Length; num8++)
				{
					particleEffectKineticEx[num7].Particles[num8].Position = particleEffectKineticEx[num7].Particles[num8].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num9 = 0; num9 < particleEffectBleed.Count; num9++)
			{
				for (int num10 = 0; num10 < particleEffectBleed[num9].Particles.Length; num10++)
				{
					particleEffectBleed[num9].Particles[num10].Position = particleEffectBleed[num9].Particles[num10].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num11 = 0; num11 < particleEffectBleeding.Count; num11++)
			{
				for (int num12 = 0; num12 < particleEffectBleeding[num11].Particles.Length; num12++)
				{
					particleEffectBleeding[num11].Particles[num12].Position = particleEffectBleeding[num11].Particles[num12].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num13 = 0; num13 < particleEffectBloodSquirting.Count; num13++)
			{
				for (int num14 = 0; num14 < particleEffectBloodSquirting[num13].Particles.Length; num14++)
				{
					particleEffectBloodSquirting[num13].Particles[num14].Position = particleEffectBloodSquirting[num13].Particles[num14].Position - (cameraTransform - cameraTransformOld);
				}
			}
			for (int num15 = 0; num15 < particleEffectUnconcious.Count; num15++)
			{
				for (int num16 = 0; num16 < particleEffectUnconcious[num15].Particles.Length; num16++)
				{
					particleEffectUnconcious[num15].Particles[num16].Position = particleEffectUnconcious[num15].Particles[num16].Position - (cameraTransform - cameraTransformOld);
				}
			}
			cameraTransformOld = cameraTransform;
			if (!Level.Blood)
			{
			}
		}
		else
		{
			_ = Level.Blood;
		}
	}
}
