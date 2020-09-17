using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 音效控制脚本 单例 
/// </summary>
public class SoundController : MonoBehaviour
{

    private static SoundController instance;

    public AudioSource audio; //短暂音效播放  
    public AudioSource bgmAudio; //bgm音效播放

    /// <summary>
    /// 音效字典集合 key = 路径
    /// </summary>
    private Dictionary<string, AudioClip> soundDic = new Dictionary<string, AudioClip>();

    //音效音量(0-1)
    public float audioVolume;
    //背景音乐音量(0-1)
    public float bgmVolume;

    public static SoundController Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
        //初始化
        AudioSource[] audios = GetComponents<AudioSource>();
        bgmAudio = audios[0];
        audio = audios[1];
    }

    /// <summary>
    /// 播放BGM
    /// </summary>
    /// <param name="path"></param>
    public void PlayBGM(string path)
    {
        StartCoroutine(FIFO(LoadAudioClipByPath(path)));
    }
    /// <summary>
    /// BGM音量淡入淡出
    /// </summary>
    private IEnumerator FIFO(AudioClip clip)
    {
        //bgm喇叭 有音频资源
        if (bgmAudio.clip != null)
        {
            //从大到小 用1s的时间将音量降低到0
            for (int i = 0; i <= 10; i++)
            {
                bgmAudio.volume = Mathf.Lerp(bgmVolume, 0, 0.1f * i);
                yield return new WaitForSeconds(0.1f);
            }
        }
        //更换音频资源
        bgmAudio.clip = clip;
        //播放
        bgmAudio.Play();
        //从小到大
        for (int i = 0; i <= 10; i++)
        {
            bgmAudio.volume = Mathf.Lerp(0, bgmVolume, 0.1f * i);
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 播放短暂音效
    /// </summary>
    /// <param name="path"></param>
    public void PlayAudio(string path)
    {
        AudioClip clip = LoadAudioClipByPath(path);
        //播放一次
        audio.PlayOneShot(clip, audioVolume);
    }

    /// <summary>
    /// 载入一个音频
    /// </summary>
    private AudioClip LoadAudioClipByPath(string path)
    {

        if (!soundDic.ContainsKey(path))
        {
            AudioClip clip = Resources.Load<AudioClip>("Audio/" + path);
            soundDic.Add(path, clip);
        }
        return soundDic[path];
    }

}
