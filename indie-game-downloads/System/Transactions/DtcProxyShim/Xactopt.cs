using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace System.Transactions.DtcProxyShim;

[NativeMarshalling(typeof(Marshaller))]
internal struct Xactopt
{
	[CustomMarshaller(typeof(Xactopt), MarshalMode.ManagedToUnmanagedIn, typeof(Marshaller))]
	[CustomMarshaller(typeof(Xactopt), MarshalMode.UnmanagedToManagedIn, typeof(Marshaller))]
	internal static class Marshaller
	{
		internal struct XactoptNative
		{
			public uint UlTimeout;

			public SzDescription SzDescription;
		}

		[InlineArray(40)]
		internal struct SzDescription
		{
			private byte _element0;
		}

		public static XactoptNative ConvertToUnmanaged(Xactopt managed)
		{
			XactoptNative result = new XactoptNative
			{
				UlTimeout = managed.UlTimeout
			};
			Encoding.ASCII.TryGetBytes(managed.SzDescription.AsSpan(), result.SzDescription, out var _);
			return result;
		}

		public static Xactopt ConvertToManaged(XactoptNative unmanaged)
		{
			return new Xactopt(unmanaged.UlTimeout, Encoding.ASCII.GetString(unmanaged.SzDescription));
		}
	}

	public uint UlTimeout;

	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
	public string SzDescription;

	internal Xactopt(uint ulTimeout, string szDescription)
	{
		UlTimeout = ulTimeout;
		SzDescription = szDescription;
	}
}
