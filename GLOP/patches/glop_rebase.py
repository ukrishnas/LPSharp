'''
Custom rebase script for merging private GLOP changes with public code.

For usage, run:

$ python glop_rebase.py -h

Typical execution:

$ python glop_rebase.py <public-tree> <old-private-tree> <new-private-tree>
--apply_patches

<public-tree> is the path to the new public tree, <old-private tree> is the path
to the GLOP folder of the old private tree, and <new-private-tree> is the path
to the copy destination.

'''

import argparse
from fnmatch import fnmatch
import glob
import logging
import os
import re
import shutil
import subprocess

# Module logger
_outputfolder = os.getcwd()
_logger = logging.getLogger(__name__)
_logger.setLevel(logging.DEBUG)
_logfile = 'glop_rebase.log'

def setup_logging(loglevel):
    """Set up logger."""
    numeric_level = getattr(logging, loglevel.upper(), None)
    if not isinstance(numeric_level, int):
        print('Invalid log level: {}'.format(loglevel), end=' ')
        numeric_level = logging.WARNING
        print('Using {}'.format(numeric_level))

    global _logger
    _logger = logging.getLogger(__name__)
    _logger.setLevel(logging.DEBUG)
    
    # Create a handler at debug level that logs to file
    handler1 = logging.FileHandler(os.path.join(_outputfolder, _logfile))
    handler1.setLevel(logging.DEBUG)
    handler1.setFormatter(logging.Formatter('%(asctime)s %(levelname)s %(message)s'))
    _logger.addHandler(handler1)
    
    # Create a handler that logs to stderr
    handler2 = logging.StreamHandler()
    handler2.setLevel(numeric_level)
    handler2.setFormatter(logging.Formatter('%(message)s'))
    _logger.addHandler(handler2)

def check_dir(dirname, expect):
    '''Checks if a directory is present when it should be and raise an exception.'''
    got = os.path.exists(dirname)
    if got != bool(expect):
        if expect:
            _logger.error('Directory {} not present',format(dirname))
            raise FileNotFoundError
        else:
            _logger.error('Directory {} already exists'.format(dirname))
            raise FileExistsError

def public_ortools_ignore(dirname, filenames):
    '''Custom ignore function for copying files from the public ortools repository.'''
    ignore = []
    allow = []

    for filename in filenames:

        # Ignore third-party and non-LP solvers in ortools/linear_solver.
        # And only keep the glop interface files.
        if fnmatch(dirname, '*linear_solver'):
            if fnmatch(filename, 'bop*') or \
                fnmatch(filename, 'gurobi*') or \
                fnmatch(filename, 'sat*') or \
                fnmatch(filename, 'scip*'):
                ignore.append(filename)
                continue
            # This pattern matches glop and clp interface files.
            rx = re.search(r'(.+)_interface', filename)
            if rx != None and rx.group(1) not in ['clp', 'glop']:
                ignore.append(filename)
                continue
        
        # Do not copy lp_data/lp_decomposer files.
        if fnmatch(dirname, '*lp_data'):
            if fnmatch(filename, 'lp_decomposer*'):
                ignore.append(filename);
                continue

        # Allow the following source directories and files.
        if fnmatch(filename, '*cc') or \
            fnmatch(filename, '*.cs') or \
            fnmatch(filename, '*.h') or \
            fnmatch(filename, '*.i') or \
            fnmatch(filename, '*.proto') or \
            fnmatch(filename, 'base') or \
            fnmatch(filename, 'csharp') or \
            fnmatch(filename, 'dotnet') or \
            fnmatch(filename, 'glop') or \
            fnmatch(filename, 'linear_solver') or \
            fnmatch(filename, 'linear_solver_natural_api.py') or \
            fnmatch(filename, 'lp_data') or \
            fnmatch(filename, 'ortools') or \
            fnmatch(filename, 'port') or \
            fnmatch(filename, 'util'):
            allow.append(filename)
            continue

        # Allow these miscellaneous files.
        if fnmatch(filename, 'LICENSE*.txt') or \
            fnmatch(filename, 'Version.txt') or \
            (fnmatch(dirname, 'patches') and fnmatch(filename, '*patch')):
            allow.append(filename)
            continue

        # Ignore the rest.
        ignore.append(filename)
    _logger.info('public_ortools_ignore dirname={} allow={} ignore={}'.format(dirname, allow, ignore))
    return ignore


