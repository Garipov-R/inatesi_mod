using InatesiCharacter.Testing.Pooling;
using Leopotam.EcsLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace InatesiCharacter.Testing.LeoEcs4.PoolSystems
{
    public static class SendEventObjectPool
    {
        public static void Send(EcsWorld ecsWorld, GameObject spawnObject, Vector3 position, Quaternion rotation, PoolType poolType = PoolType.GameObject)
        {
            Send(ecsWorld, spawnObject, position, rotation, null, null, poolType);
        }

        public static void Send(EcsWorld ecsWorld, GameObject spawnObject, Vector3 position, Quaternion rotation, object data, PoolType poolType = PoolType.GameObject)
        {
            Send(ecsWorld, spawnObject, position, rotation, data, null, poolType);
        }

        public static void Send(EcsWorld ecsWorld, GameObject spawnObject, Vector3 position, Quaternion rotation, Transform parent, PoolType poolType = PoolType.GameObject)
        {
            Send(ecsWorld, spawnObject, position, rotation, null, parent, poolType);
        }

        public static void Send(EcsWorld ecsWorld, GameObject spawnObject, Vector3 position, Quaternion rotation, object data, Transform parent, PoolType poolType = PoolType.GameObject)
        {
            ref var objectPoolSendEvent = ref ecsWorld.GetPool<ObjectPoolSendEvent>().Add(ecsWorld.NewEntity());
            objectPoolSendEvent.objectToSpawn = spawnObject;
            objectPoolSendEvent.position = position;
            objectPoolSendEvent.rotation = rotation;
            objectPoolSendEvent.poolType = poolType;
            objectPoolSendEvent.data = data;
            objectPoolSendEvent.parent = parent;
        }
    }
}
