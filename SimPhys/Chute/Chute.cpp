#include "pch.h"
#include <iostream>
#include <stdio.h>

#include <iostream>
#include <SDL.h>

#include "Vector3Double.h"
#include "LinePlotWindow.h"

Vector3Double pos0(50, 800, 0);
Vector3Double vel0(10, 0, 0);
Vector3Double acc0;
double gravity = 9.806;
double mass = 90000;
double drag = 1;
double parachuteDrag = 200;
double delta_t = .5;

Vector3Double Acceleration(Vector3Double v)
{
	return (Vector3Double(0, -gravity, 0) * mass - v.Normalized() * pow(v.Magnitude(), 2) * drag) / mass;
}

Vector3Double DeltaVelocity(double delta_t, Vector3Double a)
{
	return a * delta_t;
}

Vector3Double DeltaPosition(double delta_t, Vector3Double v)
{
	return v * delta_t;
}

void MainLoop(SDL_Renderer *ren)
{
	Vector3Double pos = pos0;
	Vector3Double vel = vel0;
	Vector3Double acc = acc0;

	bool mainLoop = true;
	double t = 0;
	SDL_Event event;

	LinePlotWindow plot(1080, 720);
	plot.minX = 0;	plot.maxX = 1080;
	plot.minY = 0;	plot.maxY = 720;

	plot.StartPlot();

	while (mainLoop)
	{
		t += delta_t;
		if (t > 20)
		{
			t = 0;
			pos = pos0;
			vel = vel0;
			acc = acc0;

			plot.EndPlot();
			plot.StartPlot();
		}

		plot.SetPlotColor(255, 0, 0);
		Vector3Double prevPos = pos;
		acc += Acceleration(vel);
		vel += DeltaVelocity(delta_t, acc);
		pos += DeltaPosition(delta_t, vel);
		plot.PlotLine(prevPos.X, prevPos.Y, pos.X, pos.Y);

		SDL_WaitEventTimeout(&event, 40);

		switch (event.type)
		{
		case SDL_KEYUP:
			
			break;

		case SDL_QUIT:
			mainLoop = false;
			break;
		}
	}

	plot.EndPlot();

	// Clean up
	plot.~LinePlotWindow();
	SDL_Quit();
}

int main(int argc, char* argv[]) {

	SDL_Init(SDL_INIT_VIDEO);	

	MainLoop(NULL);

	return 0;
}