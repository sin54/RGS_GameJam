using UnityEngine;

namespace JamesFrowen.Spawning
{
    [RequireComponent(typeof(Rigidbody))]
    public class PrefabRigidbodyPoolBehaviour : PrefabPoolBehaviour
    {
        private Rigidbody _rigidBody;
        internal override void OnInstantiate(PrefabPool pool, Transform _parent)
        {
            base.OnInstantiate(pool, _parent);
            this._rigidBody = this.GetComponent<Rigidbody>();
        }

        internal override void BeforeSpawn()
        {
            this._rigidBody.linearVelocity = Vector3.zero;
            this._rigidBody.angularVelocity = Vector3.zero;

            base.BeforeSpawn();
        }
    }
}