# **MechaHamster: Level Up with Firebase Edition**: Build and Debugging Guide

# <a name="Intro"></a> Explanation and Links to Other Resources

The following are respectively

1.  A basic guide to building and deploying the game to a connected Android or
    iOS device or emulator
2.  A very detailed guide to debugging the build process

In principle you should be able to build the game using the
[build section](#Building) but if the process either fails to produce a build or
if the build fails when run, go through the steps outlined in the
[debugging](#Debugging) section.

If after doing that, consulting the [troubleshooting FAQ](https://firebase.google.com/docs/unity/troubleshooting-faq) and searching
[documentation](https://firebase.google.com/docs/reference/unity) and helpful
forums (such as [Unity Answers](https://answers.unity.com/index.html)) feel free
to investigate the existing
[issues page of **MechaHamster: Level Up with Firebase Edition**](https://github.com/firebase/quickstart-unity/level-up-with-firebase/issues?q=)
and file an issue there if you don't see one similar to your concern.

While the following are not directly related to issues concerning this project,
if you need other sources for possible info and debugging steps, please look
into the issues pages for the
[**ORIGINAL** Mechahamster](https://github.com/google/mechahamster/issues?q=is%3Aissue+)
and the
[Firebase Unity Quickstart Guide](https://github.com/firebase/quickstart-unity/issues?q=is%3Aissue+).

<br>
<br>
<br>
<br>
<br>
<br>

# <a name="Building"></a> Building

1.  Open the project in at least Unity 2019.
2.  Before building, verify you can play the game in Editor.
3.  Resolve any errors.
    *   [Unity: Fixing compiler errors before entering playmode][]
    *   [Unity: How do I interpret a compiler error?][]
4.  Select your build target from `File > Build Settings`.
    *   Supported Targets:
        *   iOS
        *   Android
5.  If you are building for an Android or iOS Simulator/Emulator enter `File >
    Build Settings > Player Settings`:
    *   iOS
        *   Enable "Simulator SDK"
            *   Without this, the game will not run on a simulator.
    *   Android
        *   Enable the [correct target architecture][]
6.  Select the `File > Build Settings` menu option then click `Build and Run`.

<br>
<br>
<br>
<br>
<br>
<br>

# <a name="Debugging"></a> Debugging the Build, Install and Run Process

The first time building out the game can be tricky. The process is full of
pitfalls and much of the documentation on it is decentralized. To help with
this, below is a succinct guide that describes how to investigate and solve many
of the more common issues. It is arranged in order of where these errors may
occur.
<br>
<br>

##  Unity Editor Launch: Compile-time errors and Safe Mode<br>
Unity can catch compilation errors when the editor is loading/initializing. If there are issues building out the project in the current build configuration, Unity will display a dialogue asking if you want to open the project in [Safe Mode](https://docs.unity3d.com/2020.2/Documentation/Manual/SafeMode.html).

*   The choice is not actually that important: <br> You can solve the compilation issues either in or not in safe mode.
    *   The major difference is that in safe mode, once you have finished
            fixing the issues you need to restart Unity.
*   In either case follow the instructions in
        [Play in Editor Compile-time Issues](#Debugging-Compilation) to learn
        more about how to resolve these issues.
<br>
<br>

##  Play in Editor Compile-time Issues<a name="Debugging-Compilation"></a> <br>
&emsp;The first class of build issues will occur before you try to start a mobile build. When Unity starts up or detects changes to dependencies, code or other assets, it will try to rebuild the project. If the project is unable to compile at that time the Editor will log
        compilation errors to the console and if you attempt to play in
        editor you will receive a an error popup in Unity's `Scene` tab that
        reads **"All compiler errors have to be fixed before you can enter
        playmode!"**
<details>
<summary>Debugging Process</summary>

1.  Look at the compilation errors in the Unity Editor's console tab and
            try to solve them. **If play in editor still fails, proceed to the
            next step.**
    *   See the following articles for more information:
        *   [Unity: Fixing compiler errors before entering playmode][]
        *   [Unity: How do I interpret a compiler error?][]
2.  If you have implemented firebase functionality into the code have
        you imported the appropriate Firebase packages? **If play in editor
        still fails, proceed to the next step.**
    *   Either:
        *   [Add Firebase Unity SDKs](https://firebase.google.com/docs/unity/setup#add-sdks)
        *   Look into and perform one of the alternatives in
                [Additional Unity installation options](https://firebase.google.com/docs/unity/setup-alternative).
3.  If you have imported the packages as `.unitypackage`s, have you made
        sure they are at the appropriate API level?
    *   Detailed
            [here](https://firebase.google.com/docs/unity/setup#add-sdks)
4.  Check that your editor is attempting to rebuild.
    *   By default the Unity editor is set to rebuild whenever asset or
            configuration changes are detected.
    *   It is possible (and sometimes desirable) that this functionality
            has been disabled and that your Unity Editor is set to
            [manual refresh/recompile](https://support.unity.com/hc/en-us/articles/210452343-How-to-stop-automatic-assembly-compilation-from-script#:~:text=You%20can%20change%20this%20behavior,or%20Stop%20Playing%20And%20Recompile).
    *   **To Fix this** either
        *   Invoke manual refresh (Ctrl+R on Windows or ⌘+R on Mac)
        *   Enter **Edit** > **Preferences** > **General** and enable **Auto Refresh** 
</details>
    

<br>
<br>

##  Play in Editor Firebase Runtime Issues<a name="Debugging-PIE-Runtime-Errors"></a> <br>
&emsp;If your game starts but runs into issues with Firebase while running,
        proceed through the following steps regarding `google-services.json`
        (Android) or `GoogleService-Info.plist` (iOS) until the problem is
        resolved.

1.  Make sure your build settings are set for the target you intend (iOS
        or Android) in `File > Build Settings`
2.  Download the config file(s) for your app and build target from the
        Firebase console in **Project Settings** > **Your Apps**
    *   If you already have these files, delete them in your project and
            replace them with the most recent version, making sure that they
            are spelled exactly as displayed above without "(1)" or other
            numbers attached to the file names.
3.  If the console contains a message regarding files in "Assets/StreamingAssets/", make sure there are no console messages saying unity was unable to edit files there.
            <br>
<br>
<br>
##  Target Device Build Errors<br>
&emsp;This is by far the most complicated part of the process and the most prone to errors. The section is divided into three sections which are less chronological and have more to do with your build target. The first section ("General Tips'') applies regardless of your target, while the remaining two have to do with which target you are building for (Android or iOS).
<details><summary>General Tips</summary>

*   If literally nothing in this document gets the game to build and run
        successfully, upgrade your install the newest LTS Unity Editor and
        [update your packages](https://firebase.google.com/docs/unity/troubleshooting-faq#update-sdk-version).
*   If at any point you want or need more information consider enabling
        verbose logging and reading the log after trying to build again
    *   to enable verbose logging do the following on
        *   iOS
            *   Enable "Verbose Logging" in Assets > External Dependency
                    Manager > iOS Resolver > Settings
        *   Android
            *   Enable "Verbose Logging" in Assets > External Dependency
                    Manager > Android Resolver > Settings
</details>
</details>
<details><summary>Android Resolver</summary>
<blockquote>
<details>
<summary>Use Force Resolve if having issues with resolution</summary>

*   When trying to build, if you receive errors, try to resolve android
        dependencies by clicking Assets > External Dependency Manager >
        Android Resolver > Resolve
    *   If this fails, try Assets > External Dependency Manager >
            Android Resolver > **Force** Resolve
        *   While this is slower, it is more dependendable as it clears
                old intermediate data
    *   if the build is still failing proceed

</details>
<details>
<summary>JDK, SDK, NDK, Gradle Issues (including locating)</summary>

*   If you receive error logs about Unity being unable to locate the
        JDK, Android SDK Tools or NDK you may need to toggle some of your
        external tool settings to get Unity to acknowledge them
    *   Enter Unity->Preferences->External Tools
    *   Disable(uncheck) the JDK,Android SDK,Android NDK and Grandle
    *   Restore the settings to their original values
    *   try building again
</details>
<details>
<summary>Enable Jetifier if you can/ unless you know you can't</summary>

*   Unless you have a solid reason NOT to enable jetifier, ENABLE IT
            in any Unity version after 2019.2
        *   long story short, it's only not enabled by default due to
                not being something that can be done automatically
        *   If you do this you MUST enable Maintemplate.gradle (and
                custom gradle property template aif using 2019.3+ )
    *   SYMPTOMS SHOWING YOU SHOULD DO THIS
        *   When using the NONgradle version EDM4U will fail resolution when trying to bump up a package version number by changing it to <num>.0.+<br>Unity will TRY to decide on a version.
        *   common source of failure: 2 different versions of same
                library
</details>
<details>
<summary>'Single Dex' Issues and Minification</summary>

*   If you run into an issue with "single dex"
    *   [Look into minification](https://firebase.google.com/docs/unity/troubleshooting-faq#an_issue_with_single_dex_while_building_android_app)
        *   Note: Do not apply different minification rules for
                different build configurations (debug vs release etc)
</details>
</blockquote>

</details>

<details>
<summary>iOS Resolver</summary>

<blockquote>
<details>
<summary>Prefer opening Xcode Workspace files in Xcode to opening Xcode Projectr Files</summary>

*   Try to
        [build iOS builds from Xcode Workspaces](https://developer.apple.com/library/archive/featuredarticles/XcodeConcepts/Concept-Workspace.html)
        generated by cocoapods rather than Xcode Projects
    *   if you are building in an evnironment you cannot open Xcode
            workspaces from (such as unity cloud build) then
            alternatively go into the iOS resolver settings, enter the
            dropdown **Cocoapods Integration** and select "Xcode
            project"
</details>
<details>
<summary>Investigate Cocoapods if iOS resolution fails</summary>

*   First of all make sure its
            [properly installed](https://guides.cocoapods.org/using/getting-started.html)
    *   Verify that
            [`pod install` and `pod update` run without errors](https://guides.cocoapods.org/using/pod-install-vs-update.html)
        *   Cocoapods
                [requires your terminal to be using UTF-8](https://firebase.google.com/docs/unity/troubleshooting-faq#issues_when_building_for_ios_with_cocoapods)
            *   Do this if you see the following in the cocoapods log <br>`WARNING: CocoaPods requires your terminal to be using UTF-8 encoding.`
</details>
<details>
<summary>Win32 Errors when building on Mac</summary>

*   If the Unity Editor's console displays build output that
            mentions win32 errors, upgrade to a more recent LTS version
            of Unity after 2020.3.40f1.
    *   [Unity Bug](https://issuetracker.unity3d.com/issues/webgl-builderror-constant-il2cpp-build-error-after-osx-12-dot-3-upgrade)
    *   While this might seem extreme since there are a couple
            workarounds, upgrading is the fastest, most convenient
            and the most dependable method to deal with it.
</details>
</blockquote>

</details>


<br>
<br>
<br>

##  Target Device Runtime Errors<br>
&emsp;This section will cover less about solving on-device Firebase runtime errors and more about how to investigate on-device errors in the first place.
<details><summary>Android</summary>
<blockquote>
<details>
<summary>Inspecting Logs</summary>

*   On Simulator:
    *   Inspect the logs displayed in your Emulator's console.
*   On Device
    *   Familiarize yourself with
            [adb](https://developer.android.com/studio/command-line/adb)
            and
            [adb logcat](https://developer.android.com/studio/command-line/logcat#filteringOutput)
            and how to use it.
        *   While you can use your command line environments various
                tools to filter the output, consider alternatively
                looking into logcats
                [options](https://developer.android.com/studio/command-line/logcat#options).
        *   A simple way to start an ADB session with a clean slate and then going about whatever operation you care to do is:<br>
                `adb logcat -c && adb logcat <OPTIONS>`<br>
                where <options> are whatever flags you pass the command line
</details>

<details>
<summary>Installing and Running on Simulator</summary>

*   make sure to specify the CPU architecture of the machine you are
        running the emulator on in in File > Build Settings > Player
        Settings > Player > Android > Other Settings > Configuration >
        Target Architectures
    *   If you do not do this, builds may succeed but be unable to
            install/run on the simulator
</details>
</blockquote>
</details>

<details>
<summary>iOS</summary>
<blockquote>
<details>
<summary>Inspecting Logs</summary>

*   On Simulator
    *   Look at Xcode's logs
*   On Device
    *   hook up to computer and look at lldb in Xcode
</details>

<details>
<summary>Swift Issues</summary>

*   If you run into an issues on trying to run the game with error
        logs that mention swift
    1.  Make sure your project generates
        *   a podfile AND
        *   an xcworkspace file
    2.  Make sure you are opening the xcworkspace file in Xcode
            rather than an Xcode project
    3.  If the build is still not properly running on device, enable
            `Enable Swift Framework Support Workaround` in Assets >
            External Dependency Manager > iOS Resolver > Settings
</details>

<details>
<summary>Installing and Running on Simulator</summary>

*   make sure to specify the Simulator SDK in File > Build
        Settings > Player Settings > Player > iOS > Other Settings >
        Configuration > Target SDK
    *   If you do not do this, builds may succeed but be unable to
            install/run on the simulator
</details>

</blockquote>
</details>

<br>
<br>
<br>

[Unity: Fixing compiler errors before entering playmode]: https://support.unity.com/hc/en-us/articles/205637689-Why-do-I-get-a-All-compiler-errors-have-to-be-fixed-before-you-can-enter-playmode-error-
[Unity: How do I interpret a compiler error?]: https://support.unity.com/hc/en-us/articles/205930539-How-do-I-interpret-a-compiler-error-
[correct target architecture]: https://docs.unity3d.com/2021.2/Documentation/Manual/android-BuildProcess.html#:~:text=Splitting%20APKs%20by-,target%20architecture,-If%20your%20output
[Original MechaHamster Documentation]: https://google.github.io/mechahamster/
