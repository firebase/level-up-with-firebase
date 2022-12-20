// Copyright 2022 Google LLC
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using System.Threading;

namespace Hamster.States {
  class DebugMenu : BaseState {
    private Menus.DebugMenuGUI menuComponent = null;
    private readonly GameObject guiMenuPrefab;

    public DebugMenu(GameObject menuPrefabToSpawn) {
        guiMenuPrefab = menuPrefabToSpawn;
    }

    // Initialization method.  Called after the state
    // is added to the stack.
    public override void Initialize() {
        InitializeUI();
    }


    private void InitializeUI() {
      if (menuComponent == null) {
        menuComponent = SpawnUI<Menus.DebugMenuGUI>(guiMenuPrefab);
      }
      ShowUI();
    }

    public override StateExitValue Cleanup() {
      DestroyUI();
      return null;
    }

    public override void Suspend() {
      HideUI();
    }

    public override void Resume(StateExitValue results) {
      InitializeUI();
    }

    public override void HandleUIEvent(GameObject source, object eventData) {
      if (source == menuComponent.BackButton.gameObject) {
        manager.PopState();
      } else if(source == menuComponent.CrashNowButton.gameObject){
        CrashNow();
      } else if(source == menuComponent.LogNonfatalErrorButton.gameObject){
        LogNonfatalError();
      } else if(source == menuComponent.LogStringsAndCrashNowButton.gameObject){
        LogStringsAndCrashNow();
      } else if(source == menuComponent.SetCustomKeyAndCrashNowButton.gameObject){
        SetAndOverwriteCustomKeyThenCrash();
      } else if(source == menuComponent.SetLogsAndKeysBeforeANRingButton.gameObject){
        MainThreadRunner.RunOnMainThread(SetLogsAndKeysBeforeANR);
      } else if(source == menuComponent.LogProgressEventWithStringLiteralsButton.gameObject){
        LogProgressEventWithStringLiterals();
      } else if(source == menuComponent.LogIntScoreWithBuiltInEventAndParamsButton.gameObject){
        LogIntScoreWithBuiltInEventAndParams();
      } else if(source == menuComponent.SetUserBoredOfSubtitleButton.gameObject){
        SetUserBoredOfSubtitle();
      } else if(source == menuComponent.SetUserEnjoysSubtitleButton.gameObject){
        SetUserEnjoysSubtitle();
      }
    }

    private void TestCrash()
    {
#if UNITY_2018_3_OR_NEWER
      // More info: https://docs.unity3d.com/ScriptReference/Diagnostics.Utils.ForceCrash.html
      const string CRASH_METHOD_NAME = nameof(Utils.ForceCrash);
      System.Action crashMethod = ()=> Utils.ForceCrash(ForcedCrashCategory.FatalError);
#else
      // Backwards compatibility friendly.
      const string CRASH_METHOD_NAME = nameof(UnityEngine.Application.ForceCrash);
      System.Action crashMethod = ()=> UnityEngine.Application.ForceCrash(0);
#endif
      if(Application.isEditor)
      {
        Debug.LogError($"{nameof(TestCrash)} called in editor. Avoiding a real {CRASH_METHOD_NAME} of the editor itself to prevent data loss.");
      }
      else
      {
        // Causes real crashes outside of the editor. Only use while testing.
        // Crashing the editor can lead to lost work and is not a supported use case of Crashlytics.
        crashMethod();
      }
    }

    void CrashNow()
    {
      throw new System.NotImplementedException();
    }

    // Caught nonfatal exceptions can be sent to Crashlytics
    // as full issues by using Crashlytics.LogException 
    void LogNonfatalError()
    {
      throw new System.NotImplementedException();
    }

    // Crashlytics allows you to add logs to your issues (Crashes, Errors and ANRs)
    // with the Crashlytics.Log command. These are strictly additive.
    void LogStringsAndCrashNow()
    {
      throw new System.NotImplementedException();
    }

    // Crashlytics allows you to associate key-value pairs with callstacks.
    // These values can then be used to filter issues.
    // You should restrict the potential values to a small set for usability.
    // One way of doing this is to separate the potential field
    // into discrete chunks (aka "discretizing the input").
    void SetAndOverwriteCustomKeyThenCrash()
    {
      throw new System.NotImplementedException();
    }

