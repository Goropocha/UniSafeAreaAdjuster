#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SafeArea {
  //*************************************************************************************************
  /// <summary>
  /// [Safe Area] Customizable Settings
  /// </summary>
  //*************************************************************************************************
  public static class SimulateData {
    // Add new safe areas if you need
    public enum SimulateType {
      None = 0,
      iPhoneXAndXs,
      iPhoneXR,
      iPhoneXsMax,
      iPadPro11,
      iPadPro12_9_3rd
    }

    // Physical scale of Mobile (px).
    // You must match SimulateType and index of the array.
    // These values express Vector2Int(Width, Hegiht) on Portrait orientation
    public static Vector2Int[] Resolutions = new Vector2Int[] {
      Vector2Int.zero,
      new Vector2Int(1125, 2436), // iPhoneXAndXs
      new Vector2Int( 828, 1792), // iPhoneXR
      new Vector2Int(1242, 2688), // iPhoneXsMax
      new Vector2Int(1668, 2388), // iPadPro11
      new Vector2Int(2048, 2732)  // iPadPro12_9_3rd
    };

    // Physical scale of Safe Area (px). 
    // You must match SimulateType and index of the array.
    // Caliculate Method: Logical scale of Mobile (pt) * Resolution.
    // Storage order is {Portrait orientation, Landscape orientation}.
    // X and Y have lower left as origin.
    public static Rect[,] SafeAreaResolutions = new Rect[,] {
      { new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height) },
      { new Rect(0, 102, 1125, 2202), new Rect(132, 63, 2172, 1062) },  // 3x, iPhoneXAndXs
      { new Rect(0,  68,  828, 1636), new Rect( 88, 42, 1616,  786) },  // 2x, iPhoneXR
      { new Rect(0, 102, 1242, 2454), new Rect(132, 63, 2424, 1179) },  // 3x, iPhoneXsMax
      { new Rect(0,  40, 1668, 2348), new Rect(  0, 40, 2388, 1628) },  // 2x, iPadPro11
      { new Rect(0,  40, 2048, 2692), new Rect(  0, 40, 2732, 2008) }   // 2x, iPadPro12_9_3rd
    };
  }
}
#endif
