using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DPSF;

/// <summary>
/// Class to hold a List of Animations and the texture coordintes of the Pictures used by the Animations.
/// To start, Create Picture's of all images that will be used in any Animations. Then Create an Animation
/// by specifying the order of the Picture IDs to go through, and the speed to flip through them at (i.e. frame-rate).
/// </summary>
public class Animations
{
	/// <summary>
	/// Structure to store an individual Picture's position and dimensions within a texture
	/// </summary>
	private struct SPicture
	{
		public int iID;

		public Rectangle sTextureCoordinates;

		/// <summary>
		/// Explicit constructor
		/// </summary>
		/// <param name="iID">The ID of this Picture (this should be unique)</param>
		/// <param name="sTextureCoordinates">The top-left (x,y) position and (width,height) dimensions
		/// of the Picture within the texture</param>
		public SPicture(int iID, Rectangle sTextureCoordinates)
		{
			this.iID = iID;
			this.sTextureCoordinates = sTextureCoordinates;
		}
	}

	/// <summary>
	/// Class to hold a single Animation's (i.e. Walking, Running, Jumping, etc) sequence of 
	/// Pictures and how long to display each Picture in the Animation for
	/// </summary>
	private class Animation
	{
		public int miID;

		public List<int> mcPictureRotationOrder;

		public int miCurrentPictureIndex;

		public float mfPictureRotationTime;

		public int miNumberOfTimesToPlay;

		public int miNumberOfTimesPlayed;

		/// <summary>
		/// Returns the Picture ID of the Current Picture being displayed.
		/// </summary>
		public int CurrentPicturesID => mcPictureRotationOrder[miCurrentPictureIndex];

		/// <summary>
		/// Get if the Animation has finished Playing or not.
		/// NOTE: Animations with Number Of Times To Play == 0 will never end
		/// </summary>
		public bool AnimationHasEnded
		{
			get
			{
				if (miNumberOfTimesPlayed == miNumberOfTimesToPlay)
				{
					return miNumberOfTimesToPlay != 0;
				}
				return false;
			}
		}

		/// <summary>
		/// Explicit Constructor
		/// </summary>
		/// <param name="iID">The ID of this Animation (this should be unique)</param>
		/// <param name="cPictureRotationOrder">A List of Picture ID's which tell the sequence of 
		/// Pictures that make up the Animation</param>
		/// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
		/// next Picture in the Picture Rotation Order</param>
		/// <param name="iNumberOfTimesToPlay">The Number of Times the Animation should Play before stopping. A value
		/// of zero means the Animation should repeat forever.</param>
		public Animation(int iID, List<int> cPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
		{
			miID = iID;
			mcPictureRotationOrder = new List<int>(cPictureRotationOrder);
			miCurrentPictureIndex = 0;
			mfPictureRotationTime = fPictureRotationTime;
			miNumberOfTimesToPlay = iNumberOfTimesToPlay;
			miNumberOfTimesPlayed = 0;
		}

		/// <summary>
		/// Moves the Current Picture Index to the next element in the Picture Rotation Order, and loops
		/// if it reaches the end of the Animation
		/// </summary>
		public void MoveToNextPictureInAnimation()
		{
			miCurrentPictureIndex++;
			if (miCurrentPictureIndex >= mcPictureRotationOrder.Count)
			{
				if (miNumberOfTimesToPlay == 0 || miNumberOfTimesPlayed < miNumberOfTimesToPlay)
				{
					miCurrentPictureIndex = 0;
					miNumberOfTimesPlayed++;
				}
				else
				{
					miNumberOfTimesPlayed = miNumberOfTimesToPlay;
					miCurrentPictureIndex = mcPictureRotationOrder.Count;
				}
			}
		}
	}

	private List<SPicture> mcPictures = new List<SPicture>();

	private List<Animation> mcAnimations = new List<Animation>();

	private int miCurrentAnimationID = -1;

	private float mfAnimationFrameTimer;

	private bool mbPaused;

