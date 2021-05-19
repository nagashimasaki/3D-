using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlowerCircle : MonoBehaviour
{
    [Header("花輪通過時の得点")]
    public int point;

    [SerializeField]
    private BoxCollider boxCollider;

    [SerializeField]
    private GameObject effectPrefab;

    [SerializeField]
    private AudioClip flowerSE;　　// 通過時と獲得時で２回別々のSEを鳴らしたい場合には変数を２つ用意する

    [SerializeField, Header("移動させる場合スイッチ入れる")]
    private bool isMoveing;

    [SerializeField, Header("移動時間")]
    private float duration;

    [SerializeField, Header("移動距離")]
    private float moveDistance;


    [SerializeField, Header("移動する時間と距離をランダムにする割合"), Range(0, 100)]
    private int randomMovingPercent;

    [SerializeField, Header("移動時間のランダム幅")]
    private Vector2 durationRange;

    [SerializeField, Header("移動距離のランダム幅")]
    private Vector2 moveDistanceRange;

    [SerializeField, Header("大きさの設定")]
    private float[] flowerSizes;

    [SerializeField, Header("点数の倍率")]
    private float[] pointRate;

    void Start()
    {

        // アタッチしたゲームオブジェクト(花輪)を回転させる
        transform.DORotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

        // この花輪が移動する花輪の設定なら
        if (isMoveing)
        {

            // 前後にループ移動させる
            transform.DOMoveZ(transform.position.z + moveDistance, duration).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }


    // 花輪からみて、他のゲームオブジェクトが花輪に侵入した場合
    private void OnTriggerEnter(Collider other)
    {

        // 水面に触れても通過判定は行わない
        if (other.gameObject.tag == "Water")
        {
            return;
        }

        // 花輪の BoxCollider のスイッチをオフにして重複判定を防止
        boxCollider.enabled = false;

        // 花輪をキャラの子オブジェクトにする
        transform.SetParent(other.transform);

        // 花輪をくぐった際の演出
        StartCoroutine(PlayGetEffect());
    }


    /// <summary>
    /// 花輪をくぐった際の演出
    /// </summary>
    private IEnumerator PlayGetEffect()
    {
        // ①SE再生
        AudioSource.PlayClipAtPoint(flowerSE, transform.position);

        // DOTween の Sequence を宣言して利用できるようにする
        Sequence sequence = DOTween.Sequence();

        // Append を実行すると、引数でDOTweenの処理を実行できる。花輪の Scale を 1秒かけて 0 にして見えなくする
        sequence.Append(transform.DOScale(Vector3.zero, 1.0f));

        // Join を実行することで、Append と一緒にDOTweenの処理を行える。花輪の Scale が1秒かけて 0 になるのと一緒に、プレイヤーの位置に花輪を移動させる
        sequence.Join(transform.DOLocalMove(Vector3.zero, 1.0f));

        // 1秒処理を中断(待機する)
        yield return new WaitForSeconds(1.0f);

        // エフェクトを生成して、Instantiate メソッドの戻り値を effect 変数に代入
        GameObject effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);

        // エフェクトの位置(高さ)を調整する　
        effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y - 1.5f, effect.transform.position.z);

        // 1秒後にエフェクトを破棄(すぐに破棄するとエフェクトがすべて再生されないため)
        Destroy(effect, 1.0f);

        // ②SE再生
        AudioSource.PlayClipAtPoint(flowerSE, transform.position);

        // 花輪を1秒後に破棄
        Destroy(gameObject, 1.0f);
    }

    /// <summary>
    /// 移動する花輪の設定
    /// </summary>
    public void SetUpMovingFlowerCircle(bool isMoving, bool isScaleChanging)
    {
        // 移動する花輪か、通常の花輪かの設定
        isMoving = isMoving;

        // 移動する場合
        if (isMoving)
        {
            // ランダムな移動時間や距離を使うか、戻り値を持つメソッドを利用して判定
            if (DetectRandomMovingFromPercent())
            {
                // ランダムの場合には、移動時間と距離のランダム設定を行う
                ChangeRandomMoveParameters();
            }
        }

        // 花輪の大きさを変更する場合
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
    private bool DetectRandomMovingFromPercent()
    {
        // 処理結果を  bool 値で戻す。randomMovingPercent の値よりも大きければ、false、同じか小さければ true
        return Random.Range(0, 100) <= randomMovingPercent;
    }

    /// <summary>
    /// ランダム値を取得して移動
    /// </summary>
    private void ChangeRandomMoveParameters()
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
        int index = Random.Range(0, flowerSizes.Length);

        // 大きさを変更
        transform.localScale *= flowerSizes[index];

        // 点数を変更
        point = Mathf.CeilToInt(point * pointRate[index]);   // Mathf.CeilToInt メソッドについて調べてみましょう。
    }
}