using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // カメラが追従する対象のゲームオブジェクト。今回はペンギン
    [SerializeField]
    private PlayerController playerController;

    //[SerializeField]
    //private GameObject playerObj;

    // カメラが追従する対象との間の取る、一定の距離用の補正値
    private Vector3 offset;         

    void Start()
    {
        // カメラと追従対象のゲームオブジェクトとの距離を補正値として取得
        offset = transform.position - playerController.transform.position;
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
}
