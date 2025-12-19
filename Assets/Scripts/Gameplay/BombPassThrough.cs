using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BombPassThrough : MonoBehaviour
{
    [SerializeField] private float maxGraceTime = 0.6f;

    private Collider2D bombCol;
    private Collider2D playerCol;

    private void Awake()
    {
        bombCol = GetComponent<Collider2D>();
    }

    public void Init(Collider2D playerCollider)
    {
        playerCol = playerCollider;
        if (playerCol == null || bombCol == null) return;

        // Önce geçişe izin ver
        Physics2D.IgnoreCollision(playerCol, bombCol, true);

        // Sonra çıkınca (veya süre bitince) kapat
        StartCoroutine(CloseAfterLeave());
    }

    private IEnumerator CloseAfterLeave()
    {
        float t = 0f;

        // Player bombanın içindeyken bekle (veya timeout)
        while (t < maxGraceTime)
        {
            if (!bombCol.IsTouching(playerCol))
                break;

            t += Time.deltaTime;
            yield return null;
        }

        // 1 fizik frame daha bekle (stabil olsun)
        yield return new WaitForFixedUpdate();

        // Artık solid
        Physics2D.IgnoreCollision(playerCol, bombCol, false);
    }
}