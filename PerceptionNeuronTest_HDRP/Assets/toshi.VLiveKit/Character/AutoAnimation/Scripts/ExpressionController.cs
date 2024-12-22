using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VRM; // VRMの名前空間を追加

public class ExpressionController : MonoBehaviour
{
    [Header("Skinned Mesh Renderer")]
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("VRM Blend Shape Proxy")]
    public VRMBlendShapeProxy blendShapeProxy; // VRMBlendShapeProxyを追加

    [Header("Use VRM Blend Shape Proxy (On) or Skinned Mesh Renderer (Off)")]
    public bool useBlendShapeProxy = true; // 使用するコンポーネントを切り替えるフラグ

    public List<BlendShapeInfo> blendShapeInfos = new List<BlendShapeInfo>();
    public List<BlendShapeKeyMapping> blendShapeKeyMappings = new List<BlendShapeKeyMapping>();

    private Dictionary<string, int> blendShapeNameToIndex = new Dictionary<string, int>();
    private Dictionary<KeyCode, Coroutine> activeCoroutines = new Dictionary<KeyCode, Coroutine>();
    private Dictionary<int, Coroutine> blendShapeCoroutines = new Dictionary<int, Coroutine>(); // BlendShapeごとのコルーチンを管理
    private Dictionary<int, float> blendShapeMaxWeights = new Dictionary<int, float>(); // 最大値を保持する辞書

    void Start()
    {
        UpdateBlendShapeList();
    }

    void Update()
    {
        foreach (var mapping in blendShapeKeyMappings)
        {
            if (blendShapeNameToIndex.TryGetValue(mapping.blendShapeName, out int index))
            {
                bool inputDetected = false;

                switch (mapping.inputType)
                {
                    case BlendShapeKeyMapping.InputType.Key:
                        if (Input.GetKey(mapping.triggerKey))
                        {
                            inputDetected = true;
                        }
                        break;
                    case BlendShapeKeyMapping.InputType.DPadUp:
                        if (Input.GetAxis("D_Pad_V") > 0)
                        {
                            inputDetected = true;
                        }
                        break;
                    case BlendShapeKeyMapping.InputType.DPadDown:
                        if (Input.GetAxis("D_Pad_V") < 0)
                        {
                            inputDetected = true;
                        }
                        break;
                    case BlendShapeKeyMapping.InputType.DPadLeft:
                        if (Input.GetAxis("D_Pad_H") < 0)
                        {
                            inputDetected = true;
                        }
                        break;
                    case BlendShapeKeyMapping.InputType.DPadRight:
                        if (Input.GetAxis("D_Pad_H") > 0)
                        {
                            inputDetected = true;
                        }
                        break;
                    case BlendShapeKeyMapping.InputType.LTrigger:
                        if (Input.GetAxis("L_R_Trigger") < 0)
                        {
                            inputDetected = true;
                        }
                        break;
                    case BlendShapeKeyMapping.InputType.RTrigger:
                        if (Input.GetAxis("L_R_Trigger") > 0)
                        {
                            inputDetected = true;
                        }
                        break;
                }

                if (inputDetected)
                {
                    if (!mapping.isActive)
                    {
                        mapping.isActive = true;
                        if (blendShapeCoroutines.TryGetValue(index, out Coroutine routine) && routine != null)
                        {
                            StopCoroutine(routine);
                        }
                        blendShapeCoroutines[index] = StartCoroutine(ChangeExpressionCoroutine(index, mapping.transitionDuration, mapping.targetWeightPercentage, mapping.animationCurve));
                    }
                }
                else
                {
                    if (mapping.isActive)
                    {
                        mapping.isActive = false;
                        if (blendShapeCoroutines.TryGetValue(index, out Coroutine routine) && routine != null)
                        {
                            StopCoroutine(routine);
                        }
                        blendShapeCoroutines[index] = StartCoroutine(ChangeExpressionCoroutine(index, mapping.transitionDuration, 0.0f, mapping.animationCurve));
                    }
                }
            }
            else
            {
                Debug.LogWarning($"BlendShape '{mapping.blendShapeName}' not found.");
            }
        }
    }

    private IEnumerator ChangeExpressionCoroutine(int index, float duration, float endValuePercentage, AnimationCurve curve)
    {
        float maxWeight = blendShapeMaxWeights.ContainsKey(index) ? blendShapeMaxWeights[index] : 100f;
        float endValue = (endValuePercentage / 100f) * maxWeight;

        if (duration <= 0f)
        {
            SetBlendShapeWeight(index, endValue);
            yield break;
        }

        float elapsedTime = 0f;
        float startValue = GetBlendShapeWeight(index);

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            float blendValue = Mathf.Lerp(startValue, endValue, curve.Evaluate(t));
            SetBlendShapeWeight(index, blendValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetBlendShapeWeight(index, endValue);
    }

    private void SetBlendShapeWeight(int index, float weight)
    {
        if (useBlendShapeProxy && blendShapeProxy != null)
        {
            var key = blendShapeProxy.BlendShapeAvatar.Clips[index].Key;
            blendShapeProxy.ImmediatelySetValue(key, weight / 100f);
        }
        else if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(index, weight);
        }
    }

    private float GetBlendShapeWeight(int index)
    {
        if (useBlendShapeProxy && blendShapeProxy != null)
        {
            var key = blendShapeProxy.BlendShapeAvatar.Clips[index].Key;
            return blendShapeProxy.GetValue(key) * 100f;
        }
        else if (skinnedMeshRenderer != null)
        {
            return skinnedMeshRenderer.GetBlendShapeWeight(index);
        }
        return 0f;
    }

    private void UpdateBlendShapeList()
    {
        blendShapeInfos.Clear();
        blendShapeNameToIndex.Clear();
        blendShapeMaxWeights.Clear(); // 最大値の辞書をクリア

        if (useBlendShapeProxy && blendShapeProxy != null)
        {
            var clips = blendShapeProxy.BlendShapeAvatar.Clips;
            for (int i = 0; i < clips.Count; i++)
            {
                string shapeName = clips[i].Key.ToString();
                blendShapeInfos.Add(new BlendShapeInfo(i, shapeName));
                blendShapeNameToIndex[shapeName] = i;
                blendShapeMaxWeights[i] = 100f; // VRMのBlendShapeの最大値は通常100
            }
        }
        else if (skinnedMeshRenderer != null)
        {
            Mesh mesh = skinnedMeshRenderer.sharedMesh;
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string shapeName = mesh.GetBlendShapeName(i);
                blendShapeInfos.Add(new BlendShapeInfo(i, shapeName));
                blendShapeNameToIndex[shapeName] = i;
                blendShapeMaxWeights[i] = 100f; // SkinnedMeshRendererのBlendShapeの最大値は通常100
            }
        }
    }

    [ContextMenu("Update BlendShape List")]
    private void UpdateBlendShapeListContextMenu()
    {
        UpdateBlendShapeList();
    }
}

[System.Serializable]
public class BlendShapeKeyMapping
{
    public enum InputType
    {
        Key,
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight,
        LTrigger,
        RTrigger
    }

    public string blendShapeName; // 動かしたいブレンドシェイプ
    public KeyCode triggerKey; // 対応するキーをKeyCodeに変更
    [Range(0, 100)]
    public float targetWeightPercentage = 100.0f; // 目標のウェイト（パーセンテージ）
    [Range(0, 1)]
    public float transitionDuration = 0.1f; // 移り変わるまでの時間
    public InputType inputType = InputType.Key; // 入力タイプを選択
    public bool isActive = false; // 入力されている状態を示す
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // アニメーションカーブを追加
}