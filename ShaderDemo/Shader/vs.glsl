#version 430
in vec4 vertex_color;
in vec3 vertex_position; 

uniform mat4 mvp_matrix;

out vec4 color;

void main(void)
{	
    gl_Position = mvp_matrix * vec4(vertex_position, 1.0f);

	color = vertex_color;
}