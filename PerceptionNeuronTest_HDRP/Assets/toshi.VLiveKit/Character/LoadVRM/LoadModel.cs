using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// ビルド時無視
#if UNITY_EDITOR
using UnityEditor.Animations;
using SFB;
#endif

using UniGLTF;
using VRM;
using VRMShaders;


using UnityEngine.Animations;
// using UnityEditor.Animations;

using EVMC4U;
using UniHumanoid;
public class LoadModel : MonoBehaviour
{
    // ロード時のEmissiveIntensity
    [SerializeField]
    private float loadEmissiveIntensity = 1.0f;
    // smoothStepValue
    [SerializeField]
    private float loadSmoothStepValue = 0.5f;
    // rim
    [SerializeField]
    private float loadRimIntensity = 0.8f;

    // punctual
    [SerializeField]
    private float loadPunctualIntensity = 0.1f;
    // エディタモードで使用したい場合に、対象のオブジェクトをインスペクタで選択する
    [SerializeField]
    public GameObject targetObjectInEditor;
    [SerializeField]
    private bool LoatOnStart = false;
    [SerializeField]
    private float defaultEmissionIntensity = 0.0f;
    // デバック用のテキスト
    // [SerializeField]
    // public TMPro.TextMeshProUGUI text;
    [SerializeField]
    public GameObject cameraTarget;
    // このオブジェクトの子に生成する
    [SerializeField]
    public GameObject parentObject;
    public ExternalReceiver externalReceiver = default;
    RuntimeGltfInstance instance;
    // string path = "/path/to/vrmfile.vrm";
    // var path = Application.streamingAssetsPath + "/" + "AvatarSampleA.vrm";
    public Shader shaderToUse; // インスペクタから指定するシェーダ
    
    public BackgroundShaderRenderSettings backgroundShaderRenderSettings;
    public string faceMaterialName;

    private const string VRM_TAG = "VRM";

    [SerializeField]
    private bool isMotionLoad = false; // アニメーションをロードするかどうかのフラグ
    [SerializeField]
    private AnimationClip animationClip; // インスペクタから指定するアニメーションクリップ
#if UNITY_EDITOR
    [SerializeField]
    private AnimatorController animatorController; // インスペクタから指定するAnimatorController
#endif

    // Start is called before the first frame update
    void Start()
    {
        if (LoatOnStart)
        {
            Load();
        }
        // Load();
    }

    // Update is called once per frame
    
