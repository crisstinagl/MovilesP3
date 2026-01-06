using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Android;

public class ShowerManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias de Necesidades")]
    public NeedsManager higieneManager;
    public string higieneID = "HigieneValue";
    public GameObject objetoMojado;

    [Header("Efectos Visuales (UI IMAGES)")]
    public RectTransform esponjaRect;
    public RectTransform duchaRect;
    public ParticleSystem particulasJabon;

    [Header("Ajustes de Limpieza")]
    public float cantidadPorFrotacion = 0.05f;
    public float umbralDucha = 0.5f;

    [Header("Referencias de Navegación")]
    public RoomsNavigation roomsNavigation; // Referencia al script de navegación
    private const int SHOWER_ROOM_INDEX = 1; // Índice de la habitación de la ducha

    [Header("Sistema de Secado (Micrófono)")]
    [Tooltip("El audio source para reproducir y almacenar el clip del micrófono.")]
    public AudioSource audioSource;
    [Tooltip("Umbral de volumen (0 a 1) para considerar que hay un 'soplido'.")]
    public float umbralSoplido = 0.5f;
    [Tooltip("Duración máxima del clip del micrófono (en segundos).")]
    public int clipDuration = 1;
    [Tooltip("Tiempo mínimo (en segundos) que debe estar activo el soplido para secar.")]
    public float tiempoMinimoSoplido = 0.1f;

    private bool estaFrotando = false;
    private RectTransform efectoActivoRect;
    private Canvas parentCanvas;

    private string microfono;
    private float tiempoSoplando = 0f;
    private const int SAMPLE_WINDOW = 128;
    private bool estaSecando = false;


    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            Debug.LogError("ShowerManager debe ser hijo de un Canvas para funcionar en modo UI.");
            return;
        }

        if (roomsNavigation == null)
        {
            Debug.LogError("La referencia a RoomsNavigation no está asignada en ShowerManager.");
        }

        if (higieneManager != null)
        {
            CheckAndSwitchTool(true);
        }

        if (objetoMojado != null) objetoMojado.SetActive(false);

        InitializeMicrophone();
    }

    void InitializeMicrophone()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource no asignado en ShowerManager.");
            return;
        }

        if (Microphone.devices.Length > 0)
        {
            microfono = Microphone.devices[0];

            audioSource.clip = Microphone.Start(microfono, true, clipDuration, AudioSettings.outputSampleRate);

            while (!(Microphone.GetPosition(microfono) > 0)) { }

            audioSource.loop = true;
            audioSource.Play();

            Debug.Log("Micrófono inicializado: " + microfono);
        }
        else
        {
            Debug.LogWarning("No se encontraron dispositivos de micrófono.");
        }
    }

    float GetMicVolume()
    {
        if (audioSource.clip == null || microfono == null) return 0f;

        float maxVolume = 0f;
        int micPosition = Microphone.GetPosition(microfono) - (SAMPLE_WINDOW + 1);
        if (micPosition < 0) return 0f;

        float[] waveData = new float[SAMPLE_WINDOW];
        audioSource.clip.GetData(waveData, micPosition);

        for (int i = 0; i < SAMPLE_WINDOW; i++)
        {
            float absValue = Mathf.Abs(waveData[i]);
            if (absValue > maxVolume)
            {
                maxVolume = absValue;
            }
        }
        return maxVolume;
    }

    void Update()
    {
        bool inShowerRoom = roomsNavigation != null && roomsNavigation.indexHabitacion == SHOWER_ROOM_INDEX;

        if (higieneManager != null)
        {
            if (higieneManager.slider.value <= 0 && higieneManager.objetoSuciedad != null)
            {
                higieneManager.objetoSuciedad.SetActive(true);
            }
        }

        // La lógica de secado solo se ejecuta si estamos en la habitación correcta
        if (inShowerRoom && objetoMojado != null && objetoMojado.activeSelf)
        {
            float volumen = GetMicVolume();

            if (volumen > umbralSoplido)
            {
                tiempoSoplando += Time.deltaTime;

                if (tiempoSoplando >= tiempoMinimoSoplido)
                {
                    estaSecando = true;
                }
            }
            else
            {
                tiempoSoplando = 0f;
                estaSecando = false;
            }

            if (estaSecando)
            {
                objetoMojado.SetActive(false);
                estaSecando = false;
                Debug.Log("¡Secado completado con soplido!");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        bool inShowerRoom = roomsNavigation != null && roomsNavigation.indexHabitacion == SHOWER_ROOM_INDEX;

        if (!inShowerRoom)
        {
            Debug.Log("Acción de ducha/esponja ignorada: no estás en la habitación de la ducha.");
            return;
        }

        if (higieneManager != null && higieneManager.slider.value < higieneManager.slider.maxValue)
        {
            estaFrotando = true;
            CheckAndSwitchTool(false);
            ActualizarPosicionUI(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        bool inShowerRoom = roomsNavigation != null && roomsNavigation.indexHabitacion == SHOWER_ROOM_INDEX;

        if (!inShowerRoom) return; // Ignorar el arrastre si no estamos en la habitación de la ducha

        if (estaFrotando && higieneManager != null)
        {
            higieneManager.ReloadNeed(cantidadPorFrotacion * Time.deltaTime);
            CheckAndSwitchTool(false);
            ActualizarPosicionUI(eventData);

            if (higieneManager.slider.value >= higieneManager.slider.maxValue)
            {
                OnPointerUp(eventData);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!estaFrotando) return;

        estaFrotando = false;

        if (efectoActivoRect != null)
        {
            efectoActivoRect.gameObject.SetActive(false);
        }
        if (particulasJabon != null)
        {
            particulasJabon.Stop();
        }

        if (higieneManager != null)
        {
            higieneManager.GuardarDatos();
        }
    }

    void CheckAndSwitchTool(bool soloInicializar)
    {
        if (higieneManager == null) return;

        bool inShowerRoom = roomsNavigation != null && roomsNavigation.indexHabitacion == SHOWER_ROOM_INDEX;

        if (!inShowerRoom)
        {
            // Asegurarse de que las herramientas estén ocultas si no estamos en la habitación de la ducha
            if (esponjaRect != null) esponjaRect.gameObject.SetActive(false);
            if (duchaRect != null) duchaRect.gameObject.SetActive(false);
            if (particulasJabon != null) particulasJabon.Stop();
            if (objetoMojado != null) objetoMojado.SetActive(false);
            return;
        }

        float valorNormalizado = higieneManager.slider.value / higieneManager.slider.maxValue;
        RectTransform herramientaDeseada;

        bool usarDucha = (valorNormalizado < umbralDucha);

        if (usarDucha)
        {
            herramientaDeseada = duchaRect;
        }
        else
        {
            herramientaDeseada = esponjaRect;
        }

        if (soloInicializar)
        {
            efectoActivoRect = herramientaDeseada;
            if (esponjaRect != null) esponjaRect.gameObject.SetActive(false);
            if (duchaRect != null) duchaRect.gameObject.SetActive(false);
            return;
        }

        if (efectoActivoRect != herramientaDeseada || !efectoActivoRect.gameObject.activeSelf)
        {
            if (efectoActivoRect != null)
            {
                efectoActivoRect.gameObject.SetActive(false);
            }

            efectoActivoRect = herramientaDeseada;

            if (efectoActivoRect != null)
            {
                efectoActivoRect.gameObject.SetActive(true);
            }
        }

        if (efectoActivoRect == esponjaRect && objetoMojado != null && !objetoMojado.activeSelf)
        {
            objetoMojado.SetActive(true);
        }

        if (particulasJabon != null)
        {
            if (efectoActivoRect == esponjaRect)
            {
                particulasJabon.Play();
            }
            else
            {
                particulasJabon.Stop();
            }
        }
    }

    // Función de Conversión para UI
    void ActualizarPosicionUI(PointerEventData eventData)
    {
        if (efectoActivoRect == null || parentCanvas == null) return;

        Vector2 posicionLocal;

        bool exito = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out posicionLocal
        );

        if (exito)
        {
            efectoActivoRect.localPosition = posicionLocal;
        }
    }

    // Al salir de la escena, detenemos el micrófono
    void OnDisable()
    {
        if (Microphone.devices.Length > 0 && microfono != null)
        {
            Microphone.End(microfono);
        }
    }
}