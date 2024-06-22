Shader "UnitySensors/Range"
{
	Properties{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 100

		CGPROGRAM
		// Physically based Standard lighting model
#pragma surface surf Standard addshadow
#pragma multi_compile_instancing
#pragma instancing_options procedural:setup

		sampler2D _MainTex;

	struct Input
 {
float2 uv_MainTex;
};

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	struct Range
	{
		float range;
		// float3 position;
		float intensity;
	};
	StructuredBuffer<Range> RangesBuffer;
	float startAngle;
	float stepAngle;
	float minRange;
	float maxRange;
	float4x4 LocalToWorldMatrix;
#endif

	void setup()
	{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		// float3 data = mul(LocalToWorldMatrix, float4(RangesBuffer[unity_InstanceID].position, 1));
		float distance = RangesBuffer[unity_InstanceID].range;
		float angle = (startAngle + unity_InstanceID * stepAngle) * 0.0174533;
		float x = distance * cos(angle);
		float y = distance * sin(angle);

		float3 data = mul(LocalToWorldMatrix, float4(x, y, 0, 1));
		unity_ObjectToWorld._11_21_31_41 = float4(1, 0, 0, 0);
		unity_ObjectToWorld._12_22_32_42 = float4(0, 1, 0, 0);
		unity_ObjectToWorld._13_23_33_43 = float4(0, 0, 1, 0);
		unity_ObjectToWorld._14_24_34_44 = float4(data, 1);
		unity_WorldToObject = unity_ObjectToWorld;
		unity_WorldToObject._14_24_34 *= -1;
		unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
#endif
	}

	half _Glossiness;
	half _Metallic;

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		// float4 col = float4(r, g, b, 1.0);
		float4 col = 1.0f;

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
		// col = pointsBuffer[unity_InstanceID].intensity;
		float distance = RangesBuffer[unity_InstanceID].range;
		float i = (distance - minRange) / maxRange;
		float r = clamp((4.0 * i) - 2.0, 0.0, 1.0);
		float g1 = clamp(( 4.0 * i)      , 0.0, 1.0);
		float g2 = clamp((-4.0 * i) + 4.0, 0.0, 1.0);
		float g = lerp(g1, g2, step(0.5, i));;
		float b = clamp((-4.0 * i) + 2.0, 0.0, 1.0);

		col = float4(1, 1, 1, 1);
		// col = float4(r, g, b, 1);
#else
		col = float4(1, 1, 1, 1);
#endif

		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * col;
		o.Albedo = c.rgb;
		o.Metallic = _Metallic;
		o.Smoothness = _Glossiness;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
