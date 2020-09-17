using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

/// <summary>
/// http访问
/// </summary>
public class Http : MonoBehaviour {

    public string url = "https://b-ssl.duitang.com/uploads/item/201506/08/20150608193420_BYcuA.jpeg";

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(DownloadImageMov());
	}
	
	IEnumerator DownloadImageMov()
    {
        //定义www为www类型并且 等待 所下载www中的内容
        WWW www = new WWW(url);
        yield return www;
        while (!www.isDone)
        {
            yield return null;
            Debug.Log("qle");
        }       
        //1.设置路径
        string filePath = Application.dataPath + "/Resources/pic.png";
        Debug.Log(filePath);
        //2.将资源存放到固定路径
        byte[] bytes = www.texture.EncodeToPNG();
        Debug.Log(bytes.Length);
        File.WriteAllBytes(filePath,bytes);
        //2.2刷新工程资源
        AssetDatabase.Refresh(ImportAssetOptions.Default);
        //3.将工程路径中的资源，格式转换为Sprite
        TextureImporter ti = (TextureImporter)AssetImporter.GetAtPath("Assets/Resources/pic.png");
        ti.textureType = TextureImporterType.Sprite;
        AssetDatabase.ImportAsset("Assets/Resources/pic.png");
        //4.将Sprite添加到Image组件上
        GetComponent<Image>().sprite = Resources.Load<Sprite>("pic");
    }
}
