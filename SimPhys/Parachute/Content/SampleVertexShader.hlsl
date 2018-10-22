// Une m�moire tampon constante qui stocke les trois matrices colonne-major de base pour composer la g�om�trie.
cbuffer ModelViewProjectionConstantBuffer : register(b0)
{
	matrix model;
	matrix view;
	matrix projection;
};

// Donn�es par sommet utilis�es comme entr�es dans le nuanceur de sommets.
struct VertexShaderInput
{
	float3 pos : POSITION;
	float3 color : COLOR0;
};

// Donn�es de couleur par pixel transmises via le nuanceur de pixels.
struct PixelShaderInput
{
	float4 pos : SV_POSITION;
	float3 color : COLOR0;
};

// Nuanceur simple pour le traitement du sommet sur le GPU.
PixelShaderInput main(VertexShaderInput input)
{
	PixelShaderInput output;
	float4 pos = float4(input.pos, 1.0f);

	// Transformer la position vertex en un espace projet�.
	pos = mul(pos, model);
	pos = mul(pos, view);
	pos = mul(pos, projection);
	output.pos = pos;

	// Transmettre la couleur sans aucune modification.
	output.color = input.color;

	return output;
}
