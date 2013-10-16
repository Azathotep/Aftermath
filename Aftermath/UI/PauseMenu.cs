using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;
using Aftermath.Input;
using Aftermath.State;
using Aftermath.Core;
using Aftermath.Utils;

namespace Aftermath.UI
{
    class PauseMenu : Dialog
    {
        float _selectionRotate;
        int _selectedItem;
        public override void Render(XnaRenderer renderer)
        {
            renderer.Draw(new GameTexture("pausemenu", new RectangleI(0, 0, 220, 220)), new RectangleF(300, 200, 200, 200), 0.5f, 0, new Vector2F(0f, 0f));

            renderer.Draw(new GameTexture("bullet", new RectangleI(0, 0, 32, 32)), new RectangleF(345, 292 + _selectedItem * 32, 20, 20), 0.2f, _selectionRotate, new Vector2F(0.5f, 0.5f));
            _selectionRotate += 0.05f;
        }

        public override void ProcessKey(InputKey key)
        {
            switch (key)
            {
                case InputKey.Up:
                case InputKey.Left:
                    _selectedItem = Math.Max(_selectedItem - 1, 0);
                    break;
                case InputKey.Down:
                case InputKey.Right:
                    _selectedItem = Math.Min(_selectedItem + 1, 1);
                    break;
                case InputKey.Escape:
                    GameState.CurrentState = GameState.MovementState;
                    break;
                case InputKey.Enter:
                    switch (_selectedItem)
                    {
                        case 0:
                            Close();
                            break;
                        case 1:
                            Engine.Instance.Exit();
                            break;
                    }
                    break;
            }
        }
    }
}
