using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aftermath.Core;
using Aftermath.Animations;
using Aftermath.Rendering;

namespace Aftermath.Map
{
    class Door : Structure
    {
        bool _isOpen = true;

        public override bool BlocksLight
        {
            get
            {
                return !_isOpen;
            }
        }

        public override bool IsSolid
        {
            get
            {
                return !_isOpen;
            }
        }

        public override GameTexture Texture
        {
            get 
            {
                if (_isOpen)
                    return Engine.GetTexture("house.opendoor");
                else
                    return Engine.GetTexture("house.shutdoor");
            }
        }

        public bool IsOpen
        {
            get
            {
                if (_health <= 0)
                    return true;
                return _isOpen;
            }
        }

        public void Open()
        {
            if (_health <= 0)
                return;
            _isOpen = true;
        }

        public void Close()
        {
            if (_health <= 0)
                return;
            _isOpen = false;
        }

        public override void Damage(short damageAmount)
        {
            if (_health <= 0)
                return;
            _health -= damageAmount;
            Engine.Instance.AnimationManager.StartAnimation(new BashAnimation(Location));
            if (_health <= 0)
            {
                _isOpen = true;
                _health = 0;
            }
        }

        public override StructureType Type
        {
            get { return StructureType.Door; }
        }

        public override void Serialize(System.IO.BinaryWriter bw)
        {
            base.Serialize(bw);
            bw.Write((bool)_isOpen);
        }

        public override void Deserialize(System.IO.BinaryReader br)
        {
            base.Deserialize(br);
            _isOpen = br.ReadBoolean();
        }
    }
}
