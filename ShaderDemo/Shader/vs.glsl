#version 430

in vec4 vertex_color;
in vec4 vertex_pick_color;
in vec3 vertex_position; 

uniform mat4 mvp_matrix;

out vec4 frag_color;
out vec4 frag_pick_color;

void main(void)
{	
    gl_Position = mvp_matrix * vec4(vertex_position, 1.0f);

	frag_color = vertex_color;
	frag_pick_color = vertex_pick_color;
}