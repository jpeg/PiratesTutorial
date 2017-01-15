using UnityEngine;
using Improbable.General;
using Improbable.Math;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Pirates.Pickups
{
    public class SyncPosition : MonoBehaviour
    {
        [Require]
        private WorldTransform.Reader WorldTransformReader;

        void OnEnable()
        {
            transform.position = WorldTransformReader.Data.position.ToVector3();
            transform.rotation = Quaternion.Euler(0.0f, WorldTransformReader.Data.rotation, 0.0f);

            WorldTransformReader.ComponentUpdated += OnComponentUpdated;
        }

        void OnDisable()
        {
            WorldTransformReader.ComponentUpdated -= OnComponentUpdated;
        }

        void OnComponentUpdated(WorldTransform.Update update)
        {
            if (update.position.HasValue)
                transform.position = update.position.Value.ToVector3();
            if (update.rotation.HasValue)
                transform.rotation = Quaternion.Euler(0.0f, update.rotation.Value, 0.0f);
        }
    }

    public static class CoordinatesExtensions
    {
        public static Vector3 ToVector3(this Coordinates coordinates)
        {
            return new Vector3((float)coordinates.X, (float)coordinates.Y, (float)coordinates.Z);
        }
    }
}
