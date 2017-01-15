using UnityEngine;
using System.Collections;
using Improbable.Unity.Visualizer;
using Improbable.Unity;
using Improbable.Ship;
using Improbable.Unity.Core;
using Improbable.Worker;

namespace Assets.Gamelogic.Player
{
    [EngineType(EnginePlatform.FSim)]
    public class PirateShipEntityLifecycleManager : MonoBehaviour
    {
        [Require]
        private Health.Reader HealthReader;

        private void OnEnable()
        {
            InvokeRepeating("CleanupAfterSinking", 30.0f, 30.0f);
        }

        private void OnDisable()
        {
            CancelInvoke("CleanupAfterSinking");
        }

        private void CleanupAfterSinking()
        {
            if (HealthReader.Data.currentHealth <= 0 && !gameObject.GetComponentInChildren<Animation>().isPlaying)
            {
                // Delete ship entity after sinking
                SpatialOS.WorkerCommands.DeleteEntity(gameObject.EntityId(), deleteResult => {
                    if (deleteResult.StatusCode != StatusCode.Success)
                    {
                        Debug.LogError("Failed to delete PirateShip entity with error: " + deleteResult.ErrorMessage);
                        return;
                    }
                    Debug.Log("Deleted PirateShip entity: " + deleteResult.Response.Value);
                });
            }
        }
    }
}
