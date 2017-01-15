using UnityEngine;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.Gamelogic.Pirates.Behaviors
{
    // Enable this MonoBehaviour on FSim (server-side) workers only
    [EngineType(EnginePlatform.FSim)]
    public class SteerRandomly : MonoBehaviour
    {
        /*
         * An entity with this MonoBehaviour will only be enabled for the single FSim worker
         * which has write-access for its WorldTransform component.
         */
        [Require]
        private ShipControls.Writer ShipControlsWriter;

        [Require]
        private Health.Reader HealthReader;

        private void OnEnable()
        {
            // Change steering decisions every five seconds
            InvokeRepeating("RandomizeSteering", 0, 5.0f);

            HealthReader.ComponentUpdated += StopIfDead;
        }

        private void OnDisable()
        {
            CancelInvoke("RandomizeSteering");

            HealthReader.ComponentUpdated -= StopIfDead;
        }

        private void RandomizeSteering()
        {
            if (HealthReader.Data.currentHealth > 0)
            {
                ShipControlsWriter.Send(new ShipControls.Update()
                    .SetTargetSpeed(Random.value * 0.75f)
                    .SetTargetSteering((Random.value * 30.0f) - 15.0f));
            } else
            {
                ShipControlsWriter.Send(new ShipControls.Update()
                    .SetTargetSpeed(0.0f)
                    .SetTargetSteering(0.0f));
            }
        }

        private void StopIfDead(Health.Update update)
        {
            if (update.currentHealth.HasValue && update.currentHealth.Value <= 0)
                ShipControlsWriter.Send(new ShipControls.Update()
                    .SetTargetSpeed(0.0f)
                    .SetTargetSteering(0.0f));
        }
    }
}
