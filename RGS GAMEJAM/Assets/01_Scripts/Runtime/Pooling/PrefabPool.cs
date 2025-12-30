using UnityEngine;

namespace JamesFrowen.Spawning
{
    public class PrefabPool : System.IDisposable
    {
        /// <summary>
        /// parent shared by all pools
        /// </summary>
        private static Transform _poolParent;

        protected readonly PrefabPoolBehaviour _prefab;
        private readonly PrefabPoolBehaviour[] _pool;
        /// <summary>
        /// parent for this pool, should be a child of _poolParent
        /// </summary>
        private readonly Transform _parent;

        private int _next = -1;

        public PrefabPool(GameObject prefab, int capacity = 100) : this(prefab.GetComponent<PrefabPoolBehaviour>(), capacity) { }
        public PrefabPool(PrefabPoolBehaviour prefab, int capacity = 100)
        {
            if (prefab == null)
            {
                throw new System.ArgumentNullException(nameof(prefab));
            }
            this._prefab = prefab;
            this._pool = new PrefabPoolBehaviour[capacity];


            if (_poolParent == null)
            {
                _poolParent = new GameObject("PrefabPools").transform;
            }
            this._parent = new GameObject(this._prefab.name).transform;
            this._parent.SetParent(_poolParent);
        }

        public void ClearNullObject()
        {
            var nextEmpty = 0;
            for (var i = 0; i < this._pool.Length; i++)
            {
                if (this._pool[i] != null)
                {
                    this._pool[nextEmpty] = this._pool[i];
                    nextEmpty++;
                }
                else
                {
                    // clear c# object if Unity object is null
                    this._pool[i] = null;
                }
            }

            this._next = nextEmpty - 1;
        }
        public void Warnup(int createCount)
        {
            for (var i = this._next + 1; i < createCount; i++)
            {
                var item = UnityEngine.Object.Instantiate(this._prefab);
                item.OnInstantiate(this, this._parent);
                this.putBack(item);
            }
        }

        private PrefabPoolBehaviour getNext()
        {
            PrefabPoolBehaviour item;
            if (this._next == -1)
            {
                item = UnityEngine.Object.Instantiate(this._prefab);
                item.OnInstantiate(this, this._parent);
            }
            else
            {
                item = this._pool[this._next];
                this._pool[this._next] = null;
                this._next--;
            }

            item.BeforeSpawn();
            return item;
        }
        private void putBack(PrefabPoolBehaviour obj)
        {
            if (this.disposedValue)
            {
                UnityEngine.Object.Destroy(obj.gameObject);
                return;
            }

            if (this._next < this._pool.Length - 1)
            {
                this._next++;
                this._pool[this._next] = obj;
                obj.AfterUnspawn();
            }
            else
            {
                UnityEngine.Object.Destroy(obj.gameObject);
                Debug.LogWarning("NetworkWriterPool.Recycle, Pool was full leaving extra writer for GC");
            }
        }


        public PrefabPoolBehaviour Spawn()
        {
            return this.getNext();
        }
        public PrefabPoolBehaviour Spawn(Vector3 position, Quaternion rotation)
        {
            var obj = this.getNext();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }
        public PrefabPoolBehaviour Spawn(Transform parent)
        {
            var obj = this.getNext();
            obj.transform.SetParent(parent);
            return obj;
        }
        public PrefabPoolBehaviour Spawn(Vector3 position, Quaternion rotation, Transform parent)
        {
            var obj = this.getNext();
            obj.transform.SetParent(parent);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }


        public void Unspawn(PrefabPoolBehaviour obj)
        {
            this.putBack(obj);
        }


        #region IDisposable Support
        protected bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    for (var i = 0; i < this._pool.Length; i++)
                    {
                        if (this._pool[i] != null)
                        {
                            UnityEngine.Object.Destroy(this._pool[i].gameObject);
                            this._pool[i] = null;
                        }
                    }
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
        #endregion
    }
}