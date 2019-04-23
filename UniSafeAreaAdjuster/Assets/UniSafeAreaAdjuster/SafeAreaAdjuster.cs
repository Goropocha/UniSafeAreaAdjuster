//#define Unity2018_3_2orOlder // Uncomment if you use Unity 2018.3.2 or older
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace SafeArea {
  //*************************************************************************************************
  /// <summary>
  /// Safe Area Adjuster
  /// </summary>
  //*************************************************************************************************
  public class SafeAreaAdjuster : MonoBehaviour {

    private RectTransform panelRectTrans;
    /// <summary>Target RectTransfrom</summary>
    public RectTransform PanelRectTrans {
      get {
        if (panelRectTrans == null) {
          panelRectTrans = GetComponent<RectTransform>();
        }
        return panelRectTrans;
      }
    }

    [SerializeField]
    private bool isAutoScale;

    private Rect safeArea;
    private Vector2Int screenSize;

    //*************************************************************************************************
    /// <summary>
    /// Initialize
    /// </summary>
    //*************************************************************************************************
    void Awake() {
#if UNITY_EDITOR
      // For Debug at Play Mode
      if (simulateOnPlay) {
        orientationType = OrientationType.Auto;
        simulateType = getSimulateTypeFromCurrentResolution();
        if (simulateType == SimulateData.SimulateType.None) {
          return;
        }
        safeArea = getSimulationSafeArea(simulateType);
        screenSize = getSimulationResolution(simulateType);
        Apply(isInitScreenSize: false);
        return;
      }
#endif
      Apply();
    }

    //*************************************************************************************************
    /// <summary>
    /// Apply Safe Area to Screen
    /// </summary>
    /// <param name="isInitScreenSize">True: Initialize ScreenSize</param>
    //*************************************************************************************************
    [Conditional("UNITY_EDITOR"), Conditional("UNITY_IOS")]
    public void Apply(bool isInitScreenSize = true) {
      if (isInitScreenSize) {
        safeArea = UnityEngine.Screen.safeArea;
#if UNITY_EDITOR || Unity2018_3_2_or_Older
        var display = Display.displays[0];
        screenSize = new Vector2Int(display.systemWidth, display.systemHeight);
#else
        screenSize = new Vector2Int(Screen.width, Screen.height);
#endif
      }

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
        // Expand the size of RectTransform by the shrink scale
        PanelRectTrans.sizeDelta = new Vector2((oldSize.width - newSize.width), (oldSize.height - newSize.height));
      }
    }

    //*************************************************************************************************
    /// <summary>
    /// Returns the ratio of SafeArea to height
    /// </summary>
    /// <returns>The ratio of SafeArea to height</returns>
    //*************************************************************************************************
    private float getHeightRate() {
      return Mathf.Clamp01(safeArea.height / screenSize.y);
    }

    //*************************************************************************************************
    /// <summary>
    /// Stretch Local Scale to fit SafeArea size
    /// </summary>
    //*************************************************************************************************
    private void adjustScale() {
      var heightRate = getHeightRate();
      PanelRectTrans.localScale = new Vector3(heightRate, heightRate, 1.0f);
    }

#if UNITY_EDITOR
    #region ForDebug
    
    private enum OrientationType {
      Auto = 0,
      Portrait,
      Landscape
    }
    private OrientationType orientationType = OrientationType.Auto;

    [SerializeField, Header("Debug")]
    private bool simulateOnPlay = true;
    [SerializeField, Header("Debug"), Tooltip("Ignores when Play")]
    private SimulateData.SimulateType simulateType;
    [SerializeField, Header("Debug"), Tooltip("Ignores when Play")]
    private bool isPortrait;

    private bool isLandscape {
      get {
        if (orientationType != OrientationType.Auto) {
          return !isPortrait;
        }

        // Judge by Resolution at Play Mode
        var width = UnityEngine.Screen.width;
        var height = UnityEngine.Screen.height;

        var resolution = SimulateData.Resolutions[(int)simulateType];
        return width == resolution.y && height == resolution.x;
      }
    }

    //*************************************************************************************************
    /// <summary>
    /// Simulaties at Editor without Play
    /// </summary>
    //*************************************************************************************************
    public void SimulateAtEditor() {
      if (simulateType == SimulateData.SimulateType.None) {
        return;
      }
      orientationType = isPortrait ? OrientationType.Portrait : OrientationType.Landscape;
      safeArea = getSimulationSafeArea(simulateType);
      screenSize = getSimulationResolution(simulateType);
      Apply(isInitScreenSize: false);
    }

    //*************************************************************************************************
    /// <summary>
    /// Returns Resolution for Simulate
    /// </summary>
    /// <param name="type">Simulate Type</param>
    /// <returns>Resolution for Simulate</returns>
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
    /// Returns Safe Area for Simulate
    /// </summary>
    /// <param name="type">Simulate Type</param>
    /// <returns>Safe Area for Simulate</returns>
    //*************************************************************************************************
    private Rect getSimulationSafeArea(SimulateData.SimulateType type) {
      return SimulateData.SafeAreaResolutions[(int)type, isLandscape ? 1 : 0];
    }

    //*************************************************************************************************
    /// <summary>
    /// Identify the Simulate Type from the resolution on the current editor
    /// </summary>
    /// <returns>Target Simulate Type</returns>
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