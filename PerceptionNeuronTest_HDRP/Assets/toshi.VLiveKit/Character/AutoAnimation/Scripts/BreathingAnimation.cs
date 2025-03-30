using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BoneSettings {
    public Transform bone; // ボーン
    [Range(0f, 10f)]
    public float angleAmplitudeX = 5f; // X軸回転の振幅
    [Range(0f, 10f)]
    public float angleAmplitudeY = 0f; // Y軸回転の振幅
    [Range(0f, 10f)]
    public float angleAmplitudeZ = 0f; // Z軸回転の振幅
    [Range(0f, 1f)]
    public float phaseOffset = 0f; // 位相オフセット

    [HideInInspector]
    public Quaternion defaultRotation; // デフォルトの回転
}

[System.Serializable]
public class BlendShapeSettings {
    public string shapeName; // シェイプ名
    [Range(0f, 100f)]
    public float amplitude = 50f; // シェイプの振幅
    [Range(0f, 1f)]
    public float phaseOffset = 0.5f; // シェイプの位相オフセット

    [HideInInspector]
    public int shapeIndex; // シェイプインデックス
}

public class BreathingAnimation : MonoBehaviour {
    public List<BoneSettings> bones = new List<BoneSettings>(); // ボーンのリスト
    public SkinnedMeshRenderer faceMesh; // 顔のメッシュ
    public List<BlendShapeSettings> blendShapes = new List<BlendShapeSettings>(); // ブレンドシェイプのリスト

    [Header("General Settings")]
    [Range(0.1f, 5f)]
    public float frequency = 1f; // サイン波の周波数

    void Start() {
        foreach (var boneSetting in bones) {
            if (boneSetting.bone != null) {
                boneSetting.defaultRotation = boneSetting.bone.localRotation; // デフォルトの回転を保存
            }
        }

        if (faceMesh != null) {
            foreach (var blendShape in blendShapes) {
                blendShape.shapeIndex = faceMesh.sharedMesh.GetBlendShapeIndex(blendShape.shapeName);
                if (blendShape.shapeIndex == -1) {
                    Debug.LogWarning($"BlendShape '{blendShape.shapeName}' not found.");
                }
            }
        }
    }

    void Update() {
        foreach (var boneSetting in bones) {
            if (boneSetting.bone != null) {
                // サイン波に基づく回転を計算（位相オフセットを追加）
                float angleX = boneSetting.angleAmplitudeX * Mathf.Sin(Time.time * frequency + boneSetting.phaseOffset * Mathf.PI * 2);
                float angleY = boneSetting.angleAmplitudeY * Mathf.Sin(Time.time * frequency + boneSetting.phaseOffset * Mathf.PI * 2);
                float angleZ = boneSetting.angleAmplitudeZ * Mathf.Sin(Time.time * frequency + boneSetting.phaseOffset * Mathf.PI * 2);

                Quaternion offsetRotation = Quaternion.Euler(angleX, angleY, angleZ);
                boneSetting.bone.localRotation = boneSetting.defaultRotation * offsetRotation; // デフォルトの回転にオフセットを適用
            }
        }

        if (faceMesh != null) {
            foreach (var blendShape in blendShapes) {
                if (blendShape.shapeIndex != -1) {
                    // シェイプのウェイトをサイン波で調整
                    float weight = blendShape.amplitude * Mathf.Sin(Time.time * frequency + blendShape.phaseOffset * Mathf.PI * 2);
                    faceMesh.SetBlendShapeWeight(blendShape.shapeIndex, weight);
                }
            }
        }
    }
}