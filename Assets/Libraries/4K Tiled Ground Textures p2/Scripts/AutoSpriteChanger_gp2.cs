using System.Collections;
using UnityEngine;

namespace AutoSpriteChanger
{
public class AutoSpriteChanger_gp2 : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer spriteRenderer;
    private int currentSpriteIndex = 0;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (sprites.Length == 0)
        {
            Debug.LogWarning("No sprites assigned. Add sprites to the array in the inspector.");
            return;
        }

        StartCoroutine(ChangeSpriteEverySecond());
    }

    private IEnumerator ChangeSpriteEverySecond()
    {
        while (true)
        {
            spriteRenderer.sprite = sprites[currentSpriteIndex];
            currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length;
            yield return new WaitForSeconds(0.7f);
        }
    }
}
}