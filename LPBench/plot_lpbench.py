import argparse
import csv
import math
import matplotlib.pyplot as plt
import numpy as np
import statistics


def load_data(filename):
    """Loads results from a CSV file."""
    data = {}
    with open(filename,'r') as csvfile:
        reader = csv.DictReader(csvfile, lineterminator='\n')
        for row in reader:
            if 'Model' not in row or row['Model'].startswith('#'):
                continue
            for key, value in row.items():
                key = key.strip()
                if key != 'Model':
                    try:
                        value = float(value)
                    except (TypeError, ValueError):
                        value = math.inf
                if key not in data:
                    data[key] = []
                data[key].append(value)
        return data


def process_results(data, basekey, measurekey):
    """Returns data for a plot showing performance of measurement vs baseline."""
    baselines = np.array(data[basekey])
    measurements = np.array(data[measurekey])
    if len(baselines) != len(measurements):
        raise UserWarning('Unequal length of {} and {} arrays, {} != {}'.format(basekey, measurekey, len(basekey), len(measurekey)))
    n = len(baselines)
    speedups = np.zeros(n)
    for i in range(n):
        base = baselines[i]
        measure = measurements[i]
        ratio = base / measure
        if ratio < 1:
            ratio = -1/ratio
        speedups[i] = ratio

    x = np.arange(n)
    xlabels = np.array(data['Model'])
    label = 'time({}) / time({})'.format(basekey, measurekey)
    return x, xlabels, speedups, label


def compute_means(data, basekey, measurekeys):
    """Returns data for a plot showing harmonic means of measurements vs baseline."""
    x, y = [], []

    for key in measurekeys:
        x.append(key)

        series1 = np.array(data[basekey])
        series2 = np.array(data[key])
        ratios = series1 / series2
        mean = statistics.harmonic_mean(ratios)
        y.append(mean)

    title = "Harmonic mean of solve time ratios, baseline {}".format(basekey)

    return x, y, title


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


if __name__ == '__main__':

    parser = argparse.ArgumentParser()
    parser.add_argument('results_file', help='Results CSV file')
    parser.add_argument('--baseline', '-b', type=str, help='Baseline column name')
    parser.add_argument('--measurements', '-m', type=str, nargs='+', help='Measurement column names')

    args = parser.parse_args()
    results_file = args.results_file
    basekey = args.baseline
    measurekeys = args.measurements

    # If no baseline is given, then make the first measurement index the baseline.
    if basekey == None and measurekeys != None and len(measurekeys) > 0:
        basekey = measurekeys[0]
        measurekeys = measurekeys[1:]
    if measurekeys == None or len(measurekeys) == 0:
        raise UserWarning('No measurement columns to plot')

    # Load the data file. Check that the baseline and measurement column names
    # are present in the data.
    data = load_data(results_file)
    if len(data) == 0:
        raise UserWarning('No data in {}'.format(results_file))
    if basekey not in data:
        raise UserWarning('Baseline column {} not found in results file {}'.format(basekey, results_file))
    for key in measurekeys:
        if key not in data:
            raise UserWarning('Measurement column {} not found in results file {}'.format(key, results_file))

    # plt.rcParams['font.family'] = 'cmr10'
    plt.rcParams['font.size'] = fontsize = 11
    plt.rcParams['savefig.bbox'] = 'tight'
    plt.rcParams['savefig.pad_inches'] = 0.05
    plt.rcParams['savefig.dpi'] = 300

    fig, axes = plt.subplots(nrows=2, ncols=1)
    width = 0.8

    ax = axes[0]
    markers = ['s', 'o', '^', 'v']
    i = 0
    for key in measurekeys:
        x, xlabels, y, label = process_results(data, basekey, key)
        marker = markers[i % len(markers)]
        i += 1
        ax.plot(x, y, marker=marker, ms=4, markevery=1, ls='-', label=label)
    ax.set_xticks(x)
    ax.set_xticklabels(xlabels, rotation=75)
    ax.axhline(1, color='tab:gray')
    ax.axhline(-1, color='tab:gray')
    ax.add_patch(plt.Rectangle((-1, -1), len(x)+1, 2, edgecolor='tab:gray', fill=False, hatch='///'))
    ax.set_title('Ratio of solve times')
    ax.annotate('Speedup', xy=(0,2.5), va='top')
    ax.annotate('Slowdown', xy=(0,-3), va='bottom')
    ax.legend()

    ax = axes[1]
    x, y, title = compute_means(data, basekey, measurekeys)
    rects = ax.bar(x, y, width, fill=True, color='tab:cyan')
    autolabel(rects, np.around(y,1), 1, 'black')
    ax.axhline(1, color='tab:gray')
    ax.set_title(title)

    fig.set_tight_layout(True)
    fig.set_size_inches(10.0, 8.0)
    # plt.savefig('lpbench.pdf', format='pdf', bbox_inches='tight')
    plt.show()