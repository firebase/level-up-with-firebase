// Copyright 2022 Google Inc. All rights reserved.
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

// Simplified Specular shader. Differences from regular Bumped Specular one:
// - no Main Color nor Specular Color
// - specular lighting directions are approximated per vertex
// - writes zero to alpha channel
// - no Deferred Lighting support
// - no Lightmap support
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Mobile/Specular" {
Properties {
  _Shininess ("Shininess", Range (0.03, 1)) = 0.078125
  _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
}
SubShader {
  Tags { "RenderType"="Opaque" }
  LOD 250

CGPROGRAM
#pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

inline fixed4 LightingMobileBlinnPhong (SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
{
  fixed diff = max (0, dot (s.Normal, lightDir));
  fixed nh = max (0, dot (s.Normal, halfDir));
  fixed spec = pow (nh, s.Specular*128) * s.Gloss;
  fixed4 c;
  c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
  UNITY_OPAQUE_ALPHA(c.a);
  return c;
}

sampler2D _MainTex;
half _Shininess;

struct Input {
  float2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
  fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
  o.Albedo = tex.rgb;
  o.Gloss = tex.a;
  o.Alpha = tex.a;
  o.Specular = _Shininess;
}
ENDCG
}

FallBack "Mobile/VertexLit"
}
