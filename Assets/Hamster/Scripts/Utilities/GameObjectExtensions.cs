﻿// Copyright 2017 Google LLC
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

using System;
using UnityEngine;

namespace Hamster.Utilities {
  // Class to add extensions to Unity's GameObject class.
  public static class GameObjectExtensions {
    // Helper function to access all components of a type on the game object.
    public static void ForEachChildOfType<T>(this GameObject gameObject, Action<T> callback) {
      foreach (T t in gameObject.GetComponentsInChildren<T>()) {
        callback(t);
      }
    }

    // Helper function to access all components of a type on root of the given the game object.
    public static void ForEachRootChildOfType<T>(this GameObject gameObject, Action<T> callback) {
      gameObject.transform.root.gameObject.ForEachChildOfType<T>(callback);
    }
  }
}
