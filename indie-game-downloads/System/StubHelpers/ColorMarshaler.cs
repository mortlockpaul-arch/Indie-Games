using System.Reflection;

namespace System.StubHelpers;

internal static class ColorMarshaler
{
	private static readonly MethodInvoker s_oleColorToDrawingColorMethod;

	private static readonly MethodInvoker s_drawingColorToOleColorMethod;

	internal static readonly nint s_colorType;

	static ColorMarshaler()
	{
		Type type = Type.GetType("System.Drawing.ColorTranslator, System.Drawing.Primitives", throwOnError: true);
		Type type2 = Type.GetType("System.Drawing.Color, System.Drawing.Primitives", throwOnError: true);
		s_colorType = type2.TypeHandle.Value;
		s_oleColorToDrawingColorMethod = MethodInvoker.Create(type.GetMethod("FromOle", new Type[1] { typeof(int) }));
		s_drawingColorToOleColorMethod = MethodInvoker.Create(type.GetMethod("ToOle", new Type[1] { type2 }));
	}

	internal static object ConvertToManaged(int managedColor)
	{
		return s_oleColorToDrawingColorMethod.Invoke(null, managedColor);
	}

	internal static int ConvertToNative(object managedColor)
	{
		return (int)s_drawingColorToOleColorMethod.Invoke(null, managedColor);
	}
}
