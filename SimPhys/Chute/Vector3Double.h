#pragma once
#include <cmath>
#include <string>
#include <sstream>

class Vector3Double
{
public:
	double X;
	double Y;
	double Z;

public:
	Vector3Double();
	~Vector3Double();
	Vector3Double(double x, double y, double z);

public:
	Vector3Double operator+(const Vector3Double& other)	{ return Vector3Double(X+other.X, Y+other.Y, Z+other.Z);}
	Vector3Double operator-(const Vector3Double& other) { return Vector3Double(X - other.X, Y - other.Y, Z - other.Z); }
	Vector3Double operator*(const double& a) { return Vector3Double(X *a, Y *a, Z *a); }
	Vector3Double operator/(const double& a) { return Vector3Double(X /a, Y /a, Z /a); }
	void operator+=(const Vector3Double& other) { X += other.X; Y += other.Y; Z += other.Z; }
	void operator-=(const Vector3Double& other) { X -= other.X; Y -= other.Y; Z -= other.Z; }

public:
	double DistanceSquared(Vector3Double a, Vector3Double b) {
		return pow(a.X - b.X, 2) + pow(a.Y - b.Y, 2) + pow(a.Y - b.Y, 2);
	}
	double Distance(Vector3Double a, Vector3Double b) {
		return sqrt (DistanceSquared(a, b));
	}
	double Magnitude() {
		return Distance(*this, Vector3Double(0, 0, 0));
	}
	Vector3Double Normalized() {
		double m = this->Magnitude();
		if (m == 0) return *this;
		else return *this / m;
	}
	static double Dot(Vector3Double a, Vector3Double b) {
		return a.X * b.X + a.Y * b.Y * a.Z * b.Z;
	}
	static Vector3Double Cross(Vector3Double a, Vector3Double b) {
		return Vector3Double(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
	}
	Vector3Double Rotated(Vector3Double axis, double angle) {
		Vector3Double r;
		Vector3Double ax = axis.Normalized();
		double c = cos(angle);
		double s = sin(angle);

		r.X = (ax.X * ax.X * (1 - c) + c) * X
			+ (ax.X * ax.Y * (1 - c) - ax.Z * s) * Y
			+ (ax.X * ax.Z * (1 - c) + ax.Y * s) * Z;
		r.Y = (ax.X * ax.Y * (1 - c) + ax.Z * s) * X
			+ (ax.Y * ax.Y * (1 - c) + c) * Y
			+ (ax.Y * ax.Z * (1 - c) - ax.X * s) * Z;
		r.Z = (ax.X * ax.Z * (1 - c) - ax.Y * s) * X
			+ (ax.Y * ax.Z * (1 - c) + ax.X * s) * Y
			+ (ax.Z * ax.Z * (1 - c) + c) * Z;

		return r;
	}
};

