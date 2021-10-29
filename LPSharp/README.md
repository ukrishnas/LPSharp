# LPSharp

__LPSharp__ is an interactive C# interface to LP solvers. You can interact with
LPSharp using Powershell cmdlets. The cmdlets are written in C# and invoke LP
solvers, which are typically written in C++, through platform invoke calls.
Powershell gives you the freedom to use the functionality of LPSharp with a
scripting language.

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

LPSharp is meant to be a test bench for the following LP solvers:

- MSF: Microsoft Solver Foundation LP solver.
- GLOP: Google Operations Research Tools (OR-Tools) LP solver.
- CLP: Computation Infrastructure for Operations Research (COIN-OR) LP solver.


## LPSharp Cheatsheet

Start the powershell console using the executable.

```
$ PowershellConsole\bin\Release\net5.0\PowershellConsole.exe
LPSharp>
```

Read a model with `read-mps`. The model can be accessed using a key which is the
filename without extension or the name given using `-key`. Use the `-format
free` option to read files in MPS free format. The default fixed format is
faster.
```
LPSharp> read-mps 80bau38.mps
```

Create a solver with the `set-solver -create`. The argument is the solver type
(supported values are `GLOP` with future support for `CLP` and `MSF`). The
solver can be accessed using the key defined by `-key`. The `-default` option
means this will be the solver used in future `invoke-solver` commands if no key
is given.
```
LPSharp> set-solver -create GLOP -key glop -default
```

Read solver parameters. LPSharp uses a data model shown below for solver
parameters. Parameters under SolverParameters are for all solvers. These are the
LP algorithm (Default, Dual, Primal, Barrier), and time limit in seconds. Each
solver has its section of parameters. The parameters are loaded every time the
solver is invoked.
```
LPSharp> read-parameters parameters.xml

Contents of parameters.xml:

<?xml version="1.0"?>
<SolverParameters>
  <!-- These parameters are common for all solvers. -->
  <TimeLimitInSeconds>90</TimeLimitInSeconds>
  <LPAlgorithm>Default</LPAlgorithm>

  <GlopParameters>
    <Parameters>
      <Param Name="EnableOutput" Value="false" />
      <Param Name="SolveWithParameters" Value="true" />
    </Parameters>
    <SolverSpecificParameterText>
      use_dual_simplex: 1
      perturb_costs_in_dual_simplex: 1
      relative_cost_perturbation: 100
      relative_max_cost_perturbation: 1
    </SolverSpecificParameterText>
  </GlopParameters>

  <ClpParameters />
</SolverParameters>
```

Load the model into the solver and solve it. The first argument is the model
key. You can select a non-default solver using `-key`. 
```
LPSharp> invoke-solver s250r10
Loading model s250r10
Solving model s250r10...
Solved model s250r10 result=optimal
Model                                  s250r10
Solver                                    glop
SolveTimeMs                              12957
Objective                 -0.17267704190548064
Iterations                               11387
LoadTimeMs                                1192
ResultStatus                           OPTIMAL
```

Get the execution results. The execution key is a combination of the model and
solver keys. You can see more details by saving the output of `get-results` into
a variable. Each execution result is a dictionary of string keys and object
values.
```
LPSharp> get-results

Key             Value
---             -----
rmine15_lp_glop {[Model, rmine15_lp], [Solver, glop], [SolveTimeMs, 846300], [Objective, -5042.482962975563].}
```
