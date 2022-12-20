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
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Crashlytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;

namespace Hamster
{
  public class MainGame : MonoBehaviour {
    [HideInInspector]
    public States.StateManager stateManager = new States.StateManager();
    private float currentFrameTime, lastFrameTime;

    private const string PlayerPrefabID = "Player";

    // The active player object in the game.
    [HideInInspector]
    public GameObject player;

    // The player responsible for game music.
    private AudioSource musicPlayer;

    // The PlayerController component on the active player object.
    public PlayerController PlayerController {
      get {
        return player != null ? player.GetComponent<PlayerController>() : null;
      }
    }

    public UnityEvent PlayerSpawnedEvent = new UnityEvent();

    // Volume is treated as an int for the UI display.
    public const int MaxVolumeValue = 6;
    private int musicVolume = 0;
    public int MusicVolume {
      get {
        return musicVolume;
      }
      set {
        musicVolume = value;
        PlayerPrefs.SetInt(StringConstants.MusicVolume, musicVolume);
        // Music volume is controlled on the music source, which is set to
        // ignore the volume settings of the listener.
        CommonData.mainCamera.GetComponentInChildren<AudioSource>().volume =
          (float)musicVolume / MaxVolumeValue;
      }
    }
    private int soundFxVolume = 0;
    public int SoundFxVolume {
      get {
        return soundFxVolume;
      }
      set {
        soundFxVolume = value;
        PlayerPrefs.SetInt(StringConstants.SoundFxVolume, soundFxVolume);
        // Sound effect volumes are controlled by setting the listeners volume,
        // instead of each effect individually.
        AudioListener.volume = (float)soundFxVolume / MaxVolumeValue;
      }
    }

    void Start()
    {
      Screen.SetResolution(Screen.width / 2, Screen.height / 2, true);
      InitializeFirebaseAndStartGame();
    }

    void Update() {
      lastFrameTime = currentFrameTime;
      currentFrameTime = Time.realtimeSinceStartup;
      stateManager.Update();
    }

    void FixedUpdate() {
      stateManager.FixedUpdate();
    }

    // Play an audio clip as music.  If that clip is already playing,
    // we ignore it, and keep playing without restarting.
    public void PlayMusic(AudioClip music, bool shouldLoop) {
      if (musicPlayer.clip != music || !musicPlayer.isPlaying) {
        musicPlayer.Stop();
        musicPlayer.clip = music;
        musicPlayer.Play();
        musicPlayer.loop = shouldLoop;
      }
    }

    // Utility function for picking a random track to play from a selection.
    public void SelectAndPlayMusic(AudioClip[] musicArray, bool shouldLoop) {
      PlayMusic(musicArray[Random.Range(0, musicArray.Length)], shouldLoop);
    }

    // Utility function to check the time since the last update.
    // Needed, since we can't use Time.deltaTime, as we are adjusting the
    // simulation timestep.  (Setting it to 0 to pause the world.)
    public float TimeSinceLastUpdate {
      get { return currentFrameTime - lastFrameTime; }
    }

    // Utility function to check if the game is currently running.  (i.e.
    // not in edit mode.)
    public bool isGameRunning() {
      States.BaseState state = stateManager.CurrentState();
      return (state is States.Gameplay ||
        // While with LevelFinished the game is not technically running, we want
        // to mimic the traditional behavior in the background.
        state is States.LevelFinished);
    }

    // Utility function for spawning the player.
    public GameObject SpawnPlayer() {
      if (player == null) {
        player = (GameObject)Instantiate(CommonData.prefabs.lookup[PlayerPrefabID].prefab);
        PlayerSpawnedEvent.Invoke();
      }
      return player;
    }

    // Utility function for despawning the player.
    public void DestroyPlayer() {
      if (player != null) {
        Destroy(player);
        player = null;
      }
    }

    // Pass through to allow states to have their own GUI.
    void OnGUI() {
      stateManager.OnGUI();
    }

