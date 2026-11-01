shader_type spatial;
//render_mode cull_disabled;

uniform vec2 oscillation_magnitude = vec2(0.05, 0.00);
uniform float oscillation_frequency = 50;
uniform vec2 scale = vec2(1.0);
uniform sampler2D albedo;
uniform vec4 color : source_color;
uniform float resolution = 128;
uniform float static_strength = 0.1;
float rand(vec2 co) {
    return fract(sin(TIME + dot(co.xy, vec2(12.9898, 78.233))) * 43758.5453);
}
void fragment() {
	vec2 uv = UV * scale - oscillation_magnitude * sin(TIME*oscillation_frequency);
	float noise = rand(floor(UV * resolution) / resolution);
	vec4 texas = texture(albedo, uv);
	ALBEDO = texas.rgb * color.rgb + noise * static_strength;
}