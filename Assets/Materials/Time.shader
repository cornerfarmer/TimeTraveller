// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/Time" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Texture2("Noise", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
			Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 200
		Cull Front
		Lighting Off
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf NoLighting alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		sampler2D _MainTex;
		sampler2D _Texture2;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			const float PI = 3.14159;
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed2 dir = IN.worldPos.xy - fixed2(10, -5);
			float angle = acos(dot(fixed2(0, -1), dir) / length(dir)) / PI;
			float swirlFac = 0;

			if (IN.worldPos.z < -100)
				swirlFac = 0.07f;
			else
				swirlFac = IN.worldPos.z / -100 * 0.07f;
			
			angle += IN.worldPos.z*swirlFac *(dir.x < 0 ? 1 : -1);
			
			//if (angle < 0)
			//	angle += (int)(abs(angle) / (PI * 2) + 1) * (PI * 2);

			float pos = fmod(abs(angle), 1);
			if (tex2D(_Texture2, fixed2(pos, pos)).r > 5 - IN.worldPos.z / -10)
				o.Alpha = 0;
			else
				o.Alpha = 1;

			o.Albedo.rgb = tex2D(_MainTex, fixed2(IN.worldPos.z / 100, angle)).rgb * 0.7f;

		}

		ENDCG
	}
	FallBack "Diffuse"
}
