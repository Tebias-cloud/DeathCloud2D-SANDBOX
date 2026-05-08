using UnityEngine;
using Unity.Netcode;

namespace DeathCloud.Core.Network
{
    /// <summary>
    /// Asegura que solo exista un NetworkManager en la escena y lo persiste entre cargas.
    /// </summary>
    [RequireComponent(typeof(NetworkManager))]
    public class NetworkManagerPersistent : MonoBehaviour
    {
        private static NetworkManagerPersistent _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Debug.Log($"[NetworkManagerPersistent] Instancia duplicada detectada en {gameObject.name}. Destruyendo...");
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[NetworkManagerPersistent] Instancia única inicializada y persistente.");
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
