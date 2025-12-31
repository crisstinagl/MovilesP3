using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Clase para gestionar la navegacion entre los paneles del juego deslizando
public class RoomsNavigation : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    [Header("Referencias")]
    public ScrollRect scrollRect;
    public RectTransform content;
    public int roomsNum = 4;

    private float targetRoom;
    private bool swiping = false;
    private float[] roomsPositions;

    void Start()
    {
        roomsPositions = new float[roomsNum];
        float distancia = 1f / (roomsNum - 1);
        for (int i = 0; i < roomsNum; i++)
        {
            roomsPositions[i] = i * distancia;
        }
    }

    void Update()
    {
        if (!swiping)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(scrollRect.horizontalNormalizedPosition, targetRoom, Time.deltaTime * 10f);
        }
    }

    public void OnBeginDrag(PointerEventData eventData) => swiping = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        swiping = false;
        float actualPos = scrollRect.horizontalNormalizedPosition;
        float nearest = roomsPositions[0];

        foreach (float p in roomsPositions)
        {
            if (Mathf.Abs(actualPos - p) < Mathf.Abs(actualPos - nearest))
                nearest = p;
        }
        targetRoom = nearest;
    }

    // Funcion para cambiar de sala
    public void ChangeRoom(int index)
    {
        targetRoom = roomsPositions[index];
    }
}
