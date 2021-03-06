Shader "BumpedSpecular_Mapped"
{
	Properties 
	{
_Diffuse("_Diffuse", 2D) = "gray" {}
_Normal("_Normal", 2D) = "bump" {}
_SpecularMap("_SpecularMap", 2D) = "black" {}
_SpecularPower("_SpecularPower", Range(0,1) ) = 1
_SpecularColor("_SpecularColor", Color) = (1,1,1,1)
_Lighting("_Lighting", Range(10,30) ) = 11.6
_SpecularLight("_SpecularLight", Range(1,5) ) = 1

	}
	
	SubShader 
	{
		Tags
		{
"Queue"="Geometry"
"IgnoreProjector"="False"
"RenderType"="Opaque"

		}
		
Cull Back
ZWrite On
ZTest LEqual
ColorMask RGBA
Fog{
}

		CGPROGRAM
#pragma surface surf BlinnPhongEditor  vertex:vert
#pragma target 2.0


sampler2D _Diffuse;
sampler2D _Normal;
sampler2D _SpecularMap;
float _SpecularPower;
float4 _SpecularColor;
float _Lighting;
float _SpecularLight;

struct EditorSurfaceOutput {
half3 Albedo;
half3 Normal;
half3 Emission;
half3 Gloss;
half Specular;
half Alpha;
half4 Custom;
			};
			
inline half4 LightingBlinnPhongEditor_PrePass (EditorSurfaceOutput s, half4 light)
			{
half3 spec = light.a * s.Gloss;
half4 c;
c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
c.a = s.Alpha;
return c;

}

inline half4 LightingBlinnPhongEditor (EditorSurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
half3 h = normalize (lightDir + viewDir);
				
half diff = max (0, dot ( lightDir, s.Normal ));
				
float nh = max (0, dot (s.Normal, h));
float spec = pow (nh, s.Specular*128.0);
				
half4 res;
res.rgb = _LightColor0.rgb * diff;
res.w = spec * Luminance (_LightColor0.rgb);
res *= atten * 2.0;

return LightingBlinnPhongEditor_PrePass( s, res );
			}
			
struct Input {
float2 uv_Diffuse;
float2 uv_SpecularMap;
float2 uv_Normal;

			};

			void vert (inout appdata_full v, out Input o) {
float4 VertexOutputMaster0_0_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_1_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_2_NoInput = float4(0,0,0,0);
float4 VertexOutputMaster0_3_NoInput = float4(0,0,0,0);


			}
			
void surf (Input IN, inout EditorSurfaceOutput o) {
o.Normal = float3(0.0,0.0,1.0);
o.Alpha = 1.0;
o.Albedo = 0.0;
o.Emission = 0.0;
o.Gloss = 0.0;
o.Specular = 0.0;
o.Custom = 0.0;
				
float4 Tex2D1=tex2D(_Diffuse,(IN.uv_Diffuse.xyxy).xy);
float4 Tex2D3=tex2D(_SpecularMap,(IN.uv_SpecularMap.xyxy).xy);
float4 Divide0=Tex2D3 / _Lighting.xxxx;
float4 Subtract0=Tex2D1 - Divide0;
float4 Tex2D0=tex2D(_Normal,(IN.uv_Normal.xyxy).xy);
float4 UnpackNormal0=float4(UnpackNormal(Tex2D0).xyz, 1.0);
float4 Tex2D2=tex2D(_SpecularMap,(IN.uv_SpecularMap.xyxy).xy);
float4 Multiply1=_SpecularColor * _SpecularLight.xxxx;
float4 Multiply0=Tex2D2 * Multiply1;
float4 Master0_2_NoInput = float4(0,0,0,0);
float4 Master0_5_NoInput = float4(1,1,1,1);
float4 Master0_7_NoInput = float4(0,0,0,0);
float4 Master0_6_NoInput = float4(1,1,1,1);
o.Albedo = Subtract0;
o.Normal = UnpackNormal0;
o.Specular = _SpecularPower.xxxx;
o.Gloss = Multiply0;
o.Normal = normalize(o.Normal);
			}
		ENDCG
	}
	Fallback ""
}