# LP Solver Benchmark Results

This page documents the benchmark results of the LP solvers. The solvers have
been tested against the following benchmarks.

- WANLPv2 is a benchmark of 29 models, 26 are performance-oriented and 3 are for
  checking completion with default zero tolerance settings. WANLPv2 benchmark
  models are in
  [sharepoint](https://microsoft.sharepoint.com/:f:/t/AzNet_WAN/Eta127_8eHhNsylPOEupNQwB1OwjjRGGPilJtk2cf0sT_Q?e=cYFuIm)
  and explanation of the models is in Section 6 of [this
  document](https://microsoft.sharepoint.com/:b:/t/AzNet_WAN/Ed82YFQIC5xBg5_2ya0Y_bgB5OTmeu9GGMhEyFH6D7AXFg?e=GkPifF).


## LPSharp cheatsheet

LPSharp can be used to collect benchmark results. It can read MPS model files,
set up the model in the solver, and execute the solver. The steps below read two
models from uncompressed and compressed model files, and invokes CLP, GLOP, and
MSF solvers on a model.

```
LPSharp> read-mps model1.mps -key m1        # reads model 1 from an MPS file
LPSharp> read-mps model2.mps.gz -key m2     # reads model 2 from a gzipped MPS file
LPSharp> invoke-msfsolve -key m1            # invokes MSF solver on model 1
LPSharp> invoke-clpsolve -key m1            # invokes CLP solver on model 1
LPSharp> invoke-glopsolve -key m1           # invokes GLOP solver on model 1
```

## WANLPv2 results

Please see [results_wanlpv2.csv](results_wanlpv2.csv) for the optimal values and solve times.
Measurements were done on:

- Neil's laptop Intel Core i7 7500U 2.7Ghz 4 logical processors 16GB.
- Zainab's desktop Xeon 3+Ghz 64GB.

You can plot the results by executing:

```
$ python plot_lpbench.py --help  # for usage
$ python plot_lpbench.py results_wanlpv2.csv --baseline MSF_i7 --measurements CLPDual_i7 CLPPrimal_i7
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
