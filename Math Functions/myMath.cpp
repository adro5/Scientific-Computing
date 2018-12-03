#include <iostream>
#include <cmath>
#include <math.h>
#include <cctype>
#include <stdlib.h>
#include "myMath.h"
#include <fstream>
#include <vector>
#include <algorithm>
using namespace std;

/*
	Iteration Getters and Setters
*/
int myMath::getIterCount() {
	return iterations;
}
void myMath::setydot(float (*func)(float, float)) {
	ydot = func;	
}
void myMath::setf(float (*func)(float)) {
	f = func;
}
bool myMath::setErrorTolerance(float e) {
	if (e > 0) {
		tol = e;
		return true;
	}
	return false;
}
float myMath::getRoot() {
	return root;
}
bool myMath::setN(int n) {
	if (n > 0) {
		N = n;
		return true;
	}
	return false;
}
bool myMath::setdx(float h) {
	if (h != 0) {
		dx = h;
		return true;
	}
	return false;
}
bool myMath::setdt(float h) {
	if (h > 0) {
		dt = h;
		return true;
	}
	return false;
}
vector<float> myMath::getEstimatedCoefficients() {
	return c;
}

/* Root FInding Methods */

float myMath::fp(float x)
{
	return (f(x + dx) - f(x - dx))/(2*dx);
}

//Calculates the root of a function using the 
//well known bisection method.
bool myMath::bisectionMethod(float a, float b)
{
	if ((f(a) * f(b)) < 0) {
		iterations = 0;
		float aCopy = a;
		float bCopy = b;
		float c = 0;
		
		while (bCopy - aCopy > tol) {
			c = (aCopy + bCopy)/2.0;
			iterations++;
			if ((f(c) * f(b)) < 0) {
				aCopy = c;
			}
			else if ((f(c) * f(b)) >= 0) {
				bCopy = c;
			}
			if(f(c) == 0)
				root = c;
		}
		root = c;
		return true;
	}
	return false;
}

bool myMath::newtonsMethod(float x0) {

	if (fp(x0) != 0) {
		float x0c = x0; // Copy x0
		dx = tol; // Seeding dx to get a small error and interval
		iterations = 0;
		if (fp(x0c) != 0) {
			float c = f(x0c)/fp(x0c);
			dx = (2 * x0c) - c; // Once we get the initial c, we can get the actual h for the rest of the calculations
			while (fabs(c) >= tol && dx > 0)
			{
				c = f(x0c)/fp(x0c);
				dx = (2 * x0c) - c;
				x0c -= c;
				iterations++;
			}
		}
		root = x0c;
		return true;
	}
	return false;
}

bool myMath::secantMethod(float x0, float x1){

	if ((f(x1) - f(x0)) != 0) {
		iterations = 0;
		float x0c = x0; 
		float x1c = x1; 
		float x01, c, m;
		
		do {
			if (f(x1c) - f(x0c) != 0)
				x01 = (x0c * f(x1c) - x1c * f(x0c)) / (f(x1c) - f(x0c));
			else
				break;
			
			c = f(x0c) * f(x01);
			
			x0c = x1c;
			x1c = x01;
			
			if (c == 0)
				break;
			
			if (f(x1c) - f(x0c) != 0)
				m = (x0c * f(x1c) - x1c * f(x0c)) / (f(x1c) - f(x0c));
			else 
				break;
			iterations++;
			
		}while (fabs(m - x01) >= tol);
		
		root = x01;	
		return true;
	}
	return false;
}

// Numerical integration with the trapezoidal rule
float myMath::trapSum(float a, float b) {
	iterations = 0;
	float sum = f(a) + f(b);
    dx = (b - a) / static_cast<float>(N);
	
	for (float i = a + dx; i < b; i+=dx) {
		sum += 2 * f(i);
		iterations++;
	}
	
	return (sum * dx) / 2;
}

// Integration with Rieman sums
float myMath::riemanSum(float a, float b) {
	float sum = 0;
	iterations = 0;
    dx = (b - a) / static_cast<float>(N);

	for (float i = a + dx; i <= b; i += dx) {
		sum += f(i) * dx;	
		iterations++;
	}
	
	return sum;
}

