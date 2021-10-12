import matplotlib.pyplot as plt
import numpy as np
import statistics

# WANLPv2 benchmark results. Measurements were collected on Neil's laptop Intel
# Core i7 7500U 2.7Ghz 4 logical processors 16GB.
#
# CLP results are using Clp.exe built from CLP code in this repository using two
# solve methods: dual simplex and primal simplex. Reported times are iterations
# time + presolve time. MSF results are total times from Invoke-MSFSolve using
# Network Designer. Note that these results are not from LPSharp.
#
# Each data tuple is model name, CLP dualSimplex, primalSimplex, MSF primal.
data = [
('edge-pri0-diverse5.mps', 2382 + 420, 10492 + 440, 61608),
('edge-pri0-maxmin0.mps', 4612 + 380, 3652 + 380, 27002),
('edge-pri0-maxmin1.mps', 5382 + 430, 6192 + 430, 52156),
('edge-pri0-maxmin2.mps', 6452 + 420, 8192 + 410, 54399),
('edge-pri0-maxmin3.mps', 7782 + 410, 8262 + 400, 54138),
('edge-pri0-maxmin4.mps', 6212 + 450, 7272 + 420, 53300),
('edge-pri0-mincost.mps', 1752 + 400, 10592 + 390, 45836),
('edge-pri0-minutil.mps', 2292 + 820, 4652 + 820, 41737),
('sliceperf-pri0-diverse5.mps', 1842 + 510, 4722 + 50, 42852),
('sliceperf-pri0-maxmin0.mps', 2782 + 440, 1462 + 470, 20690),
('sliceperf-pri0-maxmin1.mps', 3262 + 470, 5562 + 490, 40737),
('sliceperf-pri0-maxmin2.mps', 4172 + 460, 9812 + 470, 42158),
('sliceperf-pri0-maxmin3.mps', 4392 + 480, 15682 + 480, 41377),
('sliceperf-pri0-maxmin4.mps', 3692 + 470, 14672 + 470, 39991),
('sliceperf-pri0-maxmin5.mps', 3982 + 460, 17072 + 470, 40162),
('sliceperf-pri0-mincost.mps', 1632 + 440, 5012 + 440, 36582),
('sliceperf-pri3-minutil.mps', 312 + 210, 322 + 210, 3252),
('sonal-diverse1.mps', 542 + 240, 772 + 240, 9931),
('sonal-diverse5.mps', 532 + 250, 742 + 240, 9630),
('sonal-maxmin0.mps', 762 + 280, 602 + 260, 4699),
('sonal-maxmin1.mps', 6572 + 280, 762 + 270, 9908),
('sonal-maxmin2.mps', 15202 + 290, 902 + 280, 10190),
('sonal-maxmin3.mps', 72272 + 280, 1492 + 280, 10047),
('sonal-maxmin4.mps', 38832 + 280, 1222 + 270, 10262),
('sonal-maxmin5.mps', 41112 + 280, 1362 + 280, 9966),
('sonal-mincost.mps', 482 + 220, 732 + 230, 8908),
('wander-minutil1.mps', 9482 + 60, 3642 + 60, 60000),
('wander-primal1.mps', 20, 20, 11),
('wander-primal2.mps', 20, 20, 3),
]

# Strip the wander results since they are for completion, not performance.
data = data[:-3]

def process_results(data, baseidx, measureidx):
    """Returns data for a plot showing performance of measurement vs baseline."""
    n = len(data)
    baselines = np.array([data[i][baseidx] for i in range(n)])
    measurements = np.array([data[i][measureidx] for i in range(n)])

    speedups = np.zeros(n)
    for i in range(n):
        base = baselines[i]
        measure = measurements[i]
        ratio = base / measure
        if ratio < 1:
            ratio = -1/ratio
        speedups[i] = ratio

    x = np.arange(n)
    xlabels = np.array([data[i][0] for i in range(n)])
    return x, xlabels, speedups

def compute_means(data):
    """Computes means of different combinations."""
    m = len(data[0])
    n = len(data)
    x = []
    y = []

    for comparison in [(3, 1, 'MSF/CLPDual'), (3, 2, 'MSF/CLPPrimal')]:
        idx1 = comparison[0]
        idx2 = comparison[1]
        x.append(comparison[2])
        series1 = np.array([data[i][idx1] for i in range(n)])
        series2 = np.array([data[i][idx2] for i in range(n)])
        ratios = series1 / series2
        mean = statistics.harmonic_mean(ratios)
        y.append(mean)
    return x, y

def autolabel(rects, texts, offset, color):
    """Labels the bars with text."""
    if  len(rects) != len(texts):
        return
    n = len(rects)
    for i in range(n):
        rect = rects[i]
        height = rect.get_height()
        ax.annotate('{}'.format(texts[i]), color=color, 
                    xy=(rect.get_x() + rect.get_width() / 2, height),
                    xytext=(0, offset),  # 3 points vertical offset
                    textcoords="offset points",
                    ha='center', va='bottom')

# plt.rcParams['font.family'] = 'cmr10'
plt.rcParams['font.size'] = fontsize = 11
plt.rcParams['savefig.bbox'] = 'tight'
plt.rcParams['savefig.pad_inches'] = 0.05
plt.rcParams['savefig.dpi'] = 300

fig, axes = plt.subplots(nrows=2, ncols=1)
width = 0.8

ax = axes[0]
x, xlabels, y = process_results(data, 3, 1)
ax.plot(x, y, marker='s', ms=4, markevery=1, ls='-', label='MSF/CLPDual')
_, _, y = process_results(data, 3, 2)
ax.plot(x, y, marker='o', ms=4, markevery=1, ls='-', label='MSF/CLPPrimal')
ax.set_xticks(x)
ax.set_xticklabels(xlabels, rotation=75)
ax.set_yticks([-10, -5, -1, 1, 5, 10, 15, 20])
ax.axhline(1, color='tab:gray')
ax.axhline(-1, color='tab:gray')
ax.add_patch(plt.Rectangle((-1, -1), len(x)+1, 2, edgecolor='tab:gray', fill=False, hatch='///'))
# ax.set_xlabel('Model')
ax.set_ylabel('Speedup or slowdown')
ax.annotate('Speedup', xy=(0,2.5), va='top')
ax.annotate('Slowdown', xy=(0,-3), va='bottom')
ax.legend()

ax = axes[1]
x, y = compute_means(data)
rects = ax.bar(x, y, width, fill=True, color='tab:cyan')
autolabel(rects, np.around(y,1), 1, 'black')
ax.axhline(1, color='tab:gray')
ax.set_ylabel('Mean speedup')

fig.set_tight_layout(True)
fig.set_size_inches(10.0, 8.0)
# plt.savefig('results.pdf', format='pdf', bbox_inches='tight')
plt.show()