using Improbable.Entity.Component;
using Improbable.Objects;
using Improbable.Ship;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic.Pirates.Pickups
{
    [EngineType(EnginePlatform.FSim)]
    public class ReceivePickup : MonoBehaviour
    {
        [Require]
        private Pickup.Writer PickupWriter;

        [Require]
        private Health.Writer HealthWriter;

        void OnEnable()
        {
            PickupWriter.CommandReceiver.OnAwardPickup += OnAwardPickup;
        }

        private void OnDisable()
        {
            PickupWriter.CommandReceiver.OnAwardPickup -= OnAwardPickup;
        }

        void OnAwardPickup(ResponseHandle<Pickup.Commands.AwardPickup, AwardPickupRequest, AwardPickupResponse> responseHandle)
        {
            switch (responseHandle.Request.pickupType)
            {
                case PickupType.SUPPLIES:
                    var currentHealth = HealthWriter.Data.currentHealth;
                    var maxHealth = HealthWriter.Data.maxHealth;

                    if (currentHealth > 0 && currentHealth < maxHealth)
                    {
                        var newHealth = currentHealth + responseHandle.Request.amount > maxHealth ? maxHealth : currentHealth + responseHandle.Request.amount;
                        HealthWriter.Send(new Health.Update().SetCurrentHealth(newHealth));
                    }

                    break;

                case PickupType.GOLD:
                    //TODO
                    break;

                default:
                    Debug.LogError("Unkown pickup type");
                    break;
            }

            responseHandle.Respond(new AwardPickupResponse());
        }
    }
}
