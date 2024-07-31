
Start the LPSharp Powershell console using the executable.

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
(supported values are `GLOP` and `CLP`). The solver can be accessed using the
key defined by `-key`. The `-default` option means this will be the solver used
in future `invoke-solver` commands if no key is given.
```
LPSharp> set-solver -create CLP -key clp
LPSharp> set-solver -create GLOP -key glop -default
```

LPSharp uses a data model shown below for solver parameters. Parameters under
SolverParameters are for all solvers. Each solver has its section of parameters.

```
Contents of parameters.xml:

<?xml version="1.0"?>
<SolverParameters>
  <!-- These parameters are common for all solvers. -->
  <GenericParameters>
    <Param Name="TimeLimitInSeconds" Value="90" />
    <Param Name="EnableOutput" Value="true" />
  </GenericParameters>

  <!-- GLOP solver parameters. MP solver common properties like presolve,
  scaling, etc. are in the parameters section. LP solver specific parameters
  are a protocol buffer text. -->
  <GlopParameters>
    <Parameters>
      <Param Name="LPAlgorithm" Value="Primal" />
      <Param Name="SolveWithParameters" Value="true" />
    </Parameters>
    <SolverSpecificParameterText>
      use_dual_simplex: 1
      perturb_costs_in_dual_simplex: 1
      relative_cost_perturbation: 100
      relative_max_cost_perturbation: 1
    </SolverSpecificParameterText>
  </GlopParameters>

  <!-- CLP solver parameters. -->
  <ClpParameters>
    <Param Name="SolveType" Value="Dual" />
    <Param Name="LogLevel" Value="3" />
  </ClpParameters>

</SolverParameters>
```

Set solver parameters while creating the solver using the previously read solver parameters.
```
LPSharp> read-parameters parameters.xml
LPSharp> set-solver -create GLOP -key glop -DefaultParameters
```

Set solver parameters while creating the solver by passing the parameters file.
```
LPSharp> set-solver -create GLOP -key glop -ParametersFile parameters.xml
```

Set solver parameters while creating the solver by passing parameters as key-value pairs.
```
LPSharp> set-solver -create GLOP -key glop -ParametersText "EnableLogging=true"
```

View created solvers.
```
LPSharp> get-solvers

Key  Value
---  -----
glop GLOP_LINEAR_PROGRAMMING solver Key=glop TimeLimitInSeconds=0 EnableLogging=False SolveWithParameters=True LPAlgor.
clp  CLP solver Key=clp TimeLimitInSeconds=0 EnableLogging=True Recipe=PrimalIdiot LogLevel=3
```

View loaded models.
```
LPSharp> get-models

Key    Value
---    -----
25fv47 Name=25fv47 Obj=R0000 A=(822, 727) Elements=10400 RHS=(1, 287) RowTypes=822 Lower=(0, 0) Upper=(0, 0)
80bau38 Name=80bau38 Obj=HOLLY A=(2263, 8061) Elements=21002 RHS=(1, 346) RowTypes=2263 Lower=(1, 3555) Upper=(1, 3555.
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
