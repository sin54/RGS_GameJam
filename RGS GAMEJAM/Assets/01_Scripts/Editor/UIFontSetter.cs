using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIFontSetter : MonoBehaviour
{
    //폰트 경로 설정.
    public const string PATH_FONT_UITEXT_JALNAN = "Assets/Fonts/Jalnan.ttf";
    public const string PATH_FONT_TEXTMESHPRO_JALNAN = "Assets/ETC/Fonts/SB 어그로 B SDF.asset";

    [MenuItem("CustomMenu/ChangeUITextFont")]
    public static void ChangeFontInUIText()
    {
        GameObject[] rootObj = GetSceneRootObjects();

        for (int i = 0; i < rootObj.Length; i++)
        {
            GameObject gbj = (GameObject)rootObj[i] as GameObject;
            Component[] com = gbj.transform.GetComponentsInChildren(typeof(Text), true);
            foreach (Text txt in com)
            {
                txt.font = AssetDatabase.LoadAssetAtPath<Font>(PATH_FONT_UITEXT_JALNAN);
            }
        }
    }

    [MenuItem("CustomMenu/ChangeTextMeshPro")]
    public static void ChangeFontInTextMeshPro()
    {
        GameObject[] rootObj = GetSceneRootObjects();

        for (int i = 0; i < rootObj.Length; i++)
        {
            GameObject gbj = (GameObject)rootObj[i] as GameObject;
            Component[] com = gbj.transform.GetComponentsInChildren(typeof(TextMeshProUGUI), true);
            foreach (TextMeshProUGUI txt in com)
            {
                txt.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PATH_FONT_TEXTMESHPRO_JALNAN);
            }
        }
    }

    /// <summary>
    /// 모든 최상위 Root의 GameObject를 받아옴.
    /// </summary>
    /// <returns></returns>
    private static GameObject[] GetSceneRootObjects()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        return currentScene.GetRootGameObjects();
    }
}
