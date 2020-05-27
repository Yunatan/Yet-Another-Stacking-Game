using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Slab.States
{
    public class SlabStateMoving : SlabState
    {
        private readonly Controls controls;
        private readonly SlabManager slabManager;
        private readonly SlabManager.Settings settings;
        private readonly Slab slab;
        private Vector3 movingDirection;

        public SlabStateMoving(SlabManager.Settings settings, Slab slab, Controls controls, SlabManager slabManager)
        {
            this.slab = slab;
            this.controls = controls;
            this.slabManager = slabManager;
            this.settings = settings;
        }

        public override void Update()
        {
            if (IsOutOfBounds(slab.Position))
            {
                movingDirection = GetMovingDirection(slab.Position);
            }

            //debug long-ass-tower
            //if (Vector3.Distance(slab.Position, new Vector3(0, slab.Position.y, 0)) <= 0.03)
            //{
            //    StopMoving(new InputAction.CallbackContext());
            //}

            slab.Position += movingDirection * Time.deltaTime * settings.moveSpeed;
        }

        private bool IsOutOfBounds(Vector3 pos)
        {
            return Vector3.Distance(pos, new Vector3(0, pos.y, 0)) >= settings.maximumDistance;
        }

        private Vector3 GetMovingDirection(Vector3 pos)
        {
            if (Mathf.Abs(pos.x) > Mathf.Abs(pos.z))
            {
                return new Vector3(pos.x, 0, 0).normalized * -1;
            }
            else
            {
                return new Vector3(0, 0, pos.z).normalized * -1;
            }
        }

        private void StopMoving(InputAction.CallbackContext context)
        {
            slabManager.TrySliceSlab(slab);
            slab.ChangeState(SlabStates.Stacked); 
        }

        public override void Start()
        {
            controls.Main.Stack.performed += StopMoving;
        }

        public override void Dispose()
        {
            controls.Main.Stack.performed -= StopMoving;
        }

        public class Factory : PlaceholderFactory<Slab, SlabStateMoving>
        {
        }
    }
}
