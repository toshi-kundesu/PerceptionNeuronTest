using UnityEngine;

public class AutoBlink : MonoBehaviour
{
    [SerializeField] private
    SkinnedMeshRenderer ref_SMR_EYE_DEF;
    public string blendShapeName = "EyeBlink";
    [Range(0.0f, 1.0f)]
    public float minBlinkDuration = 0.1f; // 点滅の最小継続時間
    [Range(0.0f, 1.0f)]
    public float maxBlinkDuration = 1.0f; // 点滅の最大継続時間
    [Range(0.0f, 5.0f)]
    public float minBlinkPause = 1.0f; // 最小休止時間
    [Range(0.0f, 5.0f)]
    public float maxBlinkPause = 3.0f; // 最大休止時間

    [Header("Animation Curve")]
    public AnimationCurve blinkCurve; // アニメーションカーブ

    private float blinkDuration; // 現在の点滅継続時間
    private float blinkPause; // 現在の休止時間
    private float blinkTimer = 0.0f; // 点滅のタイマー
    private float pauseTimer = 0.0f; // 休止のタイマー
    private bool isBlinking = false; // 点滅中かどうかのフラグ

    [Header("Initial Blink Weight")]
    [Range(0.0f, 100.0f)]
    public float initialBlinkWeight = 37.77778f; // 初期の点滅ウェイト

    void Start()
    {
        // 初期値を最大値に設定
        blinkDuration = maxBlinkDuration;
        blinkPause = maxBlinkPause;

        // 初期の点滅ウェイトを設定
        ref_SMR_EYE_DEF.SetBlendShapeWeight(ref_SMR_EYE_DEF.sharedMesh.GetBlendShapeIndex(blendShapeName), initialBlinkWeight);
    }

    void Update()
    {
        if (!isBlinking)
        {
            pauseTimer += Time.deltaTime;

            if (pauseTimer >= blinkPause)
            {
                pauseTimer = 0.0f;
                isBlinking = true;
                blinkDuration = Random.Range(minBlinkDuration, maxBlinkDuration); // 新しい点滅時間をランダムに設定
            }
        }
        else
        {
            blinkTimer += Time.deltaTime;

            // アニメーションカーブを使用してウェイトを計算
            float weight = initialBlinkWeight + (100.0f - initialBlinkWeight) * blinkCurve.Evaluate(blinkTimer / blinkDuration);
            ref_SMR_EYE_DEF.SetBlendShapeWeight(ref_SMR_EYE_DEF.sharedMesh.GetBlendShapeIndex(blendShapeName), weight);

            if (blinkTimer >= blinkDuration)
            {
                blinkTimer = 0.0f;
                isBlinking = false;
                blinkPause = Random.Range(minBlinkPause, maxBlinkPause); // 新しい休止時間をランダムに設定
            }
        }
    }
}