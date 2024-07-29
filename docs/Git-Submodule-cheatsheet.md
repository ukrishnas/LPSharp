
This project uses
[submodules](http://git-scm.com/book/en/v2/Git-Tools-Submodules) for some code.
This means that some subdirectories in this project are separate git
repositories. These are tips for working with submodules.

## Clone superproject

First, clone the project.
```
$ git clone https://github.com/ukrishnas/LPSharp.git LPSharp
Cloning into 'LPSharp'...
remote: Enumerating objects: 26912, done.
remote: Counting objects: 100% (26912/26912), done.
remote: Compressing objects: 100% (5851/5851), done.
```

## Update submodules

Use `git submodule update` to update submodules. This
will populate the directories of the submodules. Notice that the URLs do not
point to the public repositories. We have changes in the submodules which cannot
be upstreamed to the public repository. Hence we have changed the URL in
`.gitmodules` to a private URL. In our example, it happens to be the same URL as
the parent project but the submodule repository is still separate.
```
$ cd LPSharp
$ git submodule update --init --recursive

Submodule 'CLP/BuildTools' (https://github.com/ukrishnas/LPSharp.git) registered for path 'CLP/BuildTools'
Submodule 'CLP/Clp' (https://github.com/ukrishnas/LPSharp.git) registered for path 'CLP/Clp'
Submodule 'CLP/CoinUtils' (https://github.com/ukrishnas/LPSharp.git) registered for path 'CLP/CoinUtils'
Cloning into 'LPSharp/CLP/BuildTools'...
Cloning into 'LPSharp/CLP/Clp'...
Cloning into 'LPSharp/CLP/CoinUtils'...
Submodule path 'CLP/BuildTools': checked out 'd9fce5b3492a433bd4e39a79d7a54baae2cd2b32'
Submodule path 'CLP/Clp': checked out 'b1b47cac36718f864987343a0de55ce86473a5eb'
Submodule path 'CLP/CoinUtils': checked out '099eb15d9fda5dafc49dee62b634f5ce49c028c3'
```

## Detached HEAD

You will be in detached HEAD mode in each submodule. This is normal. A detached
HEAD means there is no local branch, and is fine if you are going to simply
build with the submodule.

```
$ cd CLP/clp
$ git status
HEAD detached at 99781af2
nothing to commit, working tree clean

$ git log -1
commit b1b47cac36718f864987343a0de55ce86473a5eb (HEAD, origin/lpsharp_clp_clp_master)
```

## Checkout branch

If you wish to do development in the submodule, you need to checkout a branch.
The naming convention of the origin master branch for each submodule is
`lpsharp_<folder>_<folder>_master`, all in lowercase. Checkout a branch based
off the submodule-specific master branch in the remote.

```
$ git checkout -b clp_dev -t origin/lpsharp_clp_clp_master
Switched to a new branch 'clp_dev'
Branch 'clp_dev' set up to track remote branch 'lpsharp_clp_clp_master' from 'origin'.
```

## Add submodule and change remote

Adding the submodule is done by the submodule add command. Since we will be
making changes in the submodule that need to be shared before they are
upstreamed, change the remote to this repository. Note that if you are not going
to edit anything in the code you are importing, then do not use submodules, but
instead use CMake FetchContent to pull and build the code. Finally, change the
name of the origin master branch. The naming convention of the origin master
branch for a submodule is `lpsharp_<folder>_<folder>_master`, all in lowercase.
The combined steps are shown below.

```
$ git submodule add https://github.com/coin-or-tools/BuildTools.git
$ git remote remove origin
$ git remote add origin https://github.com/ukrishnas/LPSharp.git
$ git branch -m lpsharp_clp_buildtools_master
$ <Perform any edits to submodule>
$ git commit
$ git push --set-upstream origin lpsharp_clp_buildtools_master
```

Edit `.gitmodule` URL parameter of the newly added submodule from the public to
the private repository.

```
[submodule "CLP/BuildTools"]
	path = CLP/BuildTools
	url = https://github.com/ukrishnas/LPSharp.git
```

## Update from public repository

Let's say we want to pull changes made by the maintainers of the public
repository. We will fetch their changes, rebase our private changes, and update
our private remote for other users. Rebase is better than merge in this scenario
since it gives a linear history. The name `origin` refers to the private remote,
and `public_origin` refers to the public remote.

- Create a branch tracking the private tip of the submodule.
- Add the public repository as a remote and fetch new updates.
- Rebase private changes onto the new tip. Let `public_old_tip` be the old public
  tip on which we put our changes, and `public_new_tip` be the new public tip.
  Rebase will replay our private changes from the old tip onto the new tip.
- Commit and push changes.

```
$ cd Clp
$ git checkout -b clp_update -t origin/lpsharp_clp_clp_master
$ git remote add public_origin https://github.com/coin-or/Clp.git
$ git fetch --all
$ git rebase --interactive --onto public_origin/<public_new_tip> public_origin/<public_old_tip> clp_update
$ git commit -a -m "Rebase private changes onto new public tip"
$ git push
```

## Reset submodule

If you need to restart your work on a submodule, you can de-initialize and
initialize the submodule. After de-initialize, the directory will become empty.
After update with the initialize option, the directory will be reset to the tip
in the remote.

```
$ git submodule deinit -f Clp
$ git submodule update --init Clp
```