	/// <summary>
	/// Get / Set the Current Animation being used. The Animation is started at its beginning.
	/// <para>NOTE: If an invalid Animiation ID is given when Setting, the Animation will not be changed.</para>
	/// <para>NOTE: If an Animation has not beeng set yet when Getting, -1 is returned.</para>
	/// </summary>
	public int CurrentAnimationID
	{
		get
		{
			return miCurrentAnimationID;
		}
		set
		{
			if (value < mcAnimations.Count && value >= 0)
			{
				miCurrentAnimationID = value;
				mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = 0;
				mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = 0;
				mfAnimationFrameTimer = 0f;
			}
		}
	}

	/// <summary>
	/// Get / Set how much Time should elapsed before switching frames in the Current Animation. 
	/// <para>NOTE: If no Animation has been set yet, zero will be returned.</para>
	/// </summary>
	public float CurrentAnimationsPictureRotationTime
	{
		get
		{
			return GetAnimationsPictureRotationTime(miCurrentAnimationID);
		}
		set
		{
			SetAnimationsPictureRotationTime(miCurrentAnimationID, value);
		}
	}

	/// <summary>
	/// Get / Set the Current Index in the Current Animation's Picture Rotation Order. 
	/// <para>NOTE: If no Animation has been set yet, Get returns -1, and Set doesn't change anything 
	/// (as well as if the specified Index is invalid).</para>
	/// </summary>
	public int CurrentAnimationsPictureRotationOrderIndex
	{
		get
		{
			if (CurrentAnimationIsValid)
			{
				return mcAnimations[miCurrentAnimationID].miCurrentPictureIndex;
			}
			return -1;
		}
		set
		{
			if (CurrentAnimationIsValid && value >= 0 && value < mcAnimations[miCurrentAnimationID].mcPictureRotationOrder.Count)
			{
				mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = value;
				mfAnimationFrameTimer = 0f;
			}
		}
	}

	/// <summary>
	/// Get / Set the Number of times the Current Animation should Play
	/// (it replays when the end of the Animation is reached). 
	/// Specify a value of zero to have the Animation repeat forever.
	/// <para>NOTE: If no Animation has been set yet, no changes are made when
	/// Setting, and -1 is returned when Getting.</para>
	/// </summary>
	public int CurrentAnimationsNumberOfTimesToPlay
	{
		get
		{
			return GetAnimationsNumberOfTimesToPlay(miCurrentAnimationID);
		}
		set
		{
			SetAnimationsNumberOfTimesToPlay(miCurrentAnimationID, value);
		}
	}

	/// <summary>
	/// Get / Set the Number of times the Current Animation has Played already.
	/// <para>NOTE: If no Animation has been set yet, Get returns -1, and Set doesn't change anything.</para>
	/// </summary>
	public int CurrentAnimationsNumberOfTimesPlayed
	{
		get
		{
			if (CurrentAnimationIsValid)
			{
				return mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed;
			}
			return -1;
		}
		set
		{
			if (CurrentAnimationIsValid)
			{
				mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = value;
			}
		}
	}

	/// <summary>
	/// Get if the Current Animation is Done Playing or not (i.e. Its Number Of Times Played is
	/// greater than or equal to its Number Of Times To Play). Returns true even if no
	/// Animation has been set to Play yet.
	/// </summary>
	public bool CurrentAnimationIsDonePlaying
	{
		get
		{
			if (CurrentAnimationIsValid)
			{
				int currentAnimationsNumberOfTimesToPlay = CurrentAnimationsNumberOfTimesToPlay;
				if (currentAnimationsNumberOfTimesToPlay != 0)
				{
					return CurrentAnimationsNumberOfTimesPlayed >= currentAnimationsNumberOfTimesToPlay;
				}
				return false;
			}
			return true;
		}
	}

	/// <summary>
	/// Gets the amount of Time (in seconds) required to play the Current Animation.
	/// <para>NOTE: If no Animation has been played yet, zero is returned.</para>
	/// </summary>
	public float TimeRequiredToPlayCurrentAnimation => TimeRequiredToPlayAnimation(miCurrentAnimationID);

