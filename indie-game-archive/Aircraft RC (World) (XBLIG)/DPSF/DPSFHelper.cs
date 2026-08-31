using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DPSF;

/// <summary>
/// Collection of static functions for performing common operations
/// </summary>
public static class DPSFHelper
{
	private static RandomNumbers RandomNumber;

	/// <summary>
	/// Return the version of the DPSF.dll being used. 
	/// This includes the Major, Minor, Build, and Revision numbers.
	/// </summary>
	public static string Version => Assembly.GetExecutingAssembly().GetName().Version.ToString();

	/// <summary>
	/// Get if the Particle Systems are inheriting from DrawableGameComponent or not.
	/// <para>If inheriting from DrawableGameComponent, the Particle Systems
	/// are automatically added to the given Game object's Components and the
	/// Update() and Draw() functions are automatically called by the
	/// Game object when it updates and draws the rest of its Components.</para>
	/// <para>If the Update() and Draw() functions are called by the user anyways,
	/// they will exit without performing any operations, so it is suggested
	/// to include them anyways to make switching between inheriting and
	/// not inheriting from DrawableGameComponent seamless; just be aware
	/// that the updates and draws are actually being performed when the
	/// Game object is told to update and draw (i.e. when base.Update() and base.Draw()
	/// are called), not when the particle system functions are called.</para>
	/// </summary>
	public static bool DPSFInheritsDrawableGameComponent => false;

	/// <summary>
	/// Initializes the <see cref="T:DPSF.DPSFHelper" /> class.
	/// This constructor cannot be explicitly called. It will be called the first time this class is accessed.
	/// </summary>
	static DPSFHelper()
	{
		RandomNumber = new RandomNumbers();
	}

	/// <summary>
	/// Returns a random number between the specified values
	/// </summary>
	/// <param name="fValue1">The first value</param>
	/// <param name="fValue2">The second value</param>
	/// <returns>Returns a random number between the specified values</returns>
	public static float RandomNumberBetween(float fValue1, float fValue2)
	{
		return RandomNumber.Between(fValue1, fValue2);
	}

	/// <summary>
	/// Returns how transparent a Particle should be, based on it's Normalized Elapsed Time, so that it
	/// fades in quickly and fades out slowly.
	/// </summary>
	/// <param name="fNormalizedElapsedTime">The current Normalized Elapsed Time (0.0 - 1.0) of a Particle</param>
	/// <returns>Returns the Alpha Color component that should be used for the Particle</returns>
	public static byte FadeInQuicklyAndFadeOutSlowlyBasedOnLifetime(float fNormalizedElapsedTime)
	{
		return (byte)((double)(fNormalizedElapsedTime * (1f - fNormalizedElapsedTime) * (1f - fNormalizedElapsedTime)) * 6.7 * 255.0);
	}

	/// <summary>
	/// Returns how transparent a Particle should be, based on it's Normalized Elapsed Time, so that it
	/// fades in quickly and fades out quickly.
	/// </summary>
	/// <param name="fNormalizedElapsedTime">The current Normalized Elapsed Time (0.0 - 1.0) of a Particle</param>
	/// <returns>Returns the Alpha Color component that should be used for the Particle</returns>
	public static byte FadeInQuicklyAndFadeOutQuicklyBasedOnLifetime(float fNormalizedElapsedTime)
	{
		if (fNormalizedElapsedTime < 0.1f)
		{
			return (byte)(int)MathHelper.Lerp(0f, 255f, fNormalizedElapsedTime * 10f);
		}
		if (fNormalizedElapsedTime > 0.8f)
		{
			return (byte)(int)MathHelper.Lerp(255f, 0f, (fNormalizedElapsedTime - 0.8f) * 5f);
		}
		return byte.MaxValue;
	}

