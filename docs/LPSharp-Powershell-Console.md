__LPSharp Powershell Console__ is an interactive C# interface to LP solvers. You
can interact with LPSharp using Powershell cmdlets. The cmdlets are written in
C# and invoke LP solvers, which are typically written in C++, through platform
invoke calls. Powershell gives you the freedom to use the functionality of
LPSharp with a scripting language.

LPSharp interfaces with LP solver libraries at a procedural level. It uses
solver native APIs to setup the model, be they methods to add rows, columns,
variables, constraints, or objectives. It makes these API calls using C#, and
translates the call to the language the solver is written in using SWIG. Using
solver native APIs serves as a proof-point that a user with an optimization
problem written in C# can interface with the LP solver.

Solver models are read using Mathematical Programming System (MPS) files in
fixed MPS format. You can use models from your existing problem or benchmark
suites. For each solver, LPSharp sets up the model using solver native APIs. It
should be noted that LPSharp does not use the solver's MPS reader to setup the
model.

New solvers can be added to LPSharp by importing the solver libraries as nuget
packages, and writing cmdlets that setup the model, solve, and extract the
result. Note that these steps require integrating the solver with C# code, and
developing with the solver native API. It may be a laborious task, but once
complete, it proves that the same steps can be used to integrate the solver with
other C# programs.

The following LP solvers are supported:

- CLP: Computation Infrastructure for Operations Research (COIN-OR) LP solver.
- GLOP: Google Operations Research Tools (OR-Tools) LP solver.
