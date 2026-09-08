using System;
using System.Threading;

namespace Microsoft.Xna.Framework.GamerServices;

public class AvatarDescription
{
	internal class AvatarDescriptionAction : IAsyncResult
	{
		public readonly AsyncCallback Callback;

		public object AsyncState { get; private set; }

		public bool CompletedSynchronously => false;

		public bool IsCompleted { get; internal set; }

		public WaitHandle AsyncWaitHandle { get; private set; }

		public AvatarDescriptionAction(object state, AsyncCallback callback)
		{
			AsyncState = state;
			Callback = callback;
			IsCompleted = false;
			AsyncWaitHandle = new ManualResetEvent(initialState: true);
		}
	}

	private const int descriptionSize = 1021;

	private byte[] description;

	public byte[] Description
	{
		get
		{
			byte[] array = new byte[1021];
			Array.Copy(description, array, 1021);
			return array;
		}
	}

	public bool IsValid => description[0] != 0;

	public float Height { get; private set; }

	public AvatarBodyType BodyType { get; private set; }

	public static event EventHandler<EventArgs> Changed;

	public AvatarDescription(byte[] data)
	{
		if (data == null)
		{
			throw new ArgumentNullException("availableSession");
		}
		if (data.Length != 1021)
		{
			throw new ArgumentException("The array is not the correct size for the amount of data requested.");
		}
		description = new byte[1021];
		Array.Copy(data, description, 1021);
	}

	internal AvatarDescription(bool isValid)
	{
		description = new byte[1021];
		if (isValid)
		{
			description[0] = 1;
		}
	}

	public static AvatarDescription CreateRandom()
	{
		return new AvatarDescription(isValid: true);
	}

	public static AvatarDescription CreateRandom(AvatarBodyType bodyType)
	{
		if (!Enum.IsDefined(typeof(AvatarBodyType), bodyType))
		{
			throw new ArgumentOutOfRangeException("bodyType");
		}
		return new AvatarDescription(isValid: true);
	}

	public static IAsyncResult BeginGetFromGamer(Gamer gamer, AsyncCallback callback, object state)
	{
		return new AvatarDescriptionAction(state, callback)
		{
			IsCompleted = true
		};
	}

	public static AvatarDescription EndGetFromGamer(IAsyncResult result)
	{
		return new AvatarDescription(isValid: false);
	}
}
