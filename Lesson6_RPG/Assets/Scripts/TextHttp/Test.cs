using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public AudioSource audio;
    //电影纹理  
    public MovieTexture movTexture;

    void Start()
    {
        audio = GetComponent<AudioSource>();
        //movTexture.loop = true;//设置电影纹理播放模式为循环 
        movTexture.loop = false;
        movTexture.Play();
        RawImage ri = gameObject.GetComponent<RawImage>();
        ri.texture = movTexture;
        audio.Play();
    }

}
