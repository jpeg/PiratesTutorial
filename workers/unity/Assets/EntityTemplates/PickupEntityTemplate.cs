using UnityEngine;
using Improbable.General;
using Improbable.Math;
using Improbable.Objects;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;

namespace Assets.EntityTemplates
{
    public class PickupEntityTemplate : MonoBehaviour
    {
        static public Entity GeneratePickupEntityTemplate(Coordinates initialPosition, PickupType type, int amount)
        {
            var pickupEntityTemplate = new Entity();

            pickupEntityTemplate.Add(new WorldTransform.Data(new WorldTransformData(initialPosition, 0)));
            pickupEntityTemplate.Add(new Pickup.Data(new PickupData(type, amount)));

            var acl = Acl.GenerateServerAuthoritativeAcl(pickupEntityTemplate);
            pickupEntityTemplate.SetAcl(acl);

            return pickupEntityTemplate;
        }
    }
}
