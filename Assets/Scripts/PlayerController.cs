﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Coffee.UIExtensions;     // ShinyEffectForUGUI を利用するために必要な宣言

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]
    public float moveSpeed;

    [Header("落下速度")]
    public float fallSpeed;

    [Header("着水判定用。trueなら着水済")]
    public bool inWater;

    public enum プレイヤーステート
    {
        空中,
        水中,
    }

    public プレイヤーステート playerState;

    // キャラの状態の種類
    public enum AttitudeType
    {
        Straight,        // 直滑降(通常時)
        Prone,           // 伏せ
    }

    [Header("現在のキャラの姿勢")]
    public AttitudeType attitudeType;

    private Rigidbody rb;

    private float x;
    private float z;

    private Vector3 straightRotation = new Vector3(180, 0, 0);     // 頭を下(水面方向)に向ける際の回転角度の値

    private int score;                                             // 花輪を通過した際の得点の合計値管理用

    private Vector3 proneRotation = new Vector3(-90, 0, 0);        // 伏せの姿勢の回転角度の値

    private float attitudeTimer;                                   // 姿勢変更が可能になるまでの計測用タイマー
    private float chargeTime = 2.0f;                               // 姿勢変更が可能になるまでのチャージ(待機)時間

    private bool isCharge;                                         // チャージ完了判定用。false は未完了(チャージ中)、true はチャージ完了

    private Animator anim;

    [SerializeField, Header("水しぶきのエフェクト")]
    private GameObject waterEffectPrefab = null;

    [SerializeField, Header("水しぶきのSE")]
    private AudioClip splashSE = null;

    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Button btnChangeAttitude;

    [SerializeField]
    private GameObject altimeterChangeAttitude;

    [SerializeField]
    private Image imgGauge;

    [SerializeField]
    private ShinyEffectForUGUI shinyEffect;

    [SerializeField]
    private Transform limitLeftBottom;　　　　// 画面左下のゲームオブジェクトの位置情報

    [SerializeField]
    private Transform limitRightTop;          // 画面右上のゲームオブジェクトの位置情報

    public void SetUpPlayer()
    {

        rb = GetComponent<Rigidbody>();

        // 初期の姿勢を設定(頭を水面方向に向ける)
        transform.eulerAngles = straightRotation;

        // 現在の姿勢を「直滑降」に変更(いままでの姿勢)
        attitudeType = AttitudeType.Straight;

        // ボタンのOnClickイベントに ChangeAttitude メソッドを追加する
        btnChangeAttitude.onClick.AddListener(ChangeAttitude);

        // ボタンを非活性化(半透明で押せない状態)
        btnChangeAttitude.interactable = false;

        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (inWater)
        {　　// 条件式に bool 型の変数名を書いた場合、inWater == true を確認しているのと同じ条件になる
            return;
        }

        // キー入力の受付
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        if(rb != null)
        {
            // velocity(速度)に新しい値を代入して移動
            rb.velocity = new Vector3(x * moveSpeed, -fallSpeed, z * moveSpeed);
        }
        
    }

    // IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("反応");
        // 通過したゲームオブジェクトのTagが Water であり、かつ、isWater が false(未着水)であるなら
        if (col.gameObject.tag == "Water" && inWater == false)
        {
            // 着水状態に変更する
            inWater = true;

            // 水しぶきのエフェクトを生成
            GameObject effect = Instantiate(waterEffectPrefab, transform.position, Quaternion.identity);
            effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y, effect.transform.position.z - 0.5f);

            // エフェクトを２秒後に破壊
            Destroy(effect, 2.0f);

            // 水しぶきのSEを再生
            AudioSource.PlayClipAtPoint(splashSE, transform.position);

            

            // コルーチンメソッドである OutOfWater メソッドを呼び出す
            StartCoroutine(OutOfWater());
        }

        // 侵入したゲームオブジェクトの Tag が FlowerCircle なら
        if (col.CompareTag("FlowerCircle"))
        {
            // 侵入した FlowerCircle Tag を持つゲームオブジェクト(Collider)の親オブジェクト(FlowerCircle)にアタッチされている FlowerCircle スクリプトを取得して、point 変数を参照し、得点を加算する
            score += col.transform.parent.GetComponent<FlowerCircle>().point;

            // 画面に表示されている得点表示を更新
            txtScore.text = score.ToString();
        }
        Debug.Log("終わり");
    }

    /// <summary>
    /// 水面に顔を出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator OutOfWater()
    {
        // １秒待つ
        yield return new WaitForSeconds(1.0f);   //  <= yield による処理。yield return new WaitForSecondsメソッドは、引数で指定した秒数だけ次の処理へ移らずに処理を一時停止する処理 

        // Rigidbody コンポーネントの IsKinematic にスイッチを入れてキャラの操作を停止する
        rb.isKinematic = true;

        // キャラの姿勢（回転）を変更する
        transform.eulerAngles = new Vector3(-30, 180, 0);

        // DOTweenを利用して、１秒かけて水中から水面へとキャラを移動させる
        transform.DOMoveY(4.7f, 1.0f);
    }

    void Update()
    {
        if (inWater)
        {
            // ボタンを非活性化して押せない状態にする
            btnChangeAttitude.interactable = false;
            return;
        }

        // 移動範囲内か確認
        LimitMoveArea();

        // スペースキーを押したら
        if (Input.GetKeyDown(KeyCode.Space))
        {

            // 姿勢の変更
            ChangeAttitude();
        }

        // チャージ完了状態ではなく、姿勢が普通の状態
        if (isCharge == false && attitudeType == AttitudeType.Straight)
        {

            // タイマーを加算する = チャージを行う
            attitudeTimer += Time.deltaTime;

            // ゲージ表示を更新
            imgGauge.DOFillAmount(attitudeTimer / chargeTime, 0.1f);

            // ボタンを非活性化(半透明で押せない状態)
            btnChangeAttitude.interactable = false;

            // タイマーがチャージ時間(満タン)になったら
            if (attitudeTimer >= chargeTime)
            {

                // タイマーの値をチャージの時間で止めるようにする
                attitudeTimer = chargeTime;

                // チャージ状態にする
                isCharge = true;

                // ボタンを活性化(押せる状態)
                btnChangeAttitude.interactable = true;

                // 満タン時のエフェクト
                shinyEffect.Play(0.5f);
            }
        }

        // 姿勢が伏せの状態
        if (attitudeType == AttitudeType.Prone)
        {

            // タイマーを減算する = チャージを減らす
            attitudeTimer -= Time.deltaTime;

            // ゲージ表示を更新
            imgGauge.DOFillAmount(attitudeTimer / chargeTime, 0.1f);

            // タイマー(チャージ)が 0 以下になったら
            if (attitudeTimer <= 0)
            {

                // タイマーをリセットして、再度計測できる状態にする
                attitudeTimer = 0;

                // ボタンを非活性化(半透明で押せない状態)
                btnChangeAttitude.interactable = false;

                // 強制的に姿勢を直滑降に戻す
                ChangeAttitude();
            }
        }
    }

    /// <summary>
    /// 姿勢の変更
    /// </summary>
    private void ChangeAttitude()
    {

        // 現在の姿勢に応じて姿勢を変更する
        switch (attitudeType)
        {

            // 現在の姿勢が「直滑降」だったら
            case AttitudeType.Straight:

                // 未チャージ状態(チャージ中)なら
                if (isCharge == false)
                {

                    // 以降の処理を行わない = 未チャージ状態なので、チャージ時の処理を行えないようにする
                    return;
                }

                // チャージ状態を未チャージ状態にする
                isCharge = false;

                // 現在の姿勢を「伏せ」に変更
                attitudeType = AttitudeType.Prone;

                // キャラを回転させて「伏せ」にする
                transform.DORotate(proneRotation, 0.25f, RotateMode.WorldAxisAdd);

                // 空気抵抗の値を上げて落下速度を遅くする
                rb.drag = 25.0f;

                // ボタンの子オブジェクトの画像を回転させる
                btnChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), 0.25f);

                altimeterChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 90), 0.25f);

                // 伏せの状態に遷移するための条件を指定する  => idle から stan のアニメーションに遷移する
                anim.SetBool("Prone", true);

                // 処理を抜ける(次の case には処理が入らない)
                break;

            // 現在の姿勢が「伏せ」だったら
            case AttitudeType.Prone:

                // 現在の姿勢を「直滑降」に変更
                attitudeType = AttitudeType.Straight;

                // キャラを回転させて「直滑降」にする
                transform.DORotate(straightRotation, 0.25f);

                // 空気抵抗の値を元に戻して落下速度を戻す
                rb.drag = 0f;

                // ボタンの子オブジェクトの画像を回転させる
                btnChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 90), 0.25f);

                altimeterChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), 0.25f);

                // 伏せの状態を止めるための遷移の条件を指定する => stan から idle に遷移する
                anim.SetBool("Prone", false);

                // 処理を抜ける
                break;
        }
    }

    /// <summary>
    /// 移動範囲の確認と制限
    /// </summary>
    private void LimitMoveArea()
    {

        // 現在のXの位置が移動範囲内に収まっているか確認し、超えていた場合には下限(左端)か上限(右端)に合わせる
        float limitX = Mathf.Clamp(transform.position.x, limitLeftBottom.position.x, limitRightTop.position.x);

        // 現在のZの位置が移動範囲内に収まっているか確認し、超えていた場合には下限(手前側)か上限(奥側)に合わせる
        float limitZ = Mathf.Clamp(transform.position.z, limitLeftBottom.position.z, limitRightTop.position.z);

        // 制限値内になるように位置情報を更新
        transform.position = new Vector3(limitX, transform.position.y, limitZ);
    }

    /// <summary>
    /// キャラの落下と移動を一時停止
    /// </summary>
    public void StopMove()
    {

        // キャラのゲームオブジェクトを物理演算の影響を受けない状態にする(重力の影響を受けない)
        rb.isKinematic = true;

        // キャラの速度を 0 にして停止する
        rb.velocity = Vector3.zero;
    }


    /// <summary>
    /// キャラの落下と移動を再開
    /// </summary>
    public void ResumeMove()
    {

        // キャラのゲームオブジェクトを物理演算の影響を受ける状態に戻す(再び重力の影響を受けるようになる)
        rb.isKinematic = false;

        // キャラに落下速度を設定する
        rb.velocity = new Vector3(0, -fallSpeed, 0);
    }


    /// <summary>
    /// スコアを半分にする
    /// </summary>
    public void HalveScore()
    {

        // スコアを半分にする
        score = Mathf.CeilToInt(score * 0.5f);

        Debug.Log("スコア半分 : " + score);

        // 画面のスコア表示を更新
        txtScore.text = score.ToString();
    }

    /// <summary>
    /// 落下速度を減衰させながら元に戻す
    /// </summary>
    /// <param name="airResistance"></param>
    public void DampingDrag(float airResistance)
    {

        // 空気抵抗の値を更新
        rb.drag = airResistance;

        // 3 秒かけて空気抵抗の値を 0 にする
        DOTween.To(() => rb.drag, (x) => rb.drag = x, 0, 3.0f)
            .OnComplete(() => {

                // DOTween.To メソッドの処理が終了したら呼ばれる処理

                // もしも直滑降の姿勢でなければ
                if (transform.rotation.x != 1)
                {

                    // 直滑降の姿勢に戻す
                    transform.DORotate(straightRotation, 0.25f);
                }
            });
    }
}
