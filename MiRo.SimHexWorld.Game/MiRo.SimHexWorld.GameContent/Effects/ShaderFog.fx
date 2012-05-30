float4x4 World : World;
float4x4 WorldViewProject : WorldViewProjection;
float3 EyePosition : CameraPosition;
texture thisTexture;
float3  LightPos  = (0,-1,0);

float4 ambient = {1.0, 1.0, 1.0, 1.0};
float4 diffuse = {1.0, 1.0, 1.0, 1.0};
float4 specularColor = {0.2, 0.2, 0.2, 1.0};
float shininess = 40;

//float fogNear = 30;
//float fogFar = 100;
//float fogAltitudeScale = 10;
//float fogThinning = 100;
//float4 fogColor = {0.8, 0.8, 0.8, 1.0};

//texture fogTexture;
//float fogTimer;
//sampler FogTexture = sampler_state
//{
//	Texture = <fogTexture>;
//};

sampler TextureSampler = sampler_state
{
	Texture = <thisTexture>;
};

struct VS_INPUT 
{
	float4 Position : POSITION0;
	float2 Texcoord : TEXCOORD0;
	float3 Normal : NORMAL0;
};

struct VS_OUTPUT 
{
	float4 Position : POSITION;
	float2 Texcoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;	
	float3 WorldPos : TEXCOORD2;
};
struct PS_INPUT 
{
	float4 Position : TEXCOORD4;
	float2 Texcoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;	
	float3 WorldPos : TEXCOORD2;
};

float4 blinn2(
        float3 N,
		float3 L,
		float3 V,
		uniform float4 diffuseColor,
		uniform float4 specularColor,
		uniform float shininess)
{
	float3 H = normalize(V+L);
	float4 lighting = lit(dot(L,N), dot(H,N), shininess);
	return diffuseColor*lighting.y + specularColor*lighting.z;
}

float2 SwirlHoriz(float2 coord)
{	
	//coord.x -= fogTimer;
	if(coord.x < 0)	
		coord.x = coord.x+1;
	
	return coord;
}
float2 SwirlVert(float2 coord)
{
	//coord.y -= fogTimer;
	if(coord.y < 0)	
		coord.y = coord.y+1;
	
	return coord;
}

VS_OUTPUT Transform(VS_INPUT Input) 
{
	VS_OUTPUT Output;

	Output.WorldPos = mul(Input.Position,World);
	Output.Position = mul(Input.Position, WorldViewProject);	
	Output.Texcoord = Input.Texcoord;
	Output.Normal = mul(Input.Normal,World);

	return Output;
}

float4 Texture(PS_INPUT Input) : COLOR0 
{
	float4 colorMap = tex2D(TextureSampler, Input.Texcoord.xy) * 1.5;
	//float2 fogUV = Input.WorldPos.zx * .001;
	//float4 fog = (tex2D(FogTexture, SwirlVert(fogUV)));
	//fog += (tex2D(FogTexture, SwirlHoriz(fogUV)));	  
    float4 C = ambient*colorMap;    
    //float4 tmpFogColor = fogColor + fog * .01;
    
	C += blinn2(normalize(Input.Normal), normalize(LightPos - Input.WorldPos), normalize(EyePosition - Input.WorldPos), colorMap * diffuse, specularColor * colorMap.a, shininess);
	//float d = length(Input.WorldPos - EyePosition);
	//float l = saturate((d - fogNear) / (fogFar - fogNear) / clamp(Input.Position.y / fogAltitudeScale + 1, 1, fogThinning));
	return C; //lerp(C, tmpFogColor, l*fog.r);
};

technique TransformTexture 
{
	pass P0 
	{
		AlphaBlendEnable	= true;
		VertexShader = compile vs_1_1 Transform();
		PixelShader  = compile ps_2_0 Texture();
	}
}