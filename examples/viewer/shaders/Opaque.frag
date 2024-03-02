#version 450


layout(location = 0) out vec4 fragColor;
layout(location = 1) out vec4 fragNormal;
layout(location = 2) out vec4 fragPosition;
layout(location = 3) out vec4 fragPbr;

layout(location = 0) in vec3 inPos;
layout(location = 1) in vec3 inNormal;
layout(location = 2) in vec2 inUV;
layout(location = 3) in mat3 inTBN;

layout( push_constant ) uniform constants
{
	uint transformIdx;
	uint materialIdx;
} PushConstants;

struct Material {
	vec4 albedo;
	vec4 bsdf;

	int colorTexture;
	float colorTextureBlend;

	int normalTexture;
	float normalTextureBlend;

	int pbrTexture;
	float pbrTextureBlend;
};


layout(set = 2, binding = 0) uniform Materials {
	Material materials[128];
} materials;

layout(set = 2, binding = 1) uniform sampler2DArray colorTextures;
layout(set = 2, binding = 2) uniform sampler2DArray normalTextures;
layout(set = 2, binding = 3) uniform sampler2DArray pbrTextures;


void main() {
	Material material = materials.materials[PushConstants.materialIdx];
	vec3 color = texture(colorTextures, vec3(inUV, material.colorTexture)).rgb;
	vec3 normal = texture(normalTextures, vec3(inUV, material.normalTexture)).rgb;
	normal = normalize(normal * 2.0 - 1.0);
	vec2 pbr = texture(pbrTextures, vec3(inUV, material.pbrTexture)).bg;

	fragColor = vec4(mix(material.albedo.rgb, color, material.colorTextureBlend), 1.0);
	fragNormal = vec4(mix(inNormal, normalize(inTBN * normalize(normal)), 1.0), 1.0);
	fragPosition = vec4(inPos, 1.0);
	fragPbr = vec4(mix(material.bsdf.xy, pbr, 1.0), 0.0, 1.0);
}
