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
models from uncompressed and compressed model files, and invokes CLP and GLOP
solvers on them.

```
LPSharp> read-mps model1.mps -key m1         # read model 1 as m1 from an MPS file
LPSharp> read-mps model2.mps.gz -key m2      # read model 2 as m2 from a gzipped MPS file

LPSharp> set-solver GLOP -default            # create GLOP solver and make it default
LPSharp> invoke-solver -model m1             # Invoke GLOP solver on model m1

LPSharp> set-solver CLP -key clp             # create CLP solver and name it clp
LPSharp> invoke-solver -solver clp -model m2 # invoke clp solver on model m2
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

GLOP results are using GLOP code in this repository, and using the C# language
wrapper driven by LPSharp. The locally built library (version 9.1.55.1) match
the public OR-Tools in nuget.org (version 9.1.9490). This confirms that we are
able to locally replicate the public build. Both versions give an error for
`edge-pri0-maxmin[1-5]` models that the model does not have an optimal solution.
Need to confirm if the bug is in LPSharp MPS reader.