using UnityEngine;
using Unity.Netcode;

namespace DeathCloud.Core.Network
{
    /// <summary>
    /// Script auxiliar para la escena Sandbox. 
    /// Inicia el Host automáticamente al darle Play para agilizar las pruebas.
    /// </summary>
    public class AutoHostSandbox : MonoBehaviour
    {
        private void Start()
        {
            // Verificamos si ya hay un NetworkManager y si NO está ya corriendo
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                Debug.Log("[AutoHost] Iniciando Host automáticamente para el Sandbox...");
                NetworkManager.Singleton.StartHost();
            }
        }
    }
}
