namespace LocomotorECS
{
    using System;
    using System.Collections.Generic;

    public class Entity
    {
        #region properties and fields
        
        public string Name;
        
        internal event Action<Entity> BeforeTagChanged;

        internal event Action<Entity> AfterTagChanged;

        public int Tag
        {
            get => this.tag;
            set
            {
                if (this.tag == value)
                {
                    return;
                }

                this.BeforeTagChanged?.Invoke(this);
                this.tag = value;
                this.AfterTagChanged?.Invoke(this);
            }
        }

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
                    this.Components.EnableEntity();
                else
                    this.Components.DisableEntity();
            }
        }


        public CacheData Cache { get; } = new CacheData();
        public ComponentList Components { get; }

        private int tag;
        private bool enabled = true;

        #endregion

        public Entity()
        {
            this.Components = new ComponentList(this);
        }

        public Entity(string name) : this()
        {
            this.Name = name;
        }
        
        #region Component Management

        public T AddComponent<T>( T component ) where T : Component
        {
            return this.Components.Add( component );
        }

        public T AddComponent<T>() where T : Component, new()
        {
            return this.Components.Add<T>();
        }

        public T GetComponent<T>(bool withPending = true) where T : Component
        {
            return this.Components.Get<T>(withPending);
        }

        public T GetOrCreateComponent<T>() where T : Component, new()
        {
            return this.Components.GetOrCreate<T>();
        }

        public void RemoveComponent<T>() where T : Component
        {
            this.Components.Remove<T>();
        }

        public void RemoveComponent( Component component )
        {
            this.Components.Remove( component );
        }

        #endregion

        public class CacheData
        {
            private readonly Dictionary<string, object> data = new Dictionary<string, object>();

            public void PutData<T>(string key, T value)
            {
                this.data[key] = value;
            }

            public T GetData<T>(string key, T defaultValue = default(T))
            {
                return this.data.ContainsKey(key) ? (T)this.data[key] : defaultValue;
            }

            public T ReplaceData<T>(string key, T value, T defaultValue = default(T))
            {
                var oldData = this.GetData(key, defaultValue);
                this.PutData(key, value);
                return oldData;
            }

            public T GetOrAddData<T>(string key, Func<T> builder)
            {
                if (this.data.ContainsKey(key))
                {
                    return (T)this.data[key];
                }

                var value = builder();
                this.data[key] = value;
                return value;
            }
        }
    }
}

