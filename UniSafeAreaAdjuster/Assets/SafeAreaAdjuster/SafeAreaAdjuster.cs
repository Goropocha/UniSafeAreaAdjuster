//#define Unity2018_3_2orOlder // Unity 2018.3.2以前の場合はTrueにしてください
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SafeArea {
  //*************************************************************************************************
  /// <summary>
  /// セーフエリア調整クラス
  /// </summary>
  //*************************************************************************************************
  public class SafeAreaAdjuster : MonoBehaviour {

    private RectTransform panelRectTrans;
    /// <summary>調整対象のRectTransfrom</summary>
    public RectTransform PanelRectTrans {
      get {
        if (panelRectTrans == null) {
          panelRectTrans = GetComponent<RectTransform>();
        }
        return panelRectTrans;
      }
    }

    [SerializeField, Header("スケールを自動調整する")]
    private bool isAutoScale;

    private Rect safeArea;
    private Vector2Int screenSize;

    //*************************************************************************************************
    /// <summary>
    /// 起動時処理
    /// </summary>
    //*************************************************************************************************
    void Awake() {
#if UNITY_EDITOR
      // 実行時のデバッグ用
      if (simulateOnPlay) {
        orientationType = OrientationType.Auto;
        simulateType = getSimulateTypeFromCurrentResolution();
        if (simulateType == SimulateData.SimulateType.None) {
          return;
        }
        safeArea = getSimulationSafeArea(simulateType);
        screenSize = getSimulationResolution(simulateType);
        Apply();
        return;
      }
#endif
      Setup();
      Apply();
    }

    //*************************************************************************************************
    /// <summary>
    /// 初期化処理
    /// </summary>
    //*************************************************************************************************
    [Conditional("UNITY_EDITOR"), Conditional("UNITY_IOS")]
    public void Setup() {
      safeArea = UnityEngine.Screen.safeArea;
#if UNITY_EDITOR || Unity2018_3_2_or_Older
      var display = Display.displays[0];
      screenSize = new Vector2Int(display.systemWidth, display.systemHeight);
#else
      screenSize = new Vector2Int(Screen.width, Screen.height);
#endif
    }

    //*************************************************************************************************
    /// <summary>
    /// 適応
    /// </summary>
    //*************************************************************************************************
    [Conditional("UNITY_EDITOR"), Conditional("UNITY_IOS")]
    public void Apply() {
      var anchorMin = safeArea.position;
      var anchorMax = safeArea.position + safeArea.size;
      anchorMin.x /= screenSize.x;
      anchorMin.y /= screenSize.y;
      anchorMax.x /= screenSize.x;
      anchorMax.y /= screenSize.y;

      PanelRectTrans.anchorMin = anchorMin;
      PanelRectTrans.anchorMax = anchorMax;

      if (isAutoScale) {
        var oldSize = PanelRectTrans.rect;
        adjustScale();
        var heightRate = getHeightRate();
        var newSize = new Rect(PanelRectTrans.rect.x * heightRate, PanelRectTrans.rect.y * heightRate, PanelRectTrans.rect.width * heightRate, PanelRectTrans.rect.height * heightRate);
        // スケールを縮小した分だけ枠のサイズを広げる
        PanelRectTrans.sizeDelta = new Vector2((oldSize.width - newSize.width), (oldSize.height - newSize.height));
      }
    }

    //*************************************************************************************************
    /// <summary>
    /// SafeAreaが高さに占める割合を返す
    /// </summary>
    /// <returns>SafeAreaが高さに占める割合</returns>
    //*************************************************************************************************
    private float getHeightRate() {
      return Mathf.Clamp01(safeArea.height / screenSize.y);
    }

    //*************************************************************************************************
    /// <summary>
    /// SafeAreaのサイズにフィットするように、指定したTransformを伸縮させる
    /// </summary>
    //*************************************************************************************************
    private void adjustScale() {
      var heightRate = getHeightRate();
      PanelRectTrans.localScale = new Vector3(heightRate, heightRate, 1.0f);
    }

#if UNITY_EDITOR
    #region デバッグ用
    
    // 端末の向き
    private enum OrientationType {
      Auto = 0,
      Portrait,
      Landscape
    }
    private OrientationType orientationType = OrientationType.Auto;

    [SerializeField, Header("Debug: PC上で実行時にも反映する")]
    private bool simulateOnPlay = true;
    [SerializeField, Header("Debug: シミュレートしたい機種"), Tooltip("実行時は無視されます")]
    private SimulateData.SimulateType simulateType;
    [SerializeField, Header("Debug: 縦持ちか"), Tooltip("実行時は無視されます")]
    private bool isPortrait;

    // 横持ちか
    private bool isLandscape {
      get {
        // Editor上での操作時は isPortrait から判定する
        if (orientationType != OrientationType.Auto) {
          return !isPortrait;
        }

        // 実行時は解像度から判定
        var width = UnityEngine.Screen.width;
        var height = UnityEngine.Screen.height;

        var resolution = SimulateData.Resolutions[(int)simulateType];
        return width == resolution.y && height == resolution.x;
      }
    }

    //*************************************************************************************************
    /// <summary>
    /// iPhoneX以降のセーフエリア シミュレーション
    /// </summary>
    //*************************************************************************************************
    public void SimulateAtEditor() {
      if (simulateType == SimulateData.SimulateType.None) {
        return;
      }
      orientationType = isPortrait ? OrientationType.Portrait : OrientationType.Landscape;
      safeArea = getSimulationSafeArea(simulateType);
      screenSize = getSimulationResolution(simulateType);
      Apply();
    }

    //*************************************************************************************************
    /// <summary>
    /// シミュレート用の解像度を取得
    /// </summary>
    /// <param name="type"シミュレート用の機種></param>
    /// <returns>シミュレート用の解像度</returns>
    //*************************************************************************************************
    private Vector2Int getSimulationResolution(SimulateData.SimulateType type) {
      var index = (int)type;
      var resolutions = SimulateData.Resolutions;
      var width = isLandscape ? resolutions[index].y : resolutions[index].x;
      var height = isLandscape ? resolutions[index].x : resolutions[index].y;
      return new Vector2Int(width, height);
    }

    //*************************************************************************************************
    /// <summary>
    /// シミュレート用のセーフエリアを取得
    /// </summary>
    /// <param name="type">シミュレート用の機種</param>
    /// <returns>シミュレート用のセーフエリア</returns>
    //*************************************************************************************************
    private Rect getSimulationSafeArea(SimulateData.SimulateType type) {
      return SimulateData.SafeAreaResolutions[(int)type, isLandscape ? 1 : 0];
    }

    //*************************************************************************************************
    /// <summary>
    /// 現在のエディタ上の解像度からシミュレートする機種を特定する
    /// </summary>
    /// <returns>シミュレートする機種</returns>
    //*************************************************************************************************
    private SimulateData.SimulateType getSimulateTypeFromCurrentResolution() {
      var width = UnityEngine.Screen.width;
      var height = UnityEngine.Screen.height;

      var resolutions = SimulateData.Resolutions;
      for (int i = 0; i < resolutions.Length; i++) {
        if (width == resolutions[i].y && height == resolutions[i].x || width == resolutions[i].x && height == resolutions[i].y) {
          return (SimulateData.SimulateType)i;
        }
      }
      return SimulateData.SimulateType.None;
    }
    #endregion
#endif
  }
}