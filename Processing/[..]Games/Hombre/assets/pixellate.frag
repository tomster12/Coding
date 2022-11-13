
// https://gist.github.com/Leland-Kwong/40edb57c87c0755eb7f5ba6f65d9c484
// https://github.com/cukiakimani/shaders-yo/blob/master/Assets/Sprite%20Outline/Shaders/SpriteOutline.shader

#ifdef GL_ES
precision highp float;
precision highp int;
#endif

varying vec4 vertTexCoord;
uniform sampler2D texture;

uniform vec2 u_offset;
uniform vec2 u_pixels;

void main(void)
{
	vec2 p = vertTexCoord.st;

	float l_px = p.x - u_offset.x + 1.0;
	float l_py = p.y + u_offset.y + 1.0;
	float l_round_px = float(floor(l_px * u_pixels.x)) / u_pixels.x;
	float l_round_py = float(floor(l_py * u_pixels.y)) / u_pixels.y;
	float round_px = l_round_px + u_offset.x - 1.0;
	float round_py = l_round_py - u_offset.y - 1.0;
	vec4 col = texture2D(texture, vec2(round_px, round_py));
	gl_FragColor = col;

	// if (col.a < 0.01) discard;
	// gl_FragColor = vec4(col.r, col.g, col.b, 1.0);
}
