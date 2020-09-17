using UnityEngine;
using System.Collections;

/// <summary>
/// 喇叭控制脚本
/// </summary>
public class Horn : MonoBehaviour {

    public RectTransform hornText;
    public RectTransform hornMask;
    // Use this for initialization
    void Start()
    {
        hornText = transform.Find("Text").GetComponent<RectTransform>();
        hornMask = transform.GetComponent<RectTransform>();
        // hornText.GetComponent<Text>().text = ""+;
        //将文本设置在horn的右边
        hornText.localPosition = new Vector3(transform.localPosition.x + (hornMask.rect.width / 2), 0, 0);
        StartCoroutine(TextMov());
    }

    IEnumerator TextMov()
    {
        yield return null;
        float distence = hornMask.rect.width + hornText.rect.width;
        float temp = 0;
        while (temp < distence)
        {
            hornText.localPosition -= Vector3.right * 1f;
            temp += 1f;
            yield return null;
        }
        Destroy(gameObject, 1f);
    }
}
