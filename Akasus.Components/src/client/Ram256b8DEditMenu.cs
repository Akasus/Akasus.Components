// File: Ram256b8DEditMenu.cs
// Project: EcconiaCPUServerComponents
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:

using System;
using LogicWorld.UI;
using System.Collections.Generic;
using System.Linq;
using LogicLog;
using LogicUI.MenuParts;
using LogicUI.MenuTypes.Tabs;
using UnityEngine;
using UnityEngine.UI;

namespace Akasus.Components.Client
{
    public class Ram256b8DEditMenu : ModEditComponentMenu
    {
        private InputSlider slider;

        private ILogicLogger Logger = LogicLogger.For("Ram256b8D Edit Menu");

        private float val = 0;

        protected override void CreateUIComponents(Transform contentRect)
        {
            if(contentRect == null)
            {
                Logger.Error("Content Rect is NULL!");
                return;
            }

            new Material(Shader.Find(""))

            var vp = contentRect.Find("Tab View").Find("Viewport");

            var slgo = Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().First(e=> e.name.Equals("UI setting - slider float")), vp);
            slider = slgo.GetComponentInChildren<InputSlider>();
            var rt = slgo.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            slider.OnValueChanged += arg0 =>
            {
                val = arg0;
                Logger.Info("Value Changed: "+ val);
            };
            slider.Min = 0;
            slider.Max = 1;
        }

        protected override IEnumerable<string> GetTextIDsOfComponentTypesThatCanBeEdited()
        {
            yield return "Akasus.Components.Ram256b8D";
        }
    }
}