    // Uses logging and custom keys to help diagnose existing & potential ANRs.
    // We will use the stopwatch class to time the execution of various functions
    // Logs functions that can potentially cause an ANR 
    // Records a name for each async call.
    // While this seems redundant, if an ANR occurs, the runtime won't be recorded
    // Instead you will not have logged or recorded what method caused the ANR.
    // Thus, if the method name is unset, it occurred there
    void SetLogsAndKeysBeforeANR()
    {
      throw new System.NotImplementedException();
    }

    // Log an event with a float parameter
    void LogProgressEventWithStringLiterals()
    {
      throw new System.NotImplementedException();
    }

    // Log an event with an int parameter using predefined string constants
    void LogIntScoreWithBuiltInEventAndParams()
    {
      throw new System.NotImplementedException();
    }

    // Sets a Google Analytics user property 'subtitle_sentiment' to 'bored'
    void SetUserBoredOfSubtitle()
    {
      throw new System.NotImplementedException();
    }

    // Sets a Google Analytics user property 'subtitle_sentiment' to 'enjoys'
    void SetUserEnjoysSubtitle()
    {
      throw new System.NotImplementedException();
    }

    public static class DayDivision {
      public enum Phase{Morning, Afternoon, Evening, Night};
      public static readonly System.TimeSpan DURATION_THAT_ENSURES_PHASE_CHANGE = new System.TimeSpan(9,0,0);
      private static readonly System.TimeSpan morningStartTime = new System.TimeSpan(5, 0, 0);
      private static readonly System.TimeSpan afternoonStartTime = new System.TimeSpan(12, 0, 0);
      private static readonly System.TimeSpan eveningStartTime = new System.TimeSpan(17, 0, 0);
      private static readonly System.TimeSpan nightStartTime = new System.TimeSpan(21, 0, 0);
      public static Phase GetPartOfDay(System.TimeSpan time) {
          // This function takes the supplied time of day and maps it to the PartOfDay enum
          
          if(time>=nightStartTime || time < morningStartTime) {
              return Phase.Night;
          }
          else if (time>= eveningStartTime) {
              return Phase.Evening;
          }
          else if(time>=afternoonStartTime) {
              return Phase.Afternoon;
          }
          
          return Phase.Morning;
      }
    }

    private static class BusyWaitSimulator {
      public const long SEVERE_DURATION_MILLIS = 4500L;
      public const long EXTREME_DURATION_MILLIS = 5000L;
      public static void WaitOnSimulatedBlockingWork(long millisecondsToWork) {
          // A simulation of work being done by a thread.
          var thread = new Thread(() => {Thread.Sleep((int)millisecondsToWork);});
          thread.Start();
          while(thread.IsAlive){}//This makes the thread blocking 
      }
    }
    
    private static class MainThreadRunner
    {
#if UNITY_ANDROID
      const string DEFAULT_UNITY_PLAYER_ACTIVITY_NAME = "com.unity3d.player.UnityPlayer";
      const string CURRENT_ACTIVITY_KEY = "currentActivity";
      const string ACTIVITY_UI_THREAD_RUN_METHOD_NAME = "runOnUiThread";
      const string UNITY_ANDROID_CUSTOM_ACTIVITY_DOC_URL = "https://docs.unity3d.com/Manual/android-custom-activity.html";
      static readonly string UI_THREAD_RUN_ERROR_MESSAGE = $"{nameof(System.InvalidOperationException)}: "+
          " The attempt to run this method on the Android main thread failed."+
          $"If your Android Project's activity is named something other than {DEFAULT_UNITY_PLAYER_ACTIVITY_NAME}, "+
          $"Change the value of {nameof(DEFAULT_UNITY_PLAYER_ACTIVITY_NAME)} to the correct name. "+
          $" More information available here: {UNITY_ANDROID_CUSTOM_ACTIVITY_DOC_URL}";
#endif
      public static void RunOnMainThread(System.Action methodToRun)
      {
        if(methodToRun==null)
        {
          return;
        }
#if UNITY_ANDROID
        try
        {
          AndroidJavaClass unityPlayer = new AndroidJavaClass(DEFAULT_UNITY_PLAYER_ACTIVITY_NAME);
          AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>(CURRENT_ACTIVITY_KEY);
          activity.Call(ACTIVITY_UI_THREAD_RUN_METHOD_NAME, new AndroidJavaRunnable(methodToRun));
        }
        catch(System.Exception exception)
        {
          throw new System.InvalidOperationException(UI_THREAD_RUN_ERROR_MESSAGE);
        }
#else
        methodToRun();
#endif
      }
    }
  }
}
