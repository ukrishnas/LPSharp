In the GLOP examples folder, there is a standalone GLOP that is only C++ code,
and does not use SWIG. After build, it can be found in `<build
folder>/RELEASE/bin/glop_solve.exe`.

See full help information.

```
glop_solve.exe -helpfull
```

Load and solve an MPS file with default parameters.

```
glop_solve.exe -mpsfile <MPS file>
```

Load and solve with a 30 second solve time limit and dual simplex algorithm.

```
glop_solve.exe -mpsfile <MPS file> -timelimit 30 -lpalgorithm dual
```

Turn on verbose logging and log to a file in the current directory (default
is the system log directory). Turn off logging to standard error which is on by default.

```
glop_solve.exe --mpsfile <MPS file> --nologtostderr -v=1 -log_dir=.
```

Set solver-specific parameters in command line.

```
glop_solve.exe -mpsfile <MPS file> --glop_params="perturb_costs_in_dual_simplex: 1 optimization_rule: 2"
```

Set solver-specific parameters that use non-default cost perturbation settings.
This is useful when solving some WANLP models in dual simplex. Due to the
combinatorial nature of the problem, the cost function and constraint
coefficients are 1. The lack of natural randomness sometimes causes a pure cycle.
The parameters perturb cost by $rand(1.0, 2.0) \times (cost
\times 100 + maxcost).$ These values are much larger than defaults in GLOP. A downside
of perturbation is it increases the number of non-zeros and hence matrix operations like
factorization take longer. The values were chosen as the minimum to prevent cycles and
have least degradation in solve times for models that do not need them.
```
glop_solve.exe ... -timelimit 30 -params_file dual_params.txt
```

Dual_params.txt.
```
perturb_costs_in_dual_simplex: 1
relative_cost_perturbation: 100
relative_max_cost_perturbation: 1
```