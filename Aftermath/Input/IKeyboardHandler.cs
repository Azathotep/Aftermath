using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aftermath.Input
{
    interface IKeyboardHandler
    {
        void Update();
        void RegisterKey(InputKey key, KeyPressHandler callback=null, int retriggerInterval=5);
    }

    enum InputKey
    {
        A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
        Escape,
        Left, Right, Up, Down,
        OemPeriod,
        Enter,
        OemComma
    }
}
