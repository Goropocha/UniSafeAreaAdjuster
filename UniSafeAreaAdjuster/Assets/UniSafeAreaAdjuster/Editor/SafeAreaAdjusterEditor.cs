using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SafeArea {
  //*************************************************************************************************
  /// <summary>
  /// SafeAreaAdjuster Inspector Editor
  /// </summary>
  //*************************************************************************************************
  [CustomEditor(typeof(SafeAreaAdjuster))]
  public class SafeAreaAdjusterEditor : Editor {
    private bool toggle;
    private SafeAreaAdjuster comp;

    //*************************************************************************************************
    /// <summary>
    /// Callback When this game object is enabled
    /// </summary>
    //*************************************************************************************************
    void OnEnable() {
      comp = target as SafeAreaAdjuster;
    }

    //*************************************************************************************************
    /// <summary>
    /// Inspector drawing
    /// </summary>
    //*************************************************************************************************
    public override void OnInspectorGUI() {
      base.OnInspectorGUI();

      EditorGUI.BeginChangeCheck();

      EditorGUILayout.Space();
      toggle = EditorGUILayout.ToggleLeft("Simulate Now at Editor", toggle);

      // True if GUI.changed was set to true, otherwise false.
      if (EditorGUI.EndChangeCheck()) {
        if (toggle) {
          comp.SimulateAtEditor();
        } else {
          comp.Apply();
        }
      }
    }
  }
}