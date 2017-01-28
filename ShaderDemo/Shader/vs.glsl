#version 430

in vec4 vertex_color;
in vec4 vertex_pick_color;
in vec3 vertex_position; 
in vec3 vertex_normal;

uniform vec3 look_at;
uniform mat4 mvp_matrix;

out vec4 frag_color;
out vec4 frag_pick_color;

void main(void)
{
    gl_Position = mvp_matrix * vec4(vertex_position, 1.0f);
	float alpha = dot(vec3(0, 0, -1), vertex_normal);
	
	frag_color = vertex_color;
	frag_color.x = vertex_color.x * (1 - sin(alpha));
	frag_color.y = vertex_color.y * (1 - sin(alpha));
	frag_color.z = vertex_color.z * (1 - sin(alpha));
	frag_pick_color = vertex_pick_color;
}