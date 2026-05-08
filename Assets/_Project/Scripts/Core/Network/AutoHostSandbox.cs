using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

namespace DeathCloud.Core.Network
{
    public class AutoHostSandbox : MonoBehaviour
    {
        private void Awake()
        {
            // Cambiamos el puerto ANTES de que nada ocurra
            var transport = GetComponent<UnityTransport>();
            if (transport == null && NetworkManager.Singleton != null) 
                transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            if (transport != null)
            {
                ushort randomPort = (ushort)Random.Range(7778, 8000);
                transport.SetConnectionData("127.0.0.1", randomPort);
                Debug.Log($"[AutoHost] Puerto configurado dinámicamente: {randomPort}");
            }
        }

        private void Start()
        {
            if (NetworkManager.Singleton == null) return;

            // Si hay un error de prefabs, nos avisará
            if (NetworkManager.Singleton.NetworkConfig.PlayerPrefab == null)
            {
                Debug.LogError("[AutoHost] CRÍTICO: No hay un Player Prefab asignado en el NetworkManager.");
                return;
            }
            
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                // Limpieza de estados anteriores
                NetworkManager.Singleton.Shutdown();

                // Cambiamos el puerto agresivamente
                var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
                if (transport != null)
                {
                    ushort randomPort = (ushort)Random.Range(7778, 8000);
                    transport.SetConnectionData("127.0.0.1", randomPort);
                    Debug.Log($"[AutoHost] Puerto configurado: {randomPort}");
                }

                Debug.Log("[AutoHost] Iniciando Host...");
                NetworkManager.Singleton.StartHost();
            }
        }
    }
}
