using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Aftermath.Input
{
    /// <summary>
    /// Manages keyboard state
    /// </summary>
    class XnaKeyboardHandler : IKeyboardHandler
    {
        List<KeyState> _registeredKeys = new List<KeyState>();
        KeyPressHandler _keyPressHandler;

        public XnaKeyboardHandler(KeyPressHandler handler)
        {
            _keyPressHandler = handler;
        }

        public void Update()
        {
            KeyboardState kbs = Keyboard.GetState();
            bool isShiftDown = kbs.IsKeyDown(Keys.LeftShift) || kbs.IsKeyDown(Keys.RightShift);
            foreach (KeyState keyHandler in _registeredKeys)
                keyHandler.Update(kbs.IsKeyDown(keyHandler.XnaKey), isShiftDown);
        }

        public void RegisterKey(InputKey key, KeyPressHandler callback=null, int retriggerInterval=5)
        {
            if (callback == null)
                callback = _keyPressHandler;
            Keys xnaKey;
            bool shiftDown = false;
            if (!Enum.TryParse<Keys>(Enum.GetName(typeof(InputKey), key), out xnaKey))
            {
                throw new Exception();
            }
            _registeredKeys.Add(new KeyState(key, xnaKey, callback, shiftDown, retriggerInterval));
        }
    }

    delegate void KeyPressHandler(InputKey key);

    class KeyState
    {
        InputKey _key;
        Keys _xnaKey;
        KeyPressHandler _callback;
        bool _latch = false;
        int _delay;
        int _triggerTime;
        bool _needShift;

        public KeyState(InputKey key, Keys xnaKey, KeyPressHandler callback, bool shift, int delay = 0)
        {
            _key = key;
            _xnaKey = xnaKey;
            _callback = callback;
            _delay = delay;
            _needShift = shift;
            _triggerTime = _delay * 5;
        }

        public Keys XnaKey
        {
            get
            {
                return _xnaKey;
            }
        }

        int _elapsedTime = 0;

        public void Update(bool keyDown, bool shiftDown)
        {
            bool keyIsDown;
            if (_needShift)
                keyIsDown = keyDown && shiftDown;
            else
                keyIsDown = keyDown;
            if (keyIsDown)
            {
                if (_latch)
                {
                    _elapsedTime++;
                    if (_elapsedTime > _triggerTime)
                    {
                        _triggerTime = _delay;
                        _elapsedTime = 0;
                        _callback.Invoke(_key);
                    }
                }
                else
                {
                    _latch = true;
                    _elapsedTime = 0;
                    _callback.Invoke(_key);
                }
            }
            else //key is up
            {
                _triggerTime = _delay * 5;
                if (_latch)
                    _latch = false;
            }
        }
    }
}
