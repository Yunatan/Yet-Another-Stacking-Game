using Assets.Scripts.Slab.States;
using ModestTree;

namespace Assets.Scripts.Slab
{
    public enum SlabStates
    {
        Moving,
        Stacked,
        Falling
    }

    public class SlabStateFactory
    {
        private readonly SlabStateFalling.Factory fallingFactory;
        private readonly SlabStateMoving.Factory movingFactory;
        private readonly SlabStateStacked.Factory stackedFactory;

        public SlabStateFactory(SlabStateFalling.Factory fallingFactory, SlabStateMoving.Factory movingFactory, SlabStateStacked.Factory stackedFactory)
        {
            this.fallingFactory = fallingFactory;
            this.movingFactory = movingFactory;
            this.stackedFactory = stackedFactory;
        }

        public SlabState CreateState(SlabStates state, Slab slab)
        {
            switch (state)
            {
                case SlabStates.Falling:
                    {
                        return fallingFactory.Create(slab);
                    }
                case SlabStates.Stacked:
                    {
                        return stackedFactory.Create(slab);
                    }
                case SlabStates.Moving:
                    {
                        return movingFactory.Create(slab);
                    }
            }

            throw Assert.CreateException();
        }
    }
}