// Monte Carlo Integration
float myMath::monteCarloIntegration(float a, float b) {
	// Work in progress
}

/*Line approximations*/

vector<vector<float>> evaluateAT(vector<vector<float>> A) {
    vector<vector<float>> AT(A[0].size());
    
	// Transposed A matrix
	for (int i = 0; i < A[0].size(); ++i) {
		for (int j = 0; j < A.size(); ++j) {
			AT[i].push_back(A[j][i]);
		}
	}

    return AT;
}

void myMath::evaluateATA(std::vector<std::vector<float>>& A, std::vector<std::vector<float>>& ATA) {
	vector<vector<float>> AT = evaluateAT(A);
    
	int degree = AT.size() - 1;

	// Initializeing ATA
	for (int i = 0; i <= degree; ++i) {
		for (int j = 0; j <= degree; ++j) {
			ATA[i].push_back(0);
		}
	}
	// ATA = A trans * A
	for (int i = 0; i <= degree; ++i) {
		for (int j = 0; j <= degree; ++j) {
			for (int k = 0; k < A.size(); ++k) {
				ATA[i][j] += AT[i][k] * A[k][j];
			}
		}
	}
}

void myMath::evaluateATY(std::vector<std::vector<float>>& A, std::vector<float> y, std::vector<float>& b) {
	vector<vector<float>> AT = evaluateAT(A);
	vector<float> ATB(AT.size());
	int degree = AT.size() - 1;

	for (int i = 0; i <= degree; ++i) {
		for (int k = 0; k < y.size(); ++k) {
			ATB[i] += AT[i][k] * y[k];
		}
	}
    
	b = ATB;
	
}

void myMath::guassianElimination(std::vector<std::vector<float>>& A, std::vector<float>& b) {

	int n = A.size();

	for (int i = 0; i<n; i++) {
		// Search for maximum in this column
		double maxEl = abs(A[i][i]);
		int maxRow = i;
		for (int k = i + 1; k<n; k++) {
			if (abs(A[k][i]) > maxEl) {
				maxEl = abs(A[k][i]);
				maxRow = k;
			}
		}

		// Swap maximum row with current row (column by column)
		for (int k = i; k<n; k++) {
			float tmp = A[maxRow][k];
			A[maxRow][k] = A[i][k];
			A[i][k] = tmp;
		}
		float tmp = b[maxRow];
		b[maxRow] = b[i];
		b[i] = tmp;

		// Make all rows below this one 0 in current column
		for (int k = i + 1; k<n; k++) {
			double c = -A[k][i] / A[i][i];
			for (int j = i; j<n; j++) 
			{
				if (i == j) {
					A[k][j] = 0;
				}
				else {
					A[k][j] += c * A[i][j];
				}
			}
			b[k] += c*b[i];
		}
	}

	// Solve equation Ax=b for an upper triangular matrix A
	for (int i = n - 1; i >= 0; i--) {
		b[i] = b[i] / A[i][i];
		for (int k = i - 1; k >= 0; k--) {
			b[k] -= A[k][i] * b[i];
		}
	}

}

void myMath::findBestFitCurve(int degree, vector<float> x, vector<float> y) {
// 2D Vector
	vector< vector<float> > A(x.size());
	vector<float> B(x.size());
	vector< vector<float> > ATA(degree + 1);
	for (int i = 0; i < x.size(); i++) {
		// Adding data from txt file to vector matrix
		for (int j = 0; j <= degree; j++)
			A[i].push_back(pow(x[i], j));
		B.push_back(y[i]);
	}
    
	evaluateATA(A, ATA);
	evaluateATY(A, y, B);
	guassianElimination(ATA, B);
	c = B;
}


// ODE approximations
float myMath::ODEMethod(float y0, float t0, float tn) {
	float yn = y0;
	
	for (float i = t0; i < tn; i += dt) {
		yn += (dt * ydot(yn, t0));
	}

	return yn;
}