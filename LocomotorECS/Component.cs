using System;

using LocomotorECS;

namespace LocomotorECS
{
    using System;

    public class Component
    {
        public Entity Entity { get; internal set; }
        
        public bool Enabled
        {
            get => this.enabled;
            set
            {
                if (this.enabled == value)
                {
                    return;
                }

                this.enabled = value;
                
                if (this.enabled)
                    this.ComponentEnabled?.Invoke(this);
                else
                    this.ComponentDisabled?.Invoke(this);
            }
        }

        internal event Action<Component> ComponentEnabled;
        internal event Action<Component> ComponentDisabled;

        private bool enabled = true;
    }
}