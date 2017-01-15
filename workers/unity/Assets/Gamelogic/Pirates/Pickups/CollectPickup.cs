using Improbable;
using Improbable.Objects;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Pickups
{
    [EngineType(EnginePlatform.FSim)]
    public class CollectPickup : MonoBehaviour
    {
        [Require]
        private Pickup.Writer PickupWriter;

        private void OnTriggerEnter(Collider other)
        {
            /*
             * Unity's OnTriggerEnter runs even if the MonoBehaviour is disabled, so non-authoritative FSims
             * must be protected against null writers
             */
            if (PickupWriter == null)
                return;

            // Collider is on the mesh which is a grandchild of the entity object
            if (other != null && other.transform.parent.parent.gameObject.IsEntityObject() && other.transform.parent.parent.gameObject.tag == "Ship")
            {
                // Get EntityId of ship
                var otherEntityId = other.gameObject.EntityId();

                if (PickupWriter.Data.pickupType == PickupType.SUPPLIES) {
                    // Only pickup supplies if ship is alive and health is not full
                    var query = Query.HasEntityId(otherEntityId).ReturnComponents(Health.ComponentId);
                    SpatialOS.Commands.SendQuery(PickupWriter, query, result => {
                        if (result.StatusCode != StatusCode.Success)
                        {
                            Debug.LogError("Failed to send query for ship health from AwardPickup with error: " + result.ErrorMessage);
                            return;
                        }
                        if (!result.Response.HasValue && result.Response.Value.EntityCount > 0)
                        {
                            Debug.LogError("Ship does not have health component, unable to pickup supplies");
                            return;
                        }

                        var healthData = result.Response.Value.Entities.First.Value.Value.Get<Health>().Value.Get().Value;
                        if (healthData.currentHealth > 0 && healthData.currentHealth < healthData.maxHealth)
                        {
                            SendAwardPickupCommand(otherEntityId);
                        }
                    });
                } else
                {
                    SendAwardPickupCommand(otherEntityId);
                }
            }
        }

        private void SendAwardPickupCommand(EntityId otherEntityId)
        {
            SpatialOS.Commands.SendCommand(PickupWriter, Pickup.Commands.AwardPickup.Descriptor, new AwardPickupRequest(PickupWriter.Data.pickupType, PickupWriter.Data.amount), otherEntityId, result => {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("Failed to send AwardPickup command with error: " + result.ErrorMessage);
                    return;
                }
                AwardPickupResponse response = result.Response.Value;
                Debug.Log("AwardPickup command succeeded; awarded pickup: " + response.ToString());

                // Delete pickup entity as it has been collected
                SpatialOS.Commands.DeleteEntity(PickupWriter, gameObject.EntityId(), deleteResult => {
                    if (deleteResult.StatusCode != StatusCode.Success)
                    {
                        Debug.LogError("Failed to delete Pickup entity with error: " + deleteResult.ErrorMessage);
                        return;
                    }
                    Debug.Log("Deleted Pickup entity: " + deleteResult.Response.Value);
                });
            });
        }
    }
}
