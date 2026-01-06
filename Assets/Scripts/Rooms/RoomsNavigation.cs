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
    public BedroomManager bedroomScript;
    public int indexHabitacion = 0; // Índice de la habitación actual (0-based)

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

            float distanciaALaHabitacion = Mathf.Abs(scrollRect.horizontalNormalizedPosition - roomsPositions[indexHabitacion]);

            if (distanciaALaHabitacion > 0.1f)
            {
                if (bedroomScript != null) bedroomScript.DespertarForzado();
            }
        }

        Debug.Log(indexHabitacion);
    }

    public void OnBeginDrag(PointerEventData eventData) => swiping = true;

    public void OnEndDrag(PointerEventData eventData)
    {
        swiping = false;
        float actualPos = scrollRect.horizontalNormalizedPosition;
        float nearest = roomsPositions[0];
        int nearestIndex = 0;

        for (int i = 0; i < roomsPositions.Length; i++)
        {
            if (Mathf.Abs(actualPos - roomsPositions[i]) < Mathf.Abs(actualPos - nearest))
            {
                nearest = roomsPositions[i];
                nearestIndex = i;
            }
        }
        targetRoom = nearest;

        // Actualizar el índice de la habitación actual
        indexHabitacion = nearestIndex;

        // Si se sale del dormitorio despertar
        if (nearestIndex != indexHabitacion && bedroomScript != null)
        {
            bedroomScript.DespertarForzado();
        }
    }

    // Funcion para cambiar de sala
    public void ChangeRoom(int index)
    {
        targetRoom = roomsPositions[index];

        // Actualizar el índice de la habitación actual
        indexHabitacion = index;

        // Si se sale del dormitorio despertar
        if (index != indexHabitacion && bedroomScript != null)
        {
            bedroomScript.DespertarForzado();
        }
    }
}