    void InitializeCommonDataAndStartGame()
    {
      CommonData.prefabs = FindObjectOfType<PrefabList>();
      CommonData.mainCamera = FindObjectOfType<CameraController>();
      CommonData.mainGame = this;

      Screen.orientation = ScreenOrientation.LandscapeLeft;

      musicPlayer = CommonData.mainCamera.GetComponentInChildren<AudioSource>();

      CommonData.gameWorld = FindObjectOfType<GameWorld>();

      // Set up volume settings.
      MusicVolume = PlayerPrefs.GetInt(StringConstants.MusicVolume, MaxVolumeValue);
      // Set the music to ignore the listeners volume, which is used for sound effects.
      CommonData.mainCamera.GetComponentInChildren<AudioSource>().ignoreListenerVolume = true;
      SoundFxVolume = PlayerPrefs.GetInt(StringConstants.SoundFxVolume, MaxVolumeValue);

      // Subscribes to on config update after first initial fetch and adtivate
      FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += ActivateValuesOnConfigUpdate;

      stateManager.PushState(new States.MainMenu());
    }

    public Firebase.FirebaseApp app = null;

    // Begins the firebase initialization process and afterwards, opens the main menu.
    private void InitializeFirebaseAndStartGame()
    {
      Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(
        previousTask => 
        {
          var dependencyStatus = previousTask.Result;
          if (dependencyStatus == Firebase.DependencyStatus.Available) {
            // Create and hold a reference to your FirebaseApp,
            app = Firebase.FirebaseApp.DefaultInstance;
            // Set the recommended Crashlytics uncaught exception behavior.
            Crashlytics.ReportUncaughtExceptionsAsFatal = true;
            SetRemoteConfigDefaults();
          } else {
            UnityEngine.Debug.LogError(
              $"Could not resolve all Firebase dependencies: {dependencyStatus}\n" +
              "Firebase Unity SDK is not safe to use here");
          }
        });
    }

    // Sets Remote Config default values and fetchs new ones
    // before starting the game.
    private void SetRemoteConfigDefaults()
    {
      var defaults = new System.Collections.Generic.Dictionary < string, object > ();
      defaults.Add(
        Hamster.MapObjects.AccelerationTile.AccelerationTileForceKey,
        Hamster.MapObjects.AccelerationTile.AccelerationTileForceDefault);
      defaults.Add(
        Hamster.States.MainMenu.SubtitleOverrideKey,
        Hamster.States.MainMenu.SubtitleOverrideDefault);
      var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
      remoteConfig.SetDefaultsAsync(defaults).ContinueWithOnMainThread(
        previousTask =>
        {
          FetchRemoteConfig(InitializeCommonDataAndStartGame);
        }
      );
    }

    // (Re)fetches Remote Config values and pass down the onFetchAndActivateSuccessful callback.
    // Called during the initialization flow but can also be called indepedently.
    public void FetchRemoteConfig(System.Action onFetchAndActivateSuccessful)
    {
      if(app==null)
      {
        Debug.LogError($"Do not use Firebase until it is properly initialized by calling {nameof(InitializeFirebaseAndStartGame)}.");
        return;
      }

      Debug.Log("Fetching data...");
      var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
      remoteConfig.FetchAsync(System.TimeSpan.Zero).ContinueWithOnMainThread(
        previousTask=>
        {
          if (!previousTask.IsCompleted)
          {
            Debug.LogError($"{nameof(remoteConfig.FetchAsync)} incomplete: Status '{previousTask.Status}'");
            return;
          }
          ActivateRetrievedRemoteConfigValues(onFetchAndActivateSuccessful);
        });
    }

    // The final method in the initialization flow that will activate fetched values
    // and on Success will call onFetchAndActivateSuccessful.
    private void ActivateRetrievedRemoteConfigValues(System.Action onFetchAndActivateSuccessful)
    {
      var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
      var info = remoteConfig.Info;
      if(info.LastFetchStatus == LastFetchStatus.Success)
      {
        remoteConfig.ActivateAsync().ContinueWithOnMainThread(
          previousTask =>
          {
            Debug.Log($"Remote data loaded and ready (last fetch time {info.FetchTime}).");
            onFetchAndActivateSuccessful();
          });
      }
    }

    private void OnDestroy() 
    {
      FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener -= ActivateValuesOnConfigUpdate;
    }

    void ActivateValuesOnConfigUpdate( object sender, ConfigUpdateEventArgs args) {
      if (args.Error != RemoteConfigError.None) {
        Debug.Log($"Error occurred while listening: {args.Error}");
        return;
      }

      Debug.Log("Updated keys: " + string.Join(", ", args.UpdatedKeys));
      // Activate all fetched values and then display a welcome message.
      var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
      remoteConfig.ActivateAsync().ContinueWithOnMainThread(
        task => {
            Debug.Log($"Keys from {nameof(ActivateValuesOnConfigUpdate)} activated.");
        });
    }
  }
}