	/// <summary>
	/// Gets the amount of Time (in seconds) required to play the remainder of the Current Animation.
	/// <para>NOTE: If no Animation has been played yet, zero is returned.</para>
	/// </summary>
	public float TimeRequiredToPlayTheRestOfTheCurrentAnimation
	{
		get
		{
			if (CurrentAnimationIsValid)
			{
				Animation animation = mcAnimations[miCurrentAnimationID];
				float num = animation.mfPictureRotationTime - mfAnimationFrameTimer;
				float num2 = (float)(animation.mcPictureRotationOrder.Count - (animation.miCurrentPictureIndex + 1)) * animation.mfPictureRotationTime + num;
				float num3 = (float)animation.mcPictureRotationOrder.Count * animation.mfPictureRotationTime * (float)(animation.miNumberOfTimesToPlay - (animation.miNumberOfTimesPlayed + 1));
				return num2 + num3;
			}
			return 0f;
		}
	}

	/// <summary>
	/// Get the Rectangle representing the Texture Coordinates of the Picture 
	/// in the Animation that should be displayed at this point in time
	/// </summary>
	public Rectangle CurrentPicturesTextureCoordinates
	{
		get
		{
			Rectangle result = default(Rectangle);
			if (CurrentAnimationIsValid && !CurrentAnimationIsDonePlaying)
			{
				return mcPictures[mcAnimations[miCurrentAnimationID].CurrentPicturesID].sTextureCoordinates;
			}
			return result;
		}
	}

	/// <summary>
	/// Get / Set if the Animation should be Paused or not. If Paused, the Animation will
	/// not be Updated.
	/// </summary>
	public bool Paused
	{
		get
		{
			return mbPaused;
		}
		set
		{
			mbPaused = value;
		}
	}

	/// <summary>
	/// Get if the Current Animation has been set yet or not
	/// </summary>
	private bool CurrentAnimationIsValid => AnimationIDIsValid(miCurrentAnimationID);

	/// <summary>
	/// Copies the given Animations data into this Animation
	/// </summary>
	/// <param name="cAnimationToCopy">The Animation to Copy from</param>
	public void CopyFrom(Animations cAnimationToCopy)
	{
		int num = 0;
		miCurrentAnimationID = cAnimationToCopy.miCurrentAnimationID;
		mfAnimationFrameTimer = cAnimationToCopy.mfAnimationFrameTimer;
		mcPictures = new List<SPicture>(cAnimationToCopy.mcPictures);
		int count = cAnimationToCopy.mcAnimations.Count;
		mcAnimations = new List<Animation>(count);
		for (num = 0; num < count; num++)
		{
			Animation item = new Animation(cAnimationToCopy.mcAnimations[num].miID, cAnimationToCopy.mcAnimations[num].mcPictureRotationOrder, cAnimationToCopy.mcAnimations[num].mfPictureRotationTime, cAnimationToCopy.mcAnimations[num].miNumberOfTimesToPlay);
			mcAnimations.Add(item);
			mcAnimations[num].miCurrentPictureIndex = cAnimationToCopy.mcAnimations[num].miCurrentPictureIndex;
			mcAnimations[num].miNumberOfTimesPlayed = cAnimationToCopy.mcAnimations[num].miNumberOfTimesPlayed;
		}
	}

	/// <summary>
	/// Creates a Picture that can be used in a Animation, and returns its unique ID. 
	/// A Picture can be used multiple times in an Animation.
	/// </summary>
	/// <param name="sTextureCoordinates">The top-left (x,y) position and (width,height) dimensions
	/// in the Texture that form this Picture</param>
	/// <returns>Returns the new Picture's unique ID.</returns>
	public int CreatePicture(Rectangle sTextureCoordinates)
	{
		int count = mcPictures.Count;
		SPicture item = new SPicture(count, sTextureCoordinates);
		mcPictures.Add(item);
		return count;
	}

