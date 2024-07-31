
Start the LPSharp Powershell console using the executable.

```
LPSharp\PowershellConsole\bin\Release\net6.0\LPSharp.exe
```

You will see the Powershell prompt. It is preloaded with LPSharp cmdlets.

```
LPSharp> 
```

Read a model with `read-mps`. The model can be accessed using a key which is the
filename without extension or the name given using `-key`. Use the `-format
free` option to read files in MPS free format. The default fixed format is
faster.
```
read-mps 80bau38.mps
```

LPSharp uses a data model shown below for solver parameters. Parameters under
SolverParameters are for all solvers. Each solver has its section of parameters.

Contents of parameters.xml.
```
<?xml version="1.0"?>
<SolverParameters>
  <!-- These parameters are common for all solvers. -->
  <GenericParameters>
    <Param Name="TimeLimitInSeconds" Value="90" />
    <Param Name="EnableLogging" Value="true" />
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
    <Param Name="Recipe" Value="Either" />
    <Param Name="LogLevel" Value="3" />
  </ClpParameters>

</SolverParameters>
```

Create a solver with the `set-solver -create`. The argument is the solver type
(supported values are `GLOP` and `CLP`). The solver can be accessed using the
key defined by `-key`. 
```
set-solver -create CLP -key clp
```

Create a solver and initialize with solver parameters from file and semicolon
separated key-value pairs. `ParametersFile` is performed first, and
`ParametersText` is performed second. So it can be used to override parameters
read from file.
```
set-solver -create GLOP -key glop -ParametersFile parameters.xml -ParametersText "EnableLogging=false"
```

If the `-create` option is not used, the operations are performed on previously
created solver. The `-default` option means the solver is the default choice in
future `invoke-solver` commands if no key is given. The `-DefaultParameters`
initializes with the solver with previously parameters read by `read-parameters`.
```
set-solver -key glop -default
read-parameters parameters.xml
set-solver -key glop -DefaultParameters
```

View created solvers.
```
get-solvers
```

Output.
```
Key  Value
---  -----
glop GLOP_LINEAR_PROGRAMMING solver Key=glop TimeLimitInSeconds=90 EnableLogging=True SolveWithParameters=True LPAlgorithm=Primal SolverSpecificParameters=.
clp  CLP solver Key=clp TimeLimitInSeconds=90 EnableLogging=True Recipe=Either LogLevel=3
```

View loaded models.
```
get-models
```

Output.
```
Key    Value
---    -----
25fv47 Name=25fv47 Obj=R0000 A=(822, 727) Elements=10400 RHS=(1, 287) RowTypes=822 Lower=(0, 0) Upper=(0, 0)
80bau38 Name=80bau38 Obj=HOLLY A=(2263, 8061) Elements=21002 RHS=(1, 346) RowTypes=2263 Lower=(1, 3555) Upper=(1, 3555.
```

Load the model into the solver and solve it. The first argument is the model
key. You can select a non-default solver using `-key`. 
```
invoke-solver -key glop 80bau38
```

Output.
```
Solver glop solving model 80bau38...
  ColIndex   ColSolution
         0      531.1245
         1      286.8013
         2      544.7416
         3      895.5393
         4      1810.744
         5       1641.95
         6      1075.996
         7      358.6983
         8      345.8736
         9      768.4426
Solver glop solved model 80bau38 result=Optimal
Model                                  80bau38
Solver                                   glop
SolveTimeMs                                258
Objective                    987224.1924090903
Iterations                                4593
LoadTimeMs                                  48
ResultStatus                           OPTIMAL
```

Get the execution results. The execution key is a combination of the model and
solver keys. You can see more details by saving the output of `get-results` into
a variable. Each execution result is a dictionary of string keys and object
values.
```
get-results
```

Output.
```
Key             Value
---             -----
rmine15_lp_glop {[Model, rmine15_lp], [Solver, glop], [SolveTimeMs, 846300], [Objective, -5042.482962975563].}
```
