#pragma once

#include <cmath>
#include <iostream>
#include <string>
#include <random>
#include <vector>


class myMath 
{
	int iterations = 0; //Number of iterations performed in the root finding methods
	float tol = .001;  //Error Tolerance allowed for the root finding methods
	float root; //Will hold the value of the most recently approximated root
	
	int N = 10; // Number of Intervals to use for the integration methods

	float dx = .001; // The h used to approximate the derivative of f
	float(*f) (float); // A function pointer to the function used in both root finding and integration methods

	float dt = .1; //The step size used in your ODE solver. 
	float(*ydot) (float, float); // A function pointer to the equation used in the ODE solver

	std::vector<float> c; //The coefficients to the curve of best fit.
public:

//Getters and Setters
	//returns the iteration count of the most recent root finding method ran
	int getIterCount();

	//Sets the error Tolerance to the value e, returns false if e is not a valid tolerance
	bool setErrorTolerance(float e);

	//returns the value of the most recently approximated root
	float getRoot();

	//if n is an appropriate number of boxes then it sets N
	bool setN(int n);

	//if h is an appropriate h for derivative approximation then we set dx
	bool setdx(float h);

	//Set the function pointer
	void setf(float(*func)(float));

	//If h is an appropriate step size for an ODE approximation then set dt
	bool setdt(float h);

	//Set the function for the ODE
	void setydot(float(*func)(float, float));

	std::vector<float> getEstimatedCoefficients();

//Root Finding Methods
	//Numerical approximation to the derivative of f
	float fp(float x);
	
	//The numerical Bisection Method for solving an equation
	bool bisectionMethod(float a, float b);

	//Newton's numerical root finding method
	bool newtonsMethod(float x0);

	// Numerical Secant method for finding roots.
	bool secantMethod(float x0, float x1);

//Integration Methods
	//Integration with Rieman sums
	float riemanSum(float a, float b);

	//Numerical integration with the trapezoid rule
	float trapSum(float a, float b);

	//Uses MonteCarlo to integrate the function f between a and b
	float monteCarloIntegration(float a, float b);

//Line approximations

	void findBestFitCurve(int degree, std::vector<float> x, std::vector<float> y);

	void evaluateATA(std::vector<std::vector<float>>& A, std::vector<std::vector<float>>& ATA);

	void evaluateATY(std::vector<std::vector<float>>& A, std::vector<float> y, std::vector<float>& b);

	void guassianElimination(std::vector<std::vector<float>>& A, std::vector<float>& b);

//ODE approximations
	float ODEMethod(float y0, float t0, float tn);
};

