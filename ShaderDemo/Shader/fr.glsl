#version 430
in vec4 frag_color;
in vec4 frag_pick_color;
in vec4 gl_FragCoord;

out vec4 color;

layout (std430, binding = 0) buffer mouse_color { vec4 mouse_color_value; };

uniform int pick_index;
uniform int mouse_x;
uniform int mouse_y;

void main(void)
{		
	if (int(gl_FragCoord.x) == mouse_x && int(gl_FragCoord.y) == mouse_y) {
		if (gl_FragCoord.z < mouse_color_value.w) 
		{
			mouse_color_value = vec4(frag_pick_color.xyz, gl_FragCoord.z);
		}
	}

	if (int(gl_FragCoord.x) == mouse_x || int(gl_FragCoord.y) == mouse_y) {
		color = vec4(0.0f, 1,0,1.0f);
	} else {
		color = frag_color;
	}
}