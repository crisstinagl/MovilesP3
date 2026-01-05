using UnityEngine;

public class CatSkin : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public SpriteRenderer rendererCara;  // Arrastra aquí el objeto hijo "RenderCara"
    public SpriteRenderer rendererCuerpo; // Arrastra aquí el objeto hijo "Skin"

    [Header("Configuración de Skins")]
    public Sprite skinPorDefecto;
    public Sprite[] listaDeSkins;

    [Header("Sprites Caras")]
    public Sprite caraNormal;
    public Sprite caraComiendo;
    public Sprite caraAsco;

    void Start()
    {
        ActualizarSkin();
    }

    public void ActualizarSkin()
    {
        if (ScenesManager.Instance != null)
        {
            int idSkin = ScenesManager.Instance.skinEquipada;

            if (idSkin == -1)
            {
                rendererCuerpo.sprite = skinPorDefecto;
            }
            else if (idSkin >= 0 && idSkin < listaDeSkins.Length)
            {
                rendererCuerpo.sprite = listaDeSkins[idSkin];
            }
        }
    }
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