	/// <summary>
	/// Automatically creates the specified Total Number Of Pictures. All pictures are assumed to have 
	/// the same width and height, as specified in the First Picture rectangle. Also, the First Picture
	/// is assumed to be at the top-left corner of the Tileset.
	/// <para>Pictures are created in left-to-right, top-to-bottom order. The ID of the first Picture created
	/// is returned, with each new Picture created incrementing the ID value, so the last Picture created
	/// will have an ID of (returned ID + (Total Number Of Pictures - 1)).</para>
	/// </summary>
	/// <param name="iTotalNumberOfPictures">The Total Number Of Pictures in the Tileset</param>
	/// <param name="iPicturesPerRow">How many Pictures are in a row in the texture</param>
	/// <param name="sFirstPicture">The Position of the top-left Picture in the Tileset, and the
	/// width and height of each Picture in the Tileset</param>
	/// <returns>The ID of the first Picture created
	/// is returned, with each new Picture created incrementing the ID value, so the last Picture created
	/// will have an ID of (returned ID + (Total Number Of Pictures - 1)).</returns>
	public int CreatePicturesFromTileSet(int iTotalNumberOfPictures, int iPicturesPerRow, Rectangle sFirstPicture)
	{
		int num = 0;
		int num2 = 0;
		for (num = 0; num < iTotalNumberOfPictures; num++)
		{
			int num3 = num / iPicturesPerRow;
			int num4 = num % iPicturesPerRow;
			Rectangle sTextureCoordinates = new Rectangle(sFirstPicture.X + sFirstPicture.Width * num4, sFirstPicture.Y + sFirstPicture.Height * num3, sFirstPicture.Width, sFirstPicture.Height);
			num2 = CreatePicture(sTextureCoordinates);
		}
		return num2 - (iTotalNumberOfPictures - 1);
	}

