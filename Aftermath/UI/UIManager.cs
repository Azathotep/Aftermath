using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Rendering;

namespace Aftermath.UI
{
    class UIManager
    {
        List<Menu> _menus = new List<Menu>();
        Dictionary<string, Menu> _menuByName = new Dictionary<string, Menu>();

        public void RegisterMenu(string name, Menu menu)
        {
            _menus.Add(menu);
            _menuByName.Add(name, menu);
        }

        public Menu GetMenuByName(string menuName)
        {
            return _menuByName[menuName];
        }

        public void ShowMenu(string menuName)
        {
            GetMenuByName(menuName).IsVisible = true;
        }

        public void HideMenu(string menuName)
        {
            GetMenuByName(menuName).IsVisible = false;
        }

        internal void RenderUI(XnaRenderer _renderer)
        {
            foreach (Menu menu in _menus)
                if (menu.IsVisible)
                    menu.Render(_renderer);
        }
    }
}