	/// <summary>
	/// Returns the interpolation amount (between 0.0 and 1.0) that should be used in a Lerp function to have a 
	/// property reach its full value when the NormalizedLifetime reaches 0.5, and go back to its original value 
	/// by the time the NormalizedLifetime reaches 1.0.
	/// <para>An example of where to use this would be if you wanted a particle to start off small, and reach
	/// its full size when the particle's lifetime is half over, and then to shrink back to being small by the
	/// time the particle dies. You would use the value returned by this function as the interpolation amount
	/// for the Lerp function. e.g. MathHelper.Lerp(SmallSize, LargeSize, ValueReturnedByThisFunction).</para>
	/// </summary>
	/// <param name="fNormalizedElapsedTime">The current Normalized Elapsed Time (0.0 - 1.0) of a Particle</param>
	/// <returns>Returns the interpolation amount (between 0.0 and 1.0) that should be used in a Lerp function to have a 
	/// property reach its full value when the NormalizedLifetime reaches 0.5, and go back to its original value 
	/// by the time the NormalizedLifetime reaches 1.0.</returns>
	public static float InterpolationAmountForEqualLerpInAndLerpOut(float fNormalizedElapsedTime)
	{
		if (fNormalizedElapsedTime <= 0.5f)
		{
			return fNormalizedElapsedTime * 2f;
		}
		return 1f - (fNormalizedElapsedTime - 0.5f) * 2f;
	}

	/// <summary>
	/// Returns a vector with a Random direction that has been Normalized.
	/// </summary>
	/// <returns>Returns a vector with a Random direction that has been Normalized.</returns>
	public static Vector3 RandomNormalizedVector()
	{
		Vector3 result = new Vector3(RandomNumber.Between(-1f, 1f), RandomNumber.Between(-1f, 1f), RandomNumber.Between(-1f, 1f));
		result.Normalize();
		return result;
	}

	/// <summary>
	/// Returns a vector representing the line from the source point to the target point.
	/// </summary>
	/// <param name="source">The source position.</param>
	/// <param name="target">The target position.</param>
	/// <returns>Returns a vector representing the line from the source point to the target point.</returns>
	public static Vector3 SourceToTargetVector(Vector3 source, Vector3 target)
	{
		return target - source;
	}

	/// <summary>
	/// Returns a normalized vector representing the direction that points from the source point to the target point.
	/// </summary>
	/// <param name="source">The source position.</param>
	/// <param name="target">The target position.</param>
	/// <returns>Returns a normalized vector representing the direction that points from the source point to the target point.</returns>
	public static Vector3 SourceToTargetDirection(Vector3 source, Vector3 target)
	{
		Vector3 result = SourceToTargetVector(source, target);
		result.Normalize();
		return result;
	}

	/// <summary>
	/// Returns a Vector whose individual XYZ components are each randomly chosen to be somewhere 
	/// between the two given Vectors' individual XYZ components. Unlike choosing a random Lerp value
	/// between two vectors, which would give a point somewhere on the LINE between the two points, this
	/// chooses a random Lerp value between each of the two vectors individual xyz components, returning
	/// a point somewhere in the cube-shaped AREA (i.e. Volume) between the two points.
	/// </summary>
	/// <param name="sVector1">The first Vector</param>
	/// <param name="sVector2">The second Vector</param>
	/// <returns>Returns a Vector whose individual XYZ components are each randomly chosen to be somewhere 
	/// between the two given Vectors' individual XYZ components.</returns>
	public static Vector3 RandomVectorBetweenTwoVectors(Vector3 sVector1, Vector3 sVector2)
	{
		return new Vector3(RandomNumber.Between(sVector1.X, sVector2.X), RandomNumber.Between(sVector1.Y, sVector2.Y), RandomNumber.Between(sVector1.Z, sVector2.Z));
	}

