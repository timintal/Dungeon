#ifndef CUSTOM_LIGHT_INCLUDED
#define CUSTOM_LIGHT_INCLUDED

struct Light {
	float3 color;
	float3 direction;
};

int GetDirectionalLightCount () {
	return _DirectionalLightCount;
}

Light GetDirectionalLight (int index) {
	Light light;
	light.color = _DirectionalLightColors[index].rgb;
	light.direction = _DirectionalLightDirections[index].xyz;
	return light;
}

#endif
