// Copyright 2017 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using System.Collections.Generic;

namespace Hamster.Menus {

  // Interface class for providing code access to the GUI
  // elements in the main menu prefab.
  public class MainMenuGUI : BaseMenu {

    // These fields are set in the inspector.
    public GUIButton PlayButton;
    public GUIButton SettingsButton;
    public GUIButton LicenseButton;
    public GUIButton DebugMenuButton;
    public GameObject DebugMenuPrefab;
    [SerializeField] private GameObject MenuSubtitle;
    public UnityEngine.UI.Text MenuSubtitleText=> MenuSubtitle?.GetComponent<UnityEngine.UI.Text>();
    [System.Serializable]
    public class SubtitleOverride {
      public SubtitleOverride(string text,int fontSize,Color textColor) {
        this.text = text;
        this.fontSize=fontSize;
        this.textColor = textColor;
      }
      [SerializeField]
      public string text;
      [SerializeField]
      public int fontSize;
      [SerializeField]
      public Color textColor;
      public bool IsValidOverride() {
        return textColor != default(Color) && fontSize != default(int) && !string.IsNullOrEmpty(text);
      }
    }
  }
}