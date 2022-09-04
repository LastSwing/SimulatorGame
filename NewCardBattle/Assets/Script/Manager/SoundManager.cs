using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    /// <summary>
    /// 声音存放节点
    /// </summary>
    Transform SoundRoot;

    /// <summary>
    /// 唯一声音<轨道，声音>
    /// </summary>
    Dictionary<int, SoundItem> OnlyOneSound = new Dictionary<int, SoundItem>();

    /// <summary>
    /// 任意声音
    /// </summary>
    List<SoundItem> Sounds = new List<SoundItem>();

    /// <summary>
    /// 播放唯一声音
    /// </summary>
    /// <param name="soundName">声音名</param>
    /// <param name="track">声音轨道，可以播多个轨道的唯一声音</param>
    /// <param name="isLoop">循环</param>
    public void PlayOnlyOneSound(string soundName, int track = 1, bool isLoop = false)
    {
        if (!OnlyOneSound.ContainsKey(track))
        {
            OnlyOneSound[track] = CreateSoundItem();
        }
        else
        {
            if (OnlyOneSound[track].CheckIsPlaying())
            {
                OnlyOneSound[track].Stop();
            }
        }
        OnlyOneSound[track].Play(soundName, isLoop);
    }
    /// <summary>
    /// 任意声音播放，不可循环，循环必须用唯一声音
    /// </summary>
    /// <param name="soundName"></param>
    public void PlaySound(string soundName)
    {
        int freeIndex = -1;
        for (int i = 0; i < Sounds.Count; i++)
        {
            if (!Sounds[i].CheckIsPlaying())
            {
                freeIndex = i;
                break;
            }
        }
        if (freeIndex == -1)
        {
            SoundItem soundItem = CreateSoundItem();
            soundItem.Play(soundName);
            Sounds.Add(soundItem);
        }
        else
        {
            Sounds[freeIndex].Play(soundName);
        }
    }

    /// <summary>
    /// 停止所有轨道唯一声音
    /// </summary>
    public void StopOnlyOneSound()
    {
        foreach (var v in OnlyOneSound)
        {
            v.Value.Stop();
        }
    }

    /// <summary>
    /// 停止指定轨道的唯一声音
    /// </summary>
    /// <param name="track"></param>
    public void StopOnlyOneSound(int track)
    {
        OnlyOneSound[track].Stop();
    }

    /// <summary>
    /// 停止所有一次性声音
    /// </summary>
    public void StopOnceSound()
    {
        foreach (var v in Sounds)
        {
            v.Stop();
        }
    }

    /// <summary>
    /// 停止所有声音
    /// </summary>
    public void StopAllSound()
    {
        StopOnceSound();
        StopOnlyOneSound();
    }

    /// <summary>
    /// 创建一个声音
    /// </summary>
    /// <returns></returns>
    private SoundItem CreateSoundItem()
    {
        GameObject go = ResourcesManager.instance.Load("SoundItem") as GameObject;
        return GameObjectPool.instance.GetObject(go, SoundRoot).GetComponent<SoundItem>();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        SoundRoot = GameObject.Find("SoundRoot").transform;
    }

    /// <summary>
    /// 变化当前背景音量大小
    /// </summary>
    /// <param name="vloume">音量大小</param>
    /// <param name="track">声音轨道，可以播多个轨道的唯一声音</param>
    public void ChangeMusicVolume(float vloume, int track = 1)
    {
        OnlyOneSound[track].ChangeVolume(vloume);
    }

    /// <summary>
    /// 当前音量
    /// </summary>
    /// <param name="track">声音轨道，可以播多个轨道的唯一声音</param>
    public float CurrentVolume(int track = 1)
    {
        if (!OnlyOneSound.ContainsKey(track))
        {
            OnlyOneSound[track] = CreateSoundItem();
        }
        return OnlyOneSound[track].GetCurrentVolume();
    }
}
public enum TrackType
{
    BGM,
    Talk,
    Voice
}