#version 450

in vec3 position;
in vec3 color;
out vec3 vColor;

uniform mat4 transform;

void main()
{
	vec3 axis = vec3(0.0,1.0,1.0);
    gl_Position = transform * vec4(position, 1.0);

	vColor = color;
}