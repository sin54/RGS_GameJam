using UnityEngine;
using Mirror;
namespace JamesFrowen.Spawning
{
    public class PrefabPoolBehaviour : MonoBehaviour
    {
        private PrefabPool _pool;
        private Transform _parent;

        internal virtual void OnInstantiate(PrefabPool pool, Transform _parent)
        {
            this._pool = pool;
            this._parent = _parent;

            this.transform.SetParent(this._parent);
        }

        internal virtual void BeforeSpawn()
        {
            this.gameObject.SetActive(true);
        }

        internal virtual void AfterUnspawn()
        {
            this.gameObject.SetActive(false);
            this.transform.SetParent(this._parent);
        }

        public void Unspawn()
        {
            this._pool.Unspawn(this);
        }
    }
}