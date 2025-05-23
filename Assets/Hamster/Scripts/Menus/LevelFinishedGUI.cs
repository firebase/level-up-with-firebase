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

using UnityEngine;
using System.Collections.Generic;

namespace Hamster.Menus {

  // Interface class for providing code access to the GUI
  // elements in the level complete prefab.  It's the menu
  // that shows up when you finish a level.
  public class LevelFinishedGUI : BaseMenu {

    // These fields are set in the inspector.
    public UnityEngine.UI.Text ElapsedTimeText;
    public UnityEngine.UI.Text NewRecordText;
    public GUIButton RetryButton;
    public GUIButton MainButton;
    public GUIButton LevelsButton;
  }
}
