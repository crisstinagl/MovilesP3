using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KitchenManager : MonoBehaviour
{
    [Header("Navegación")]
    public RoomsNavigation roomsNavigation;
    public int kitchenRoomIndex = 2;

    [Header("Carrusel de Comida")]
    public Image imagenComidaDisplay;
    public Sprite[] spritesComida;
    public Button flechaIzquierda;
    public Button flechaDerecha;

    [Header("Área de Detección")]
    public RectTransform areaBocaGato;

    [Header("Mascota")]
    public NeedsManager hungerManager;

    [Header("Comida")]
    [Range(0f, 1f)]
    public float cantidadHambrePorComida = 0.4f;

    [Header("Sonido")]
    public AudioClip sonidoMasticar;

    private int indiceComida = 0;
    private Vector2 posicionOriginal;
    private Canvas parentCanvas;

    [HideInInspector]
    public bool estaTocandoBoca = false;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        if (imagenComidaDisplay != null)
        {
            posicionOriginal = imagenComidaDisplay.rectTransform.anchoredPosition;
            ActualizarDibujoComida();
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

    public bool PuedeArrastrarComida()
    {
        if (roomsNavigation != null &&
            roomsNavigation.indexHabitacion != kitchenRoomIndex)
            return false;

        return true;
    }

    public void OnComidaDragStart()
    {
        SetFlechasActivas(false);
    }

    public void OnComidaDragging(PointerEventData eventData)
    {
        estaTocandoBoca = RectTransformUtility.RectangleContainsScreenPoint(
            areaBocaGato,
            eventData.position,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : parentCanvas.worldCamera);
    }

    public void OnComidaDragEnd()
    {
        SetFlechasActivas(true);
        estaTocandoBoca = false;
    }

    public void ConsumirComidaDirecto()
    {
        if (hungerManager != null && hungerManager.slider != null)
        {
            if (ScenesManager.Instance != null && ScenesManager.Instance.uiSource != null && sonidoMasticar != null)
            {
                ScenesManager.Instance.uiSource.PlayOneShot(sonidoMasticar);
            }

            float cantidadReal = hungerManager.slider.maxValue * cantidadHambrePorComida;
            hungerManager.slider.value = Mathf.Min(
                hungerManager.slider.value + cantidadReal,
                hungerManager.slider.maxValue);

            hungerManager.GuardarDatos();
        }

        RegresarPosicionDirecto();
    }

    public void RegresarPosicionDirecto()
    {
        if (imagenComidaDisplay != null)
            imagenComidaDisplay.rectTransform.anchoredPosition = posicionOriginal;
    }

    void SetFlechasActivas(bool state)
    {
        if (flechaIzquierda != null)
            flechaIzquierda.gameObject.SetActive(state);

        if (flechaDerecha != null)
            flechaDerecha.gameObject.SetActive(state);
    }
}
