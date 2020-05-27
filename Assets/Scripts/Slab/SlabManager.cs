using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Slab.Slab;
using static Assets.Scripts.Util.GameEvents;

namespace Assets.Scripts.Slab
{
    public class SlabManager : ITickable, IInitializable, IDisposable
    {
        private readonly List<Slab> slabs = new List<Slab>();
        private readonly SlabFactory slabFactory;
        private readonly Slab initialSlab;
        private readonly SignalBus signalBus;
        private readonly Settings settings;
        private readonly Camera mainCamera;
        private Vector3 defaultCameraPosition;
        private float defaultCameraOrthographicSize;
        private readonly Vector3[] sides = new[] { Vector3.back, Vector3.forward, Vector3.left, Vector3.right };
        private bool started;

        public SlabManager(Settings settings, SlabFactory slabFactory, Slab initialSlab, Camera mainCamera, SignalBus signalBus)
        {
            this.settings = settings;
            this.slabFactory = slabFactory;
            this.initialSlab = initialSlab;
            this.mainCamera = mainCamera;
            this.signalBus = signalBus;
        }

        public void Tick()
        {
            if (started)
            {
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(
                    mainCamera.transform.position.x,
                    slabs.Last().Position.y + 4f, 
                    mainCamera.transform.position.z),
                    0.1f); 
            }
        }

        public void Initialize()
        {
            initialSlab.Color = GetRandomColor();
            defaultCameraPosition = mainCamera.transform.position;
            defaultCameraOrthographicSize = mainCamera.orthographicSize;

            signalBus.Subscribe<GameStartSignal>(Start);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<GameStartSignal>(Start);
        }

        public void Start()
        {
            Assert.That(!started);
            started = true;
            ResetAll();
            SpawnNext();
        }

        public void Stop()
        {
            started = false;
            ShowFullTower();
            signalBus.Fire<GameOverSignal>();
        }

        private void ShowFullTower()
        {
            var towerBounds = new Bounds();
            towerBounds.Encapsulate(GetPreviousSlab().GetComponent<BoxCollider>().bounds);
            towerBounds.Encapsulate(slabs.First().GetComponent<BoxCollider>().bounds);

            float cameraDistance = mainCamera.orthographicSize;
            Vector3 objectSizes = towerBounds.max - towerBounds.min;
            float objectSize = Mathf.Max(objectSizes.x, objectSizes.y, objectSizes.z);
            float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * mainCamera.fieldOfView); // Visible height 1 meter in front
            float distance = cameraDistance * objectSize / cameraView; // Combined wanted distance from the object
            distance += 0.5f * objectSize; // Estimated offset from the center to the outside of the object
            mainCamera.transform.position = towerBounds.center - distance * mainCamera.transform.forward;
            mainCamera.orthographicSize = Math.Max(towerBounds.size.y / Mathf.Tan(mainCamera.fieldOfView * Mathf.Deg2Rad), 2);
        }

        private void ResetAll()
        {
            if (slabs.Any())
            {
                slabs.RemoveAt(0);
            }

            foreach (var slab in slabs)
            {
                GameObject.Destroy(slab.gameObject);
            }

            slabs.Clear();
            slabs.Add(initialSlab);

            mainCamera.transform.position = defaultCameraPosition;
            mainCamera.orthographicSize = defaultCameraOrthographicSize;
            initialSlab.Color = GetRandomColor();
        }

        public Slab GetPreviousSlab()
        {
            if (slabs.Count >= 2)
            {
                return slabs[slabs.Count - 2];
            }
            else
            {
                return slabs.Single();
            }
        }

        public void SpawnNext()
        {
            var slab = slabFactory.Create();
            slabs.Add(slab);
            var previousSlab = GetPreviousSlab();
            slab.Position = GetStartPosition(slab.Position, previousSlab.Position);
            slab.Scale = new Vector3(previousSlab.Scale.x, slab.Scale.y, previousSlab.Scale.z);
            slab.Color = Color.Lerp(previousSlab.Color, GetRandomColor(), 0.15f);
        }

        public void TrySliceSlab(Slab slab)
        {
            var previousSlab = GetPreviousSlab();

            if (FastApproximately(slab.Position.z, previousSlab.Position.z, settings.snapThreshold) 
                && FastApproximately(slab.Position.x, previousSlab.Position.x, settings.snapThreshold))
            {
                slab.Position = new Vector3(previousSlab.Position.x, slab.Position.y, previousSlab.Position.z);
                signalBus.Fire<PerfectStackSignal>();
                SpawnNext();
                return;
            }

            var slip = slab.Position - previousSlab.Position;
            slip.y = 0;
            var overhangX = Mathf.Abs(slip.x);
            var overhangZ = Mathf.Abs(slip.z);

            if (overhangX >= slab.Scale.x || overhangZ >= slab.Scale.z)
            {
                slab.ChangeState(SlabStates.Falling);
                Stop();
                return;
            }

            var newScaleX = previousSlab.Scale.x - overhangX;
            var newScaleZ = previousSlab.Scale.z - overhangZ;
            var newPositionX = previousSlab.Position.x + slip.x / 2;
            var newPositionZ = previousSlab.Position.z + slip.z / 2;

            var fallingSlabScaleX = overhangX == 0 ? slab.Scale.x : overhangX;
            var fallingSlabScaleZ = overhangZ == 0 ? slab.Scale.z : overhangZ;
            var fallingSlabPositionX = slab.Position.x + (newScaleX / 2 * slip.normalized.x);
            var fallingSlabPositionZ = slab.Position.z + (newScaleZ / 2 * slip.normalized.z);

            slab.Scale = new Vector3(newScaleX, slab.Scale.y, newScaleZ);
            slab.Position = new Vector3(newPositionX, slab.Position.y, newPositionZ);

            var fallingSlab = slabFactory.Create();
            fallingSlab.Position = new Vector3(fallingSlabPositionX, slab.Position.y, fallingSlabPositionZ);
            fallingSlab.Scale = new Vector3(fallingSlabScaleX, slab.Scale.y, fallingSlabScaleZ);
            fallingSlab.Color = slab.Color;
            fallingSlab.StartingState = SlabStates.Falling;
            UnityEngine.Object.Destroy(fallingSlab.gameObject, 1f);

            signalBus.Fire<StackSignal>();
            SpawnNext();
        }

        private static bool FastApproximately(float a, float b, float threshold)
        {
            if (threshold > 0f)
            {
                return Mathf.Abs(a - b) <= threshold;
            }
            else
            {
                return Mathf.Approximately(a, b);
            }
        }

        private static Color GetRandomColor()
        {
            return UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        }

        private Vector3 GetStartPosition(Vector3 pos, Vector3 prevPos)
        {
            var newPos = sides[UnityEngine.Random.Range(0, sides.Length)] * settings.maximumDistance;
            var yPos = pos.y + 0.1f * (slabs.Count - 2);
            return new Vector3(newPos.x != 0 ? newPos.x : prevPos.x, yPos, newPos.z != 0 ? newPos.z : prevPos.z);
        }

        [Serializable]
        public class Settings
        {
            public float maximumDistance;
            public float moveSpeed;
            public float snapThreshold;
        }
    }
}