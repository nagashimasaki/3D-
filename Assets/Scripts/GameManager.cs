using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;     // キャラの位置情報

    [SerializeField]
    private Transform goal = null;       // ゴール地点(水面)の位置情報

    //距離の値を受け取って更新するためのコンポーネントを代入する
    [SerializeField]
    private Text txtDistance;

    [SerializeField]
    private CameraController cameraController;

    [SerializeField]
    private ResultPopUp resultPopUp;

    [SerializeField]
    private AudioManager audioManager;

    // キャラと水面までの距離の計測用
    private float distance;

    // ゴール判定用。距離が 0 以下になったらゴールと判定して true にする。false の間はゴールしていない状態(着水判定と同じ bool 型の利用方法)
    private bool isGoal;                

    void Update()
    {
        // 距離が 0 以下になったらゴールしたと判定して距離の計算は行わないようにする
        if (isGoal == true)
        {
            // return があると、この処理よりも下の処理は処理されない
            return;
        }

        // Y軸が高さの情報なので、双方の高さの値を減算して差分値を距離とする
        distance = player.transform.position.y - goal.position.y;

        // Consoleビューに距離を表示する
        //Debug.Log(distance.ToString("F2"));
        txtDistance.text = distance.ToString("F2");

        // 距離が 0 以下になったら
        if (distance <= 0)
        {
            // 距離が 0 以下になったので、ゴールと判定する
            isGoal = true;

            // 距離を 0 にする
            distance = 0;

            // カメラを初期のカメラに戻す
            cameraController.SetDefaultCamera();

            // リザルト表示
            resultPopUp.DisplayResult();

            // ゲームクリアのBGMを再生する
            audioManager.PlayBGM(AudioManager.BgmType.GameClear);
        }
    }
}
