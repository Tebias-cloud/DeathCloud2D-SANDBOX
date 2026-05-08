using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace DeathCloud.Core.Network
{
    public class AutoHostSandbox : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError("[AutoHost] ERROR: No se encontró un NetworkManager.");
                return;
            }

            // Limpieza preventiva: Asegurar que no haya managers "zombies" de escenas anteriores
            var allManagers = FindObjectsByType<NetworkManager>(FindObjectsSortMode.None);
            foreach (var manager in allManagers)
            {
                if (manager != NetworkManager.Singleton)
                {
                    Debug.LogWarning($"[AutoHost] Destruyendo NetworkManager zombie: {manager.gameObject.name}");
                    Destroy(manager.gameObject);
                }
            }

            // Si está apagando, esperamos un poco (raro en Start, pero por seguridad)
            if (NetworkManager.Singleton.ShutdownInProgress)
            {
                Debug.LogWarning("[AutoHost] Shutdown en progreso, abortando inicio inmediato.");
                return;
            }

            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartHostWithRetry();
            }
        }

        private void StartHostWithRetry()
        {
            Debug.Log("[AutoHost] Intentando iniciar Host...");
            
            // Intento 1: Puerto original
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("[AutoHost] Host iniciado con ÉXITO.");
                return;
            }

            Debug.LogWarning("[AutoHost] FALLÓ el inicio inicial. Iniciando secuencia de bypass de puertos...");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            if (transport == null)
            {
                Debug.LogError("[AutoHost] No se encontró UnityTransport para realizar el bypass.");
                return;
            }

            // Intentar 5 puertos diferentes (7777, 7778, 7779, 7780, 7781)
            for (int i = 1; i <= 5; i++)
            {
                ushort newPort = (ushort)(7777 + i);
                transport.ConnectionData.Port = newPort;
                Debug.Log($"[AutoHost] Reintentando en puerto: {newPort} (Intento {i}/5)...");

                if (NetworkManager.Singleton.StartHost())
                {
                    Debug.Log($"[AutoHost] Host iniciado con ÉXITO en puerto {newPort}.");
                    return;
                }
            }

            Debug.LogError("[AutoHost] Fallo crítico: Agotados todos los intentos de bypass de puerto.");
        }
    }
}
