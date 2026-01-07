using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KitchenManager : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Navegación")]
    public RoomsNavigation roomsNavigation;
    public int kitchenRoomIndex = 2;

    [Header("Referencias Mascota")]
    public NeedsManager hungerManager;
    public Image imagenCaraMascota;
    public Sprite caraNormal;
    public Sprite caraComiendo;
    public Sprite caraAsco;

    [Header("Carrusel de Comida")]
    public Image imagenComidaDisplay;
    public Sprite[] spritesComida;
    public Button flechaIzquierda;
    public Button flechaDerecha;

    [Header("Área de Detección")]
    public RectTransform areaBocaGato;

    [Header("Comida")]
    [Range(0f, 1f)]
    public float cantidadHambrePorComida = 0.4f;

    private int indiceComida = 0;
    private Vector2 posicionOriginal;
    private Canvas parentCanvas; 
    private Canvas canvasComida;

    private bool isDragging = false;
    private bool estaTocandoBoca = false;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        if (imagenComidaDisplay != null)
        {
            posicionOriginal = imagenComidaDisplay.rectTransform.anchoredPosition;
            ActualizarDibujoComida();
            
            if (imagenComidaDisplay.GetComponent<Canvas>() == null)
            {
                canvasComida = imagenComidaDisplay.gameObject.AddComponent<Canvas>();
                imagenComidaDisplay.gameObject.AddComponent<GraphicRaycaster>();
            }
            else
            {
                canvasComida = imagenComidaDisplay.GetComponent<Canvas>();
            }
        }

        if (flechaIzquierda != null)
            flechaIzquierda.onClick.AddListener(AnteriorComida);

        if (flechaDerecha != null)
            flechaDerecha.onClick.AddListener(SiguienteComida);
    }

    public void SiguienteComida()
    {
        if (spritesComida.Length == 0) return;
        indiceComida = (indiceComida + 1) % spritesComida.Length;
        ActualizarDibujoComida();
    }

    public void AnteriorComida()
    {
        if (spritesComida.Length == 0) return;
        indiceComida--;
        if (indiceComida < 0)
            indiceComida = spritesComida.Length - 1;
        ActualizarDibujoComida();
    }

    void ActualizarDibujoComida()
    {
        if (imagenComidaDisplay != null && spritesComida.Length > 0)
            imagenComidaDisplay.sprite = spritesComida[indiceComida];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (roomsNavigation != null && roomsNavigation.indexHabitacion != kitchenRoomIndex)
            return;

        isDragging = true;
        estaTocandoBoca = false;

        SetFlechasActivas(false);

        if (canvasComida != null)
        {
            canvasComida.sortingOrder = 50;
        }

        if (hungerManager != null && hungerManager.slider != null)
        {
            float porcentajeHambre = hungerManager.slider.value / hungerManager.slider.maxValue;

            if (porcentajeHambre >= 0.9f)
            {
                if (imagenCaraMascota != null && caraAsco != null)
                    imagenCaraMascota.sprite = caraAsco;

                return;
            }
        }

        if (imagenCaraMascota != null && caraComiendo != null)
            imagenCaraMascota.sprite = caraComiendo;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        imagenComidaDisplay.rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;

        estaTocandoBoca = RectTransformUtility.RectangleContainsScreenPoint(
            areaBocaGato,
            eventData.position,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;

        if (estaTocandoBoca)
        {
            if (hungerManager != null && hungerManager.slider != null)
            {
                float porcentajeHambre = hungerManager.slider.value / hungerManager.slider.maxValue;

                if (porcentajeHambre >= 0.9f)
                {
                    RegresarPosicion();
                    return;
                }
            }

            ConsumirComida();
        }
        else
        {
            RegresarPosicion();
        }
    }

    void ConsumirComida()
    {
        if (hungerManager != null && hungerManager.slider != null)
        {
            float cantidadReal = hungerManager.slider.maxValue * cantidadHambrePorComida;
            float nuevoValor = Mathf.Min(hungerManager.slider.value + cantidadReal, hungerManager.slider.maxValue);
            hungerManager.slider.value = nuevoValor;

            hungerManager.GuardarDatos();
        }

        RegresarPosicion();
    }

    void RegresarPosicion()
    {
        imagenComidaDisplay.rectTransform.anchoredPosition = posicionOriginal;
        
        if (canvasComida != null)
        {
            canvasComida.sortingOrder = 1;
        }

        SetFlechasActivas(true);

        if (imagenCaraMascota != null)
            imagenCaraMascota.sprite = caraNormal;
            
        estaTocandoBoca = false;
    }

    void SetFlechasActivas(bool state)
    {
        if (flechaIzquierda != null) flechaIzquierda.gameObject.SetActive(state);
        if (flechaDerecha != null) flechaDerecha.gameObject.SetActive(state);
    }
}