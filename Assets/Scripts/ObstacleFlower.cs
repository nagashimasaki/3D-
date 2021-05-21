using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ObstacleFlower : MonoBehaviour
{
    private Animator anim;
    private BoxCollider boxCol;

    [SerializeField, Header("障害物に食べられた時のSE")]
    private AudioClip obstacleSE = null;

    [SerializeField, Header("移動する時間と距離をランダムにする割合"), Range(0, 100)]
    private int randomMovingPercentObstacle;

    [SerializeField, Header("移動時間")]
    private float duration;

    [SerializeField, Header("移動時間のランダム幅")]
    private Vector2 durationRange;

    [SerializeField, Header("移動距離")]
    private float moveDistance;

    [SerializeField, Header("移動させる場合スイッチ入れる")]
    private bool isMoveing;

    [SerializeField, Header("移動距離のランダム幅")]
    private Vector2 moveDistanceRange;

    [SerializeField, Header("大きさの設定")]
    private float[] obstacleFlowerSizes;

    void Start()
    {
        anim = GetComponent<Animator>();
        boxCol = GetComponent<BoxCollider>();

        // この障害物が移動する花輪の設定なら
        if (isMoveing)
        {

            // 前後にループ移動させる
            transform.DOMoveZ(transform.position.z + moveDistance, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnTriggerEnter(Collider col)
    {

        //障害物に食べられた時のSE
        AudioSource.PlayClipAtPoint(obstacleSE, transform.position);

        // 指定されたタグのゲームオブジェクトが侵入した場合には、判定を行わない
        if (col.gameObject.tag == "Water" || col.gameObject.tag == "FlowerCircle")
        {
            return;
        }

        // 侵入してきた col.gameObject(つまり、Penguin ゲームオブジェクト)に対して、TryGetComponent メソッドを実行し、PlayerController クラスの情報を取得できるか判定する
        if (col.gameObject.TryGetComponent(out PlayerController player))
        {

            // PlayerController クラスを取得出来た場合のみ、この if 文の中の処理が実行される
            // 食べる
            StartCoroutine(EatingTarget(player));
        }
    }

    /// <summary>
    /// 対象を食べて吐き出す
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator EatingTarget(PlayerController player)
    {

        // コライダーをオフにして重複判定を防止する
        boxCol.enabled = false;

        // キャラを口の中央に移動させる　　　　　　　
        player.transform.SetParent(transform);
        player.transform.localPosition = new Vector3(0, -2.0f, 0);
        player.transform.SetParent(null);

        // 食べるアニメ再生
        anim.SetTrigger("attack");

        // キャラの移動を一時停止し、キー入力を受け付けない状態にする
        player.StopMove();

        // 食べているアニメの時間の間だけ処理を中断
        yield return new WaitForSeconds(0.75f);

        // キャラを移動できる状態に戻す
        player.ResumeMove();

        // キャラを上空に吐き出す
        player.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 300, ForceMode.Impulse);

        // キャラを回転させる
        player.transform.DORotate(new Vector3(180, 0, 1080), 0.5f, RotateMode.FastBeyond360);

        // スコアを半分にする
        player.HalveScore();

        // 小さくなりながら消す
        transform.DOScale(Vector3.zero, 0.5f);
        Destroy(gameObject, 0.5f);
    }

    public void SetUpMovingObstacleFlower(bool isMoving, bool isScaleChanging)
    {
        // 移動する障害物か、通常の障害物かの設定
        isMoving = isMoving;

        // 移動する場合
        if (isMoving)
        {
            // ランダムな移動時間や距離を使うか、戻り値を持つメソッドを利用して判定
            if (DetectRandomMovingFromPercentObstacle())
            {
                // ランダムの場合には、移動時間と距離のランダム設定を行う
                ChangeRandomMoveParametersObstacle();
            }
        }

        // 障害物の大きさを変更する場合
        if (isScaleChanging)
        {
            // 大きさを変更
            ChangeRandomScales();
        }
    }

    /// <summary>
    /// 移動時間と距離をランダムにするか判定。true の場合はランダムとする
    /// </summary>
    /// <returns></returns>
    private bool DetectRandomMovingFromPercentObstacle()
    {
        // 処理結果を  bool 値で戻す。randomMovingPercent の値よりも大きければ、false、同じか小さければ true
        return Random.Range(0, 100) <= randomMovingPercentObstacle;
    }

    /// <summary>
    /// ランダム値を取得して移動
    /// </summary>
    private void ChangeRandomMoveParametersObstacle()
    {
        // 移動時間をランダム値の範囲で設定
        duration = Random.Range(durationRange.x, durationRange.y);

        // 移動距離をランダム値の範囲で設定
        moveDistance = Random.Range(moveDistanceRange.x, moveDistanceRange.y);
    }

    /// <summary>
    /// 大きさを変更して点数に反映
    /// </summary>
    private void ChangeRandomScales()
    {
        // ランダム値の範囲内で大きさを設定
        int index = Random.Range(0, obstacleFlowerSizes.Length);

        // 大きさを変更
        transform.localScale *= obstacleFlowerSizes[index];

    }
}