	/// <summary>
	/// Creates a new Animation and returns the Animation's unique ID.
	/// <para>NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</para>
	/// <para>NOTE: Be sure to Create the Pictures before creating the Animation.</para>
	/// </summary>
	/// <param name="cPictureRotationOrder">A List of Picture IDs that specifies the Order of Pictures
	/// to Rotate through in order to produce the Animation. A single Picture ID can be used many times.</param>
	/// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
	/// next Picture in the Picture Rotation Order (i.e. The frame-rate of the Animation)</param>
	/// <param name="iNumberOfTimesToPlay">The number of times this Animation should be played 
	/// (it replays when the end of the Animation is reached). Specify a value of zero to have the 
	/// Animation repeat forever</param>
	/// <returns>Returns the new Animation's unique ID.</returns>
	public int CreateAnimation(List<int> cPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
	{
		int count = cPictureRotationOrder.Count;
		for (int i = 0; i < count; i++)
		{
			int num = cPictureRotationOrder[i];
			if (num < 0 || num > mcPictures.Count)
			{
				return -1;
			}
		}
		int count2 = mcAnimations.Count;
		Animation item = new Animation(count2, cPictureRotationOrder, fPictureRotationTime, iNumberOfTimesToPlay);
		mcAnimations.Add(item);
		return count2;
	}

	/// <summary>
	/// Creates a new Animation and returns the Animation's unique ID.
	/// <para>NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</para>
	/// <para>NOTE: Be sure to Create the Pictures before creating the Animation.</para>
	/// </summary>
	/// <param name="iaPictureRotationOrder">An array of Picture IDs that specifies the Order of Pictures
	/// to Rotate through in order to produce the Animation</param>
	/// <param name="fPictureRotationTime">How long (in seconds) to wait before switching to the
	/// next Picture in the Picture Rotation Order (i.e. The next Frame in the Animation)</param>
	/// <param name="iNumberOfTimesToPlay">The number of times this Animation should be played 
	/// (it replays when the end of the Animation is reached). Specify a value of zero to have the 
	/// Animation repeat forever</param>
	/// <returns>Returns the new Animation's unique ID.
	/// NOTE: Returns -1 if an invalid Picture ID was specified in the PictureRotationOrder.</returns>
	public int CreateAnimation(int[] iaPictureRotationOrder, float fPictureRotationTime, int iNumberOfTimesToPlay)
	{
		int num = 0;
		int num2 = iaPictureRotationOrder.Length;
		List<int> list = new List<int>(num2);
		for (num = 0; num < num2; num++)
		{
			list.Add(iaPictureRotationOrder[num]);
		}
		return CreateAnimation(list, fPictureRotationTime, iNumberOfTimesToPlay);
	}

	/// <summary>
	/// Returns true if the given Picture ID is valid (i.e. A Picture with the same ID exists).
	/// </summary>
	/// <param name="iPictureID">The Picture ID to look for</param>
	/// <returns>Returns true if the given Picture ID is valid (i.e. A Picture with the same ID exists).</returns>
	public bool PictureIDIsValid(int iPictureID)
	{
		if (iPictureID >= 0)
		{
			return iPictureID < mcPictures.Count;
		}
		return false;
	}

	/// <summary>
	/// Returns true if the given Animation ID is valid (i.e. An Animation with the same ID exists).
	/// </summary>
	/// <param name="iAnimationID">The Animation ID to look for</param>
	/// <returns>Returns true if the given Animation ID is valid (i.e. An Animation with the same ID exists).</returns>
	public bool AnimationIDIsValid(int iAnimationID)
	{
		if (iAnimationID >= 0)
		{
			return iAnimationID < mcAnimations.Count;
		}
		return false;
	}

	/// <summary>
	/// Sets the Current Animation being used, as well as what index in the Animation's Picture Rotation
	/// Order the Animation should start at. 
	/// <para>NOTE: If the specified Animiation to use is not valid, the Current Animation will not be 
	/// changed, and if the specified Picture Rotation Order Index is not valid, the Animation will 
	/// start from the beginning of the Animation.</para>
	/// </summary>
	/// <param name="iAnimationID">The ID of the Animation to use</param>
	/// <param name="iPictureRotationOrderIndex">The Index in the Animation's Picture Rotation Order
	/// that the Animation should begin playing from</param>
	public void SetCurrentAnimationAndPositionInAnimation(int iAnimationID, int iPictureRotationOrderIndex)
	{
		if (AnimationIDIsValid(iAnimationID))
		{
			miCurrentAnimationID = iAnimationID;
			if (iPictureRotationOrderIndex >= 0 && iPictureRotationOrderIndex < mcAnimations[miCurrentAnimationID].mcPictureRotationOrder.Count)
			{
				mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = iPictureRotationOrderIndex;
			}
			else
			{
				mcAnimations[miCurrentAnimationID].miCurrentPictureIndex = 0;
			}
			mcAnimations[miCurrentAnimationID].miNumberOfTimesPlayed = 0;
			mfAnimationFrameTimer = 0f;
		}
	}

	/// <summary>
	/// Returns how much Time (in seconds) should elapse before switching frames in the Animation.
	/// <para>NOTE: Returns zero if the specified Animation ID is not valid.</para>
	/// </summary>
	/// <param name="iAnimationID">The ID of the Animation containing the Picture Rotation Time to retrive</param>
	/// <returns>Returns how much Time (in seconds) should elapse before switching frames in the Animation.
	/// NOTE: Returns zero if the specified Animation ID is not valid.</returns>
	public float GetAnimationsPictureRotationTime(int iAnimationID)
	{
		float result = 0f;
		if (AnimationIDIsValid(iAnimationID))
		{
			result = mcAnimations[iAnimationID].mfPictureRotationTime;
		}
		return result;
	}

	/// <summary>
	/// Sets how much Time should elapse before switching frames in the Animation
	/// </summary>
	/// <param name="iAnimationID">The ID of the Animation to update</param>
	/// <param name="fNewPictureRotationTime">The Time (in seconds) to wait before moving to the
	/// next Picture in the Animations Picture Rotation Order</param>
	public void SetAnimationsPictureRotationTime(int iAnimationID, float fNewPictureRotationTime)
	{
		if (AnimationIDIsValid(iAnimationID))
		{
			mcAnimations[iAnimationID].mfPictureRotationTime = fNewPictureRotationTime;
		}
	}

	/// <summary>
	/// Returns the Number of times the given Animation ID is set to Play.
	/// Zero means the Animation will repeat forever.
	/// <para>NOTE: If the given Animation ID is invalid, -1 is returned.</para>
	/// </summary>
	/// <param name="iAnimationID">The ID of the Animation to update</param>
	/// <returns>Returns the Number of times the given Animation ID is set to Play.
	/// Zero means the Animation will repeat forever.
	/// NOTE: If the given Animation ID is invalid, -1 is returned.</returns>
	public int GetAnimationsNumberOfTimesToPlay(int iAnimationID)
	{
		int result = -1;
		if (AnimationIDIsValid(iAnimationID))
		{
			result = mcAnimations[iAnimationID].miNumberOfTimesToPlay;
		}
		return result;
	}

	/// <summary>
	/// Sets the Number of times the given Animation ID should Play
	/// (it replays when the end of the Animation is reached). 
	/// Specify a value of zero to have the Animation repeat forever.
	/// <para>NOTE: If the given Animation ID is invalid, no changes are made.</para>
	/// </summary>
	/// <param name="iAnimationID">The ID of the Animation to update</param>
	/// <param name="iNewNumberOfTimesToPlay">The New Number of times the Animation should Play</param>
	public void SetAnimationsNumberOfTimesToPlay(int iAnimationID, int iNewNumberOfTimesToPlay)
	{
		if (AnimationIDIsValid(iAnimationID))
		{
			mcAnimations[iAnimationID].miNumberOfTimesToPlay = iNewNumberOfTimesToPlay;
		}
	}

	/// <summary>
	/// Returns the amount of Time required to play the specified Animation.
	/// <para>NOTE: If an invalid AnimationID is specified, zero is returned.</para>
	/// </summary>
	/// <param name="iAnimationID">The ID of the Animation to check</param>
	/// <returns>Returns the amount of Time required to play the specified Animation.
	/// NOTE: If an invalid AnimationID is specified, zero is returned.</returns>
	public float TimeRequiredToPlayAnimation(int iAnimationID)
	{
		if (AnimationIDIsValid(iAnimationID))
		{
			Animation animation = mcAnimations[iAnimationID];
			return (float)animation.mcPictureRotationOrder.Count * animation.mfPictureRotationTime * (float)animation.miNumberOfTimesToPlay;
		}
		return 0f;
	}

	/// <summary>
	/// Returns the Rectangle representing the Texture Coordinates of the specified Picture.
	/// </summary>
	/// <param name="iPictureID">The Picture ID of the Picture whose Texture Coordinates 
	/// should be retrieved</param>
	/// <returns>Returns the Rectangle representing the Texture Coordinates of the specified Picture.</returns>
	public Rectangle GetPicturesTextureCoordinates(int iPictureID)
	{
		Rectangle result = default(Rectangle);
		if (iPictureID >= 0 && iPictureID < mcPictures.Count)
		{
			return mcPictures[iPictureID].sTextureCoordinates;
		}
		return result;
	}

	/// <summary>
	/// Updates the Animation according to how much time has elapsed
	/// </summary>
	/// <param name="fElapsedTime">The amount of Time (in seconds) since the last Update</param>
	public void Update(float fElapsedTime)
	{
		if (!Paused && CurrentAnimationIsValid)
		{
			mfAnimationFrameTimer += fElapsedTime;
			if (mfAnimationFrameTimer >= mcAnimations[miCurrentAnimationID].mfPictureRotationTime)
			{
				mfAnimationFrameTimer -= mcAnimations[miCurrentAnimationID].mfPictureRotationTime;
				mcAnimations[miCurrentAnimationID].MoveToNextPictureInAnimation();
			}
		}
	}
}
