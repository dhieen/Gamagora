#pragma once
#include <SDL.h>
#include <iostream>
#include <vector>

using namespace std;

class LinePlotWindow
{
private:
	SDL_Window *sdlw;
	SDL_Renderer *ren;
	char* name;
	int width;
	int height;

public:
	double minX;
	double maxX;
	double minY;
	double maxY;

public:
	LinePlotWindow();
	LinePlotWindow(int w, int h);
	~LinePlotWindow();

public:
	void StartPlot();
	void EndPlot();

	void SetPlotColor(int R, int G, int B);
	void PlotLine(double x1, double y1, double x2, double y2);
};

