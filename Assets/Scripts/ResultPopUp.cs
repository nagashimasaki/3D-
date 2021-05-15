using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResultPopUp : MonoBehaviour
{
    [SerializeField]
    private Button btnRetry;                      

    [SerializeField]
    private CanvasGroup canvasGroupTxt;

    [SerializeField]
    private CanvasGroup canvasGroupPopUp;

    [SerializeField]
    private Image imgTitle;

    private bool isClickable;


    void Start()
    {
        // 各アルファ値を 0 にして、リザルト表示を非表示にしておく
        canvasGroupPopUp.alpha = 0;
        canvasGroupTxt.alpha = 0;

        // ボタンの OnClick イベントにメソッドを登録
        btnRetry.onClick.AddListener(OnClickRetry);

        // ボタンを非活性化して押せない状態にする
        btnRetry.interactable = false;

        // ボタンの連打防止用の判定値を未タップ状態にする
        isClickable = false;
    }

    /// <summary>
    /// リザルト表示を行う
    /// </summary>
    public void DisplayResult()
    {
        // CanvasGroup のアルファを変更してリザルト表示。OnComplete メソッドを利用すると、DOFadeメソッドの処理を終わった後に登録してある処理を自動的に実行してくれる
        canvasGroupPopUp.DOFade(1.0f, 1.0f)
.OnComplete(() =>
{
    // ボタンを活性化して押せるようにする
    btnRetry.interactable = true;

    // リトライの文字を点滅
    canvasGroupTxt.DOFade(1.0f, 1.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
});

        // タイトルの大きさを変更するため、一旦、現在の大きさを変数に代入して保持する
        Vector3 scale = imgTitle.transform.localScale;

        // タイトルの大きさを 0 にして非表示状態にする
        imgTitle.transform.localScale = Vector3.zero;

        // シーケンスの宣言
        Sequence sequence = DOTween.Sequence();

        // 1秒待つ
        sequence.AppendInterval(1.0f);

        // タイトルの大きさを 0.25 秒かけて 1.5 倍にする
        sequence.Append(imgTitle.transform.DOScale(1.5f, 0.25f));

        // 前の Append メソッドが終了してから、このメソッドが処理される
        // タイトルの大きさを 0.15 秒かけて元の大きさに戻す　=>　一瞬大きくなる演出になる
        sequence.Append(imgTitle.transform.DOScale(scale, 0.15f));
    }

    /// <summary>
    /// リザルトをタップした際の処理
    /// </summary>
    private void OnClickRetry()
    {

        // すでにタップ済の場合
        if (isClickable == true)
        {
            // 重複防止のためリトライ処理を行わない
            return;
        }

        // タップ済にする。以降は、上にある if 文の制御により、タップしてもこちらの処理にはこなくなる
        isClickable = true;

        // リトライ処理
        StartCoroutine(Retry());
    }

    /// <summary>
    /// リトライ
    /// </summary>
    /// <returns></returns>
    private IEnumerator Retry()
    {

        // リザルトを徐々に非表示にする
        canvasGroupPopUp.DOFade(0, 1.0f);

        // リザルトが非表示になるまで待機
        yield return new WaitForSeconds(1.0f);

        // 現在と同じシーンを読み込む
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
