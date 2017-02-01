#version 430

in vec4 vertex_color;
in vec3 vertex_position; 
in vec3 vertex_normal;
in vec2 vertex_tex_coord;
in int vertex_pick_index;

uniform mat4 mvp_matrix;

out vec4 frag_color;
out vec2 frag_tex_coord;
flat out int frag_pick_index;

void main(void)
{
	frag_pick_index = vertex_pick_index;

	vec3 light = normalize(vec3(0.25f, -0.75f, -1));
	float alpha = acos(dot(light, vertex_normal));
	
	frag_color.x = vertex_color.x * sin(alpha);
	frag_color.y = vertex_color.y * sin(alpha);
	frag_color.z = vertex_color.z * sin(alpha);

	gl_Position = mvp_matrix * vec4(vertex_position, 1.0f);

	frag_tex_coord = vertex_tex_coord;
}