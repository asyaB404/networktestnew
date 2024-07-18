using System.Collections.Generic;
using UnityEngine;

public class MusicMgrForRes
{
    private GameObject _soundObj = null;
    private readonly List<AudioSource> _soundList = new();
    private float _soundValue = 1;

    /// <summary>
    /// 音效大小(0-1)
    /// </summary>
    public float SoundValue
    {
        set
        {
            _soundValue = value;
            foreach (var source in _soundList)
            {
                if (source)
                {
                    source.volume = value;
                }
            }
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isLoop"></param>
    /// <param name="count"></param>
    public AudioSource PlaySound(string name, bool isLoop = false, int count = 1)
    {
        if (_soundObj == null)
        {
            _soundObj = new GameObject("soundOBJ");
            if (Camera.main != null) _soundObj.transform.position = Camera.main.transform.position;
        }

        var source = _soundObj.AddComponent<AudioSource>();
        AudioClip audioClip;
        name = "Sounds/" + name;
        if (count >= 2)
            name += MyRandom.NextInt(1, count + 1);
        audioClip = ResourcesMgr.Instance.LoadRes<AudioClip>(name);
        source.clip = Object.Instantiate(audioClip);
        _soundList.Add(source);
        source.volume = _soundValue;
        source.loop = isLoop;
        source.Play();
        return source;
    }

    /// <summary>
    /// 停止音效
    /// </summary>
    /// <param name="source"></param>
    public void StopSound(AudioSource source)
    {
        if (!_soundList.Contains(source)) return;
        source.Stop();
        _soundList.Remove(source);
        Object.Destroy(source);
    }

    private AudioSource _bkMusic = null;
    private float _bkValue = 1;

    public float BkValue
    {
        set
        {
            _bkValue = value;
            if (_bkMusic)
            {
                _bkMusic.volume = _bkValue;
            }
        }
    }

    private static MusicMgrForRes _instance;

    public static MusicMgrForRes Instance
    {
        get
        {
            _instance ??= new MusicMgrForRes();
            _instance.Check();
            return _instance;
        }
    }


    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name"></param>
    public void PlayBkMusic(string name)
    {
        if (_bkMusic == null)
        {
            _bkMusic = new GameObject("BkMusic").AddComponent<AudioSource>();
            if (Camera.main != null) _bkMusic.transform.position = Camera.main.transform.position;
        }

        name = "Sounds/" + name;
        var audioClip = ResourcesMgr.Instance.LoadRes<AudioClip>(name);
        _bkMusic.clip = Object.Instantiate(audioClip);
        _bkMusic.volume = _bkValue;
        _bkMusic.loop = true;
        _bkMusic.Play();
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic()
    {
        if (_bkMusic == null)
            return;
        _bkMusic.Stop();
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseBkMusic()
    {
        if (_bkMusic == null)
            return;
        _bkMusic.Pause();
    }

    private void Check()
    {
        for (int i = _soundList.Count - 1; i >= 0; i--)
        {
            if (!_soundList[i] || _soundList[i].isPlaying) continue;
            Object.Destroy(_soundList[i]);
            _soundList.RemoveAt(i);
        }
    }
}