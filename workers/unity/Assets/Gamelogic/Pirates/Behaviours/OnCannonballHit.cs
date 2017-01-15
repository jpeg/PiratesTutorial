using Assets.EntityTemplates;
using Improbable;
using Improbable.Math;
using Improbable.Objects;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Cannons
{
    // Enable this MonoBehaviour on FSim (server-side) workers only
    [EngineType(EnginePlatform.FSim)]
    public class OnCannonballHit : MonoBehaviour
    {
        // Enable this MonoBehaviour only on the worker with write access for the entity's Health component
        [Require]
        private Health.Writer HealthWriter;

        private void OnTriggerEnter(Collider other)
        {
            /*
             * Unity's OnTriggerEnter runs even if the MonoBehaviour is disabled, so non-authoritative FSims
             * must be protected against null writers
             */
            if (HealthWriter == null)
                return;

            // Ignore collision if this ship is already dead
            if (HealthWriter.Data.currentHealth <= 0)
                return;

            if (other != null && other.gameObject.tag == "Cannonball")
            {
                // Reduce health of this entity when hit
                int newHealth = HealthWriter.Data.currentHealth - 200;
                HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));

                // Notify firer to increment score if this entity was killed
                if (newHealth <= 0)
                {
                    AwardPointsForKill(new EntityId(other.GetComponent<DestroyCannonball>().firerEntityId.Id));
                    SpawnPickup();
                }
            }
        }

        private void AwardPointsForKill(EntityId firerEntityId)
        {
            uint pointsToAward = 1;
            // Use Commands API to issue an AwardPoints request to the entity who fired the cannonball
            SpatialOS.Commands.SendCommand(HealthWriter, Score.Commands.AwardPoints.Descriptor, new AwardPoints(pointsToAward), firerEntityId, result =>
                {
                    if (result.StatusCode != StatusCode.Success)
                    {
                        Debug.LogError("Failed to send AwardPoints command with error: " + result.ErrorMessage);
                        return;
                    }
                    AwardResponse response = result.Response.Value;
                    Debug.Log("AwardPoints command succeeded; awarded points: " + response.ToString());
                }
            );
        }

        private void SpawnPickup()
        {
            string prefab;
            PickupType type;
            int amount;
            var rand = Random.value;

            // Determine pickup type and amount
            if (rand >= 0.0f && rand <= 1.0f)
            {
                prefab = "Supplies";
                type = PickupType.SUPPLIES;
                amount = 400;
            } else if (rand > 1.0) //gold not yet supported
            {
                prefab = "Gold";
                type = PickupType.GOLD;
                amount = Random.Range(10, 50);
            } else
            {
                // Spawn nothing
                return;
            }

            var position = new Coordinates(this.transform.position.x, this.transform.position.y, this.transform.position.z);

            SpatialOS.Commands.CreateEntity(HealthWriter, prefab, PickupEntityTemplate.GeneratePickupEntityTemplate(position, type, amount), result => {
                if (result.StatusCode != StatusCode.Success)
                {
                    Debug.LogError("Failed to create " + prefab + " pickup entity with error: " + result.ErrorMessage);
                    return;
                }
                Debug.Log("Created " + prefab + " pickup entity with Id: " + result.Response.Value);
            });
        }
    }
}
