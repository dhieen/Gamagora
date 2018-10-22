#include "pch.h"
#include <iostream>

#include "Vector3Double.h"

const double gravity = 9.806;
const double mass = 90000;
const double drag = 1;
const double delta_t = 0.1;

Vector3Double Acceleration(double t)
{
	return Vector3Double(0, 0, -gravity);
}

Vector3Double DeltaVelocity(double t, double delta_t, Vector3Double a)
{
	return a * delta_t;
}

Vector3Double DeltaPosition(double t, double delta_t, Vector3Double v)
{
	return v * delta_t;
}

int main()
{
	double g = 9.806;
	double m = 90000;

	Vector3Double pos = Vector3Double(0, 0, 1000);
	Vector3Double vel;
	Vector3Double acc;

	for (double t = 0; t < 100; t+=delta_t)
	{
		acc = Acceleration(t);
		vel += DeltaVelocity(t, delta_t, acc);
		pos += DeltaPosition(t, delta_t, vel);

		if (pos.Z <= 0) break;

		std::cout << pos.Z << std::endl;
	}
}