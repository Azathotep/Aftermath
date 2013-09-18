using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;

namespace Aftermath.Animations
{
    /// <summary>
    /// Manages animations. Animations occur between turns to highlight what has happened. Eg if the player fires a 
    /// gun at a zombie there will be a muzzleflash animation and an impact animation on the zombie. The turn system pauses
    /// until the animations complete (they are very short)
    /// </summary>
    class AnimationManager : ITurnInihibitor
    {
        List<Animation> _activeAnimations = new List<Animation>();
        List<Animation> _animationsToRemove = new List<Animation>();

        /// <summary>
        /// Called by the game engine each frame to update all animations
        /// </summary>
        public void Update()
        {
            while (_activeAnimations.Count > 0)
            {
                bool anyAnimationsVisible = false;
                _animationsToRemove.Clear();
                foreach (Animation animation in _activeAnimations.ToArray())
                {
                    //update animation. If the animation has reached its end then remove it from
                    //active animations
                    bool stillRunning = animation.Update();
                    if (!stillRunning)
                        _animationsToRemove.Add(animation);
                    //if (animation.IsVisible(BaseEngine.Instance.Player.GetVisibleTiles()))
                    //TODO will need to handle animations that are not visible
                    anyAnimationsVisible = true;   
                }
                foreach (Animation animation in _animationsToRemove)
                    _activeAnimations.Remove(animation);

                //if any animations are visible then break to allow them to be drawn frame by frame
                //(if none are visible can just advance until they all complete)
                if (anyAnimationsVisible)
                    break;
            }
        }

        #region ITurnInhibitor
        /// <summary>
        /// Returns whether the turn system should be paused due to running animations
        /// </summary>
        public bool IsBlocking
        {
            get 
            {
                return _activeAnimations.Count > 0;
            }
        }
        #endregion

        /// <summary>
        /// Returns list of animations currently active
        /// </summary>
        public IEnumerable<Animation> Animations 
        {
            get
            {
                return _activeAnimations;
            }
        }

        /// <summary>
        /// Starts a new animation
        /// </summary>
        internal void StartAnimation(Animation animation)
        {
            _activeAnimations.Add(animation);
        }
    }
}
