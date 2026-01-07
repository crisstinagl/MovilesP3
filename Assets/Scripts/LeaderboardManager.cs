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
        // 1. Inicializar Firebase
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

        // 1. PRIMERO LEEMOS LOS DATOS QUE YA EXISTEN
        usuarioRef.GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al leer datos para comparar");
                return;
            }

            DataSnapshot snapshot = task.Result;

            // 2. COMPROBAMOS SI YA EXISTE DATOS
            if (snapshot.Exists && snapshot.Value != null)
            {
                // El usuario ya jugó antes, comprobamos si superó su récord
                string jsonExistente = snapshot.GetRawJsonValue();
                UserScore datosViejos = JsonUtility.FromJson<UserScore>(jsonExistente);

                if (nuevaPuntuacion > datosViejos.score)
                {
                    // ¡ES UN RÉCORD! Sobrescribimos
                    Debug.Log($"Nuevo récord ({nuevaPuntuacion} > {datosViejos.score}). Actualizando...");
                    SubirDatosAFirebase(userId, nombreUsuario, nuevaPuntuacion);
                }
                else
                {
                    // NO ES RÉCORD. No hacemos nada.
                    Debug.Log($"Puntuación {nuevaPuntuacion} no supera el récord de {datosViejos.score}. No se guarda.");
                }
            }
            else
            {
                // Es la primera vez que juega. Guardamos directamente.
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
                    int posicionRanking = 1;

                    // Creamos las filas visuales
                    foreach (UserScore s in listaScores)
                    {
                        GameObject nuevaFila = Instantiate(filaPrefab, contentTabla);
                        // Asumimos que el prefab tiene un script o textos hijos
                        // Aquí lo hago simple buscando componentes:
                        TMP_Text[] textos = nuevaFila.GetComponentsInChildren<TMP_Text>();
                        if (textos.Length >= 3)
                        {
                            textos[0].text = "#" + posicionRanking; // Posición (ej: #1)
                            textos[1].text = s.name;               // Nombre
                            textos[2].text = s.score.ToString();   // Puntos
                        }
                        // POR SI ACASO SOLO TIENES 2 TEXTOS (Para que no falle si no actualizas el prefab)
                        else if (textos.Length >= 2)
                        {
                            textos[0].text = posicionRanking + ". " + s.name; // Juntamos número y nombre
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