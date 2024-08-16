# Planet-Sim-9000
This is a toy game built in Godot to experiment with optimizations to n-body simulations.

`Logic/planets.cs` contains the n-body math and assigns a force vector to each object based on the gravity acting on it.

`Logic/Moon.cs` is the actual Node2D object representing a body in space. Each Moon double integrates the force acting on it 
each time a physics frame is calculated to obtain a new position.