    [ContextMenu("LoadVRM")]
    public async void Load()
    {
        // // もしEditorModeであれば、targetObjectInEditorを使う
        // if (Application.isEditor)
        // {
        //     targetObjectInEditor.SetActive(true);
        // }
        Renderer[] renderers;

        // もしプレイモードであれは
        if (Application.isPlaying)
        {
            
        
            var extensions = new[]
            {
            new ExtensionFilter( "VRM Files", "vrm" ),
            new ExtensionFilter( "All Files", "*" ),
            };
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Open VRM File", "", extensions, false);
            if (paths.Length == 0)
            {
                return;
            }
            Debug.Log(paths[0]);
            var PATH = paths[0];
            // シーン内で名前が "VRM" のオブジェクトを検索し、削除する

            // タグの存在を確認し、存在しない場合は追加
            EnsureTagExists(VRM_TAG);

            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("VRM");
            
            // var path = Application.streamingAssetsPath + "/" + "AvatarSampleA.vrm";
            // Debug.Log(path);
            this.instance = await VrmUtility.LoadAsync(PATH, new RuntimeOnlyAwaitCaller());

            var avatar = this.instance.gameObject;
            var humanoid = avatar.AddComponent<Humanoid>();
            humanoid.AssignBonesFromAnimator();

            var head = humanoid.Head;
            // this.instance.EnableUpdateWhenOffscreen();

            // BackgroundShaderRenderSettingsのpublic GameObject targetHeadPositionにheadをいれる
            backgroundShaderRenderSettings.targetHeadPosition = head.gameObject;


            // 位置をｙに1.0上げる
            this.instance.transform.position = new Vector3(0, 0, 0);
            this.instance.transform.SetParent(parentObject.transform, false);
            // VRMというタグを付与する
            this.instance.gameObject.tag = "VRM";
            if (targetObjects != null && targetObjects.Length > 0)
            {
                foreach (GameObject targetObject in targetObjects)
                {
                    Destroy(targetObject);
                }
            }
            this.instance.ShowMeshes();
            externalReceiver.Model = this.instance.gameObject;
            renderers = this.instance.GetComponentsInChildren<Renderer>();

            // Debug.Log($"isMotionLoad: {isMotionLoad}, animatorController: {animatorController != null}");

#if UNITY_EDITOR

            if (isMotionLoad && animatorController != null)
            {
                // すでにAnimatorコンポーネントがある場合は、それを取得して、runtimeAnimatorControllerを設定する
                var animator = avatar.GetComponent<Animator>();
                if (animator == null)
                {
                    animator = avatar.AddComponent<Animator>();
                }
                animator.runtimeAnimatorController = animatorController;
                // apply root motion
                animator.applyRootMotion = true;
                // animator.rootMotionMode = RootMotionMode.RootMotion;
            }
#endif
        }
        else
        {
            renderers = targetObjectInEditor.GetComponentsInChildren<Renderer>();
        }
        
        int testInt = 0;

        #region change renderer
        foreach (var renderer in renderers)
        {
            // skinmeshrendererの場合、rootboneを親オブジェクト（this.instance.gameObject）にする
            if (renderer is SkinnedMeshRenderer)
            {
                SkinnedMeshRenderer smr = renderer as SkinnedMeshRenderer;
                if (Application.isPlaying)
                {
                    smr.rootBone = this.instance.transform;
                }
                else
                {
                    smr.rootBone = targetObjectInEditor.transform;
                }
            }
            
            // マテリアルの名前を格納するリスト
            // List<string> materialNames = new List<string>();
            foreach (var material in renderer.sharedMaterials)
            {
                #region change shader
                if (material == null) continue;

                // materialのshaderの名前がshaderToUseの名前と一致していたら処理をスキップ
                if (material.shader.name == shaderToUse.name)
                {
                    continue;
                }


             
                Debug.Log("Before:" + material.shader.name);
      
                // Get MToon Prperties
                #region Get MToon Properties
                float _tmpBlendMode = 1.0f;
                if (material.HasProperty("_BlendMode"))
                {
                    _tmpBlendMode = material.GetFloat("_BlendMode");
                }
                int _tmpCullMode = 2;
                if (material.HasProperty("_CullMode"))
                {
                    _tmpCullMode = material.GetInt("_CullMode");
                }
                bool isAlphaTestOn = material.IsKeywordEnabled("_ALPHATEST_ON");
                #endregion // Get MToon Properties


                material.shader = shaderToUse;

             
                // material.SetFloat("_SphereNormalIntensity", 0.5f);
                //material.SetFloat("_RimIntensity", 0.8f);
                material.SetFloat("_EmissiveIntensity", loadEmissiveIntensity);
                material.SetFloat("_PunctualLightIntensity", loadPunctualIntensity);
                material.SetFloat("_RimIntensity", loadRimIntensity);
                material.SetFloat("_SmoothStepValue", loadSmoothStepValue);
                string renderTypeDebugText = "";
                // }

                // depth writeをオンにする
                // material.SetInt("_DepthWrite", 1);

                #region Opaque
                if (_tmpBlendMode == 0.0f)
                {
                    renderTypeDebugText = "Opaque";
                    material.SetInt("_CullMode", _tmpCullMode);
                    material.SetInt("_OpaqueCullMode", _tmpCullMode);
                    material.SetInt("_CullModeForward", _tmpCullMode);
              
                    material.DisableKeyword("_ENABLE_FOG_ON_TRANSPARENT");
                    material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.DisableKeyword("_ALPHATEST_ON"); // ok
                    material.renderQueue = 2000; // ok
                    material.SetOverrideTag("RenderType", "");
                    // material.SetInt("_AlphaCutofffEnable", 0);
                    material.SetInt("_AlphaCutofffEnable", 0); // ok
                    material.SetInt("_AlphaDstBlend", 10); // ok
                    material.SetInt("_DstBlend", 0);
                    material.SetInt("_RenderQueueType", 1);
                    material.SetInt("_SurfaceType", 0);
                    material.SetInt("_ZTestDepthEqualForOpaque", 3);
                    material.SetInt("_ZWrite", 1);
                    
                    testInt += 1;


                        
                }
                #endregion

                #region Cutout
                if (_tmpBlendMode == 1.0f)
                {
                    renderTypeDebugText = "Cutout";
                    testInt += 1;
                    
                    material.SetInt("_CullMode", _tmpCullMode);
                    material.SetInt("_OpaqueCullMode", _tmpCullMode);
                    material.SetInt("_CullModeForward", _tmpCullMode);
        
            
                    material.DisableKeyword("_ENABLE_FOG_ON_TRANSPARENT");
                    material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    material.EnableKeyword("_ALPHATEST_ON"); // ok
                    material.renderQueue = 2450; // ok
                    material.SetOverrideTag("RenderType", "TransparentCutout"); // ok
                    material.SetInt("_AlphaCutofffEnable", 1); // ok
                    material.SetInt("_AlphaDstBlend", 0); // ok
                    material.SetInt("_DstBlend", 0);
                    material.SetInt("_RenderQueueType", 1);
                    material.SetInt("_SurfaceType", 0);
                    material.SetInt("_ZTestDepthEqualForOpaque", 3);
                    material.SetInt("_ZWrite", 1);

                }
                #endregion

                #region Transparent
                if (_tmpBlendMode == 2.0f)
                {
                    renderTypeDebugText = "Transparent"; 
                    testInt += 1;
                    material.SetFloat("_CutOff", 0);
                    material.SetOverrideTag("RenderType", "Transparent"); // ok
                    material.SetInt("_CullMode", _tmpCullMode);
                    material.SetInt("_OpaqueCullMode", _tmpCullMode);
                    material.SetInt("_CullModeForward", _tmpCullMode);
                    // material.SetInt("_CullMode", _tmpCullMode);
                    // means alphaCutOffEnable
                    bool utilSwitch = false;
                    material.SetInt("_BlendMode", 0);
                    material.EnableKeyword("_ENABLE_FOG_ON_TRANSPARENT"); // ok
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); // ok
                    // 分岐
                    if (utilSwitch)
                    {
                        material.EnableKeyword("_ALPHATEST_ON");
                    }
                    else
                    {
                        material.DisableKeyword("_ALPHATEST_ON");
                    }

                    material.EnableKeyword("_ALPHATEST_ON");
          
                    material.renderQueue = 3000; // ok
                    
      
                    material.SetInt("_AlphaCutofffEnable", 0); // ok

                    material.SetInt("_AlphaDstBlend", 10); // ok
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_RenderQueueType", 4);
                    material.SetInt("_SurfaceType", 1);
                    material.SetInt("_ZTestDepthEqualForOpaque", 4);
                    material.SetInt("_ZWrite", 0);

                 

                    
                }
                #endregion

                if (_tmpBlendMode == 3.0f)
                {
                    renderTypeDebugText = "TransparentWithZWrite"; 
                    testInt += 1;
                    material.SetFloat("_CutOff", 0);
                    material.SetOverrideTag("RenderType", "Transparent"); // ok
                    material.SetInt("_CullMode", _tmpCullMode);
                    material.SetInt("_OpaqueCullMode", _tmpCullMode);
                    material.SetInt("_CullModeForward", _tmpCullMode);
                    // material.SetInt("_CullMode", _tmpCullMode);
                    // means alphaCutOffEnable
                    bool utilSwitch = false;
                    material.SetInt("_BlendMode", 0);
                    material.EnableKeyword("_ENABLE_FOG_ON_TRANSPARENT"); // ok
                    material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); // ok
                    // 分岐
                    if (utilSwitch)
                    {
                        material.EnableKeyword("_ALPHATEST_ON");
                    }
                    else
                    {
                        material.DisableKeyword("_ALPHATEST_ON");
                    }

                    material.EnableKeyword("_ALPHATEST_ON");
          
                    material.renderQueue = 3000; // ok

                    material.SetInt("_AlphaCutofffEnable", 0); // ok

                    material.SetInt("_AlphaDstBlend", 10); // ok
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_RenderQueueType", 4);
                    material.SetInt("_SurfaceType", 1);
                    material.SetInt("_ZTestDepthEqualForOpaque", 4);
                    material.SetInt("_ZWrite", 1);
                    material.SetInt("_DepthWrite", 1);
                    material.SetInt("_TransparentZWrite", 1);

                 

                    
                }

                #endregion // change shader

                // マテリアル名をデバック
                Debug.Log("After:" + material.name);

                // // 変更したマテリアル名をTMPに加える
                // text.text += "[" + material.name + "]" + "\n" + renderTypeDebugText + "\n";
                // if (_tmpBlendMode == 0.0f)
                // {
                //     text.text += "BeforeRenderType: " + "Opaque" + "\n";
                // }
                // if (_tmpBlendMode == 1.0f)
                // {
                //     text.text += "BeforeRenderType: " + "Cutout" + "\n";
                // }
                // if (_tmpBlendMode == 2.0f)
                // {
                //     text.text += "BeforeRenderType: " + "Transparent" + "\n";
                // }
                // if (_tmpBlendMode == 3.0f)
                // {
                //     text.text += "BeforeRenderType: " + _tmpBlendMode + "\n";
                // }
                // // 改行
                // text.text += "\n";
                // text.text += "BeforeRenderType: " + _tmpBlendMode + "\n";

                // Debug.Log("Changed Count: " + testInt);


            }

            


        }
        // マテリアルのemissionの強度をdefaultEmissionIntensityにする
        foreach (var renderer in renderers)
        {
            foreach (var material in renderer.sharedMaterials)
            {
                // material.SetFloat("_EmissiveIntensity", defaultEmissionIntensity);
                material.SetFloat("_EmissiveIntensity", loadEmissiveIntensity);
            }
        }

        Debug.Log("Last Changed Count: " + testInt);

        #endregion // change renderer
                // headの子にcameraTargetをいれる
        // if (Application.isPlaying)
        // {
        //     cameraTarget.transform.SetParent(head, false);
        //     // ローカルポジションは0
        //     cameraTarget.transform.localPosition = new Vector3(0, 0, 0);
        // }

        

    }

    void EnsureTagExists(string tag)
    {
        // タグリストを取得
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        // タグが既に存在するかチェック
                bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tag)) { found = true; break; }
        }

        // タグが存在しない場合は追加
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
            newTag.stringValue = tag;
            tagManager.ApplyModifiedProperties();
            Debug.Log($"Tag '{tag}' added.");
        }
    }
}

// SFB Sample
// using System.Collections;
// using UnityEngine;
// using SFB;

// public class SelectVRM : MonoBehaviour 
// { 
//     public void OnClick()
//     {
//         // フィルタ付きでファイルダイアログを開く
        
//         var extensions = new[]
//         {
//         new ExtensionFilter( "VRM Files", "vrm" ),
//         new ExtensionFilter( "All Files", "*" ),
//         };
//         string[] paths = StandaloneFileBrowser.OpenFilePanel("Open VRM File", "", extensions, false);

//         Debug.Log(paths[0]);
//     }
// }