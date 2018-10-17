using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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
      if (simulateType == SimulateType.None) {
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
  public void Setup() {
    safeArea = UnityEngine.Screen.safeArea;
    // NOTE: CanvasのAnchorがずれる問題の解決策（http://appleorbit.hatenablog.com/entry/2018/05/29/235021）
    var display = Display.displays[0];
    screenSize = new Vector2Int(display.systemWidth, display.systemHeight);
  }

  //*************************************************************************************************
  /// <summary>
  /// 適応
  /// </summary>
  //*************************************************************************************************
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

  #region デバッグ用
#if UNITY_EDITOR
  // シミュレート用の機種
  private enum SimulateType {
    None = 0,
    iPhoneXAndXs,
    iPhoneXR,
    iPhoneXsMax
  }
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
  private SimulateType simulateType;
  [SerializeField, Header("Debug: 縦持ちか"), Tooltip("実行時は無視されます")]
  private bool isPortrait;

  // 端末の解像度. インデックスは SimulateType と一致させる
  private Vector2Int[] resolutions = new Vector2Int[] {
    Vector2Int.zero,
    new Vector2Int(1125, 2436),
    new Vector2Int(828, 1792),
    new Vector2Int(1242, 2688)
  };

  // 横持ちか
  private bool isLandscape {
    get {
      // Editor上での操作時は isPortrait から判定する
      if(orientationType != OrientationType.Auto) {
        return !isPortrait;
      }

      // 実行時は解像度から判定
      var width = UnityEngine.Screen.width;
      var height = UnityEngine.Screen.height;
      
      var iPhoneX = resolutions[(int)SimulateType.iPhoneXAndXs];
      var iPhoneXR = resolutions[(int)SimulateType.iPhoneXR];
      var iPhoneXsMax = resolutions[(int)SimulateType.iPhoneXsMax];
      return width == iPhoneX.y && height == iPhoneX.x || width == iPhoneXR.y && height == iPhoneXR.x || width == iPhoneXsMax.y && height == iPhoneXsMax.x;
    }
  }

  //*************************************************************************************************
  /// <summary>
  /// iPhoneX以降のセーフエリア シミュレーション
  /// </summary>
  //*************************************************************************************************
  public void SimulateAtEditor() {
    if (simulateType == SimulateType.None) {
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
  private Vector2Int getSimulationResolution(SimulateType type) {
    var index = (int)type;
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
  private Rect getSimulationSafeArea(SimulateType type) {
    // NOTE: 解像度は物理解像度から3倍する
    switch (type) {
      case SimulateType.iPhoneXAndXs:
        return isLandscape ? new Rect(132, 63, 2172, 1062) : new Rect(0, 102, 1125, 2202);
      case SimulateType.iPhoneXR:
        return isLandscape ? new Rect(132, 63, 1528, 765) : new Rect(0, 102, 1242, 2454);
      case SimulateType.iPhoneXsMax:
        return isLandscape ? new Rect(132, 63, 2424, 1179) : new Rect(0, 102, 1242, 2454);
      case SimulateType.None:
      default:
        return new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height);
    }
  }

  //*************************************************************************************************
  /// <summary>
  /// 現在のエディタ上の解像度からシミュレートする機種を特定する
  /// </summary>
  /// <returns>シミュレートする機種</returns>
  //*************************************************************************************************
  private SimulateType getSimulateTypeFromCurrentResolution() {
    var width = UnityEngine.Screen.width;
    var height = UnityEngine.Screen.height;
    if (width == 2436 && height == 1125 || width == 1125 && height == 2436) {
      return SimulateType.iPhoneXAndXs;
    } else if (width == 1792 && height == 828 || width == 828 && height == 1792) {
      return SimulateType.iPhoneXR;
    } else if (width == 2688 && height == 1242 || width == 1242 && height == 2688) {
      return SimulateType.iPhoneXsMax;
    } else {
      return SimulateType.None;
    }
  }
#endif
  #endregion
}
