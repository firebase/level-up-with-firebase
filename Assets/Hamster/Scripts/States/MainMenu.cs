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
using Firebase.RemoteConfig;

namespace Hamster.States {
  class MainMenu : BaseState {
    // Width/Height of the menu, expressed as a portion of the screen width:
    const float MenuWidth = 0.40f;
    const float MenuHeight = 0.75f;

    private GUIStyle titleStyle;
    private GUIStyle subTitleStyle;

    public const string SubtitleOverrideKey = "subtitle_override";
    public static readonly string SubtitleOverrideDefault = JsonUtility.ToJson(new Menus.MainMenuGUI.SubtitleOverride("",8,new Color(255,0,0,255)));
    private Stack<BaseState> statesToShow = new Stack<BaseState>();
    private Object stateStackLock = new Object();

    private Menus.MainMenuGUI menuComponent;

    public MainMenu() {
      // Initialize some styles that we'll for the title.
      titleStyle = new GUIStyle();
      titleStyle.alignment = TextAnchor.UpperCenter;
      titleStyle.fontSize = 50;
      titleStyle.normal.textColor = Color.white;

      subTitleStyle = new GUIStyle();
      subTitleStyle.alignment = TextAnchor.UpperCenter;
      subTitleStyle.fontSize = 20;
      subTitleStyle.normal.textColor = Color.white;
    }

    // Initialization method.  Called after the state
    // is added to the stack.
    public override void Initialize() {
      CommonData.mainGame.SelectAndPlayMusic(CommonData.prefabs.menuMusic, true);
      InitializeUI();
    }

    public override void Resume(StateExitValue results) {
      CommonData.mainGame.SelectAndPlayMusic(CommonData.prefabs.menuMusic, true);
      CommonData.mainGame.FetchRemoteConfig(InitializeUI);
    }

    private void InitializeUI() {
      if (menuComponent == null) {
        menuComponent = SpawnUI<Menus.MainMenuGUI>(StringConstants.PrefabMainMenu);
      }

      var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
      var subtitleOverride = JsonUtility.FromJson<Menus.MainMenuGUI.SubtitleOverride>(
        remoteConfig.GetValue(SubtitleOverrideKey).StringValue);
      // Only sets values if all fields of the override are non-default.
      if(subtitleOverride != null &&subtitleOverride.IsValidOverride())
      {
        menuComponent.MenuSubtitleText.text = subtitleOverride.text;
        menuComponent.MenuSubtitleText.fontSize = subtitleOverride.fontSize;
        menuComponent.MenuSubtitleText.color = subtitleOverride.textColor;
      }
      ShowUI();
    }

    public override void Suspend() {
      HideUI();
    }

    public override StateExitValue Cleanup() {
      DestroyUI();
      return null;
    }

    public override void HandleUIEvent(GameObject source, object eventData) {
      if (source == menuComponent.PlayButton.gameObject) {
        manager.SwapState(new LevelSelect());
      } else if (source == menuComponent.SettingsButton.gameObject) {
        manager.PushState(new SettingsMenu());
      } else if (source == menuComponent.LicenseButton.gameObject) {
        manager.PushState(new LicenseDialog());
      } else if (source == menuComponent.DebugMenuButton.gameObject) {
        manager.PushState(new DebugMenu(menuComponent.DebugMenuPrefab));
      }
    }

    // Update function.  If any states are waiting to be shown, swap to them.
    public override void Update() {
      if (statesToShow.Count != 0) {
        lock (stateStackLock) {
          manager.PushState(statesToShow.Pop());
        }
      }
    }

    // Helper function for adding states that need to be shown.
    // Made a helper function, because it needs a lock, in case
    // randomly firing listeners cause race conditions.
    private void QueueState(BaseState newState) {
      lock (stateStackLock) {
        statesToShow.Push(newState);
      }
    }

    void HandleConversionResult(System.Threading.Tasks.Task convertTask) {
      if (convertTask.IsCanceled) {
        Debug.Log("Conversion canceled.");
      } else if (convertTask.IsFaulted) {
        Debug.Log("Conversion encountered an error:");
        Debug.Log(convertTask.Exception.ToString());
      } else if (convertTask.IsCompleted) {
        Debug.Log("Conversion completed successfully!");
        Debug.Log("ConvertInvitation: Successfully converted invitation");
      }
    }
  }
}
