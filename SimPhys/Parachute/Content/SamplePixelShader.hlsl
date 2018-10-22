// Donn�es de couleur par pixel transmises via le nuanceur de pixels.
struct PixelShaderInput
{
	float4 pos : SV_POSITION;
	float3 color : COLOR0;
};

// Fonction de transmission directe pour les donn�es de couleur (interpol�).
float4 main(PixelShaderInput input) : SV_TARGET
{
	return float4(input.color, 1.0f);
}