def private_ortools_ignore(dirname, filenames):
    '''Custom ignore function for copying files from the private ortools repository.'''
    ignore = []
    allow = []

    for filename in filenames:
        # Always exclude the build directory.
        if fnmatch(dirname, 'build*'):
            ignore.append(filename)
            continue

        # Allow CMake build files.
        if fnmatch(filename, 'cmake') or \
            fnmatch(dirname, '*cmake') or \
            fnmatch(filename, '*CMakeLists.txt*') or \
            fnmatch(filename, 'csharp') or \
            fnmatch(filename, 'dotnet') or \
            fnmatch(dirname, '*dotnet*') or \
            fnmatch(filename, 'ortools') or \
            fnmatch(dirname, '*ortools'):
            allow.append(filename)
            continue

        # Allow these miscellaneous files.
        if fnmatch(filename, 'README.md') or \
            fnmatch(filename, 'patches') or \
            fnmatch(dirname, '*patches') or \
            fnmatch(filename, 'examples') or \
            fnmatch(dirname, '*examples*'):
            allow.append(filename)
            continue

        # Ignore the rest.
        ignore.append(filename)
    _logger.info('private_ortools_ignore dirname={} allow={} ignore={}'.format(dirname, allow, ignore))
    return ignore

def apply_private_patches(dirname):
    '''Apply private patches.
    TODO(user): redirect standard output in subprocess call.
    '''

    os.chdir(dirname)

    globpath = os.path.join('.', 'patches', 'glop*.patch')
    patch_files = glob.glob(globpath)
    if patch_files == None or len(patch_files) == 0:
        _logger.warning('No patches found matching {}'.format(globpath))

    for patch_file in patch_files:
        response = input('Apply {}, p to pick, c to check, any other letter to skip: '.format(patch_file))
        response = response.lower()
        if response.startswith('p'): 
            _logger.info('Applying patch file {}'.format(patch_file))
            subprocess.call(['git', 'apply', patch_file])
        elif response.startswith('c'):
            _logger.info('Checking patch file {}'.format(patch_file))
            subprocess.call(['git', 'apply', '--check', patch_file])

if __name__ == '__main__':

    parser = argparse.ArgumentParser()
    parser.add_argument('public_tree', help='The path to the new public tree')
    parser.add_argument('private_tree', help='The path to the current private tree')
    parser.add_argument('new_private', help='The path to the new private tree')
    parser.add_argument('--apply_patches', action='store_true', default=False, help='Apply private patches to new copy')
    parser.add_argument('--loglevel', default='WARN', help='Logging level')

    args = parser.parse_args()
    setup_logging(args.loglevel)
    _logger.info('Starting new copy args={}'.format(args))

    public_tree = args.public_tree
    private_tree = args.private_tree
    new_private = args.new_private
    check_dir(public_tree, expect=True)
    check_dir(private_tree, expect=True)

    if os.path.exists(new_private):
        _logger.info('Deleting existing {}'.format(new_private))
        response = input('Deleting existing {}. Press [yY] to continue: '.format(new_private))
        if response.lower().startswith('y'):
            shutil.rmtree(new_private)

    _logger.info('Copying private tree files to destination, private={} destination={}'.format(private_tree, new_private))
    shutil.copytree(private_tree, new_private, ignore=private_ortools_ignore, dirs_exist_ok=True)
    
    _logger.info('Copying public tree files to destination, public={} destination={}'.format(public_tree, new_private))
    shutil.copytree(public_tree, new_private, ignore=public_ortools_ignore, dirs_exist_ok=True)

    if args.apply_patches:
        _logger.info('Applying private patches')
        apply_private_patches(new_private)
