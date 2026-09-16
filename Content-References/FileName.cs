using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FNA.Contents
{
    public static class RenderStateHelper
    {
        public static void SetBlendState(
            GraphicsDevice device,
            Blend source,
            Blend destination,
            BlendFunction function = BlendFunction.Add)
        {
            device.BlendState = new BlendState
            {
                ColorSourceBlend = source,
                ColorDestinationBlend = destination,
                ColorBlendFunction = function,

                AlphaSourceBlend = source,
                AlphaDestinationBlend = destination,
                AlphaBlendFunction = function
            };
        }

        public static void SetAlphaBlendState(
            GraphicsDevice device,
            Blend source,
            Blend destination,
            Blend alphaSource,
            Blend alphaDestination,
            BlendFunction function = BlendFunction.Add,
            BlendFunction alphaFunction = BlendFunction.Add)
        {
            device.BlendState = new BlendState
            {
                ColorSourceBlend = source,
                ColorDestinationBlend = destination,
                ColorBlendFunction = function,

                AlphaSourceBlend = alphaSource,
                AlphaDestinationBlend = alphaDestination,
                AlphaBlendFunction = alphaFunction
            };
        }

        public static void SetColorWriteChannels(
            GraphicsDevice device,
            ColorWriteChannels channels)
        {
            device.BlendState = new BlendState
            {
                ColorWriteChannels = channels
            };
        }

        public static void SetBlendFactor(
            GraphicsDevice device,
            Color blendFactor)
        {
            device.BlendState = new BlendState
            {
                BlendFactor = blendFactor
            };
        }
    }
}
