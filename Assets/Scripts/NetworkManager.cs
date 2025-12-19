using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

public class NetworkManager : MonoBehaviour
{
    private HubConnection connection;

    async void Start()
    {
        Debug.Log("Sunucuya bağlanılıyor...");

        // 1. Senin Server Adresin (localhost:5000)
        connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5100/gameHub")
            .WithAutomaticReconnect()
            .Build();

        // 2. Server'dan gelen mesajı dinle
        connection.On<string>("PlayerJoined", (playerName) =>
        {
            Debug.Log($"[SERVER]: Yeni oyuncu katıldı -> {playerName}");
        });

        // 3. Bağlan
        try
        {
            await connection.StartAsync();
            Debug.Log("BAĞLANTI BAŞARILI! Server ile konuşuyorum.");
            
            // Test için servera 'Ben Geldim' diyoruz
            await connection.InvokeAsync("JoinGame", "Unity_Test_Oyuncusu");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Bağlantı Hatası: {ex.Message}");
        }
    }
}
