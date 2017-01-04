using System;
using System.Collections.Generic;
using UnityEngine;
using Improbable;
using Improbable.General;
using Improbable.Math;
using Improbable.Ship;
using Improbable.Unity.Core.Acls;
using Improbable.Worker;
using Random = UnityEngine.Random;

namespace Assets.EntityTemplates
{
    public class PirateEntityTemplate : MonoBehaviour
    {
        private static int spawnDiameter = 300;
        private static int totalPirates = 50;

        // Template definition for a PlayerSpawner snapshot entity
        public static SnapshotEntity GeneratePirateSnapshotEntityTemplate()
        {
            // Choose a starting position for this pirate entity
            Coordinates pirateCoordinates = new Coordinates((Random.value - 0.5) * spawnDiameter, 0,
                (Random.value - 0.5) * spawnDiameter);
            uint pirateRotation = Convert.ToUInt32(Random.value * 360);

            // Set name of Unity prefab associated with this entity
            var pirateSnapshotEntity = new SnapshotEntity { Prefab = "PirateShip" };

            // Define components attached to snapshot entity
            pirateSnapshotEntity.Add(new WorldTransform.Data(new WorldTransformData(pirateCoordinates, pirateRotation)));
            pirateSnapshotEntity.Add(new Health.Data(new HealthData(500)));
            pirateSnapshotEntity.Add(new ShipControls.Data(new ShipControlsData(0, 0)));

            // Grant FSim (server-side) workers write-access over all of this entity's components, read-access for visual (e.g. client) workers
            var acl = Acl.GenerateServerAuthoritativeAcl(pirateSnapshotEntity);
            pirateSnapshotEntity.SetAcl(acl);

            return pirateSnapshotEntity;
        }

        public static void PopulateSnapshotWithPirateEntities(ref Dictionary<EntityId, SnapshotEntity> snapshotEntities, ref int nextAvailableId)
        {
            for (var i = 0; i < totalPirates; i++)
            {
                snapshotEntities.Add(new EntityId(nextAvailableId++), GeneratePirateSnapshotEntityTemplate());
            }
        }
    }
}
