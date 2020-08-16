namespace LocomotorECS
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using LocomotorECS.Matching;

    public class ComponentList
    {
        private readonly Entity entity;

        private readonly Dictionary<int, Component> components = new Dictionary<int, Component>();

        private readonly Dictionary<int, Component> componentsToAdd = new Dictionary<int, Component>();

        private readonly HashSet<int> componentIdxToRemove = new HashSet<int>();

        private readonly HashSet<int> componentIdxToDisable = new HashSet<int>();

        private readonly HashSet<int> componentIdxToEnable = new HashSet<int>();

        internal BitSet Bits = new BitSet();

        public ComponentList( Entity entity )
        {
            this.entity = entity;
        }

        public T Add<T>() where T : Component, new()
        {
            return this.Add(new T());
        }

        public T Add<T>( T component ) where T : Component
        {
            var idx = ComponentTypeManager.GetIndexFor(component.GetType());
            this.componentsToAdd.Add(idx, component);
            return component;
        }

        public void Remove<T>() where T : Component
        {
            var idx = ComponentTypeManager.GetIndexFor(typeof(T));
            this.componentsToAdd.Remove(idx);
            this.componentIdxToRemove.Add(idx);
        }

        public void Remove(Component component)
        {
            var idx = ComponentTypeManager.GetIndexFor(component.GetType());
            this.componentsToAdd.Remove(idx);
            this.componentIdxToRemove.Add(idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get<T>(bool withPending = true) where T : Component
        {
            var idx = ComponentTypeManager.GetIndexFor(typeof(T));
            if (this.components.ContainsKey(idx))
            {
                return (T)this.components[idx];
            }

            if (withPending && this.componentsToAdd.ContainsKey(idx))
            {
                return (T)this.componentsToAdd[idx];
            }

            return null;
        }

        public T GetOrCreate<T>()
            where T : Component, new()
        {
            var comp = this.Get<T>();
            if (comp == null)
            {
                comp = this.Add<T>();
            }

            return comp;
        }

        internal bool CommitChanges()
        {
            if (this.componentIdxToDisable.Count > 0)
            {
                foreach (var idx in this.componentIdxToDisable)
                {
                    this.Bits.Set(idx, false);
                }
            }

            if (this.componentIdxToEnable.Count > 0)
            {
                if (!this.entity.Enabled)
                {
                    this.componentIdxToEnable.Clear();
                }

                foreach (var idx in this.componentIdxToEnable)
                {
                    this.Bits.Set(idx, true);
                }
            }

            if (this.componentIdxToRemove.Count > 0)
            {
                foreach (var idx in this.componentIdxToRemove)
                {
                    this.Bits.Set(idx, false);

                    if (this.components.ContainsKey(idx))
                    {
                        var component = this.components[idx];
                        component.Entity = null;
                        component.ComponentEnabled -= this.ComponentEnabled;
                        component.ComponentDisabled -= this.ComponentDisabled;
                        this.components.Remove(idx);
                    }
                }
            }

            if (this.componentsToAdd.Count > 0)
            {
                foreach (var component in this.componentsToAdd)
                {
                    var idx = ComponentTypeManager.GetIndexFor(component.Value.GetType());
                    this.Bits.Set(idx, this.entity.Enabled && component.Value.Enabled);

                    component.Value.Entity = this.entity;
                    component.Value.ComponentEnabled += this.ComponentEnabled;
                    component.Value.ComponentDisabled += this.ComponentDisabled;
                    this.components.Add(idx, component.Value);
                }
            }

            var isSomethingChanged = this.componentIdxToRemove.Count > 0 || this.componentsToAdd.Count > 0
                                                                         || this.componentIdxToDisable.Count > 0
                                                                         || this.componentIdxToEnable.Count > 0;

            this.componentIdxToRemove.Clear();
            this.componentsToAdd.Clear();
            this.componentIdxToDisable.Clear();
            this.componentIdxToEnable.Clear();

            return isSomethingChanged;
        }

        internal void DisableEntity()
        {
            this.componentIdxToEnable.Clear();

            foreach (var component in this.components)
            {
                this.componentIdxToDisable.Add(component.Key);
            }
        }

        internal void EnableEntity()
        {
            foreach (var component in this.components)
            {
                if (component.Value.Enabled)
                {
                    this.componentIdxToEnable.Add(component.Key);
                    this.componentIdxToDisable.Remove(component.Key);
                }
            }
        }

        private void ComponentDisabled(Component component)
        {
            var idx = ComponentTypeManager.GetIndexFor(component.GetType());
            this.componentIdxToEnable.Remove(idx);
            this.componentIdxToDisable.Add(idx);
        }

        private void ComponentEnabled(Component component)
        {
            var idx = ComponentTypeManager.GetIndexFor(component.GetType());
            this.componentIdxToDisable.Remove(idx);
            this.componentIdxToEnable.Add(idx);
        }
    }
}
