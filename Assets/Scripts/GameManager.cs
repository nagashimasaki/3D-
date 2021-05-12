using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;     // キャラの位置情報

    [SerializeField]
    private Transform goal = null;       // ゴール地点(水面)の位置情報

    private float distance;              // キャラと水面までの距離の計測用

    private bool isGoal;                 // ゴール判定用。距離が 0 以下になったらゴールと判定して true にする。false の間はゴールしていない状態(着水判定と同じ bool 型の利用方法)

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

        // 距離が 0 以下になったら(条件を distance == 0 とはしないようにします。なぜなら、distance の値が 0 ピッタリに値が止まることが出来ないためです)
        if (distance <= 0)
        {

            // 距離が 0 以下になったので、ゴールと判定する
            isGoal = true;

            // 距離を 0 にする(この処理がないと、distance の値がマイナス方向になります。試しに書かないで処理を実行してみましょう)
            distance = 0;

            // TODO 着水した際の処理を追加する

        }

        // Consoleビューに距離を表示する
        Debug.Log(distance.ToString("F2"));

    }
}
