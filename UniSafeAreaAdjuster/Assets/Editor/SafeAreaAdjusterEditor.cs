using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//*************************************************************************************************
/// <summary>
/// SafeAreaAdjuster Inspector拡張クラス
/// </summary>
//*************************************************************************************************
[CustomEditor(typeof(SafeAreaAdjuster))]
public class SafeAreaAdjusterEditor : Editor {
  private bool toggle;
  private SafeAreaAdjuster comp;

  //*************************************************************************************************
  /// <summary>
  /// 表示時の通知
  /// </summary>
  //*************************************************************************************************
  void OnEnable() {
    comp = target as SafeAreaAdjuster;
  }

  //*************************************************************************************************
  /// <summary>
  /// Inspector 描画
  /// </summary>
  //*************************************************************************************************
  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    EditorGUI.BeginChangeCheck();

    // toggle をマウスでクリックして値を変更する
    EditorGUILayout.Space();
    toggle = EditorGUILayout.ToggleLeft("Editor上で即シミュレート", toggle);

    // toggle の値が変更されるたびに true になる
    if (EditorGUI.EndChangeCheck()) {
      if (toggle) {
        comp.SimulateAtEditor();
      } else {
        comp.Setup();
        comp.Apply();
      }
    }
  }
}
