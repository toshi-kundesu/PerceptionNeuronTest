using UnityEngine;

public class EyeDirt : MonoBehaviour {
    public Transform rightEye; // 右目のトランスフォーム
    public Transform leftEye; // 左目のトランスフォーム
    public Transform cameraTransform; // カメラのトランスフォーム

    [Header("Angle Settings")]
    public Vector3 minAngle = new Vector3(0f, 0f, 0f); // 最小角度
    public Vector3 maxAngle = new Vector3(10f, 10f, 10f); // 最大角度
    [SerializeField] [Range(0.0f, 5.0f)] public float IntensityMultiplier = 1.0f; // 強度の倍率

    [Header("Time Settings")]
    public float minTime = 0.5f; // 最小時間間隔
    public float maxTime = 2.0f; // 最大時間間隔

    [Header("Camera Settings")]
    [SerializeField] private float cameraLookProbability = 0.5f; // カメラ目線の確率
    [SerializeField] private bool autoSwitchState = true; // 状態を自動で切り替えるかどうか
    [SerializeField] private bool lookAtCamera = false; // カメラ目線のオンオフ
    [SerializeField] private bool invertLookAt = false; // カメラ目線のインバート
    [SerializeField] private Vector3 lookAtOffset = Vector3.zero; // カメラ目線のオフセット

    [Header("Eye Offset Settings")]
    [SerializeField] private Vector3 rightEyeOffset = Vector3.zero; // 右目のオフセット
    [SerializeField] private Vector3 leftEyeOffset = Vector3.zero; // 左目のオフセット
    [SerializeField] private Vector3 defaultViewOffset = Vector3.zero; // デフォルト視点のオフセット

    private Quaternion defaultRightEyeRotation; // 右目のデフォルトの回転
    private Quaternion defaultLeftEyeRotation; // 左目のデフォルトの回転

    [SerializeField] private float timer; // タイマー
    [SerializeField] private float elapsedTime; // 経過時間

    [Header("Camera Look Intensity")]
    [SerializeField] [Range(0.0f, 1.0f)] private float cameraLookIntensity = 0.7f; // カメラ目線の強度

    void Start() {
        defaultRightEyeRotation = rightEye.localRotation;
        defaultLeftEyeRotation = leftEye.localRotation;
        timer = Random.Range(minTime, maxTime); // 初期タイマー設定
        elapsedTime = 0f; // 経過時間の初期化
    }

    void Update() {
        elapsedTime += Time.deltaTime;
        timer -= Time.deltaTime;

        if (timer <= 0.0f) {
            // 次のタイマー設定
            timer = Random.Range(minTime, maxTime);

            if (autoSwitchState) {
                // 状態をランダムに切り替え
                lookAtCamera = Random.value < cameraLookProbability;
            }

            // ランダムな角度を生成
            Vector3 randomAngleLeft = new Vector3(
                Random.Range(minAngle.x * IntensityMultiplier, maxAngle.x * IntensityMultiplier),
                Random.Range(minAngle.y * IntensityMultiplier, maxAngle.y * IntensityMultiplier),
                Random.Range(minAngle.z * IntensityMultiplier, maxAngle.z * IntensityMultiplier)
            );

            Vector3 randomAngleRight = randomAngleLeft; // 右目も同じ角度を使用

            Quaternion targetRotationLeft = Quaternion.Euler(randomAngleLeft);
            Quaternion targetRotationRight = Quaternion.Euler(randomAngleRight);

            if (!lookAtCamera) {
                // 目のローカル回転を設定し、デフォルト視点のオフセットを適用
                leftEye.localRotation = defaultLeftEyeRotation * targetRotationLeft * Quaternion.Euler(leftEyeOffset + defaultViewOffset);
                rightEye.localRotation = defaultRightEyeRotation * targetRotationRight * Quaternion.Euler(rightEyeOffset + defaultViewOffset);
            } else {
                // カメラ目線の回転を計算し、ランダムな角度を適用
                if (cameraTransform != null) {
                    Vector3 direction = cameraTransform.position - leftEye.position;
                    if (invertLookAt) {
                        direction = -direction; // インバート
                    }
                    Quaternion lookAtRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(leftEyeOffset);
                    leftEye.localRotation = Quaternion.Slerp(defaultLeftEyeRotation, lookAtRotation * targetRotationLeft, cameraLookIntensity);

                    direction = cameraTransform.position - rightEye.position;
                    if (invertLookAt) {
                        direction = -direction; // インバート
                    }
                    lookAtRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rightEyeOffset);
                    rightEye.localRotation = Quaternion.Slerp(defaultRightEyeRotation, lookAtRotation * targetRotationRight, cameraLookIntensity);
                }
            }
        }
    }
}
