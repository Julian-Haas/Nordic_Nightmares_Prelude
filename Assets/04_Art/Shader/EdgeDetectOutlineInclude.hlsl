#ifndef SOBELOUTLINES_INCLUDED
#define SOBELOUTLINES_INCLUDED

//These are points to sample relative to the starting point
static float2 sobelSamplePoints[9] = {
	float2(-1,1), float2(0,1), float2(1,1),
	float2(-1,0), float2(0,0), float2(1,1),
	float2(-1,-1), float2(0,-1), float2(1,-1),
};

//weights for the x component
static float sobelXMatrix[9] = {
	1, 0, -1,
	2, 0, -2,
	1, 0, -1
};

//weights for the y component
static float sobelYMatrix[9] = {
	1, 2, 1,
	0, 0, 0,
	-1, -2, -1
};

void DepthSobel_float(float2 UV, float Thickness, out float Out) {
	float2 sobel = 0;
	//we can unroll this loop to make it more efficient
	//the compiler is smart enough to remove the i=4 iteration, which is always zero
	[unroll] for (int i = 0; i < 9; i++) {
		float depth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV + sobelSamplePoints[i] * Thickness);
		sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
	}

	//get final sobel value
	Out = length(sobel);
}

#endif