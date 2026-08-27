using System;
using System.Collections.Generic;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace EGEngine;

public class Animation
{
	private const int ReloadBoneCount = 11;

	private const int KnifeBoneCount = 9;

	private const int MeleeAttackCount = 11;

	private const int JumpMergeCount = 11;

	public PlayerBase Owner;

	private WeaponAnim[] currentAnim = new WeaponAnim[3];

	private int boneCount;

	private int numberAnimations;

	public float[] TransitionInTime = new float[3];

	public float[] TransitionOutTime = new float[3];

	private AnimationPlayer[] animationPlayer = new AnimationPlayer[3];

	public FPSAnimationState[] m_Anims0;

	public static List<FPSAnimationState[]> AnimationsBaseList = null;

	public static FPSAnimationState[] m_AnimsBase = new FPSAnimationState[131]
	{
		new FPSAnimationState("fpshands_idle", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_pistol", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_throwknife", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_m249", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_sword", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_flamethrower", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_baitbomb", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_cam", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_axe", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_sights", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 6f, sighted: true, AnimationType.Sights),
		new FPSAnimationState("fpshands_sights_pistol", AnimFlag.AF_CAN_FIRE, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI * 2f / 9f, sighted: true, AnimationType.Sights),
		new FPSAnimationState("fpshands_sights_m249", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 6f, sighted: true, AnimationType.Sights),
		new FPSAnimationState("fpshands_walk", AnimFlag.AF_CAN_FIRE, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_pistol", AnimFlag.AF_CAN_FIRE, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_throwknife", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_m249", AnimFlag.AF_CAN_FIRE, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_sword", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_flamethrower", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_cam", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_walk_axe", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_run", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Run),
		new FPSAnimationState("fpshands_run_pistol", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Run),
		new FPSAnimationState("fpshands_run_throwknife", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_run_sword", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_run_axe", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_throwknife", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPutaway),
		new FPSAnimationState("fpshands_throwbaitbomb", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPutaway),
		new FPSAnimationState("fpshands_putaway", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPutaway),
		new FPSAnimationState("fpshands_pullout", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPullout),
		new FPSAnimationState("fpshands_AlienSMGReload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_AlienLMGReload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_AlienGrenaderReload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_AlienShottyReload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_AlienSniperReload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_sword_attack00", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Attack),
		new FPSAnimationState("fpshands_sword_attack01", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Attack),
		new FPSAnimationState("fpshands_scarreload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_ak74ureload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_pistolreload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_nadereload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_m249reload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_tac50reload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_rpgreload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_javlinreload", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_shotgunreload", AnimFlag.AF_ONEOFF, bor: false, 0.25f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_shotgunreloadbase", AnimFlag.AF_CLEAR, bor: false, 0.25f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_shotguncock", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_shotguncocksight", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 6f, sighted: false, AnimationType.Sights),
		new FPSAnimationState("fpshands_idle_knife", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_jump", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Jump),
		new FPSAnimationState("fpshands_jump_pistol", (AnimFlag)3, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Jump),
		new FPSAnimationState("fpshands_jump_0", (AnimFlag)3, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Jump),
		new FPSAnimationState("fpshands_jump_2", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Jump),
		new FPSAnimationState("fps_coop_idle", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_empty", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_axe", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_sighted", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_emptysighted", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_emptystrafeleft", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_emptystraferight", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_axestrafeleft", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_idle_axestraferight", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_crouch", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_crouchwalk", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_crouchwalkback", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_crouch_empty", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_crouchwalk_empty", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_crouchwalkback_empty", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_crouch_axe", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_crouchwalk_axe", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_crouchwalkback_axe", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walk", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walkback", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.3f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walk_empty", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.3f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walk_empty", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.3f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walk_axe", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.3f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walk_sighted", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_walk_emptysighted", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.2f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_sidestep_left", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.3f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_sidestep_right", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.3f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("fps_coop_roll_left", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.25f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("fps_coop_roll_right", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.25f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("fps_coop_run", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("fps_coop_run_empty", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("fps_coop_run_axe", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("fps_coop_reload", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fps_coop_knife", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.IdleKnife),
		new FPSAnimationState("fps_coop_swap", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPutaway),
		new FPSAnimationState("fps_coop_jump", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Jump),
		new FPSAnimationState("fps_coop_rightpunch", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_axeswing", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_climbwall", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_climbup", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_death00", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_death00", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_death00", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_death00", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("idle", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("walk", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.35f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("runa", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 1f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("attack", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Attack),
		new FPSAnimationState("attacka", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Attack),
		new FPSAnimationState("search", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.35f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("hit00", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("hit01", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("ZombieKeepWalk", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0.35f, (float)Math.PI / 4f, sighted: false, AnimationType.Walk),
		new FPSAnimationState("ZombieKeepClimb", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("ZombieKeepClimbUp", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("ZombieKeepAttack", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Attack),
		new FPSAnimationState("ZombieKeepDeath00", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("ZombieKeepDeath01", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("ZombieKeepDeath02", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("ZombieKeepDeath03", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("SurvivorChildWalk", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("SurvivorManRun", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("SurvivorWomanRun", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Run),
		new FPSAnimationState("SurvivorChildCower", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("SurvivorManCower", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("SurvivorWomanCower", AnimFlag.AF_CLEAR, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_idle_empty", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_sights_empty", AnimFlag.AF_CAN_FIRE, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Sights),
		new FPSAnimationState("fpshands_walk_empty", AnimFlag.AF_CAN_FIRE, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Walk),
		new FPSAnimationState("fpshands_run_empty", AnimFlag.AF_CLEAR, bor: false, 0.2f, new TimeSpan(0, 0, 0, 0, 100), 0.5f, (float)Math.PI / 4f, sighted: true, AnimationType.Run),
		new FPSAnimationState("fpshands_pullout_empty", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPullout),
		new FPSAnimationState("fpshands_putaway_empty", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.WeaponPutaway),
		new FPSAnimationState("fpshands_reload_empty", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 500), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Reload),
		new FPSAnimationState("fpshands_jump_empty", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0.5f, (float)Math.PI / 4f, sighted: false, AnimationType.Jump),
		new FPSAnimationState("fpshands_rightpunch", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fpshands_axeswing", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("fps_coop_axeswing", AnimFlag.AF_ONEOFF, bor: false, 0.1f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle),
		new FPSAnimationState("zombiea_ragdollpose", AnimFlag.AF_CLEAR, bor: true, 0.2f, new TimeSpan(0, 0, 0, 0, 200), 0f, (float)Math.PI / 4f, sighted: false, AnimationType.Idle)
	};

	private static int MaxScheduledAnimation = 4;

	public int NumScheduledAnimations;

	public FPSAnimationQueue[] ScheduledAnimations = new FPSAnimationQueue[MaxScheduledAnimation];

	private float mergeTransitionInTime;

	private float mergeTransitionOutTime;

	public int mergeBoneCount;

	public int[] mergBoneIndices = new int[64];

	private WeaponAnim mergeAnimation;

	public AnimationPlayer mergeAnimPlayer;

	private static int[] ReloadMergeBones = new int[11]
	{
		9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
		19
	};

	private static int[] KnifeMergeBones = new int[9] { 11, 12, 13, 14, 15, 16, 17, 18, 19 };

	private static int[] MeleeAttackMergeBones = new int[11]
	{
		9, 10, 11, 12, 13, 14, 15, 16, 17, 18,
		19
	};

	private static int[] JumpMergeBones = new int[11]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10
	};

	public AnimationEventArgs animEventArg = new AnimationEventArgs();

	public Matrix[][] SkinTransformBlend = new Matrix[2][];

	public Matrix[][] ModelTransform = new Matrix[2][];

	public Matrix[][] WorldTransformBlend = new Matrix[2][];

	private static bool clipInit = false;

	private static bool animationInit = false;

	private SkinningData skinningData;

	public WeaponAnim CurrentAnimation
	{
		get
		{
			return currentAnim[numberAnimations];
		}
		set
		{
			currentAnim[numberAnimations] = value;
		}
	}

	public FPSAnimationState CurrentAnimationState => m_Anims0[(int)currentAnim[numberAnimations]];

	public TimeSpan ElapsedTime => animationPlayer[numberAnimations].ElapsedTimeStep;

	public AnimFlag CurrentAnimFlags => animationPlayer[numberAnimations].CurrentClip.AnimFlag;

	public event EventHandler<AnimationEventArgs> AnimationBlendIn;

	public event EventHandler<AnimationEventArgs> AnimationBlendOut;

	public event EventHandler<AnimationEventArgs> AnimationEndReached;

	public WeaponAnim CurrentAnimationStackIndex(int i)
	{
		return currentAnim[i];
	}

	public void ReStartCurrentClip()
	{
		animationPlayer[numberAnimations].ReStartCurrentClip();
	}

	public bool IsAnimTypeOnStack(AnimationType e)
	{
		for (int i = 0; i < 3; i++)
		{
			if (animationPlayer[i].CurrentClip != null && animationPlayer[i].CurrentClip.AnimType == e)
			{
				return true;
			}
		}
		return false;
	}

	public Matrix[] GetSkinTransforms(int q)
	{
		return SkinTransformBlend[q];
	}

	public Matrix[] GetBoneTransforms(int q)
	{
		return ModelTransform[q];
	}

	public void GetWorldTransformBlend(int qIndex, int bone, out Matrix m)
	{
		m = WorldTransformBlend[qIndex][bone];
	}

	public bool PlayingOneOff()
	{
		bool result = false;
		for (int i = 0; i <= numberAnimations; i++)
		{
			if (animationPlayer[i].CurrentClip != null && (animationPlayer[i].CurrentClip.AnimFlag & AnimFlag.AF_ONEOFF) > AnimFlag.AF_CLEAR)
			{
				result = true;
			}
		}
		return result;
	}

	public void ApplyUserTransform(int b, ref Matrix t)
	{
		animationPlayer[0].ApplyUserTransform[b].Valid = true;
		animationPlayer[0].ApplyUserTransform[b].Transform = t;
		animationPlayer[1].ApplyUserTransform[b].Valid = true;
		animationPlayer[1].ApplyUserTransform[b].Transform = t;
		animationPlayer[2].ApplyUserTransform[b].Valid = true;
		animationPlayer[2].ApplyUserTransform[b].Transform = t;
	}

	public void ApplyUserTransform(int b0, int b1, ref Matrix t)
	{
		animationPlayer[0].ApplyUserTransform[b0].Valid = true;
		animationPlayer[0].ApplyUserTransform[b0].Transform = t;
		animationPlayer[1].ApplyUserTransform[b0].Valid = true;
		animationPlayer[1].ApplyUserTransform[b0].Transform = t;
		animationPlayer[2].ApplyUserTransform[b0].Valid = true;
		animationPlayer[2].ApplyUserTransform[b0].Transform = t;
		mergeAnimPlayer.ApplyUserTransform[b1].Valid = true;
		mergeAnimPlayer.ApplyUserTransform[b1].Transform = t;
	}

	public void SetBaseAnimation(WeaponAnim anim)
	{
		TransitionInTime[0] = 1f;
		TransitionOutTime[0] = 1f;
		currentAnim[0] = anim;
		animationPlayer[0].StartClip(m_Anims0[(int)anim].Clip);
		animationPlayer[0].AnimStateFlags = (AnimationStateFlags)7;
	}

	public void SetBaseOneAnimation(WeaponAnim anim)
	{
		TransitionInTime[1] = 1f;
		TransitionOutTime[1] = 1f;
		currentAnim[1] = anim;
		animationPlayer[1].StartClip(m_Anims0[(int)anim].Clip);
		animationPlayer[1].AnimStateFlags = (AnimationStateFlags)7;
	}

	public void AddForce(ref Vector3 axis, float force, float timeScale, int bone)
	{
		for (int i = 0; i < numberAnimations; i++)
		{
		}
	}

	public float GetAnimationBlendTime()
	{
		float result = 1f;
		if (numberAnimations == 2)
		{
			result = TransitionInTime[2] / animationPlayer[2].CurrentClip.BlendInTime;
			result = ((result > 1f) ? 1f : result);
		}
		else if (numberAnimations == 1)
		{
			result = TransitionInTime[1] / animationPlayer[1].CurrentClip.BlendInTime;
			result = ((result > 1f) ? 1f : result);
		}
		return result;
	}

	public void UpdateOnlyTransitionTime(TimeSpan elapsedGameTime)
	{
		TransitionInTime[2] += (float)elapsedGameTime.Milliseconds * 0.001f;
		TransitionInTime[1] += (float)elapsedGameTime.Milliseconds * 0.001f;
	}

	public void Update(TimeSpan elapsedGameTime, ref Matrix transform, int qIndex, float blendTime)
	{
		int num = numberAnimations;
		for (int i = 0; i <= num; i++)
		{
			this.animationPlayer[i].Update(elapsedGameTime, relativeToCurrentTime: true, transform);
			m_Anims0[this.animationPlayer[i].CurrentAnimation].UpdateKeyEvents(this.animationPlayer[i].CurrentTime);
		}
		if (num == 0)
		{
			for (int j = 0; j < boneCount; j++)
			{
				ref Matrix reference = ref ModelTransform[qIndex][j];
				reference = this.animationPlayer[0].boneTransforms[j];
			}
		}
		if (num > 0)
		{
			TransitionInTime[1] += (float)elapsedGameTime.Milliseconds * 0.001f;
			float num2 = TransitionInTime[1] / this.animationPlayer[1].CurrentClip.BlendInTime;
			if (this.animationPlayer[1].CurrentClip.BlendOverRide)
			{
				num2 = blendTime;
			}
			if (num2 < 1f)
			{
				for (int k = 0; k < boneCount; k++)
				{
					ref Matrix reference2 = ref ModelTransform[qIndex][k];
					reference2 = this.animationPlayer[0].boneTransforms[k] * (1f - num2);
					ModelTransform[qIndex][k] += this.animationPlayer[1].boneTransforms[k] * num2;
				}
			}
			else
			{
				if (AnimationBlendIn != null && animEventArg.ValidateEvent(this.animationPlayer[1], currentAnim[1], 1, m_Anims0[(int)currentAnim[1]].AnimType, AnimationStateFlags.BlendInExecuted))
				{
					AnimationBlendIn(this, animEventArg);
					AnimationBlendIn = animEventArg.NewHandler;
				}
				if (this.animationPlayer[1].BlendOutTimeReached && (this.animationPlayer[1].CurrentClip.AnimFlag & AnimFlag.AF_ONEOFF) != AnimFlag.AF_CLEAR)
				{
					if (AnimationBlendOut != null && animEventArg.ValidateEvent(this.animationPlayer[1], currentAnim[1], 1, m_Anims0[(int)currentAnim[1]].AnimType, AnimationStateFlags.BlendOutExecuted))
					{
						AnimationBlendOut(this, animEventArg);
						AnimationBlendOut = animEventArg.NewHandler;
					}
					TransitionOutTime[1] += (float)elapsedGameTime.Milliseconds * 0.001f;
					float num3 = (float)this.animationPlayer[1].CurrentTime.Subtract(this.animationPlayer[1].CurrentClip.BlendOutTime).Milliseconds * 0.001f;
					num3 /= this.animationPlayer[1].CurrentClip.fBlendOutTime;
					if (num3 >= 0f && num3 < 1f && !this.animationPlayer[1].EndReached)
					{
						for (int l = 0; l < boneCount; l++)
						{
							ref Matrix reference3 = ref ModelTransform[qIndex][l];
							reference3 = this.animationPlayer[0].boneTransforms[l] * num3;
							ModelTransform[qIndex][l] += this.animationPlayer[1].boneTransforms[l] * (1f - num3);
						}
					}
					else
					{
						if (AnimationEndReached != null && animEventArg.ValidateEvent(this.animationPlayer[1], currentAnim[1], 1, m_Anims0[(int)currentAnim[1]].AnimType, AnimationStateFlags.BlendEndExecuted))
						{
							AnimationEndReached(this, animEventArg);
							AnimationEndReached = animEventArg.NewHandler;
						}
						for (int m = 0; m < boneCount; m++)
						{
							ref Matrix reference4 = ref ModelTransform[qIndex][m];
							reference4 = this.animationPlayer[0].boneTransforms[m];
						}
						if (num == 2)
						{
							TransitionOutTime[1] = TransitionOutTime[2];
							TransitionInTime[1] = TransitionInTime[2];
							currentAnim[1] = currentAnim[2];
							AnimationPlayer animationPlayer = this.animationPlayer[1];
							this.animationPlayer[1] = this.animationPlayer[2];
							this.animationPlayer[2] = animationPlayer;
							this.animationPlayer[2].StartClip(null);
							num = 1;
						}
						else
						{
							num = 0;
							this.animationPlayer[1].StartClip(null);
						}
					}
				}
				else
				{
					for (int n = 0; n < boneCount; n++)
					{
						ref Matrix reference5 = ref ModelTransform[qIndex][n];
						reference5 = this.animationPlayer[1].boneTransforms[n];
					}
					if (this.animationPlayer[1].CurrentClip.Equals(this.animationPlayer[0].CurrentClip))
					{
						TransitionOutTime[0] = TransitionOutTime[1];
						TransitionInTime[0] = TransitionInTime[1];
						currentAnim[0] = currentAnim[1];
						AnimationPlayer animationPlayer2 = this.animationPlayer[0];
						this.animationPlayer[0] = this.animationPlayer[1];
						this.animationPlayer[1] = animationPlayer2;
						this.animationPlayer[1].StartClip(null);
						if (num == 2)
						{
							TransitionOutTime[1] = TransitionOutTime[2];
							TransitionInTime[1] = TransitionInTime[2];
							currentAnim[1] = currentAnim[2];
							animationPlayer2 = this.animationPlayer[1];
							this.animationPlayer[1] = this.animationPlayer[2];
							this.animationPlayer[2] = animationPlayer2;
							num = 1;
						}
						else
						{
							num = 0;
						}
					}
				}
			}
		}
		if (num == 2)
		{
			TransitionInTime[2] += (float)elapsedGameTime.Milliseconds * 0.001f;
			float num4 = TransitionInTime[2] / this.animationPlayer[2].CurrentClip.BlendInTime;
			if (num4 < 1f)
			{
				for (int num5 = 0; num5 < boneCount; num5++)
				{
					ref Matrix reference6 = ref ModelTransform[qIndex][num5];
					reference6 = ModelTransform[qIndex][num5] * (1f - num4);
					ModelTransform[qIndex][num5] += this.animationPlayer[2].boneTransforms[num5] * num4;
				}
			}
			else
			{
				if (AnimationBlendIn != null && animEventArg.ValidateEvent(this.animationPlayer[2], currentAnim[2], 2, m_Anims0[(int)currentAnim[2]].AnimType, AnimationStateFlags.BlendInExecuted))
				{
					AnimationBlendIn(this, animEventArg);
					AnimationBlendIn = animEventArg.NewHandler;
				}
				if ((this.animationPlayer[2].CurrentClip.AnimFlag & AnimFlag.AF_ONEOFF) != AnimFlag.AF_CLEAR && this.animationPlayer[2].CurrentClip.AnimType != AnimationType.WeaponPullout && this.animationPlayer[2].CurrentClip.AnimType != AnimationType.WeaponPutaway)
				{
					if (this.animationPlayer[2].BlendOutTimeReached)
					{
						if (AnimationBlendOut != null && animEventArg.ValidateEvent(this.animationPlayer[2], currentAnim[2], 2, m_Anims0[(int)currentAnim[2]].AnimType, AnimationStateFlags.BlendOutExecuted))
						{
							AnimationBlendOut(this, animEventArg);
							AnimationBlendOut = animEventArg.NewHandler;
						}
						TransitionOutTime[2] += (float)elapsedGameTime.Milliseconds * 0.001f;
						float num6 = (float)this.animationPlayer[2].CurrentTime.Subtract(this.animationPlayer[2].CurrentClip.BlendOutTime).Milliseconds * 0.001f;
						num6 /= this.animationPlayer[2].CurrentClip.fBlendOutTime;
						if (num6 >= 0f && num6 < 1f && !this.animationPlayer[2].EndReached)
						{
							for (int num7 = 0; num7 < boneCount; num7++)
							{
								ref Matrix reference7 = ref ModelTransform[qIndex][num7];
								reference7 = this.animationPlayer[1].boneTransforms[num7] * num6;
								ModelTransform[qIndex][num7] += this.animationPlayer[2].boneTransforms[num7] * (1f - num6);
							}
						}
						else
						{
							if (AnimationEndReached != null && animEventArg.ValidateEvent(this.animationPlayer[2], currentAnim[2], 2, m_Anims0[(int)currentAnim[2]].AnimType, AnimationStateFlags.BlendEndExecuted))
							{
								AnimationEndReached(this, animEventArg);
								AnimationEndReached = animEventArg.NewHandler;
							}
							for (int num8 = 0; num8 < boneCount; num8++)
							{
								ref Matrix reference8 = ref ModelTransform[qIndex][num8];
								reference8 = this.animationPlayer[1].boneTransforms[num8];
							}
							num = 1;
							this.animationPlayer[2].StartClip(null);
						}
					}
					else
					{
						for (int num9 = 0; num9 < boneCount; num9++)
						{
							ref Matrix reference9 = ref ModelTransform[qIndex][num9];
							reference9 = this.animationPlayer[2].boneTransforms[num9];
						}
					}
				}
				else
				{
					for (int num10 = 0; num10 < boneCount; num10++)
					{
						ref Matrix reference10 = ref ModelTransform[qIndex][num10];
						reference10 = this.animationPlayer[2].boneTransforms[num10];
					}
					TransitionOutTime[1] = TransitionOutTime[2];
					TransitionInTime[1] = TransitionInTime[2];
					currentAnim[1] = currentAnim[2];
					AnimationPlayer animationPlayer3 = this.animationPlayer[1];
					this.animationPlayer[1] = this.animationPlayer[2];
					this.animationPlayer[2] = animationPlayer3;
					this.animationPlayer[2].StartClip(null);
					num = 1;
				}
			}
		}
		numberAnimations = num;
		RunScheduledAnimations();
		if (mergeAnimPlayer.CurrentClip != null)
		{
			mergeAnimPlayer.Update(elapsedGameTime, relativeToCurrentTime: true, transform);
			mergeTransitionInTime += (float)elapsedGameTime.Milliseconds * 0.001f;
			float num11 = mergeTransitionInTime / mergeAnimPlayer.CurrentClip.BlendInTime;
			if (num11 < 1f)
			{
				for (int num12 = 0; num12 < mergeBoneCount; num12++)
				{
					int num13 = mergBoneIndices[num12];
					ref Matrix reference11 = ref ModelTransform[qIndex][num13];
					reference11 = ModelTransform[qIndex][num13] * (1f - num11);
					ModelTransform[qIndex][num13] += mergeAnimPlayer.boneTransforms[num13] * num11;
				}
			}
			else
			{
				if (AnimationBlendIn != null && animEventArg.ValidateEvent(mergeAnimPlayer, mergeAnimation, 0, m_Anims0[(int)currentAnim[0]].AnimType, AnimationStateFlags.BlendInExecuted))
				{
					AnimationBlendIn(this, animEventArg);
					AnimationBlendIn = animEventArg.NewHandler;
				}
				if (mergeAnimPlayer.BlendOutTimeReached)
				{
					if (AnimationBlendOut != null && animEventArg.ValidateEvent(mergeAnimPlayer, mergeAnimation, 0, m_Anims0[(int)currentAnim[0]].AnimType, AnimationStateFlags.BlendOutExecuted))
					{
						AnimationBlendOut(this, animEventArg);
						AnimationBlendOut = animEventArg.NewHandler;
					}
					mergeTransitionOutTime += (float)elapsedGameTime.Milliseconds * 0.001f;
					float num14 = (float)mergeAnimPlayer.CurrentTime.Subtract(mergeAnimPlayer.CurrentClip.BlendOutTime).Milliseconds * 0.001f;
					num14 /= mergeAnimPlayer.CurrentClip.fBlendOutTime;
					if (num14 >= 0f && num14 < 1f && !mergeAnimPlayer.EndReached)
					{
						for (int num15 = 0; num15 < mergeBoneCount; num15++)
						{
							int num16 = mergBoneIndices[num15];
							ref Matrix reference12 = ref ModelTransform[qIndex][num16];
							reference12 = ModelTransform[qIndex][num16] * num14;
							ModelTransform[qIndex][num16] += mergeAnimPlayer.boneTransforms[num16] * (1f - num14);
						}
					}
					else
					{
						if (AnimationEndReached != null && animEventArg.ValidateEvent(mergeAnimPlayer, mergeAnimation, 0, m_Anims0[(int)currentAnim[0]].AnimType, AnimationStateFlags.BlendEndExecuted))
						{
							AnimationEndReached(this, animEventArg);
							AnimationEndReached = animEventArg.NewHandler;
						}
						mergeAnimPlayer.StartClip(null);
					}
				}
				else
				{
					for (int num17 = 0; num17 < mergeBoneCount; num17++)
					{
						int num18 = mergBoneIndices[num17];
						ref Matrix reference13 = ref ModelTransform[qIndex][num18];
						reference13 = mergeAnimPlayer.boneTransforms[num18];
					}
				}
			}
		}
		ref Matrix reference14 = ref WorldTransformBlend[qIndex][0];
		reference14 = ModelTransform[qIndex][0] * this.animationPlayer[0].RootTransform;
		for (int num19 = 1; num19 < boneCount; num19++)
		{
			int num20 = skinningData.SkeletonHierarchy[num19];
			ref Matrix reference15 = ref WorldTransformBlend[qIndex][num19];
			reference15 = ModelTransform[qIndex][num19] * WorldTransformBlend[qIndex][num20];
		}
		for (int num21 = 0; num21 < boneCount; num21++)
		{
			ref Matrix reference16 = ref SkinTransformBlend[qIndex][num21];
			reference16 = skinningData.InverseBindPose[num21] * WorldTransformBlend[qIndex][num21];
		}
	}

	public void UpdateTimeStep()
	{
		animationPlayer[numberAnimations].UpdateTimeStep();
	}

	public void UpdateJustBoneTransforms(ref Matrix rootTransform, int qIndex, bool applyToModelTransforms)
	{
		animationPlayer[numberAnimations].UpdateJustBoneTransforms(ref rootTransform);
		if (applyToModelTransforms)
		{
			for (int i = 0; i < boneCount; i++)
			{
				ref Matrix reference = ref ModelTransform[qIndex][i];
				reference = animationPlayer[numberAnimations].boneTransforms[i];
			}
		}
	}

	public void UpdateTopAnim(ref Matrix transform, int qIndex)
	{
		int num = numberAnimations;
		for (int i = 0; i < boneCount; i++)
		{
			ref Matrix reference = ref ModelTransform[qIndex][i];
			reference = animationPlayer[num].boneTransforms[i];
		}
		ref Matrix reference2 = ref WorldTransformBlend[qIndex][0];
		reference2 = ModelTransform[qIndex][0] * animationPlayer[0].RootTransform;
		for (int j = 1; j < boneCount; j++)
		{
			int num2 = skinningData.SkeletonHierarchy[j];
			ref Matrix reference3 = ref WorldTransformBlend[qIndex][j];
			reference3 = ModelTransform[qIndex][j] * WorldTransformBlend[qIndex][num2];
		}
		for (int k = 0; k < boneCount; k++)
		{
			ref Matrix reference4 = ref SkinTransformBlend[qIndex][k];
			reference4 = skinningData.InverseBindPose[k] * WorldTransformBlend[qIndex][k];
		}
	}

	public bool PlayMergedAnimation(WeaponAnim animClip)
	{
		return PlayMergedAnimation(animClip, EndGameEngine.FIXED_TIME_STEP);
	}

	public bool PlayMergedAnimation(WeaponAnim animClip, int timeStep)
	{
		if (mergeAnimPlayer.CurrentClip != null && mergeAnimPlayer.CurrentClip.Equals(m_Anims0[(int)animClip].Clip))
		{
			return false;
		}
		mergeTransitionInTime = 0f;
		mergeTransitionOutTime = 0f;
		mergeAnimation = animClip;
		mergeAnimPlayer.StartClip(m_Anims0[(int)mergeAnimation].Clip);
		mergeAnimPlayer.ElapsedTimeStep = new TimeSpan(timeStep);
		if (mergeAnimation == WeaponAnim.CoOpReload || mergeAnimation == WeaponAnim.CoOpSwap)
		{
			mergeBoneCount = 11;
			for (int i = 0; i < mergeBoneCount; i++)
			{
				mergBoneIndices[i] = ReloadMergeBones[i];
			}
		}
		else if (mergeAnimation == WeaponAnim.CoOpKnife)
		{
			mergeBoneCount = 9;
			for (int j = 0; j < mergeBoneCount; j++)
			{
				mergBoneIndices[j] = KnifeMergeBones[j];
			}
		}
		else if (mergeAnimation == WeaponAnim.CoOpRightPunch || mergeAnimation == WeaponAnim.CoOpAxeSwing)
		{
			if (Owner.Stance == PlayerStance.Idle)
			{
				mergeBoneCount = boneCount;
				for (int k = 0; k < mergeBoneCount; k++)
				{
					mergBoneIndices[k] = k;
				}
				if (mergeAnimation == WeaponAnim.CoOpRightPunch)
				{
					SetBaseAnimation(WeaponAnim.CoOpIdleEmptySighted);
					PlayAnimation(WeaponAnim.CoOpIdleEmptySighted, force: true);
				}
			}
			else if (Owner.Stance == PlayerStance.Crouch)
			{
				mergeBoneCount = 9;
				for (int l = 2; l < mergeBoneCount; l++)
				{
					mergBoneIndices[l - 2] = MeleeAttackMergeBones[l];
				}
			}
			else
			{
				mergeBoneCount = 11;
				for (int m = 0; m < mergeBoneCount; m++)
				{
					mergBoneIndices[m] = MeleeAttackMergeBones[m];
				}
			}
		}
		else if (mergeAnimation == WeaponAnim.CoOpJump)
		{
			mergeBoneCount = 11;
			for (int n = 0; n < mergeBoneCount; n++)
			{
				mergBoneIndices[n] = JumpMergeBones[n];
			}
		}
		return true;
	}

	public void QueueAnimation(WeaponAnim animClip, bool force)
	{
		QueueAnimation(animClip, force, 166000);
	}

	public void QueueAnimation(WeaponAnim animClip, bool force, int timeStep)
	{
		if (NumScheduledAnimations < MaxScheduledAnimation)
		{
			ScheduledAnimations[NumScheduledAnimations].force = force;
			ScheduledAnimations[NumScheduledAnimations].animClip = animClip;
			ScheduledAnimations[NumScheduledAnimations].timeStepTicks = timeStep;
			NumScheduledAnimations++;
		}
	}

	public void RunScheduledAnimations()
	{
		for (int i = 0; i < NumScheduledAnimations; i++)
		{
			PlayAnimation(ScheduledAnimations[i].animClip, ScheduledAnimations[i].force, ScheduledAnimations[i].timeStepTicks);
		}
		NumScheduledAnimations = 0;
	}

	public bool PlayAnimation(WeaponAnim animClip, bool force)
	{
		return PlayAnimation(animClip, force, EndGameEngine.FIXED_TIME_STEP);
	}

	public bool PlayAnimation(WeaponAnim animClip, bool force, int timeStep)
	{
		if (this.animationPlayer[numberAnimations].CurrentClip == null)
		{
			return false;
		}
		if (!force && (this.animationPlayer[numberAnimations].CurrentClip.AnimFlag & AnimFlag.AF_ONEOFF) != AnimFlag.AF_CLEAR)
		{
			return false;
		}
		if (this.animationPlayer[numberAnimations].CurrentClip.Equals(m_Anims0[(int)animClip].Clip))
		{
			return true;
		}
		if (numberAnimations < 2)
		{
			TransitionInTime[numberAnimations] = 1f;
			numberAnimations++;
			CurrentAnimation = animClip;
			TransitionInTime[numberAnimations] = 0f;
			TransitionOutTime[numberAnimations] = 0f;
			this.animationPlayer[numberAnimations].CurrentAnimation = (int)CurrentAnimation;
			this.animationPlayer[numberAnimations].StartClip(m_Anims0[(int)CurrentAnimation].Clip);
			this.animationPlayer[numberAnimations].ElapsedTimeStep = new TimeSpan(timeStep);
			return true;
		}
		if (force)
		{
			TransitionOutTime[1] = TransitionOutTime[2];
			TransitionInTime[1] = TransitionInTime[2];
			currentAnim[1] = currentAnim[2];
			AnimationPlayer animationPlayer = this.animationPlayer[1];
			this.animationPlayer[1] = this.animationPlayer[2];
			this.animationPlayer[2] = animationPlayer;
			numberAnimations = 2;
			CurrentAnimation = animClip;
			TransitionInTime[numberAnimations] = 0f;
			TransitionOutTime[numberAnimations] = 0f;
			this.animationPlayer[numberAnimations].CurrentAnimation = (int)CurrentAnimation;
			this.animationPlayer[numberAnimations].StartClip(m_Anims0[(int)CurrentAnimation].Clip);
			this.animationPlayer[numberAnimations].ElapsedTimeStep = new TimeSpan(timeStep);
			return true;
		}
		return false;
	}

	public void SetCharacter(Model m, int animationSet)
	{
		m_Anims0 = AnimationsBaseList[animationSet];
		skinningData = ((SkinnedAnimationData)m.Tag).skinningData;
		boneCount = skinningData.BindPose.Count;
		numberAnimations = 0;
		NumScheduledAnimations = 0;
		mergeTransitionInTime = 0f;
		mergeTransitionOutTime = 0f;
		TransitionInTime[0] = 0f;
		TransitionInTime[1] = 0f;
		TransitionInTime[2] = 0f;
		TransitionOutTime[0] = 0f;
		TransitionOutTime[1] = 0f;
		TransitionOutTime[2] = 0f;
		AnimationPlayer animationPlayer;
		for (int i = 0; i < 3; i++)
		{
			animationPlayer = new AnimationPlayer(skinningData);
			this.animationPlayer[i] = animationPlayer;
		}
		animationPlayer = new AnimationPlayer(skinningData);
		mergeAnimPlayer = animationPlayer;
		currentAnim[0] = WeaponAnim.Idle;
		this.animationPlayer[0].StartClip(m_Anims0[(int)currentAnim[0]].Clip);
	}

	public static void LoadAnimations(List<CharacterData> animList)
	{
		if (AnimationsBaseList != null)
		{
			return;
		}
		string[] array = EndGameEngine.GameAssetMgr.Load<string[]>("data\\AnimationDataXml");
		for (WeaponAnim weaponAnim = WeaponAnim.Idle; weaponAnim < WeaponAnim.NumOfAnimations; weaponAnim++)
		{
			m_AnimsBase[(int)weaponAnim].Name = array[(int)weaponAnim];
		}
		AnimationsBaseList = new List<FPSAnimationState[]>(animList.Count);
		for (int i = 0; i < animList.Count; i++)
		{
			FPSAnimationState[] array2 = new FPSAnimationState[131];
			for (WeaponAnim weaponAnim2 = WeaponAnim.Idle; weaponAnim2 < WeaponAnim.NumOfAnimations; weaponAnim2++)
			{
				array2[(int)weaponAnim2] = new FPSAnimationState(m_AnimsBase[(int)weaponAnim2]);
			}
			array2[53].Name = animList[i].CoOpIdle;
			array2[62].Name = animList[i].CoOpCrouch;
			array2[63].Name = animList[i].CoOpCrouchWalk;
			array2[64].Name = animList[i].CoOpCrouchWalkBack;
			array2[71].Name = animList[i].CoOpWalk;
			array2[72].Name = animList[i].CoOpWalkBack;
			array2[78].Name = animList[i].CoOpSideStepLeft;
			array2[79].Name = animList[i].CoOpSideStepRight;
			array2[82].Name = animList[i].CoOpRun;
			array2[85].Name = animList[i].CoOpReload;
			array2[86].Name = animList[i].CoOpKnife;
			array2[87].Name = animList[i].CoOpSwap;
			array2[88].Name = animList[i].CoOpJump;
			array2[91].Name = animList[i].CoOpClimb;
			array2[92].Name = animList[i].CoOpClimbUp;
			array2[93].Name = animList[i].CoOpDeath00;
			array2[94].Name = animList[i].CoOpDeath01;
			array2[95].Name = animList[i].CoOpDeath02;
			array2[96].Name = animList[i].CoOpDeath03;
			AnimationsBaseList.Add(array2);
		}
		for (int j = 0; j < animList.Count; j++)
		{
			for (WeaponAnim weaponAnim3 = WeaponAnim.Idle; weaponAnim3 < WeaponAnim.NumOfAnimations; weaponAnim3++)
			{
				if (AnimationsBaseList[j][(int)weaponAnim3].Name != "null")
				{
					string assetName = "animations\\" + AnimationsBaseList[j][(int)weaponAnim3].Name;
					Model model = EndGameEngine.GameAssetMgr.Load<Model>(assetName);
					SkinningData skinningData = ((SkinnedAnimationData)model.Tag).skinningData;
					AnimationsBaseList[j][(int)weaponAnim3].Clip = skinningData.AnimationClips["Take 001"];
					AnimationsBaseList[j][(int)weaponAnim3].BoneIndices = new int[skinningData.BoneIndices.Count];
					skinningData.BoneIndices.CopyTo(AnimationsBaseList[j][(int)weaponAnim3].BoneIndices, 0);
					AnimationsBaseList[j][(int)weaponAnim3].AnimationTexture = ((SkinnedAnimationData)model.Tag).animationTexture;
					AnimationsBaseList[j][(int)weaponAnim3].Clip.AnimType = AnimationsBaseList[j][(int)weaponAnim3].AnimType;
					AnimationsBaseList[j][(int)weaponAnim3].Clip.AnimFlag = AnimationsBaseList[j][(int)weaponAnim3].Flags;
					AnimationsBaseList[j][(int)weaponAnim3].Clip.BlendInTime = AnimationsBaseList[j][(int)weaponAnim3].BlendInTime;
					AnimationsBaseList[j][(int)weaponAnim3].Clip.Speed = AnimationsBaseList[j][(int)weaponAnim3].Speed;
					AnimationsBaseList[j][(int)weaponAnim3].Clip.BlendOverRide = AnimationsBaseList[j][(int)weaponAnim3].BlendOverRide;
					AnimationsBaseList[j][(int)weaponAnim3].Clip.BlendOutTime = AnimationsBaseList[j][(int)weaponAnim3].Clip.Duration.Subtract(AnimationsBaseList[j][(int)weaponAnim3].BlendOutTime);
					AnimationsBaseList[j][(int)weaponAnim3].Clip.fBlendOutTime = (float)AnimationsBaseList[j][(int)weaponAnim3].BlendOutTime.Milliseconds * 0.001f;
				}
			}
		}
	}

	public void Initialize(Model m, int animationSet)
	{
		m_Anims0 = AnimationsBaseList[animationSet];
		numberAnimations = 0;
		TransitionInTime[0] = 0f;
		TransitionInTime[1] = 0f;
		TransitionInTime[2] = 0f;
		TransitionOutTime[0] = 0f;
		TransitionOutTime[1] = 0f;
		TransitionOutTime[2] = 0f;
		skinningData = ((SkinnedAnimationData)m.Tag).skinningData;
		boneCount = skinningData.BindPose.Count;
		if (!animationInit)
		{
			for (int i = 0; i < 2; i++)
			{
				ModelTransform[i] = new Matrix[boneCount];
				WorldTransformBlend[i] = new Matrix[boneCount];
				SkinTransformBlend[i] = new Matrix[boneCount];
				for (int j = 0; j < boneCount; j++)
				{
					ref Matrix reference = ref ModelTransform[i][j];
					reference = Matrix.Identity;
					ref Matrix reference2 = ref WorldTransformBlend[i][j];
					reference2 = Matrix.Identity;
					ref Matrix reference3 = ref SkinTransformBlend[i][j];
					reference3 = Matrix.Identity;
				}
			}
			for (int k = 0; k < MaxScheduledAnimation; k++)
			{
				ScheduledAnimations[k] = default(FPSAnimationQueue);
			}
		}
		SetCharacter(m, animationSet);
	}

	public void SetAnimationKeyEvent(WeaponAnim anim, int frame, EventHandler<AnimationEventArgs> cb)
	{
		m_Anims0[(int)anim].SetAnimationKey(frame, cb);
	}

	public void Set(Model m)
	{
	}
}
