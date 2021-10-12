# Networking-WANLP

WANLP is a repository of LP solvers, CLP, GLOP, and MSF, and LPSharp, an
interactive test bench for these solvers.

## Submodule cheat sheet

This project uses
[submodules](http://git-scm.com/book/en/v2/Git-Tools-Submodules). This means
that some subdirectories in this project are separate git repositories. These
are tips for working with submodules.

__Clone superproject__. First, clone the project.
```
$ git clone https://github.com/ukrishnas/Networking-WANLP.git WANLP
Cloning into 'WANLP2'...
remote: Enumerating objects: 26912, done.
remote: Counting objects: 100% (26912/26912), done.
remote: Compressing objects: 100% (5851/5851), done.
```

__Update submodules__. Use `git submodule update` to update submodules. This
will populate the directories of the submodules. Notice that the URLs do not
point to the public repositories. We have changes in the submodules which cannot
be upstreamed to the public repository. Hence we have changed the URL in
`.gitmodules` to a private URL. In our example, it happens to be the same URL as
the parent project but the submodule repository is still separate.
```
$ cd WANLP
$ git submodule update --init --recursive

Submodule 'CLP/BuildTools' (https://github.com/ukrishnas/Networking-WANLP.git) registered for path 'BuildTools'
Submodule 'CLP/Clp' (https://github.com/ukrishnas/Networking-WANLP.git) registered for path 'Clp'
Submodule 'CLP/CoinUtils' (https://github.com/ukrishnas/Networking-WANLP.git) registered for path 'CoinUtils'
Cloning into 'WANLP/CLP/BuildTools'...
Cloning into 'WANLP/CLP/Clp'...
Cloning into 'WANLP/CLP/CoinUtils'...
Submodule path 'BuildTools': checked out 'a8720e29aa7db47f0127550a2644386d3ef95d5d'
Submodule path 'Clp': checked out '99781af262c5bad38c35fdd36d463c6337d69590'
Submodule path 'CoinUtils': checked out 'e368cc9dc5b95a1326f246c7d3f6b9dcc94eb66a'
```

 __Detached HEAD__. You will be in detached HEAD mode in each submodule. This is
normal. A detached HEAD means there is no local branch, and is fine if you are
going to simply build with the submodule.

```
$ cd CLP\clp
$ git status
HEAD detached at 99781af2
nothing to commit, working tree clean

$ git log -1
commit 99781af262c5bad38c35fdd36d463c6337d69590 (HEAD, origin/wanlp_clp_clp_master)
```

__Checkout__. If you wish to do development in the submodule, you need to
checkout a branch. The naming convention of the origin master branch for each
submodule is `wanlp_<folder>_<folder>_master`, all in lowercase. Checkout a
branch based off the submodule-specific master branch in the remote.

```
$ git checkout wanlp_clp_clp_master
Switched to a new branch 'wanlp_clp_clp_master'
Branch 'wanlp_clp_clp_master' set up to track remote branch 'wanlp_clp_clp_master' from 'origin'.
```

__Reset submodule__. If you need to restart your work on a submodule, you can
de-initialize and initialize the submodule. After de-initialize, the directory
will become empty. After update with the initialize option, the directory will
be reset to the tip in the remote.

```
$ git submodule deinit -f Clp
$ git submodule update --init Clp
```

__Update from public repository__: Let's say we want to pull changes made by the
maintainers of the public repository. We will fetch their changes, rebase our
private changes, and update our private remote for other users. Rebase is better
than merge in this scenario since it gives a linear history. The name `origin`
refers to the private remote, and `public_origin` refers to the public remote.

- Create a branch tracking the private tip of the submodule.
- Add the public repository as a remote and fetch new updates.
- Rebase private changes onto the new tip. Let `public_old_tip` be the old public
  tip on which we put our changes, and `public_new_tip` be the new public tip.
  Rebase will replay our private changes from the old tip onto the new tip.
- Commit and push changes.

```
$ cd Clp
$ git checkout -b clp_update -t origin/wanlp_clp_clp_master
$ git remote add public_origin https://github.com/coin-or/Clp.git
$ git fetch --all
$ git rebase --interactive --onto public_origin/<public_new_tip> public_origin/<public_old_tip> clp_update
$ git commit -a -m "Rebase private changes onto new public tip"
$ git push
```

