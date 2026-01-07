using UnityEngine;
using UnityEngine.EventSystems;

public class FoodDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Referencias")]
    public KitchenManager kitchenManager;
    public Canvas dragCanvasGlobal;

    [Header("Mascota")]
    public UnityEngine.UI.Image imagenCaraMascota;
    public Sprite caraNormal;
    public Sprite caraComiendo;
    public Sprite caraAsco;

    private RectTransform rectTransform;
    private Transform originalParent;
    private Vector2 originalPosition;

    private bool dragging = false;
    private bool hambreLleno = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;

        hambreLleno = false;
        if (kitchenManager != null && kitchenManager.hungerManager != null &&
            kitchenManager.hungerManager.slider != null)
        {
            float porcentaje = kitchenManager.hungerManager.slider.value /
                               kitchenManager.hungerManager.slider.maxValue;
            if (porcentaje >= 0.9f)
                hambreLleno = true;
        }

        if (imagenCaraMascota != null)
            imagenCaraMascota.sprite = hambreLleno ? caraAsco : caraComiendo;

        transform.SetParent(dragCanvasGlobal.transform, true);

        kitchenManager?.OnComidaDragStart();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        rectTransform.anchoredPosition += eventData.delta / dragCanvasGlobal.scaleFactor;

        kitchenManager?.OnComidaDragging(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!dragging) return;

        dragging = false;

        transform.SetParent(originalParent, true);
        rectTransform.anchoredPosition = originalPosition;

        if (imagenCaraMascota != null)
            imagenCaraMascota.sprite = caraNormal;

        if (!hambreLleno)
        {
            kitchenManager?.ConsumirComidaDirecto();
        }
        else
        {
            kitchenManager?.RegresarPosicionDirecto();
        }

        kitchenManager?.OnComidaDragEnd();
    }
}
