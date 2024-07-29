This page documents the benchmark results of the LP solvers. The solvers have
been tested against the following benchmarks.

- __WANLP__ is a benchmark of 7 models from Microsoft Azure wide area network
  traffic engineering scheduler. They solve optimization problems of approximate
  max-min fairness, diverse path allocation, minimizing cost and maximum
  utilization.

- __Netlib__ has a collection small to medium sized MPS models that can be found
  [here](https://www.cuter.rl.ac.uk/Problems/netlib.shtml). We collected results
  with the largest models.

- __Mittelmann__. Hans D. Mittelmann has a good collection of
  LP models with execution logs from multiple solvers including CLP and GLOP.
  Please start from this [readme](http://plato.asu.edu/ftp/lpsimp.html) to
  download models. We reproduced his results for 9 out of 40 models.

## Results summary

The table below shows comparative solver performance. Each cell shows speedup when
compared to the reference solver. Speedups of different models are averaged
using geometric mean.

|Benchmark|Reference|GLOP Primal|GLOP Dual|CLP Primal|CLP Dual|CLP Either|
|--|--|--|--|--|--|--|
|WANLP|MSF Primal|5|11|12|19|8|
|Netlib|GLOP Primal|1|0.8|1.1|2.1|3.2|
|Mittelmann|GLOP Primal|1||||1.9|

Microsoft Solver Foundation (MSF) results are from Network Designer using
Invoke-MSFSolve, because LPSharp does not support MSF.

CLP results are using LPSharp Powershell console and CLP nuget packages from
this repository. The solver can also be invoked using `coinwrap.exe`, a
standalone executable of the Clp interface, and `Clp.exe`, a standalone
executable of the Clp solver. The results are the same when used with the same
settings.

GLOP results are using LPSharp Powershell console and GLOP nuget packages from
this repository. The locally built library (9.1.9490.22) matches the public
OR-Tools in nuget.org (version 9.1.9490). This confirms that we are able to
locally replicate the public build. `glop_solve.exe` is our C++ based standalone
executable linked with the C++ libraries and is another way to involve Glop
solver. It may be used to debug LPSharp or control the solver in ways not
possible with the dotnet interface.

See [LPSharp Powershell cmdlets guide](LPSharp-Powershell-Cmdlets-Guide) for
more information on invoking the solver and collecting results.

__Visualization__. You can plot the results using
[plot_lpbench.py](https://github.com/microsoft/LPSharp/blob/main/LPBench/plot_lpbench.py)
that is part of the source tree.

```
$ python plot_lpbench.py --help  # for usage
$ python plot_lpbench.py results_i7.csv --model_pattern wanlp --baseline Msf --measurements GlopPrimal GlopDualPerturb ClpEither ClpPrimalIdiot ClpDualCrash
$ python plot_lpbench.py results_i7.csv --model_pattern netlib --measurements GlopPrimal GlopDualPerturb ClpPrimalIdiot ClpDualCrash ClpEither
$ python plot_lpbench.py results_i7.csv --model_pattern mittelmann --measurements GlopPrimal ClpEither
```

[Results_i7.csv](https://github.com/microsoft/LPSharp/blob/main/LPBench/results_i7.csv)
are the results on Intel Core i7 7500U 2.7Ghz 4 logical processors 16GB. This is
a laptop processor and there is a variability in the results due to processor
clock speed changes, and possibly Windows scheduler.

## WANLP results

- MSF times are for primal simplex. Its dual simplex exceeded solve time limits.
- CLP dual with default settings uses all slacks starting basis that converges
  more slowly for the sonal-maxmin model. Using the crash starting basis method
  is faster by 40%. CLP primal automatically selects from three starting basis
  methods. Fixing it to Idiot starting basis gives a smaller speedup of 10%.
- CLP message handler has two log level controls. One of them is a facility
  based logger and it affects wall clock time. This has been turned off in
  CoinWrap.
- Although we forced the solvers to primal or dual simplex for this benchmark,
  CLP and GLOP can select either simplex based the model. GLOP does this by
  default, and CLP does this when the solve method is automatic.
- GLOP dual simplex could not solve the max-min models with default settings.
  Max-min models have high degeneracy - non-zero elements in the constraint
  matrix are 1, and the cost vector coefficients are 1. So many columns are the
  same. Pure cycling is possible in such models. Dual simplex needs cost
  perturbation to jiggle the values a bit and get it out of the cycle. The
  downside of is that it decreases sparsity in models like min-cost which do not
  need it, and operations like a bit longer. We have tuned the cost perturbation
  parameters. With these, GLOP dual is twice as fast as GLOP primal. The custom
  settings can be done in LPSharp Powershell console or `glopsolve`.


## Netlib results

These models execute in less than 30 seconds per model.

If using the `Clp.exe` or `coinwrap.exe` executables, it will complain `Unknown
image  at line 1 of file`. Please open the file and remove the first four lines
before the NAME record. The line starting with NAME should be the first line.
The `LPSharp` MPS reader can read this file without issue.

- CLP dual simplex uses crash starting basis heuristic and primal simplex uses
  idiot crash starting basis. GLOP dual simplex uses the modified perturbation
  cost settings.
- CLP either simplex is the fastest of the CLP techniques.
- GLOP dual simplex is 20% slower than its primal simplex. We tried with default
  and modified perturbation cost settings. The modified ones are better.
- Qap12 does not lend itself to dual simplex. We see this with qap15 in the
  Mittelmann benchmark too. Some problems are easier to solve with one
  technique. GLOP dual simplex could not solve qap12.

## Mittelmann results

The models in the ftp site are bz2 compressed and some need another
decompression with [emps](http://www.netlib.org/lp/data/emps.exe.gz). Please
decompress them before executing them with LPSharp. Please read the files into
LPSharp using free format and verify that there are no read errors. 
```
LPSharp> read-mps s250r10.mps -Format free
```

The problems are quite hard to solve and take minutes on my laptop. Please
compare your execution with the logs kept in this folder or the ftp site to
verify your own execution.

For GLOP, we enabled logging, and set solver-specific parameter
`log_iteration_period` to 500, to decrease frequency of log messages to about
the same rate as CLP. GLOP LP algorithm was set to primal.

CLP is run with `either` recipe. This option switches between primal and dual
based on an analysis of the model, accumulated errors, etc. CLP log level was
set to 3.

The starting basis heuristics of GLOP are Bixby, Triangular (default), and
Moros. The starting basis methods for CLP are all-slacks, crash, idiot, and
sprint (in primal only), and various combinations of them. If a heuristic
selects a better starting basis, fewer iterations are needed to reach optimal,
which makes the solve time better. 

Model specific results:

- __Cont1__: GLOP when solving cont1 spends nearly 12 minutes after the last
  iteration to clean up the results.
- __S250r10__ is very fast with GLOP and slow with CLP either. GLOP guesses the
  starting basis at -0.082 and needs 10,000 iterations to reach the objective of
  -0.17. CLP either starts at -0.55 and takes 84,000 iterations to reach the
  objective. Its primal simplex struggles with primal error.


