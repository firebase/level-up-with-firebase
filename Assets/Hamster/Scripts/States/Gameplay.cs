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

namespace Hamster.States {
  public class Gameplay : BaseState {
    // Default/Label
    public const string PhysicsGravityKey ="physics_gravity";
    public const float PhysicsGravityDefault=-20.0f;

    public enum GameplayMode {
      Gameplay,
      Editor,
      TestLoop,
    }

    GameplayMode mode;

    public Gameplay(GameplayMode mode) {
      this.mode = mode;
    }

    Menus.FloatingButtonGUI dialogComponent;

    // Number of fixedupdates that have happened so far in this
    // gameplay session.  Used for replay synchronization.
    public int fixedUpdateTimestamp { get; private set;  }

    // Are we saving gameplay replay to local file?
    // This is never set true in the game.  It is intended as a
    // temporary option for developers to record levels.
    const bool saveReplayToFile = false;

    // The file name to save the replay under.
    const string gameplayReplayFileName = "test_replay.bytes";

    // Whether the best replay record of the current level is available in the storage.
    // If true, the option to download and to play the record will be presented to the player.
    private bool isBestReplayAvailable {
      get {
        return !string.IsNullOrEmpty(bestReplayPath);
      }
    }

    // The storage path to the best replay record stored in the database
    private string bestReplayPath = null;

    // Prefab ID to spawn the object for replay animation
    private const string replayPrefabID = "ReplayPlayer";

    // The state of a replay record
    private enum ReplayState {
      None,         // No record downloaded
      Stopped,      // The record downloaded but not played
      Downloading,  // Downloading the record
      Playing       // Playing the record
    }

    // Initialization method.  Called after the state
    // is added to the stack.
    public override void Initialize() {
      CommonData.mainGame.SelectAndPlayMusic(CommonData.prefabs.gameMusic, true);
      fixedUpdateTimestamp = 0;
      Time.timeScale = 1.0f;
      double gravity_y = PhysicsGravityDefault;
      Physics.gravity = new Vector3(0, (float)gravity_y, 0);
      CommonData.gameWorld.ResetMap();
      Utilities.HideDuringGameplay.OnGameplayStateChange(true);
      CommonData.mainCamera.mode = CameraController.CameraMode.Gameplay;

      Screen.sleepTimeout = SleepTimeout.NeverSleep;

      CommonData.gameWorld.MergeMeshes();
    }

    // Resume the state.  Called when the state becomes active
    // when the state above is removed.  That state may send an
    // optional object containing any results/data.  Results
    // can also just be null, if no data is sent.
    public override void Resume(StateExitValue results) {
      ShowUI();
      CommonData.mainGame.SelectAndPlayMusic(CommonData.prefabs.gameMusic, true);
      Time.timeScale = 1.0f;
      CommonData.mainCamera.mode = CameraController.CameraMode.Gameplay;
      Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public override StateExitValue Cleanup() {
      DestroyUI();
      CommonData.mainCamera.mode = CameraController.CameraMode.Menu;
      Utilities.HideDuringGameplay.OnGameplayStateChange(false);
      Time.timeScale = 0.0f;
      Screen.sleepTimeout = SleepTimeout.SystemSetting;

      return new StateExitValue(typeof(Gameplay));
    }

    public override void Suspend() {
      HideUI();
      Time.timeScale = 0.0f;
      CommonData.mainCamera.mode = CameraController.CameraMode.Menu;
      Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // The event triggered when the player is spawned.
    // This is primarily to reset replay data recording whenever the player respawns
    // However, if saveReplayToFile is set to record replay data for testing, everything
    // will be recorded even if the player dies several times before success.
    void OnPlayerSpawned() {
    }


    public override void HandleUIEvent(GameObject source, object eventData) {
      ExitGameplay();
    }

    void ExitGameplay() {
      CommonData.gameWorld.ResetMap();
      manager.PopState();
    }

    // Called once per frame when the state is active.
    public override void FixedUpdate() {
      if (Input.GetKeyDown(KeyCode.Escape)) {
        ExitGameplay();
        return;
      }

      if (CommonData.mainGame.PlayerController != null) {

        // If the goal was reached, then we want to finish the Gameplay state.
        if (CommonData.mainGame.PlayerController.ReachedGoal) {

          if (mode == GameplayMode.TestLoop) {
            CommonData.mainGame.SelectAndPlayMusic(CommonData.prefabs.menuMusic, true);
            manager.PopState();
          } else {
            CommonData.mainGame.SelectAndPlayMusic(CommonData.prefabs.winMusic, false);
            manager.SwapState(new LevelFinished(mode));
          }
          return;
        }
      }
      fixedUpdateTimestamp++;
    }

    // Set the player's position directly.  Only really used by replay
    // playerinputcontrollers, to compenstate for drift in the physics playback.
    public void SetPlayerPosition(Vector3 position, Quaternion rotation,
        Vector3 velocity, Vector3 angularVelocity) {
      if (CommonData.mainGame.PlayerController != null) {
        Transform transform = CommonData.mainGame.PlayerController.GetComponent<Transform>();
        Rigidbody rigidbody = CommonData.mainGame.PlayerController.GetComponent<Rigidbody>();
        rigidbody.position = position;
        rigidbody.rotation = rotation;
        rigidbody.velocity = velocity;
        rigidbody.angularVelocity = angularVelocity;
      }
    }

  }
}
