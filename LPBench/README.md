# LP Solver Benchmark Results

This page documents the benchmark results of the LP solvers. The solvers have
been tested against the following benchmarks.

- __WANLPv2__ is a benchmark of 29 models, 26 are performance-oriented and 3 are for
  checking completion with default zero tolerance settings. WANLPv2 benchmark
  models are in
  [sharepoint](https://microsoft.sharepoint.com/:f:/t/AzNet_WAN/Eta127_8eHhNsylPOEupNQwB1OwjjRGGPilJtk2cf0sT_Q?e=cYFuIm)
  and explanation of the models is in Section 6 of [this
  document](https://microsoft.sharepoint.com/:b:/t/AzNet_WAN/Ed82YFQIC5xBg5_2ya0Y_bgB5OTmeu9GGMhEyFH6D7AXFg?e=GkPifF).

- __Netlib__ has a collection small to medium sized MPS models that can be found
  [here](https://www.cuter.rl.ac.uk/Problems/netlib.shtml). We collected results
  with the largest models.

- __Plato__ is an ftp server where H. Mittelman has a good collection of LP
  models with execution logs from multiple solvers including CLP and GLOP.
  Please start from this [readme](http://plato.asu.edu/ftp/lpsimp.html) to
  download models. We reproduced his results using a few models.

__Results summary__. Each cell shows speedup when compared to the reference
solver. ClpBest is the best of different Clp settings. We have not been able to
try GLOP in different settings.

|Benchmark|Reference|GLOP Primal|GLOP Dual|CLP Primal|CLP Dual|CLP Either|
|--|--|--|--|--|--|--|
|WANLPv2|MSF Primal|2|7|11|15|
|Netlib|GLOP Primal|1|0.7|1|1.8|2.3|

MSF results are from Network Designer using Invoke-MSFSolve, because LPSharp
does not support MSF.

CLP results are using LPSharp and CLP nuget packages from this repository. The solver can
also be invoked using `coinwrap.exe`, a standalone executable of the Clp
interface, and `Clp.exe`, a standalone executable of the Clp solver. The results
are the same when used with the same settings.

GLOP results are using LPSharp and GLOP nuget packages from this repository. The
locally built library (version 9.1.90.1) matches the public OR-Tools in
nuget.org (version 9.1.9490). This confirms that we are able to locally
replicate the public build. `glop_solve.exe` is our C++ based standalone
executable linked with the C++ libraries and is another way to involve Glop
solver. It may be used to debug LPSharp or control the solver in ways not
possible with the dotnet interface.

[Results_i7.csv](results_i7.csv) are the results on Intel Core i7 7500U 2.7Ghz 4
logical processors 16GB. This is a laptop processor and there is a variability
in the results due to processor clock speed changes, and possibly Windows
scheduler.

__LPSharp cheatsheet__

LPSharp can be used to collect benchmark results. It can read MPS model files,
set up the model in the solver, and execute the solver. For Plato models, read
the model in free format. This is not required for WANLPv2 and Netlib models.
See [LPSharp readme](../LPSharp/Readme.md) for more help.
```
LPSharp> read-mps netlib\80bau38.mps              # if WANLPv2 or Netlib
LPSharp> read-mps plato\s250r10.mps -Format free  # if Plato
LPSharp> set-solver -create CLP -key clp
LPSharp> set-solver -create GLOP -key glop -default
LPSharp> read-parameters parameters.xml           # to read custom solver parameters
LPSharp> invoke-solver s250r10
```

__CoinWrap cheatsheet__

Example execution of Netlib and Plato models in different settings. The second
argument is the recipe and choices are: barrier, dualcrash, duals, either,
primals, primalidiot.

```
$ coinwrap.exe netlib\80bau38.mps either
$ coinwrap.exe netlib\80bau38.mps duals
$ coinwrap.exe netlib\80bau38.mps primals
```

__GlopSolve cheatsheet__

```
$ glop_solve.exe -helpfull
$ glop_solve.exe -mpsfile edge-pri0-maxmin0.mps -timelimit 30 -lpalgorithm dual
$ glop_solve.exe ... -timelimit 30 -params_file dual_params.txt
```

Dual_params.txt to use with WANLP models.
```
perturb_costs_in_dual_simplex: 1
relative_cost_perturbation: 100
relative_max_cost_perturbation: 1
```

__Plot_lpbench cheatsheet__

You can plot the results by executing:

```
$ python plot_lpbench.py --help  # for usage

$ python plot_lpbench.py results_i7.csv --model_pattern wanlp --baseline Msf --measurements ClpDualCrash ClpPrimalIdiot GlopDualPerturb GlopPrimal

$ python plot_lpbench.py results_i7.csv --model_pattern netlib --measurements GlopPrimal GlopDual GlopDualPerturb ClpEither ClpDualCrash ClpPrimal

$ python plot_lpbench.py results_i7.csv --model_pattern plato --measurements GlopPrimal ClpEither
```

## WANLPv2 results

- MSF times are for primal simplex. Its dual simplex exceeded solve time limits.
- Both CLP and GLOP solved all 29 models with default and modified settings.
- CLP dual with default settings uses all slacks starting basis that converges
  more slowly for the sonal-maxmin[0-5].mps models. Using the crash starting
  basis method is faster by 40%. CLP primal automatically selects from three
  starting basis methods. Fixing it to Idiot starting basis gives a smaller
  speedup of 10%.
- CLP message handler has two log level controls. One of them is a facility
  based logger and it affects wall clock time. This has been turned off in
  CoinWrap.
- Although we forced the solvers to primal or dual simplex for this benchmark,
  CLP and GLOP can select either simplex based the model. GLOP does this by
  default, and CLP does this when the solve method is automatic.
- GLOP dual simplex could not solve the max-min models like edge-pri0-maxmin0
  with default settings. Max-min models have high degeneracy - non-zero elements
  in the constraint matrix are 1, and the cost vector coefficients are 1. So
  many columns are the same. Pure cycling is possible in such models. Dual
  simplex needs cost perturbation to jiggle the values a bit and get it out of
  the cycle. The downside of is that it decreases sparsity in models like
  min-cost which do not need it, and operations like a bit longer. We have tuned
  the cost perturbation parameters. With these, GLOP dual is twice as fast as
  GLOP primal. The custom settings can be done in LPSharp or `glopsolve`.


## Netlib results

These models execute in less than 30 seconds per model.

If using the `Clp.exe` or `coinwrap.exe` executables, it will complain `Unknown
image  at line 1 of file`. Please open the file and remove the first four lines
before the NAME record. The line starting with NAME should be the first line.
The `LPSharp` MPS reader can read this file without issue.

- CLP is 2 times faster than GLOP for the executed Netlib models.
- CLP and GLOP primal simplex methods are the same speed.
- GLOP dual simplex is 20% slower than its primal simplex. We tried with default
  and modified perturbation cost settings. The modified ones are better.
  Generally, GLOP dual is slightly more fragile than its primal simplex.
- Qap12 does not lend itself to dual simplex. We see this with qap15 in the
  Plato benchmark too. Some problems are easier to solve with one technique.
  GLOP dual simplex could not solve qap12.

## Plato results

The models in the ftp site are bz2 compressed. Please decompress them before
executing them with LPSharp. Please read the files into LPSharp using free
format and verify that there are no read errors. The problems are quite hard to
solve and take minutes on my laptop. Please compare your execution with the logs
kept in this folder or the ftp site to verify your own execution.

CLP `either` option switches between primal and dual based on an analysis of the
model, accumulated errors, etc. With unknown problems like in this benchmark, it
is the best option. CLP log level was set to 3.

The starting basis is another determinant of performance. Notice how s250r10 is
fast with GLOP and CLP primal but slow with CLP dual and either. The starting
basis heuristics of GLOP are Bixby, Triangular (default), and Moros. The
starting basis methods for CLP are all-slacks, crash, idiot, and sprint (in
primal only), and various combinations of them. If a heuristic selects a better
starting basis, fewer iterations are needed to reach optimal, which makes the
solve time better.

