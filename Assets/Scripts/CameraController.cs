using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // カメラが追従する対象のゲームオブジェクト。今回はペンギン
    [SerializeField]
    private PlayerController playerController;

    // 一人称カメラの Camera コンポーネントの代入用

    [SerializeField]
    private Camera fpsCamera;

    // 自撮りカメラの Camera コンポーネントの代入用
    [SerializeField]
    private Camera selfishCamera;

    // カメラの制御用ボタンの Button コンポーネントの代入用
    [SerializeField]
    private Button btnChangeCameara;

    // 現在適用しているカメラの通し番号
    private int cameraIndex;

    // Main Camera ゲームオブジェクトの Camera コンポーネントの代入用
    private Camera mainCamera;              


    //[SerializeField]
    //private GameObject playerObj;

    // カメラが追従する対象との間の取る、一定の距離用の補正値
    private Vector3 offset;         

    void Start()
    {
        // カメラと追従対象のゲームオブジェクトとの距離を補正値として取得
        offset = transform.position - playerController.transform.position;

        // MainCamera Tag を持つゲームオブジェクト(MainCamera ゲームオブジェクト)の Camera コンポーネントを取得して代入
        mainCamera = Camera.main;

        // ボタンのOnClickイベントにメソッドを登録
        btnChangeCameara.onClick.AddListener(ChangeCamera);

        // カメラを初期カメラ(三人称カメラ)に戻す 
        SetDefaultCamera();

    }

    void Update()
    {
        // 着水状態になったら
        if (playerController.inWater == true) 
        {
            //ここから下の処理は動かなくなる
            return;
        }

        //追従対象がいる場合
        if (playerController != null)
        {
            // カメラの位置を追従対象の位置 + 補正値にする
            transform.position = playerController.transform.position + offset;
        }
    }

    /// <summary>
    /// カメラを変更(ボタンを押すたびに呼び出される)
    /// </summary>
    private void ChangeCamera()
    {

        // 現在のカメラの通し番号に応じて、次のカメラを用意して切り替える
        switch (cameraIndex)
        {
            case 0:
                cameraIndex++;
                mainCamera.enabled = false;
                fpsCamera.enabled = true;
                break;
            case 1:
                cameraIndex++;
                fpsCamera.enabled = false;
                selfishCamera.enabled = true;
                break;
            case 2:
                cameraIndex = 0;
                selfishCamera.enabled = false;
                mainCamera.enabled = true;
                break;
        }
    }

    /// <summary>
    /// カメラを初期カメラ(三人称カメラ)に戻す
    /// </summary>
    public void SetDefaultCamera()
    {
        cameraIndex = 0;

        mainCamera.enabled = true;
        fpsCamera.enabled = false;
        selfishCamera.enabled = false;
    }

}
