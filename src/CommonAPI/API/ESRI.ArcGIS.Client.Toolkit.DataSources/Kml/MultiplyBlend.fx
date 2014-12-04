sampler2D input : register(s0);
float4 multiplyColor : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
  float4 color = tex2D(input, uv);
  float4 result = color * multiplyColor;
  result.rgb *= multiplyColor.a; // WPF uses premultiplied values
  return result;
}