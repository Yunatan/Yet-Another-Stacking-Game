using UnityEngine;
using Zenject;

namespace Assets.Scripts.Slab.States
{
    public class SlabStateFalling : SlabState
    {
        private readonly Slab slab;

        public SlabStateFalling(Slab slab)
        {
            this.slab = slab;
        }

        public override void Update()
        {
        }

        public override void Start()
        {
            slab.gameObject.AddComponent<Rigidbody>();
        }

        public class Factory : PlaceholderFactory<Slab, SlabStateFalling>
        {
        }
    }
}
