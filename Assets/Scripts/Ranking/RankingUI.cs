using UnityEngine;

public class RankingUI : MonoBehaviour
{
    public Transform elContentNuevo;

    void OnEnable()
    {
        if (LeaderboardManager.Instance != null)
        {
            LeaderboardManager.Instance.contentTabla = elContentNuevo;

            LeaderboardManager.Instance.CargarTopScores();
        }
    }
}