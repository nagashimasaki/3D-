using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Header属性を変数の宣言に追加すると、インスペクター上に( )内に記述した文字が表示されます。
    [Header("移動速度")]          
    public float moveSpeed;

    [Header("落下速度")]
    public float fallSpeed;

    [Header("着水判定用。trueなら着水済")]
    public bool inWater;

    [SerializeField,Header("水しぶきのエフェクト")]
    private GameObject splashEffectPrefab = null;

    [SerializeField, Header("水しぶきのSE")]
    private AudioClip splashSE = null;

    [SerializeField]
    private Text txtScore;

    [SerializeField]
    private Button btnChangeAttitude;

    //イーナム型
    // キャラの状態の種類
    public enum AttitudeType
    {
        // 直滑降(通常時)
        Straight,

        // 伏せ
        Prone,          
    }

    [Header("現在のキャラの姿勢")]
    public AttitudeType attitudeType;

    private Rigidbody rb;

    // 伏せの姿勢の回転角度の値
    private Vector3 proneRotation = new Vector3(-90, 0, 0);        

    private float x;
    private float z;

    // 花輪を通過した際の得点の合計値管理用
    private int score;      


    // 頭を下(水面方向)に向ける際の回転角度の値
    private Vector3 straightRotation = new Vector3(180, 0, 0);
                
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 初期の姿勢を設定(頭を水面方向に向ける)
        transform.eulerAngles = straightRotation;

        // 現在の姿勢を「直滑降」に変更(いままでの姿勢)
        attitudeType = AttitudeType.Straight;

        // ボタンのOnClickイベントに ChangeAttitude メソッドを追加する
        btnChangeAttitude.onClick.AddListener(ChangeAttitude);

    }

    void FixedUpdate()
    {
        // キー入力の受付
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        // キー入力の確認
        // Debug.Log(x);
        // Debug.Log(z);

        // velocity(速度)に新しい値を代入して移動
        rb.velocity = new Vector3(x * moveSpeed, -fallSpeed, z * moveSpeed);

        // velocityの値の確認
        // Debug.Log(rb.velocity);
    }

    /// <summary>
    /// IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider col)
    {
        // 通過したゲームオブジェクトのTagが Water であり、かつ、 inWater が false（未着水）であるなら
        if (col.gameObject.tag == "Water" && inWater == false) 
        {
            // 着水状態に変更
            inWater = true;

            // 水しぶきのエフェクトを生成して、生成された水しぶきのエフェクトを effect 変数に代入する
            GameObject effect = Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);

            // effect 変数を利用して、エフェクトの位置を調整する
            effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y, effect.transform.position.z - 0.5f);

            // effect 変数を利用して、エフェクトを2秒後に破壊
            Destroy(effect, 2.0f);

            // 水しぶきのSEを再生
            AudioSource.PlayClipAtPoint(splashSE, transform.position);

            // StartCoroutine(呼び出すコルーチン・メソッドの名前(引数))　の書式で記述する
            // コルーチンメソッドである OutOfWater メソッドを呼び出す
            StartCoroutine(OutOfWater());                            　
        }

        // 侵入したゲームオブジェクトの Tag が FlowerCircle なら
        if (col.gameObject.tag == "FlowerCircle") 
        {

            Debug.Log("花輪ゲット");

            // 侵入した FlowerCircle Tag を持つゲームオブジェクト(Collider)の親オブジェクト(FlowerCircle)にアタッチされている FlowerCircle スクリプトを取得して、point 変数を参照し、得点を加算する
            score += col.transform.parent.GetComponent<FlowerCircle>().point;

            // 文字列に追加して int 型や float 型の情報を表示する場合には、ToString()メソッドを省略できます
            Debug.Log("現在の得点 : " + score);

            // 画面に表示されている得点表示を更新
            txtScore.text = score.ToString();

            // TODO 画面に表示されている得点表示を更新する処理を追加する

        }
    }

    /// <summary>
    /// 水面に顔を出す
    /// </summary>
    /// <returns></returns>
    private IEnumerator OutOfWater()
    {
        // yield による処理。yield return new WaitForSecondsメソッドは、引数で指定した秒数だけ次の処理へ移らずに処理を一時停止する処理
        // １秒待つ
        yield return new WaitForSeconds(1.0f);    

        // Rigidbody コンポーネントの IsKinematic にスイッチを入れてキャラの操作を停止する
        rb.isKinematic = true;

        // キャラの姿勢（回転）を変更する
        transform.eulerAngles = new Vector3(-30, 180, 0);

        // DOTweenを利用して、１秒かけて水中から水面へとキャラを移動させる
        transform.DOMoveY(4.7f, 1.0f);
    }

    void Update()
    {

        // スペースキーを押したら
        if (Input.GetKeyDown(KeyCode.Space))
        {

            // 姿勢の変更
            ChangeAttitude();
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

                // 現在の姿勢を「伏せ」に変更
                attitudeType = AttitudeType.Prone;

                // キャラを回転させて「伏せ」にする
                transform.DORotate(proneRotation, 0.25f, RotateMode.WorldAxisAdd);

                // 空気抵抗の値を上げて落下速度を遅くする
                rb.drag = 25.0f;

                // ボタンの子オブジェクトの画像を回転させる
                btnChangeAttitude.transform.GetChild(0).DORotate(new Vector3(0, 0, 180), 0.25f);

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

                // 処理を抜ける
                break;
        }
    }
}
