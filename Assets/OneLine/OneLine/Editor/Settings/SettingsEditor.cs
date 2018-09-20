﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

using RectEx;
using System;

namespace OneLine.Settings {
    [CustomEditor(typeof(Settings))]
    public class SettingsEditor : Editor {

        private new Settings target { get { return (Settings) base.target;} }
        
        private ReorderableList list;

        private void OnEnable() {
            if (target.Layers == null) {
                target.Layers = new List<SettingsLayer>();
            }
            list = new ReorderableList(target.Layers, typeof(SettingsLayer), true, true, true, true);
            list.drawElementCallback += DrawListElement;
            list.onAddCallback += x => target.Layers.Add(SettingsMenu.CreateSettingsLayer());
            list.drawHeaderCallback += DrawDefaults;
        }

        private void DrawDefaults(Rect rect) {
            var rects = rect.Row(new float[]{0, 1}, new float[]{18, 0})[1]
                            .Row(2);
            EditorGUI.LabelField(rects[0], "Defaults:");
            EditorGUI.LabelField(rects[1], "" + target.Defaults.Enabled);
        }

        private void DrawListElement(Rect rect, int index, bool active, bool focused) {
            if (index < 0 || index >= target.Layers.Count) {
                return;
            }

            var layer = target.Layers[index];
            if (layer == null) {
                target.Layers.RemoveAt(index);
                return;
            }
            
            var rects = rect.Row(2);
            EditorGUI.LabelField(rects[0], layer.name);
            layer.Enabled = (Boolean) EditorGUI.EnumPopup(rects[1].Intend(1), layer.Enabled);

        }

        private void DrawFooter() {
            var height = 4 * (EditorGUIUtility.singleLineHeight + 2) - 2;
            var rect = EditorGUILayout.GetControlRect(false, height);

            var lines = rect.Column(4);
            var rects = lines[0].Row(new float[]{0, 1}, new float[]{18, 0})[1]
                                .Row(2);
            EditorGUI.LabelField(rects[0], "Result:");
            EditorGUI.LabelField(rects[1], "" + target.Enabled);

            rects = lines[2].Row(new float[]{1,1,2});
            if (GUI.Button(rects[0], "New")) {
                target.Layers.Add(SettingsMenu.CreateSettingsLayer());
            }

            EditorGUI.LabelField(rects[1], " or open: ");
            SettingsLayer openLayer = null;
            openLayer = (SettingsLayer) EditorGUI.ObjectField(rects[2], openLayer, typeof(SettingsLayer), false);
            if (openLayer != null && !target.Layers.Contains(openLayer)) {
                target.Layers.Add(openLayer);
            }

            if (GUI.Button(lines[3].CutFromRight(50)[1], "Save")){
                foreach (var layer in target.Layers) {
                    EditorUtility.SetDirty(layer);
                }
                EditorUtility.SetDirty(target);
                target.ApplyDirectivesInOrderToCurrentSettings();
            }
        }

        public override void OnInspectorGUI() {
            list.DoLayoutList();
            DrawFooter();
        }

    }
}