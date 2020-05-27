using System;

namespace Assets.Scripts.Slab.States
{
    public abstract class SlabState : IDisposable
    {
        public abstract void Update();

        public virtual void Start()
        {
            // optionally overridden
        }

        public virtual void Dispose()
        {
            // optionally overridden
        }
    }
}
