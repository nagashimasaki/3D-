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

    void Start()
    {
        // アタッチしたゲームオブジェクト(花輪)を回転させる
        transform.DORotate(new Vector3(0, 360, 0), 5.0f, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
    }

    /// <summary>
    /// 花輪からみて、他のゲームオブジェクトが花輪に侵入した場合
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {

        // 花輪の BoxCollider のスイッチをオフにして重複判定を防止
        boxCollider.enabled = false;

        // 花輪をキャラの子オブジェクトにする
        transform.SetParent(other.transform);

        // コルーチンメソッドの呼び出し命令に変更する
        // 花輪をくぐった際の演出
        StartCoroutine(PlayGetEffect());
    }


    /// <summary>
    /// 花輪をくぐった際の演出
    /// </summary>
    private IEnumerator PlayGetEffect()
    {

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

        // 花輪を1秒後に破棄
        Destroy(gameObject, 1.0f);
    }
}
