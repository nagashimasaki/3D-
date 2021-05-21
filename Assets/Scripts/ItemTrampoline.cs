using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemTrampoline : MonoBehaviour
{

    [SerializeField, Header("移動する時間と距離をランダムにする割合"), Range(0, 100)]
    private int randomMovingPercentTrampoline;

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
    private float[] trampolineSizes;
    private BoxCollider boxCol;

    [SerializeField, Header("跳ねたときの空気抵抗値")]
    private float airResistance;

    void Start()
    {
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
        // 指定されたタグのゲームオブジェクトが接触した場合には、判定を行わない
        if (col.gameObject.tag == "Water" || col.gameObject.tag == "FlowerCircle")
        {
            return;
        }

        // 侵入してきたゲームオブジェクトが PlayerController スクリプトを持っていたら取得
        if (col.gameObject.TryGetComponent(out PlayerController player))
        {

            // バウンドさせる
            Bound(player);
        }
    }

    /// <summary>
    /// バウンドさせる
    /// </summary>
    /// <param name="player"></param>
    private void Bound(PlayerController player)
    {

        // コライダーをオフにして重複判定を防止する
        boxCol.enabled = false;

        // キャラを上空にバウンドさせる(操作は可能)
        player.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * Random.Range(800, 1000), ForceMode.Impulse);

        // キャラを回転させる(回転させる方向を色々と変えてみましょう！)
        player.transform.DORotate(new Vector3(90, 1080, 0), 1.5f, RotateMode.FastBeyond360)
            .OnComplete(() => {
                // しばらくの間、落下速度をゆっくりにする
                player.DampingDrag(airResistance);
            });

        Destroy(gameObject);
    }

    public void SetUpMovingTrampoline(bool isMoving, bool isScaleChanging)
    {
        // 移動するアイテムか、通常のアイテムかの設定
        isMoving = isMoving;

        // 移動する場合
        if (isMoving)
        {
            // ランダムな移動時間や距離を使うか、戻り値を持つメソッドを利用して判定
            if (DetectRandomMovingFromPercentTrampoline())
            {
                // ランダムの場合には、移動時間と距離のランダム設定を行う
                ChangeRandomMoveParametersTrampoline();
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
    private bool DetectRandomMovingFromPercentTrampoline()
    {
        // 処理結果を  bool 値で戻す。randomMovingPercent の値よりも大きければ、false、同じか小さければ true
        return Random.Range(0, 100) <= randomMovingPercentTrampoline;
    }

    /// <summary>
    /// ランダム値を取得して移動
    /// </summary>
    private void ChangeRandomMoveParametersTrampoline()
    {
        // 移動時間をランダム値の範囲で設定
        duration = Random.Range(durationRange.x, durationRange.y);

        // 移動距離をランダム値の範囲で設定
        moveDistance = Random.Range(moveDistanceRange.x, moveDistanceRange.y);
    }

    /// <summary>
    /// 大きさを変更
    /// </summary>
    private void ChangeRandomScales()
    {
        // ランダム値の範囲内で大きさを設定
        int index = Random.Range(0, trampolineSizes.Length);

        // 大きさを変更
        transform.localScale *= trampolineSizes[index];

    }
}
