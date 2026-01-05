using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions; // Necesario para las tareas asíncronas
using System.Collections.Generic;
using System.Linq; // Para ordenar listas
using TMPro; // Para la UI

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    DatabaseReference reference;

    [Header("UI del Ranking")]
    public GameObject filaPrefab; // El diseño de una fila (Nombre - Puntos)
    public Transform contentTabla; // El contenedor dentro del ScrollView

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // 1. Inicializar Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                // Conexión exitosa
                reference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase conectado correctamente");
            }
            else
            {
                Debug.LogError("Error al conectar Firebase: " + task.Result);
            }
        });
    }

    // --- GUARDAR PUNTUACIÓN ---
    public void GuardarPuntuacion(string nombreUsuario, int puntuacion)
    {
        if (reference == null) return;

        // Generamos una ID única basada en el dispositivo (o podrías usar Auth)
        string userId = SystemInfo.deviceUniqueIdentifier;

        // Creamos la clase de datos
        UserScore nuevoScore = new UserScore(nombreUsuario, puntuacion);
        string json = JsonUtility.ToJson(nuevoScore);

        // Guardamos en la ruta: "ranking / ID_USUARIO"
        reference.Child("ranking").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Puntuación subida a la nube");
                }
            });
    }

    // --- DESCARGAR Y MOSTRAR RANKING ---
    public void CargarTopScores()
    {
        if (reference == null) return;

        // Limpiamos la tabla vieja
        foreach (Transform child in contentTabla) Destroy(child.gameObject);

        // Pedimos los datos ordenados por puntuación (Firebase ordena ascendente)
        // LimitToLast(10) nos da los 10 últimos (que son los más altos)
        reference.Child("ranking").OrderByChild("score").LimitToLast(10)
            .GetValueAsync().ContinueWithOnMainThread(task => {

                if (task.IsFaulted)
                {
                    Debug.LogError("Error al cargar ranking");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<UserScore> listaScores = new List<UserScore>();

                    // Convertimos los datos de Firebase a nuestra lista
                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        UserScore userScore = JsonUtility.FromJson<UserScore>(json);
                        listaScores.Add(userScore);
                    }

                    // COMO FIREBASE ORDENA DE MENOR A MAYOR, LE DAMOS LA VUELTA
                    listaScores.Reverse();

                    // Creamos las filas visuales
                    foreach (UserScore s in listaScores)
                    {
                        GameObject nuevaFila = Instantiate(filaPrefab, contentTabla);
                        // Asumimos que el prefab tiene un script o textos hijos
                        // Aquí lo hago simple buscando componentes:
                        TMP_Text[] textos = nuevaFila.GetComponentsInChildren<TMP_Text>();
                        if (textos.Length >= 2)
                        {
                            textos[0].text = s.name;  // Nombre
                            textos[1].text = s.score.ToString(); // Puntos
                        }
                    }
                }
            });
    }
}

// Clase auxiliar para guardar los datos limpios
[System.Serializable]
public class UserScore
{
    public string name;
    public int score;

    public UserScore(string name, int score)
    {
        this.name = name;
        this.score = score;
    }
}