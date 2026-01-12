using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Android;

public class ShowerManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Referencias de Necesidades")]
    public NeedsManager higieneManager;
    public string higieneID = "HigieneValue";
    public Image imagenMojado;

    [Header("Efectos Visuales (UI)")]
    public RectTransform esponjaRect;
    public RectTransform duchaRect;

    [Header("Sistema de Burbujas")]
    public RectTransform[] burbujas;
    public float velocidadAparicionBurbujas = 2f;
    private float burbujasVisibles = 0f;

    [Header("Ajustes de Limpieza")]
    public float cantidadPorFrotacion = 0.05f;
    public float umbralDucha = 0.5f;
    public float velocidadCambioHumedad = 0.5f;

    [Header("Referencias de Navegación")]
    public RoomsNavigation roomsNavigation;
    private const int SHOWER_ROOM_INDEX = 1;

    [Header("Sistema de Secado (Micrófono)")]
    public AudioSource audioSource;
    public float umbralSoplido = 0.3f;
    private string microfono;
    private const int SAMPLE_WINDOW = 128;

    private bool estaFrotando = false;
    private RectTransform herramientaActiva;
    private Vector2 posicionBaseHerramienta;
    private Canvas parentCanvas;
    private float nivelHumedad = 0f;
    private bool estabaEnDucha = false;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (esponjaRect != null) posicionBaseHerramienta = esponjaRect.localPosition;
        if (imagenMojado != null) SetImageAlpha(imagenMojado, 0);
        foreach (var b in burbujas) b.gameObject.SetActive(false);

        InitializeMicrophone();
        CheckAndSwitchTool(true);
    }

    void Update()
    {
        bool actualmenteEnDucha = IsInShowerRoom();

        if (!actualmenteEnDucha)
        {
            if (estabaEnDucha)
            {
                OcultarHerramientas();
                estabaEnDucha = false;
            }
            return;
        }

        if (!estabaEnDucha && actualmenteEnDucha)
        {
            CheckAndSwitchTool(true);
            estabaEnDucha = true;
        }

        CheckAndSwitchTool(false);
        ManejarHumedadYSecado();

        if (!estaFrotando && herramientaActiva != null)
        {
            herramientaActiva.localPosition = Vector2.Lerp(herramientaActiva.localPosition, posicionBaseHerramienta, Time.deltaTime * 5f);
        }
    }

    private void ManejarHumedadYSecado()
    {
        float delta = Time.deltaTime * velocidadCambioHumedad;

        if (nivelHumedad > 0)
        {
            float volumen = GetMicVolume();
            if (volumen > umbralSoplido)
            {
                nivelHumedad = Mathf.Max(0, nivelHumedad - delta);
            }
        }

        if (estaFrotando && herramientaActiva == esponjaRect)
        {
            nivelHumedad = Mathf.Min(1, nivelHumedad + delta);
        }

        if (imagenMojado != null)
        {
            SetImageAlpha(imagenMojado, nivelHumedad);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsInShowerRoom()) return;
        estaFrotando = true;
        CheckAndSwitchTool(false);
        ActualizarPosicionUI(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsInShowerRoom() || !estaFrotando) return;

        higieneManager.ReloadNeed(cantidadPorFrotacion * Time.deltaTime);
        ActualizarPosicionUI(eventData);
        CheckAndSwitchTool(false);
        ActualizarBurbujas();

        if (higieneManager.slider.value >= higieneManager.slider.maxValue)
            OnPointerUp(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        estaFrotando = false;
        if (higieneManager != null) higieneManager.GuardarDatos();
    }

    void ActualizarBurbujas()
    {
        if (burbujas == null || burbujas.Length == 0) return;

        if (herramientaActiva == duchaRect)
        {
            burbujasVisibles += Time.deltaTime * velocidadAparicionBurbujas;
        }
        else if (herramientaActiva == esponjaRect)
        {
            burbujasVisibles -= Time.deltaTime * velocidadAparicionBurbujas * 1.5f;
        }

        burbujasVisibles = Mathf.Clamp(burbujasVisibles, 0, burbujas.Length);

        for (int i = 0; i < burbujas.Length; i++)
        {
            bool debeEstarActiva = i < Mathf.FloorToInt(burbujasVisibles);
            if (burbujas[i].gameObject.activeSelf != debeEstarActiva)
            {
                burbujas[i].gameObject.SetActive(debeEstarActiva);
            }
        }
    }

    void CheckAndSwitchTool(bool inicializar)
    {
        if (higieneManager == null || !IsInShowerRoom()) return;

        float progreso = higieneManager.slider.value / higieneManager.slider.maxValue;
        RectTransform herramientaAnterior = herramientaActiva;

        herramientaActiva = (progreso < umbralDucha) ? duchaRect : esponjaRect;

        if (herramientaAnterior != herramientaActiva || inicializar)
        {
            if (esponjaRect != null) esponjaRect.gameObject.SetActive(herramientaActiva == esponjaRect);
            if (duchaRect != null) duchaRect.gameObject.SetActive(herramientaActiva == duchaRect);

            if (!estaFrotando && herramientaActiva != null)
                herramientaActiva.localPosition = posicionBaseHerramienta;
        }
    }

    private void OcultarHerramientas()
    {
        estaFrotando = false;
        if (esponjaRect != null) esponjaRect.gameObject.SetActive(false);
        if (duchaRect != null) duchaRect.gameObject.SetActive(false);
    }

    void ActualizarPosicionUI(PointerEventData eventData)
    {
        if (herramientaActiva == null || parentCanvas == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPos
        );
        herramientaActiva.localPosition = localPos;
    }

    private void SetImageAlpha(Image img, float alpha)
    {
        Color c = img.color;
        c.a = alpha;
        img.color = c;
    }

    private bool IsInShowerRoom() => roomsNavigation != null && roomsNavigation.indexHabitacion == SHOWER_ROOM_INDEX;

    void InitializeMicrophone()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            Permission.RequestUserPermission(Permission.Microphone);

        if (Microphone.devices.Length > 0)
        {
            microfono = Microphone.devices[0];
            audioSource.clip = Microphone.Start(microfono, true, 1, AudioSettings.outputSampleRate);
            audioSource.loop = true;
            while (!(Microphone.GetPosition(microfono) > 0)) { }
            audioSource.Play();
        }
    }

    float GetMicVolume()
    {
        if (audioSource.clip == null || microfono == null) return 0f;
        float[] waveData = new float[SAMPLE_WINDOW];
        int micPosition = Microphone.GetPosition(microfono) - (SAMPLE_WINDOW + 1);
        if (micPosition < 0) return 0f;
        audioSource.clip.GetData(waveData, micPosition);
        float maxVolume = 0;
        foreach (var sample in waveData) maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample));
        return maxVolume;
    }

    void OnDisable()
    {
        if (!string.IsNullOrEmpty(microfono)) Microphone.End(microfono);
    }
}