using Zenject;

namespace Assets.Scripts.Slab.States
{
    public class SlabStateStacked : SlabState
    {
        private readonly Slab slab;

        public SlabStateStacked(Slab slab)
        {
            this.slab = slab;
        }

        public override void Update()
        {
        }

        public class Factory : PlaceholderFactory<Slab, SlabStateStacked>
        {
        }
    }
}
