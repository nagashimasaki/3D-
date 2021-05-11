using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移動速度")]          // Header属性を変数の宣言に追加すると、インスペクター上に( )内に記述した文字が表示されます。
    public float moveSpeed;

    [Header("落下速度")]
    public float fallSpeed;

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
        Debug.Log(x);
        Debug.Log(z);

        // velocity(速度)に新しい値を代入して移動
        rb.velocity = new Vector3(x * moveSpeed, -fallSpeed, z * moveSpeed);

        // velocityの値の確認
        Debug.Log(rb.velocity);
    }
}
