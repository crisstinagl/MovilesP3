using UnityEngine;

public class CatSkin : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public SpriteRenderer rendererCara;  // Arrastra aquí el objeto hijo "RenderCara"
    public SpriteRenderer rendererCuerpo; // Arrastra aquí el objeto hijo "Skin"

    [Header("Sprites Caras")]
    public Sprite caraNormal;
    public Sprite caraComiendo;
    public Sprite caraAsco;

    // Función que llamará el Controller
    public void PonerCara(string tipo)
    {
        if (tipo == "normal") rendererCara.sprite = caraNormal;
        if (tipo == "comiendo") rendererCara.sprite = caraComiendo;
        if (tipo == "asco") rendererCara.sprite = caraAsco;
    }

    // Función para volver a la normalidad tras un tiempo
    public void ResetearCara()
    {
        rendererCara.sprite = caraNormal;
    }
}