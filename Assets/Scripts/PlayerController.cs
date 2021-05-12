using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Rigidbody rb;

    private float x;
    private float z;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        // キー入力の受付
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        // キー入力の確認
        //Debug.Log(x);
        //Debug.Log(z);

        // velocity(速度)に新しい値を代入して移動
        rb.velocity = new Vector3(x * moveSpeed, -fallSpeed, z * moveSpeed);

        // velocityの値の確認
        //Debug.Log(rb.velocity);
    }

    /// <summary>
    /// IsTriggerがオンのコライダーを持つゲームオブジェクトを通過した場合に呼び出されるメソッド
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider col)
    {
        //通過したゲームオブジェクトのTagが Water であり、かつ、 inWater が false（未着水）であるなら
        if (col.gameObject.tag == "Water" && inWater == false) 
        {
            //着水状態に変更
            inWater = true;

            //水しぶきのエフェクトを生成して、生成された水しぶきのエフェクトを effect 変数に代入する
            GameObject effect = Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);

            //effect 変数を利用して、エフェクトの位置を調整する
            effect.transform.position = new Vector3(effect.transform.position.x, effect.transform.position.y, effect.transform.position.z - 0.5f);

            //effect 変数を利用して、エフェクトを2秒後に破壊
            Destroy(effect, 2.0f);

            // 水しぶきのSEを再生
            AudioSource.PlayClipAtPoint(splashSE, transform.position);
        }
    }
}
