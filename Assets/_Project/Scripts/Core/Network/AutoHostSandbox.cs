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
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("[AutoHost] ERROR: No se encontró un NetworkManager en la escena.");
                return;
            }

            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                Debug.Log("[AutoHost] Intentando iniciar Host...");
                bool success = NetworkManager.Singleton.StartHost();
                if (success) Debug.Log("[AutoHost] Host iniciado con ÉXITO.");
                else Debug.LogError("[AutoHost] FALLÓ el inicio del Host. Revisa errores de transporte en la consola.");
            }
            else
            {
                Debug.Log("[AutoHost] El NetworkManager ya está corriendo (Host/Cliente/Servidor).");
            }
        }
    }
}
