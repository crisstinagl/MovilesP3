using UnityEngine;
using UnityEngine.UI;

// Clase para gestionar la apariencia del gato
public class VisualizadorMascota : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer rendererCuerpo;
    public Image imagenCuerpoUI;

    [Header("Base de Datos Visual")]
    public Sprite skinOriginal;
    public Sprite[] todasLasSkins;

    void Start()
    {
        ActualizarApariencia();
    }

    void OnEnable()
    {
        ActualizarApariencia();
    }

    public void ActualizarApariencia()
    {
        if (ScenesManager.Instance == null) return;

        int id = ScenesManager.Instance.skinEquipada;
        Sprite spriteAUsar = skinOriginal;

        if (id >= 0 && id < todasLasSkins.Length)
        {
            spriteAUsar = todasLasSkins[id];
        }

        if (rendererCuerpo != null) rendererCuerpo.sprite = spriteAUsar;
        if (imagenCuerpoUI != null) imagenCuerpoUI.sprite = spriteAUsar;
    }
}