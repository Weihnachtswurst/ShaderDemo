#version 430
in vec4 frag_color;
in vec4 frag_pick_color;
in vec4 gl_FragCoord;

out vec4 color;

layout (std430, binding = 0) buffer mouse_color { int pick_index; float depth_value; };

uniform int mouse_x;
uniform int mouse_y;

void main(void)
{		
	if (int(gl_FragCoord.x) == mouse_x && int(gl_FragCoord.y) == mouse_y) {
		if (gl_FragCoord.z < depth_value) 
		{
			pick_index = 42;
			depth_value = gl_FragCoord.z;
		}
	}

	if (int(gl_FragCoord.x) == mouse_x || int(gl_FragCoord.y) == mouse_y) {
		color = vec4(0.0f, 1,0,1.0f);
	} else {
		color = frag_color;
	}
}