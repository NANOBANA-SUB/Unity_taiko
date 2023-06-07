using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite donSprite;
    public Sprite katsuSprite;
    // 他のノーツ種類に対応するために必要なスプライトを追加してください

    private float startTime;
    private int type;

    public void StartPlayback(float startTime, int type)
    {
        this.startTime = startTime;
        this.type = type;
        StartCoroutine(PlayNote());
    }

    private IEnumerator PlayNote()
    {
        // ノーツのアニメーションや音声再生などの処理を追加してください
        while (Time.time < startTime)
        {
            yield return null;
        }

        // ノーツの描画
        switch (type)
        {
            case 1:
                spriteRenderer.sprite = donSprite;
                break;
            case 2:
                spriteRenderer.sprite = katsuSprite;
                break;
                // 他のノーツ種類に対応する場合、適切なスプライトを設定してください
        }

        // ノーツの再生完了までの待機
        // 適切な時間やアニメーションなどを追加してください
        yield return new WaitForSeconds(1f);

        // ノーツの削除や非表示化などの処理を追加してください
        Destroy(gameObject);
    }
}