	/// <summary>
	/// Returns a random opaque Color (i.e. no transparency)
	/// </summary>
	/// <returns>Returns a random opaque Color (i.e. no transparency)</returns>
	public static Color RandomColor()
	{
		return new Color(RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
	}

	/// <summary>
	/// Returns a random Color with a random alpha value as well (i.e. random transparency)
	/// </summary>
	/// <returns>Returns a random Color with a random alpha value as well (i.e. random transparency)</returns>
	public static Color RandomColorWithRandomTransparency()
	{
		return new Color(RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat(), RandomNumber.NextFloat());
	}

	/// <summary>
	/// Returns the linearly interpolated Color between Color 1 and Color 2
	/// </summary>
	/// <param name="sColor1">The Color value used with InterpolationAmount 0.0</param>
	/// <param name="sColor2">The Color value used with InterpolationAmount 1.0</param>
	/// <param name="fInterpolationAmount">A value between 0.0 and 1.0 indicating how 
	/// much to interpolate the RGBA values between Color 1 and Color 2</param>
	/// <returns>Returns the interpolated Color</returns>
	public static Color LerpColor(Color sColor1, Color sColor2, float fInterpolationAmount)
	{
		return LerpColor(sColor1, sColor2, fInterpolationAmount, fInterpolationAmount, fInterpolationAmount, fInterpolationAmount);
	}

	/// <summary>
	/// Returns the linearly interpolated Color between Color 1 and Color 2
	/// </summary>
	/// <param name="sColor1">The Color value used with InterpolationAmount 0.0</param>
	/// <param name="sColor2">The Color value used with InterpolationAmount 1.0</param>
	/// <param name="fRInterpolationAmount">A value between 0.0 and 1.0 indicating how 
	/// much to interpolate the Red value between Color 1 and Color 2</param>
	/// <param name="fGInterpolationAmount">A value between 0.0 and 1.0 indicating how 
	/// much to interpolate the Green value between Color 1 and Color 2</param>
	/// <param name="fBInterpolationAmount">A value between 0.0 and 1.0 indicating how 
	/// much to interpolate the Blue value between Color 1 and Color 2</param>
	/// <param name="fAInterpolationAmount">A value between 0.0 and 1.0 indicating how 
	/// much to interpolate the Alpha value between Color 1 and Color 2</param>
	/// <returns>Returns the interpolated Color</returns>
	public static Color LerpColor(Color sColor1, Color sColor2, float fRInterpolationAmount, float fGInterpolationAmount, float fBInterpolationAmount, float fAInterpolationAmount)
	{
		byte r = (byte)((float)(int)sColor1.R + (float)(sColor2.R - sColor1.R) * fRInterpolationAmount);
		byte g = (byte)((float)(int)sColor1.G + (float)(sColor2.G - sColor1.G) * fGInterpolationAmount);
		byte b = (byte)((float)(int)sColor1.B + (float)(sColor2.B - sColor1.B) * fBInterpolationAmount);
		byte a = (byte)((float)(int)sColor1.A + (float)(sColor2.A - sColor1.A) * fAInterpolationAmount);
		return new Color(r, g, b, a);
	}

	/// <summary>
	/// Returns a point on a circle with a radius of one, on the X-Y axis plane. 
	/// To use a different radius simply multiply the returned value by the desired radius value.
	/// </summary>
	/// <param name="fAngle">The angle on the circle in radians</param>
	/// <returns>Returns a 2D position on a circle</returns>
	public static Vector2 PointOnCircle(float fAngle)
	{
		float x = (float)Math.Cos(fAngle);
		float y = (float)Math.Sin(fAngle);
		return new Vector2(x, y);
	}

	/// <summary>
	/// Returns a point on a circle with a radius of one, on the X-Y axis plane (Z value of zero). 
	/// To use a different radius simply multiply the returned value by the desired radius value.
	/// </summary>
	/// <param name="fAngle">The angle on the circle in radians</param>
	/// <returns>Returns a 3D position on a circle</returns>
	public static Vector3 PointOnCircleVector3(float fAngle)
	{
		return new Vector3(PointOnCircle(fAngle), 0f);
	}

	/// <summary>
	/// Returns a random point on a circle with a radius of one, on the X-Y axis plane. 
	/// To use a different radius simply multiply the returned value by the desired radius value.
	/// </summary>
	/// <returns>Returns a random 2D position on a circle</returns>
	public static Vector2 RandomPointOnCircle()
	{
		return PointOnCircle(RandomNumber.Between(0f, (float)Math.PI * 2f));
	}

	/// <summary>
	/// Returns a random point on a circle with a radius of one, on the X-Y axis plane (Z value of zero). 
	/// To use a different radius simply multiply the returned value by the desired radius value.
	/// </summary>
	/// <returns>Returns a random 3D position on a circle, with a Z value of zero</returns>
	public static Vector3 RandomPointOnCircleVector3()
	{
		return new Vector3(RandomPointOnCircle(), 0f);
	}

	/// <summary>
	/// Returns a point on a sphere with a radius of one.  To use a different radius simply 
	/// multiply the returned value by the desired radius value, before translating it
	/// to the sphere's position in world coordinates.  To create a circle simply use a
	/// constant value for one of the Angles while changing the other Angle.
	/// </summary>
	/// <param name="fYawAngle">Imagine a point on the surface of the sphere at the front 
	/// center of the sphere. This value will be how much to rotate that point around the 
	/// horizontal ring around the center of the sphere, in radians.</param>
	/// <param name="fPitchAngle">This value will be how much to rotate that point around 
	/// the vertical ring around the sphere, at the current position of the point, in radians.</param>
	/// <returns>Returns a point on a sphere with a radius of one</returns>
	public static Vector3 NormalizedPointOnSphere(float fYawAngle, float fPitchAngle)
	{
		return PivotPoint3D.RotatePosition(Matrix.CreateFromYawPitchRoll(fYawAngle, fPitchAngle, 0f), Vector3.Zero, Vector3.Forward);
	}

	/// <summary>
	/// Returns a random point on a sphere with a radius of one.  To use a different radius simply 
	/// multiply the returned value by the desired radius value, before translating it
	/// to the sphere's position in world coordinates.
	/// </summary>
	/// <returns>Returns a random point on a sphere with a radius of one</returns>
	public static Vector3 RandomNormalizedPointOnSphere()
	{
		return NormalizedPointOnSphere(RandomNumber.Between(0f, (float)Math.PI * 2f), RandomNumber.Between(0f, (float)Math.PI * 2f));
	}

	/// <summary>
	/// Returns a point on a sphere with the given Radius. To create a circle simply use a
	/// constant value for one of the Angles while changing the other Angle.
	/// </summary>
	/// <param name="fYawAngle">Imagine a point on the surface of the sphere at the front 
	/// center of the sphere. This value will be how much to rotate that point around the 
	/// horizontal ring around the center of the sphere, in radians.</param>
	/// <param name="fPitchAngle">This value will be how much to rotate that point around 
	/// the vertical ring around the sphere, at the current position of the point, in radians.</param>
	/// <param name="fRadius">The radius the sphere should have</param>
	/// <returns>Returns a point on a sphere with the given radius</returns>
	public static Vector3 PointOnSphere(float fYawAngle, float fPitchAngle, float fRadius)
	{
		return NormalizedPointOnSphere(fYawAngle, fPitchAngle) * fRadius;
	}

	/// <summary>
	/// Returns a random point on a sphere with the given Radius
	/// </summary>
	/// <param name="fRadius">The radius the sphere should have</param>
	/// <returns>Returns a random point on a sphere with the given Radius</returns>
	public static Vector3 RandomPointOnSphere(float fRadius)
	{
		return PointOnSphere(RandomNumber.Between(0f, (float)Math.PI * 2f), RandomNumber.Between(0f, (float)Math.PI * 2f), fRadius);
	}

	/// <summary>
	/// Returns a point on a sphere with the given Radius, at the specified Sphere Position.
	/// To create a circle simply use a constant value for one of the Angles while changing 
	/// the other Angle.
	/// </summary>
	/// <param name="fYawAngle">Imagine a point on the surface of the sphere at the front 
	/// center of the sphere. This value will be how much to rotate that point around the 
	/// horizontal ring around the center of the sphere, in radians.</param>
	/// <param name="fPitchAngle">This value will be how much to rotate that point around 
	/// the vertical ring around the sphere, at the current position of the point, in radians.</param>
	/// <param name="fRadius">The radius the sphere should have</param>
	/// <param name="sSpherePosition">The center position of the sphere in world coordinates</param>
	/// <returns>Returns a point on a sphere with the given Radius, at the specified Sphere Position</returns>
	public static Vector3 PointOnSphere(float fYawAngle, float fPitchAngle, float fRadius, Vector3 sSpherePosition)
	{
		return PointOnSphere(fYawAngle, fPitchAngle, fRadius) + sSpherePosition;
	}

	/// <summary>
	/// Returns a random point on a sphere with the given Radius, at the specified Sphere Position
	/// </summary>
	/// <param name="fRadius">The radius the sphere should have</param>
	/// <param name="sSpherePosition">The center position of the sphere in world coordinates</param>
	/// <returns>Returns a random point on a sphere with the given Radius, at the specified Sphere Position</returns>
	public static Vector3 RandomPointOnSphere(float fRadius, Vector3 sSpherePosition)
	{
		return PointOnSphere(RandomNumber.Between(0f, (float)Math.PI * 2f), RandomNumber.Between(0f, (float)Math.PI * 2f), fRadius, sSpherePosition);
	}

	/// <summary>
	/// Returns true if the difference between the individual XYZ components of the
	/// given Vectors are all less than the specified Tolerance
	/// </summary>
	/// <param name="sVector1">The first Vector</param>
	/// <param name="sVector2">The second Vector</param>
	/// <param name="fTolerance">How much of a difference there may be between the individual
	/// XYZ components for them to be considered equal</param>
	/// <returns>Returns true if the difference between the individual XYZ components of the
	/// given Vectors are each less than the specified Tolerance</returns>
	public static bool VectorsAreEqualWithinTolerance(Vector3 sVector1, Vector3 sVector2, float fTolerance)
	{
		sVector1.Normalize();
		sVector2.Normalize();
		Vector3 vector = sVector1 - sVector2;
		if (Math.Abs(vector.X) <= fTolerance && Math.Abs(vector.X) <= fTolerance && Math.Abs(vector.X) <= fTolerance)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Returns if the polarity (i.e. positive or negative) of each X,Y,Z component of the two vectors are the same or not.
	/// <para>Example: if Vector 1's X component is positive and Vector 2's X component is negative, this will return false.</para>
	/// <para>Example: if Vector 1's X,Y,Z components are all positive, and Vector 2's X,Y,Z components are all positive, this will return true.</para>
	/// </summary>
	/// <param name="sVector1">The first Vector.</param>
	/// <param name="sVector2">The second Vector.</param>
	/// <returns></returns>
	public static bool VectorsAreTheSamePolarity(Vector3 sVector1, Vector3 sVector2)
	{
		if ((sVector1.X > 0f && sVector2.X < 0f) || (sVector1.X < 0f && sVector2.X > 0f))
		{
			return false;
		}
		if ((sVector1.Y > 0f && sVector2.Y < 0f) || (sVector1.Y < 0f && sVector2.Y > 0f))
		{
			return false;
		}
		if ((sVector1.Z > 0f && sVector2.Z < 0f) || (sVector1.Z < 0f && sVector2.Z > 0f))
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Returns a clone of the given BlendState.
	/// Read-only properties, such as the GraphicsDevice property, are not copied though.
	/// </summary>
	/// <param name="blendStateToClone">The BlendState to clone.</param>
	public static BlendState CloneBlendState(BlendState blendStateToClone)
	{
		BlendState blendState = new BlendState();
		blendState.AlphaBlendFunction = blendStateToClone.AlphaBlendFunction;
		blendState.AlphaDestinationBlend = blendStateToClone.AlphaDestinationBlend;
		blendState.AlphaSourceBlend = blendStateToClone.AlphaSourceBlend;
		blendState.BlendFactor = blendStateToClone.BlendFactor;
		blendState.ColorBlendFunction = blendStateToClone.ColorBlendFunction;
		blendState.ColorDestinationBlend = blendStateToClone.ColorDestinationBlend;
		blendState.ColorSourceBlend = blendStateToClone.ColorSourceBlend;
		blendState.ColorWriteChannels = blendStateToClone.ColorWriteChannels;
		blendState.ColorWriteChannels1 = blendStateToClone.ColorWriteChannels1;
		blendState.ColorWriteChannels2 = blendStateToClone.ColorWriteChannels2;
		blendState.ColorWriteChannels3 = blendStateToClone.ColorWriteChannels3;
		blendState.MultiSampleMask = blendStateToClone.MultiSampleMask;
		blendState.Name = blendStateToClone.Name;
		blendState.Tag = blendStateToClone.Tag;
		return blendState;
	}

	/// <summary>
	/// Returns a clone of the given DepthStencilState.
	/// Read-only properties, such as the GraphicsDevice property, are not copied though.
	/// </summary>
	/// <param name="depthStencilStateToClone">The DepthStencilState to clone.</param>
	public static DepthStencilState CloneDepthStencilState(DepthStencilState depthStencilStateToClone)
	{
		DepthStencilState depthStencilState = new DepthStencilState();
		depthStencilState.CounterClockwiseStencilFail = depthStencilStateToClone.CounterClockwiseStencilFail;
		depthStencilState.CounterClockwiseStencilDepthBufferFail = depthStencilStateToClone.CounterClockwiseStencilDepthBufferFail;
		depthStencilState.CounterClockwiseStencilFunction = depthStencilStateToClone.CounterClockwiseStencilFunction;
		depthStencilState.CounterClockwiseStencilPass = depthStencilStateToClone.CounterClockwiseStencilPass;
		depthStencilState.DepthBufferEnable = depthStencilStateToClone.DepthBufferEnable;
		depthStencilState.DepthBufferFunction = depthStencilStateToClone.DepthBufferFunction;
		depthStencilState.DepthBufferWriteEnable = depthStencilStateToClone.DepthBufferWriteEnable;
		depthStencilState.Name = depthStencilStateToClone.Name;
		depthStencilState.ReferenceStencil = depthStencilStateToClone.ReferenceStencil;
		depthStencilState.StencilDepthBufferFail = depthStencilStateToClone.StencilDepthBufferFail;
		depthStencilState.StencilEnable = depthStencilStateToClone.StencilEnable;
		depthStencilState.StencilFail = depthStencilStateToClone.StencilFail;
		depthStencilState.StencilFunction = depthStencilStateToClone.StencilFunction;
		depthStencilState.StencilMask = depthStencilStateToClone.StencilMask;
		depthStencilState.StencilPass = depthStencilStateToClone.StencilPass;
		depthStencilState.StencilWriteMask = depthStencilStateToClone.StencilWriteMask;
		depthStencilState.Tag = depthStencilStateToClone.Tag;
		depthStencilState.TwoSidedStencilMode = depthStencilStateToClone.TwoSidedStencilMode;
		return depthStencilState;
	}

	/// <summary>
	/// Returns a clone of the given RasterizerState.
	/// Read-only properties, such as the GraphicsDevice property, are not copied though.
	/// </summary>
	/// <param name="rasterizerStateToClone">The RasterizerState to clone.</param>
	public static RasterizerState CloneRasterizerState(RasterizerState rasterizerStateToClone)
	{
		RasterizerState rasterizerState = new RasterizerState();
		rasterizerState.CullMode = rasterizerStateToClone.CullMode;
		rasterizerState.DepthBias = rasterizerStateToClone.DepthBias;
		rasterizerState.FillMode = rasterizerStateToClone.FillMode;
		rasterizerState.MultiSampleAntiAlias = rasterizerStateToClone.MultiSampleAntiAlias;
		rasterizerState.Name = rasterizerStateToClone.Name;
		rasterizerState.ScissorTestEnable = rasterizerStateToClone.ScissorTestEnable;
		rasterizerState.SlopeScaleDepthBias = rasterizerStateToClone.SlopeScaleDepthBias;
		rasterizerState.Tag = rasterizerStateToClone.Tag;
		return rasterizerState;
	}

	/// <summary>
	/// Returns a clone of the given SamplerState.
	/// Read-only properties, such as the GraphicsDevice property, are not copied though.
	/// </summary>
	/// <param name="samplerStateToClone">The SamplerState to clone.</param>
	public static SamplerState CloneSamplerState(SamplerState samplerStateToClone)
	{
		SamplerState samplerState = new SamplerState();
		samplerState.AddressU = samplerStateToClone.AddressU;
		samplerState.AddressV = samplerStateToClone.AddressV;
		samplerState.AddressW = samplerStateToClone.AddressW;
		samplerState.Filter = samplerStateToClone.Filter;
		samplerState.MaxAnisotropy = samplerStateToClone.MaxAnisotropy;
		samplerState.MaxMipLevel = samplerStateToClone.MaxMipLevel;
		samplerState.MipMapLevelOfDetailBias = samplerStateToClone.MipMapLevelOfDetailBias;
		samplerState.Name = samplerStateToClone.Name;
		samplerState.Tag = samplerStateToClone.Tag;
		return samplerState;
	}
}
