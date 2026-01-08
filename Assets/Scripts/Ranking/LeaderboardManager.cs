using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    DatabaseReference reference;

    [Header("UI del Ranking")]
    public GameObject filaPrefab; // diseño de una fila (Nombre - Puntos)
    public Transform contentTabla;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Inicializar Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.Result == DependencyStatus.Available)
            {
                // Conexión exitosa
                reference = FirebaseDatabase.GetInstance("https://miou-41221-default-rtdb.europe-west1.firebasedatabase.app/").RootReference;
                Debug.Log("Firebase conectado correctamente");
            }
            else
            {
                Debug.LogError("Error al conectar Firebase: " + task.Result);
            }
        });
    }

    // --- GUARDAR PUNTUACIÓN ---
    public void GuardarPuntuacion(string nombreUsuario, int nuevaPuntuacion)
    {
        if (reference == null) return;

        // Usamos la ID del dispositivo para que el usuario sea UNICO
        string userId = SystemInfo.deviceUniqueIdentifier;

        // Referencia al nodo de este usuario específico
        DatabaseReference usuarioRef = reference.Child("ranking").Child(userId);

        // LEEMOS LOS DATOS QUE YA EXISTEN
        usuarioRef.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al leer datos para comparar");
                return;
            }

            DataSnapshot snapshot = task.Result;

            // COMPROBAR SI YA EXISTE DATOS
            if (snapshot.Exists && snapshot.Value != null)
            {
                string jsonExistente = snapshot.GetRawJsonValue();
                UserScore datosViejos = JsonUtility.FromJson<UserScore>(jsonExistente);

                if (nuevaPuntuacion > datosViejos.score)
                {
                    // Nuevo record - Sobrescribir
                    Debug.Log($"Nuevo récord ({nuevaPuntuacion} > {datosViejos.score}). Actualizando...");
                    SubirDatosAFirebase(userId, nombreUsuario, nuevaPuntuacion);
                }
                else
                {
                    // No record
                    Debug.Log($"Puntuación {nuevaPuntuacion} no supera el récord de {datosViejos.score}. No se guarda.");
                }
            }
            else
            {
                // Primera vez - guardar
                Debug.Log("Primer juego de este usuario. Guardando...");
                SubirDatosAFirebase(userId, nombreUsuario, nuevaPuntuacion);
            }
        });
    }

    private void SubirDatosAFirebase(string userId, string nombre, int score)
    {
        UserScore nuevoScore = new UserScore(nombre, score);
        string json = JsonUtility.ToJson(nuevoScore);

        reference.Child("ranking").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Datos guardados en la nube exitosamente.");
                }
            });
    }

    // --- DESCARGAR Y MOSTRAR RANKING ---
    public void CargarTopScores()
    {
        if (reference == null) return;

        // Limpiar tabla vieja
        foreach (Transform child in contentTabla) Destroy(child.gameObject);

        // Acceder a los 10 ultimos datos ordenados por puntuación en ascendente
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

                    // Convertir los datos de Firebase a la lista
                    foreach (DataSnapshot child in snapshot.Children)
                    {
                        string json = child.GetRawJsonValue();
                        UserScore userScore = JsonUtility.FromJson<UserScore>(json);
                        listaScores.Add(userScore);
                    }

                    // Dar la vuelta al orden
                    listaScores.Reverse();
                    int posicionRanking = 1;

                    // Crear filas visuales
                    foreach (UserScore s in listaScores)
                    {
                        GameObject nuevaFila = Instantiate(filaPrefab, contentTabla);
                        TMP_Text[] textos = nuevaFila.GetComponentsInChildren<TMP_Text>();
                        if (textos.Length >= 3)
                        {
                            textos[0].text = "#" + posicionRanking; // Posición
                            textos[1].text = s.name;               // Nombre
                            textos[2].text = s.score.ToString();   // Puntos
                        }
                        else if (textos.Length >= 2)
                        {
                            textos[0].text = posicionRanking + ". " + s.name; // Se junta número y nombre
                            textos[1].text = s.score.ToString();
                        }

                        posicionRanking++;
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