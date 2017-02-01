#version 430
in vec4 frag_color;
in vec2 frag_tex_coord;
in vec4 gl_FragCoord;
flat in int frag_pick_index;

out vec4 color;

layout (std430, binding = 0) buffer mouse_color { int pick_index; float depth_value; };

uniform int mouse_x;
uniform int mouse_y;
uniform sampler2D main_texture;

void main(void)
{		
	if (int(gl_FragCoord.x) == mouse_x && int(gl_FragCoord.y) == mouse_y) {
		if (gl_FragCoord.z < depth_value) 
		{
			pick_index = frag_pick_index;
			depth_value = gl_FragCoord.z;
		}
	}

	if (int(gl_FragCoord.x) == mouse_x || int(gl_FragCoord.y) == mouse_y) {
		color = vec4(0.0f, 1,0,1.0f);
	} else {
		color = frag_color;
	}

	// color = texture(main_texture, frag_tex_coord);
}