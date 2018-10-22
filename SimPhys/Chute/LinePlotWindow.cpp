#include "pch.h"
#include "LinePlotWindow.h"


LinePlotWindow::LinePlotWindow()
{
	LinePlotWindow(800, 600);
}

LinePlotWindow::LinePlotWindow(int w, int h) : width(w), height(h)
{
	sdlw = SDL_CreateWindow(
		"Blabla",                  // window title
		SDL_WINDOWPOS_UNDEFINED,           // initial x position
		SDL_WINDOWPOS_UNDEFINED,           // initial y position
		w,                               // width, in pixels
		h,                               // height, in pixels
		SDL_WINDOW_OPENGL                  // flags - see below
	);

	// Check that the window was successfully created
	if (sdlw == NULL) {
		// In the case that the window could not be made...
		printf("Could not create window: %s\n", SDL_GetError());
		return;
	}

	ren = SDL_CreateRenderer(sdlw, -1, SDL_RENDERER_ACCELERATED | SDL_RENDERER_PRESENTVSYNC);
	if (ren == nullptr) {
		SDL_DestroyWindow(sdlw);
		std::cout << "SDL_CreateRenderer Error: " << SDL_GetError() << std::endl;
		SDL_Quit();
		return;
	}
}


LinePlotWindow::~LinePlotWindow()
{
	// Close and destroy the window
	SDL_DestroyWindow(sdlw);
}

void LinePlotWindow::StartPlot()
{
	SetPlotColor(0, 0, 0);
	SDL_RenderClear(ren);
}

void LinePlotWindow::EndPlot()
{
	SetPlotColor(0, 0, 0);
	SDL_RenderPresent(ren);
}

void LinePlotWindow::SetPlotColor(int R, int G, int B)
{
	SDL_SetRenderDrawColor(ren, R, G, B, SDL_ALPHA_OPAQUE);
}

void LinePlotWindow::PlotLine(double x1, double y1, double x2, double y2)
{
	double _x1 = (x1 - minX) / (maxX - minX);
	double _x2 = (x2 - minX) / (maxX - minX);
	double _y1 = (y1 - minY) / (maxY - minY);
	double _y2 = (y2 - minY) / (maxY - minY);

	SDL_RenderDrawLine(ren, _x1 * (double)width, (1 - _y1) * (double)height, _x2 * (double)width, (1 - _y2) * (double)height);
}
