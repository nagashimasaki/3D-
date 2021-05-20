using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField]
    private Material[] skyboxMaterials;   // Skybox 用のマテリアルを登録するための配列

    [SerializeField, Header("Skybox設定用の指定値。999 の場合、ランダムにする")]
    private int skyboxMaterialsIndex;

    /// <summary>
    /// Skyboxを変更
    /// </summary>
    public void ChangeSkybox()
    {

        // ランダム設定の場合
        if (skyboxMaterialsIndex == 999)
        {

            // Skybox をランダムな要素番号のマテリアルの Skybox に変更
            RenderSettings.skybox = skyboxMaterials[RandomSelectIndexOfSkyboxMaterials()];
        }
        else
        {

            // Skybox を指定された要素番号のマテリアルの Skybox に変更
            RenderSettings.skybox = skyboxMaterials[skyboxMaterialsIndex];
        }

        Debug.Log("Skybox 変更");
    }

    /// <summary>
    /// Skyboxのランダムな要素番号を取得
    /// </summary>
    /// <returns></returns>
    public int RandomSelectIndexOfSkyboxMaterials()
    {

        return Random.Range(0, skyboxMaterials.Length);
    }

}
