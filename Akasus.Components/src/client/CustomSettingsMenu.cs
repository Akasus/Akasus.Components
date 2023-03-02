// File: CustomSettingsMenu.cs
// Project: Akasus.Components
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:

using JimmysUnityUtilities;
using LogicSettings;
using LogicUI.MenuTypes.Tabs;
using LogicWorld.UnityHacksAndExtensions;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Reflection;
using UnityEngine;

namespace Akasus.Components.Client
{
    public class CustomSettingsMenu : MonoBehaviour, IInitializable
    {
        [SerializeField]
        private GameObject SettingsPagePrefab;

        [SerializeField]
        private GameObject SettingPrefab_SliderFloat;

        [SerializeField]
        private GameObject SettingPrefab_SliderInt;

        [SerializeField]
        private GameObject SettingPrefab_Toggle;

        [SerializeField]
        private GameObject SettingPrefab_DropdownEnum;

        [SerializeField]
        private GameObject SettingPrefab_DropdownDynamic;

        [SerializeField]
        private GameObject SettingPrefab_TexturePicker;

        [SerializeField]
        private GameObject SettingPrefab_ColorPicker;

        [SerializeField]
        private GameObject SettingPrefab_TextBox;

        [Space]
        [SerializeField]
        private TabView TabView;

        private Dictionary<string, SettingUI> AllSettingUIs = new Dictionary<string, SettingUI>();

        private Dictionary<string, SettingsPage> AllPages = new Dictionary<string, SettingsPage>();

        private int NewlyAddedPageCounter;

        private SettingsManager Settings => SettingsManager.Instance;

        public event Action OnMenuGenerated;

        void ResolvePrefabs()
        {

        }

        void IInitializable.Initialize()
        {
            CoroutineUtility.RunAfterOneFrame(delegate
            {
                if (Settings.RegisteredSettings)
                {
                    GenerateMenu();
                }
                else
                {
                    Settings.SettingsRegistered += GenerateMenu;
                }
            });
        }

        private void GenerateMenu()
        {
            var edcm = GetComponent<ModEditComponentMenu>();

            foreach (var pInfo in edcm.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                pInfo.GetCustomAttributes()
            }



            foreach (string textID in Settings.AllSettingTextIDsWithMetadataInOrder)
            {
                SettingInfo settingInfo = Settings.GetSettingInfo(textID);
                SettingMetadata settingMetadata = Settings.GetSettingMetadata(textID);
                if (settingMetadata != null && !settingMetadata.Page.IsNullOrEmpty() && !settingMetadata.Heading.IsNullOrEmpty())
                {
                    GameObject original = GetSettingPrefab(settingMetadata);
                    RectTransform parent = GetSettingParent(settingMetadata);
                    SettingUI component = UnityEngine.Object.Instantiate(original, parent).GetComponent<SettingUI>();

                    var methInfo = typeof(SettingUI).GetMethod("LoadMetadata", BindingFlags.Instance | BindingFlags.NonPublic);
                    methInfo.Invoke(component, new[] { settingMetadata });

                    //component.LoadMetadata(settingMetadata);
                    component.SetLocalizationKey(textID);
                    component.OnSettingChanged += delegate (object value)
                    {
                        Settings.SetSetting(textID, value, updateUI: false, updateDependents: true);
                    };
                    SettingHighlight component2 = component.GetComponent<SettingHighlight>();
                    component2.OnHover += delegate
                    {
                        Sidebar.ShowSidebar(settingInfo);
                    };
                    component2.ResetButton.OnClickEnd += delegate
                    {
                        Settings.ResetSettingToDefaultValue(textID);
                    };
                    AllSettingUIs.Add(textID, component);
                }
            }
            TabView.SelectFirstTab();
            Settings.UpdateValue += UpdateSettingValue;
            Settings.UpdateVisibility += UpdateSettingVisibility;
            Settings.ReloadAllSettings();
            this.OnMenuGenerated?.Invoke();
            RectTransform GetSettingParent(SettingMetadata metadata)
            {
                string text = metadata.Page;
                if (!text.Contains("."))
                {
                    text = "MHG.SettingsMenu.Pages." + metadata.Page;
                }
                string identifier = text + ".Headings." + metadata.Heading;
                return GetPage(text).GetHeading(identifier).SettingItemsParent;
            }
            GameObject GetSettingPrefab(SettingMetadata metadata)
            {
                if (metadata != null)
                {
                    if (metadata is SettingMetadata_SliderFloat)
                    {
                        return SettingPrefab_SliderFloat;
                    }
                    if (metadata is SettingMetadata_SliderInt)
                    {
                        return SettingPrefab_SliderInt;
                    }
                    if (metadata is SettingMetadata_Toggle)
                    {
                        return SettingPrefab_Toggle;
                    }
                    if (metadata is SettingMetadata_DropdownEnum)
                    {
                        return SettingPrefab_DropdownEnum;
                    }
                    if (metadata is SettingMetadata_DropdownDynamic)
                    {
                        return SettingPrefab_DropdownDynamic;
                    }
                    if (metadata is SettingMetadata_TexturePicker)
                    {
                        return SettingPrefab_TexturePicker;
                    }
                    if (metadata is SettingMetadata_ColorPicker)
                    {
                        return SettingPrefab_ColorPicker;
                    }
                    if (metadata is SettingMetadata_TextBox)
                    {
                        return SettingPrefab_TextBox;
                    }
                }
                throw new Exception($"couldn't find setting prefab for metadata type {metadata.GetType()} :(");
            }
        }

        public SettingsPage GetPage(string identifier)
        {
            if (AllPages.TryGetValue(identifier, out var value))
            {
                return value;
            }
            if (TabView.TabPagesByIdentifier.TryGetValue(identifier, out var value2))
            {
                value = value2.GetComponent<SettingsPage>();
                if (value != null)
                {
                    AllPages.Add(identifier, value);
                    return value;
                }
            }
            value = TabView.AddTabPageWithPrefab(SettingsPagePrefab, identifier, NewlyAddedPageCounter).GetComponent<SettingsPage>();
            NewlyAddedPageCounter++;
            AllPages.Add(identifier, value);
            return value;
        }

        private void UpdateSettingValue(string textID, object value)
        {
            if (AllSettingUIs.TryGetValue(textID, out var value2))
            {
                value2.SetValue(value);
            }
        }

        private void UpdateSettingVisibility(string textID)
        {
            if (AllSettingUIs.TryGetValue(textID, out var value))
            {
                SettingControls controls = Settings.GetSettingMetadata(textID).Controls;
                bool visible = Settings.ShouldBeVisible(controls);
                value.SetVisible(visible);
            }
        }
    }
}
}