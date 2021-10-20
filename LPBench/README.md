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

## LPSharp cheatsheet

LPSharp can be used to collect benchmark results. It can read MPS model files,
set up the model in the solver, and execute the solver.

Read a model. The model can be accessed using a key which is the filename
without extension or the name given using `-key`. Use the `-format free` option
to read files in MPS free format. The default fixed format is faster.

```
LPSharp> read-mps 80bau38.mps
```

Create a solver with the `-create` option. The argument is the solver type
(supported values are `GLOP` with future support for `CLP` and `MSF`). The
solver can be accessed using the key defined by `-key`. The `-default` option
means this will be the solver used in future `invoke-solver` commands if no key
is given. 
```
LPSharp> set-solver -create GLOP -key glop -default
```

Load the model into the solver and solve it. The first argument is the model
key. You can select a non-default solver using `-key`. 

```
LPSharp> invoke-solver 80bau38
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

## WANLPv2 results

Please see [wanlpv2_results.csv](wanlpv2_results.csv) for the optimal values and solve times.
Measurements were done on:

- Neil's laptop Intel Core i7 7500U 2.7Ghz 4 logical processors 16GB.
- Zainab's desktop Xeon 3+Ghz 64GB.

You can plot the results by executing:

```
$ python plot_lpbench.py --help  # for usage
$ python plot_lpbench.py wanlpv2_results.csv --baseline MSF_i7 --measurements CLPDual_i7 CLPPrimal_i7
```

CLP results are using Clp.exe built from CLP code in this repository using two
solve methods: dual simplex and primal simplex. Reported times are iterations
time + presolve time. MSF results are total times from Invoke-MSFSolve using
Network Designer. Note that these results are not from LPSharp.

CLP has a dual Simplex, primal Simplex, idiot solve methods. In terms of
performance idiot is not fast, dual simplex is very fast but took a while with
sonal-* mps models. Primal simplex solved sonal-* models quickly but not the
edge and sliceperf ones (primal becomes infeasible and it needs to readjust and
solve).

GLOP results are using GLOP code in this repository and the C# language wrapper
driven by LPSharp. The locally built library (version 9.1.55.1) match the public
OR-Tools in nuget.org (version 9.1.9490). This confirms that we are able to
locally replicate the public build.

GLOP is 3 times and CLP is 5 times faster than MSF. Both CLP and GLOP solve all
29 models.

## Netlib results

Please see [netlib_results.csv](netlib_results.csv) for the solve times. You can
plot the results by executing:

```
$ python plot_lpbench.py netlib_results.csv --measurements Glop_i7 ClpEither_i7
```

These models execute in less than 30 seconds per model.

If using the `Clp.exe` executable, it will complain `Unknown image  at line 1 of
file`. Please open the file and remove the first four lines before the NAME
record. The line starting with NAME should be the first line. The `LPSharp` MPS
reader can read this file without issue.

GLOP results were collected using LPSharp. CLP results were collected using the
CLP executable built from this repository.

```
$ Clp.exe 80bau38.mps -either
```

CLP is 2 times faster than GLOP for the executed Netlib models.

## Plato results

Please see [plato_results.csv](plato_results.csv) for the solve times. You can
plot the results by executing:

```
$ python plot_lpbench.py plato_results.csv --measurements Glop_i7 ClpEither_i7
```

The models in this ftp site are bz2 compressed. Please decompress them before
executing them with LPSharp. Please read the files into LPSharp using free
format and verify that there are no read errors.

```
LPSharp> read-mps plato\datt256_lp.mps -Format free
Read MPS file plato\datt256_lp.mps ... read_errors=0 ...
```

GLOP results were collected using LPSharp. CLP results were collected using the
CLP executable built from this repository. `Clp.exe` binary has an `-either`
option and it is the best way to run the models. The problems are quite hard to
solve and take minutes on my laptop. Please check with the logs at the ftp site
to get a sense of how the solver behaves with the model.

CLP is 2 times faster than GLOP for the executed Plato models after excluding
two outliers.