using Mirror;
using UnityEngine;

namespace JamesFrowen.Spawning
{
    public class MirrorPrefabPool : PrefabPool
    {
        private readonly NetworkIdentity _networkPrefab;
        private bool _handlersRegistered;

        public MirrorPrefabPool(GameObject prefab, int capacity = 100) : this(prefab.GetComponent<PrefabPoolBehaviour>(), prefab.GetComponent<NetworkIdentity>(), capacity) { }
        public MirrorPrefabPool(PrefabPoolBehaviour prefab, int capacity = 100) : this(prefab, prefab.GetComponent<NetworkIdentity>(), capacity) { }
        private MirrorPrefabPool(PrefabPoolBehaviour prefab, NetworkIdentity identity, int capacity = 100) : base(prefab, capacity)
        {
            if (identity == null)
            {
                throw new System.ArgumentNullException(nameof(identity), "Network prefab should have NetworkIdentity");
            }

            _networkPrefab = identity;

            if (NetworkClient.active)
            {
                this.RegisterMirrorHandlers();
            }
        }

        #region Mirror Handlers
        private GameObject networkSpawnHandler(SpawnMessage msg)
        {
            var behaviour = this.Spawn(msg.position, msg.rotation);
            return behaviour.gameObject;
        }
        private void networkUnSpawnHandler(GameObject gameObject)
        {
            var behaviour = gameObject.GetComponent<PrefabPoolBehaviour>();
            this.Unspawn(behaviour);
        }

        public void RegisterMirrorHandlers()
        {
            if (this._handlersRegistered) { return; }

            NetworkClient.RegisterPrefab(this._networkPrefab.gameObject, this.networkSpawnHandler, this.networkUnSpawnHandler);
            this._handlersRegistered = true;
        }

        public void UnregisterMirrorHandlers()
        {
            if (!this._handlersRegistered) { return; }
            NetworkClient.UnregisterPrefab(this._networkPrefab.gameObject);
            this._handlersRegistered = false;
        }
        #endregion

        #region IDisposable Support
        protected override void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.UnregisterMirrorHandlers();
            }

            base.Dispose(disposing);
        }
        #endregion
    }
}