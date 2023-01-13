**MechaHamster: Level Up with Firebase Edition**
======

The original version of MechaHamster is a game about guiding a futuristic hamster ball through dangerous space mazes, allowing users to create mazes of their own and share them with friends.

This new version has gone back to basics and removed Firebase and all other Google products’ functionality altogether. It's your mission, if you choose to accept it, to personally integrate some of it yourself!

## Motivation

While the original MechaHamster serves as a demonstration, sample, and reference for how to integrate 
[Firebase][] into a game project using the the [Firebase Unity SDK][] - this time, as you rebuild the Firebase
functionality of the game yourself you'll learn more about how Firebase works first hand - trial by fire!

## Overview

After this codelab, the completed version of **MechaHamster: Level Up with Firebase Edition** demonstrates the following concepts:
   * [Firebase Analytics][] to measure various aspects of user behavior.
   * [Firebase Crashlytics][] to capture and annotate crashes in gameplay that will help developers diagnose and fix issues.
   * [Firebase Remote Config][] to allow game admins to run experiments on game data without
     redeploying a new build of the game.

## Downloading

**MechaHamster: Level Up with Firebase Edition** can be downloaded from [GitHub][] using  the following command:
```
git clone --branch main  https://github.com/firebase/level-up-with-firebase.git
```

## Building and Debugging

While the intended behavior path is that you will build the game after adding Firebase, it can be helpful to understand and refine your build process before that in order to make sure everything is set up properly ahead of time.

Though the [steps to build the game](build-and-debug-guide.md#Building) are in principle the same before and after adding Firebase functionality, debugging the build process after adding Firebase has [additional steps](build-and-debug-guide.md#Debugging). 

 
To contribute the this project see [CONTRIBUTING][].

  [CONTRIBUTING]: https://github.com/firebase/level-up-with-firebase/blob/main/CONTRIBUTING.txt 
  [GitHub]: https://github.com/firebase/level-up-with-firebase/
  [Firebase]: https://firebase.google.com/docs/
  [Firebase Unity SDK]: https://firebase.google.com/docs/unity/setup
  [Firebase Analytics]: https://firebase.google.com/docs/analytics/
  [Firebase Crashlytics]: https://firebase.google.com/docs/crashlytics/
  [Firebase Remote Config]: https://firebase.google.com/docs/remote-config/
