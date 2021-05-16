using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField, Header("BGM用オーディオファイル")]
    private AudioClip[] bgms = null;

    // BGM再生用コンポーネント
    private AudioSource audioSource;         

    // BGMの種類を登録
    public enum BgmType
    {
        Main,          // 自動的に 0 が割り振られている
        GameClear      // こちらも同様に、1 が割り振られている
    }

    void Start()
    {

        // コンポーネントを取得して代入する
        audioSource = GetComponent<AudioSource>();

        // ゲーム中のBGMを再生
        PlayBGM(BgmType.Main);
    }

    /// <summary>
    /// 指定した種類のBGMを再生
    /// </summary>
    /// <param name="bgmType"></param>
    public void PlayBGM(BgmType bgmType)
    {
        // BGM停止
        audioSource.Stop();

        // 再生するBGMを設定する
        audioSource.clip = bgms[(int)bgmType];

        // BGM再生
        audioSource.Play();

        Debug.Log("再生中のBGM : " + bgmType);
    }
}
