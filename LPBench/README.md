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

|Benchmark|Reference|GLOP Primal|GLOP Dual|CLP Primal|CLP Dual|CLP Either|CLP Best|
|--|--|--|--|--|--|--|--|
|WANLPv2|MSF Primal|3|5|7|10|
|Netlib|GLOP Primal|1|0.8|1|1.8|2.3|
|Plato|GLOP Primal|1||||1.2|1.7|

MSF results are from Network Designer using Invoke-MSFSolve, because LPSharp
does not support MSF.

CLP results are using `coinwrap.exe`, which is our wrapper around Clp solver
methods in `libClp` and `libCoinUtils` built from code this repository. It is
not yet using LPSharp because the support is under development. Clp build also
produces a standalone executable, and the results of CoinWrap and Clp standalone
executables are the same when used with default settings. Clp standalone exposes
a number of settings in a non-obvious way, and CoinWrap exposes a subset in
a user-friendly manner.

GLOP results are using GLOP nuget packages in this repository and the C#
language wrapper driven by LPSharp. The locally built library (version 9.1.90.1)
matches the public OR-Tools in nuget.org (version 9.1.9490). This confirms that
we are able to locally replicate the public build. `Glopsolve.exe` is our C++
based standalone executable linked with the C++ libraries. It is used to
generate verbose logs or control the solver in ways not possible with the dotnet
interface.

__LPSharp cheatsheet__

LPSharp can be used to collect benchmark results. It can read MPS model files,
set up the model in the solver, and execute the solver. For Plato models, read
the model in free format. This is not required for WANLPv2 and Netlib models.
See [LPSharp readme](../LPSharp/Readme.md) for more help.
```
LPSharp> read-mps netlib\80bau38.mps              # if WANLPv2 or Netlib
LPSharp> read-mps plato\s250r10.mps -Format free  # if Plato
LPSharp> set-solver -create GLOP -key glop -default
LPSharp> set-solver -key glop -name PrimalSimplex
LPSharp> set-solver -key glop -name TimeLimitInSeconds -value @(900)
LPSharp> invoke-solver s250r10
```

__CoinWrap cheatsheet__

Example execution of Netlib and Plato models in different settings. The choice
of starting basis is baked into the executable: dual uses crash and primal uses
idiot in the code version of 10/27/2021, because they give the best results in
WANLPv2.

```
$ coinwrap.exe netlib\80bau38.mps either
$ coinwrap.exe netlib\80bau38.mps dual
$ coinwrap.exe netlib\80bau38.mps primal
```

```
$ coinwrap.exe plato\s250r10.mps either
$ coinwrap.exe plato\s250r10.mps dual
$ coinwrap.exe plato\s250r10.mps primal
```

__GlopSolve cheatsheet__

All solver-specific settings are baked into the executable. You need to
recompile the program to change them.
```
$ glopsolve.exe -helpfull
$ glopsolve.exe -mpsfile edge-pri0-maxmin0.mps -timelimit 30 -lpalgorithm dual
$ glopsolve.exe ... -timelimit 30 -params_file example_params.txt
```

## WANLPv2 results

Please see [wanlpv2_results.csv](wanlpv2_results.csv) for the optimal values and
solve times. Measurements were done on:

- Neil's laptop Intel Core i7 7500U 2.7Ghz 4 logical processors 16GB.
- Zainab's desktop Xeon 3+Ghz 64GB.

You can plot the results by executing:

```
$ python plot_lpbench.py --help  # for usage
$ python plot_lpbench.py wanlpv2_results.csv --baseline Msf_i7 --measurements ClpDual_i7 ClpPrimal_i7 ClpDualCrash_i7 ClpPrimalIdiot_i7 GlopPrimal_i7 GlopDualPerturb_i7
```

- MSF times are for primal simplex. Its dual simplex exceeded solve time limits.
  MSF primal simplex is the reference for all speedup ratios even if the target
  solver uses dual simplex.
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
- GLOP dual simplex could not solve the max-min models with default settings.
  Dual simplex does not converge due to high degeneracy in the model. Cost
  perturbation changes costs by random values and this helps in convergence. I
  had to enable cost perturbation and change its default thresholds. The
  modified perturbation settings help the max-min models but penalize the other
  models. However, we have kept the modified settings for all models since our
  benchmark criterion is that same settings should be used for all models. The
  custom settings were done using `glopsolve -paramsfile <filename>` with these
  solver-specific settings.
  ```
  perturb_costs_in_dual_simplex: 1
  relative_cost_perturbation: 1e3
  relative_max_cost_perturbation: 1e5
  ```



## Netlib results

Please see [netlib_results.csv](netlib_results.csv) for the solve times. You can
plot the results by executing:

```
$ python plot_lpbench.py netlib_results.csv --measurements GlopPrimal_i7 GlopDualPerturb_i7 GlopDualDefault_i7 ClpEither_i7 ClpDualCrash_i7 ClpPrimal_i7
```

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

Please see [plato_results.csv](plato_results.csv) for the solve times. You can
plot the results by executing:

```
$ python plot_lpbench.py plato_results.csv --measurements GlopPrimal_i7 ClpEither_i7 ClpBest_i7
```

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

