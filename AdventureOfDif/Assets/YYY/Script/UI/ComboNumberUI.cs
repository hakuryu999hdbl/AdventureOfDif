using UnityEngine;
using UnityEngine.UI;

public class ComboNumberUI : MonoBehaviour
{
    [Header("数字图片 0~9")]
    public Sprite[] numberSprites = new Sprite[10];

    [Header("数字 Image")]
    public Image hundredsImage; // 百位
    public Image tensImage;     // 十位
    public Image onesImage;     // 个位

    /// <summary>
    /// 设置显示数字
    /// </summary>
    public void SetNumber(int number)
    {
        // 最多显示 999
        number = Mathf.Clamp(number, 0, 999);

        int hundreds = number / 100;
        int tens = (number / 10) % 10;
        int ones = number % 10;

        // 百位
        if (number >= 100)
        {
            hundredsImage.gameObject.SetActive(true);
            hundredsImage.sprite = numberSprites[hundreds];
        }
        else
        {
            hundredsImage.gameObject.SetActive(false);
        }

        // 十位
        if (number >= 10)
        {
            tensImage.gameObject.SetActive(true);
            tensImage.sprite = numberSprites[tens];
        }
        else
        {
            tensImage.gameObject.SetActive(false);
        }

        // 个位
        onesImage.gameObject.SetActive(true);
        onesImage.sprite = numberSprites[ones];
    }
}