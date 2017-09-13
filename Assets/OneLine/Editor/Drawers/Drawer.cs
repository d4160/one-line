﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OneLine {
    internal abstract class Drawer {

        public virtual float GetWeight(SerializedProperty property) {
            var weights = property.GetCustomAttributes<WeightAttribute>()
                                  .Select(x => x.Weight)
                                  .ToArray();
            return weights.Length > 0 ? weights.Sum() : 1;
        }

        public virtual float GetFixedWidth(SerializedProperty property) {
            return property.GetCustomAttributes<WidthAttribute>()
                           .Select(x => x.Width)
                           .Sum();
        }

        public abstract void Draw(Rect rect, SerializedProperty property);

        protected void DrawHighlight(Rect rect, SerializedProperty property) {
            property.GetCustomAttribute<HighlightAttribute>()
                    .IfPresent(x => GuiUtil.DrawRect(rect.WithBounds(1), x.Color));
        }
    }
}
