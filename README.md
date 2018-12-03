# Scientific-Computing
Compilation of projects from Introduction to Scientific Computing (FSU course ISC3313)

# makefile
Makefile was made to run with the prebuilt driver.cpp file. This may be modified and included in this repo later

# Derivative member function float fp(float x)
Function fp is currently being used to represent the centeral finite difference formula. Newton's Method function can be modified to work with the actual derivative instead of approximating it. I'll upload that version later

# Monte Carlo Integration 
This is being fixed and will come later. Everything else works and should return the correct values

# Driver file
If a custom driver file is made, the driver will need to set an equation (such as cos(x) - x + 3 or anything else) to the class's ydot and f function pointers.

# Finding the curve of best fit
1000 coordinate pairs are provided in data3x.txt and can be passed into the program to calculate the curve that best fits the data points.

If vector B needs to be a 2D vector of size nx1, change it and arguments from type vector<float> to vector<vector<float>> and update gaussianElimination function and other functions that include B (ex. B[i][0]). Probably not gonna need to but it's an option
