using Assets.Scripts.Slab.States;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Slab
{
    public class Slab : MonoBehaviour
    {
        [SerializeField]
        private SlabStates startingState;
        private SlabState state;
        private SlabStateFactory stateFactory;
        private Rigidbody rigidBody;
        private MeshRenderer meshRenderer;

        [Inject]
        public void Construct(SlabStateFactory stateFactory)
        {
            this.stateFactory = stateFactory;
            rigidBody = GetComponent<Rigidbody>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public SlabStates StartingState { set => startingState = value; }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }

        public Vector3 Scale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        public Color Color 
        { 
            get { return meshRenderer.material.color; } 
            set { meshRenderer.material.color = value; } 
        }

        public void Start()
        {
            ChangeState(startingState);
        }

        public void Update()
        {
            state.Update();
        }

        public void ChangeState(SlabStates newState)
        {
            if (state != null)
            {
                state.Dispose();
                state = null;
            }

            state = stateFactory.CreateState(newState, this);
            state.Start();
        }

        public class SlabFactory : PlaceholderFactory<Slab>
        {
        }
    }
}