using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

/// <summary>
/// 界面加载脚本
/// </summary>
public class UIPanelLoad : MonoBehaviour
{
    private static UIPanelLoad instance;

    public static UIPanelLoad Instance
    {
        get
        {
            if (instance == null)
            {
                GameController.Instance.uiController.CreatePanel("PanelLoad");
            }
            return instance;
        }
    }

    public DOTweenAnimation[] doTweenAnimations;

    public Slider slider;
    public Text sliderText;

    private void Awake()
    {
        instance = this;
        //初始化
        doTweenAnimations = transform.Find("Text").GetComponentsInChildren<DOTweenAnimation>();
        slider = transform.Find("Slider").GetComponent<Slider>();
        sliderText = transform.Find("TextPlane").GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine(TextMov());//实现动画效果
    }

    public delegate void UIPanelLoadHandler();

    public UIPanelLoadHandler handler;
    public string SceneName;

    public void Init(string sceneName, UIPanelLoadHandler callback = null)
    {
        SceneName = sceneName;//拿到场景的参数
        handler = callback;//回调函数的赋值
        StartCoroutine(LoadMov());//开启加载协程
    }


    IEnumerator LoadMov()
    {

        // 异步的加载 场景
        //AsyncOperation op = SceneManager.LoadSceneAsync("MainCity");

        //while (op.isDone == false)
        //{
        //    Debug.Log(op.progress);
        //    //显示信息
        //    yield return null;
        //}
        //Debug.Log("ok");

        while (slider.value < 1)
        {
            slider.value += Time.deltaTime * 0.9f;
            sliderText.text = string.Format("{0:f1}%", slider.value * 100);
            yield return null;
        }
        SceneManager.LoadScene(SceneName);
        Debug.Log(SceneName + " load ok");

        yield return null; //

        if (handler !=null)
        {
            handler();
        }

        Destroy(gameObject);
    }


    IEnumerator TextMov()
    {
        while (true)
        {
            yield return new WaitForSeconds(4f);
            for (int i = 0; i < doTweenAnimations.Length; i++)
            {
                doTweenAnimations[i].DORestart();
            }
            
        }
    }
}
