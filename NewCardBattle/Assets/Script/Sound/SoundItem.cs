using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundItem : MonoBehaviour
{
    /// <summary>
    /// 音频组件
    /// </summary>
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="audioClip">音频</param>
    /// <param name="isLoop">循环</param>
    public void Play(AudioClip audioClip, bool isLoop)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.loop = isLoop;
    }

    /// <summary>
    /// 播放音频
    /// </summary>
    /// <param name="name">音频名</param>
    /// <param name="isLoop">循环</param>
    public void Play(string name, bool isLoop = false)
    {
        AudioClip audioClip = ResourcesManager.instance.Load(name) as AudioClip;
        Play(audioClip, isLoop);
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
    }

    /// <summary>
    /// 检查是否正在播放
    /// </summary>
    /// <returns></returns>
    public bool CheckIsPlaying()
    {
        return audioSource.isPlaying;
    }

    /// <summary>
    /// 当前音量
    /// </summary>
    public float GetCurrentVolume()
    {
        return audioSource.volume;
    }
    /// <summary>
    /// 音量变化
    /// </summary>
    /// <param name="volume"></param>
    public void ChangeVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
