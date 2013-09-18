using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;

namespace Aftermath.Animations
{
    /// <summary>
    /// Base class for all animations
    /// </summary>
    abstract class Animation
    {
        /// <summary>
        /// Called to update the animation
        /// </summary>
        /// <returns>Animation should return true to indicate the animation is still running. 
        /// False to indicate the animation has ended.</returns>
        public abstract bool Update();

        /// <summary>
        /// Called each frame to render the animation
        /// </summary>
        public abstract void Render(XnaRenderer renderer);

        //TODO all animations currently block until they end. Add non-blocking animations?
        //bool _isBlocking = false;
        //public bool IsBlocking
        //{
        //    get
        //    {
        //        return _isBlocking;
        //    }
        //}
    }
}
